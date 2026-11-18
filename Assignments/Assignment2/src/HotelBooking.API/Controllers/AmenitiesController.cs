namespace HotelBooking.API.Controllers;

using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Manages the master amenities catalog (Wi-Fi, Jacuzzi, Balcony, etc.).
/// </summary>
[ApiController]
[Route("api/amenities")]
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _service;

    public AmenitiesController(IAmenityService service)
    {
        _service = service;
    }

    /// <summary>
    /// Adds a new amenity to the master catalog. Restricted to Administrators.
    /// </summary>
    /// <param name="request">Amenity definition (name, description, icon code).</param>
    /// <returns>Created amenity with HTTP 201 Created.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<AmenityResponseDto>>> Create([FromBody] CreateAmenityDto request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.AmenityId }, ApiResponse<AmenityResponseDto>.Ok(created, "Amenity created successfully."));
    }

    /// <summary>
    /// Retrieves all active amenities. Publicly accessible.
    /// </summary>
    /// <param name="includeInactive">Optional flag to include deactivated amenities.</param>
    /// <returns>List of amenities.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<AmenityResponseDto>>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var list = await _service.GetAllAsync(includeInactive);
        return Ok(ApiResponse<List<AmenityResponseDto>>.Ok(list));
    }

    /// <summary>
    /// Retrieves a single amenity by ID.
    /// </summary>
    /// <param name="id">Amenity identifier.</param>
    /// <returns>Amenity record.</returns>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AmenityResponseDto>>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound(ApiResponse<AmenityResponseDto>.Fail("Amenity not found."));

        return Ok(ApiResponse<AmenityResponseDto>.Ok(item));
    }

    /// <summary>
    /// Modifies an existing amenity's name, description, or icon code. Restricted to Administrators.
    /// </summary>
    /// <param name="id">Amenity identifier to update.</param>
    /// <param name="request">Updated amenity properties.</param>
    /// <returns>Updated amenity record.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<AmenityResponseDto>>> Update(int id, [FromBody] UpdateAmenityDto request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<AmenityResponseDto>.Ok(updated, "Amenity updated successfully."));
    }

    /// <summary>
    /// Soft-deletes (deactivates) an amenity. Restricted to Administrators.
    /// </summary>
    /// <param name="id">Amenity identifier.</param>
    /// <returns>Boolean indicating success.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
    {
        var success = await _service.SoftDeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(success, "Amenity deactivated successfully."));
    }
}
