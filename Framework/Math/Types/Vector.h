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

// Defines the 192 bit 3D double vector struct and the affiliated type
typedef struct { double a, b, c; } vector3_t;

// Defines the 128 bit 4D int vector struct and the affiliated type
typedef struct { int32_t a, b, c, d; } vector4_t;

// Perform a scalar operation on any vector with three entries and create a new unamed struct with the result values
#define VEC3_SC_OPERATION(vec0, scalar, op) {vec0.a op scalar, vec0.b op scalar, vec0.c op scalar}

// Perform a memberwise operation on two vectors with three entries and creates new unamed struct with the result values
#define VEC3_MW_OPERATION(vec0, vec1, op) {vec0.a op vec1.a, vec0.b op vec1.b, vec0.c op vec1.c}

// Perform a scalar operation on any vector with four entries and create a new unamed struct with the values
#define VEC4_SC_OPERATION(vec0, scalar, op) {vec0.a op scalar, vec0.b op scalar, vec0.c op scalar, vec0.d op scalar}

// Perform a memberwise operation on two vectors with four entries and creates new unamed struct with the result values
#define VEC4_MW_OPERATION(vec0, vec1, op) {(vec0).a op (vec1).a, (vec0).b op vec1.b, vec0.c op vec1.c, vec0.c op vec1.c, vec0.d op vec1.d}

// Performs a vector addition (a + b) and returns the resulting vector
vector3_t AddVector3(const vector3_t* restrict lhs, const vector3_t* restrict rhs);

// Performs a vector substraction (a - b) and returns the resulting vector
vector3_t SubstractVector3(const vector3_t* lhs, const vector3_t* rhs);

// Performs a multiplication of the vector with a scalar and returns the resulting vector
vector3_t ScalarMultVector3(const vector3_t* lhs, const double rhs);

// Performs a divion of the vector with a scalar and returns the resulting vector
vector3_t ScalarDivideVector3(const vector3_t* lhs, double rhs);

// Performs a dot product claculation (a * b) and returns the result
double CalcVector3DotProduct(const vector3_t* lhs, const vector3_t* rhs);

// Performs a cross product calculation (a x b) and returns the resulting vector
vector3_t CalcVector3CrossProduct(const vector3_t* lhs, const vector3_t* rhs);

// Performs a spat product calculation (a * (b x c)) and returns the resulting value (Directional volume)
double CalcVector3SpatProduct(const vector3_t* vec_a, const vector3_t* vec_b, const vector3_t * vec_c);


// Performs a vector addition (a + b) and retruns the resulting int vector
vector4_t AddVector4(const vector4_t* lhs, const vector4_t* rhs);

// Performs a vector substraction (a - b) and retruns the resulting int vector
vector4_t SubstractVector4(const vector4_t* lhs, const vector4_t* rhs);

// Performs a multiplication of the int vector with a scalar and returns the resulting vector
vector4_t ScalarMultVector4(const vector4_t* lhs, int32_t rhs);

// Performs a divion of the vector with a scalar and returns the resulting vector
vector4_t ScalarDivideVector4(const vector4_t* lhs, int32_t rhs);

// Performs a conversion of a 4d int vector into an size_t value with the provided 4d size information (4D coordinate decoding by block sizes)
int32_t Int32FromVector4(const vector4_t* restrict value, const int32_t* restrict blockSizes);

// Performs conversion of a 4d int vector pair describing start and relative offset into a linear id (4D coordinate decoding by block sizes)
int32_t Int32FromVector4Pair(const vector4_t* restrict start, const vector4_t* restrict offset, const int32_t* restrict blockSizes);

// Performs a conversion of a size_t value into a 4d int vector with the provided 4d size information (4D coordinate encoding by block sizes)
vector4_t Vector4FromInt32(int32_t value, const int32_t* restrict blockSizes);

// Performs a periodic trim of a 4d integer vector with the provided sizes (Loop based, faster than modulo due to rare occurence of actual required trim)
static inline void PeriodicTrimVector4(vector4_t* restrict vector, const vector4_t* restrict sizes)
{
    while (vector->a <  sizes->a) vector->a += sizes->a;
    while (vector->a >= sizes->a) vector->a -= sizes->a;
    while (vector->b <  sizes->b) vector->b += sizes->b;
    while (vector->b >= sizes->b) vector->b -= sizes->b;
    while (vector->c <  sizes->c) vector->c += sizes->c;
    while (vector->c >= sizes->c) vector->c -= sizes->c;
}