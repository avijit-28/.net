namespace HotelBooking.BAL.Services;

using HotelBooking.BAL.Security;
using HotelBooking.DAL.Repositories;
using HotelBooking.Models.DTOs;
using HotelBooking.Models.Entities;

// -----------------------------------------------------------------------------
// Authentication Service
// -----------------------------------------------------------------------------
public interface IAuthService
{
    Task<UserProfileDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto request);
    Task<UserProfileDto> ToggleStatusAsync(int userId, bool isActive, int adminUserId);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserProfileDto> RegisterAsync(RegisterRequestDto request)
    {
        var (hash, salt) = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = "Customer"
        };

        var created = await _userRepository.RegisterAsync(user, hash, salt);
        return MapProfile(created);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailForLoginAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password, or account is deactivated.");
        }

        bool isValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return MapProfile(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto request)
    {
        var updated = await _userRepository.UpdateProfileAsync(userId, request.FullName, request.PhoneNumber);
        return MapProfile(updated);
    }

    public async Task<UserProfileDto> ToggleStatusAsync(int userId, bool isActive, int adminUserId)
    {
        var updated = await _userRepository.ToggleStatusAsync(userId, isActive, adminUserId);
        return MapProfile(updated);
    }

    private static UserProfileDto MapProfile(User user) => new()
    {
        UserId = user.UserId,
        FullName = user.FullName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Role = user.Role,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}

// -----------------------------------------------------------------------------
// Room Type Service
// -----------------------------------------------------------------------------
public interface IRoomTypeService
{
    Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto);
    Task<List<RoomTypeResponseDto>> GetAllAsync(bool includeInactive = false);
    Task<RoomTypeResponseDto?> GetByIdAsync(int id);
    Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto);
    Task<bool> SoftDeleteAsync(int id);
}

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _repository;

    public RoomTypeService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeDto dto)
    {
        var entity = new RoomType
        {
            TypeName = dto.TypeName,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            MaxOccupancy = dto.MaxOccupancy
        };
        var created = await _repository.CreateAsync(entity);
        return MapDto(created);
    }

    public async Task<List<RoomTypeResponseDto>> GetAllAsync(bool includeInactive = false)
    {
        var list = await _repository.GetAllAsync(includeInactive);
        return list.Select(MapDto).ToList();
    }

    public async Task<RoomTypeResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MapDto(entity);
    }

    public async Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeDto dto)
    {
        var entity = new RoomType
        {
            RoomTypeId = id,
            TypeName = dto.TypeName,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            MaxOccupancy = dto.MaxOccupancy,
            IsActive = dto.IsActive
        };
        var updated = await _repository.UpdateAsync(entity);
        return MapDto(updated);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        return await _repository.SoftDeleteAsync(id);
    }

    private static RoomTypeResponseDto MapDto(RoomType e) => new()
    {
        RoomTypeId = e.RoomTypeId,
        TypeName = e.TypeName,
        Description = e.Description,
        BasePrice = e.BasePrice,
        MaxOccupancy = e.MaxOccupancy,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}

// -----------------------------------------------------------------------------
// Room Inventory & Search Service
// -----------------------------------------------------------------------------
public interface IRoomService
{
    Task<RoomResponseDto> CreateAsync(CreateRoomDto dto);
    Task<List<RoomResponseDto>> GetAllAsync(int? roomTypeId = null, string? status = null, bool includeInactive = false);
    Task<RoomDetailResponseDto?> GetByIdAsync(int roomId);
    Task<RoomResponseDto> UpdateAsync(int roomId, UpdateRoomDto dto);
    Task<bool> UpdateStatusAsync(int roomId, string status);
    Task<bool> SoftDeleteAsync(int roomId);
    Task<PagedResultDto<RoomSearchResultDto>> SearchAvailableAsync(RoomSearchQueryDto query);
}

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repository;

    public RoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public Task<RoomResponseDto> CreateAsync(CreateRoomDto dto) => _repository.CreateAsync(dto);
    public Task<List<RoomResponseDto>> GetAllAsync(int? roomTypeId = null, string? status = null, bool includeInactive = false) => _repository.GetAllAsync(roomTypeId, status, includeInactive);
    public Task<RoomDetailResponseDto?> GetByIdAsync(int roomId) => _repository.GetByIdAsync(roomId);
    public Task<RoomResponseDto> UpdateAsync(int roomId, UpdateRoomDto dto) => _repository.UpdateAsync(roomId, dto);
    public Task<bool> UpdateStatusAsync(int roomId, string status) => _repository.UpdateStatusAsync(roomId, status);
    public Task<bool> SoftDeleteAsync(int roomId) => _repository.SoftDeleteAsync(roomId);
    public Task<PagedResultDto<RoomSearchResultDto>> SearchAvailableAsync(RoomSearchQueryDto query) => _repository.SearchAvailableAsync(query);
}

// -----------------------------------------------------------------------------
// Amenity Service
// -----------------------------------------------------------------------------
public interface IAmenityService
{
    Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto);
    Task<List<AmenityResponseDto>> GetAllAsync(bool includeInactive = false);
    Task<AmenityResponseDto?> GetByIdAsync(int id);
    Task<AmenityResponseDto> UpdateAsync(int id, UpdateAmenityDto dto);
    Task<bool> SoftDeleteAsync(int id);
    Task<List<AmenityResponseDto>> AssignAmenitiesToRoomAsync(int roomId, List<int> amenityIds);
    Task<List<AmenityResponseDto>> GetByRoomIdAsync(int roomId);
    Task<bool> RemoveAmenityFromRoomAsync(int roomId, int amenityId);
}

public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _repository;

    public AmenityService(IAmenityRepository repository)
    {
        _repository = repository;
    }

    public Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto) => _repository.CreateAsync(dto);
    public Task<List<AmenityResponseDto>> GetAllAsync(bool includeInactive = false) => _repository.GetAllAsync(includeInactive);
    public Task<AmenityResponseDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<AmenityResponseDto> UpdateAsync(int id, UpdateAmenityDto dto) => _repository.UpdateAsync(id, dto);
    public Task<bool> SoftDeleteAsync(int id) => _repository.SoftDeleteAsync(id);
    public Task<List<AmenityResponseDto>> AssignAmenitiesToRoomAsync(int roomId, List<int> amenityIds) => _repository.AssignAmenitiesToRoomAsync(roomId, amenityIds);
    public Task<List<AmenityResponseDto>> GetByRoomIdAsync(int roomId) => _repository.GetByRoomIdAsync(roomId);
    public Task<bool> RemoveAmenityFromRoomAsync(int roomId, int amenityId) => _repository.RemoveAmenityFromRoomAsync(roomId, amenityId);
}

// -----------------------------------------------------------------------------
// Booking Service
// -----------------------------------------------------------------------------
public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(int userId, CreateBookingRequestDto dto);
    Task<BookingDetailDto?> GetByIdAsync(int bookingId);
    Task<BookingDetailDto?> GetByReferenceAsync(string bookingReference);
    Task<List<BookingResponseDto>> GetByUserAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<PagedResultDto<BookingDetailDto>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 20);
}

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public Task<BookingResponseDto> CreateAsync(int userId, CreateBookingRequestDto dto) => _repository.CreateAsync(userId, dto);
    public Task<BookingDetailDto?> GetByIdAsync(int bookingId) => _repository.GetByIdAsync(bookingId);
    public Task<BookingDetailDto?> GetByReferenceAsync(string bookingReference) => _repository.GetByReferenceAsync(bookingReference);
    public Task<List<BookingResponseDto>> GetByUserAsync(int userId, int pageNumber = 1, int pageSize = 10) => _repository.GetByUserAsync(userId, pageNumber, pageSize);
    public Task<PagedResultDto<BookingDetailDto>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 20) => _repository.GetAllAsync(status, startDate, endDate, pageNumber, pageSize);
}

// -----------------------------------------------------------------------------
// Cancellation Service
// -----------------------------------------------------------------------------
public interface ICancellationService
{
    Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason);
    Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId);
}

public class CancellationService : ICancellationService
{
    private readonly ICancellationRepository _repository;

    public CancellationService(ICancellationRepository repository)
    {
        _repository = repository;
    }

    public Task<CancellationResponseDto> CancelBookingAsync(int bookingId, int cancelledByUserId, string? reason) => _repository.CancelBookingAsync(bookingId, cancelledByUserId, reason);
    public Task<CancellationResponseDto?> GetDetailsByBookingIdAsync(int bookingId) => _repository.GetDetailsByBookingIdAsync(bookingId);
}
