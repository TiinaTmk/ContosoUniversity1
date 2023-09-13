using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ContosoUniversity
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add service to the container.
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

            // The default HSTS value is 30 days.
            app.UseHsts();
            }

            else
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }


            // Enable HTTPS redirection and serve static files like CSS, JavaScript, and image
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            
            // Enable routing and authorization middleware.
            // Routing is responsible for determining which controller and action should handle a given request,
            // while authorization ensures that users have the necessary permissions to access certain resources.
            app.UseRouting();
            app.UseAuthorization();

            
            // Map default controller route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.CreateDbIfNotExists();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<SchoolContext>();
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);
            }


            app.Run();
        }
            private static void CreateDbIfNotExists(IHost host)
        {
            //Creates a new scope within which you can resolve and use services.
            using (var scope = host.Services.CreateScope())
            {
                // This line retrieves the IServiceProvider from the created scope.
                // The IServiceProvider is responsible for managing and providing access to registered services.
                var services = scope.ServiceProvider;
                try
                {
                    // Requesting an instance of the SchoolContext class from the service provider.
                    // The GetRequiredService method is used to retrieve a service of a specific type. 
                    var context = services.GetRequiredService<SchoolContext>();
                    //Assuming that context now holds an instance of SchoolContext, this line calls the DbInitializer.Initialize method
                    //and passes the SchoolContext instance as an argument. This is where the actual database initialization and seed data insertion occur.
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}