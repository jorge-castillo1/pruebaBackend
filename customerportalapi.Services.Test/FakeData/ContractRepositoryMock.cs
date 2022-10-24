using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class ContractRepositoryMock
    {
        public static Mock<IContractRepository> ContractRepositoryFeature()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(
                new List<Contract>
                {
                    new Contract
                    {
                        ContractNumber = "1234567890",
                        ContractDate = "01/01/2020",
                        SmContractCode = "123456786",
                        Store = "Fake Store",
                        StoreData = new Store
                        {
                            StoreName = "Fake Store",
                            Telephone = "Fake telephone",
                            CoordinatesLatitude = "Fake CoordinatesLatitude",
                            CoordinatesLongitude = "Fake CoordinatesLongitude",
                            StoreCode = "RI1BBFRI120920060001",
                            StoreId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                            EmailAddress1 = "EmailAddress1",
                            EmailAddress2 = "EmailAddress2",
                            CountryCode = "ES"
                        },
                        Unit = new Unit
                        {
                            Depth = "4.96",
                            Height = "2.5",
                            Size = "14.5000000000",
                            SmUnitId = "BD17",
                            Subtype = "GR",
                            UnitCategory = "SS",
                            UnitName = "6104",
                            Width = "2.92"
                        },
                        OpportunityId = "OpportunityId"
                    }
                })).Verifiable();


            return db;
        }

        public static Mock<IContractRepository> ContractRepositoryOne()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<Contract>
            {
                new Contract
                {
                   ContractNumber = "1234567890",
                   ContractDate = "01/01/2020",
                   SmContractCode ="123456786",
                   Store = "Fake Store",
                   StoreData = new Store
                   {
                       StoreName = "Fake Store",
                       Telephone = "Fake telephone",
                       CoordinatesLatitude = "Fake CoordinatesLatitude",
                       CoordinatesLongitude = "Fake CoordinatesLongitude",
                       StoreCode ="RI1BBFRI120920060001",
                       StoreId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        EmailAddress1 = "EmailAddress1",
                        EmailAddress2 = "EmailAddress2",
                        CountryCode = "ES"
                   },
                   Unit = new Unit
                   {
                        Depth = "4.96",
                        Height = "2.5",
                        Size = "14.5000000000",
                        SmUnitId = "BD17",
                        Subtype = "GR",
                        UnitCategory = "SS",
                        UnitName = "6104",
                        Width = "2.92"
                   },
                   OpportunityId = "OpportunityId",
                   ContractExpectedMoveIn = "01/01/2020",
                   ExpectedMoveIn = "01/01/2020",
                   TimeZone = "Fake timezone"
                }
            })).Verifiable();

            db.Setup(x => x.GetContractAsync(It.IsAny<string>())).Returns(Task.FromResult(new Contract
            {
                ContractNumber = "1234567893",
                ContractDate = "01/01/2020",
                Store = "Fake Store 2",
                StoreData = new Store
                {
                    StoreName = "Fake Store",
                    Telephone = "Fake telephone",
                    CoordinatesLatitude = "Fake CoordinatesLatitude",
                    CoordinatesLongitude = "Fake CoordinatesLongitude",
                    StoreCode = "RI1BBFRI120920060000",
                    StoreId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    EmailAddress1 = "EmailAddress1",
                    EmailAddress2 = "EmailAddress2"
                },
                SmContractCode = "123456789",
                OpportunityId = "OpportunityId",
                ContractExpectedMoveIn = "01/01/2020",
                ExpectedMoveIn = "01/01/2020",
                TimeZone = "Fake timezone"
            })).Verifiable();

            return db;
        }


        public static Mock<IContractRepository> ContractRepository()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<Contract>
            {
                new Contract
                {
                   ContractNumber = "1234567890",
                   ContractDate = "01/01/2020",
                   SmContractCode ="123456786",
                   Store = "Fake Store",
                   StoreData = new Store
                   {
                       StoreName = "Fake Store",
                       Telephone = "Fake telephone",
                       CoordinatesLatitude = "Fake CoordinatesLatitude",
                       CoordinatesLongitude = "Fake CoordinatesLongitude",
                       StoreCode ="RI1BBFRI120920060001",
                       StoreId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        EmailAddress1 = "EmailAddress1",
                        EmailAddress2 = "EmailAddress2",
                        CountryCode = "ES"
                   },
                   Unit = new Unit
                   {
                        Depth = "4.96",
                        Height = "2.5",
                        Size = "14.5000000000",
                        SmUnitId = "BD17",
                        Subtype = "GR",
                        UnitCategory = "SS",
                        UnitName = "6104",
                        Width = "2.92"
                   },
                   OpportunityId = "OpportunityId",
                   ContractExpectedMoveIn = "01/01/2020",
                   ExpectedMoveIn = "01/01/2020",
                   TimeZone = "Fake timezone"
                },
                new Contract
                {
                   ContractNumber = "1234567891",
                   ContractDate = "01/01/2019",
                   Store = "Fake Store",
                   SmContractCode ="123456787",
                   StoreData = new Store
                   {
                        StoreName = "Fake Store",
                        Telephone = "Fake telephone",
                        CoordinatesLatitude = "Fake CoordinatesLatitude",
                        CoordinatesLongitude = "Fake CoordinatesLongitude",
                        StoreCode="RI1BBFRI120920060001",
                        StoreId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        EmailAddress1 = "EmailAddress1",
                        EmailAddress2 = "EmailAddress2",
                        CountryCode = "ES"
                    },
                   Unit = new Unit
                   {
                        Depth = "4.96",
                        Height = "2.5",
                        Size = "14.5000000000",
                        SmUnitId = "BD17",
                        Subtype = "GR",
                        UnitCategory = "SS",
                        UnitName = "6105",
                        Width = "2.92"
                   },
                   OpportunityId = "OpportunityId",
                   ContractExpectedMoveIn = "01/01/2020",
                   ExpectedMoveIn = "01/01/2020",
                   TimeZone = "Fake timezone"
                },
                new Contract
                {
                   ContractNumber = "1234567892",
                   ContractDate = "01/01/2020",
                   SmContractCode ="123456788",
                   Store = "Fake Store 2",
                   StoreData = new Store
                   {
                        StoreName = "Fake Store",
                        Telephone = "Fake telephone",
                        CoordinatesLatitude = "Fake CoordinatesLatitude",
                        CoordinatesLongitude = "Fake CoordinatesLongitude",
                        StoreCode="RI1BBFRI120920060000",
                        StoreId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        EmailAddress1 = "EmailAddress1",
                        EmailAddress2 = "EmailAddress2",
                        CountryCode = "ES"
                   },
                   Unit = new Unit
                   {
                        Depth = "4.96",
                        Height = "2.5",
                        Size = "14.5000000000",
                        SmUnitId = "BD17",
                        Subtype = "GR",
                        UnitCategory = "SS",
                        UnitName = "4100",
                        Width = "2.92"
                   }
                   ,
                   OpportunityId = "OpportunityId",
                   ContractExpectedMoveIn = "01/01/2020",
                   ExpectedMoveIn = "01/01/2020",
                   TimeZone = "Fake timezone"
                },
                new Contract
                {
                   ContractNumber = "1234567893",
                   ContractDate = "01/01/2020",
                   SmContractCode ="123456789",
                   Store = "Fake Store 2",
                   StoreData = new Store
                   {
                        StoreName = "Fake Store",
                        Telephone = "Fake telephone",
                        CoordinatesLatitude = "Fake CoordinatesLatitude",
                        CoordinatesLongitude = "Fake CoordinatesLongitude",
                        StoreCode="RI1BBFRI120920060000",
                        StoreId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        EmailAddress1 = "EmailAddress1",
                        EmailAddress2 = "EmailAddress2",
                        CountryCode = "ES"
                   },
                   Unit = new Unit
                   {
                        Depth = "4.96",
                        Height = "2.5",
                        Size = "14.5000000000",
                        SmUnitId = "BD17",
                        Subtype = "GR",
                        UnitCategory = "SS",
                        UnitName = "5101",
                        Width = "2.92"
                   },
                   OpportunityId = "OpportunityId",
                   ContractExpectedMoveIn = "01/01/2020",
                   ExpectedMoveIn = "01/01/2020",
                   TimeZone = "Fake timezone"
                },
            })).Verifiable();

            db.Setup(x => x.GetContractAsync(It.IsAny<string>())).Returns(Task.FromResult(new Contract
            {
                ContractNumber = "1234567893",
                ContractDate = "01/01/2020",
                Store = "Fake Store 2",
                StoreData = new Store
                {
                    StoreName = "Fake Store",
                    Telephone = "Fake telephone",
                    CoordinatesLatitude = "Fake CoordinatesLatitude",
                    CoordinatesLongitude = "Fake CoordinatesLongitude",
                    StoreCode = "RI1BBFRI120920060000",
                    StoreId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    EmailAddress1 = "EmailAddress1",
                    EmailAddress2 = "EmailAddress2",
                    CountryCode = "ES"
                },
                SmContractCode = "123456789",
                OpportunityId = "OpportunityId",
                ContractExpectedMoveIn = "01/01/2020",
                ExpectedMoveIn = "01/01/2020",
                TimeZone = "Fake timezone"
            })).Verifiable();

            return db;
        }

        public static Mock<IContractRepository> InvalidContractRepository()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractAsync(It.IsAny<string>())).Returns(Task.FromResult(new Contract
            {
                ContractDate = "01/01/2020",
                Store = "Fake Store",
                StoreData = new Store
                {
                    StoreName = "Fake Store",
                    Telephone = "Fake telephone",
                    CoordinatesLatitude = "Fake CoordinatesLatitude",
                    CoordinatesLongitude = "Fake CoordinatesLongitude",
                    CountryCode = "ES"
                },
                OpportunityId = "OpportunityId",
                ContractExpectedMoveIn = "01/01/2020",
                ExpectedMoveIn = "01/01/2020",
                TimeZone = "Fake timezone"
            })).Verifiable();

            db.Setup(x => x.GetContractsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(
                new List<Contract>
                {
                    new Contract
                    {
                        ContractDate = "01/01/2020",
                        Store = "Fake Store",
                        StoreData = new Store
                        {
                            StoreName = "Fake Store",
                            Telephone = "Fake telephone",
                            CoordinatesLatitude = "Fake CoordinatesLatitude",
                            CoordinatesLongitude = "Fake CoordinatesLongitude"
                        },
                        OpportunityId = "OpportunityId",
                        ContractExpectedMoveIn = "01/01/2020",
                        ExpectedMoveIn = "01/01/2020",
                        TimeZone = "Fake timezone"
                    }
                }
            )).Verifiable();

            return db;
        }

        public static Mock<IContractRepository> ValidContractRepository()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractAsync(It.IsAny<string>())).Returns(Task.FromResult(new Contract
            {
                ContractNumber = "1234567890",
                ContractDate = "01/01/2020",
                SmContractCode = "R1234567890",
                Store = "Fake Store",
                StoreData = new Store
                {
                    StoreName = "Fake Store",
                    Telephone = "Fake telephone",
                    CoordinatesLatitude = "Fake CoordinatesLatitude",
                    CoordinatesLongitude = "Fake CoordinatesLongitude",
                    EmailAddress1 = "EmailAddress1",
                    EmailAddress2 = "EmailAddress2",
                    CountryCode = "ES"
                },
                OpportunityId = "FakeOpportunityId",
                ContractExpectedMoveIn = "01/01/2020",
                ExpectedMoveIn = "01/01/2020",
                TimeZone = "Fake timezone"
            })).Verifiable();

            return db;
        }

        public static Mock<IContractRepository> ValidContractRepositoryWithPaymentMethod()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractAsync(It.IsAny<string>())).Returns(Task.FromResult(new Contract
            {
                ContractNumber = "1234567890",
                ContractDate = "01/01/2020",
                SmContractCode = "R1234567890",
                Store = "Fake Store",
                StoreData = new Store
                {
                    StoreName = "Fake Store",
                    Telephone = "Fake telephone",
                    CoordinatesLatitude = "Fake CoordinatesLatitude",
                    CoordinatesLongitude = "Fake CoordinatesLongitude",
                    CountryCode = "ES"
                },
                OpportunityId = "FakeOpportunityId",
                PaymentMethodId = "FakePaymentMethodId",
                ContractExpectedMoveIn = "01/01/2020",
                ExpectedMoveIn = "01/01/2020",
                TimeZone = "Fake timezone"
            })).Verifiable();

            return db;
        }

        public static Mock<IContractRepository> ValidContractRepositoryWithoutStoreEmail()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractAsync(It.IsAny<string>())).Returns(Task.FromResult(new Contract
            {
                ContractNumber = "1234567890",
                ContractDate = "01/01/2020",
                SmContractCode = "R1234567890",
                Store = "Fake Store",
                StoreData = new Store
                {
                    StoreName = "Fake Store",
                    Telephone = "Fake telephone",
                    CoordinatesLatitude = "Fake CoordinatesLatitude",
                    CoordinatesLongitude = "Fake CoordinatesLongitude",
                    CountryCode = "ES"
                },
                OpportunityId = "FakeOpportunityId",
                PaymentMethodId = "FakePaymentMethodId",
                ContractExpectedMoveIn = "01/01/2020",
                ExpectedMoveIn = "01/01/2020",
                TimeZone = "Fake timezone"
            })).Verifiable();

            return db;
        }
    }
}