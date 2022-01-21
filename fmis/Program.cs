using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using fmis.Models;

namespace fmis.Models
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();

            /*            using (var context = new MyDbContext())
                        {
                            var blogs = context.Blogs
                                .Include(blog => blog.Posts)
                                .ToList();
                        }*/
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
}
