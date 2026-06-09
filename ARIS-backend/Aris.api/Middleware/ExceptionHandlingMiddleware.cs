using System.Net;
using System.Text.Json;

namespace Aris.api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            var message = ex.Message.Trim();

            if (message.Equals("Email already exists.", StringComparison.OrdinalIgnoreCase))
            {
                await WriteErrorAsync(context, HttpStatusCode.Conflict, "Já existe uma conta com esse e-mail.");
                return;
            }

            if (message.Equals("Email or password invalid.", StringComparison.OrdinalIgnoreCase))
            {
                await WriteErrorAsync(context, HttpStatusCode.Unauthorized, "E-mail ou senha inválidos.");
                return;
            }

            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new
        {
            timestamp = DateTime.UtcNow,
            status = (int)statusCode,
            error = statusCode.ToString(),
            message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
