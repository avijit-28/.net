namespace HotelBooking.API.Controllers;

using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/amenities")]
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _service;

    public AmenitiesController(IAmenityService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<AmenityResponseDto>>> Create([FromBody] CreateAmenityDto request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.AmenityId }, ApiResponse<AmenityResponseDto>.Ok(created, "Amenity created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<AmenityResponseDto>>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var list = await _service.GetAllAsync(includeInactive);
        return Ok(ApiResponse<List<AmenityResponseDto>>.Ok(list));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AmenityResponseDto>>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound(ApiResponse<AmenityResponseDto>.Fail("Amenity not found."));

        return Ok(ApiResponse<AmenityResponseDto>.Ok(item));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<AmenityResponseDto>>> Update(int id, [FromBody] UpdateAmenityDto request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<AmenityResponseDto>.Ok(updated, "Amenity updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
    {
        var success = await _service.SoftDeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(success, "Amenity deactivated successfully."));
    }
}
