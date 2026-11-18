namespace HotelBooking.API.Controllers;

using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Manages room categories, occupancy limits, and base nightly rate pricing tiers.
/// </summary>
[ApiController]
[Route("api/room-types")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _service;

    public RoomTypesController(IRoomTypeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates a new room type. Restricted to Administrators only.
    /// </summary>
    /// <param name="request">Room type specifications including name, base price, and max occupancy.</param>
    /// <returns>Created room type details with HTTP 201 Created.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponseDto>>> Create([FromBody] CreateRoomTypeDto request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.RoomTypeId }, ApiResponse<RoomTypeResponseDto>.Ok(created, "Room type created successfully."));
    }

    /// <summary>
    /// Retrieves all room types. Publicly accessible.
    /// </summary>
    /// <param name="includeInactive">Optional flag to include inactive room types (for Admin views).</param>
    /// <returns>Collection of room types.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RoomTypeResponseDto>>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var list = await _service.GetAllAsync(includeInactive);
        return Ok(ApiResponse<List<RoomTypeResponseDto>>.Ok(list));
    }

    /// <summary>
    /// Retrieves a single room type by ID.
    /// </summary>
    /// <param name="id">Room type identifier.</param>
    /// <returns>Room type details.</returns>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RoomTypeResponseDto>>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound(ApiResponse<RoomTypeResponseDto>.Fail("Room type not found."));

        return Ok(ApiResponse<RoomTypeResponseDto>.Ok(item));
    }

    /// <summary>
    /// Updates room type pricing, occupancy, description, or active status. Restricted to Administrators only.
    /// </summary>
    /// <param name="id">Room type identifier to update.</param>
    /// <param name="request">Updated room type data.</param>
    /// <returns>Updated room type record.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponseDto>>> Update(int id, [FromBody] UpdateRoomTypeDto request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<RoomTypeResponseDto>.Ok(updated, "Room type updated successfully."));
    }

    /// <summary>
    /// Soft-deletes (deactivates) a room type. Restricted to Administrators only.
    /// </summary>
    /// <param name="id">Room type identifier to deactivate.</param>
    /// <returns>Boolean indicating successful deactivation.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
    {
        var success = await _service.SoftDeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(success, "Room type deactivated successfully."));
    }
}
