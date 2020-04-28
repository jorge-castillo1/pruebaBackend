using System.Runtime.CompilerServices;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class PaymentMethodRepositoryMock
    {
       

        public static Mock<IPaymentMethodRepository> PaymentMethodRepository()
        {
            var db = new Mock<IPaymentMethodRepository>();
            db.Setup(x => x.GetPaymentMethod(It.IsAny<string>())).Returns(Task.FromResult(new PaymentMethodCRM()
            {
                Name = "Name",
                StoreId = "fakestoreId",
                SMId = "fakeSMId"
            })).Verifiable();

            return db;
        }
    }
}
