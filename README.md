A simple console application to pull a list of tickets you've been working on from your JIRA.

Usage:

```TicketFeed.exe --jira https://comrex.atlassian.net --user you --password password --range Today[Yesterday|Week|Month] --out File[Console|Clipboard]```

Supported outputs:
* File
* Console
* Clipboard

However, you can write your own. For example, you can write a custom output which will fill the data in your reporting system :)
