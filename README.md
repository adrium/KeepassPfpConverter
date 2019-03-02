# KeePass Pain-free Passwords Converter

KeepassPfpConverter is a KeePass 2.x plugin, command-line tool and library
to work with Pain-free Passwords backups.

Compatible with [Pain-free Passwords](https://pfp.works) and [Adrium Easy Pass](https://github.com/adrium/easypass).

Join the discussion on the [SourceForge announcement thread](https://sourceforge.net/p/keepass/discussion/329220/thread/cc159b85bd/)

## Features

* Import & Export supported
* Imports stored and generated passwords to the root group with a default icon
* Exports all entries as "stored" entries
* Handles password revisions and notes
* Handles custom fields

## Tested

* Mono 5.16 on Ubuntu 18.10
* .NET Framework 2.0 on Windows XP
* .NET Framework 4.0 on Windows 10

### Unsupported

* Legacy versions

## Download and installation

You can get the latest release from the [Releases](https://github.com/adrium/KeepassPfpConverter/releases/latest) page. To install, unpack the archive and copy its contents to the KeePass Plugins directory.

KeePass 2.09 or higher is required for the plugin to work.

## Custom Fields

Custom fields (string fields in *Advanced* tab)
are exported to the note field as `Name: Value`.

They can also be imported in the same format.
Values consisting of multiple lines are not supported.

## Command-line Tool

The tool can be used to decrypt and encrypt backup JSON files
for manual editing or import from other tools.

## Build

Just run the [build script](plgx/build.sh).

Instead of sacrificing syntactic suger, the build relies on [ILSpyConvert](https://github.com/adrium/ILSpyConvert) to generate C# 2 compatible code.
