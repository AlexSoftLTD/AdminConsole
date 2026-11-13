# Original AdminConsole icon: a terminal chevron and an amber cursor.
# All geometry is drawn here; no third-party image is used.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$frames = @()
foreach ($size in @(16,24,32,48,64,128,256)) {
    $bitmap = [Drawing.Bitmap]::new($size,$size)
    $g = [Drawing.Graphics]::FromImage($bitmap)
    try {
        $g.SmoothingMode = 'AntiAlias'
        $g.ScaleTransform($size/64.0,$size/64.0)
        $background = [Drawing.SolidBrush]::new([Drawing.Color]::FromArgb(22,32,49))
        $accent = [Drawing.Pen]::new([Drawing.Color]::FromArgb(78,204,202),5)
        $cursor = [Drawing.Pen]::new([Drawing.Color]::FromArgb(255,170,122),5)
        try {
            $g.FillRectangle($background,3,3,58,58)
            $g.DrawLines($accent,[Drawing.PointF[]]@([Drawing.PointF]::new(15,20),[Drawing.PointF]::new(29,32),[Drawing.PointF]::new(15,44)))
            $g.DrawLine($cursor,35,44,49,44)
        } finally { $background.Dispose(); $accent.Dispose(); $cursor.Dispose() }
        $ms = [IO.MemoryStream]::new()
        try { $bitmap.Save($ms,[Drawing.Imaging.ImageFormat]::Png); $frames += ,([byte[]]$ms.ToArray()) } finally { $ms.Dispose() }
    } finally { $g.Dispose(); $bitmap.Dispose() }
}
$path = Join-Path (Split-Path $PSScriptRoot) 'src\assets\AdminConsole.ico'
$stream = [IO.File]::Create($path)
$writer = [IO.BinaryWriter]::new($stream)
try {
    $writer.Write([uint16]0); $writer.Write([uint16]1); $writer.Write([uint16]$frames.Count)
    $offset = 6 + 16*$frames.Count
    $sizes = @(16,24,32,48,64,128,256)
    for ($i=0;$i -lt $frames.Count;$i++) {
        $dim = if ($sizes[$i] -eq 256) {0} else {$sizes[$i]}
        $writer.Write([byte]$dim); $writer.Write([byte]$dim); $writer.Write([byte]0); $writer.Write([byte]0)
        $writer.Write([uint16]1); $writer.Write([uint16]32)
        $writer.Write([uint32]$frames[$i].Length); $writer.Write([uint32]$offset)
        $offset += $frames[$i].Length
    }
    foreach($frame in $frames) { $writer.Write([byte[]]$frame) }
} finally { $writer.Dispose() }
Write-Output $path
