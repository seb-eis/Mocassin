buildDir="./build/"
cmake -B $buildDir -D"CMAKE_BUILD_TYPE=Release"
oldPath=$PWD
cd $buildDir
make
cd $oldPath