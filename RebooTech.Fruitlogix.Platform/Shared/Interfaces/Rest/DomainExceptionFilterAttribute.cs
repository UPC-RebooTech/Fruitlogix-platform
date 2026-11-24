using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

/// <summary>
///     Traduce las excepciones de reglas de dominio a respuestas HTTP en vez de 500:
///     <list type="bullet">
///         <item><see cref="InvalidOperationException" /> → 409 (estado no permite la operación).</item>
///         <item><see cref="ArgumentException" /> → 400 (dato inválido).</item>
///         <item><see cref="KeyNotFoundException" /> → 404.</item>
///     </list>
///     Cuerpo: <c>{ "message": "..." }</c>, el mismo formato que ya usan otros endpoints.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DomainExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        IActionResult? result = context.Exception switch
        {
            InvalidOperationException ex => new ConflictObjectResult(new { message = ex.Message }),
            ArgumentException ex => new BadRequestObjectResult(new { message = ex.Message }),
            KeyNotFoundException ex => new NotFoundObjectResult(new { message = ex.Message }),
            _ => null
        };

        if (result is null) return;
        context.Result = result;
        context.ExceptionHandled = true;
    }
}
