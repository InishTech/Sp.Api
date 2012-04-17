param(
	[string] $targets="Build",
	[string] $additionalMsBuildArgs="",
	[string] $configuration="Debug", 
	[string] $verbosity="n", 
	[string] $fileVerbosity="d"
)

function warn( [string]$message) {
	Write-Host "$message" -BackgroundColor Yellow
}

$msbuildProperties=@("Configuration=$configuration")

# using FW64 breaks in Slps Sdk versions <1928
$msbuild="$env:windir\Microsoft.NET\Framework\v4.0.30319\MSBuild"
$properties="/p:$([string]::Join(';',$msBuildProperties))"

warn "Starting at $([datetime]::Now)" 
$parameters=@("$(dir *.sln)","/v:$verbosity","/m","/t:$targets","/fl","/flp:LogFile=build.log;Verbosity=$fileVerbosity",$properties,$additionalMsBuildArgs)
Write-Host "Running $msbuild $parameters"
. $msbuild @parameters