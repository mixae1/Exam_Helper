
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using Exam_Helper.Controllers;
namespace Exam_Helper
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
            services.AddControllersWithViews().AddSessionStateTempDataProvider();

            services.AddHttpContextAccessor();
            services.AddTransient<ISessionWorker, SessionWorker>();


            var temp = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<CommonDbContext>(opt=>opt.UseNpgsql(temp));

            services.AddIdentity<User, IdentityRole>(
                opt=>
                {
                    opt.User.RequireUniqueEmail = true;
                    opt.Password.RequireNonAlphanumeric = false;
                    
                }
                ).AddEntityFrameworkStores<CommonDbContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opt => 
            opt.LoginPath = "/UserAccount/Login");

            services.AddSession(opt=>opt.IdleTimeout= TimeSpan.FromSeconds(1800));
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
                // The default HSTS value is 30 days.  You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

           
            app.UseAuthentication();    // подключение аутентификации
            app.UseAuthorization();

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
