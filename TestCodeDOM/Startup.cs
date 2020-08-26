using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestCodeDOM.Context;

namespace TestCodeDOM
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
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddSingleton(new TypeHelper(GetType()));

            services.AddControllers();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            AddSomeType(serviceProvider);
        }


        //Add someType

        void AddSomeType(IServiceProvider serviceProvider)
        {
            var typeHelper = serviceProvider.GetRequiredService<TypeHelper>();
            typeHelper.AddType("Student1", new[] { "Id", "Name", "Address" }, new[] { typeof(int), typeof(string), typeof(string) });
            typeHelper.AddType("Student2", new[] { "Id", "Name", "Address" }, new[] { typeof(int), typeof(string), typeof(string) });
            typeHelper.AddType("Student3", new[] { "Id", "Name", "Address" }, new[] { typeof(int), typeof(string), typeof(string) });
        }
    }
}
