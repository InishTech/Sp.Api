# Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param(
	[string]$zipfilename, 
	[string] $destination
)

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