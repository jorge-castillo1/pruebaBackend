using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;

namespace customerportalapi.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly IMongoCollectionWrapper<Card> _cards;

        public CardRepository(IConfiguration config, IMongoCollectionWrapper<Card> cards)
        {
            _cards = cards;
        }

        public Card Get(string username, string smContractCode)
        {
            Card card = new Card();

            var cardsInfo = _cards.FindOne(t => t.Username == username && t.SmContractCode == smContractCode);
            foreach (var c in cardsInfo)
            {
                card = c;
            }
            return card;
        }
        public Card GetCurrent(string username, string smContractCode)
        {
            Card card = new Card();

            var cardsInfo = _cards.FindOne(t => t.Username == username && t.SmContractCode == smContractCode && t.Current == true);
            foreach (var c in cardsInfo)
            {
                card = c;
            }
            return card;
        }
        public Card GetByExternalId(string externalId)
        {
            Card card = new Card();

            var cardsInfo = _cards.FindOne(t => t.ExternalId == externalId);
            foreach (var c in cardsInfo)
            {
                card = c;
            }
            return card;
        }

        public Card Update(Card card)
        {
            //update Card
            var filter = Builders<Card>.Filter.Eq(s => s.ExternalId, card.ExternalId);
            var result = _cards.ReplaceOne(filter, card);

            return card;
        }

        public Task<bool> Create(Card card)
        {
            //create Card
            _cards.InsertOne(card);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(Card card)
        {
            //update Card
            var filter = Builders<Card>.Filter.Eq("_id", card.Id);
            _cards.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public List<Card> Find(CardSearchFilter filter)
        {
            FilterDefinition<Card> filters = Builders<Card>.Filter.Empty;

            if (!string.IsNullOrEmpty(filter.Username))
                filters = filters & Builders<Card>.Filter.Eq(x => x.Username, filter.Username);

            if (!string.IsNullOrEmpty(filter.ExternalId))
                filters = filters & Builders<Card>.Filter.Eq(x => x.ExternalId, filter.ExternalId);

            if (!string.IsNullOrEmpty(filter.SmContractCode))
                filters = filters & Builders<Card>.Filter.Eq(x => x.SmContractCode, filter.SmContractCode);

            if (!string.IsNullOrEmpty(filter.ContractNumber))
                filters = filters & Builders<Card>.Filter.Eq(x => x.ContractNumber, filter.ContractNumber);

            if (filter.Current == true || filter.Current == false )
                filters = filters & Builders<Card>.Filter.Eq(x => x.Current, filter.Current);

          
            return _cards.Find(filters, 1, 0);
        }

    }
}
