$s = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$p = Invoke-WebRequest -Uri 'http://localhost:64722/Staff/Login' -WebSession $s -UseBasicParsing
$token = ''
if ($p.Content -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $token = $matches[1]
}
$body = '__RequestVerificationToken=' + [Uri]::EscapeDataString($token) + '&Email=staff%40example.com&Password=password123'
$resp = Invoke-WebRequest -Uri 'http://localhost:64722/Staff/Login' -Method Post -Body $body -ContentType 'application/x-www-form-urlencoded' -WebSession $s -UseBasicParsing
Write-Host "Staff Login Status: $($resp.StatusCode)"

$dash = Invoke-WebRequest -Uri 'http://localhost:64722/Staff/Dashboard' -WebSession $s -UseBasicParsing
$dashToken = ''
if ($dash.Content -match '__RequestVerificationToken"[^>]*value="([^"]+)"') {
    $dashToken = $matches[1]
}

$updateBody = '__RequestVerificationToken=' + [Uri]::EscapeDataString($dashToken) + '&orderId=1&subOrderId=1&status=Cooking'
$updateResp = Invoke-WebRequest -Uri 'http://localhost:64722/Staff/UpdateOrderStatus' -Method Post -Body $updateBody -ContentType 'application/x-www-form-urlencoded' -WebSession $s -UseBasicParsing
Write-Host "Update Status: $($updateResp.StatusCode)"
Write-Host "Has Error: $($updateResp.Content.Contains('HttpAntiForgeryException'))"
