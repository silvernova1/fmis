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

            #region CONTEXTS

            //no errors on local
            services.AddDbContext<fmisContext>(
                options=>
                {
                    options.UseMySql(Configuration.GetConnectionString("UserConnection"),
                        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));
                
                });

            /*services.AddDbContext<fmisContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("fmisContext")));*/
            services.AddDbContext<DesignationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DesignationContext")));
            services.AddDbContext<DivisionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DivisionContext")));
            services.AddDbContext<SectionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SectionContext")));
            services.AddDbContext<ObligationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ObligationContext")));
            services.AddDbContext<PrexcContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PrexcContext")));
            services.AddDbContext<UacsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("UacsContext")));
            services.AddDbContext<ObligationAmountContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ObligationAmountContext")));
            services.AddDbContext<AppropriationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AppropriationContext")));
            services.AddDbContext<AllotmentClassContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AllotmentClassContext")));
            services.AddDbContext<FundSourceContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundSourceContext")));
            services.AddDbContext<BudgetAllotmentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BudgetAllotmentContext")));
            services.AddDbContext<Yearly_referenceContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Yearly_referenceContext")));
            services.AddDbContext<AppropriationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AppropriationContext")));
            services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyDbContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<FundSourceAmountContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundSourceAmountContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<Ors_headContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Ors_headContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<SubAllotmentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SubAllotmentContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<Suballotment_amountContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Suballotment_amountContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<FundsRealignmentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundsRealignmentContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<SubAllotment_RealignmentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SubAllotment_RealignmentContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<RequestingOfficeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RequestingOfficeContext")));
            services.AddDbContext<ManageUsersContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ManageUsersContext")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<SummaryReportContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SummaryReportContext")));
            services.AddDbContext<SaobContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SaobContext")));
            services.AddDbContext<LogsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LogsContext")));
            services.AddDbContext<RespoCenterContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RespoCenterContext")));
            services.AddDbContext<FundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundContext")));
            services.AddDbContext<UacsTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("UacsTrustFundContext")));
            services.AddDbContext<PrexcTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PrexcTrustFundContext")));
            services.AddDbContext<RespoCenterTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundContext")));
            services.AddDbContext<RequestingOfficeTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RequestingOfficeTrustFundContext")));
            services.AddDbContext<BudgetAllotmentTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BudgetAllotmentTrustFundContext")));
            services.AddDbContext<FundSourceTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundSourceTrustFundContext")));
            services.AddDbContext<FundSourceAmountTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundSourceAmountTrustFundContext")));
            services.AddDbContext<FundsRealignmentTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundsRealignmentTrustFundContext")));
            services.AddDbContext<ObligationTrustFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ObligationTrustFundContext")));
            services.AddDbContext<ObligationAmountTrustFundContext >(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ObligationAmountTrustFundContext")));
            services.AddDbContext<CategoryContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CategoryContext")));
            services.AddDbContext<IndexofpaymentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IndexofpaymentContext")));
            services.AddDbContext<PayeeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PayeeContext")));
            services.AddDbContext<DeductionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DeductionContext")));
            services.AddDbContext<DvContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DvContext")));
            services.AddDbContext<FundClusterContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FundClusterContext")));
            services.AddDbContext<InOfPayDeductionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("InOfPayDeductionContext")));
            services.AddDbContext<SectionsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SectionsContext")));
            #endregion



            //services.AddSignalR();
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
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "CookieAuth";
                options.LoginPath = "/Index/Login";
                options.LogoutPath = "/Index/Logout";
                options.AccessDeniedPath = "/Account/NotFound";
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            });


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
                    name: "scheme1",
                    pattern: "/Account/Login",
                    defaults: new { controller = "Account", action = "Login" });

                endpoints.MapControllerRoute(
                    name: "scheme2",
                    pattern: "/Index/Login",
                    defaults: new { controller = "Index", action = "Login" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });            
        }
    }
}
