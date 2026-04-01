# Admin ile giriş yapıp sipariş durumunu güncelle (yeni bildirim oluşturmak için)
$loginBody = @{ email = "admin@example.com"; password = "Admin123!" } | ConvertTo-Json
$loginResp = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/login" -Method Post -ContentType "application/json" -Body $loginBody
$adminToken = $loginResp.token
$headers = @{ Authorization = "Bearer $adminToken" }

# Siparişleri getir
$orders = Invoke-RestMethod -Uri "http://localhost:5243/api/admin/orders" -Headers $headers
$orderId = $orders[0].id
Write-Host "Order ID: $orderId -> status: Shipped"

# Durumu Shipped yap
$statusBody = @{ status = "Shipped" } | ConvertTo-Json
$resp = Invoke-RestMethod -Uri "http://localhost:5243/api/admin/orders/$orderId/status" -Method Put -ContentType "application/json" -Headers $headers -Body $statusBody
Write-Host "Status update: $($resp.message)"

# Kullanıcı ile giriş yapıp bildirimleri kontrol et
$userBody = @{ email = "erhankeskin0661@gmail.com"; password = "Password123!" } | ConvertTo-Json
$userResp = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/login" -Method Post -ContentType "application/json" -Body $userBody
$userHeaders = @{ Authorization = "Bearer $($userResp.token)" }

$notifications = Invoke-RestMethod -Uri "http://localhost:5243/api/notifications" -Headers $userHeaders
Write-Host "`nBildirimler (son 3):"
foreach ($n in ($notifications | Select-Object -First 3)) {
    Write-Host "  TR: $($n.title) | $($n.message)"
    Write-Host "  EN: $($n.titleEn) | $($n.messageEn)"
    Write-Host "  ---"
}
