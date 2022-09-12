using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollectionWrapper<User> _users;

        public UserRepository(IConfiguration config, IMongoCollectionWrapper<User> users)
        {
            _users = users;
        }

        public User GetCurrentUserByUsername(string username)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.Username == username);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }

        public User GetCurrentUserByEmail(string email)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.Email == email);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }

        public User GetCurrentUserByDniAndType(string dni, int userType)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.Dni == dni && t.Usertype == userType);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }

        public User GetCurrentUserByEmailAndDniAndType(string email, string dni, int userType)
        {
            return _users.FindOne(t => t.Email == email && t.Dni == dni && t.Usertype == userType).LastOrDefault();
        }

        public User Update(User user)
        {
            //update User
            var filter = Builders<User>.Filter.Eq(s => s.Username, user.Username);
            var result = _users.ReplaceOne(filter, user);

            return user;
        }

        public User UpdateById(User user)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
            var result = _users.ReplaceOne(filter, user);

            return user;
        }

        public Task<bool> Create(User user)
        {
            //update User
            _users.InsertOne(user);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(User user)
        {
            //update User
            var filter = Builders<User>.Filter.Eq("username", user.Username);
            _users.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public User GetUserByInvitationToken(string invitationToken)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.Invitationtoken == invitationToken);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }

        public User GetUserByForgotPasswordToken(string forgotPasswordToken)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.ForgotPasswordtoken == forgotPasswordToken);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }

        public Task<bool> TrimUserData()
        {
            var filters = Builders<User>.Filter.Empty;

            var usersInfo = _users.Find(filters, 1, 0);
            foreach (var u in usersInfo)
            {
                var name = "";
                try
                {

                    if (u.Username is string)
                    {
                        if (u.Username != null && ((string)u.Username).EndsWith(" ") ||
                            u.Dni != null && u.Dni.EndsWith(" "))
                            name = u.Username?.ToString().Trim();
                        else
                        {
                            continue;
                        }
                    }
                    /*else
                    {

                        IDictionary<string, object> dict = u.Username;

                        foreach (KeyValuePair<string, object> kvp in dict.ToList())
                        {
                            IDictionary<string, object> value = (System.Dynamic.ExpandoObject)kvp.Value;

                            foreach (KeyValuePair<string, object> v in value)
                            {
                                name = (string)v.Value;
                            }
                        }
                    }*/

                    var user1 = new User()
                    {
                        Id = u.Id,
                        Dni = u.Dni?.Trim(),
                        Username = name?.Trim(),
                        Email = u.Email?.Trim(),
                        Name = u.Name,
                        Password = u.Password,
                        Phone = u.Phone,
                        Language = u.Language,
                        Profilepicture = u.Profilepicture,
                        Usertype = u.Usertype,
                        Emailverified = u.Emailverified,
                        Invitationtoken = u.Invitationtoken,
                        ExternalId = u.ExternalId,
                        ForgotPasswordtoken = u.ForgotPasswordtoken,
                        LoginAttempts = u.LoginAttempts,
                        LastLoginAttempts = u.LastLoginAttempts,
                        AccessCodeAttempts = u.AccessCodeAttempts,
                        LastAccessCodeAttempts = u.LastAccessCodeAttempts,
                        LastEmailSent = u.LastEmailSent
                    };

                    var getUserInfo = _users.FindOne(user => user.Id == u.Id);
                    if (getUserInfo == null || !getUserInfo.Any())
                    {
                        //Create(user1);
                    }
                    else
                    {
                        var dese = JsonConvert.SerializeObject(user1);
                        Console.WriteLine(dese);

                        try
                        {
                            UpdateById(user1);
                        }
                        catch (Exception e)
                        {
                            //var resultDelete = Delete(user1).Result;
                            //var resultCreate = Create(user1).Result;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return Task.FromResult(true);
        }
    }
}
