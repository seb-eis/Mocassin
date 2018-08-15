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

int main(int argc, char const * const *argv)
{   
    sim_context_t SCT;
    for(byte_t i = 0; i < 8; i++)
    {
        RefActPathArray(&SCT)[i] = NULL;
    }
    return (0);
}