#!/usr/bin/env bash

BASEDIR=$(dirname "$0")
src=$BASEDIR/src
test=$BASEDIR/test

dotnet format $src/AspNetClassic
dotnet format $test/AspNetClassic.Tests
dotnet format $src/AspNetClassic.Authorization
dotnet format $test/AspNetClassic.Authorization.Tests