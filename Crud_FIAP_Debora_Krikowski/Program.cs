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

            // Conexão com DB
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Adição serviços ao container
            builder.Services.AddMemoryCache(); // cache
            builder.Services.AddControllers(); // requisições http
            builder.Services.AddEndpointsApiExplorer(); // suporte a API
            builder.Services.AddSwaggerGen(); // Swagger

            var app = builder.Build();

            // Middleware entrada de requisições
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

            // Configuração do pipeline HTTP - Ambiente DEV
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Remova a segunda chamada de app.UseRouting()

            app.UseRouting();  // <-- mantém essa só

            // Middleware Prometheus padrão que já captura métricas básicas
            app.UseHttpMetrics();

            // Middleware customizado para contar todas as requisições por status code
            var counter = Metrics.CreateCounter("http_requests_total",
                "Número total de requisições HTTP",
                new CounterConfiguration {
                    LabelNames = new[] { "method", "endpoint", "status_code" }
                });

            app.Use(async (context, next) => {
                await next();

                var path = context.Request.Path.Value;

                // Ignora métricas para swagger e /metrics
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
                endpoints.MapMetrics(); // expõe /metrics para Prometheus
            });


            app.Run();
        }
    }
}
