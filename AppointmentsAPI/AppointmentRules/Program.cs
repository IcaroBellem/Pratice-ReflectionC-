
using AppointmentRules.Command;
using AppointmentRules.Command.Handler;
using AppointmentRules.Command.Query;
using AppointmentRules.Data;
using AppointmentRules.Service;
using AppointmentRules.Service.DTOs;
using AppointmentRules.Service.Interface;
using AppointmentRules.Service.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AppointmentRules
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMediatR(typeof(Program).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AppointmentAPI", Version = "v1" });
            });

            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IRuleService, RuleService>();
            builder.Services.AddAuthorization();

            // Add this line to register controller services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configurar middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppointmentAPI v1"));
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }
}
