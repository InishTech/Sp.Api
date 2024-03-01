# Copyright (c) 2019 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param( [string] $path, [string] $zipFilename, [string] $os )

#requires -version 3
$ErrorActionPreference = "Stop"
$chromeDriverVersionPath = "$path/chromeDriverVersion.txt"
$zipPath = "$path\$zipFilename"
$versionUrl = "https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json"
$lastKnownGoodVersions = Invoke-RestMethod $versionUrl
$currentStable = $lastKnownGoodVersions.channels.stable
$version = $currentstable.version
$platformObject = $currentStable.downloads.chromedriver | Where-Object {$_.platform -eq $os} |  Select-Object -First 1
$downloadUrl = $platformObject.url;

Write-Host $downloadUrl

if(Test-Path $chromeDriverVersionPath) {
	$chromeDriverVersion = Get-Content -Path $chromeDriverVersionPath
}

if($chromeDriverVersion -eq $version) {
	Write-Host "ChromeDriver is up to date at $chromeDriverVersion. Skipping download."
	return
}

New-Item -Path $chromeDriverVersionPath -ItemType "file" -Value "$version" -Force
Write-Host "Downloading $downloadUrl to $zipPath"
(New-Object System.Net.WebClient).DownloadFile( $downloadUrl, $zipPath)
Unblock-File $zipPath

& $PSScriptRoot\Extract-Zip.ps1 $zipPath $path;