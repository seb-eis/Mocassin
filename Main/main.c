#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Initializers/ContextInitializer.h"

int main(int argc, char const * const *argv)
{   
    sim_context_t* SCONTEXT = malloc(sizeof(sim_context_t));
    
    char const * values[] = { "./", "-dbPath", "./Main/Simulator.c", "-outPluginPath", "./Main/Simulator.c", "-outPluginSymbol", "MyFunction" };
    int32_t count = sizeof(values) / sizeof(char*);

    ResolveCommandLineArguments(SCONTEXT, count, &values[0]);
     
    return (0);
}