using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;
using IamUser = RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates.User;

namespace RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;

/// <summary>
///     Usuario autenticado de la petición, tomado del JWT validado por
///     <c>RequestAuthorizationMiddleware</c> (HttpContext.Items["User"]).
///     Úsalo para autorizar: nunca confíes en ids de la URL sin compararlos con este.
/// </summary>
public sealed record CurrentUser(int Id, UserRole Role, string Username = "")
{
    public bool IsAdmin => Role == UserRole.Admin;
    public bool IsCommercialClient => Role == UserRole.CommercialClient;
    public bool IsProducer => Role == UserRole.Producer;
    public bool IsDistributor => Role == UserRole.Distributor;

    /// <summary>El usuario es el mismo que <paramref name="userId" /> o es Admin.</summary>
    public bool IsSelfOrAdmin(int userId) => IsAdmin || Id == userId;

    public static CurrentUser? From(HttpContext httpContext) =>
        httpContext.Items["User"] is IamUser user ? new CurrentUser(user.Id, user.Role, user.Username) : null;
}

public static class CurrentUserControllerExtensions
{
    /// <summary>
    ///     Usuario autenticado. En endpoints no anónimos el middleware ya garantiza que exista;
    ///     devuelve null solo en endpoints [AllowAnonymous].
    /// </summary>
    public static CurrentUser? GetCurrentUser(this ControllerBase controller) =>
        CurrentUser.From(controller.HttpContext);
}
