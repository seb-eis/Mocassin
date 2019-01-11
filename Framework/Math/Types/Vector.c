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

#if defined(__SSE2__)

Vector3_t AddVector3(const Vector3_t * lhs, const Vector3_t * rhs)
{
    __sse4d result = addressAs(__sse4d, lhs) + addressAs(__sse4d, rhs);
    return addressAs(Vector3_t, &result);
}

Vector3_t SubstractVector3(const Vector3_t * lhs, const Vector3_t * rhs)
{
    __sse4d result = addressAs(__sse4d, lhs) - addressAs(__sse4d, rhs);
    return addressAs(Vector3_t, &result);
}

Vector3_t ScalarMultVector3(const Vector3_t * lhs, double rhs)
{
    __sse4d result = addressAs(__sse4d, lhs) * rhs;
    return addressAs(Vector3_t, &result);
}

Vector3_t ScalarDivideVector3(const Vector3_t * lhs, double rhs)
{
    __sse4d result = addressAs(__sse4d, lhs) / rhs;
    return addressAs(Vector3_t, &result);
}

double CalcVector3DotProduct(const Vector3_t * lhs, const Vector3_t * rhs)
{
    __sse4s result = addressAs(__sse4s, lhs) * addressAs(__sse4s, rhs);
    return result[0] + result[1] + result[2];
}

Vector4_t AddVector4(const Vector4_t * lhs, const Vector4_t * rhs)
{
    __sse4s result = addressAs(__sse4s, lhs) + addressAs(__sse4s, rhs);
	return addressAs(Vector4_t, &result);
}

Vector4_t SubstractVector4(const Vector4_t * lhs, const Vector4_t * rhs)
{
    __sse4s result = addressAs(__sse4s, lhs) - addressAs(__sse4s, rhs);
    return addressAs(Vector4_t, &result);
}

Vector4_t ScalarMultVector4(const Vector4_t * lhs, int32_t rhs)
{
    __sse4s result = addressAs(__sse4s, lhs) + rhs;
    return addressAs(Vector4_t, &result);
}

Vector4_t ScalarDivideVector4(const Vector4_t * lhs, int32_t rhs)
{
    __sse4s result = addressAs(__sse4s, lhs) / rhs;
    return addressAs(Vector4_t, &result);
}

#else

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

Vector4_t ScalarDivideVector4(const Vector4_t * lhs, int32_t rhs)
{
	return (Vector4_t) { lhs->a / rhs,lhs->b / rhs,lhs->c / rhs,lhs->d / rhs };
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
