<#
    .SYNOPSIS
    Send request including json to validation endpoint to test validation

    .DESCRIPTION
    This function can be used the test Azure Functions handler endpoint is running.

    .PARAMETER SettinsFile
    Settings file that includes environment settings.
    Defaults to 'developer-settings.json'
#>
[CmdLetBinding()]
param(
    [Parameter(Mandatory)][string]$SettingsFile = 'developer-settings.json'
)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Write-Host "Reading settings from file $SettingsFile"
$settingsJson = Get-Content -Raw -Path $SettingsFile | ConvertFrom-Json

$address = ./Deployment/Get-FunctionUri.ps1 `
    -ResourceGroup $settingsJson.ResourceGroupName `
    -FunctionName 'AnswerHandlerHook'

# Minimum required to get a response
$json = @{
    'type'        = 'dialog_submission'
    'callback_id' = 'Test call back'
    'channel'     = @{
        'name' = 'Test channel name'
    }
    'user'        = @{
        'name' = 'Test user name'
    }
    'submission'  = @{
        'answer' = 'Test answer'
    }
} | ConvertTo-Json

$params = "payload=$json"

Write-Host 'Send post request'
Invoke-RestMethod -Method POST -Uri $address -Body $params -ContentType 'application/json;charset=UTF-8'
Write-Host 'Finished execution'