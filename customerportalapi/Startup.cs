using AutoWrapper;
using customerportalapi.Entities;
using customerportalapi.Repositories;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Repositories.utils;
using customerportalapi.Security;
using customerportalapi.Services;
using customerportalapi.Services.interfaces;
using customerportalapi.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDbCache;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Serilog;

namespace customerportalapi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            //configuration = builder.Build();
            //Configuration = configuration;
            Configuration = builder.BuildAndReplacePlaceholders();

            // creates custom collection `applog`
            IMongoDatabase db = GetDatabase();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.MongoDB(db, collectionName: "logs")
                .CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {          
            //Mongo Database services
            services.AddScoped<IMongoCollectionWrapper<User>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<User>(database, "users");
            });
            services.AddScoped<IMongoCollectionWrapper<EmailTemplate>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<EmailTemplate>(database, "emailtemplates");
            });
            services.AddScoped<IMongoCollectionWrapper<WebTemplate>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<WebTemplate>(database, "webtemplates");
            });
            services.AddScoped<IMongoCollectionWrapper<Process>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<Process>(database, "processes");
            });
            services.AddScoped<IMongoCollectionWrapper<UserAccount>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<UserAccount>(database, "useraccounts");
            });
            services.AddScoped<IMongoCollectionWrapper<Card>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<Card>(database, "cards");
            });
            services.AddScoped<IMongoCollectionWrapper<Pay>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<Pay>(database, "pays");
            });
            services.AddScoped<IMongoCollectionWrapper<EkomiWidget>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<EkomiWidget>(database, "ekomiwidgets");
            });
            services.AddScoped<IMongoCollectionWrapper<UnitLocation>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<UnitLocation>(database, "unitlocations");
            });
            services.AddScoped<IMongoCollectionWrapper<Feature>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<Feature>(database, "features");
            });

            //Mail service
            services.AddScoped(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                SmtpClient client = new SmtpClient { ServerCertificateValidationCallback = (s, c, h, e) => true };
                client.Connect(config.GetValue<string>("Email:Smtp:Host"),
                            config.GetValue<int>("Email:Smtp:Port"),
                            config.GetValue<bool>("Email:Smtp:EnableSSL"));

                //// Note: only needed if the SMTP server requires authentication
                client.Authenticate(config.GetValue<string>("Email:Smtp:Username"), config.GetValue<string>("Email:Smtp:Password"));
                return client;
            });

            //Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IContractSMRepository, ContractSMRepository>();
            services.AddScoped<IMailClient, MailClientWrapper>();
            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IWebTemplateRepository, WebTemplateRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<ISignatureRepository, SignatureRepository>();
            services.AddScoped<IAccountSMRepository, AccountSMRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodsRepository>();
            services.AddScoped<IPayRepository, PayRepository>();
            services.AddScoped<IEkomiWidgetRepository, EkomiWidgetRepository>();
            services.AddScoped<IOpportunityCRMRepository, OpportunityCRMRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IUnitLocationRepository, UnitLocationRepository>();
            services.AddScoped<IFeatureRepository, FeatureRepository>();

            //Register Business Services
            services.AddTransient<IUserServices, UserServices>();
            services.AddTransient<ISiteServices, SiteServices>();
            services.AddTransient<IWebTemplateServices, WebTemplateServices>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ICountryServices, CountryServices>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPaymentService, PaymentServices>();
            services.AddTransient<IProcessService, ProcessService>();
            services.AddTransient<IContractServices, ContractServices>();
            services.AddTransient<IEkomiWidgetService, EkomiWidgetService>();
            services.AddTransient<ILanguageServices, LanguageServices>();

            services.AddHttpClient("httpClient", c =>
            {
                c.BaseAddress = new Uri(Configuration["GatewayUrl"]);
                c.Timeout = new TimeSpan(0, 2, 0);  //2 minutes
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = new TimeSpan(0),
                    MustRevalidate = true
                };
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                //Credentials = GetCredentials()
            });

            services.AddHttpClient("identityClient", c =>
            {
                c.BaseAddress = new Uri(Configuration["Identity:BaseUri"]);
                c.Timeout = new TimeSpan(0, 2, 0);  //2 minutes
                //c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = new TimeSpan(0),
                    MustRevalidate = true
                };
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                        $"{Configuration["Identity:Credentials:ClientId"]}:{Configuration["Identity:Credentials:ClientSecret"]}"))
                );
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });

            services.AddHttpClient("httpClientSignature", c =>
            {
                c.BaseAddress = new Uri(Configuration["GatewaySignatureUrl"]);
                c.Timeout = new TimeSpan(0, 2, 0);  //2 minutes
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = new TimeSpan(0),
                    MustRevalidate = true
                };
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                //Credentials = GetCredentials()
            });

            services.AddHttpClient("httpClientDocument", c =>
            {
                c.BaseAddress = new Uri(Configuration["GatewayDocumentsUrl"]);
                c.Timeout = new TimeSpan(0, 2, 0);  //2 minutes
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = new TimeSpan(0),
                    MustRevalidate = true
                };
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                //Credentials = GetCredentials()
            });
            services.AddHttpClient("httpClientPayment", c =>
            {
                c.BaseAddress = new Uri(Configuration["GatewayPaymentUrl"]);
                c.Timeout = new TimeSpan(0, 2, 0);  //2 minutes
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = new TimeSpan(0),
                    MustRevalidate = true
                };
                var a = Configuration["PaymentCredentials:User"];
                var b = Configuration["PaymentCredentials:Password"];

                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                        $"{Configuration["PaymentCredentials:User"]}:{Configuration["PaymentCredentials:Password"]}"))
                );
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            });

            services.AddMvc()
                 .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p =>
                {
                    p.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CustomerPortalAPI", Version = "v1" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "CustomerPortalApi.xml");
                c.IncludeXmlComments(filePath);

                // Define the Api Key scheme that's in use
                c.AddSecurityDefinition("apiKey", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                    {
                        { "apiKey", new[] {"Bearer {token}"}}
                    });
            });

            services.AddMongoDbCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("customerportaldb");
                options.DatabaseName = Configuration["DatabaseName"];
                options.CollectionName = "appcache";
                options.ExpiredScanInterval = TimeSpan.FromMinutes(10);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = "scheme name";

                // you can also skip this to make the challenge scheme handle the forbid as well
                options.DefaultForbidScheme = "scheme name";

                // of course you also need to register that scheme, e.g. using
                options.AddScheme<SchemeHandler>("scheme name", "scheme display name");
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Register Logger
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //  GET https://localhost:44332/api/users/X8028916F net::ERR_INVALID_HTTP_RESPONSE
            // https://stackoverflow.com/questions/53906866/neterr-invalid-http-response-error-after-post-request-with-angular-7
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions { IsDebug = env.IsDevelopment(), IsApiOnly = true, ShowStatusCode = true });
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerPortalAPI V1");
            });
        }

        private IMongoDatabase GetDatabase()
        {
            MongoClient client = new MongoClient(Configuration.GetConnectionString("customerportaldb"));
            return client.GetDatabase(Configuration["DatabaseName"]);
        }
    }
}
