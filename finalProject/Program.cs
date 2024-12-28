using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using finalProject.Data;

namespace finalProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure the database context
            builder.Services.AddDbContext<finalProjectContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("finalProjectContext")
                ?? throw new InvalidOperationException("Connection string 'finalProjectContext' not found.")));

            // Add Razor Pages and Controllers
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();

            // Optional: Configure CORS if requests are coming from a different origin
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable CORS middleware before authorization
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers(); // Map API Controllers

            app.Run();
        }
    }
}
