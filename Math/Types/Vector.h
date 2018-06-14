//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Vector.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   3D, 4D vector math support  //
//////////////////////////////////////////

#pragma once
#include <stdint.h>

// Basic definition of the 192 bit three entry double vector with fields, a, b, c
typedef struct { double a, b, c; } vector_t;

// Basic definition of the 128 bit four entry singned integer vector with fields, a, b, c, d
typedef struct { int32_t a, b, c, d; } int_vector_t;

// Perform a scalar operation on any vector with three entries and create a new unamed struct with the result values
#define VEC3_SC_OPERATION(vec0, scalar, op) {vec0.a op scalar, vec0.b op scalar, vec0.c op scalar}

// Perform a memberwise operation on two vectors with three entries and creates new unamed struct with the result values
#define VEC3_MW_OPERATION(vec0, vec1, op) {vec0.a op vec1.a, vec0.b op vec1.b, vec0.c op vec1.c}

// Perform a scalar operation on any vector with four entries and create a new unamed struct with the values
#define VEC4_SC_OPERATION(vec0, scalar, op) {vec0.a op scalar, vec0.b op scalar, vec0.c op scalar, vec0.d op scalar}

// Perform a memberwise operation on two vectors with four entries and creates new unamed struct with the result values
#define VEC4_MW_OPERATION(vec0, vec1, op) {vec0.a op vec1.a, vec0.b op vec1.b, vec0.c op vec1.c, vec0.c op vec1.c, vec0.d op vec1.d}

// Performs a vector addition (a + b) and returns the resulting vector
vector_t AddVectors(const vector_t* lhs, const vector_t* rhs);

// Performs a vector substraction (a - b) and returns the resulting vector
vector_t SubstractVectors(const vector_t* lhs, const vector_t* rhs);

// Performs a multiplication of the vector with a scalar and returns the resulting vector
vector_t ScalarMultiplyVector(const vector_t* lhs, double rhs);

// Performs a divion of the vector with a scalar and returns the resulting vector
vector_t ScalarDivideVector(const vector_t* lhs, double rhs);

// Performs a dot product claculation (a * b) and returns the result
double CalcDotProduct(const vector_t* lhs, const vector_t* rhs);

// Performs a cross product calculation (a x b) and returns the resulting vector
vector_t CalcCrossProduct(const vector_t* lhs, const vector_t* rhs);

// Performs a spat product calculation (a * (b x c)) and returns the resulting value (Directional volume)
double CalcSpatProduct(const vector_t* vec_a, const vector_t* vec_b, const vector_t * vec_c);


// Performs a vector addition (a + b) and retruns the resulting int vector
int_vector_t AddIntVectors(const int_vector_t* lhs, const int_vector_t* rhs);

// Performs a vector substraction (a - b) and retruns the resulting int vector
int_vector_t SubstractIntVectors(const int_vector_t* lhs, const int_vector_t* rhs);

// Performs a multiplication of the int vector with a scalar and returns the resulting vector
int_vector_t ScalarMultiplyIntVector(const int_vector_t* lhs, int32_t rhs);

// Performs a divion of the vector with a scalar and returns the resulting vector
int_vector_t ScalarDivideIntVector(const int_vector_t* lhs, int32_t rhs);

// Performs a conversion of a 4d int vector into an size_t value with the provided 4d size information (4D coordinate decoding by block sizes)
size_t ConvIntVectorToInt32(const int_vector_t* value, const int_vector_t* block_sizes);

// Performs a conversion of a size_t value into a 4d int vector with the provided 4d size information (4D coordinate encoding by block sizes)
int_vector_t ConvInt32ToIntVector(size_t value, const int_vector_t* block_sizes);