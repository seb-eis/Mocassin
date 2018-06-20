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

vector_t AddVectors(const vector_t * lhs, const vector_t * rhs)
{
	return (vector_t) { lhs->a + rhs->a,lhs->b + rhs->b,lhs->c + rhs->c };
}

vector_t SubstractVectors(const vector_t * lhs, const vector_t * rhs)
{
	return (vector_t) { lhs->a - rhs->a,lhs->b - rhs->b,lhs->c - rhs->c };
}

vector_t ScalarMultiplyVector(const vector_t * lhs, double rhs)
{
	return (vector_t) { lhs->a * rhs, lhs->b * rhs, lhs->c * rhs };
}

vector_t ScalarDivideVector(const vector_t * lhs, double rhs)
{
	return (vector_t) { lhs->a / rhs, lhs->b / rhs, lhs->c / rhs };
}

double CalcDotProduct(const vector_t * lhs, const vector_t * rhs)
{
	return lhs->a * rhs->a + lhs->b * rhs->b + lhs->c * rhs->c;
}

vector_t CalcCrossProduct(const vector_t * lhs, const vector_t * rhs)
{
	return (vector_t)
	{
			lhs->b * rhs->c - lhs->c * rhs->b,
			lhs->c * rhs->a - lhs->a * rhs->c,
			lhs->a * rhs->b - lhs->b * rhs->a
	};
}

double CalcSpatProduct(const vector_t * a, const vector_t * b, const vector_t * c)
{
	vector_t cross_product = CalcCrossProduct(b, c);
	return CalcDotProduct(a, &cross_product);
}

int_vector_t AddIntVectors(const int_vector_t * lhs, const int_vector_t * rhs)
{
	return (int_vector_t) { lhs->a + rhs->a,lhs->b + rhs->b,lhs->c + rhs->c,lhs->d + rhs->d };
}

int_vector_t SubstractIntVectors(const int_vector_t * lhs, const int_vector_t * rhs)
{
	return (int_vector_t) { lhs->a - rhs->a,lhs->b - rhs->b,lhs->c - rhs->c,lhs->d - rhs->d };
}

int_vector_t ScalarMultiplyIntVector(const int_vector_t * lhs, int32_t rhs)
{
	return (int_vector_t) { lhs->a * rhs,lhs->b * rhs,lhs->c * rhs,lhs->d * rhs };
}

int_vector_t ScalarDivideIntVector(const int_vector_t * lhs, int32_t rhs)
{
	return (int_vector_t) { lhs->a / rhs,lhs->b / rhs,lhs->c / rhs,lhs->d / rhs };
}

size_t ConvIntVectorToInt32(const int_vector_t * value, const int_vector_t * block_sizes)
{
	return (size_t)(value->a*block_sizes->a + value->b*block_sizes->b + value->c*block_sizes->c + value->d*block_sizes->d);
}

int_vector_t ConvInt32ToIntVector(size_t value, const int_vector_t * block_sizes)
{
	int_vector_t result;
	result.a = (int32_t)(value / block_sizes->a);
	value %= block_sizes->a;
	result.b = (int32_t)(value / block_sizes->b);
	value %= block_sizes->b;
	result.c = (int32_t)(value / block_sizes->c);
	value %= block_sizes->c;
	result.d = (int32_t)(value);
	return result;
}
