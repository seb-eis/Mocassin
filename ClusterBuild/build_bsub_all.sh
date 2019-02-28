#!/usr/bin/env zsh

build_cpu_models="Westmere_EP Beckton"
for value in $build_cpu_models; do
   echo "Executing build script for cpu model: $value"
   bsubbuild_cpu_target.sh $value
done