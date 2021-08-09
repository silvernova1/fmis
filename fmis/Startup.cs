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
            services.AddControllersWithViews();
            services.AddDbContext<fmisContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("fmisContext")));
           services.AddDbContext<PersonalInformationContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("PersonalInformationContext")));
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
            services.AddDbContext<Obligated_amountContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("Obligated_amountContext")));
            services.AddDbContext<Sub_allotmentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Sub_allotmentContext")));
            services.AddDbContext<Suballotment_amountContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Suballotment_amountContext")));
            services.AddDbContext<RequestingOfficeContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("RequestingOfficeContext")));
            services.AddDbContext<BudgetAllotmentContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("BudgetAllotmentContext")));
            services.AddDbContext<YearlyReferenceContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("YearlyReferenceContext")));
            services.AddDbContext<Ors_headContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("Ors_headContext")));
            services.AddDbContext<UacsContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("UacsContext")));

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
