//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	BaseTypes.c        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Array + type definitions    //
//////////////////////////////////////////

#include "Framework/Basic/BaseTypes/BaseTypes.h"

bool_t buffer_is_identical(const buffer_t* buffer_0, const buffer_t* buffer_1)
{
    if (buffer_0->start_it == buffer_1->start_it && buffer_0->end_it == buffer_1->end_it)
    {
        return true;
    }
    if (get_buffer_size(buffer_0) != get_buffer_size(buffer_1))
    {
        return false;
    }
    byte_t* it_0 = buffer_0->start_it;
    byte_t* it_1 = buffer_1->start_it;
    while (it_0 != buffer_0->end_it)
    {
        if (*(++it_0) != *(++it_1))
        {
            return false;
        }
    }
    return true;
}