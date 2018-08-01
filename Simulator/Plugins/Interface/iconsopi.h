//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	icons_opi.h                 //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Output provider interface   //
//////////////////////////////////////////

typedef int (*on_output_func_t)(const void*);

typedef int (*on_final_output_func_t)(const void*);

int icon_export_on_output(const void* sim_output);

int icon_export_on_final_ouput(const void* sim_output);