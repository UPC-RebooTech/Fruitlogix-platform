using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RebooTech.Fruitlogix.Platform.Iam.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Resources;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Transform;
using RebooTech.Fruitlogix.Platform.Resources.Errors;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
// For ProblemDetailsFactory

// For IamError enum

namespace RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;


[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available User endpoints")]
public class UsersController(
    IUserQueryService userQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer, // Renamed for clarity
    ProblemDetailsFactory problemDetailsFactory) // Inject ProblemDetailsFactory
    : ControllerBase
{
    private readonly IStringLocalizer<ErrorMessages> _errorLocalizer = errorLocalizer;
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    /**
     * <summary>
     *     Get user by id endpoint. It allows to get a user by id
     * </summary>
     * <param name="id">The user id</param>
     * <param name="cancellationToken">The cancellation token.</param>
     * <returns>The user resource</returns>
     */
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get a user by its id",
        Description = "Get a user by its id",
        OperationId = "GetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was found", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The user was not found")]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        // Quien no es Admin ni Distributor solo se ve a sí mismo; un id ajeno responde 404.
        var canSeeOthers = me.IsAdmin || me.IsDistributor;
        var user = canSeeOthers || me.Id == id
            ? await userQueryService.Handle(new GetUserByIdQuery(id), cancellationToken)
            : null;

        return IamActionResultAssembler.ToActionResultFromGetUserByIdResult(
            this,
            user,
            _errorLocalizer,
            _problemDetailsFactory,
            foundUser => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(foundUser))
        );
    }

    /**
     * <summary>
     *     Get all users' endpoint. It allows getting all users
     * </summary>
     * <returns>The user resources</returns>
     */
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Get all users",
        OperationId = "GetAllUsers")]
    [SwaggerResponse(StatusCodes.Status200OK, "The users were found", typeof(IEnumerable<UserResource>))]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var getAllUsersQuery = new GetAllUsersQuery();
        var users = await userQueryService.Handle(getAllUsersQuery, cancellationToken);
        var visibleUsers = me.IsAdmin || me.IsDistributor ? users : users.Where(u => u.Id == me.Id);
        var userResources = visibleUsers.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(userResources);
    }
}