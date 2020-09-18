buildDir="./build/"
cmake -B $buildDir
oldPath=$PWD
cd $buildDir
make
cd $oldPath