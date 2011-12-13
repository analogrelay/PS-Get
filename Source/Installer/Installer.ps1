param(
	[string]$InstallPath = $null, 
	[switch]$NoPrompt, 
	[string]$NuGetExeUrl = "http://cloud.github.com/downloads/anurse/PS-Get/NuGet.exe", 
	[string]$PsGetFeed = "http://www.myget.org/F/psget/"
)
#$ExpectedHash = "0000000";
$ExpectedHash = "386121DCF41F16B8CA6B0F9945F65A9D01E08B0E";

$paths = @($env:PsModulePath.Split(';'))

while([String]::IsNullOrEmpty($InstallPath)) {
	Write-Host "Please select a path to install PS-Get to" -ForegroundColor Yellow
	$i = 1;
	$paths | ForEach-Object {
		Write-Host "$i) $_"
		$i += 1
	}
	$selection = Read-Host "Select an entry"
	[int]$parsed = 0
	$success = $false
	if([Int32]::TryParse($selection, [ref]$parsed)) {
		if(($parsed-1 -ge 0) -and ($parsed -le $paths.Length)) {
			$InstallPath = $paths[$parsed - 1]
			$success = $true
		}
	}
	if(!$success) {
		Clear-Host
		Write-Host "Invalid Selection" -ForegroundColor Red
	}
}

if(!$NoPrompt) {
	$answer = $null
	while($answer -eq $null -or $answer.ToLowerInvariant() -ne "yes") {
		Write-Host "Preparing to install PS-Get to $InstallPath"
		Write-Host "Modules installed via PS-Get will be installed into this folder as well"
		$answer = Read-Host "Ready to install? Type YES to start installing, NO to cancel"
		if($answer.ToLowerInvariant() -eq "no") {
			return;
		}
	}
}

Clear-Host
Write-Host "Installing PS-Get to $InstallPath ..."
Write-Host "Fetching NuGet.exe"
Write-Host "`tDownloading $NuGetExeUrl"
$tmpExe = "$([System.IO.Path]::ChangeExtension([System.IO.Path]::GetTempFileName(), ".NuGet.exe"))"
$wc = New-Object System.Net.WebClient
$wc.DownloadFile($NuGetExeUrl, $tmpExe)

$sha1 = [System.Security.Cryptography.SHA1Managed]::Create()
Write-Host "`tVerifying Hash"
$strm = [System.IO.File]::OpenRead($tmpExe)
$hash = [String]::Join("", ($sha1.ComputeHash($strm) | foreach { $_.ToString("X").PadLeft(2, "0") }))
$strm.Dispose()
if(![String]::Equals($hash, $ExpectedHash)) {
	Write-Host "`t`tExpected: $ExpectedHash"
	Write-Host "`t`tActual: $hash"
	throw "Hash did not match expected value, NuGet.exe may have been tampered with. Automated install cannot continue."
}

# Run it once to update it
Write-Host "`tRunning NuGet Self-Updater"
&$tmpExe | Out-Null

# Now install PS-Get in the modules directory without side-by-side
Write-Host "Installing PS-Get Package"
Write-Host "`tInstalling PS-Get from $PsGetFeed"

if(!(Test-Path $InstallPath)) {
	mkdir $InstallPath
}

pushd $InstallPath
&$tmpExe install PS-Get -Source $PsGetFeed -ExcludeVersion
Write-Host "`tRemoving Temporary NuGet.exe"
del $tmpExe
