using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace customerportalapi.Services.Test.FakeData
{
    public static class MongoFeaturesRepositoryMock
    {

        public static Mock<IMongoCollectionWrapper<Feature>> FeatureRepository_NoMail()
        {

            var db = new Mock<IMongoCollectionWrapper<Feature>>();
            db.Setup(x => x.FindOne(It.IsAny<Expression<Func<Feature, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<Feature>(){
                    new Feature() {
                        Name = "emailWelcomeInvitation",
                        CountryAvailable = new List<string> () {"fr","es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=false
                        } },

                    },
                    new Feature() {
                        Name = "emailWelcomeInvitationExtended",
                        CountryAvailable = new List<string> () {"fr","es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } }
                    }

                }).Verifiable();


            return db;
        }

        public static Mock<IMongoCollectionWrapper<Feature>> FeatureRepository_NoMail2()
        {

            var db = new Mock<IMongoCollectionWrapper<Feature>>();
            db.Setup(x => x.FindOne(It.IsAny<Expression<Func<Feature, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<Feature>(){
                    new Feature() {
                        Name = "emailWelcomeInvitation",
                        CountryAvailable = new List<string> () {"fr"},
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } },

                    },
                    new Feature() {
                        Name = "emailWelcomeInvitationExtended",
                         CountryAvailable = new List<string> () {"es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } }
                    }

                }).Verifiable();


            return db;
        }

        public static Mock<IMongoCollectionWrapper<Feature>> FeatureRepository_WelcomeLong()
        {

            var db = new Mock<IMongoCollectionWrapper<Feature>>();
            db.Setup(x => x.FindOne(It.IsAny<Expression<Func<Feature, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<Feature>(){ 
                    new Feature() { 
                        Name = "emailWelcomeInvitation", 
                        CountryAvailable = new List<string> () {"fr","es" }, 
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } },

                    },
                    new Feature() {
                        Name = "emailWelcomeInvitationExtended",
                         CountryAvailable = new List<string> () {"fr","es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } } 
                    }

                }).Verifiable();


            return db;
        }

        public static Mock<IMongoCollectionWrapper<Feature>> FeatureRepository_WelcomeLong2()
        {
            var db = new Mock<IMongoCollectionWrapper<Feature>>();
            db.Setup(x => x.FindOne(It.IsAny<Expression<Func<Feature, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<Feature>(){
                    new Feature() {
                        Name = "emailWelcomeInvitation",
                        CountryAvailable = new List<string> () {"fr","es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } },

                    },
                    new Feature() {
                        Name = "emailWelcomeInvitationExtended",
                         CountryAvailable = null,
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } }
                    }

                }).Verifiable();


            return db;
        }

        public static Mock<IMongoCollectionWrapper<Feature>> FeatureRepository_WelcomeShort()
        {
            var db = new Mock<IMongoCollectionWrapper<Feature>>();
            db.Setup(x => x.FindOne(It.IsAny<Expression<Func<Feature, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<Feature>(){
                    new Feature() {
                        Name = "emailWelcomeInvitation",
                        CountryAvailable = new List<string> () {"fr","es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } },

                    },
                    new Feature() {
                        Name = "emailWelcomeInvitationExtended",
                         CountryAvailable = new List<string> () {"es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=false
                        } }
                    }

                }).Verifiable();


            return db;
        }

        public static Mock<IMongoCollectionWrapper<Feature>> FeatureRepository_WelcomeShort2()
        {
            var db = new Mock<IMongoCollectionWrapper<Feature>>();
            db.Setup(x => x.FindOne(It.IsAny<Expression<Func<Feature, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<Feature>(){
                    new Feature() {
                        Name = "emailWelcomeInvitation",
                        CountryAvailable = new List<string> () {"fr","es" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } },

                    },
                    new Feature() {
                        Name = "emailWelcomeInvitationExtended",
                         CountryAvailable = new List<string> () {"fr" },
                        Environments =new List<FeatureEnvironment>(){ new FeatureEnvironment()
                        {
                            Name="DEV",
                            Value=true
                        } }
                    }

                }).Verifiable();


            return db;
        }

    }
}
