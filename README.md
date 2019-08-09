# Slack AskBot
A bot to perform small questionaries in Slack

This is still a very much a work in progress so this shouldn't probably be used by anyone.

## Roadmap
 1. Loading questionnaire from file / console / etc
 1. Deployment from CI
 1. Answer reporting to proper place
 1. Table storage cleanup

## Setup steps

### Creating application
1. Create the environment
   For example 
   ```
   New-AzResourceGroupDeployment `
    -Name "my-deploy-name" `
    -TemplateFile Deployment/azuredeploy.json `
    -ResourceGroupName "my-resource-group" `
    -appName "my-function-app-name" `
    -environment Development
   ```

   Save tableStorageConnection that is generated by the deployment. This can be later fetched from
   table storage settings. This is used by ConsoleTester to manage questionnaires.
1. Publish application
    Following will build and publis this application
    ```
    .\Deployment\Publish.ps1 -ResourceGroup "my-resource-group" -WebAppName "my-function-app-name"
    ```
1. Retrieve function url
    Azure Functions Webhook URL can only be retrieved after the application is deployed.
   ```
   .\Deployment\GetFunctionUri.ps1 -ResourceGroup "my-resource-group" -WebAppName "my-function-app-name"
   ```

   Note: This script may throw `The underlying connection was closed: An unexpected error occurred on a send.`
   or similar error if wrong SecurityProtocol is used. This requires TLS 1.2.

   Following will fix the issue for PowerShell session
   ```
   [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12;
   ```
1. Change logic apps web hook uri at https://api.slack.com/apps "Interactive Components"

## Usage
Console interface of this software uses [CommandLineParser](https://github.com/commandlineparser/commandline).

Using command line parameters differs a little bit depending on where you run this software.

Examples in this section assume that commands are run from `ConsoleTester`-folder.

For up to date help

```
dotnet run -- help 
```

### Configurating ConsoleTester
Configurations are read from `appsettings.json` and `appsettings.Development.json`.

For actual usage, set connection string generated in [Create Application](#create-application) as
ConnectionString under TableStorage-section in `appsettings.json`-file

For development, create `appsettings.Delopment.json` and fill your
storage connections string generated earlier there.

### Adding new channels / Configuring Slack Incoming Webhooks
These are the target addresses for our questionaires. These should be available in https://api.slack.com/apps "Incoming Webhooks"

Use `upsertChannel` to add or update these

Example
```
dotnet run -- upsertChannel -c channel-here -w https://hooks.slack.com/services/HOOK_HERE
```

### Creating new questionnaires
Questionnaires are created by reading JSON file.

1. Generate questionnaire template `dotnet run -- generateTemplate -o 'example-questionnaire.json'` 
1. Change question, add/change/remove answers
1. Add questionnaire `dotnet run -- create -f 'example-questionnaire.json' -c test-channel`

Example questionnaire JSON
```
{
  "Question": "How are our code monkeys doing today?",
  "AnswerOptions": [
    "Yes",
    "Very much no.",
    ":feelsbadman:"
  ]
}
```

### Fetching answers

Fetching questionnaire ID:s
```
dotnet run -- questionnaires
```

Fetching answers for all questionnaires
```
dotnet run -- answers
```

Fetching answers for single questionnaire
```
dotnet run -- answers -q e746a0db-6984-4fc4-8d6d-b5fad5baaa90
```

Writing answers to CSV-file
```
dotnet run -- answers -q e746a0db-6984-4fc4-8d6d-b5fad5baaa90 -o test.csv
```