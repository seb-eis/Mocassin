#!/usr/bin/env zsh

build_scripts=$(ls | grep bsubbuild)
for value in $build_scripts; do
   echo "Executing build script: $value"
   eval $value
done