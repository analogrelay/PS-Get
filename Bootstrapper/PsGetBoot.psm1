$env:PSModulePath = "$($env:PSModulePath);$PSScriptRoot\packages"
Import-Module PS-Get* -args $PSScriptRoot