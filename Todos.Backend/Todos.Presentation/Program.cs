using Microsoft.EntityFrameworkCore;
using Todos.API;
using Todos.Infrastructure;
using Todos.Service;
using Todos.Middlewares;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var corsAllow = "AllowFrontend";
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: corsAllow, policy =>
            {
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
                else
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
            });
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        builder.Services.AddScoped<ITodoService, TodoService>();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<ErrorHandler>();

        app.UseCors(corsAllow);

        app.MapTodosEndpoint();

        app.Run();
    }
}
