#!/bin/sh

rm -r build

mkdir build
cp src/*.plgx.csproj build
find src -name \*.cs -exec cp {} build \;
rm -f build/Program.cs

cp packages/Newtonsoft.Json.*/lib/net20/*.dll build
cp packages/BouncyCastle.*/lib/*.dll build


mono packages/KeePass.exe -plgx-create build
mv build.plgx PfpConverter.plgx
