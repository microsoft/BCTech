# BC TechDays 2024 - MoveTable Demo

Welcome to the MoveTable demo for BC TechDays 2024! This demo showcases the concept of moving tables between apps in Microsoft Business Central.
These are the demo apps used during the BC TechDays 2024 session 'Microsoft Presents: Obsolete Move, Future of delocalization and componentization'. You can find a video of the session here: https://youtu.be/Du1Rtjh6wYc

## Overview

In this demo, we have built an app called Human Worker Efficiency Tracker. The app tracks employee coffee consumption throughout the day and measures their task efficiency. The goal is to correlate caffeine intake with efficiency and provide more coffee when needed. Please note that this is just a demo app and does not have any real functionality.

As the app grew in size and complexity, we decided to refactor it into smaller apps, each with a single responsibility. During the refactoring process, we utilize the new MoveFrom/MoveTo properties to move tables between the apps without duplicating the data. The purpose of this demo is to showcase the capability of moving tables.

## Table Moves

In this demo, we demonstrate three types of table moves:

1. Move 'down': Tables can be moved from an app into a dependency.
2. Move 'sideways': Tables can be moved to an unrelated app without any existing dependency.
3. Move 'up': Tables can be moved into an app that depends on the original app.

For more information on table moves, you can refer to the [Microsoft documentation](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-move-table-fields-between-extensions).

## Running the Demo

Each version of the apps has its own folder and workspace. To run the demo, you need to set up Business Central and Visual Studio Code to compile and publish each version.

Here is a brief description of each version:

- v1.0: The original app with all functionalities in one.
- v1.1: The app is still big, but functionalities are split into folders and extension objects. ObsoleteState = PendingMove is added to tables.
- v2.0: The app is split into 5 smaller apps, and tables are moved 'down'.
- v2.1: ObsoleteState = PendingMove is added to a table.
- v3.0: A table is moved 'sideways'.
- v3.1: ObsoleteState = PendingMove is added to a table.
- v4.0: A table is moved 'up'.

Note: This demo is intended to be run from Visual Studio Code as a developer scenario. After publishing the apps in v1.1, you need to uninstall (not unpublish) the Human Worker Efficiency Tracker app from Extension Management in order to publish the apps in v2.0.

Enjoy the demo and explore the power of table moves in Microsoft Business Central!