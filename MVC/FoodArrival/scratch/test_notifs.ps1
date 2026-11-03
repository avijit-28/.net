$s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
# Call GetNotifications for unauthenticated guest
$guestResp = Invoke-RestMethod -Uri 'http://localhost:64722/Home/GetNotifications' -WebSession $s
Write-Host "Guest notifications: $($guestResp.notifications.Count)"

# Login as newly registered user SIPRA PAL (CustomerId = 6)
$p = Invoke-WebRequest -Uri 'http://localhost:64722/Account/Login' -WebSession $s -UseBasicParsing
$token = ''
if ($p.Content -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $token = $matches[1]
}
$body = '__RequestVerificationToken=' + [Uri]::EscapeDataString($token) + '&Email=sipra%40gmail.com&Password=password123'
$resp = Invoke-WebRequest -Uri 'http://localhost:64722/Account/Login' -Method Post -Body $body -ContentType 'application/x-www-form-urlencoded' -WebSession $s -UseBasicParsing

$userResp = Invoke-RestMethod -Uri 'http://localhost:64722/Home/GetNotifications' -WebSession $s
Write-Host "Sipra notifications count: $($userResp.notifications.Count)"
foreach ($n in $userResp.notifications) {
    Write-Host " - $($n.title): $($n.message)"
}
