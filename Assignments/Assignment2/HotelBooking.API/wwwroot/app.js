// Grand Azure Hotel - Enterprise SPA Application Script

const API_BASE = '/api';

// State Management
let currentUser = null;
let currentToken = null;
let cachedRoomTypes = [];
let cachedAmenities = [];
let activeSearchParams = null;

// Initialize Application
document.addEventListener('DOMContentLoaded', () => {
  initDates();
  loadStoredAuth();
  loadRoomTypes();
  loadAmenities();
  setupEventListeners();

  // Run initial room search
  triggerSearch();
});

// Setup Default Dates (Check-in = Tomorrow, Check-out = Day after Tomorrow)
function initDates() {
  const today = new Date();
  const tomorrow = new Date(today);
  tomorrow.setDate(tomorrow.getDate() + 1);

  const dayAfter = new Date(tomorrow);
  dayAfter.setDate(dayAfter.getDate() + 2);

  const formatDate = (d) => d.toISOString().split('T')[0];

  const checkInInput = document.getElementById('checkInDate');
  const checkOutInput = document.getElementById('checkOutDate');

  checkInInput.min = formatDate(today);
  checkInInput.value = formatDate(tomorrow);

  checkOutInput.min = formatDate(tomorrow);
  checkOutInput.value = formatDate(dayAfter);

  checkInInput.addEventListener('change', () => {
    const nextDay = new Date(checkInInput.value);
    nextDay.setDate(nextDay.getDate() + 1);
    checkOutInput.min = formatDate(nextDay);
    if (new Date(checkOutInput.value) <= new Date(checkInInput.value)) {
      checkOutInput.value = formatDate(nextDay);
    }
  });
}

// Auth State Management
function loadStoredAuth() {
  const token = localStorage.getItem('token');
  const user = localStorage.getItem('user');

  if (token && user) {
    try {
      currentToken = token;
      currentUser = JSON.parse(user);
      renderAuthUI();
    } catch {
      logout();
    }
  } else {
    renderAuthUI();
  }
}

function renderAuthUI() {
  const badge = document.getElementById('userProfileBadge');
  const authActions = document.getElementById('authActions');
  const btnLogout = document.getElementById('btnLogout');
  const adminTabBtn = document.getElementById('adminTabBtn');

  if (currentUser) {
    const roleClass = currentUser.role === 'Admin' ? 'badge-admin' : 'badge-customer';
    badge.className = `badge ${roleClass}`;
    badge.innerHTML = `<i class="fa-solid fa-user"></i> ${currentUser.fullName} (${currentUser.role})`;
    authActions.style.display = 'none';
    btnLogout.style.display = 'inline-flex';

    if (currentUser.role === 'Admin') {
      adminTabBtn.style.display = 'inline-flex';
    } else {
      adminTabBtn.style.display = 'none';
    }
  } else {
    badge.className = 'badge badge-guest';
    badge.innerHTML = 'Not Signed In';
    authActions.style.display = 'inline-flex';
    btnLogout.style.display = 'none';
    adminTabBtn.style.display = 'none';
  }
}

function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  currentToken = null;
  currentUser = null;
  renderAuthUI();
  showToast('Logged out successfully.', 'info');
  switchTab('searchTab');
  document.getElementById('myBookingsTableBody').innerHTML = '<tr><td colspan="10" style="text-align: center; color: var(--text-muted);">Please login to view your reservations.</td></tr>';
}

// Navigation Tabs
function setupEventListeners() {
  // Tabs
  document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => switchTab(btn.dataset.tab));
  });

  // Admin Subtabs
  document.querySelectorAll('.admin-subtab-btn').forEach(btn => {
    btn.addEventListener('click', () => switchAdminSubtab(btn.dataset.subtab));
  });

  // Modals
  document.getElementById('btnOpenLogin').addEventListener('click', () => openModal('loginModal'));
  document.getElementById('btnOpenRegister').addEventListener('click', () => openModal('registerModal'));
  document.getElementById('btnLogout').addEventListener('click', logout);
  document.querySelectorAll('.btnCloseModal').forEach(btn => {
    btn.addEventListener('click', () => closeAllModals());
  });

  // Quick Demo Logins
  document.getElementById('btnQuickAdmin').addEventListener('click', () => {
    document.getElementById('loginEmail').value = 'admin@hotel.com';
    document.getElementById('loginPassword').value = 'Admin@Hotel2026!';
  });

  document.getElementById('btnQuickCustomer').addEventListener('click', () => {
    document.getElementById('loginEmail').value = 'customer@example.com';
    document.getElementById('loginPassword').value = 'Customer@Pass2026!';
  });

  // Forms
  document.getElementById('loginForm').addEventListener('submit', handleLogin);
  document.getElementById('registerForm').addEventListener('submit', handleRegister);
  document.getElementById('searchForm').addEventListener('submit', (e) => {
    e.preventDefault();
    triggerSearch();
  });
  document.getElementById('confirmBookingForm').addEventListener('submit', handleConfirmBooking);
  document.getElementById('cancelBookingForm').addEventListener('submit', handleConfirmCancellation);
  document.getElementById('btnRefreshBookings').addEventListener('click', loadMyBookings);
  document.getElementById('btnRefreshAdminBookings').addEventListener('click', loadAdminAllBookings);
  document.getElementById('btnOpenCreateRoomModal').addEventListener('click', () => openModal('createRoomModal'));
  document.getElementById('createRoomForm').addEventListener('submit', handleCreateRoom);
}

function switchTab(tabId) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.toggle('active', b.dataset.tab === tabId));
  document.querySelectorAll('.tab-panel').forEach(p => p.classList.toggle('active', p.id === tabId));

  if (tabId === 'bookingsTab') {
    loadMyBookings();
  } else if (tabId === 'adminTab') {
    loadAdminRooms();
  }
}

function switchAdminSubtab(subpanelId) {
  document.querySelectorAll('.admin-subtab-btn').forEach(b => {
    const isActive = b.dataset.subtab === subpanelId;
    b.className = isActive ? 'btn btn-primary btn-sm admin-subtab-btn' : 'btn btn-outline btn-sm admin-subtab-btn';
  });

  document.querySelectorAll('.admin-subpanel').forEach(p => {
    p.style.display = p.id === subpanelId ? 'block' : 'none';
  });

  if (subpanelId === 'adminRooms') loadAdminRooms();
  if (subpanelId === 'adminRoomTypes') loadAdminRoomTypes();
  if (subpanelId === 'adminAmenities') loadAdminAmenities();
  if (subpanelId === 'adminAllBookings') loadAdminAllBookings();
}

// Master Data Fetching
async function loadRoomTypes() {
  try {
    const res = await fetch(`${API_BASE}/room-types`);
    const data = await res.json();
    if (data.success && data.data) {
      cachedRoomTypes = data.data;
      const select = document.getElementById('roomTypeFilter');
      const createSelect = document.getElementById('createRoomTypeId');
      select.innerHTML = '<option value="">All Categories</option>';
      createSelect.innerHTML = '';

      cachedRoomTypes.forEach(rt => {
        select.innerHTML += `<option value="${rt.roomTypeId}">${rt.typeName} ($${rt.basePrice}/night)</option>`;
        createSelect.innerHTML += `<option value="${rt.roomTypeId}">${rt.typeName} (Cap: ${rt.maxOccupancy})</option>`;
      });
    }
  } catch (err) {
    console.error('Failed to load room types', err);
  }
}

async function loadAmenities() {
  try {
    const res = await fetch(`${API_BASE}/amenities`);
    const data = await res.json();
    if (data.success && data.data) {
      cachedAmenities = data.data;
      const container = document.getElementById('amenitiesFilterList');
      container.innerHTML = '';
      cachedAmenities.forEach(a => {
        container.innerHTML += `
          <label class="amenity-chip" style="cursor: pointer; display: inline-flex; align-items: center; gap: 0.35rem; user-select: none;">
            <input type="checkbox" name="amenityFilter" value="${a.amenityId}">
            <span>${a.name}</span>
          </label>
        `;
      });
    }
  } catch (err) {
    console.error('Failed to load amenities', err);
  }
}

// Room Search Execution
async function triggerSearch() {
  const checkIn = document.getElementById('checkInDate').value;
  const checkOut = document.getElementById('checkOutDate').value;
  const guests = document.getElementById('guestCount').value;
  const roomTypeId = document.getElementById('roomTypeFilter').value;
  const maxPrice = document.getElementById('maxPriceFilter').value;

  const checkedAmenities = Array.from(document.querySelectorAll('input[name="amenityFilter"]:checked'))
    .map(cb => cb.value);

  const params = new URLSearchParams({
    checkInDate: checkIn,
    checkOutDate: checkOut,
    pageNumber: 1,
    pageSize: 20
  });

  if (guests) params.append('guests', guests);
  if (roomTypeId) params.append('roomTypeId', roomTypeId);
  if (maxPrice) params.append('maxPrice', maxPrice);
  if (checkedAmenities.length > 0) params.append('amenityIds', checkedAmenities.join(','));

  activeSearchParams = { checkIn, checkOut, guests: parseInt(guests) || 1 };

  const container = document.getElementById('roomCardsContainer');
  container.innerHTML = '<div style="grid-column: 1/-1; text-align: center; padding: 2rem; color: var(--text-muted);"><i class="fa-solid fa-spinner fa-spin fa-2x"></i><p style="margin-top:0.5rem;">Checking availability & date overlaps...</p></div>';

  try {
    const res = await fetch(`${API_BASE}/rooms/search?${params.toString()}`);
    const data = await res.json();

    if (data.success && data.data) {
      const items = data.data.items || [];
      document.getElementById('resultsCountBadge').textContent = `${data.data.totalRecords} Rooms Available`;

      if (items.length === 0) {
        container.innerHTML = `
          <div style="grid-column: 1/-1; text-align: center; padding: 3rem; background: white; border-radius: 8px; border: 1px dashed var(--border);">
            <i class="fa-solid fa-bed fa-3x" style="color: var(--text-muted); margin-bottom: 1rem;"></i>
            <h3>No Rooms Available for These Dates</h3>
            <p style="color: var(--text-muted); margin-top: 0.5rem;">Try selecting different dates, lowering the guest count, or clearing amenity filters.</p>
          </div>
        `;
        return;
      }

      container.innerHTML = items.map(room => `
        <div class="room-card">
          <div class="room-card-header">
            <div>
              <strong style="font-size: 1.1rem;">Room ${room.roomNumber}</strong>
              <div style="font-size: 0.8rem; color: var(--text-muted);">Floor ${room.floorNumber}</div>
            </div>
            <span class="badge badge-status">${room.status}</span>
          </div>
          <div class="room-card-body">
            <h4 style="color: var(--primary); margin-bottom: 0.35rem;">${room.roomTypeName}</h4>
            <p style="font-size: 0.85rem; color: var(--text-muted); min-height: 40px;">
              ${room.roomTypeDescription || 'Premium accommodation with modern hotel amenities.'}
            </p>
            <div style="margin-top: 0.5rem; font-size: 0.85rem;">
              <i class="fa-solid fa-users" style="color: var(--secondary);"></i> Max Capacity: <strong>${room.maxOccupancy} Guests</strong>
            </div>
            <div class="room-price-box">
              <div style="display: flex; justify-content: space-between; font-size: 0.85rem; color: var(--text-muted);">
                <span>$${room.basePricePerNight} / night</span>
                <span>${room.stayNights} Nights</span>
              </div>
              <div style="display: flex; justify-content: space-between; align-items: baseline; margin-top: 0.35rem;">
                <span style="font-weight: 600;">Calculated Stay Total:</span>
                <span class="price-total">$${room.totalStayPrice.toFixed(2)}</span>
              </div>
            </div>
          </div>
          <div class="room-card-footer">
            <button class="btn btn-primary" style="width: 100%;" onclick="openBookingModal(${room.roomId}, '${room.roomNumber}', '${room.roomTypeName}', ${room.basePricePerNight}, ${room.totalStayPrice}, ${room.stayNights}, ${room.maxOccupancy})">
              <i class="fa-solid fa-calendar-check"></i> Book This Room
            </button>
          </div>
        </div>
      `).join('');
    } else {
      showToast(data.message || 'Failed to search rooms.', 'error');
    }
  } catch (err) {
    showToast('Network error during room search.', 'error');
  }
}

// Booking Modal Logic
function openBookingModal(roomId, roomNumber, roomTypeName, nightlyRate, totalStayPrice, stayNights, maxOccupancy) {
  if (!currentUser) {
    showToast('Please sign in or register to book a room.', 'info');
    openModal('loginModal');
    return;
  }

  document.getElementById('bookRoomId').value = roomId;
  document.getElementById('bookRoomTitle').textContent = `Room ${roomNumber} (${roomTypeName})`;
  document.getElementById('bookNightlyRate').textContent = `$${nightlyRate} / night`;
  document.getElementById('bookDatesSummary').textContent = `${activeSearchParams.checkIn} to ${activeSearchParams.checkOut}`;
  document.getElementById('bookNightsSummary').textContent = stayNights;
  document.getElementById('bookTotalAmount').textContent = `$${totalStayPrice.toFixed(2)}`;

  const guestInput = document.getElementById('bookGuestCount');
  guestInput.max = maxOccupancy;
  guestInput.value = Math.min(activeSearchParams.guests || 1, maxOccupancy);
  document.getElementById('bookOccupancyHint').textContent = `Max Occupancy: ${maxOccupancy} Guests`;

  openModal('bookingModal');
}

async function handleConfirmBooking(e) {
  e.preventDefault();

  const roomId = parseInt(document.getElementById('bookRoomId').value);
  const numberOfGuests = parseInt(document.getElementById('bookGuestCount').value);
  const customerNotes = document.getElementById('bookCustomerNotes').value;

  const payload = {
    roomId: roomId,
    checkInDate: activeSearchParams.checkIn,
    checkOutDate: activeSearchParams.checkOut,
    numberOfGuests: numberOfGuests,
    customerNotes: customerNotes
  };

  try {
    const res = await fetch(`${API_BASE}/bookings`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${currentToken}`
      },
      body: JSON.stringify(payload)
    });

    const data = await res.json();
    if (res.ok && data.success) {
      closeAllModals();
      showToast(`Booking Confirmed! Reference: ${data.data.bookingReference}`, 'success');
      switchTab('bookingsTab');
    } else {
      showToast(data.detail || data.message || 'Booking failed: room may have been taken.', 'error');
    }
  } catch (err) {
    showToast('Error communicating with reservation service.', 'error');
  }
}

// My Bookings Tab
async function loadMyBookings() {
  if (!currentUser) return;

  const tbody = document.getElementById('myBookingsTableBody');
  tbody.innerHTML = '<tr><td colspan="10" style="text-align: center;"><i class="fa-solid fa-spinner fa-spin"></i> Loading reservations...</td></tr>';

  try {
    const res = await fetch(`${API_BASE}/bookings/my-bookings`, {
      headers: { 'Authorization': `Bearer ${currentToken}` }
    });
    const data = await res.json();

    if (data.success && data.data) {
      const items = data.data;
      if (items.length === 0) {
        tbody.innerHTML = '<tr><td colspan="10" style="text-align: center; color: var(--text-muted);">You have no active or past bookings.</td></tr>';
        return;
      }

      tbody.innerHTML = items.map(b => {
        const isConfirmed = b.bookingStatus === 'Confirmed';
        const isFuture = new Date(b.checkInDate) > new Date();
        const canCancel = isConfirmed && isFuture;

        return `
          <tr>
            <td><strong>${b.bookingReference}</strong></td>
            <td>Room ${b.roomNumber}</td>
            <td>${b.roomTypeName}</td>
            <td>${b.checkInDate.split('T')[0]}</td>
            <td>${b.checkOutDate.split('T')[0]}</td>
            <td>${b.totalNights}</td>
            <td>${b.numberOfGuests}</td>
            <td><strong>$${b.totalAmount.toFixed(2)}</strong></td>
            <td>
              <span class="badge ${b.bookingStatus === 'Confirmed' ? 'badge-status' : 'badge-cancelled'}">
                ${b.bookingStatus}
              </span>
            </td>
            <td>
              ${canCancel ? `<button class="btn btn-danger btn-sm" onclick="openCancellationModal(${b.bookingId}, '${b.bookingReference}', ${b.totalAmount}, '${b.checkInDate}')"><i class="fa-solid fa-ban"></i> Cancel</button>` : '<span style="color:var(--text-muted); font-size:0.8rem;">None</span>'}
            </td>
          </tr>
        `;
      }).join('');
    }
  } catch (err) {
    tbody.innerHTML = '<tr><td colspan="10" style="text-align: center; color: var(--danger);">Failed to load reservations.</td></tr>';
  }
}

// Cancellation Modal & 3-Tier Policy Preview
function openCancellationModal(bookingId, bookingReference, totalAmount, checkInDateStr) {
  document.getElementById('cancelBookingId').value = bookingId;
  document.getElementById('cancelRefLabel').textContent = bookingReference;
  document.getElementById('cancelOriginalAmount').textContent = `$${totalAmount.toFixed(2)}`;

  // Calculate estimated hours to check-in (Standardized at 14:00 UTC on check-in date)
  const checkInDate = new Date(checkInDateStr);
  checkInDate.setUTCHours(14, 0, 0, 0);

  const now = new Date();
  const diffHours = (checkInDate - now) / (1000 * 60 * 60);

  let refundPct = 0;
  let tierBadgeClass = 'badge-maintenance';
  let tierText = '0% Refund ($0.00)';

  if (diffHours >= 48) {
    refundPct = 100;
    tierBadgeClass = 'badge-status';
    tierText = `100% Refund ($${totalAmount.toFixed(2)})`;
  } else if (diffHours >= 24) {
    refundPct = 50;
    tierBadgeClass = 'badge-admin';
    tierText = `50% Refund ($${(totalAmount * 0.5).toFixed(2)})`;
  }

  document.getElementById('cancelHoursLabel').textContent = `${Math.max(0, Math.round(diffHours))} hours`;
  const badge = document.getElementById('cancelRefundTierBadge');
  badge.className = `badge ${tierBadgeClass}`;
  badge.textContent = tierText;

  openModal('cancellationModal');
}

async function handleConfirmCancellation(e) {
  e.preventDefault();
  const bookingId = document.getElementById('cancelBookingId').value;
  const reason = document.getElementById('cancelReasonInput').value;

  try {
    const res = await fetch(`${API_BASE}/bookings/${bookingId}/cancel`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${currentToken}`
      },
      body: JSON.stringify({ reason: reason })
    });

    const data = await res.json();
    if (res.ok && data.success) {
      closeAllModals();
      showToast(`Reservation Cancelled. Refund Amount: $${data.data.refundAmount.toFixed(2)} (${data.data.refundPercentage}% tier)`, 'success');
      loadMyBookings();
    } else {
      showToast(data.detail || data.message || 'Failed to cancel booking.', 'error');
    }
  } catch (err) {
    showToast('Network error during cancellation request.', 'error');
  }
}

// Admin Tab Logic
async function loadAdminRooms() {
  const tbody = document.getElementById('adminRoomsTableBody');
  tbody.innerHTML = '<tr><td colspan="7" style="text-align: center;"><i class="fa-solid fa-spinner fa-spin"></i> Loading rooms...</td></tr>';

  try {
    const res = await fetch(`${API_BASE}/rooms?includeInactive=true`, {
      headers: { 'Authorization': `Bearer ${currentToken}` }
    });
    const data = await res.json();
    if (data.success && data.data) {
      tbody.innerHTML = data.data.map(r => `
        <tr>
          <td><strong>Room ${r.roomNumber}</strong></td>
          <td>Floor ${r.floorNumber}</td>
          <td>${r.roomTypeName}</td>
          <td>$${r.basePrice.toFixed(2)}</td>
          <td>${r.maxOccupancy}</td>
          <td>
            <span class="badge ${r.status === 'Available' ? 'badge-status' : 'badge-maintenance'}">${r.status}</span>
          </td>
          <td>
            <button class="btn btn-outline btn-sm" onclick="toggleRoomMaintenance(${r.roomId}, '${r.status}')">
              ${r.status === 'Available' ? '<i class="fa-solid fa-wrench"></i> Set Maintenance' : '<i class="fa-solid fa-check"></i> Set Available'}
            </button>
          </td>
        </tr>
      `).join('');
    }
  } catch (err) {
    tbody.innerHTML = '<tr><td colspan="7" style="text-align: center; color: var(--danger);">Failed to load rooms.</td></tr>';
  }
}

async function toggleRoomMaintenance(roomId, currentStatus) {
  const newStatus = currentStatus === 'Available' ? 'Under Maintenance' : 'Available';
  try {
    const res = await fetch(`${API_BASE}/rooms/${roomId}/status`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${currentToken}`
      },
      body: JSON.stringify({ status: newStatus })
    });
    const data = await res.json();
    if (res.ok && data.success) {
      showToast(`Room status updated to ${newStatus}.`, 'success');
      loadAdminRooms();
    } else {
      showToast(data.detail || 'Failed to update status.', 'error');
    }
  } catch {
    showToast('Network error.', 'error');
  }
}

async function handleCreateRoom(e) {
  e.preventDefault();
  const roomNumber = document.getElementById('createRoomNumber').value;
  const roomTypeId = parseInt(document.getElementById('createRoomTypeId').value);
  const floorNumber = parseInt(document.getElementById('createRoomFloor').value);
  const status = document.getElementById('createRoomStatus').value;

  try {
    const res = await fetch(`${API_BASE}/rooms`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${currentToken}`
      },
      body: JSON.stringify({ roomNumber, roomTypeId, floorNumber, status })
    });

    const data = await res.json();
    if (res.ok && data.success) {
      closeAllModals();
      showToast(`Room ${roomNumber} created successfully!`, 'success');
      loadAdminRooms();
    } else {
      showToast(data.detail || data.message || 'Failed to create room.', 'error');
    }
  } catch {
    showToast('Error creating room.', 'error');
  }
}

async function loadAdminRoomTypes() {
  const tbody = document.getElementById('adminRoomTypesTableBody');
  tbody.innerHTML = cachedRoomTypes.map(rt => `
    <tr>
      <td>${rt.roomTypeId}</td>
      <td><strong>${rt.typeName}</strong></td>
      <td>${rt.description || 'N/A'}</td>
      <td>$${rt.basePrice.toFixed(2)}</td>
      <td>${rt.maxOccupancy} Guests</td>
      <td><span class="badge ${rt.isActive ? 'badge-status' : 'badge-maintenance'}">${rt.isActive ? 'Active' : 'Inactive'}</span></td>
    </tr>
  `).join('');
}

async function loadAdminAmenities() {
  const tbody = document.getElementById('adminAmenitiesTableBody');
  tbody.innerHTML = cachedAmenities.map(a => `
    <tr>
      <td>${a.amenityId}</td>
      <td><i class="fa-solid fa-${a.iconCode || 'tag'}"></i></td>
      <td><strong>${a.name}</strong></td>
      <td>${a.description || 'N/A'}</td>
      <td><span class="badge badge-status">Active</span></td>
    </tr>
  `).join('');
}

async function loadAdminAllBookings() {
  const tbody = document.getElementById('adminAllBookingsTableBody');
  tbody.innerHTML = '<tr><td colspan="8" style="text-align: center;"><i class="fa-solid fa-spinner fa-spin"></i> Loading master booking register...</td></tr>';

  try {
    const res = await fetch(`${API_BASE}/bookings?pageSize=50`, {
      headers: { 'Authorization': `Bearer ${currentToken}` }
    });
    const data = await res.json();
    if (data.success && data.data) {
      const items = data.data.items || [];
      if (items.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; color: var(--text-muted);">No bookings found in system.</td></tr>';
        return;
      }

      tbody.innerHTML = items.map(b => `
        <tr>
          <td><strong>${b.bookingReference}</strong></td>
          <td>${b.customerName}</td>
          <td>${b.customerEmail}</td>
          <td>Room ${b.roomNumber}</td>
          <td>${b.checkInDate.split('T')[0]}</td>
          <td>${b.checkOutDate.split('T')[0]}</td>
          <td><strong>$${b.totalAmount.toFixed(2)}</strong></td>
          <td><span class="badge ${b.bookingStatus === 'Confirmed' ? 'badge-status' : 'badge-cancelled'}">${b.bookingStatus}</span></td>
        </tr>
      `).join('');
    }
  } catch (err) {
    tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; color: var(--danger);">Failed to load bookings.</td></tr>';
  }
}

// Authentication Handlers
async function handleLogin(e) {
  e.preventDefault();
  const email = document.getElementById('loginEmail').value;
  const password = document.getElementById('loginPassword').value;

  try {
    const res = await fetch(`${API_BASE}/users/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });

    const data = await res.json();
    if (res.ok && data.success) {
      currentToken = data.data.token;
      currentUser = {
        userId: data.data.userId,
        fullName: data.data.fullName,
        email: data.data.email,
        role: data.data.role
      };

      localStorage.setItem('token', currentToken);
      localStorage.setItem('user', JSON.stringify(currentUser));

      renderAuthUI();
      closeAllModals();
      showToast(`Welcome back, ${currentUser.fullName}!`, 'success');

      if (currentUser.role === 'Admin') {
        switchTab('adminTab');
      } else {
        switchTab('searchTab');
      }
    } else {
      showToast(data.detail || data.message || 'Invalid credentials.', 'error');
    }
  } catch (err) {
    showToast('Network error during authentication.', 'error');
  }
}

async function handleRegister(e) {
  e.preventDefault();
  const fullName = document.getElementById('regFullName').value;
  const email = document.getElementById('regEmail').value;
  const password = document.getElementById('regPassword').value;
  const phoneNumber = document.getElementById('regPhone').value;

  try {
    const res = await fetch(`${API_BASE}/users/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ fullName, email, password, phoneNumber })
    });

    const data = await res.json();
    if (res.ok && data.success) {
      closeAllModals();
      showToast('Registration successful! Please sign in.', 'success');
      openModal('loginModal');
      document.getElementById('loginEmail').value = email;
      document.getElementById('loginPassword').value = password;
    } else {
      showToast(data.detail || data.message || 'Registration failed.', 'error');
    }
  } catch (err) {
    showToast('Network error during registration.', 'error');
  }
}

// Modal & Toast Utilities
function openModal(modalId) {
  closeAllModals();
  const modal = document.getElementById(modalId);
  if (modal) modal.classList.add('show');
}

function closeAllModals() {
  document.querySelectorAll('.modal-backdrop').forEach(m => m.classList.remove('show'));
}

function showToast(message, type = 'info') {
  const container = document.getElementById('toastContainer');
  const toast = document.createElement('div');
  toast.className = `toast toast-${type}`;

  const icon = type === 'success' ? 'circle-check' : type === 'error' ? 'circle-exclamation' : 'circle-info';
  toast.innerHTML = `<i class="fa-solid fa-${icon}"></i> <span>${message}</span>`;

  container.appendChild(toast);
  setTimeout(() => {
    toast.remove();
  }, 4000);
}
