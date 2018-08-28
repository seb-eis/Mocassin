cmake_minimum_required(VERSION 3.0.0)
project(ICon.Simulator VERSION 0.1.0)

include(CTest)
enable_testing()

FILE(GLOB_RECURSE FRAMEWORK_SOURCE ./Framework/*.c)
FILE(GLOB_RECURSE SIMULATOR_SOURCE ./Simulator/*.c)

set(C_STANDARD 11)
set(CMAKE_C_FLAGS_DEBUG "-Wall -g")
set(CMAKE_C_FLAGS_RELEASE "-O2")

include_directories(./)
add_executable(ICon.Simulator ./Main/main.c ${FRAMEWORK_SOURCE} ${SIMULATOR_SOURCE})

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
