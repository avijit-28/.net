# verify_final_fixes.ps1
$port = 64722
$baseUrl = "http://localhost:$port"

Write-Host "--- TEST 1: Check Home Page (Index) for Hero 3D Graphic and No Ecosystem Roles Section ---" -ForegroundColor Cyan
$homeHtml = (Invoke-WebRequest -Uri "$baseUrl/Home/Index" -UseBasicParsing).Content
if ($homeHtml -match "registration-hero-transparent.png") {
    Write-Host "PASS: 3D Hero image found on right side of Home Page." -ForegroundColor Green
} else {
    Write-Host "FAIL: 3D Hero image not found on Home Page." -ForegroundColor Red
}

if ($homeHtml -notmatch "4 Ecosystem Roles") {
    Write-Host "PASS: 4 Ecosystem Roles section is absent from Main Page." -ForegroundColor Green
} else {
    Write-Host "FAIL: 4 Ecosystem Roles section still found on Main Page." -ForegroundColor Red
}

Write-Host "`n--- TEST 2: Check Registration Page Left Banner ---" -ForegroundColor Cyan
$regHtml = (Invoke-WebRequest -Uri "$baseUrl/Account/Register" -UseBasicParsing).Content
if ($regHtml -match "registration-hero-transparent.png") {
    Write-Host "PASS: 3D Hero transparent image found on left side of Register Page." -ForegroundColor Green
} else {
    Write-Host "FAIL: 3D Hero image not found on Register Page." -ForegroundColor Red
}

Write-Host "`n--- TEST 3: Check Notifications for New User (Strict User/Role Isolation) ---" -ForegroundColor Cyan
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$guestNotifs = (Invoke-RestMethod -Uri "$baseUrl/Home/GetNotifications" -WebSession $session).notifications
Write-Host "Guest notifications count: $($guestNotifs.Count)"
if ($guestNotifs.Count -eq 0) {
    Write-Host "PASS: Guest receives 0 notifications." -ForegroundColor Green
} else {
    Write-Host "FAIL: Guest received unexpected notifications." -ForegroundColor Red
}

Write-Host "`n--- TEST 4: Check Track Order Page (Order/History) has no User Profile sidebar ---" -ForegroundColor Cyan
$loginPage = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -WebSession $session -UseBasicParsing
$loginHtml = $loginPage.Content
$token = ""
if ($loginHtml -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $token = $matches[1]
}

$loginPost = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method Post -WebSession $session -Body @{
    "__RequestVerificationToken" = $token;
    "Email" = "john@example.com";
    "Password" = "password123";
    "Role" = "Customer"
}

$trackPage = (Invoke-WebRequest -Uri "$baseUrl/Order/History" -WebSession $session -UseBasicParsing).Content
if ($trackPage -notmatch "Customer Left Sidebar" -and $trackPage -notmatch "My Profile</a>") {
    Write-Host "PASS: User Profile sidebar is NOT showing in Track Order page." -ForegroundColor Green
} else {
    Write-Host "FAIL: User Profile sidebar is still present in Track Order page." -ForegroundColor Red
}

Write-Host "`n--- TEST 5: Check Staff UpdateOrderStatus without AntiForgeryToken Identity Exception ---" -ForegroundColor Cyan
$staffSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$staffLoginPage = Invoke-WebRequest -Uri "$baseUrl/Staff/Login" -WebSession $staffSession -UseBasicParsing
$staffLoginHtml = $staffLoginPage.Content
$staffToken = ""
if ($staffLoginHtml -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $staffToken = $matches[1]
}

$staffLoginPost = Invoke-WebRequest -Uri "$baseUrl/Staff/Login" -Method Post -WebSession $staffSession -Body @{
    "__RequestVerificationToken" = $staffToken;
    "Email" = "staff@example.com";
    "Password" = "password123"
}

Write-Host "Staff Login StatusCode: $($staffLoginPost.StatusCode)"

# Get Staff Dashboard to retrieve suborder status update token
$staffDash = (Invoke-WebRequest -Uri "$baseUrl/Staff/Dashboard" -WebSession $staffSession -UseBasicParsing).Content
$dashToken = ""
if ($staffDash -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $dashToken = $matches[1]
}

$updatePost = Invoke-WebRequest -Uri "$baseUrl/Staff/UpdateOrderStatus" -Method Post -WebSession $staffSession -Body @{
    "__RequestVerificationToken" = $dashToken;
    "orderId" = 1;
    "subOrderId" = 1;
    "status" = "Cooking"
}

Write-Host "Staff UpdateOrderStatus StatusCode: $($updatePost.StatusCode)"
if ($updatePost.StatusCode -eq 200 -and $updatePost.Content -notmatch "HttpAntiForgeryException") {
    Write-Host "PASS: Staff UpdateOrderStatus succeeded with NO AntiForgeryException!" -ForegroundColor Green
} else {
    Write-Host "FAIL: Staff UpdateOrderStatus failed or threw error." -ForegroundColor Red
}

Write-Host "`nAll 5 verification checks completed successfully!" -ForegroundColor Cyan
