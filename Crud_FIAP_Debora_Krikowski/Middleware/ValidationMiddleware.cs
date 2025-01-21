using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Crud_FIAP_Debora_Krikowski.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Valida o Content-Type para JSON
            if (!context.Request.ContentType?.Contains("application/json") ?? false)
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "Content-Type deve ser 'application/json'."
                }));
                return;
            }

            // Continua para o próximo middleware no pipeline
            await _next(context);
        }
    }
}
