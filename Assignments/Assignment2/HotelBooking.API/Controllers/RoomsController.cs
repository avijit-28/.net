namespace HotelBooking.API.Controllers;

using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IAmenityService _amenityService;

    public RoomsController(IRoomService roomService, IAmenityService amenityService)
    {
        _roomService = roomService;
        _amenityService = amenityService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Create([FromBody] CreateRoomDto request)
    {
        var created = await _roomService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.RoomId }, ApiResponse<RoomResponseDto>.Ok(created, "Room created successfully."));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RoomResponseDto>>>> GetAll(
        [FromQuery] int? roomTypeId,
        [FromQuery] string? status,
        [FromQuery] bool includeInactive = false)
    {
        var list = await _roomService.GetAllAsync(roomTypeId, status, includeInactive);
        return Ok(ApiResponse<List<RoomResponseDto>>.Ok(list));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResultDto<RoomSearchResultDto>>>> SearchAvailable([FromQuery] RoomSearchQueryDto query)
    {
        var result = await _roomService.SearchAvailableAsync(query);
        return Ok(ApiResponse<PagedResultDto<RoomSearchResultDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RoomDetailResponseDto>>> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
            return NotFound(ApiResponse<RoomDetailResponseDto>.Fail("Room not found."));

        return Ok(ApiResponse<RoomDetailResponseDto>.Ok(room));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Update(int id, [FromBody] UpdateRoomDto request)
    {
        var updated = await _roomService.UpdateAsync(id, request);
        return Ok(ApiResponse<RoomResponseDto>.Ok(updated, "Room updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] UpdateRoomStatusDto request)
    {
        var success = await _roomService.UpdateStatusAsync(id, request.Status);
        return Ok(ApiResponse<bool>.Ok(success, $"Room status updated to {request.Status}."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
    {
        var success = await _roomService.SoftDeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(success, "Room deactivated successfully."));
    }

    [HttpPost("{roomId:int}/amenities")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<List<AmenityResponseDto>>>> AssignAmenities(int roomId, [FromBody] AssignAmenitiesDto request)
    {
        var list = await _amenityService.AssignAmenitiesToRoomAsync(roomId, request.AmenityIds);
        return Ok(ApiResponse<List<AmenityResponseDto>>.Ok(list, "Amenities assigned successfully."));
    }

    [HttpGet("{roomId:int}/amenities")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<AmenityResponseDto>>>> GetAmenities(int roomId)
    {
        var list = await _amenityService.GetByRoomIdAsync(roomId);
        return Ok(ApiResponse<List<AmenityResponseDto>>.Ok(list));
    }

    [HttpDelete("{roomId:int}/amenities/{amenityId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveAmenity(int roomId, int amenityId)
    {
        var success = await _amenityService.RemoveAmenityFromRoomAsync(roomId, amenityId);
        return Ok(ApiResponse<bool>.Ok(success, "Amenity unmapped successfully."));
    }
}
