//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	MocsimRoutineExtension.h   	//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Extension .dll/.so header   //
//////////////////////////////////////////

#pragma once
#include "Extensions/Interface/MocsimExtesionTypes.h"

// Get the pointer to the start of the 16 byte routine identification guid/uuid
const mocuuid_t* MOCEXTENSION_GET_UUID_FUNC();

// Get the pointer to the routine execution entry function that will accept an initialized simulation context
FMocassinRoutine_t MOCEXTENSION_GET_ROUTINE_FUNC();

