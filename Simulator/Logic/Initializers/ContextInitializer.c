//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	ContextInitializer.h   		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Context initializer logic   //
//////////////////////////////////////////

#include "Simulator/Logic/Initializers/ContextInitializer.h"
#include "Simulator/Logic/Routines/HelperRoutines.h"

#define CHECKED_BUFFER_CONSTRUCTION(__TYPE, __ACCESSORTYPE, __ACCESSOR, __SIZE){\
    buffer_t tmp = AllocateBufferUnchecked(__SIZE, sizeof(__TYPE));\
    *__ACCESSOR = (__ACCESSORTYPE) { __SIZE, tmp.Start, tmp.End };\
    return (tmp.Start == NULL) ? ERR_OK : ERR_MEMALLOCATION;}

static error_t ConstructEngStateBuffer(eng_states_t* restrict bufferAccess, const byte_t count)
{
    CHECKED_BUFFER_CONSTRUCTION(double, eng_states_t, bufferAccess, count);
}

static error_t ConstructEnvLinkBuffer(env_links_t* restrict bufferAccess, const int32_t count)
{
    CHECKED_BUFFER_CONSTRUCTION(env_link_t, env_links_t, bufferAccess, count);
}

static error_t ConstructCluStateBuffer(clu_states_t* restrict bufferAccess, const byte_t count)
{
    CHECKED_BUFFER_CONSTRUCTION(clu_state_t, clu_states_t, bufferAccess, count);
}

static error_t ConstructEnvBuffers(env_state_t* restrict env, env_def_t* restrict envDef)
{
    ZeroBuffer(env, sizeof(env_state_t));
    if(ConstructEngStateBuffer(&env->EnergyStates, FindLastEnvParId(envDef) + 1) != ERR_OK)
    {
        MC_ERROREXIT(ERR_MEMALLOCATION, "Allocation error. Out of memory - Energy state buffers.")
    }
    if(ConstructCluStateBuffer(&env->ClusterStates, envDef->CluDefs.Count) != ERR_OK)
    {
        MC_ERROREXIT(ERR_MEMALLOCATION, "Allocation error. Out of memory - Cluster state buffers.")
    }
    if(ConstructEnvLinkBuffer(&env->EnvLinks, envDef->PairDefs.Count) != ERR_OK)
    {
        MC_ERROREXIT(ERR_MEMALLOCATION, "Allocation error. Out of memory - Env linker buffers.")
    }
    return ERR_OK;
}

static error_t ConstructEnvLattice(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t ConstructSimulationContext(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t PopulateSimulationContext(__SCONTEXT_PAR)
{
    return ERR_OK;
}
