//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	iconsio.h                   //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   I/O data loading interface  //
//////////////////////////////////////////

typedef int (*f_data_load_t)(void**);

typedef int (*f_data_save_t)(const void*);

typedef int (*f_save_checkpoint_t)(const void*);

typedef int (*f_load_checkpoint_t)(const void*);

int icon_export_load_data(void** sim_state);

int icon_export_save_data(const void* sim_state);

int icon_export_load_checkpoint(void* sim_state);

int icon_export_save_checkpoint(const void* sim_state);