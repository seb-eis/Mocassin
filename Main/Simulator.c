#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include "Framework/Math/Random/PcgRandom.h"
#include "Framework/Math/Types/Vector.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Framework/Errors/McErrors.h"
#include "Simulator/States/SimStates.h"
#include "Simulator/Types/DbModel.h"

DEFINE_MATRIX(uint32_array_t, uint32_matrix_t);

int main(int argc, char const * const *argv)
{ 
    buffer_t buffer = allocate_block_buffer(102);
    matrix_t matrix = buffer_to_matrix(&buffer, 2);
    uint32_matrix_t int_matrix = MATRIX_CAST(uint32_matrix_t, matrix);

    int_matrix.header->rank = 2;
    (&int_matrix.header->block_sizes)[0] = 50;

    uint32_t value = -1;
    for (uint32_t* it = int_matrix.values.start_it; it < int_matrix.values.end_it; it++)
    {
        *it = ++value;
    }
    for (size_t i = 0; i < 2; i++)
    {
        for (size_t j = 0; j < 50; j++)
        {
            printf("%i\n", *(uint32_t*)matrix_get_2d(&int_matrix, 4, i, j));
        }
    }

    return (0);
}