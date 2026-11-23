using Microsoft.AspNetCore.Mvc;
using IamUser = RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates.User;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class MembershipsController(IMembershipQueryService membershipQueryService) : ControllerBase
{
    /// <summary>
    ///     Plan del usuario autenticado (JWT). La app móvil lo consulta después del
    ///     Payment Sheet para saber si el webhook ya activó "Miembro Pro".
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(MembershipResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyMembership()
    {
        if (HttpContext.Items["User"] is not IamUser user) return Unauthorized();

        var membership = await membershipQueryService.Handle(new GetMembershipByUserIdQuery(user.Id));
        return Ok(membership is null
            ? MembershipResourceFromEntityAssembler.Free(user.Id)
            : MembershipResourceFromEntityAssembler.ToResourceFromEntity(membership));
    }
}
