# Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
#  
# This code is licensed under the BSD 3-Clause License included with this source
# 
# FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License 
param( [string] $filename, [string] $namePrefix, [string] $url )

#requires -version 3

$result = Invoke-WebRequest $url 

$href = $result.AllElements |
    where TagName -eq "a" |
    where href -match $namePrefix |
    where title -eq Download | 
    select -ExpandProperty href |
    select -first 1 

$downloadUrl="http:$href"

Write-Host "Downloading $downloadUrl to $filename"

(New-Object System.Net.WebClient).DownloadFile( $downloadUrl, $filename)
Unblock-File $filename