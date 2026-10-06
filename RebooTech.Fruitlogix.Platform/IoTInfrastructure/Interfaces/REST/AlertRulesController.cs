using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertRulesController(
    IAlertRuleCommandService commandService,
    IAlertRuleQueryService queryService) : ControllerBase{
    [HttpPost]
    [ProducesResponseType(typeof(AlertRuleResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAlertRule([FromBody] CreateAlertRuleResource resource)
    {
        try
        {
            var command = CreateAlertRuleCommandFromResourceAssembler.ToCommandFromResource(resource);
            var rule = await commandService.Handle(command);
            return StatusCode(201, AlertRuleResourceFromEntityAssembler.ToResourceFromEntity(rule));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertRuleResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAlertRules()
    {
        var query = new GetAllAlertRulesQuery();
        var rules = await queryService.Handle(query);
        var resources = rules.Select(AlertRuleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}