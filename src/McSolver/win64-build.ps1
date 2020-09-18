$buildDir="./build/"
$makePath="C:\msys64\mingw64\bin\mingw32-make.exe"
cmake -B $buildDir -G "MinGW Makefiles" -D"CMAKE_MAKE_PROGRAM:PATH=$makePath"
$oldPath=$PWD
cd $buildDir
Invoke-Expression "$makePath"
cd $oldPath