Add-Type -AssemblyName System.Drawing
$b = [System.Drawing.Bitmap]::FromFile('D:\avi\C#\MVC\FoodArrival\Content\images\registration-hero.png')
Write-Host "Dims: $($b.Width)x$($b.Height)"
$p = $b.GetPixel(5, 5)
Write-Host "Pixel 5,5: R=$($p.R) G=$($p.G) B=$($p.B) A=$($p.A)"
$b.Dispose()
