#!/bin/sh

if [ ! -f KeepassPfpConverter.sln ]; then
	echo cd /path/to/KeepassPfpConverter.sln
	exit 1
fi

msbuild /p:Configuration=Release
rm bin/Release/KeePass.exe

mv bin/Release KeepassPfpConverter

rm KeepassPfpConverter.zip
zip -Dr KeepassPfpConverter.zip KeepassPfpConverter
rm -r KeepassPfpConverter
