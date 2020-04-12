#!/bin/sh

if [ "x$ILSPYCONVERT" = x ]; then
	echo export ILSPYCONVERT=/path/to/ILSpyConvert.exe
	exit 1
fi

if [ ! -f KeepassPfpConverter.sln ]; then
	echo cd /path/to/KeepassPfpConverter.sln
	exit 1
fi

msbuild /p:Configuration=Plugin
mono "$ILSPYCONVERT" 2 bin/Plugin/KeepassPfpConverter.dll > bin/Plugin/PfpConverter.cs
rm bin/Plugin/KeepassPfpConverter.dll
rm bin/Plugin/KeePass.exe

mono packages/KeePass.exe -plgx-create "$PWD/bin/Plugin"
mv bin/Plugin.plgx PfpConverter.plgx
