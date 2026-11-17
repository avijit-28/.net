namespace HotelBooking.API.Controllers;

using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/room-types")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _service;

    public RoomTypesController(IRoomTypeService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponseDto>>> Create([FromBody] CreateRoomTypeDto request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.RoomTypeId }, ApiResponse<RoomTypeResponseDto>.Ok(created, "Room type created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RoomTypeResponseDto>>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var list = await _service.GetAllAsync(includeInactive);
        return Ok(ApiResponse<List<RoomTypeResponseDto>>.Ok(list));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RoomTypeResponseDto>>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound(ApiResponse<RoomTypeResponseDto>.Fail("Room type not found."));

        return Ok(ApiResponse<RoomTypeResponseDto>.Ok(item));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponseDto>>> Update(int id, [FromBody] UpdateRoomTypeDto request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<RoomTypeResponseDto>.Ok(updated, "Room type updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
    {
        var success = await _service.SoftDeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(success, "Room type deactivated successfully."));
    }
}
