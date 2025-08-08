@echo off
pushd Client
dotnet publish -c Release
popd

pushd Server
dotnet publish -c Release
popd
@REM 
@REM rmdir /s /q dist
@REM mkdir dist
@REM 
@REM copy /y fxmanifest.lua dist
@REM xcopy /y /e Client\bin\Release\net452\publish dist\Client\bin\Release\net452\publish\
@REM xcopy /y /e Server\bin\Release\netstandard2.0\publish dist\Server\bin\Release\netstandard2.0\publish\