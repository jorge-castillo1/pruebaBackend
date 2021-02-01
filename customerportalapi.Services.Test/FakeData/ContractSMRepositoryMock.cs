using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class ContractSMRepositoryMock
    {
        public static Mock<IContractSMRepository> ContractSMRepository()
        {
            var db = new Mock<IContractSMRepository>();
            db.Setup(x => x.GetAccessCodeAsync(It.IsAny<string>())).Returns(Task.FromResult(new SMContract()
            {
                Password = "fake password",
                Contractnumber = "fake contract number",
                Customerid = "fake customer id"
            })).Verifiable();

            db.Setup(x => x.GetInvoicesAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Invoice>()
                {
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 139.34M,
                      OurReference = "SJ19/281",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-02-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 90.37M,
                      OurReference = "SJ19/282",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-03-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 120.50M,
                      OurReference = "SJ19/283",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-04-16"),
                      UnitDescription = "6105: SS3PSS",
                      Amount = 190.00M,
                      OurReference = "SJ19/284",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-05-16"),
                      UnitDescription = "6105: SS3PSS",
                      Amount = 90.37M,
                      OurReference = "SJ19/282",
                      OutStanding = 1.00M
                    },
                    new Invoice()
                    {
                      SiteID = "XX1EJ1XX280720060000",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "4100: SS15PT",
                      Amount = 217.73M,
                      OurReference = "P19/232",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "XX1EJ1XX280720060000",
                      DocumentDate = DateTime.Parse("2019-02-16"),
                      UnitDescription = "5101: SS15PT",
                      Amount = 97.55M,
                      OurReference = "P19/233",
                      OutStanding = 0.00M
                    }
                }));

            db.Setup(x => x.GetInvoicesByCustomerIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Invoice>()
                {
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 139.34M,
                      OurReference = "SJ19/281",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-02-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 90.37M,
                      OurReference = "SJ19/282",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-03-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 120.50M,
                      OurReference = "SJ19/283",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-04-16"),
                      UnitDescription = "6105: SS3PSS",
                      Amount = 190.00M,
                      OurReference = "SJ19/284",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-05-16"),
                      UnitDescription = "6105: SS3PSS",
                      Amount = 90.37M,
                      OurReference = "SJ19/282",
                      OutStanding = 1.00M
                    },
                    new Invoice()
                    {
                      SiteID = "XX1EJ1XX280720060000",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "4100: SS15PT",
                      Amount = 217.73M,
                      OurReference = "P19/232",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "XX1EJ1XX280720060000",
                      DocumentDate = DateTime.Parse("2019-02-16"),
                      UnitDescription = "5101: SS15PT",
                      Amount = 97.55M,
                      OurReference = "P19/233",
                      OutStanding = 0.00M
                    }
                }));

            return db;
        }

        public static Mock<IContractSMRepository> ContractSMNoInvoiceRepository()
        {
            var db = new Mock<IContractSMRepository>();

            db.Setup(x => x.GetAccessCodeAsync(It.IsAny<string>())).Returns(Task.FromResult(new SMContract()
            {
                Password = "fake password",
                Contractnumber = "fake contract number",
                Customerid = "fake customer id"
            })).Verifiable();

            db.Setup(x => x.GetInvoicesAsync(It.IsAny<string>())).Returns(Task.FromResult(
                new List<Invoice>()
                {
                }));

            db.Setup(x => x.GetInvoicesByCustomerIdAsync(It.IsAny<string>())).Returns(Task.FromResult(
                new List<Invoice>()
                {
                }));

            return db;
        }

        public static Mock<IContractSMRepository> ContractSMRepositoryInactiveContractSM()
        {
            var db = new Mock<IContractSMRepository>();
            db.Setup(x => x.GetAccessCodeAsync(It.IsAny<string>())).Returns(Task.FromResult(new SMContract()
            {
                Password = "fake password",
                Contractnumber = "fake contract number",
                Customerid = "fake customer id",
                Leaving = "01/01/2021"
            })).Verifiable();

            db.Setup(x => x.GetInvoicesAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Invoice>()
                {
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 139.34M,
                      OurReference = "SJ19/281",
                      OutStanding = 0.00M
                    }
                }));

            db.Setup(x => x.GetInvoicesByCustomerIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Invoice>()
                {
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 139.34M,
                      OurReference = "SJ19/281",
                      OutStanding = 0.00M
                    }
                }));

            return db;
        }

        public static Mock<IContractSMRepository> ContractSMRepositoryUnpaidInvoices()
        {
            var db = new Mock<IContractSMRepository>();
            db.Setup(x => x.GetAccessCodeAsync(It.IsAny<string>())).Returns(Task.FromResult(new SMContract()
            {
                Password = "fake password",
                Contractnumber = "fake contract number",
                Customerid = "fake customer id"
            })).Verifiable();

            db.Setup(x => x.GetInvoicesByCustomerIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Invoice>()
                {
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 139.34M,
                      OurReference = "SJ19/281",
                      OutStanding = 1.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-02-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 90.37M,
                      OurReference = "SJ19/282",
                      OutStanding = 1.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-03-16"),
                      UnitDescription = "6104: SS3PSS",
                      Amount = 120.50M,
                      OurReference = "SJ19/283",
                      OutStanding = 3.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-03-16"),
                      UnitDescription = "6104: SS4PSS",
                      Amount = 120.50M,
                      OurReference = "SJ19/285",
                      OutStanding = 4.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-04-16"),
                      UnitDescription = "6105: SS3PSS",
                      Amount = 190.00M,
                      OurReference = "SJ19/284",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "RI1BBFRI120920060001",
                      DocumentDate = DateTime.Parse("2019-05-16"),
                      UnitDescription = "6105: SS3PSS",
                      Amount = 90.37M,
                      OurReference = "SJ19/282",
                      OutStanding = 1.00M
                    },
                    new Invoice()
                    {
                      SiteID = "XX1EJ1XX280720060000",
                      DocumentDate = DateTime.Parse("2019-01-16"),
                      UnitDescription = "4100: SS15PT",
                      Amount = 217.73M,
                      OurReference = "P19/232",
                      OutStanding = 0.00M
                    },
                    new Invoice()
                    {
                      SiteID = "XX1EJ1XX280720060000",
                      DocumentDate = DateTime.Parse("2019-02-16"),
                      UnitDescription = "5101: SS15PT",
                      Amount = 97.55M,
                      OurReference = "P19/233",
                      OutStanding = 0.00M
                    }
                }));

            return db;
        }
    }
}
