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
using fmis.Data.silver;
using fmis.Areas.Identity.Data;
using Microsoft.Owin;
using fmis.Hubs;

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

            /*services.AddDbContext<MyDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("UserContextConnection")));
            services.AddScoped<IUserClaimsPrincipalFactory<fmisUser>,
                ApplicationUserClaimsPrincipalFactory>();

            services.AddDefaultIdentity<fmisUser>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.AllowedUserNameCharacters = " abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            }).AddRoles<IdentityRole>()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<MyDbContext>();*/



            services.AddControllers();

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddSession();
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddDbContext<fmisContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("fmisContext")));
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
            services.AddDbContext<UtilizationContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("UtilizationContext")));
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
            services.AddDbContext<Sub_allotmentContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("Sub_allotmentContext")));
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
            services.AddDbContext<UtilizationAmountContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("UtilizationAmountContext")));
            services.AddDbContext<LogsContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("LogsContext")));
            services.AddDbContext<RespoCenterContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("RespoCenterContext")));
            services.AddDbContext<FundContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("FundContext")));



            services.Add(new ServiceDescriptor(typeof(PersonalInformationMysqlContext), new PersonalInformationMysqlContext(Configuration.GetConnectionString("PersonalInformationMysqlContext"))));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //amalio
            services.Add(new ServiceDescriptor(typeof(Personal_InfoMysqlContext), new Personal_InfoMysqlContext(Configuration.GetConnectionString("Personal_InfoMysqlContext"))));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.Add(new ServiceDescriptor(typeof(DtsMySqlContext), new DtsMySqlContext(Configuration.GetConnectionString("DtsMySqlContext"))));
            services.AddDatabaseDeveloperPageExceptionFilter();
        }    

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            CreateRoles(serviceProvider).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<fmisUser>>();
            string[] roleNames = { "Admin", "Budget" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new fmisUser
            {

                UserName = "doh7budget",
                Email = "doh7budget@gmail.com",
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = "doh7budget";
            var _user = await UserManager.FindByEmailAsync("doh7budget");

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Budget");

                }
            }
        }
    }
}
