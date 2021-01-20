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
            // Find by User by Username or Email
            if (credentials.Username != null)
            {
                loginUser = _userRepository.GetCurrentUserByUsername(credentials.Username);
            } else if (credentials.Email != null && loginUser == null)
            {
                loginUser = _userRepository.GetCurrentUserByEmail(credentials.Email);
            }

            if (loginUser.Id != null)
            {
                Token token = null;
                //Set username with DB info
                credentials.Username = loginUser.Username;

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
                User currentUser = null;

                //1.1 Fidn User by Username or Email
                if (credentials.Username != null)
                {
                    currentUser = _userRepository.GetCurrentUserByUsername(credentials.Username);
                }
                else if (credentials.Email != null && currentUser == null)
                {
                    currentUser = _userRepository.GetCurrentUserByEmail(credentials.Email);
                }

                if (currentUser.Id != null)
                {
                    credentials.Username = currentUser.Username;

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

                }
                else
                {
                    throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "UserName or Email", "Not exist");
                }

            } catch(HttpRequestException ex) {
                throw new ServiceException("Password not valid", HttpStatusCode.BadRequest);

            }

        }

        public async Task<bool> SendNewCredentialsAsync(Login credentials)
        {
            bool result = false;

            //1. Get user from bbdd
            User user = null;
            
            //1.1 Find by Username or Email
            if (credentials.Username != null)
            {
                user = _userRepository.GetCurrentUserByUsername(credentials.Username);
            }
            else if (credentials.Email != null && user == null)
            {
                user = _userRepository.GetCurrentUserByEmail(credentials.Email);
            }

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.UserOrEmail, ValidationMessages.NotExist);

            //2. If emailverified is false, first invitation was not accepted yet
            if (!user.Emailverified)
                throw new ServiceException("User must accept invitation before use forgot password function", HttpStatusCode.NotFound, FieldNames.User, ValidationMessages.InvitationNotAccepted) ;

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
                message.Body = string.Format(htmlbody, user.Name, user.Username, user.Password,
                    $"{_config["ResetPassword"]}{user.ForgotPasswordtoken}");
                result = await _mailRepository.Send(message);
            }

            return result;
        }
    }
}