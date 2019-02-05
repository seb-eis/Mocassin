cmake_minimum_required(VERSION 3.0.0)
project(ICon.Simulator VERSION 0.1.0)

include(CTest)
enable_testing()

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c11")

FILE(GLOB_RECURSE FRAMEWORK_SOURCE ./Framework/*.c)
FILE(GLOB_RECURSE SIMULATOR_SOURCE ./Simulator/*.c)


add_executable(ICon.Simulator
        ./Main/main.c
        ${FRAMEWORK_SOURCE}
        ${SIMULATOR_SOURCE}
        Framework/Basic/BaseTypes/Buffers.h
        Framework/Basic/BaseTypes/Buffers.c
        Framework/Basic/Macros/Macros.h
        Framework/Sqlite/SqliteReader.h
        Framework/Sqlite/SqliteReader.c
        UnitTesting/MinimalUnitTest.h
        UnitTesting/UnitTests.h
        Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInit.h
        Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInit.c
        Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.h
        Simulator/Logic/Initializers/CmdArgResolver/CmdArgumentResolver.c
        Simulator/Logic/Routines/Tracking/TransitionTracking.h
        Simulator/Logic/Routines/Tracking/TransitionTracking.c)

include_directories(${PROJECT_SOURCE_DIR})

set_property(TARGET ICon.Simulator PROPERTY C_STANDARD 11)
set(CMAKE_C_FLAGS_DEBUG "-Wall -g -O2")
set(CMAKE_C_FLAGS_RELEASE "-Wall -O2")

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)

find_path(sqlPath sqlite3.h)
find_library(sqlLib sqlite3)
include_directories(${sqlPath})
target_link_libraries(ICon.Simulator "${sqlLib}")