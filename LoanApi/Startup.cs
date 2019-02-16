using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using LoanApi.Exceptions;
using LoanApi.Models;
using LoanApi.Repository;
using LoanApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace LoanApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            //Add Identity and Jwt
            services.AddIdentity<AppUser, IdentityRole>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 3;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;

                // Lockout settings
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                option.Lockout.MaxFailedAccessAttempts = 3;
                option.Lockout.AllowedForNewUsers = true;

                // User settings
                option.User.RequireUniqueEmail = true;
                option.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                //o.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "http://loan.acyst.tech",
                        ValidAudience = "http://localhost:53720",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wertyuiopasdfghjklzxcvbnm123456")),
                    };
                }).AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "662510424697-6na0e00bgn73tf5s9sn0iv89sjjja9k4.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "o3P-Xb6jmpEg6uEQvWukqnWx";
                });

            services.AddDataProtection();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", a =>
                {
                    a.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(20000));
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("CanAssignRoles", "true"));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeId"));
            });
            
            services.AddOptions();
            services.AddLogging();
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new Info { Title = "LoanApi Api", Version = "v1" });
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Dedawa Api",
                    Description = "A Loan Mgt Software Api",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Harmony Alabi",
                        Email = "Harmonizerblinks@gmail.com",
                        Url = "http://www.linkedin.com/in/harmony-alabi-93907086/"
                    },
                    License = new License
                    {
                        Name = "Acyst Technology Ltd",
                        Url = "https://github.com/harmonizerblinks/LoanApi-software-blob/master/LICENSE"
                    }
                });
            });

            services.AddMvc(o =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                //o.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new LowercaseContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).AddXmlDataContractSerializerFormatters().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IAppUserRepository, AppUserRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountTypeRepository, AccountTypeRepository>();
            services.AddTransient<ICotRepository, CotRepository>();
            services.AddTransient<IChargeRepository, ChargeRepository>();
            services.AddTransient<IChequeRepository, ChequeRepository>();
            services.AddTransient<ILocationRepository, LocationRepository>();
            services.AddTransient<INominalRepository, NominalRepository>();
            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ISequenceRepository, SequenceRepository>();
            services.AddTransient<ISmsApiRepository, SmsApiRepository>();
            services.AddTransient<ISmsRepository, SmsRepository>();
            services.AddTransient<IAlertRepository, AlertRepository>();
            services.AddTransient<ITellerRepository, TellerRepository>();
            services.AddTransient<ITransitRepository, TransitRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();

            services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<IMyServices, MyServices>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        
        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpStatusCodeExceptionMiddleware();
            } else {
                app.UseHttpStatusCodeExceptionMiddleware();
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            app.UseCors("AllowAny");
            app.UseAuthentication();
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.UseDatabaseErrorPage();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Files")),
                RequestPath = "/Files"
            });
            // app.UseSerilog();
            app.UseStatusCodePages();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dedawa Api");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

    }
}
