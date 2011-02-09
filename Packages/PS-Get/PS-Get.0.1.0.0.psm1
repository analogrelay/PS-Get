$script:PackageRoot = $args[0]

function Install-PSPackage($Id = $(throw 'Id is required'), $Version = $null, $Source = "https://go.microsoft.com/fwlink/?LinkID=206669") {
		pushd $script:PackageRoot
		$versionArg = ""
		if($Version -ne $null) {
			$versionArg = "-v $Version"
		}
		& "$PSScriptRoot\NuGet.exe" install -s $Source $Id $versionArg
		popd
}
Export-ModuleMember -Function Install-PSPackage

function Get-PSPackage($Search = "", $Source = "https://go.microsoft.com/fwlink/?LinkID=206669") {
		& "$PSScriptRoot\NuGet.exe" list -s $Source
}
Export-ModuleMember -Function Get-PSPackage

$env:PSModulePath = "$($env:PSModulePath);$($script:PackageRoot)\packages"