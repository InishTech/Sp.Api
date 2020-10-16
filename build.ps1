# Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param(
	[string] $solution="build.proj",
	[string] $targets="Build",
	[string] $additionalMsBuildArgs="",
	[string] $configuration="Debug", 
	[string] $verbosity="n", 
	[string] $fileVerbosity="d",
	[string] $logFile="build.log"
)

function warn( [string]$message) {
	Write-Host "$message" -BackgroundColor Yellow
}

$msbuildProperties=@("Configuration=$configuration")

# the following will calculate the MSBuild path for VS2017 and above. Alternatively hard code your own MSBuild path 
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
if (-not (Get-Module -ListAvailable -Name VSSetup )){
	Install-Module VSSetup -Scope CurrentUser -Force
}
$instance = Get-VSSetupInstance -All | Select-VSSetupInstance -Product * -Require 'Microsoft.Component.MSBuild' -Latest # -Product * will include BuildTools install (without it is only full VSInstances)
$installDir = $instance.installationPath
$msBuild = $installDir + '\MSBuild\Current\Bin\MSBuild.exe'
if(Test-Path $installDir'\MSBuild\Current\Bin\MSBuild.exe'){
	$msBuild =   $installDir + '\MSBuild\Current\Bin\MSBuild.exe'
}elseif(Test-Path $installDir'\MSBuild\15.0\Bin\MSBuild.exe'){
	$msBuild =  $installDir +  '\MSBuild\15.0\Bin\MSBuild.exe';
}
else {
	$msbuild="$env:windir\Microsoft.NET\Framework\v4.0.30319\MSBuild"
}
$properties="/p:$([string]::Join(';',$msBuildProperties))"

warn "Starting at $([datetime]::Now)" 
$parameters=@("$solution","/v:$verbosity","/m","/t:$targets","/fl","/flp:LogFile=$logFile;Verbosity=$fileVerbosity",$properties,$additionalMsBuildArgs)
Write-Host "Running $msbuild $parameters"
. $msbuild @parameters
