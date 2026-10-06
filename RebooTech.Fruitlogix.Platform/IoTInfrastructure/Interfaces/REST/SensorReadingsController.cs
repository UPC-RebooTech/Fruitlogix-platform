using System.Net.Mime;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Commands;
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
public class SensorReadingsController(
    ISensorReadingCommandService commandService,
    ISensorReadingQueryService queryService) : ControllerBase{
    [HttpPost]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterReading([FromBody] RegisterSensorReadingResource resource)
    {
        try
        {
            var command = RegisterSensorReadingCommandFromResourceAssembler.ToCommandFromResource(resource);
            var reading = await commandService.Handle(command);
            if (reading is null) return NotFound("Device not found.");
            return StatusCode(201, SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(reading));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReadings()
    {
        var query = new GetAllSensorReadingsQuery();
        var readings = await queryService.Handle(query);
        var resources = readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
    
    [HttpGet("device/{deviceId:int}")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReadingsByDeviceId(int deviceId)
    {
        var query = new GetSensorReadingsByDeviceIdQuery(deviceId);
        var readings = await queryService.Handle(query);
        var resources = readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}