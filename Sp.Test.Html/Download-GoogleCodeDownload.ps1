# Copyright (c) 2019 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param( [string] $filename, [string] $namePrefix, [string] $url )

#requires -version 3
$ErrorActionPreference = "Stop"

function extractDownloadUrlFromGoogleCloudStorageIndexPage($pageResult, $release) {
    [xml] $releaseIndex = $pageResult
    $file = $releaseIndex.ListBucketResult.Contents | Where-Object { $_.Key.Contains($namePrefix) } | Where-Object { $_.Key.Contains($release) }
    return 	"$url/$($file.Key)"
}

$result = Invoke-WebRequest $url
$latestRelease = Invoke-WebRequest "$url/LATEST_RELEASE"
$downloadUrl = extractDownloadUrlFromGoogleCloudStorageIndexPage $result $latestRelease

Write-Host "Downloading $downloadUrl to $filename"

(New-Object System.Net.WebClient).DownloadFile( $downloadUrl, $filename)
Unblock-File $filename
