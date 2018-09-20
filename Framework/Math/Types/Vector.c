//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	Vector.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   3D, 4D vector math support  //
//////////////////////////////////////////

#include "Vector.h"
#include "Framework/Basic/Macros/Macros.h"
#include "avxintrin.h"

Vector3_t AddVector3(const Vector3_t * lhs, const Vector3_t * rhs)
{
	return (Vector3_t) { lhs->a + rhs->a,lhs->b + rhs->b,lhs->c + rhs->c };
}

Vector3_t SubstractVector3(const Vector3_t * lhs, const Vector3_t * rhs)
{
	return (Vector3_t) { lhs->a - rhs->a,lhs->b - rhs->b,lhs->c - rhs->c };
}

Vector3_t ScalarMultVector3(const Vector3_t * lhs, double rhs)
{
	return (Vector3_t) { lhs->a * rhs, lhs->b * rhs, lhs->c * rhs };
}

Vector3_t ScalarDivideVector3(const Vector3_t * lhs, double rhs)
{
	return (Vector3_t) { lhs->a / rhs, lhs->b / rhs, lhs->c / rhs };
}

double CalcVector3DotProduct(const Vector3_t * lhs, const Vector3_t * rhs)
{
	return lhs->a * rhs->a + lhs->b * rhs->b + lhs->c * rhs->c;
}

#if !defined(__SSE2__)
Vector4_t AddVector4(const Vector4_t * lhs, const Vector4_t * rhs)
{
	return (Vector4_t) { lhs->a + rhs->a,lhs->b + rhs->b,lhs->c + rhs->c,lhs->d + rhs->d };
}

Vector4_t SubstractVector4(const Vector4_t * lhs, const Vector4_t * rhs)
{
	return (Vector4_t) { lhs->a - rhs->a,lhs->b - rhs->b,lhs->c - rhs->c,lhs->d - rhs->d };
}

Vector4_t ScalarMultVector4(const Vector4_t * lhs, int32_t rhs)
{
	return (Vector4_t) { lhs->a * rhs,lhs->b * rhs,lhs->c * rhs,lhs->d * rhs };
}

<<<<<<< HEAD
vector4_t SubtractVector4(const vector4_t * lhs, const vector4_t * rhs)
=======
Vector4_t ScalarDivideVector4(const Vector4_t * lhs, int32_t rhs)
>>>>>>> master
{
	return (Vector4_t) { lhs->a / rhs,lhs->b / rhs,lhs->c / rhs,lhs->d / rhs };
}

int32_t Int32FromVector4(const Vector4_t * restrict value, const int32_t * restrict blockSizes)
{
	return (int32_t)(value->a*blockSizes[0] + value->b*blockSizes[1] + value->c*blockSizes[2] + value->d);
}

int32_t Int32FromVector4Pair(const Vector4_t* restrict start, const Vector4_t* restrict offset, const int32_t* restrict blockSizes)
{
	Vector4_t target = AddVector4(start, offset);
	return Int32FromVector4(&target, blockSizes);
}

Vector4_t Vector4FromInt32(int32_t value, const int32_t * restrict blockSizes)
{
	Vector4_t result;
	result.a = value / blockSizes[0];
	value %= blockSizes[0];
	result.b = value / blockSizes[1];
	value %= blockSizes[1];
	result.c = value / blockSizes[2];
	value %= blockSizes[2];
	result.d = value;
	return result;
}
#else
Vector4_t AddVector4(const Vector4_t * lhs, const Vector4_t * rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) + addressAs(sse2v4i, rhs);
	return addressAs(Vector4_t, &result);
}

Vector4_t SubstractVector4(const Vector4_t * lhs, const Vector4_t * rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) - addressAs(sse2v4i, rhs);
    return addressAs(Vector4_t, &result);
}

Vector4_t ScalarMultVector4(const Vector4_t * lhs, int32_t rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) + rhs;
    return addressAs(Vector4_t, &result);
}

Vector4_t ScalarDivideVector4(const Vector4_t * lhs, int32_t rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) / rhs;
    return addressAs(Vector4_t, &result);
}
#endif

int32_t Int32FromVector4(const Vector4_t * restrict value, const int32_t * restrict blockSizes)
{
    return (int32_t)(value->a*blockSizes[0] + value->b*blockSizes[1] + value->c*blockSizes[2] + value->d);
}

int32_t Int32FromVector4Pair(const Vector4_t* restrict start, const Vector4_t* restrict offset, const int32_t* restrict blockSizes)
{
    Vector4_t target = AddVector4(start, offset);
    return Int32FromVector4(&target, blockSizes);
}

Vector4_t Vector4FromInt32(int32_t value, const int32_t * restrict blockSizes)
{
    Vector4_t result;
    result.a = value / blockSizes[0];
    value %= blockSizes[0];
    result.b = value / blockSizes[1];
    value %= blockSizes[1];
    result.c = value / blockSizes[2];
    value %= blockSizes[2];
    result.d = value;
    return result;
}
