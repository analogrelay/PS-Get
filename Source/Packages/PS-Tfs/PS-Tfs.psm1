dir $PsScriptRoot\Scripts\*.ps1 | ForEach-Object {
  . $_
  Export-ModuleMember -Function "$($_.BaseName)"
}
