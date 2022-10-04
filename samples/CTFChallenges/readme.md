# Purpose of this extension

**This extension is meant for demo and training purposes.**

The extension implements different problematic scenarios. Some examples:
- Long-running queries, leading to performance problems
- A bad event subscriber that messes up logic on a page
- Non-printable characters stored in the database, which causes errors

You can run these scenarios and use the standard troubleshooting tools to identify the root cause.

In addition to getting some hands-on experience with these tools, the examples also make it easy to _demonstrate_ to others how the tools work.

# Installing the extension

Build the extension (e.g. against your Business Central Online sandbox environment).

Then upload the extension, either using the Extension Management page, or just publish the extension directly from VS Code.

# Running the scenarios

Use global search (alt-q) and search for the "CTF Challenges" page.

Once opened, the page will explain how to run the scenarios.

# Running the scenarios in "competition mode"

If you are in a class-room setting, you can also run the challenges in "competition mode". In this mode,
you have another website - e.g. ctfd.io - where you manage the challenges. Each challenge will have a name,
a flag that you must find, the number of points you get when solving the challenge by finding the flags, and
optionally a number of hints.

The "CTFD-portal-setup" files are exported from a ctfd.io website. You can import this file into your own ctfd website
in order to get things set up quickly.

Also, if you want to run in this mode:
- Enable "External mode" by default by changing CTFChallengesSetup."External Mode" to true in the CTFChallengesInstall.CodeUnit.al file.
- See the hint codes on the CTF Challenges Setup page. The password to the setup page is "clue".


