@echo off
set simulator=./bin/Mocassin.Simulator.exe

set /p dbPath=Enter database path: 
echo Value is - %dbPath%

set /p ioPath=Enter IO path: 
echo Value is - %ioPath%

set /p jobId=Enter job context id: 
echo Value is %jobId%

set /p cmdArgs=Enter additional command argument pairs:
echo Value is: %cmdArgs%

START %simulator% -dbPath %dbPath% -dbQuery %jobId% -ioPath %ioPath% %cmdArgs%