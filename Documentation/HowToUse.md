# How to use

This document describes how Slack Ask Bot is used from Slack. For Console App
usage, see [Usage](ConsoleApp.md#usage) in console application help.

Note: This guide assumes that the Slack App is installed to the workspace with
name `Pinja Slack Ask Bot`.

## Creating questionnaires

Questionnaires can be created with Shortcuts-button in slack. This button is on
the left side of the message field.

![Shortcut button](/images/message_box.png?raw.jpg "Shortcut button")

This button shows list shortcuts provided by different applications and each
workspace may have different application. Select `Pinja Slack Ask Bot` from the
list. This shows all options available for the Slack Ask Bot.

![List of options](/images/shortcut_options?raw.jpg "List of options")

Choosing `Create questionnaire` will open a dialog which allows user to define
the question, available options and to which channel the questionnaire is sent
to. Options can be added or removed with the `Add another option` or
`delete option` -buttons. Title and the options supports emojis available in
workspace, but those are not visible when written in the dialog.

After pressing `Submit` questionnaire is sent to channel and can be answerred
there.

## Answering questionnaire

When questionnaire is sent to channel, it can be answerred with `Answer` button
in the message. This shows the dialog where user can choose the option.

![Answer button](/images/answer_button?raw.jpg "Answer button")

If user answers twice, the second answer replaces the old answer given by user.
The old answer is shown in answer dialog.

## Checking answers

To show answers, select `Get answers` from the shortcut menu. This will show a
dialog with all questionnaires and user can choose any questionnaire. After
questionnaire is selected and `Submit` is pressed, dialog shows results.

Results dialog doesn't show which user has answerred what option.

## Deleting questionnaires

To delete questionnaires, select `Delete a questionnaire` from the shortcut
menu. This will show a dialog all questionnaires and user can choose any
questionnaire. After questionnaire is selected and `Submit` is pressed, the
questionnaire is deleted!

Shortuct also provide `Deletete questionnaires` option which will remove all
questionnaires!