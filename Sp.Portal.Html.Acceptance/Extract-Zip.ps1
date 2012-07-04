param([string]$zipfilename, [string] $destination)

$zipfilename=(Resolve-Path $zipfilename).Path

if ( -not $( test-path($zipfilename))) {
	throw "$zipfilename does not exist"
}

$destination=(Resolve-Path $destination).Path

if ( -not $( test-path($destination))) {
	throw "$destination does not exist"
}

$shellApplication = new-object -com shell.application
$zipPackage = $shellApplication.NameSpace($zipfilename)
$destinationFolder = $shellApplication.NameSpace($destination)
$destinationFolder.CopyHere($zipPackage.Items())