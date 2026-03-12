# KeePass Pain-free Passwords Converter

KeepassPfpConverter is a KeePass 2.x plugin, command-line tool and library
to work with Pain-free Passwords version 2 backups.

Compatible with [Pain-free Passwords](https://pfp.works) version 2
and [Adrium Easy Pass](https://github.com/adrium/easypass).

Join the discussion on the [SourceForge announcement thread](https://sourceforge.net/p/keepass/discussion/329220/thread/cc159b85bd/)

## Features

* Import & Export supported
* Imports stored and generated passwords to the root group with a default icon
* Exports all entries as "stored" entries
* Custom Fields & Tags supported
* Revisions & Aliases supported
* Handles duplicated users for sites

## Support

* Mono on Ubuntu
* .NET Framework 4.0 on Windows

## Download and installation

You can get the latest release from the [Releases](https://github.com/adrium/KeepassPfpConverter/releases/latest) page.
Copy `PfpConverter.plgx` to the [KeePass Plugins](https://keepass.info/help/v2/plugins.html) directory to install.

KeePass 2.18 or higher is required for the plugin to work.

## Custom Fields

Custom fields (string fields in *Advanced* tab) and Tags
are exported to the note field as `Name: Value`.

They can also be imported in the same format.
Values consisting of multiple lines are not supported.

## Command-line Tool

The tool can be used to decrypt and encrypt backup JSON files
for manual editing or import from other tools.

It can also be used to generate passwords.
