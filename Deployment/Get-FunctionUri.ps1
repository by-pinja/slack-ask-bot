<#
    .SYNOPSIS
    Retrieves function uri that needs to be configured to Slack API
    
    .DESCRIPTION
    This assumes that user has already logged in with az login.
    
    .PARAMETER ResourceGroup
    Name of the resource group that has the web app deployed
    
    .PARAMETER WebAppName
    Name of the target web app

    .PARAMETER FunctionName
    Name of the function web hook
    
    .EXAMPLE
    .\Publish.ps1 -ResourceGroup "github-test" -WebAppName "hjni-test"
 #>
param(
    [Parameter(Mandatory)][string]$ResourceGroup,
    [Parameter()][string]$WebAppName = $ResourceGroup,
    [Parameter(Mandatory)][string]$FunctionName
)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
    
. "./Deployment/FunctionUtil.ps1"

$kuduCreds = Get-KuduCredentials $WebAppName $ResourceGroup
$code = Get-FunctionKey $WebAppName $FunctionName $kuduCreds
$url = Get-InvokeUrl $WebAppName $FunctionName $kuduCreds
return $url + "?code=" + $code