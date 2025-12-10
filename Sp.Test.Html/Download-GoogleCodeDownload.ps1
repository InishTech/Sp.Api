# Copyright (c) 2019 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param( [string] $path, [string] $zipFilename, [string] $os, [bool] $useInstalledVersion = $true )

#requires -version 3
$ErrorActionPreference = "Stop"

function Invoke-WithBackoff {
	param (
		[scriptblock] $func, 
		[scriptblock] $backoffFunc,
		[Type] $exceptionType,
		[int] $minBackoff = 50, 
		[int] $maxBackoff = 5000, 
		[int] $maxAttempts = 5
	) 
  	for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
        try {
            return & $func
        } catch {

			if ($exceptionType -eq $null -or $_.Exception.GetType() -eq $exceptionType) {
                $backoff = & $backoffFunc $attempt
				if($backoff -gt $maxBackoff) {
					$backoff = $maxBackoff
				}
				Write-Host $_
                Write-Host "Attempt $attempt failed. Retrying in $backoff ms..."
                Start-Sleep -Milliseconds  $backoff
			}
			else {
				throw $_
			}
		}
	}

	throw "Max attempts ($maxAttempts) reached."
}

function Invoke-WithRandomBackoff {
	param (
		[scriptblock] $func, 
		[Type] $exceptionType,
		[int] $minBackoff = 50, 
		[int] $maxBackoff = 5000, 
		[int] $maxAttempts = 5
	)

	Invoke-WithBackoff $func {
		return Get-Random -Minimum $minBackoff -Maximum $MaxBackoff
	} $exceptionType $minBackoff $maxBackoff $maxAttempts
}

function Invoke-WithExponentialBackoff {
	param (
		[scriptblock] $func, 
		[Type] $exceptionType,
		[int] $minBackoff = 50, 
		[int] $maxBackoff = 5000, 
		[int] $maxAttempts = 5
	)

	Invoke-WithBackoff $func {
		return   ([Math]::Pow( $attempt, 2)) * $minBackoff
	} $exceptionType $minBackoff $maxBackoff $maxAttempts
}

function Get-ExeVersion($exePath) {
	return [System.Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).ProductVersion
}

$installedChromeVersion = Get-ExeVersion "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
$extractedDriverPath = "$path\chromedriver-$os\chromedriver.exe"
$isDriverPresent = Test-Path $extractedDriverPath

if($useInstalledVersion) {
	Write-Host "Currently installed Chrome version: $installedChromeVersion"
	
	$downloadUrl = "https://storage.googleapis.com/chrome-for-testing-public/$installedChromeVersion/win32/chromedriver-$os.zip"

	$extractedDriverVersion = Get-ExeVersion $extractedDriverPath
	if($isDriverPresent -and $extractedDriverVersion -eq $installedChromeVersion) {
		Write-Host "Chromedriver is already matches installed Chrome. Skipping download."
		return;
	}
} else {
	$chromeDriverVersionPath = "$path/chromeDriverVersion.txt"
	$versionUrl = "https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json"
	$lastKnownGoodVersions = Invoke-RestMethod $versionUrl
	$currentStable = $lastKnownGoodVersions.channels.stable
	$currentStableVersion = $currentstable.version
	Write-Host "Current stable version: $currentStableVersion"

	$platformObject = $currentStable.downloads.chromedriver | Where-Object {$_.platform -eq $os} |  Select-Object -First 1
	$downloadUrl = $platformObject.url;

	$lastDownloadedVersion = 'Unknown'

	if(Test-Path $chromeDriverVersionPath) {
		$lastDownloadedVersion = Get-Content -Path $chromeDriverVersionPath
	}
	Write-Host "Last downloaded version: $lastDownloadedVersion"
	
	if($lastDownloadedVersion -eq $currentStableVersion -and $isDriverPresent ) {
		Write-Host "ChromeDriver is up to date at $lastDownloadedVersion and chromedriver.exe is present. Skipping download."
		return
	}
	
	Invoke-WithRandomBackoff {
		Write-Host "Writing current stable version to file..."
		New-Item -Path $chromeDriverVersionPath -ItemType "file" -Value "$currentStableVersion" -Force
		
		Write-Host "This attempt at restoring the Chrome driver restore failed: $($_.Exception.Message)"
	} System.IO.IOException
}

$zipPath = "$path\$zipFilename"
Write-Host "Downloading $downloadUrl to $zipPath..."

Invoke-WithExponentialBackoff {
	(New-Object System.Net.WebClient).DownloadFile( $downloadUrl, $zipPath)
}

Invoke-WithRandomBackoff {
	Unblock-File $zipPath

	Write-Host "Extracting zipped download to $path..."
	& $PSScriptRoot\Extract-Zip.ps1 $zipPath $path
}  System.IO.IOException

if(Test-Path $extractedDriverPath) {
	Write-Host "Chromedriver extracted to $extractedDriverPath"
} else {
	Write-Host "Failed to extract Chromedriver to $extractedDriverPath"
}
