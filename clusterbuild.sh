#!/usr/bin/env zsh

cpu_name=$1
make_flags="-DCMAKE_BUILD_TYPE=Release"

base_target="$HOME/Mocassin/Simulator"
gcc_target="$base_target/gcc_release_$cpu_name"
icc_target="$base_target/icc_release_$cpu_name"
build_source="$base_target/"

rm -rf $gcc_target
rm -rf $icc_target

mkdir $gcc_target
mkdir $icc_target

echo "Building $cpu_name gcc release ..\n"
cd $gcc_target
CC=gcc cmake $make_flags $base_target
make
echo "Building gcc release finished"

echo "Building icc release ..\n"
cd $icc_target
CC=icc cmake $make_flags $base_target
make
echo "Building $cpu_name icc release finished"

cd $HOME