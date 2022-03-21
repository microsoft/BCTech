These extensions are meant for demo and training purposes.

The extensions implement different cases of bad-performing AL Code. Some examples:
 - Long-running queries
 - Running substantial code in OnAfterGetRecord
 - Listening for too many database triggers

With these extensions, you can execute scenarios that trigger these bad behaviors. What you can do next is try to troubleshoot them using the available tools:
 - Profiler (both in VSCode and the in-client version)
 - In-client pages with performance information
 - Telemetry

In addition to getting some hands-on experience with these tools, the examples also make it easy to _demonstrate_ to others how the tools work.