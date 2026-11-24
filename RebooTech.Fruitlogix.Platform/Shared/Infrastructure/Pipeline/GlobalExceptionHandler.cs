using Microsoft.AspNetCore.Diagnostics;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Pipeline;

/// <summary>
///     Última red para excepciones no controladas: responde 500 con ProblemDetails
///     (<c>application/problem+json</c>) en vez de un body vacío.
///     <list type="bullet">
///         <item>
///             <c>detail</c> trae el tipo y el mensaje de la excepción en Development, o si
///             <c>ErrorHandling:IncludeExceptionDetails</c> = true. En Production el mensaje es genérico,
///             para no exponer datos internos (SQL, rutas, etc.).
///         </item>
///         <item><c>traceId</c> siempre va en el body y en el log, para cruzar el error.</item>
///     </list>
/// </summary>
public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    IConfiguration configuration,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        logger.LogError(exception, "Unhandled exception on {Method} {Path} (traceId {TraceId})",
            httpContext.Request.Method, httpContext.Request.Path, traceId);

        var includeDetails = environment.IsDevelopment()
                             || configuration.GetValue<bool>("ErrorHandling:IncludeExceptionDetails");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected server error",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Detail = includeDetails
                    ? $"{exception.GetType().Name}: {exception.Message}"
                    : "An unexpected error occurred. Report the traceId to the backend team.",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = { ["traceId"] = traceId }
            }
        });
    }
}
