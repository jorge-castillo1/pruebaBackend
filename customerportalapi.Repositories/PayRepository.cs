using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;

namespace customerportalapi.Repositories
{
    public class PayRepository : IPayRepository
    {
        private readonly IMongoCollectionWrapper<Pay> _pays;

        public PayRepository(IConfiguration config, IMongoCollectionWrapper<Pay> pays)
        {
            _pays = pays;
        }

        public Pay Get(string username, string invoiceNumber)
        {
            Pay pay = new Pay();

            var paysInfo = _pays.FindOne(t => t.Username == username && t.InvoiceNumber == invoiceNumber);
            foreach (var p in paysInfo)
            {
                pay = p;
            }
            return pay;
        }
        public Pay GetByExternalId(string externalId)
        {
            Pay pay = new Pay();

            var paysInfo = _pays.FindOne(t => t.ExternalId == externalId);
            foreach (var c in paysInfo)
            {
                pay = c;
            }
            return pay;
        }

        public Pay Update(Pay pay)
        {
            // Update Pay
            var filter = Builders<Pay>.Filter.Eq(s => s.ExternalId, pay.ExternalId);
            var result = _pays.ReplaceOne(filter, pay);

            return pay;
        }

        public Task<bool> Create(Pay pay)
        {
            // Create Pay
            _pays.InsertOne(pay);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(Pay pay)
        {
            // Delete Pay
            var filter = Builders<Pay>.Filter.Eq("_id", pay.Id);
            _pays.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

    }
}
