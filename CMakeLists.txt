cmake_minimum_required(VERSION 3.0.0)
project(Mocassin.Simulator VERSION 0.1.0)

include(CTest)
enable_testing()

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c11")

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)

include_directories(${PROJECT_SOURCE_DIR})

# SSE Variables
set(COMPILE_FLAGS_SSE_INTEL "-axSSE4.2 -xSSE2")
set(COMPILE_FLAGS_SSE_GNU "-msse2 -msse3 -msse4.2")

# Resursive search for source files
file(GLOB_RECURSE FRAMEWORK_SOURCE ./Framework/*.c)
file(GLOB_RECURSE SIMULATOR_SOURCE ./Simulator/*.c)
file(GLOB_RECURSE FRAMEWORK_HEADER ./Framework/*.h)
file(GLOB_RECURSE SIMULATOR_HEADER ./Simulator/*.c)
file(GLOB_RECURSE UTILITY_HEADER ./Utility/*.h)
file(GLOB_RECURSE UTILITY_SOURCE ./Utility/*.c)

set(VAR_LIBRARY_TYPE "SHARED")
set(VAR_COMPILE_SQLITE "TRUE")

# Library and executable definitions
add_executable(Mocassin.Simulator ./Main/main.c)
set_target_properties(Mocassin.Simulator PROPERTIES C_STANDARD 11)

add_library(simulator ${VAR_LIBRARY_TYPE} ${SIMULATOR_SOURCE})
set_target_properties(simulator PROPERTIES C_STANDARD 11)

if ("${VAR_COMPILE_SQLITE}" STREQUAL "TRUE")
    add_library(sqlite3 ${VAR_LIBRARY_TYPE} ./ExternalLibraries/sqlite3.h ./ExternalLibraries/sqlite3.c)
    set_target_properties(sqlite3 PROPERTIES PUBLIC_HEADER ./ExternalLibraries/sqlite3.h C_STANDARD 11)
endif()

add_library(jobloader ${VAR_LIBRARY_TYPE} ./InternalLibraries/SqliteJobLoader/Sqlite3JobLoader.h ./InternalLibraries/SqliteJobLoader/Sqlite3JobLoader.c)
set_target_properties(jobloader PROPERTIES PUBLIC_HEADER ./InternalLibraries/Interfaces/JobLoader.h C_STANDARD 11)

add_library(progressprint ${VAR_LIBRARY_TYPE} InternalLibraries/Interfaces/ProgressPrint.h InternalLibraries/ProgressPrint/ProgressPrint.c
            Simulator/Logic/Routines/Statistics/McStatistics.c Simulator/Logic/Routines/Statistics/McStatistics.h)
set_target_properties(progressprint PROPERTIES PUBLIC_HEADER InternalLibraries/Interfaces/ProgressPrint.h C_STANDARD 11)

add_library(framework ${VAR_LIBRARY_TYPE} ${FRAMEWORK_SOURCE})
set_target_properties(framework PROPERTIES C_STANDARD 11)

add_library(progressprint.minimal ${VAR_LIBRARY_TYPE} InternalLibraries/Interfaces/ProgressPrint.h InternalLibraries/ProgressPrint/MinimalProgressPrint.c
            Simulator/Logic/Routines/Statistics/McStatistics.c Simulator/Logic/Routines/Statistics/McStatistics.h)
set_target_properties(progressprint.minimal PROPERTIES PUBLIC_HEADER InternalLibraries/Interfaces/ProgressPrint.h C_STANDARD 11)

# Utility executables and library
add_executable(Mocassin.Utility Main/UtilityMain.c)
set_target_properties(Mocassin.Utility PROPERTIES C_STANDARD 11)

add_library(utility SHARED ${UTILITY_SOURCE})
set_target_properties(utility PROPERTIES C_STANDARD 11)

# Extensions
add_library(mmcfe.mocext SHARED Extensions/Mmcfe/MmcFERoutine.h Extensions/Mmcfe/MmcFERoutine.c)
set_target_properties(mmcfe.mocext PROPERTIES C_STANDARD 11 PUBLIC_HEADER Extensions/Interface/IMocsimExtension.h)

# Compiler settings
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
    set(CMAKE_C_FLAGS_RELEASE "${COMPILE_FLAGS_SSE_GNU} -O3 -w -fno-cx-limited-range -funroll-loops -march=native -mtune=native -m64 -s")
    set(CMAKE_C_FLAGS_DEBUG "${COMPILE_FLAGS_SSE_GNU} -g -m64")

elseif (UNIX)
    # Set the linker flags to use origin linking
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,-rpath -Wl,$ORIGIN")

    # Advanced gnu compiler settings
    if ("${CMAKE_C_COMPILER_ID}" STREQUAL "GNU")
        message(STATUS "** Compiling with GNU settings **")
        set(CMAKE_C_FLAGS_RELEASE "-O3 ${COMPILE_FLAGS_SSE_GNU} -w -fno-cx-limited-range -funroll-loops -march=native -mtune=native -lm -ldl -pthread -s")
        set(CMAKE_C_FLAGS_DEBUG "-g ${COMPILE_FLAGS_SSE_GNU} -fno-cx-limited-range -funroll-loops -march=native -mtune=native -lm -ldl -pthread")

    # Advanced intel compiler settings
    elseif("${CMAKE_C_COMPILER_ID}" STREQUAL "Intel")
        message(STATUS "** Compiling with Intel settings **")
        set(CMAKE_C_FLAGS_RELEASE "-O3 ${COMPILE_FLAGS_SSE_INTEL} -w -fno-cx-limited-range -funroll-loops -ldl -pthread -s")
        set(CMAKE_C_FLAGS_DEBUG "-g ${COMPILE_FLAGS_SSE_INTEL} -fno-cx-limited-range -funroll-loops -ldl -pthread")

    # Fallback settings
    else()
        set(CMAKE_C_FLAGS_DEBUG "-Wall -g")
        set(CMAKE_C_FLAGS_RELEASE "-Wall -O3")
    endif()
endif ()

# Link main libraries and simulator

target_link_libraries(jobloader sqlite3 framework)
target_link_libraries(progressprint framework)
target_link_libraries(simulator framework progressprint)
target_link_libraries(progressprint.minimal framework simulator)
target_link_libraries(Mocassin.Simulator jobloader progressprint framework simulator)

# Utility executable links
target_link_libraries(utility framework simulator sqlite3)
target_link_libraries(Mocassin.Utility utility)

# Extension links
target_link_libraries(mmcfe.mocext sqlite3 framework simulator)

# Profiling flags
#target_compile_options(Mocassin.Simulator PUBLIC -pg -no-pie)
#target_link_options(Mocassin.Simulator PUBLIC -pg -no-pie)
