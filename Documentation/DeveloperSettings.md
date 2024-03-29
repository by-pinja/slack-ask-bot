# Developer settings

This document describes how `developer-settings.json` is generated
and how it is used.

NOTE: `developer-settings.json` should be ignored in version control because
it contains developer specific settings which should/may be different for
each developer. This same thing could also be implemented with environment
variables.

## Creating settings

1. Copy `developer-settings.example.json` and rename it as
`developer-settings.json`
1. Replace values in `developer-settings.json` with values that you want to use
   * **ResourceGroupName** is the name of the resource group that is created to
  to Azure. It is also the name of the Azure Function App which is created.
   * **Location** is the resource location for Resource Group and the resources
   inside it. [See here for more information](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/resource-location?tabs=azure-powershell)
   * **SlackBearerToken** is generated by Slack and can be only known after Slack
   App is created. See [Slack App instructions](SlackApp.md)
   * **Tags** is optional. These tags are set for Azure Resource Group.

## How these are used

These settings are used by `Deployment/Prepare-Environment.ps1` script. User
can also give different settings file with parameter.
