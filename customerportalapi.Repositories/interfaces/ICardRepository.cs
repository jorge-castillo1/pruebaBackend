using customerportalapi.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Repositories.Interfaces
{
    public interface ICardRepository
    {
        Card Get(string username, string smContractCode);
        Card GetCurrent(string username, string smContractCode);
        Card GetByExternalId(string externalId);
        Card Update(Card card);
        Task<bool> Create(Card card);
        Task<bool> Delete(Card card);
        List<Card> Find(CardSearchFilter filter);

    }
}
