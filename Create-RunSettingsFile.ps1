<#
    .SYNOPSIS
    Retrieves necessary values from Azure and creates .runsettings-file. This should only be executed from the continuous integration.
    .DESCRIPTION
    The Azure environment should already exist and the webapp already deployed.
    .PARAMETER ResourceGroup
    Name of the resource group that has the web app deployed.    
    .PARAMETER WebAppName
    Name of the target web app. If not set, resource group name is used.
#>
param(
    [Parameter(Mandatory)][string]$ResourceGroup,
    [Parameter()][string]$WebAppName = $ResourceGroup
)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$address = ./Deployment/Get-FunctionUri.ps1 `
    -ResourceGroup $ResourceGroup `
    -WebAppName $WebAppName `
    -FunctionName 'AskBotHook'

# *table is the suffix of the storage account which is used for storage.
$storageAccount = Get-AzStorageAccount -ResourceGroupName 'slack-ask-bot-hjni' | Where-Object { $_.StorageAccountName -like '*table' }
$storageAccountKey = ( $storageAccount `
    | Get-AzStorageAccountKey)[0].Value

$connectionString = "DefaultEndpointsProtocol=https;AccountName=$($storageAccount.StorageAccountName);AccountKey=$storageAccountKey;EndpointSuffix=core.windows.net" 

[xml]$document = New-Object System.Xml.XmlDocument
$declaration = $document.CreateXmlDeclaration('1.0', 'UTF-8', $null)
$document.AppendChild($declaration)
$root = $document.CreateElement('RunSettings')
$document.AppendChild($root)

$parameters = $document.CreateElement('TestRunParameters')
$root.AppendChild($parameters)

$appNameNode = $document.CreateElement('Parameter')
$appNameNode.SetAttribute('name', 'StorageConnectionString')
$appNameNode.SetAttribute('value', $connectionString)
$parameters.AppendChild($appNameNode);

$appNameNode = $document.CreateElement('Parameter')
$appNameNode.SetAttribute('name', 'FunctionAppUrl')
$appNameNode.SetAttribute('value', $address)
$parameters.AppendChild($appNameNode);

Write-Host "Create settings..."
$document.save("AcceptanceTests/.runsettings")
$document.save(".runsettings")
