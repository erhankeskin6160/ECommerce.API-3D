# Step 1: Login
$loginBody = @{ email = "erhankeskin0661@gmail.com"; password = "Password123!" } | ConvertTo-Json
$loginResp = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/login" -Method Post -ContentType "application/json" -Body $loginBody
$token = $loginResp.token
Write-Host "TOKEN alindi!"

# Step 2: Get orders to find productId
$headers = @{ Authorization = "Bearer $token" }
$orders = Invoke-RestMethod -Uri "http://localhost:5243/api/orders" -Headers $headers
$productId = $orders[1].items[0].productId  # Order #2, first item
Write-Host "Product ID: $productId"

# Step 3: Upload review with image
$filePath = "C:\Users\Erhan Keskin\Desktop\test_review.png"
$fileBytes = [System.IO.File]::ReadAllBytes($filePath)
$boundary = [System.Guid]::NewGuid().ToString()

$bodyLines = @(
    "--$boundary",
    'Content-Disposition: form-data; name="ProductId"',
    '',
    "$productId",
    "--$boundary",
    'Content-Disposition: form-data; name="Rating"',
    '',
    '5',
    "--$boundary",
    'Content-Disposition: form-data; name="Comment"',
    '',
    'Harika bir urun, resimli degerlendirme!',
    "--$boundary",
    "Content-Disposition: form-data; name=`"Image`"; filename=`"test_review.png`"",
    'Content-Type: image/png',
    ''
)

$encoding = [System.Text.Encoding]::UTF8
$bodyStart = $encoding.GetBytes(($bodyLines -join "`r`n") + "`r`n")
$bodyEnd = $encoding.GetBytes("`r`n--$boundary--`r`n")

$ms = [System.IO.MemoryStream]::new()
$ms.Write($bodyStart, 0, $bodyStart.Length)
$ms.Write($fileBytes, 0, $fileBytes.Length)
$ms.Write($bodyEnd, 0, $bodyEnd.Length)
$body = $ms.ToArray()

$contentType = "multipart/form-data; boundary=$boundary"

try {
    $resp = Invoke-RestMethod -Uri "http://localhost:5243/api/products/$productId/reviews" -Method Post -Headers $headers -ContentType $contentType -Body $body
    Write-Host "BASARILI! Review sonucu:"
    Write-Host ($resp | ConvertTo-Json -Depth 5)
} catch {
    Write-Host "HATA: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        Write-Host "YANIT: $($reader.ReadToEnd())"
    }
}
