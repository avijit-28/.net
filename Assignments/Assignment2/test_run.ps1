$loginRes = Invoke-RestMethod -Uri 'http://localhost:5102/api/users/login' -Method Post -Body (@{ email = 'customer@example.com'; password = 'Customer@Pass2026!' } | ConvertTo-Json) -ContentType 'application/json'
$token = $loginRes.data.token
Write-Output "CUSTOMER_TOKEN_OBTAINED: $($token.Substring(0, 20))..."

# Search rooms
$search = Invoke-RestMethod -Uri 'http://localhost:5102/api/rooms/search?checkInDate=2026-12-20&checkOutDate=2026-12-25' -Method Get
Write-Output "SEARCH_RESULTS: $($search.data.totalRecords) rooms found"

# Book Room 101 (RoomId 1)
$headers = @{ "Authorization" = "Bearer $token" }
$bookBody = @{
    roomId = 1
    checkInDate = '2026-12-20'
    checkOutDate = '2026-12-25'
    numberOfGuests = 1
    customerNotes = 'Testing end-to-end booking flow'
} | ConvertTo-Json

$bookRes = Invoke-RestMethod -Uri 'http://localhost:5102/api/bookings' -Method Post -Body $bookBody -Headers $headers -ContentType 'application/json'
$ref = $bookRes.data.bookingReference
$bookingId = $bookRes.data.bookingId
$amount = $bookRes.data.totalAmount
Write-Output "BOOKING_CREATED: ID=$bookingId, Ref=$ref, Amount=$$amount"

# Try double-booking same room for overlapping dates (Dec 22 to Dec 27)
try {
    $dupBody = @{
        roomId = 1
        checkInDate = '2026-12-22'
        checkOutDate = '2026-12-27'
        numberOfGuests = 1
    } | ConvertTo-Json
    Invoke-RestMethod -Uri 'http://localhost:5102/api/bookings' -Method Post -Body $dupBody -Headers $headers -ContentType 'application/json'
    Write-Output "ERROR: Double booking was allowed!"
} catch {
    Write-Output "DOUBLE_BOOKING_PREVENTED: $($_.Exception.Message)"
}

# Cancel booking and verify 100% refund
$cancelBody = @{ reason = 'Customer requested cancellation' } | ConvertTo-Json
$cancelRes = Invoke-RestMethod -Uri "http://localhost:5102/api/bookings/$bookingId/cancel" -Method Post -Body $cancelBody -Headers $headers -ContentType 'application/json'
Write-Output "CANCELLATION_SUCCESS: RefundPercentage=$($cancelRes.data.refundPercentage)%, RefundAmount=$$($cancelRes.data.refundAmount)"
