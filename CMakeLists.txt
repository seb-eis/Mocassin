cmake_minimum_required(VERSION 3.0.0)
project(ICon.Simulator VERSION 0.1.0)

include(CTest)
enable_testing()

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c11")

FILE(GLOB_RECURSE FRAMEWORK_SOURCE ./Framework/*.c)
FILE(GLOB_RECURSE SIMULATOR_SOURCE ./Simulator/*.c)

add_executable(ICon.Simulator ./Main/main.c ${FRAMEWORK_SOURCE} ${SIMULATOR_SOURCE} Framework/Basic/BaseTypes/Buffers.h Framework/Basic/BaseTypes/Buffers.c Framework/Basic/Macros/Macros.h Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInit.h Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInit.c Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.c Simulator/Logic/Routines/Tracking/TransitionTracking.h Simulator/Logic/Routines/Tracking/TransitionTracking.c Framework/Basic/Macros/BinarySearch.h Simulator/Logic/Routines/Debug/DebugRoutines.h Simulator/Logic/Routines/Debug/DebugRoutines.c Framework/Basic/Time/McTime.h Simulator/Logic/Routines/Output/DefaultOutput.h Simulator/Logic/Routines/Output/DefaultOutput.c)

include_directories(${PROJECT_SOURCE_DIR})

set_property(TARGET ICon.Simulator PROPERTY C_STANDARD 11)
set(CMAKE_C_FLAGS_DEBUG "-Wall -g")
set(CMAKE_C_FLAGS_RELEASE "-Wall -O3")

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
