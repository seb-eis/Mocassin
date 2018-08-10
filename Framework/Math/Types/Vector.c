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
	return calc_dot_product(a, &cross_product);
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

int32_t Vector4ToInt32(const vector4_t * value, const vector4_t * blockSizes)
{
	return (int32_t)(value->a*blockSizes->a + value->b*blockSizes->b + value->c*blockSizes->c + value->d*blockSizes->d);
}

vector4_t Int32ToVector4(size_t value, const vector4_t * blockSizes)
{
	vector4_t result;
	result.a = (int32_t)(value / blockSizes->a);
	value %= blockSizes->a;
	result.b = (int32_t)(value / blockSizes->b);
	value %= blockSizes->b;
	result.c = (int32_t)(value / blockSizes->c);
	value %= blockSizes->c;
	result.d = (int32_t)(value);
	return result;
}
