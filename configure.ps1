# Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param(
	[string] $username = $(Read-Host -prompt "Software Potential username (account@domain.com)"),
	[string] $password = $(Read-Host -prompt "Software Potential password"),
	[bool] $skipValidation=$false,
	[string] $baseUrl="https://web.softwarepotential.com",
	[string] $portalBaseUrl="https://portal.softwarepotential.com"
)

$msbuildProperties=@("TestAppConfigUsername=$username")
$msbuildProperties=$msbuildProperties+"TestAppConfigPassword=$password"
$msbuildProperties=$msbuildProperties+"TestAppConfigSkipCertValidation=$skipValidation"
$msbuildProperties=$msbuildProperties+"TestAppConfigBaseUrl=$baseUrl"
$msbuildProperties=$msbuildProperties+"TestAppConfigPortalBaseUrl=$portalBaseUrl"

$properties="/p:$([string]::Join(';',$msBuildProperties))"

./build build.proj -t CustomizeAppConfigs -add "$properties"