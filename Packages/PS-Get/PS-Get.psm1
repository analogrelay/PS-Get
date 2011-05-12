$PsGetInstallRoot = (Convert-Path $PSScriptRoot\..)

function Install-PSPackage($Id = $(throw 'Id is required'), $Version = $null, $Source = "https://go.microsoft.com/fwlink/?LinkID=206669") {
		pushd $PsGetInstallRoot
		$versionArg = ""
		if($Version -ne $null) {
			$versionArg = "-v $Version"
		}
		& "$PSScriptRoot\NuGet.exe" install -x -s $Source $Id $versionArg
		popd
}
Export-ModuleMember -Function Install-PSPackage

function Get-PSPackage($Source = "https://go.microsoft.com/fwlink/?LinkID=206669") {
		& "$PSScriptRoot\NuGet.exe" list -s $Source
}
#Foo
Export-ModuleMember -Function Get-PSPackage