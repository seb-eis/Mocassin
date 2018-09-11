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

vector3_t AddVector3(const vector3_t * lhs, const vector3_t * rhs)
{
	return (vector3_t) { lhs->a + rhs->a,lhs->b + rhs->b,lhs->c + rhs->c };
}

vector3_t SubstractVector3(const vector3_t * lhs, const vector3_t * rhs)
{
	return (vector3_t) { lhs->a - rhs->a,lhs->b - rhs->b,lhs->c - rhs->c };
}

vector3_t ScalarMultVector3(const vector3_t * lhs, double rhs)
{
	return (vector3_t) { lhs->a * rhs, lhs->b * rhs, lhs->c * rhs };
}

vector3_t ScalarDivideVector3(const vector3_t * lhs, double rhs)
{
	return (vector3_t) { lhs->a / rhs, lhs->b / rhs, lhs->c / rhs };
}

double CalcVector3DotProduct(const vector3_t * lhs, const vector3_t * rhs)
{
	return lhs->a * rhs->a + lhs->b * rhs->b + lhs->c * rhs->c;
}

#if !defined(__SSE2__)
vector4_t AddVector4(const vector4_t * lhs, const vector4_t * rhs)
{
	return (vector4_t) { lhs->a + rhs->a,lhs->b + rhs->b,lhs->c + rhs->c,lhs->d + rhs->d };
}

vector4_t SubstractVector4(const vector4_t * lhs, const vector4_t * rhs)
{
	return (vector4_t) { lhs->a - rhs->a,lhs->b - rhs->b,lhs->c - rhs->c,lhs->d - rhs->d };
}

vector4_t ScalarMultVector4(const vector4_t * lhs, int32_t rhs)
{
	return (vector4_t) { lhs->a * rhs,lhs->b * rhs,lhs->c * rhs,lhs->d * rhs };
}

vector4_t ScalarDivideVector4(const vector4_t * lhs, int32_t rhs)
{
	return (vector4_t) { lhs->a / rhs,lhs->b / rhs,lhs->c / rhs,lhs->d / rhs };
}

int32_t Int32FromVector4(const vector4_t * restrict value, const int32_t * restrict blockSizes)
{
	return (int32_t)(value->a*blockSizes[0] + value->b*blockSizes[1] + value->c*blockSizes[2] + value->d);
}

int32_t Int32FromVector4Pair(const vector4_t* restrict start, const vector4_t* restrict offset, const int32_t* restrict blockSizes)
{
	vector4_t target = AddVector4(start, offset);
	return Int32FromVector4(&target, blockSizes);
}

vector4_t Vector4FromInt32(int32_t value, const int32_t * restrict blockSizes)
{
	vector4_t result;
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
vector4_t AddVector4(const vector4_t * lhs, const vector4_t * rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) + addressAs(sse2v4i, rhs);
	return addressAs(vector4_t, &result);
}

vector4_t SubstractVector4(const vector4_t * lhs, const vector4_t * rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) - addressAs(sse2v4i, rhs);
    return addressAs(vector4_t, &result);
}

vector4_t ScalarMultVector4(const vector4_t * lhs, int32_t rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) + rhs;
    return addressAs(vector4_t, &result);
}

vector4_t ScalarDivideVector4(const vector4_t * lhs, int32_t rhs)
{
    sse2v4i result = addressAs(sse2v4i, lhs) / rhs;
    return addressAs(vector4_t, &result);
}
#endif

int32_t Int32FromVector4(const vector4_t * restrict value, const int32_t * restrict blockSizes)
{
    return (int32_t)(value->a*blockSizes[0] + value->b*blockSizes[1] + value->c*blockSizes[2] + value->d);
}

int32_t Int32FromVector4Pair(const vector4_t* restrict start, const vector4_t* restrict offset, const int32_t* restrict blockSizes)
{
    vector4_t target = AddVector4(start, offset);
    return Int32FromVector4(&target, blockSizes);
}

vector4_t Vector4FromInt32(int32_t value, const int32_t * restrict blockSizes)
{
    vector4_t result;
    result.a = value / blockSizes[0];
    value %= blockSizes[0];
    result.b = value / blockSizes[1];
    value %= blockSizes[1];
    result.c = value / blockSizes[2];
    value %= blockSizes[2];
    result.d = value;
    return result;
}
