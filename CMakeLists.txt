cmake_minimum_required(VERSION 3.0.0)
project(Mocassin.Simulator VERSION 0.1.0)

include(CTest)
enable_testing()

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c11")

FILE(GLOB_RECURSE FRAMEWORK_SOURCE ./Framework/*.c)
FILE(GLOB_RECURSE SIMULATOR_SOURCE ./Simulator/*.c)
FILE(GLOB_RECURSE FRAMEWORK_HEADER ./Framework/*.h)
FILE(GLOB_RECURSE SIMULATOR_HEADER ./Simulator/*.c)


add_executable(Mocassin.Simulator ./Main/main.c)
set_target_properties(Mocassin.Simulator PROPERTIES C_STANDARD 11)

add_library(simulator SHARED ${SIMULATOR_SOURCE})
set_target_properties(simulator PROPERTIES C_STANDARD 11)

add_library(sqlite3 SHARED ./ExternalLibraries/sqlite3.h ./ExternalLibraries/sqlite3.c)
set_target_properties(sqlite3 PROPERTIES PUBLIC_HEADER ./ExternalLibraries/sqlite3.h C_STANDARD 11)

add_library(jobloader SHARED ./InternalLibraries/SqliteJobLoader/Sqlite3JobLoader.h ./InternalLibraries/SqliteJobLoader/Sqlite3JobLoader.c ${FRAMEWORK_HEADER})
set_target_properties(jobloader PROPERTIES PUBLIC_HEADER ./InternalLibraries/Interfaces/JobLoader.h C_STANDARD 11)

add_library(progressprint SHARED InternalLibraries/Interfaces/ProgressPrint.h ./InternalLibraries/ProgressPrint/ProgressPrint.c ${FRAMEWORK_HEADER} ${SIMULATOR_HEADER})
set_target_properties(progressprint PROPERTIES PUBLIC_HEADER ./InternalLibraries/Interfaces/ProgressPrint.h C_STANDARD 11)

add_library(framework SHARED ${FRAMEWORK_SOURCE})
set_target_properties(framework PROPERTIES C_STANDARD 11)

include_directories(${PROJECT_SOURCE_DIR})

set(CMAKE_C_FLAGS_DEBUG "-Wall -g")
set(CMAKE_C_FLAGS_RELEASE "-Wall -O3")

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)

#target_compile_options(Mocassin.Simulator PUBLIC -pg)
#target_link_options(Mocassin.Simulator PUBLIC -pg)
target_link_libraries(jobloader sqlite3 framework)
target_link_libraries(progressprint framework)
target_link_libraries(simulator framework progressprint)
target_link_libraries(Mocassin.Simulator jobloader progressprint framework simulator)
