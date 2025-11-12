
using Jahez_Task.Mapper.BookLoanMapping;
using Jahez_Task.Mapper.BookMapping;
using Jahez_Task.Models;
using Jahez_Task.Repository.BookLoanRepo;
using Jahez_Task.Repository.BookRepo;
using Jahez_Task.Services.BookService;
using Jahez_Task.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Jahez_Task
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //configure database connection 
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Config Services 
            builder.Services.AddScoped<IBookRepository , BookRepository>();
            builder.Services.AddScoped<IBookService  , BookService>();
            builder.Services.AddScoped<IBookLoanRepository , BookLoanRepository>();
            builder.Services.AddScoped<IBookService, BookService>();
            //AutoMapper registration 
           
            builder.Services.AddAutoMapper(typeof(BookMapper));
            builder.Services.AddAutoMapper(typeof(BookLoanMapper));
            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(
               options =>
               {
                   options.Password.RequireDigit = true;
                   options.Password.RequiredLength = 6;
                   options.Password.RequireLowercase = false;
                   options.Password.RequireUppercase = false;
                   options.Password.RequireNonAlphanumeric = false;

               }
                )
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Add authentication with JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes("your_super_secret_key"))
                };
            });

            // Register the UnitOfWork service
            builder.Services.AddScoped<unitOfWork>();


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
