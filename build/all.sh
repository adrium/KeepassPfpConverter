#!/bin/sh

dotnet build -c Release
dotnet build -c Program
dotnet test

mkdir -p output/plugin
cp -v plugin/*plgx* output/plugin
cp -v tool/bin/Release/*/*.dll output/plugin
cat plugin/*.cs | grep ^using | sort -u > output/plugin/Plugin.cs
cat plugin/*.cs | grep ^.assembly: >> output/plugin/Plugin.cs
cat plugin/*.cs | grep -Ev '^(using|.assembly:)' >> output/plugin/Plugin.cs
keepass2 -plgx-create $PWD/output/plugin

mkdir -p output/tool
cp -rv tool/bin/Program/* output/tool/PfpTool
