using BusinessLayer.IRepository;
using BusinessLayer.Repository;
using EntityModel.Models;
using Microsoft.EntityFrameworkCore;

namespace Retailer_webApi
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<PosContext>(
         options => options.UseSqlServer(
        Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddSwaggerGen();

            // Add services to the service collection

            // Register authentication if needed
            services.AddAuthentication("Cookies")
                    .AddCookie("Cookies");

            // Register authorization services
            services.AddAuthorization();

            // Register controllers
            services.AddControllers();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add middlware components here
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V2");
            });

        }
    }
}
