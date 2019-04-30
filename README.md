A simple console application to pull a list of tickets you've been working on from your bug tracking system and push them to selected output.

Usage:

```TicketFeed.exe --src jir --range Today --out Clipboard```

Supported time ranges:
* Today
* Yesterday
* Week 
* Month

Supported (but extendable) outputs:
* File
* Console
* Clipboard

Supported (but extendable) sources:
* Jira (configurable in Jira.json) - your Jira activity feed
* Excuse (configurable in Excuse.txt) - your daily lame excuses

Miss a source or a target? You can write your own. Just inherit Source or Output and implement your own providers as you want.

