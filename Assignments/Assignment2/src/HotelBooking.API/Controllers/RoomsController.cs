namespace HotelBooking.API.Controllers;

using HotelBooking.BAL.Services;
using HotelBooking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Manages physical hotel rooms, availability search with overlap detection, status transitions, and amenity assignments.
/// </summary>
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

    /// <summary>
    /// Creates a new physical room. Enforces unique RoomNumber. Restricted to Administrators.
    /// </summary>
    /// <param name="request">Room creation payload (number, floor, room type).</param>
    /// <returns>Created room record with HTTP 201 Created.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Create([FromBody] CreateRoomDto request)
    {
        var created = await _roomService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.RoomId }, ApiResponse<RoomResponseDto>.Ok(created, "Room created successfully."));
    }

    /// <summary>
    /// Retrieves all physical rooms, joined with their RoomType metadata.
    /// </summary>
    /// <param name="roomTypeId">Optional filter by room category.</param>
    /// <param name="status">Optional filter by room status (Available, Occupied, UnderMaintenance).</param>
    /// <param name="includeInactive">Optional flag to include inactive rooms.</param>
    /// <returns>List of room inventory records.</returns>
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

    /// <summary>
    /// High-performance multi-criteria room search with relational division on amenities and date overlap detection.
    /// </summary>
    /// <param name="query">Search criteria (checkIn, checkOut, guests, price bounds, required amenities).</param>
    /// <returns>Paginated room search results with calculated TotalStayPrice.</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResultDto<RoomSearchResultDto>>>> SearchAvailable([FromQuery] RoomSearchQueryDto query)
    {
        var result = await _roomService.SearchAvailableAsync(query);
        return Ok(ApiResponse<PagedResultDto<RoomSearchResultDto>>.Ok(result));
    }

    /// <summary>
    /// Retrieves detailed room specifications including mapped amenities by ID.
    /// </summary>
    /// <param name="id">Room identifier.</param>
    /// <returns>Room details with amenity list.</returns>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RoomDetailResponseDto>>> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
            return NotFound(ApiResponse<RoomDetailResponseDto>.Fail("Room not found."));

        return Ok(ApiResponse<RoomDetailResponseDto>.Ok(room));
    }

    /// <summary>
    /// Modifies room details (room number, floor number, room type). Restricted to Administrators.
    /// </summary>
    /// <param name="id">Room identifier to update.</param>
    /// <param name="request">Updated room properties.</param>
    /// <returns>Updated room record.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Update(int id, [FromBody] UpdateRoomDto request)
    {
        var updated = await _roomService.UpdateAsync(id, request);
        return Ok(ApiResponse<RoomResponseDto>.Ok(updated, "Room updated successfully."));
    }

    /// <summary>
    /// Updates operational room status (Available, Occupied, UnderMaintenance). Restricted to Administrators.
    /// </summary>
    /// <param name="id">Room identifier.</param>
    /// <param name="request">New status payload.</param>
    /// <returns>Boolean indicating success.</returns>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] UpdateRoomStatusDto request)
    {
        var success = await _roomService.UpdateStatusAsync(id, request.Status);
        return Ok(ApiResponse<bool>.Ok(success, $"Room status updated to {request.Status}."));
    }

    /// <summary>
    /// Soft-deletes a room from active inventory. Restricted to Administrators.
    /// </summary>
    /// <param name="id">Room identifier.</param>
    /// <returns>Boolean indicating success.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(int id)
    {
        var success = await _roomService.SoftDeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(success, "Room deactivated successfully."));
    }

    /// <summary>
    /// Bulk assigns amenities to a specific room. Restricted to Administrators.
    /// </summary>
    /// <param name="roomId">Target room ID.</param>
    /// <param name="request">List of amenity IDs to map.</param>
    /// <returns>Updated list of mapped amenities.</returns>
    [HttpPost("{roomId:int}/amenities")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<List<AmenityResponseDto>>>> AssignAmenities(int roomId, [FromBody] AssignAmenitiesDto request)
    {
        var list = await _amenityService.AssignAmenitiesToRoomAsync(roomId, request.AmenityIds);
        return Ok(ApiResponse<List<AmenityResponseDto>>.Ok(list, "Amenities assigned successfully."));
    }

    /// <summary>
    /// Retrieves all active amenities mapped to a specific room.
    /// </summary>
    /// <param name="roomId">Target room ID.</param>
    /// <returns>List of mapped amenities.</returns>
    [HttpGet("{roomId:int}/amenities")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<AmenityResponseDto>>>> GetRoomAmenities(int roomId)
    {
        var list = await _amenityService.GetByRoomIdAsync(roomId);
        return Ok(ApiResponse<List<AmenityResponseDto>>.Ok(list));
    }

    /// <summary>
    /// Removes a single amenity mapping from a specific room. Restricted to Administrators.
    /// </summary>
    /// <param name="roomId">Target room ID.</param>
    /// <param name="amenityId">Amenity ID to unmap.</param>
    /// <returns>Boolean indicating removal success.</returns>
    [HttpDelete("{roomId:int}/amenities/{amenityId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveAmenity(int roomId, int amenityId)
    {
        var success = await _amenityService.RemoveAmenityFromRoomAsync(roomId, amenityId);
        return Ok(ApiResponse<bool>.Ok(success, "Amenity removed from room."));
    }
}
