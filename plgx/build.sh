#!/bin/sh

if [ "x$ILSPYCONVERT" = x ]; then
	echo export ILSPYCONVERT=/path/to/ILSpyConvert.exe
	exit 1
fi

if [ ! -f KeepassPfpConverter.sln ]; then
	echo cd /path/to/KeepassPfpConverter.sln
	exit 1
fi

rm -r build

mkdir build
cp plgx/*.csproj build

cp packages/Newtonsoft.Json.*/lib/net20/*.dll build
cp packages/BouncyCastle.*/lib/*.dll build

msbuild /p:Configuration=Release
mono "$ILSPYCONVERT" 2 bin/Release/KeepassPfpConverter.exe > build/PfpConverter.cs
cat plgx/correct-decompiler.patch | patch -p 1
mono packages/KeePass.exe -plgx-create build
mv build.plgx PfpConverter.plgx
