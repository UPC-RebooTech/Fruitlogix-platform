using RebooTech.Fruitlogix.Platform.Iam.Application.Internal.OutboundServices;
using RebooTech.Fruitlogix.Platform.Iam.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

/**
 * RequestAuthorizationMiddleware is a custom middleware.
 * This middleware is used to authorize requests.
 * It validates a token is included in the request header and that the token is valid.
 * If the token is valid then it sets the user in HttpContext.Items["User"].
 */
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /**
     * InvokeAsync is called by the ASP.NET Core runtime.
     * It is used to authorize requests.
     * It validates a token is included in the request header and that the token is valid.
     * If the token is valid then it sets the user in HttpContext.Items["User"].
     */
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        var cancellationToken = context.RequestAborted;

        var endpoint = context.GetEndpoint();

        // Si no hay endpoint resuelto (ruta no mapeada a ningún controlador:
        // "/", favicon, assets de Swagger, etc.), simplemente deja pasar.
        if (endpoint is null)
        {
            await next(context);
            return;
        }

        // skip authorization if endpoint is decorated with [AllowAnonymous] attribute
        var allowAnonymous = endpoint.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute));

        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        // get token from request header
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrWhiteSpace(token))
        {
            await WriteUnauthorizedAsync(context, "Missing or malformed bearer token.");
            return;
        }

        var userId = await tokenService.ValidateToken(token);
        if (userId == null)
        {
            await WriteUnauthorizedAsync(context, "Invalid or expired token.");
            return;
        }

        var getUserByIdQuery = new GetUserByIdQuery(userId.Value);
        var user = await userQueryService.Handle(getUserByIdQuery, cancellationToken);
        if (user == null)
        {
            await WriteUnauthorizedAsync(context, "The token user no longer exists.");
            return;
        }

        context.Items["User"] = user;

        await next(context);
    }

    /// <summary>Responde 401 (ProblemDetails) en vez de lanzar una excepción que termina en 500.</summary>
    private static async Task WriteUnauthorizedAsync(HttpContext context, string detail)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.WWWAuthenticate = "Bearer";
        await context.Response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = detail
        }, options: null, contentType: "application/problem+json");
    }
}
