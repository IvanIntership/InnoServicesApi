using Microsoft.AspNetCore.Mvc;
using ServicesApi.Application.Dto.ServiceCategories;
using ServicesApi.Application.Dto.Shared;
using ServicesApi.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicesApi.API.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
public sealed class ServiceCategoriesController : ControllerBase
{
    private readonly IServiceCategoryManager _serviceCategoryManager;

    public ServiceCategoriesController(IServiceCategoryManager serviceCategoryManager)
    {
        _serviceCategoryManager = serviceCategoryManager ?? throw new ArgumentNullException(nameof(serviceCategoryManager));
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Adds a new service category",
        Description = "Registers a new service category with the specified details",
        OperationId = "AddServiceCategory"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Category was created successfully", typeof(ServiceCategoryDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> AddServiceCategory([FromBody] AddServiceCategoryDto createServiceCategoryDto, CancellationToken ct = default)
    {
        var result = await _serviceCategoryManager.CreateServiceCategoryAsync(createServiceCategoryDto, ct);
        return Created($"/serviceCategories/{result.Id}", result);
    }
    
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a service category",
        Description = "Permanently removes a service category by its unique identifier.",
        OperationId = "DeleteServiceCategory"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Service category was successfully deleted")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> DeleteServiceCategory([FromRoute] Guid id, CancellationToken ct = default)
    {
        await _serviceCategoryManager.DeleteServiceCategoryAsync(id, ct);
        return NoContent();
    }
    
    [HttpPut]
    [SwaggerOperation(
        Summary = "Edits a service category",
        Description = "Edits a service category specified details",
        OperationId = "EditServiceCategory"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service category was successfully edited", typeof(ServiceCategoryDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or parameters")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> UpdateServiceCategory([FromBody] UpdateServiceCategoryDto updateServiceCategoryDto, CancellationToken ct = default)
    {
        var editedServiceCategory = await _serviceCategoryManager.UpdateServiceCategoryAsync(updateServiceCategoryDto, ct);
        return Ok(editedServiceCategory);
    }
    
    [HttpGet("{serviceCategoryId:guid}")]
    [SwaggerOperation(
        Summary = "Gets a service category by ID",
        Description = "Retrieves detailed information for a specific service category using its unique identifier",
        OperationId = "GetServiceCategoryById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service category retrieved successfully", typeof(ServiceCategoryDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServiceCategoryById([FromRoute] Guid serviceCategoryId, CancellationToken ct = default)
    {
        var serviceCategory = await _serviceCategoryManager.GetServiceCategoryByIdAsync(serviceCategoryId, ct);
        return Ok(serviceCategory);
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all service categories",
        Description = "Retrieves detailed information all service categories",
        OperationId = "GetServiceCategories"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service categories retrieved successfully", typeof(IEnumerable<ServiceCategoryDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServiceCategories(CancellationToken ct = default)
    {
        var serviceCategories = await _serviceCategoryManager.GetAllServiceCategoriesAsync(ct);
        return Ok(serviceCategories);
    }
    
    [HttpPost("searchByTerm")]
    [SwaggerOperation(
        Summary = "Gets service categories by term",
        Description = "Retrieves detailed information for service categories using search term",
        OperationId = "GetServiceCategoriesByTerm"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service categories retrieved successfully", typeof(IEnumerable<ServiceCategoryDto>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal service error")]
    public async Task<IActionResult> GetServiceCategoryByTerm([FromBody] SearchByTermDto termDto, CancellationToken ct = default)
    {
        var serviceCategories = await _serviceCategoryManager.GetServiceCategoriesByTermAsync(termDto, ct);
        return Ok(serviceCategories);
    }
}