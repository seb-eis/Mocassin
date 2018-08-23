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

static size_t ConfigStateHeaderAccess(__SCONTEXT_PAR)
{
    RefStateHeader(SCONTEXT)->Data = (hdr_info_t*) RefStateBuffer(SCONTEXT)->Start;
    return sizeof(hdr_info_t);
}

static size_t ConfigStateMetaAccess(__SCONTEXT_PAR, const int32_t usedBufferBytes)
{
    size_t cfgBufferBytes = sizeof(meta_info_t);

    RefStateHeaderData(SCONTEXT)->MetaByte = usedBufferBytes;   

    RefStateMeta(SCONTEXT)->Data = (meta_info_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateLatticeAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = RefDbModelLattInfo(SCONTEXT)->Lattice.Header->Size;
    RefStateHeaderData(SCONTEXT)->LatticeByte = usedBufferBytes;

    RefStateLattice(SCONTEXT)->Count = cfgBufferBytes;
    RefStateLattice(SCONTEXT)->Start = RefStateBufferAt(SCONTEXT, usedBufferBytes);
    RefStateLattice(SCONTEXT)->End = RefStateLattice(SCONTEXT)->Start + RefStateLattice(SCONTEXT)->Count;

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateCountersAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = sizeof(cnt_col_t) * (size_t) (GetMaxParId(SCONTEXT) + 1);
    RefStateHeaderData(SCONTEXT)->CountersByte = usedBufferBytes;

    RefStateCounters(SCONTEXT)->Count = cfgBufferBytes / sizeof(cnt_col_t);
    RefStateCounters(SCONTEXT)->Start = (cnt_col_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);
    RefStateCounters(SCONTEXT)->End = RefStateCounters(SCONTEXT)->Start + RefStateCounters(SCONTEXT)->Count;

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateGlobalTrcAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    RefStateHeaderData(SCONTEXT)->GlobalTrcByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        RefGlobalMoveTrackers(SCONTEXT)->Count = RefDbModelStructure(SCONTEXT)->GloTrcCount;
        cfgBufferBytes = RefGlobalMoveTrackers(SCONTEXT)->Count * sizeof(tracker_t);

        RefGlobalMoveTrackers(SCONTEXT)->Start = (tracker_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);
        RefGlobalMoveTrackers(SCONTEXT)->End = RefGlobalMoveTrackers(SCONTEXT)->Start + RefGlobalMoveTrackers(SCONTEXT)->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateMobileTrcAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    RefStateHeaderData(SCONTEXT)->MobileTrcByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        RefMobileMoveTrackers(SCONTEXT)->Count = RefDbModelLattInfo(SCONTEXT)->MobCount;
        cfgBufferBytes = RefMobileMoveTrackers(SCONTEXT)->Count * sizeof(tracker_t);

        RefMobileMoveTrackers(SCONTEXT)->Start = (tracker_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);
        RefMobileMoveTrackers(SCONTEXT)->End = RefGlobalMoveTrackers(SCONTEXT)->Start + RefGlobalMoveTrackers(SCONTEXT)->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateStaticTrcAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    RefStateHeaderData(SCONTEXT)->StaticTrcByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        RefStaticMoveTrackers(SCONTEXT)->Count = RefDbModelStructure(SCONTEXT)->CellTrcCount * GetNumberOfUnitCells(SCONTEXT);
        cfgBufferBytes = RefStaticMoveTrackers(SCONTEXT)->Count * sizeof(tracker_t);

        RefStaticMoveTrackers(SCONTEXT)->Start = (tracker_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);
        RefStaticMoveTrackers(SCONTEXT)->End = RefStaticMoveTrackers(SCONTEXT)->Start + RefStaticMoveTrackers(SCONTEXT)->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateMobileTrcIdxAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    RefStateHeaderData(SCONTEXT)->MobileTrcIdxByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        RefMobileMoveTrackerIdx(SCONTEXT)->Count = RefDbModelLattInfo(SCONTEXT)->MobCount;
        cfgBufferBytes = RefMobileMoveTrackerIdx(SCONTEXT)->Count * sizeof(int32_t);

        RefMobileMoveTrackerIdx(SCONTEXT)->Start = (int32_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);
        RefMobileMoveTrackerIdx(SCONTEXT)->End = RefMobileMoveTrackerIdx(SCONTEXT)->Start + RefMobileMoveTrackerIdx(SCONTEXT)->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static size_t ConfigStateProbStatMapAccess(__SCONTEXT_PAR, const size_t usedBufferBytes)
{
    size_t cfgBufferBytes = 0;
    RefStateHeaderData(SCONTEXT)->ProbStatMapByte = JobHeaderHasFlgs(SCONTEXT, FLG_KMC) ? usedBufferBytes : -1;

    if (JobHeaderHasFlgs(SCONTEXT, FLG_KMC))
    {
        RefProbabilityStatMap(SCONTEXT)->Count = RefDbModelLattInfo(SCONTEXT)->MobCount;
        cfgBufferBytes = RefProbabilityStatMap(SCONTEXT)->Count * sizeof(int32_t);

        RefProbabilityStatMap(SCONTEXT)->Start = (int64_t*) RefStateBufferAt(SCONTEXT, usedBufferBytes);
        RefProbabilityStatMap(SCONTEXT)->End = RefProbabilityStatMap(SCONTEXT)->Start + RefProbabilityStatMap(SCONTEXT)->Count;
    }

    return usedBufferBytes + cfgBufferBytes;
}

static error_t ConstructMainStateBufferAccessors(__SCONTEXT_PAR)
{
    size_t usedBufferBytes = 0;

    usedBufferBytes = ConfigStateHeaderAccess(SCONTEXT);
    usedBufferBytes = ConfigStateMetaAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateLatticeAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateCountersAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateGlobalTrcAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrcAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateStaticTrcAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateMobileTrcIdxAccess(SCONTEXT, usedBufferBytes);
    usedBufferBytes = ConfigStateProbStatMapAccess(SCONTEXT, usedBufferBytes);

    return (usedBufferBytes == GetBufferSize(RefStateBuffer(SCONTEXT)));
}

static void ConstructMainState(__SCONTEXT_PAR)
{
    ZeroBuffer(&SCONTEXT->SimState, sizeof(mc_state_t));

    if (AllocateBufferChecked(RefJobInfo(SCONTEXT)->StateSize, 1, RefStateBuffer(SCONTEXT)) != ERR_OK)
    {
        MC_ERROREXIT(ERR_MEMALLOCATION, "Failed to construct main state.");
    }

    ZeroBuffer(RefStateBuffer(SCONTEXT)->Start, RefJobInfo(SCONTEXT)->StateSize);

    if (ConstructMainStateBufferAccessors(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(ERR_DATACONSISTENCY, "Size mismatch between allocated and mapped main state buffer.");
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
