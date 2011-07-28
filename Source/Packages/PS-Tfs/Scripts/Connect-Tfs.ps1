function Connect-Tfs {
  param(
    [Parameter(Mandatory=$false, Position=0)][string]$PathOrServerUri = "."
  )

  [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Client") | Out-Null
  [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.WorkItemTracking.Client") | Out-Null
  [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.VersionControl.Client") | Out-Null

  $LocalPath = $null
  $ServerUri = $null
  if(![Uri]::TryCreate($PathOrServerUri, "Absolute", [ref]$ServerUri)) {
    $LocalPath = Convert-Path $PathOrServerUri
  } elseif($ServerUri.Scheme -ne [Uri]::UriSchemeHttp -and $ServerUri.Scheme -ne [Uri]::UriSchemeHttps) {
    $LocalPath = Convert-Path $PathOrServerUri
  }

  if(![String]::IsNullOrEmpty($LocalPath)) {
    $WorkspaceInfo = [Microsoft.TeamFoundation.VersionControl.Client.Workstation]::Current.GetLocalWorkspaceInfo($LocalPath)
    if($WorkspaceInfo -eq $null) {
      throw "Unable to determine the workspace for $LocalPath"
    } else {
      $ServerUri = $WorkspaceInfo.ServerUri
    }
  }

  New-Object Microsoft.TeamFoundation.Client.TfsTeamProjectCollection $ServerUri
}