# Slack AskBot
A bot to perform small questionaries in Slack

This is still a very much a work in progress so this shouldn't probably be used by anyone.

## (Bad) Setup steps

### Creating application
1. Create a google sheet
2. Change googleSheetId in variables to match your google sheet.
   This is the identifying part of the URL. For example, in https://docs.google.com/spreadsheets/d/mock-id/edit#gid=0
   the ID is mock-id
3. Create the environment
   For example `New-AzResourceGroupDeployment -Name "my-deploy-name" -TemplateFile Deployment/azuredeploy.json -ResourceGroupName "my-resource-group" -appName "my-logic-app-nname" -googleSheetId "mock-id"`
   Save the script output URL. It is needed later.
4. Authorize Google connection in Azure. Navigate to your resource group in Azure Portal and find the created google connection and authorize it. This must be done after every time the environent build script has been executed. This should be fixed at some point.
5. Change logic apps web hook uri at https://api.slack.com/apps "Interactive Components"

### Configuring Slack Incoming Webhook
This is the target address for our questionaires. This should be available in https://api.slack.com/apps "Incoming Webhooks"

Create a `appsettings.Development.json` file to `ConsoleTester` directory and change te WebHookUrl to correct url. 

Example
```
{
    "Slack": {
        "WebHookUrl": "https://hooks.slack.com/services/SECRET_WEBHOOK_URL"
    }
}
```

## Creating questionaire
Change the questionaire content in `ConsoleTester.Program` and run console tester.

```
dotnet run --project .\ConsoleTester\
```