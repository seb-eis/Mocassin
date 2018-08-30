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

vector3_t CalcVector3CrossProduct(const vector3_t * lhs, const vector3_t * rhs)
{
	return (vector3_t)
	{
			lhs->b * rhs->c - lhs->c * rhs->b,
			lhs->c * rhs->a - lhs->a * rhs->c,
			lhs->a * rhs->b - lhs->b * rhs->a
	};
}

double CalcVector3SpatProduct(const vector3_t * a, const vector3_t * b, const vector3_t * c)
{
	vector3_t cross_product = CalcVector3CrossProduct(b, c);
	return CalcVector3DotProduct(a, &cross_product);
}

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
