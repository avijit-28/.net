$s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$p = Invoke-WebRequest -Uri 'http://localhost:64722/Account/Login' -WebSession $s -UseBasicParsing
$token = ''
if ($p.Content -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $token = $matches[1]
}
$body = '__RequestVerificationToken=' + [Uri]::EscapeDataString($token) + '&Email=john%40example.com&Password=password123'
$resp = Invoke-WebRequest -Uri 'http://localhost:64722/Account/Login' -Method Post -Body $body -ContentType 'application/x-www-form-urlencoded' -WebSession $s -UseBasicParsing
Write-Host "Status: $($resp.StatusCode)"
$track = Invoke-WebRequest -Uri 'http://localhost:64722/Order/History' -WebSession $s -UseBasicParsing
Write-Host "Track length: $($track.Content.Length)"
Write-Host "Has Sidebar: $($track.Content.Contains('Customer Left Sidebar'))"
Write-Host "Has My Profile sidebar link: $($track.Content.Contains('My Profile</a>'))"
