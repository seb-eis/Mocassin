@echo off

set deployFolder=C:\Users\hims-user\Documents\Gitlab\Mocassin.Builds_1.1.X.X\simulator_win64\bin
set sourceFolder=C:\Users\hims-user\source\repos\ICon.Simulator\cmake-build-release-mingw_x86_64_msys2
echo "Copying data to deploy"
FOR %%G IN ("%sourceFolder%\*.dll" "%sourceFolder%\*.exe") DO (
echo "%%G"
copy %%G %deployFolder%)
echo "Done"