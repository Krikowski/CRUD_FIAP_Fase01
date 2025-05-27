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
using Prometheus;



namespace Crud_FIAP_Debora_Krikowski {
    public class Program {

        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Conex�o com DB
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Adi��o servi�os ao container
            builder.Services.AddMemoryCache(); // cache
            builder.Services.AddControllers(); // requisi��es http
            builder.Services.AddEndpointsApiExplorer(); // suporte a API
            builder.Services.AddSwaggerGen(); // Swagger

            var app = builder.Build();

            // Middleware entrada de requisi��es
            app.UseMiddleware<ValidationMiddleware>();

            // Migration ambiente DEV
            if (app.Environment.IsDevelopment()) {
                using (var scope = app.Services.CreateScope()) {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Aplica Migrations pendentes
                    try {
                        dbContext.Database.Migrate();
                        logger.LogInformation("Migrations aplicadas com sucesso.");
                    } catch (Exception ex) {
                        logger.LogError(ex, "Erro ao aplicar migrations.");
                    }
                }
            }

            // Configura��o do pipeline HTTP - Ambiente DEV
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Remova a segunda chamada de app.UseRouting()

            app.UseRouting();  // <-- mant�m essa s�

            // Middleware Prometheus padr�o que j� captura m�tricas b�sicas
            app.UseHttpMetrics();

            // Middleware customizado para contar todas as requisi��es por status code
            var counter = Metrics.CreateCounter("http_requests_total",
                "N�mero total de requisi��es HTTP",
                new CounterConfiguration {
                    LabelNames = new[] { "method", "endpoint", "status_code" }
                });

            app.Use(async (context, next) => {
                await next();

                var path = context.Request.Path.Value;

                // Ignora m�tricas para swagger e /metrics
                if (path.StartsWith("/swagger") || path.StartsWith("/metrics")) {
                    return;
                }

                var method = context.Request.Method;
                var endpoint = path ?? "unknown";
                var statusCode = context.Response.StatusCode.ToString();

                counter.WithLabels(method, endpoint, statusCode).Inc();
            });


            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapMetrics(); // exp�e /metrics para Prometheus
            });


            app.Run();
        }
    }
}
