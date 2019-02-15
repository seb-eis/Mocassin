//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Errors.h         		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Error codes + messages      //
//////////////////////////////////////////

#pragma once
#include <time.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>

// Defines the MC test build macro that enables most of the debugging assertions and save getter/setters
#define MC_TESTBUILD

// Define to enable all debug assertions
#define ENABLE_DEBUG_ASSERTIONS

#if defined(ENABLE_DEBUG_ASSERTIONS)
    // Active debug assertion macro. Asserts that the condition is true during runtime
    #define debug_assert(cond) if (!(cond)) { ErrorToStdout(ERR_DEBUGASSERT, __FUNCTION__, __LINE__, #cond); }
#else
    // Deactivated assertion macro. Expands to nothing
    #define debug_assert(cond)
#endif

// Macro for throwing an error information to the console on debugging

#if defined(MC_TESTBUILD)
    // Defines the default exception handling for debug builds
    #define errorhandle_default(code, msg) error_display(code, msg)
#else
    // Defines the default exception handling action for non debug builds
    #define errorhandle_default(code, msg) error_exit(code, msg)
#endif

// Defines error codes to be a 32 bit signed integer. Returns type of functions that can return error codes
typedef int32_t error_t;

// Defines long error codes to be 64 bit signed integer. Returns type of functions that perform count operations and might return negatice error codes
typedef int64_t cerror_t;

// Defines the format of the default error output
#define ERROR_FORMAT "ERROR:\t0x%08x\nFunc\t%s\nLine:\t%d\nType:\t%s\nInfo:\t%s\n"

// Defines the format of the default error output with memory dump
#define ERROR_FORMAT_WDUMP "ERROR:\t0x%08x\nFunc:t%s\nLine:\t%d\nType:\t%s\nInfo:\t%s\nBuffer:\n\n"

// Defines the path to the debug stderr dump folder
#define STDERR_PATH "./Debug/stderr.log"

// Defines the error code that indicates that a default value should be used (Not translatable to string)
#define ERR_USEDEFAULT (-1)

// Defines error code for cases where an uncritical error should trigger continue
#define ERR_CONTINUE (-2)

// Defines the error code for no error
#define ERR_OK 0

// Defines the error code for errors during stream usage
#define ERR_STREAM 1

// Defines the error code for errors during memory block dumping due to incompatible dump buffer length
#define ERR_BLOCKDUMP 2

// Defines the error code for errors during array access type casting where accessing thorugh the new type would cause a buffer overflow
#define ERR_BUFFERCAST 3

// Defines the error code for buffer overflows
#define ERR_BUFFEROVERFLOW 4

// Defines the error code for file errors
#define ERR_FILE 5

// Defines the error codes for cases where a wrong file mode was passed to a function
#define ERR_FILEMODE 6

// Defines the error code for cases where the database access caused an unspecified error
#define ERR_DATABASE 7

// Defines the error code for cases where simulation routines cause an unspecififed error
#define ERR_UNKNOWN 8

// Defines the error code for cases where memory allocation fails
#define ERR_MEMALLOCATION 9

// Defines the error code for cases when a data inconsistency is found
#define ERR_DATACONSISTENCY 10

// Defines the error code for cases where hash value checks for file manipulation detection fail
#define ERR_HASHPROTECTION 11

// Defines error code for cases where a requested plugin is not found
#define ERR_LIBRARYLOADING 12

// Defines the error code for cases where a requested plugin import function is not found
#define ERR_FUNCTIONIMPORT 13

// Defines the error code for cases where a requested command line argument does not exist or is invalid
#define ERR_CMDARGUMENT 14

// Defines error code for validation failures
#define ERR_VALIDATION 15

// Defines the error code for functions that are not implemented
#define ERR_NOTIMPLEMENTED 16

// Defines error for cases where a nullpointer is an invalid result or argument
#define ERR_NULLPOINTER 17

// Defines error for cases where a function argument is invalid
#define ERR_ARGUMENT 18

// Defines error for cases where a debug assertion fails
#define ERR_DEBUGASSERT 19

// Defines the default error display with code and message
#define error_display(__CODE, __MSG) ErrorToStdout(__CODE, __FUNCTION__, __LINE__, __MSG);

// Defines the simulator error dump macro. Dumps error information to stderr and quits programm with error code
#define error_exit(__CODE, __MSG) OnErrorExit(__CODE, __FUNCTION__, __LINE__, __MSG);

// Defines the simulator error and memory dump macro. Dumps error information to stderr and quits programm with error code
#define error_exitdump(__CODE, __MSG, __BSTART, __BEND) OnErrorExitWithMemDump(__CODE, __FUNCTION__, __LINE__, __MSG, __BSTART, __BEND);

// Asserts that the passed condition is true. Calls default error handling if condition is false
#define runtime_assertion(cond, error, msg) if (!(cond)) errorhandle_default((error), (msg))

// Asserts that the error is ERR_OK and if not calls the default runtime assert reaction
#define error_assert(error, msg) runtime_assertion((error) == (ERR_OK), (error), (msg))

// Macro for conditional one line returns statements
#define return_if(cond, ...) if (cond) return __VA_ARGS__

// Macro for conditional one line continue statements
#define continue_if(cond) if (cond) continue

// Macro for conditional one line break statements
#define break_if(cond) if (cond) break

// Get an error description string for the passed error Code
const char* ErrorCodeToString(error_t errCode);

// Error handling call for debugging that dumps the information the stdout
void ErrorToStdout(int32_t errCode, const char *errFunc, int32_t errLine, const char *errMsg);

// Dumps the passed error information to stderr and exists the program with the provided code. 
void OnErrorExit(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg);

// Dumps the passed error information to stderr and exists the program with the provided code. Dumps memory bytes between passed byte pointers
void OnErrorExitWithMemDump(int32_t errCode, const char* errFunc, int32_t errLine, const char* errMsg, uint8_t* memStart, uint8_t* memEnd);

// Invokes the passed function pointer and returns the elapsed time in milliseconds
static inline double InvokeAndProfile(void (*func)(void))
{
    clock_t start = clock();
    func();
    clock_t end = clock();
    return (double)(end - start);
}