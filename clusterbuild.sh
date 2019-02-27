#!/usr/bin/env zsh

make_flags="-DCMAKE_BUILD_TYPE=Release"

base_target="$HOME/Mocassin/Simulator"
gcc_target="$base_target/gcc_release"
icc_target="$base_target/icc_release"
build_source="$base_target/"

rm -rf $gcc_target
rm -rf $icc_target

mkdir $gcc_target
mkdir $icc_target

echo "Building gcc release ..\n"
cd $gcc_target
CC=gcc cmake $make_flags $base_target
make
echo "Building gcc release finished"

echo "Building icc release ..\n"
cd $icc_target
CC=icc cmake $make_flags $base_target
make
echo "Building icc release finished"

cd $HOME