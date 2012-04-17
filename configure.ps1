param(
	[string] $username = $(Read-Host -prompt "Software Potential username (account@domain.com)"),
	[string] $password = $(Read-Host -prompt "Software Potential password"),
	[bool] $skipValidation=$false, 
	[string] $baseUrl="https://srv.softwarepotential.com"
)

$msbuildProperties=@("TestAppConfigUsername=$username")
$msbuildProperties=$msbuildProperties+"TestAppConfigPassword=$password"
$msbuildProperties=$msbuildProperties+"TestAppConfigSkipCertValidation=$skipValidation"
$msbuildProperties=$msbuildProperties+"TestAppConfigBaseUrl=$baseUrl"

$properties="/p:$([string]::Join(';',$msBuildProperties))"

./build -t CustomizeAppConfigs -add "$properties"