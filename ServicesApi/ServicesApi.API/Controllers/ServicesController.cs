using Microsoft.AspNetCore.Mvc;
using ServicesApi.Application.Dto.Services;
using ServicesApi.Application.Dto.Shared;
using ServicesApi.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicesApi.API.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
public sealed class ServicesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ServicesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Adds a new service",
        Description = "Registers a new service with the specified details",
        OperationId = "AddService"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Service was created successfully", typeof(ServiceDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No such service category or specialization exists")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A service with such name already exists.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> AddService([FromBody] AddServiceDto createServiceDto, CancellationToken ct = default)
    {
        var result = await _serviceManager.CreateServiceAsync(createServiceDto, ct);
        return Created($"/services/{result.Id}", result);
    }
    
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a service",
        Description = "Permanently removes a service by its unique identifier.",
        OperationId = "DeleteService"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Service was successfully deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No such service category or specialization exists")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> DeleteService([FromRoute] Guid id, CancellationToken ct = default)
    {
        await _serviceManager.DeleteServiceAsync(id, ct);
        return NoContent();
    }
    
    [HttpPut]
    [SwaggerOperation(
        Summary = "Edits a service",
        Description = "Edits a service specified details",
        OperationId = "EditService"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service was successfully edited", typeof(ServiceDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No such service category or specialization exists")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A service with such name already exists.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> UpdateService([FromBody] UpdateServiceDto updateServiceDto, CancellationToken ct = default)
    {
        var editedService = await _serviceManager.UpdateServiceAsync(updateServiceDto, ct);
        return Ok(editedService);
    }
    
    [HttpGet("{serviceId:guid}")]
    [SwaggerOperation(
        Summary = "Gets a service by ID",
        Description = "Retrieves detailed information for a specific service using its unique identifier",
        OperationId = "GetServiceById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service retrieved successfully", typeof(ServiceDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No such service category or specialization exists")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServiceById([FromRoute] Guid serviceId, CancellationToken ct = default)
    {
        var service = await _serviceManager.GetServiceByIdAsync(serviceId, ct);
        return Ok(service);
    }
    
    [HttpGet("category/{categoryId:guid}")]
    [SwaggerOperation(
        Summary = "Gets services by category ID",
        Description = "Retrieves detailed information for a specific services using its category unique identifier",
        OperationId = "GetServicesByCategoryId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Services retrieved successfully", typeof(IEnumerable<ServiceDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServicesByCategoryId([FromRoute] Guid categoryId, CancellationToken ct = default)
    {
        var services = await _serviceManager.GetServicesByCategoryIdAsync(categoryId, ct);
        return Ok(services);
    }
    
    [HttpGet("specialization/{specializationId:guid}")]
    [SwaggerOperation(
        Summary = "Gets services by specialization ID",
        Description = "Retrieves detailed information for specific services using its specialization unique identifier",
        OperationId = "GetServicesBySpecializationId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Services retrieved successfully", typeof(IEnumerable<ServiceDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServiceBySpecializationId([FromRoute] Guid specializationId, CancellationToken ct = default)
    {
        var service = await _serviceManager.GetServicesBySpecializationIdAsync(specializationId, ct);
        return Ok(service);
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all services",
        Description = "Retrieves detailed information all services",
        OperationId = "GetServices"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Services retrieved successfully", typeof(IEnumerable<ServiceDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServices(CancellationToken ct = default)
    {
        var services = await _serviceManager.GetAllServicesAsync(ct);
        return Ok(services);
    }
    
    [HttpPost("searchByTerm")]
    [SwaggerOperation(
        Summary = "Gets services by term",
        Description = "Retrieves detailed information for services using search term",
        OperationId = "GetServicesByTerm"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Services retrieved successfully", typeof(IEnumerable<ServiceDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServicesByTerm([FromBody] SearchByTermDto termDto, CancellationToken ct = default)
    {
        var services = await _serviceManager.GetServicesByTermAsync(termDto, ct);
        return Ok(services);
    }
    
    [HttpPost("search/paged")]
    [SwaggerOperation(
        Summary = "Gets a paged list of services",
        Description = "Retrieves a paginated and filtered list of services based on search parameters.",
        OperationId = "GetServicesPaged"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Paged list of services retrieved successfully", typeof(PagedResult<ServiceDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid search or filter parameters")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServicesPaged(
        [FromBody] GetPagedServicesDto getPagedServicesDto, CancellationToken ct = default)
    {
        var pagedServices = await _serviceManager.GetServicesPagedAsync(getPagedServicesDto, ct);
        return Ok(pagedServices);
    }
}