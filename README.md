# Slack AskBot
A bot to perform small questionaries in Slack

This is still a very much a work in progress so this shouldn't probably be used by anyone.

## Roadmap
 1. Loading questionnaire from file / console / etc
 1. Deployment from CI
 1. Answer reporting to proper place
 1. Table storage cleanup

## (Bad) Setup steps

### Creating application
1. Create the environment
   For example 
   ```
   New-AzResourceGroupDeployment -Name "my-deploy-name" -TemplateFile Deployment/azuredeploy.json -ResourceGroupName "my-resource-group" -logicAppName "my-logic-app-nname"
   ```
   Save the script output URL. It is needed later.
2. Change logic apps web hook uri at https://api.slack.com/apps "Interactive Components"

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

## Usage
For help

```
dotnet run --project .\ConsoleTester\ -- help 
```

Questionnaire is hard coded for now.
