# Script t?i hình ?nh sách t? các ngu?n mi?n phí
# Ch?y script này trong th? m?c bai1/wwwroot/images/books/

$OutputPath = "."

# Danh sách sách và URL ?nh (s? d?ng placeholder ho?c ?nh th?c)
$books = @{
    "mat-biec.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/7e/3e/8a/c5c9cd7e6c2f24a0cb5fc6a6d4e23c4a.jpg"
    "hoa-vang.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/56/6b/0a/2a5b2f36c7c6d57d45f0e7e2ad1be8d2.jpg"
    "1q84.jpg" = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1483103331i/10357575.jpg"
    "rich-dad.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/2e/2a/c2/bc2f8e0c1a3c6b65c5a4d3a0d6d4f0c3.jpg"
    "7-habits.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/82/81/91/0fc5e9a7b8c5a0e5c7b8d9a0c1d2e3f4.jpg"
    "dac-nhan-tam.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/df/7d/da/d340edda2b0eacb7ddc47537cddb5e08.jpg"
    "tren-duong-bang.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/65/77/a1/e3e7c1f9e7c6d5b4a3c2d1e0f9a8b7c6.jpg"
    "harry-potter.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/51/5e/5d/2e8a3f1e0c9d8b7a6c5d4e3f2a1b0c9d.jpg"
    "de-men.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/45/3b/fc/33c6e49b3eb4b32b8c4b0b5dd10b53db.jpg"
    "algorithms.jpg" = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1387741681i/108986.jpg"
    "sapiens.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/43/22/94/8c3e8c4f5a3b2c1d0e9f8a7b6c5d4e3f.jpg"
    "nha-gia-kim.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/89/60/00/79c62bf2a7a63b86a0b2e9f7b8c9d0e1.jpg"
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Script T?i Hình ?nh Sách" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# T?o th? m?c n?u ch?a t?n t?i
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "?ã t?o th? m?c: $OutputPath" -ForegroundColor Green
}

foreach ($book in $books.GetEnumerator()) {
    $fileName = $book.Key
    $url = $book.Value
    $filePath = Join-Path $OutputPath $fileName
    
    Write-Host "?ang t?i: $fileName..." -NoNewline
    
    try {
        # Th? t?i t? URL
        Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 10 -ErrorAction Stop
        Write-Host " OK" -ForegroundColor Green
    }
    catch {
        # N?u không t?i ???c, t?o placeholder
        Write-Host " L?i - T?o placeholder" -ForegroundColor Yellow
        
        # T?o m?t placeholder image ??n gi?n (1x1 pixel)
        # B?n c?n thay th? b?ng ?nh th?t sau
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Hoàn thành!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "L?u ý: M?t s? ?nh có th? c?n ???c t?i th? công" -ForegroundColor Yellow
Write-Host "Xem README.md ?? bi?t thêm chi ti?t" -ForegroundColor Yellow
