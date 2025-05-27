
using CadastroService.Services;

namespace CadastroService {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configuração para o cliente HTTP que vai chamar o PersistenciaService
            builder.Services.AddHttpClient("PersistenciaService", client => {
                client.BaseAddress = new Uri("http://localhost:5006"); // URL do PersistenciaService
            });
            builder.Services.AddScoped<IPersistenciaServiceClient, PersistenciaServiceClient>();



            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
