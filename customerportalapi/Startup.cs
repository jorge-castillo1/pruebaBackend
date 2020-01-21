using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoWrapper;
using customerportalapi.Entities;
using customerportalapi.Repositories;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Repositories.utils;
using customerportalapi.Services;
using customerportalapi.Services.interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;

namespace customerportalapi
{
    public class Startup
    {
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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "customerportalapi-{Date}.txt"))
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Mongo Database services
            services.AddScoped<IMongoCollectionWrapper<User>>((serviceProvider) =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<User>(database, "users");
            });
            services.AddScoped<IMongoCollectionWrapper<EmailTemplate>>(serviceProvider =>
            {
                IMongoDatabase database = GetDatabase();
                return new MongoCollectionWrapper<EmailTemplate>(database, "emailtemplates");
            });

            //Mail service
            services.AddScoped<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                SmtpClient client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(config.GetValue<String>("Email:Smtp:Host"),
                            config.GetValue<int>("Email:Smtp:Port"),
                            config.GetValue<bool>("Email:Smtp:EnableSSL"));

                //// Note: only needed if the SMTP server requires authentication
                client.Authenticate(config.GetValue<String>("Email:Smtp:Username"), config.GetValue<String>("Email:Smtp:Password"));
                return client;
            });  

            //Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IMailClient, MailClientWrapper>();
            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

            //Register Business Services
            services.AddTransient<IUserServices, UserServices>();
            services.AddTransient<ISiteServices, SiteServices>();

            services.AddHttpClient("httpClientCRM", c =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
            app.UseMvc();
            app.UseApiResponseAndExceptionWrapper(
                new AutoWrapperOptions
                {
                    ShowStatusCode = true,
                    IsApiOnly = true,
                    IsDebug = env.IsDevelopment()
                });
        }

        private IMongoDatabase GetDatabase()
        {
            MongoClient client = new MongoClient(Configuration.GetConnectionString("customerportaldb"));
            return client.GetDatabase(Configuration["DatabaseName"]);
        }
    }
}
