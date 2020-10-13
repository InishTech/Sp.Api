# Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param(
	[string] $Username = $(Read-Host -prompt "Software Potential username (account@domain.com)"),
	[string] $portalUsername = $(Read-Host -prompt "Software Potential Portal username (account@domain.com)"),
	[string] $password = $(Read-Host -prompt "Software Potential password"),
	[string] $portalPassword = $(Read-Host -prompt "Software Potential Portal password"),	
	[string] $clientId = $(Read-Host -prompt "Software Potential api client id (Client can be registered within the srv.softwarepotential.com serverice)"),	
	[string] $clientSecret = $(Read-Host -prompt "Software Potential api client secret"),
	[string] $baseUrl="https://srv.softwarepotential.com",
	[string] $authority="https://sts.softwarepotential.com",
	[string] $authBaseUrl="https://auth.softwarepotential.com",
	[string] $portalBaseUrl="https://srv.softwarepotential.com/portal",	
	[bool] $skipValidation=$false
)

$msbuildProperties=@("TestAppConfigUsername=$username")
$msbuildProperties=$msbuildProperties+"TestAppConfigPassword=$password"
$msbuildProperties=$msbuildProperties+"TestAppConfigBaseUrl=$baseUrl"

$msbuildProperties=$msbuildProperties+"TestAppConfigClientId=$clientId"
$msbuildProperties=$msbuildProperties+"TestAppConfigClientSecret=$clientSecret"
$msbuildProperties=$msbuildProperties+"TestAppConfigAuthority=$authority"
$msbuildProperties=$msbuildProperties+"TestAppConfigAuthBaseUrl=$authBaseUrl"
$msbuildProperties=$msbuildProperties+"TestAppConfigPortalBaseUrl=$portalBaseUrl"
$msbuildProperties=$msbuildProperties+"TestAppConfigPortalUsername=$portalUsername"
$msbuildProperties=$msbuildProperties+"TestAppConfigPortalPassword=$portalPassword"
$msbuildProperties=$msbuildProperties+"TestAppConfigSkipCertValidation=$skipValidation"

$properties="/p:$([string]::Join(';',$msBuildProperties))"

./build build.proj -t CustomizeAppConfigs -add "$properties"