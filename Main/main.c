#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <time.h>
#include <math.h>
#include "Simulator/Data/Model/Database/DbModelLoad.h"
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Objects/JumpSelection.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"
#include "Simulator/Logic/Routines/Main/MainRoutines.h"
#include "Simulator/Logic/Validators/Validators.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Initializers/ContextInitializer.h"
#include "Framework/Basic/BaseTypes/Buffers.h"

typedef struct IntSpan { int* Begin, * End; } IntSpan_t ;

NEW_STRUCTTYPE(struct IntSpan, Int32Span_t);

#if !defined(MC_TESTBUILD)

    int main(int argc, char const * const *argv)
    {
        Array_t(Int32Span_t, 3) array = new_Array(array, 2,3,3);

        for (int i = 0; i < 2; ++i)
        {
          for (int j = 0; j < 3; ++j)
          {
            for (int k = 0; k < 3; ++k)
            {
                IntSpan_t span = new_Span(array_Get(array, i,j,k), 100);

                int32_t index = 0;
                cpp_foreach(it, span)
                {
                    *it = index++;
                }
            }
          }
        }

        vtypeof_span(array) span = array_Get(array, 1,1,1);
        cpp_rforeach(it, span)
        {
            printf("%i, ", *it);
        }

        return (0);
    }

#else

    int main(int argc, char const * const *argv)
    {
        sim_context_t SCONTEXT;

        ResolveCommandLineArguments(&SCONTEXT, argc, argv);

        LoadSimulationModelFromDatabase(&SCONTEXT);

        PrepareContextForSimulation(&SCONTEXT);

        PrepareForMainRoutine(&SCONTEXT);

        StartMainRoutine(&SCONTEXT);
    }

#endif