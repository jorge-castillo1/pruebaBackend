using customerportalapi.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IIdentityRepository
    {
        Task<Token> Authorize(Login credentials);
        Task<TokenStatus> Validate(string token);
        JwtSecurityToken ValidateAzuereAD(string token);

        Task<UserIdentity> AddUser(UserIdentity userIdentity);
        Task<UserIdentity> UpdateUser(UserIdentity userIdentity);
        Task<UserIdentity> GetUser(string userId);
        Task DeleteUser(string userId);

        Task<UserIdentity> AddUserToGroup(UserIdentity userIdentity, Group group);
        Task<bool> RemoveUserFromGroup(UserIdentity userIdentity, Group group);

        Task<GroupResults> FindGroup(string groupName);

        Task<Token> RefreshToken(string token);
        Task<bool> Logout(string token);

    }
}
