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
Console interface of this software uses [CommandLineParser](https://github.com/commandlineparser/commandline).

Using command line parameters differs a little bit depending on where you run this software.

Examples in this section assume that commands are run from `ConsoleTester`-folder.

For up to date help

```
dotnet run -- help 
```

### Creating new questionnaires
Questionnaires are created by reading JSON file.

NOTE: Currently questionnaires are only posted to `#hjni-test`

1. Generate questionnaire template `dotnet run -- generateTemplate -o 'example-questionnaire.json'` 
2. Change question, add/change/remove answers
3. Add questionnaire `dotnet run -- create -f 'example-questionnaire.json'`

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