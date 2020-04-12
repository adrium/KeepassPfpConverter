#!/bin/sh

if [ ! -f KeepassPfpConverter.sln ]; then
	echo cd /path/to/KeepassPfpConverter.sln
	exit 1
fi

msbuild /p:Configuration=Release
rm bin/Release/KeePass.exe

mv bin/Release PfpConverter

rm PfpConverter.zip
zip -Dr PfpConverter.zip PfpConverter
rm -r PfpConverter
