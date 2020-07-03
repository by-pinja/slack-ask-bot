<#
    .SYNOPSIS
    Connects to Azure

    .DESCRIPTION
    Connects the powershell session to azure

    Note: this should be no longer needed after generic azure login
    option is added to jenkins-ptcs-library

    .PARAMETER ApplicationId
    Application ID ("username" of Service Principal)

    .PARAMETER ApplicationKey
    Key ("password" of Service Principal)

    .PARAMETER TenantId
    Tenant ID

    .PARAMETER SubscriptionId
    Subscription Id
#>
[CmdLetBinding()]
param(
    [Parameter(Mandatory)][string]$ApplicationId,
    [Parameter(Mandatory)][string]$ApplicationKey,
    [Parameter(Mandatory)][string]$TenantId,
    [Parameter(Mandatory)][string]$SubscriptionId
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$securePassword = ConvertTo-SecureString -String $ApplicationKey -AsPlainText -Force
$credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $ApplicationId, $securePassword
Connect-AzAccount -Credential $credential -TenantId $TenantId -Subscription $SubscriptionId -ServicePrincipal