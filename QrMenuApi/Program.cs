using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QrMenuApi.AutoMapper;
using QrMenuApi.Data;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.Models;
using System.Reflection;

namespace QrMenuApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<QrMenuApiContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<QrMenuApiContext>();

            //builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            QrMenuApiContext? context = app.Services.CreateScope().ServiceProvider.GetService<QrMenuApiContext>();
            RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
            SignInManager<ApplicationUser>? signInManager = app.Services.CreateScope().ServiceProvider.GetService<SignInManager<ApplicationUser>>();
            DbInitializer dBInitializer = new DbInitializer(context, roleManager, userManager);

            app.Run();
        }
    }
}
