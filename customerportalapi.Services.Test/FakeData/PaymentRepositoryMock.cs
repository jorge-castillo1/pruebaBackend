using System.Runtime.CompilerServices;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace customerportalapi.Services.Test.FakeData
{
    public static class PaymentRepositoryMock
    {

        public static Mock<IPaymentRepository> PaymentRepository()
        {
            var db = new Mock<IPaymentRepository>();
            db.Setup(x => x.ChangePaymentMethodCard(It.IsAny<HttpContent>())).Returns(Task.FromResult("<form></form>")).Verifiable();
            
            db.Setup(x => x.ConfirmChangePaymentMethodCard(It.IsAny<PaymentMethodCardConfirmationToken>())).Returns(Task.FromResult(
                new PaymentMethodCardConfirmationResponse()
                {
                    ExternalId = "fake ExternalId",
                    Message = "success",
                    Status = "Success"
                }
            )).Verifiable();

            return db;
        }
    }
}
