//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DataModel.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Database model definition   //
//////////////////////////////////////////

#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

// Defines the matrix header type tat supplies a rank and a pointer to the block sizes
typedef struct { int32_t rank; int32_t block_sizes; } matrix_header_t;

// Defines a memory accessor with a specified header type and array access struct
#define DEFINE_MEM_ACCESSOR(__NAME, __HEADER_TYPE, __ARRAY_TYPE) typedef struct { __HEADER_TYPE* header; __ARRAY_TYPE values; } __NAME;

// Defines the basic bainary large object memory accessor that has byte pointers to header start, data start and end + 1 entry
DEFINE_MEM_ACCESSOR(blob_t, byte_t, buffer_t);

// Define a new matrix type that carries an array access struct and a header pointer
#define DEFINE_MATRIX(__ARRAY_TYPE, __NAME) typedef struct { matrix_header_t* header; __ARRAY_TYPE values; } __NAME;

// Casts a byte matrix to any other type of matrix access struct
#define MATRIX_CAST(__MATRIX_TYPE, __MATRIX) (__MATRIX_TYPE) { __MATRIX.header, { __MATRIX.values.start_it, __MATRIX.values.end_it } };

// Defines the basic byte matrix
DEFINE_MATRIX(buffer_t, matrix_t);

// Free the memory of a blob on the heap
static inline void free_blob(const blob_t* restrict blob_in)
{
    free(blob_in->header);
}

// Takes a buffer access struct and a header size info and creates a new access struct for the buffer of type blob_t
static inline blob_t buffer_to_blob(const buffer_t* restrict buffer_in, const int32_t header_size)
{
    return (blob_t) { buffer_in->start_it, buffer_in->start_it + header_size, buffer_in->end_it };
}

// Takes a buffer access struct and a rank info and creates a new access struct for the basic matrix type
static inline matrix_t buffer_to_matrix(const buffer_t* restrict buffer_in, const int32_t rank)
{
    return (matrix_t) { buffer_in->start_it, buffer_in->start_it + sizeof(int32_t) * (rank-1), buffer_in->end_it };
}

// Compares the rank of a matrix against an expected value and exits with error if the rank does not match
static inline void debug_rank_check(const matrix_t* restrict matrix_in, const int32_t expected_rank)
{
    if (matrix_in->header->rank != expected_rank)
    {
        MC_DUMP_ERROR_AND_EXIT(MC_MATRIX_RANK_ERROR, "the matrix rank does not match the expected rank");
    }
}

// Access matrix as an 1D matrix with items of the specififed size at the specified index
static inline byte_t* matrix_get_1d(const matrix_t* restrict matrix_in, const size_t item_size, const int32_t a)
{
    #ifdef _DEBUG
        debug_rank_check(matrix_in, 1);
    #endif
    return array_get(&matrix_in->values, item_size, a);
}

// Access matrix as an 2D matrix with items of the specififed size at the specified index
static inline byte_t* matrix_get_2d(const matrix_t* restrict matrix_in, const size_t item_size, const int32_t a, const int32_t b)
{
    #ifdef _DEBUG
        debug_rank_check(matrix_in, 2);
    #endif
    int32_t* sizes = &matrix_in->header->block_sizes;
    return array_get(&matrix_in->values, item_size, a * sizes[0] + b);
}

// Access matrix as an 3D matrix with items of the specififed size at the specified index
static inline byte_t* matrix_get_3d(const matrix_t* restrict matrix_in, const size_t item_size, const int32_t a, const int32_t b, const int32_t c)
{
    #ifdef _DEBUG
        debug_rank_check(matrix_in, 3);
    #endif
    int32_t* sizes = &matrix_in->header->block_sizes;
    return array_get(&matrix_in->values, item_size, a * sizes[0] + b * sizes[1] + c);
}

// Access matrix as an 4D matrix with items of the specififed size at the specified index
static inline byte_t* matrix_get_4d(const matrix_t* restrict matrix_in, const size_t item_size, const int32_t a, const int32_t b, const int32_t c, const int32_t d)
{
    #ifdef _DEBUG
        debug_rank_check(matrix_in, 4);
    #endif
    int32_t* sizes = &matrix_in->header->block_sizes;
    return array_get(&matrix_in->values, item_size, a * sizes[0] + b * sizes[1] + c * sizes[2] + d);
}

// Defines the interaction struct for describing pair interactions of an environment
typedef struct { int_vector_t rel_vector; int32_t table_id; } interaction_t;

// Defines the cluster interaction struct for describing cluster interactions of an environment
typedef struct { int16_t pos_ids[8]; int32_t table_id; } cluster_t;

DEFINE_DYNAMIC_ARRAY(interaction_array_t, interaction_t);
DEFINE_MATRIX(interaction_array_t, interaction_matrix_t);

DEFINE_DYNAMIC_ARRAY(cluster_array_t, cluster_t);
DEFINE_MATRIX(cluster_array_t, cluster_matrix_t);

typedef struct { interaction_matrix_t interaction_matrix; cluster_matrix_t cluster_matrix; } environment_t;


