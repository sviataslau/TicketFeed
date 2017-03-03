A simple console application to pull a list of tickets you've been working on from your bug tracking system and push them to selected output.

Usage:

```TicketFeed.exe --src jira --url https://comrex.atlassian.net --user sseviaryn --password flamingmadness1987 --range Today --out Clipboard```

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
* Jira 

Miss a source or a target? You can write your own. Just inherit Source or Output and implement your own providers as you want.

Possibilities are endless. For example, you can automate your reporting routine: just write a custom output which will fill the data in your reporting system.

