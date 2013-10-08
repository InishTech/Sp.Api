# Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param( [string] $filename, [string] $namePrefix, [string] $url, [string] $indexPageType )

#requires -version 3
$ErrorActionPreference = "Stop"

function extractDownloadUrlFromGoogleCodeIndexPage($pageResult){
	$href = $pageResult.AllElements |
          where TagName -eq "a" |
          where href -match $namePrefix |
          where title -eq Download | 
          select -ExpandProperty href |
          select -first 1 
    return "http:$href"
}

function extractDownloadUrlFromGoogleCloudStorageIndexPage($pageResult){
	[xml] $releaseIndex = $pageResult
	$lastRelease = $releaseIndex.ListBucketResult.Contents | where { $_.Key.Contains($namePrefix) } | sort Key -desc | select -first 1
	return "$url/$($lastRelease.Key)"
}

$result = Invoke-WebRequest $url 

if( $indexPageType -eq "GoogleCloudStorage"){
	$downloadUrl = extractDownloadUrlFromGoogleCloudStorageIndexPage $result
}
else {
	$downloadUrl = extractDownloadUrlFromGoogleCodeIndexPage $result
}

Write-Host "Downloading $downloadUrl to $filename"

(New-Object System.Net.WebClient).DownloadFile( $downloadUrl, $filename)
Unblock-File $filename
