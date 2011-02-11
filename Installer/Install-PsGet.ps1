param(
	[Parameter(Mandatory=$false)][string]$NuGetSource = "http://anur.se/nugetexe",
	[Parameter(Mandatory=$false)][string]$PsGetSource = "https://go.microsoft.com/fwlink/?LinkID=206669"
)

Write-Host "Preparing to Install PsGet, press Ctrl-C at any time to cancel..."

Write-Host "PsGet needs to be installed in your PSModulePath."
Write-Host "The installer found the following paths in your PSModulePath:"
$paths = $env:PSModulePath.Split(";")
$i = 0;
$paths | ForEach-Object {
	$i += 1
	Write-Host "$i) $_"
}
$selected = Read-Host "Which of the paths do you want to install PsGet into?"
$installPath = $paths[([int]$selected) - 1]

if(!(Test-Path $installPath)) {
	Write-Host "Creating directory $installPath"
	mkdir $installPath | Out-Null
}

Write-Host "Installing PsGet to $installPath"
pushd $installPath

$temp = [System.IO.Path]::GetTempPath();
if(Test-Path $temp\NuGet.exe) {
	del $temp\NuGet.exe
}

Write-Host "Downloading NuGet.exe Command Line Tool..."
$wc = New-Object System.Net.WebClient
$wc.DownloadFile($NuGetSource, "$(Convert-Path $temp)\NuGet.exe");
Write-Host "Updating to latest version of NuGet.exe"
& $temp\NuGet.exe update
del $temp\NuGet.exe.old

Write-Host "Using NuGet.exe to install PsGetBoot package"
& $temp\NuGet.exe install PS-Get -s $PsGetSource

Write-Host "Deleting Temporary version of NuGet.exe..."
del $temp\NuGet.exe
popd