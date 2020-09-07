using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Net.Http;
using customerportalapi.Services.Exceptions;
using PasswordGenerator;
using customerportalapi.Entities.enums;
using Microsoft.Extensions.Configuration;

namespace customerportalapi.Services
{
    public class LoginService : ILoginService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IConfiguration _config;

        public LoginService(IIdentityRepository identityRepository, IUserRepository userRepository, IEmailTemplateRepository emailTemplateRepository, IMailRepository mailRepository, IConfiguration config)
        {
            _identityRepository = identityRepository;
            _userRepository = userRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _mailRepository = mailRepository;
            _config = config;
        }

        public async Task<Token> GetToken(Login credentials)
        {
            User loginUser = null;
            loginUser = _userRepository.GetCurrentUser(credentials.Username);
            
            if (loginUser.Id != null)
            {
                Token token = null;

                try
                {
                    if (DateTime.Now.ToUniversalTime().AddMinutes(Int32.Parse(_config["LoginUnblockedTime"]) * -1) > loginUser.LastLoginAttempts)
                    {
                        token = await _identityRepository.Authorize(credentials);

                        //Initialize login attempts
                        loginUser.LoginAttempts = 0;
                        loginUser.LastLoginAttempts = DateTime.Now.ToUniversalTime();
                        _userRepository.Update(loginUser);
                    }
                    else if (loginUser.LoginAttempts < Int32.Parse(_config["LoginMaxAttempts"]))
                    {
                        token = await _identityRepository.Authorize(credentials);

                        //Initialize login attempts
                        loginUser.LoginAttempts = 0;
                        loginUser.LastLoginAttempts = DateTime.Now.ToUniversalTime();
                        _userRepository.Update(loginUser);
                    }
                }
                catch
                {
                    //Accumulate invalid attempt
                    loginUser.LoginAttempts = loginUser.LoginAttempts + 1;
                    loginUser.LastLoginAttempts = DateTime.Now.ToUniversalTime();
                    _userRepository.Update(loginUser);

                    throw new ServiceException("Login Failed", HttpStatusCode.Unauthorized, "LoginAttempts", $"Attempts: {loginUser.LoginAttempts} , last attempt:{loginUser.LastLoginAttempts.ToLocalTime()}");
                }

                if (token == null)
                    throw new ServiceException("Login Failed", HttpStatusCode.Unauthorized, "LoginAttempts", $"Attempts: {loginUser.LoginAttempts} , last attempt:{loginUser.LastLoginAttempts.ToLocalTime()}");
                else
                    return token;
            }
            else
                throw new ServiceException("Login Failed", HttpStatusCode.Unauthorized);
        }

        public async Task<Token> ChangePassword(ResetPassword credentials)
        {
            try {
                //1. Get User From backend
                User currentUser = _userRepository.GetCurrentUser(credentials.Username);

                //2. Validate Old Password is valid
                Token validateOld = await _identityRepository.Authorize(new Login()
                {
                    Username = credentials.Username,
                    Password = credentials.OldPassword
                });

                //3. Update user
                UserIdentity user = await _identityRepository.GetUser(currentUser.ExternalId);
                if (user != null)
                {
                    user.Password = credentials.NewPassword;
                    user = await _identityRepository.UpdateUser(user);
                }

                //4. Get new Token
                Token newToken = await _identityRepository.Authorize(new Login()
                {
                    Username = credentials.Username,
                    Password = credentials.NewPassword
                });

                return newToken;

            } catch(HttpRequestException ex) {
                throw new ServiceException("Password not valid", HttpStatusCode.BadRequest);

            }
        }

        public async Task<bool> SendNewCredentialsAsync(string userName)
        {
            bool result = false;

            //1. Get user from bbdd
            User user = _userRepository.GetCurrentUser(userName);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "UserName", "Not exist");

            //2. If emailverified is false, first invitation was not accepted yet
            if (!user.Emailverified)
                throw new ServiceException("User must accept invitation before use forgot password function", HttpStatusCode.NotFound, "User", "Invitation not accepted yet");

            var pwd = new Password(true, true, true, false, 6);
            var password = pwd.Next();

            //3. Update invitation data
            user.Password = password;
            user.ForgotPasswordtoken = Guid.NewGuid().ToString();
            _userRepository.Update(user);
            
            //5. Get Email ForgotPassword Template
            EmailTemplate forgotPasswordTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.ForgotPassword, user.Language);
            if (forgotPasswordTemplate._id == null)
            {
                forgotPasswordTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.ForgotPassword, LanguageTypes.en.ToString());
            }

            if (forgotPasswordTemplate._id != null)
            {
                //6. Send forgot password invitation
                Email message = new Email();
                message.To.Add(user.Email);
                message.Subject = forgotPasswordTemplate.subject;
                string htmlbody = forgotPasswordTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                message.Body = string.Format(htmlbody, user.Name, user.Password,
                    $"{_config["ResetPassword"]}{user.ForgotPasswordtoken}");
                result = await _mailRepository.Send(message);
            }

            return result;
        }
    }
}