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

int main(int argc, char const * const *argv)
{   

    int32_t count = 1000000000;
    buffer_t buffer = AllocateBufferUnchecked(count, 1);
    double result = 0;

    FOR_EACH(byte_t, it, buffer)
    {
        *it = Pcg32GlobalNext() % UINT8_MAX;
    }

    clock_t start = clock();
    FOR_EACH(byte_t, it, buffer)
    {
        result += *it;
    }
    clock_t end = clock();
    printf("Run 0 - Value: %f, Time is %ld ms\n", result, end-start);

    start = clock();
    for(int32_t i = 0; i < count; i++)
    {
        result += buffer.Start[i];
    }
    end = clock();
    printf("Run 1 - Value: %f, Time is %ld ms\n", result, end-start);

    return (0);
}