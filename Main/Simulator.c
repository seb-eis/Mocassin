#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/Logic/Objects/JumpSelection.h"

int main(int argc, char const * const *argv)
{   
    buffer_t tmp_buffer = AllocateBufferUnchecked(10, sizeof(int32_t));
    int32_list_t int_list = BUFFER_TO_LIST(tmp_buffer, int32_list_t);
    for (int32_t i = 0; i < 10; i++)
    {
        LIST_ADD(int_list, i);
    }
    LIST_POP_BACK(int_list);
    for (int32_t* it = int_list.Start; it < int_list.CurEnd; it++)
    {
        printf("%i\n", *it);
    }
    return (0);
}