param([String] $filePath)

if ([string]::IsNullOrEmpty($filePath) -eq $true)
{
	$filePath = Read-Host "Please provide a file name to compress"
}

if (![System.IO.File]::Exists($filePath))
{
	echo "File not found"
	return
}

$zippedFileName = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($filePath), [System.IO.Path]::GetFileName($filePath) + ".zip")


$inputStream = [System.IO.File]::OpenRead($filePath)
$outputStream = [System.IO.File]::OpenWrite($zippedFileName)
$gZipStream = New-Object System.IO.Compression.GZipStream($outputStream, [System.IO.Compression.CompressionMode]::Compress)

$buffer = New-Object byte[] 1024

while($true)
{
	$len = $inputStream.Read($buffer, 0, $buffer.Length)
    if ($len -eq 0) { break }
    $gZipStream.Write($buffer, 0, $len);
}

$inputStream.Close()
$gZipStream.Close()