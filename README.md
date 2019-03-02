# KeePass Pain-free Passwords Converter

KeepassPfpConverter is a KeePass 2.x plugin, command-line tool and library
to work with Pain-free Passwords backups.

Compatible with [Pain-free Passwords](https://pfp.works) and [Adrium Easy Pass](https://github.com/adrium/easypass).

## Features

* Import & Export supported
* Imports stored and generated passwords to the root group with a default icon
* Exports all entries as "stored" entries
* Handles password revisions and notes

## Tested

* Mono 5.16 on Ubuntu 18.10
* .NET Framework 2.0 on Windows XP
* .NET Framework 4.0 on Windows 10

### Unsupported

* Legacy versions

## Download and installation

You can get the latest release from the [Releases](https://github.com/adrium/KeepassPfpConverter/releases/latest) page. To install, unpack the archive and copy its contents to the KeePass Plugins directory.

KeePass 2.09 or higher is required for the plugin to work.

## Command-line Tool

The tool can be used to decrypt and encrypt backup JSON files
for manual editing or import from other tools.

## Build

Just run the [build script](plgx/build.sh).

Instead of sacrificing syntactic suger, the build relies on [ILSpyConvert](https://github.com/adrium/ILSpyConvert) to generate C# 2 compatible code.
