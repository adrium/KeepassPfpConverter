#!/bin/sh

if [ ! -f KeepassPfpConverter.sln ]; then
	echo cd /path/to/KeepassPfpConverter.sln
	exit 1
fi

msbuild /p:Configuration=Release

cp -a bin/Release PfpConverter
rm PfpConverter/KeePass.exe

rm PfpConverter.zip
zip -Dr PfpConverter.zip PfpConverter
rm -r PfpConverter
