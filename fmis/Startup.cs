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

namespace fmis
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
            services.AddRazorPages();

            /* services.AddRazorPages()
                    .AddRazorPagesOptions(options => {
                        options.RootDirectory = "/Budget";
                    });*/
          

            services.AddControllersWithViews();

            services.AddDbContext<fmisContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("fmisContext")));

           services.AddDbContext<PersonalInformationContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("PersonalInformationContext")));

            services.AddDbContext<DesignationContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DesignationContext")));

            services.AddDbContext<DivisionContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DivisionContext")));

            /*var userconn = "Data Source=LAMBORGHINI\\VEEAMSQL2016;Initial Catalog=FMIS;Integrated Security=False";
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(userconn));*/

            services.AddDbContext<SectionContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("SectionContext")));

            /*    services.AddMvc().AddRazorPagesOptions(options => {
                    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
    */

            /* services.AddDbContext<PersonalInformationContext>(o =>
                         o.UseSqlServer(Configuration.GetConnectionString("PersonalInformationContext"))
                         .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));*/


            /* services.AddDbContext<PersonalInformationContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("PersonalInformationContext")));*/

            /*services.AddScoped<PersonalInformationContext>();*/
        }


     
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseStaticFiles();

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
    }
}
