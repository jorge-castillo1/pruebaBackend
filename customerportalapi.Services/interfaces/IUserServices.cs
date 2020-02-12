﻿using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IUserServices
    {
        Task<Profile> GetProfileAsync(string dni);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<bool> InviteUserAsync(Invitation value);

        Task<Token> ConfirmUserAsync(string invitationToken);

        Task<bool> UnInviteUserAsync(string dni);

        Task<Account> GetAccountAsync(string dni);

        Task<Account> UpdateAccountAsync(Account account);

        Task<bool> ContactAsync(FormContact value);
    }
}
