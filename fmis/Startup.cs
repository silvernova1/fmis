using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using fmis.Data.John;
using Microsoft.AspNetCore.Identity;
using fmis.Data.Carlo;
using fmis.Data.Accounting;
using fmis.Data.silver;
using Microsoft.Owin;
using fmis.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using fmis.Services;
using Newtonsoft.Json;
using fmis;
using Microsoft.AspNetCore.Authorization;
using System.Web.Http.Controllers;

[assembly: OwinStartup(typeof(fmis.Startup))]

namespace fmis
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddRazorPages();
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            services.AddTransient<IUserService, UserService>();

            services.AddSingleton<AutoIncrementGenerator>();
            services.AddHttpContextAccessor();

            #region CONTEXTS

            //no errors on local
            services.AddDbContext<fmisContext>(
                options=>
                {
                    options.UseMySql(Configuration.GetConnectionString("UserConnection"),
                        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));
                
                });

            services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyDbContext")));
            #endregion



            services.AddSignalR();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });


            services.Add(new ServiceDescriptor(typeof(PersonalInformationMysqlContext), new PersonalInformationMysqlContext(Configuration.GetConnectionString("PersonalInformationMysqlContext"))));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //amalio
            services.Add(new ServiceDescriptor(typeof(Personal_InfoMysqlContext), new Personal_InfoMysqlContext(Configuration.GetConnectionString("Personal_InfoMysqlContext"))));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.Add(new ServiceDescriptor(typeof(DtsMySqlContext), new DtsMySqlContext(Configuration.GetConnectionString("DtsMySqlContext"))));
            services.AddDatabaseDeveloperPageExceptionFilter();


            services.AddDistributedMemoryCache();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Scheme1";
                options.DefaultChallengeScheme = "Scheme1";
            })
                .AddCookie("Scheme1", options =>
                {
                    options.Cookie.Name = "Scheme1";
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/NotFound";
                    options.ExpireTimeSpan = TimeSpan.FromHours(5);
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                })
                .AddCookie("Scheme2", options =>
                {
                    options.Cookie.Name = "Scheme2";
                    options.LoginPath = "/Index/Login";
                    options.LogoutPath = "/Index/Logout";
                    options.AccessDeniedPath = "/Index/NotFound";
                    options.ExpireTimeSpan = TimeSpan.FromHours(5);
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                }); ;


            services.AddAuthorization(options =>
            {
                options.AddPolicy("BudgetAdmin", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "budget_admin"));
                options.AddPolicy("Permanent", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "Permanent"));

                options.AddPolicy("BudgetUser", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "budget_user"));
                options.AddPolicy("AccountingUser", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "accounting_user"));
                options.AddPolicy("Administrator", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "admin"));

                options.AddPolicy("Accounting", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "accounting_admin"));
                options.AddPolicy("User", polBuilder => polBuilder.RequireClaim(ClaimTypes.Role, "user"));
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });
        }


        public class CustomRequirement : IAuthorizationRequirement
        {
            // Empty requirement
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyOrigin());
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<DvHub>("/dvHub");
            });            
        }
    }
}
