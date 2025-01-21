using Crud_FIAP_Debora_Krikowski.Data;
using Crud_FIAP_Debora_Krikowski.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;


namespace Crud_FIAP_Debora_Krikowski
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            // Conexão com DB
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Adição serviços ao container
            builder.Services.AddMemoryCache(); //cache
            builder.Services.AddControllers(); //requisições http
            builder.Services.AddEndpointsApiExplorer(); //suporte a API
            builder.Services.AddSwaggerGen(); //Swegger

            var app = builder.Build();

            //Middleware entrada de requisições
            app.UseMiddleware<ValidationMiddleware>();


            // migration ambiente DEV
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); 

                    //Aplica Migrations pendentes
                    try
                    {
                        dbContext.Database.Migrate(); 
                        logger.LogInformation("Migrations aplicadas com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erro ao aplicar migrations.");
                    }
                }
            }

            // Configuração do pipeline HTTP - Ambiente DEV
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers(); //mapeia controlers

            app.Run();

        }
    }
}
