# Console Application

Console interface of this software uses [CommandLineParser](https://github.com/commandlineparser/commandline).

Using command line parameters differs a little bit depending on where you run this software.

Examples in this section assume that commands are run from `ConsoleInterface`-folder.

For up to date help

```bash
dotnet run -- help 
```

## Configurating Console Application

Configurations are read from `appsettings.json` and `appsettings.Development.json`.

1. Create `appsettings.Development.json` using `appsettings.json` as base.
1. Replace necessary configuration values.

**TableStorage.ConnectionString** can be retrieved from Azure Portal or from
the output of `Prepare-Environment.ps1`. See
[Function App instructions](FunctionApp.md#creating-the-environment) for more.

**SlackClient.BearerToken** can be retrieved from
[Slack Application](SlackApp.md)

### Creating new questionnaires

Questionnaires are created by reading JSON file.

1. Generate questionnaire template `dotnet run -- generateTemplate -o 'example-questionnaire.json'`
1. Change question, add/change/remove answers
1. Add questionnaire `dotnet run -- create -f 'example-questionnaire.json' -c test-channel`

Example questionnaire JSON

```json
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

```bash
dotnet run -- questionnaires
```

Fetching answers for all questionnaires

```bash
dotnet run -- answers
```

Fetching answers for single questionnaire

```bash
dotnet run -- answers -q e746a0db-6984-4fc4-8d6d-b5fad5baaa90
```

Writing answers to CSV-file

```bash
dotnet run -- answers -q e746a0db-6984-4fc4-8d6d-b5fad5baaa90 -o test.csv
```
