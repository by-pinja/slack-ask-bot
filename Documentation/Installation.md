# Installation

This document describes how this application is installed.

## Requirements

In addition to build requirements, following thing are needed for setting up
the development environment where this application can be fully tested or used.

* Admin access to Slack Workspace where you wish to install this application.
* Subscription in Azure
* Powershell
* Azure Powershell Module

## Step by step

1. [Create Slack Application](SlackApp.md)
1. [Create developer-settings.json](DeveloperSettings.md)
1. [Create Azure Function app](FunctionApp.md)
1. [Set Azure Function Url to Slack application](SlackApp.md#setting-function-app-url)

After installation, the Slack Workspace where the Slack Application was
installed should have shortcuts available for creating questionnaires.

[For more information about shortcuts](https://api.slack.com/interactivity/shortcuts)
