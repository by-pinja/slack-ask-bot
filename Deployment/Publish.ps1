<#
    .SYNOPSIS
    Packs and publishes AzureFunctions to Azure.
    
    .DESCRIPTION
    Packs and publishes AzureFunctions to Azure.
    Scripts expects that the web app is already created.
    
    This assumes that user has already logged in with az login.
    
    .PARAMETER ResourceGroup
    Name of the resource group that has the web app deployed
    
    .PARAMETER WebAppName
    Name of the target web app
    
    .EXAMPLE
    .\Publish.ps1 -ResourceGroup "github-test" -WebAppName "hjni-test"
#>
param(
    [Parameter(Mandatory)][string]$ResourceGroup,
    [Parameter()][string]$WebAppName = $ResourceGroup,
    [Parameter()][string]$Project = "AzureFunctions",
    [Parameter()][string]$VersionSuffx = "DEV")

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$publishFolder = "publish"

# delete any previous publish
if (Test-path $publishFolder) { Remove-Item -Recurse -Force $publishFolder }

dotnet publish -c Release -o $publishFolder $Project --version-suffix $VersionSuffx

$destination = "publish.zip"
$fullSourcePath = (Resolve-Path "$publishFolder").Path
$fullTargetPath = (Resolve-Path ".\").Path
$fullZipTarget = Join-Path -Path $fullTargetPath -ChildPath $destination

Write-Host "Compressing $fullSourcePath\*"
Compress-Archive -Path "$fullSourcePath\*" -DestinationPath $fullZipTarget -Force
Write-Host "Deploying $fullZipTarget to $WebAppName in $ResourceGroup..."
$webApp = Get-AzWebApp -ResourceGroupName $ResourceGroup -Name $WebAppName
Publish-AzWebApp -WebApp $webApp -ArchivePath $fullZipTarget -Force
Write-Host "Published."