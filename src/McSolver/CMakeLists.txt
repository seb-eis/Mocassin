cmake_minimum_required(VERSION 3.0.0)
project(Mocassin.Simulator VERSION 0.1.0)

include(CTest)
enable_testing()

find_package(Threads REQUIRED)

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c11")

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)

include_directories(${PROJECT_SOURCE_DIR})

# SSE Variables
set(COMPILE_FLAGS_SSE_INTEL "-axSSE4.2 -xSSE2")
set(COMPILE_FLAGS_SSE_GNU "-msse2 -msse3 -msse4.2 -mavx")

# Resursive search for source files
file(GLOB_RECURSE FRAMEWORK_HEADER Libraries/Framework/*.h)
file(GLOB_RECURSE FRAMEWORK_SOURCE Libraries/Framework/*.c)
file(GLOB_RECURSE SIMULATOR_HEADER Libraries/Simulator/*.h)
file(GLOB_RECURSE SIMULATOR_SOURCE Libraries/Simulator/*.c)
file(GLOB_RECURSE UTILITY_HEADER Libraries/Utility/*.h)
file(GLOB_RECURSE UTILITY_SOURCE Libraries/Utility/*.c)
file(GLOB_RECURSE EXE_RESOURCES ./Main/Resources/*.rc)

set(VAR_LIBRARY_TYPE "SHARED")
set(VAR_COMPILE_SQLITE "TRUE")

# Library and executable definitions
add_executable(Mocassin.Simulator Main/simulator_main.c ${EXE_RESOURCES})
set_target_properties(Mocassin.Simulator PROPERTIES C_STANDARD 11)

add_library(simulator ${VAR_LIBRARY_TYPE} ${SIMULATOR_SOURCE})
set_target_properties(simulator PROPERTIES C_STANDARD 11)

if ("${VAR_COMPILE_SQLITE}" STREQUAL "TRUE")
    add_library(sqlite3 ${VAR_LIBRARY_TYPE} Libraries/Sqlite/sqlite3.h Libraries/Sqlite/sqlite3.c)
    set_target_properties(sqlite3 PROPERTIES PUBLIC_HEADER Libraries/Sqlite/sqlite3.h C_STANDARD 11)
endif()

add_library(jobloader ${VAR_LIBRARY_TYPE} Libraries/JobLoader/Sqlite3JobLoader.h Libraries/JobLoader/Sqlite3JobLoader.c)
set_target_properties(jobloader PROPERTIES PUBLIC_HEADER Libraries/JobLoader/JobLoader.h C_STANDARD 11)

add_library(progressprint ${VAR_LIBRARY_TYPE} Libraries/ProgressPrint/ProgressPrint.h Libraries/ProgressPrint/ProgressPrint.c
        Libraries/Simulator/Logic/Routines/StatisticsRoutines.c Libraries/Simulator/Logic/Routines/StatisticsRoutines.h)
set_target_properties(progressprint PROPERTIES PUBLIC_HEADER Libraries/ProgressPrint/ProgressPrint.h C_STANDARD 11)

add_library(framework ${VAR_LIBRARY_TYPE} ${FRAMEWORK_SOURCE})
set_target_properties(framework PROPERTIES C_STANDARD 11)

add_library(progressprint.minimal ${VAR_LIBRARY_TYPE} Libraries/ProgressPrint/ProgressPrint.h Libraries/ProgressPrint/MinimalProgressPrint.c
        Libraries/Simulator/Logic/Routines/StatisticsRoutines.c Libraries/Simulator/Logic/Routines/StatisticsRoutines.h)
set_target_properties(progressprint.minimal PROPERTIES PUBLIC_HEADER Libraries/ProgressPrint/ProgressPrint.h C_STANDARD 11)

# Utility executables and library
add_executable(Mocassin.Utility Main/utility_main.c ${EXE_RESOURCES})
set_target_properties(Mocassin.Utility PROPERTIES C_STANDARD 11)

add_library(utility SHARED ${UTILITY_SOURCE})
set_target_properties(utility PROPERTIES C_STANDARD 11)

# Extensions
add_library(mmcfe.mocext SHARED Extensions/MmcfeExtension.h Extensions/MmcfeExtension.c)
set_target_properties(mmcfe.mocext PROPERTIES C_STANDARD 11 PUBLIC_HEADER Extensions/MocassinSolverExtension.h)

# Compiler settings
if (CMAKE_BUILD_TYPE STREQUAL "Debug")
    add_compile_definitions(MC_TESTBUILD)
endif()

if (WIN32)
    # Compiler settings for windows
    message(STATUS "** Compiling with WINDOWS settings **")
    # Basic compiler settings, not machine specific
    #set(CMAKE_C_FLAGS_DEBUG "-Wall -g")
    #set(CMAKE_C_FLAGS_RELEASE "-Wall -O3")

    # Set the flags to allow unicode and wmain
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -municode")
    add_compile_definitions(UNICODE _UNICODE)
    if (CMAKE_BUILD_TYPE STREQUAL "Release")
        message(STATUS "** Win Relase build, setting removal of debug information**")
        set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -s")
        set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -s")
    endif()

    # Advanced compiler settings, small performance improvement
    set(CMAKE_C_FLAGS_RELEASE "${COMPILE_FLAGS_SSE_GNU} -O3 -Wall -fno-cx-limited-range -funroll-loops -m64 -s")
    set(CMAKE_C_FLAGS_DEBUG "${COMPILE_FLAGS_SSE_GNU} -g -Wall -m64")

elseif (UNIX)
    # Set the linker flags to use origin linking
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,-rpath -Wl,$ORIGIN")

    # Advanced gnu compiler settings
    if ("${CMAKE_C_COMPILER_ID}" STREQUAL "GNU")
        message(STATUS "** Compiling with GNU settings **")
        set(CMAKE_C_FLAGS_RELEASE "-O3 ${COMPILE_FLAGS_SSE_GNU} -w -fno-cx-limited-range -Wl,--no-as-needed -lm -ldl -pthread -s")
        set(CMAKE_C_FLAGS_DEBUG "-g ${COMPILE_FLAGS_SSE_GNU} -fno-cx-limited-range -Wl,--no-as-needed -lm -ldl -pthread")

    # Advanced intel compiler settings
    elseif("${CMAKE_C_COMPILER_ID}" STREQUAL "Intel")
        message(STATUS "** Compiling with Intel settings **")
        set(CMAKE_C_FLAGS_RELEASE "-O3 ${COMPILE_FLAGS_SSE_INTEL} -w -fno-cx-limited-range -ldl -pthread -s")
        set(CMAKE_C_FLAGS_DEBUG "-g ${COMPILE_FLAGS_SSE_INTEL} -fno-cx-limited-range -ldl -pthread")

    # Fallback settings
    else()
        set(CMAKE_C_FLAGS_DEBUG "-Wall -g")
        set(CMAKE_C_FLAGS_RELEASE "-Wall -O3")
    endif()
endif ()

# Link main libraries and simulator
target_link_libraries(sqlite3 ${CMAKE_DL_LIBS} Threads::Threads)
target_link_libraries(jobloader sqlite3 framework ${CMAKE_DL_LIBS})
target_link_libraries(progressprint framework ${CMAKE_DL_LIBS})
target_link_libraries(simulator framework progressprint m ${CMAKE_DL_LIBS})
target_link_libraries(progressprint.minimal framework simulator ${CMAKE_DL_LIBS})
target_link_libraries(Mocassin.Simulator jobloader progressprint framework simulator ${CMAKE_DL_LIBS})

# Utility executable links
target_link_libraries(utility framework simulator sqlite3 ${CMAKE_DL_LIBS})
target_link_libraries(Mocassin.Utility utility ${CMAKE_DL_LIBS})

# Extension links
target_link_libraries(mmcfe.mocext sqlite3 framework simulator ${CMAKE_DL_LIBS})

# Profiling flags
#target_compile_options(Mocassin.Simulator PUBLIC -pg -no-pie)
#target_link_options(Mocassin.Simulator PUBLIC -pg -no-pie)
