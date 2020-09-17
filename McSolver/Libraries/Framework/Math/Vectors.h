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

#include <math.h>
#include <stdint.h>
#include <immintrin.h>
#include "Libraries/Framework/Basic/BaseTypes/BaseTypes.h"
#include "Libraries/Framework/Basic/Macros/Macros.h"

// Defines the 256 bit 3D double vector struct (A,B,C) and the affiliated type
typedef struct Vector3 { double A, B, C; } Vector3_t;

// Defines the 128 bit 4D int vector struct (A,B,C,D) and the affiliated type (Supports 16 bit alignment)
typedef struct Vector4 { int32_t A, B, C, D; } Vector4_t;

// Perform a vector 3 operation for each value that manipulates the left vector, e.g. adding the second vector to the first
#define vector3VectorOp(LHS, RHS, OP)\
(LHS).A OP (RHS).A;\
(LHS).B OP (RHS).B;\
(LHS).C OP (RHS).C;

// Perform a scalar vector 3 operation for each value that manipulates the left vector, e.g. adding a scalar to all values
#define vector3ScalarOp(LHS, SCALAR, OP)\
(LHS).A OP (SCALAR);\
(LHS).B OP (SCALAR);\
(LHS).C OP (SCALAR);

// Perform a vector 4 operation for each value that manipulates the left vector, e.g. adding the second vector to the first
#define vector4VectorOp(LHS, RHS, OP)\
(LHS).A OP (RHS).A;\
(LHS).B OP (RHS).B;\
(LHS).C OP (RHS).C;\
(LHS).D OP (RHS).D;

// Perform a scalar vector 4 operation for each value that manipulates the left vector, e.g. adding a scalar to all values
#define vector4ScalarOp(LHS, SCALAR, OP)\
(LHS).A OP (SCALAR);\
(LHS).B OP (SCALAR);\
(LHS).C OP (SCALAR);\
(LHS).D OP (SCALAR);

// Expand the first 2 coordinate values of a vector into "A,B"
#define vecCoorSet2(VEC) (VEC).A, (VEC).B

// Expand the first 3 coordinate values of a vector into "A,B,C"
#define vecCoorSet3(VEC) vecCoorSet2(VEC), (VEC).C

// Expand the first 4 coordinate values of a vector into "A,B,C,D"
#define vecCoorSet4(VEC) vecCoorSet3(VEC), (VEC).D


// Performs a vector addition (a + b) and returns the resulting vector
static inline Vector3_t AddVector3(const Vector3_t* restrict lhs, const Vector3_t* restrict rhs)
{
    return (Vector3_t) { lhs->A + rhs->A,lhs->B + rhs->B,lhs->C + rhs->C };
}

// Performs a vector subtraction (a - b) and returns the resulting vector
static inline Vector3_t SubtractVector3(const Vector3_t *lhs, const Vector3_t *rhs)
{
	return (Vector3_t) { lhs->A - rhs->A,lhs->B - rhs->B,lhs->C - rhs->C };
}

// Performs a multiplication of the vector with a scalar and returns the resulting vector
static inline Vector3_t ScalarMultiplyVector3(const Vector3_t *lhs, double rhs)
{
	return (Vector3_t) { lhs->A* rhs, lhs->B * rhs, lhs->C * rhs };
}

// Performs a division of the vector with a scalar and returns the resulting vector
static inline Vector3_t ScalarDivideVector3(const Vector3_t* lhs, double rhs)
{
	return (Vector3_t) { lhs->A / rhs, lhs->B / rhs, lhs->C / rhs };
}

// Performs a dot product calculation (a * b) and returns the result
static inline double CalcVector3DotProduct(const Vector3_t* lhs, const Vector3_t* rhs)
{
	return lhs->A * rhs->A + lhs->B * rhs->B + lhs->C * rhs->C;
}

// Performs a vector addition (a + b) and returns the resulting int vector
static inline Vector4_t AddVector4(const Vector4_t*restrict lhs, const Vector4_t*restrict rhs)
{
	return (Vector4_t) { lhs->A + rhs->A,lhs->B + rhs->B,lhs->C + rhs->C,lhs->D + rhs->D };
}

// Performs a vector subtraction (a - b) and returns the resulting int vector
static inline Vector4_t SubtractVector4(const Vector4_t *lhs, const Vector4_t *rhs)
{
	return (Vector4_t) { lhs->A - rhs->A,lhs->B - rhs->B,lhs->C - rhs->C,lhs->D - rhs->D };
}

// Performs a multiplication of the int vector with a scalar and returns the resulting vector
static inline Vector4_t ScalarMultiplyVector4(const Vector4_t *lhs, int32_t rhs)
{
	return (Vector4_t) { lhs->A * rhs,lhs->B * rhs,lhs->C * rhs,lhs->D * rhs };
}

// Performs a division of the vector with a scalar and returns the resulting vector
static inline Vector4_t ScalarDivideVector4(const Vector4_t* lhs, int32_t rhs)
{
	return (Vector4_t) { lhs->A / rhs,lhs->B / rhs,lhs->C / rhs,lhs->D / rhs };
}

// Performs a cross product calculation (a x b) and returns the resulting vector
static inline Vector3_t CalcVector3CrossProduct(const Vector3_t* lhs, const Vector3_t* rhs)
{
    return (Vector3_t) { lhs->B * rhs->C - lhs->C * rhs->B, lhs->C*rhs->A - lhs->A * rhs->C, lhs->A * rhs->B - lhs->B * rhs->A };
}

// Calculates the euclidean norm calculation for the passed vector
static inline double CalcVector3Length(const Vector3_t* vectorA)
{
    let dotProduct = CalcVector3DotProduct(vectorA, vectorA);
    return sqrt(dotProduct);
}

// Calculates the normalization of the passed vector 3
static inline Vector3_t CalcVector3Normalization(const Vector3_t* vector)
{
    let length = CalcVector3Length(vector);
    return ScalarDivideVector3(vector, length);
}

// Performs a spat product calculation (a * (b x c)) and returns the resulting value (Directional volume)
static inline double CalcVector3SpatProduct(const Vector3_t* vectorA, const Vector3_t* vectorB, const Vector3_t * vectorC)
{
    let crossProduct = CalcVector3CrossProduct(vectorB, vectorC);
    return CalcVector3DotProduct(&crossProduct, vectorA);
}

// Calculates the vector projection of A onto B
static inline Vector3_t CalcVector3Projection(const Vector3_t* vectorA, const Vector3_t* vectorB)
{
    var result = ScalarMultiplyVector3(vectorB, CalcVector3DotProduct(vectorA, vectorB));
    vector3ScalarOp(result, CalcVector3DotProduct(vectorB, vectorB), /=);
    return result;
}

// Performs a conversion of a 4d int vector into an size_t value with the provided 4d size information (4D coordinate decoding by block sizes)
static inline int32_t Int32FromVector4(const Vector4_t* restrict value, const int32_t* restrict blockSizes)
{
    return (int32_t)(value->A*blockSizes[0] + value->B*blockSizes[1] + value->C*blockSizes[2] + value->D);
}

// Performs conversion of a 4d int vector pair describing start and relative offset into a linear id (4D coordinate decoding by block sizes)
static inline int32_t Int32FromVector4Pair(const Vector4_t* restrict start, const Vector4_t* restrict offset, const int32_t* restrict blockSizes)
{
    let target = AddVector4(start, offset);
    return Int32FromVector4(&target, blockSizes);
}

// Performs a conversion of a size_t value into a 4d int vector with the provided 4d size information (4D coordinate encoding by block sizes)
static inline Vector4_t Vector4FromInt32(int32_t value, const int32_t* restrict blockSizes)
{
    Vector4_t result;
    var divValue = div(value, blockSizes[0]);
    result.A = divValue.quot;
    divValue = div(divValue.rem, blockSizes[1]);
    result.B = divValue.quot;
    divValue = div(divValue.rem, blockSizes[2]);
    result.C = divValue.quot;
    result.D = divValue.rem;
    return result;
}


// Checks if the passed 4D vector is out of bounds of the passed reference size 4D vector
static inline bool_t Vector4IsOutOfBounds(const Vector4_t* restrict value, const Vector4_t* restrict refSize)
{
    return_if(value->A >= refSize->A || value->A < 0, true);
    return_if(value->B >= refSize->B || value->B < 0, true);
    return_if(value->C >= refSize->C || value->C < 0, true);
    return_if(value->D >= refSize->D || value->D < 0, true);
    return false;
}

// Performs a periodic trim of a 4d integer vector with the provided sizes (Loop based, faster than modulo due to rare occurence of actual required trim)
static inline void PeriodicTrimVector4(Vector4_t* restrict vector, const Vector4_t* restrict sizes)
{
    while (vector->A <  0) vector->A += sizes->A;
    while (vector->A >= sizes->A) vector->A -= sizes->A;
    while (vector->B <  0) vector->B += sizes->B;
    while (vector->B >= sizes->B) vector->B -= sizes->B;
    while (vector->C <  0) vector->C += sizes->C;
    while (vector->C >= sizes->C) vector->C -= sizes->C;
}