# Creating Slack App

This integration requires a Slack App with correct permissions work. App can be
created with `manifest.json` file in `Deployment` folder.

After the Slack App is created, following steps need to be taken:

1. OAuth Bearer token needs to be retrieved and set for the function app. OAuth
token can be found in `OAuth & Permissions` section at Slack API settings.
1. Slack app needs to be installed to the Workspace (this is the Slack
"server")
1. After Azure Functions App is created, the function app url must be set to
Slack App.

## Setting Function App URI

The function app must be created before this step!

The function app URI must be set as Interactivity Request Url. This is setting
is found under `Interactivty & Shortcuts` section at Slack API settings.

If you don't already have the URI is described in
[Azure Function App guide](FunctionApp.md#retrieving-function-uri)

## Permissions

This section describes what permissions and why the permissions are required by this
application.

* Bot scope
  * chat:write - This is required for sending messages.
  * chat:write.public - This is required for sending messages to channels
  without inviting the bot the channel. Technically this can be left out,
  but then the bot needs to be invited to the channel and the bot is designed
  to work this way.
  * commands - This is required so the application can show shortcuts next to
  the message box.
