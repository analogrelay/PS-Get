param(
	[string]$InstallPath = $null, 
	[switch]$NoPrompt, 
	[string]$NuGetExeUrl = "http://cloud.github.com/downloads/anurse/PS-Get/NuGet.exe", 
	[string]$PsGetFeed = "http://packages.psget.org"
)

if([Environment]::Version.Major -lt 4) {
	Write-Host "PS-Get requires .NET 4.0. The easiest way to do this is to download the PowerShell 3.0 CTP from http://www.microsoft.com/download/en/details.aspx?id=27548"
	throw "Installation Failed."
}

function YesNoPrompt($msg) {
	while($answer -eq $null -or $answer.ToLowerInvariant() -ne "yes") {
		$answer = Read-Host $msg
		if($answer.ToLowerInvariant() -eq "no") {
			return $false
		}
	}
	return $true
}

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
	Write-Host "Preparing to install PS-Get to $InstallPath"
	Write-Host "Modules installed via PS-Get will be installed into this folder as well"
	if(!(YesNoPrompt "Ready to install? Type YES to start installing, NO to cancel")) {
		return;
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
	mkdir $InstallPath | Out-Null
}

pushd $InstallPath
$result = &$tmpExe install PS-Get -Source $PsGetFeed -ExcludeVersion 2>&1  
if($LASTEXITCODE -gt 0) {
	throw "PS-Get could not be installed from $PsGet. Error from NuGet.exe:`r`n$result";
}
Write-Host "`tRemoving Temporary NuGet.exe"
del $tmpExe
popd

# Ask the user if we should add it to the profile
Write-Host

Write-Host "PS-Get is now installed, activate it by running 'Import-Module PS-Get'"
Write-Host "Would you like PS-Get to add an entry to your profile to import PS-Get?"
Write-Host "NOTE: This will add the import entry to the end of your profile file, if you have customized your profile you may wish to do this manually"
Write-Host "The installer does not check if the code already exists, so this may result in duplicated code if you've already installed PS-Get"
Write-Host "According to our information, your profile is in: $profile"
Write-Host "If this is incorrect, you should add the Import line manually"
if(YesNoPrompt "Add PS-Get to your profile script?") {
	# Make sure there's a profile folder
	if(!(Test-Path (Split-Path -Parent $profile))) {
		mkdir (Split-Path -Parent $profile)
	}

	# Add Import lines to the end of the profile
	"`r`n" | Out-File -FilePath $profile -Append -Encoding ASCII
	"### Added by PS-Get Installer, feel free to move elsewhere if you know what you're doing ;) ###" | Out-File -FilePath $profile -Append -Encoding ASCII
	"# Import PS-Get Core Module, this line must happen before the others in this block" | Out-File -FilePath $profile -Append -Encoding ASCII
	"Import-Module PS-Get" | Out-File -FilePath $profile -Append -Encoding ASCII
	"### END Added by PS-Get Installer section ###" | Out-File -FilePath $profile -Append -Encoding ASCII

	Write-Host "Done! Code added to profile"
}

# Register the Main PS-Get Source
Import-Module PS-Get
Add-PackageSource -Scope Machine -Name "PS-Get Gallery" -Source $PsGetFeed
Remove-Module PS-Get

Write-Host
Write-Host "PS-Get has been installed successfully!"