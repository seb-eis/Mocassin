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

static error_t ConstructEngStateBuffer(eng_states_t *restrict bufferAccess, const byte_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(double));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, eng_states_t);
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructEnvLinkBuffer(env_links_t *restrict bufferAccess, const int32_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(env_link_t));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, env_links_t);
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructCluStateBuffer(clu_states_t *restrict bufferAccess, const byte_t count)
{
    buffer_t tmp = AllocateBufferUnchecked(count, sizeof(clu_state_t));
    *bufferAccess = BUFFER_TO_ARRAY_WCOUNT(tmp, count, clu_states_t);
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static error_t ConstructEnvBuffers(env_state_t *restrict env, env_def_t *restrict envDef)
{
    error_t error = ERR_OK;
    ZeroBuffer(env, sizeof(env_state_t));

    error |= ConstructEngStateBuffer(&env->EnergyStates, FindLastEnvParId(envDef) + 1);
    error |= ConstructCluStateBuffer(&env->ClusterStates, envDef->CluDefs.Count);
    error |= ConstructEnvLinkBuffer(&env->EnvLinks, envDef->PairDefs.Count);

    return ERR_OK;
}

static void ConstructEnvLattice(__SCONTEXT_PAR)
{
    if (AllocateMdaChecked(4, sizeof(env_state_t), (int32_t*) RefLatticeSize(SCONTEXT), (blob_t*)RefEnvLattice(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(ERR_MEMALLOCATION, "Environment lattice construction.");
    }

    for (int32_t i = 0; i < RefEnvLattice(SCONTEXT)->Header->Size; i++)
    {
        if (ConstructEnvBuffers(RefEnvStateAt(SCONTEXT, i), RefEnvDefAt(SCONTEXT, i)) != ERR_OK)
        {
            MC_ERROREXIT(ERR_MEMALLOCATION, "Environment buffer constructions.");
        }
    }
}

static error_t ConstructMmcEngBuffer(flp_buffer_t* restrict bufferAccess, mmc_header_t* restrict header)
{
    buffer_t tmp = AllocateBufferUnchecked(header->AbortSeqLen, sizeof(double));
    *bufferAccess = (flp_buffer_t) { header->AbortSeqLen, 0.0, (void*) tmp.Start, (void*) tmp.End, (void*) tmp.End };
    return tmp.Start ? ERR_OK : ERR_MEMALLOCATION;
}

static void ConstructAbortCondBuffers(__SCONTEXT_PAR)
{
    if (JobInfoHasFlgs(SCONTEXT, FLG_MMC))
    {
        if (ConstructMmcEngBuffer(RefMmcAbortBuffer(SCONTEXT), RefJobInfo(SCONTEXT)->JobHeader) != ERR_OK)
        {
            MC_ERROREXIT(ERR_MEMALLOCATION, "Mmc energy buffers.")
        }
    }
}

static void ConstructSimModel(__SCONTEXT_PAR)
{
    ConstructEnvLattice(SCONTEXT);
    ConstructAbortCondBuffers(SCONTEXT);
}

static void ConstructJumpPool(__SCONTEXT_PAR)
{

}

static error_t ConstructMainStateBufferAccessors(__SCONTEXT_PAR)
{
    return ERR_OK;
}

static void ConstructMainState(__SCONTEXT_PAR)
{
    if (AllocateBufferChecked(RefJobInfo(SCONTEXT)->StateSize, 1, RefStateBuffer(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(ERR_MEMALLOCATION, "Main state construction.");
    }
    if (ConstructMainStateBufferAccessors(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(ERR_DATACONSISTENCY, "Main state accessor construction.");
    }
}

void ConstructSimulationContext(__SCONTEXT_PAR)
{
    ConstructSimModel(SCONTEXT);
    ConstructMainState(SCONTEXT);
    ConstructJumpPool(SCONTEXT);
}

static error_t LoadAndSetPluginFunctions(__SCONTEXT_PAR)
{
    return ERR_OK;
}

void PopulateSimulationContext(__SCONTEXT_PAR)
{

}
