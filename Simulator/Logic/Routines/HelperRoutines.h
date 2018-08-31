
//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Inlinable helper routines   //
//////////////////////////////////////////

#pragma once
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Constants/Constants.h"
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

#define get_compare(lhs,rhs) ((lhs)==(rhs)) ? 0 : ((lhs)<(rhs)) ? -1 : 1;

static inline void SetCodeByteAt(occode_t* restrict code, const byte_t id, const byte_t value)
{
    MARSHAL_AS(byte_t, code)[id] = value;
}

static inline byte_t GetCodeByteAt(occode_t* restrict code, const byte_t id)
{
    return MARSHAL_AS(byte_t, code)[id];
}

static inline void AddToLhsVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs)
{
    lhs->a += rhs->a;
    lhs->b += rhs->b;
    lhs->c += rhs->c;
    lhs->d += rhs->d;
}

static inline void AddToLhsAndTrimVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs, const vector4_t* sizes)
{
    AddToLhsVector4(lhs, rhs);
    PeriodicTrimVector4(lhs, sizes);
}

static inline env_state_t* GetEnvByVector4(const vector4_t* restrict vec, const env_lattice_t* restrict latt)
{
    return MDA_GET_4(*latt, vec->a, vec->b, vec->c, vec->d);
}

static inline double GetEnergyConvValue(const double temp)
{
    return 1.0 / (temp * NATCONST_BLOTZMANN);
}

static inline void ConvEnergyPhysToBoltz(double* restrict value, const double convValue)
{
    *value *= convValue;
}

static inline void ConvEnergyBoltzToPhys(double* restrict value, const double convValue)
{
    *value /= convValue;
}

static inline int32_t FindMaxJumpDirectionCount(const jump_counts_t* restrict jumpCountTable)
{
    int32_t max = 0;
    FOR_EACH(int32_t, count, *jumpCountTable)
    {
        max = (max < *count) ? *count : max;
    }
    return max;
}

static inline int32_t GetNextCompareDouble(__SCONTEXT_PAR)
{
    return Pcg32NextDouble(&SCONTEXT->RnGen);
}

static inline int32_t GetNextCeiledRnd(__SCONTEXT_PAR, const int32_t upperLimit)
{
    return (int32_t) (Pcg32Next(&SCONTEXT->RnGen) % upperLimit);
}

static inline env_state_t* ResolvePairDefTargetEnvironment(__SCONTEXT_PAR, const pair_def_t* restrict pairDef, const env_state_t* startEnv)
{
    vector4_t target = AddVector4(&startEnv->PosVector, &pairDef->RelVector);
    PeriodicTrimVector4(&target, Get_LatticeSizeVector(SCONTEXT));
    return Get_EnvironmentStateByVector4(SCONTEXT, &target);
}

static inline pair_table_t* RefPairTableAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDbModel.Energy.PairTables.Start[id];
}

static inline clu_table_t* RefCluTableAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDbModel.Energy.CluTables.Start[id];
}

static inline env_state_t* RefLatticeEnvAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDynModel.EnvLattice.Start[id];
}

static inline double* RefActStateEngAt(const __SCONTEXT_PAR, const byte_t id)
{
    return (void*) &SCONTEXT->CycleState.ActWorkEnv->EnergyStates.Start[id];
}

static inline double* RefPathStateEngAt(const __SCONTEXT_PAR, const byte_t pathId, const byte_t parId)
{
    return (void*) &JUMPPATH[pathId]->ClusterStates.Start[parId];
}

static inline double GetPathStateEngAt(const __SCONTEXT_PAR, const byte_t pathId, const byte_t parId)
{
    return *RefPathStateEngAt(SCONTEXT, pathId, parId);
}

static inline jump_dir_t* RefActJumpDir(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActJumpDir;
}

static inline jump_col_t* RefActJumpCol(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActJumpCol;
}

static inline jump_rule_t* RefActJumpRule(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActJumpRule;
}

static inline roll_info_t* RefActRollInfo(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->CycleState.ActRollInfo;
}

static inline occode_t* RefActStateCode(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->CycleState.ActStateCode;
}

static inline occode_t GetActStateCode(const __SCONTEXT_PAR)
{
    return *RefActStateCode(SCONTEXT);
}

static inline env_state_t* RefActWorkEnv(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActWorkEnv;
}

static inline clu_state_t* RefActWorkClu(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActWorkClu;
}

static inline clu_table_t* RefActCluTable(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActCluTable;
}

static inline pair_table_t* RefActPairTable(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActPairTable;
}

static inline byte_t GetActUpdateIdAt(const __SCONTEXT_PAR, const byte_t id)
{
    return RefActWorkEnv(SCONTEXT)->EnvDef->UptParIds[id];
}

static inline eng_info_t* RefActEngInfo(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->CycleState.ActEngInfo;
}

static inline cnt_col_t* RefActCounters(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActCntCol;
}

static inline cnt_state_t* RefStateCounters(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.Counters;
}

static inline cnt_col_t* RefStateCountersAt(const __SCONTEXT_PAR, const byte_t id)
{
    return (void*) &SCONTEXT->SimState.Counters.Start[id];
}

static inline trc_state_t* RefGlobalMoveTrackers(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.GlobalTrackers;
}

static inline tracker_t* RefGlobalMoveTrackerAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &RefGlobalMoveTrackers(SCONTEXT)->Start[id];
}

static inline trc_state_t* RefMobileMoveTrackers(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.MobileTrackers;
}

static inline tracker_t* RefMobileMoveTrackerAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &RefMobileMoveTrackers(SCONTEXT)->Start[id];
}

static inline trc_state_t* RefStaticMoveTrackers(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.StaticTrackers;
}

static inline tracker_t* RefStaticMoveTrackerAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &RefStaticMoveTrackers(SCONTEXT)->Start[id];
}

static inline idx_state_t* RefMobileMoveTrackerIdx(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.MobileTrackerIdx;
}

static inline int32_t* RefMobileMoveTrackerIdxAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &RefMobileMoveTrackerIdx(SCONTEXT)->Start[id];
}

static inline prb_state_t* RefProbabilityStatMap(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.ProbStatMap;
}

static inline int64_t* RefProbabilityStatMapAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &RefProbabilityStatMap(SCONTEXT)->Start[id];
}


static inline env_lattice_t* RefEnvLattice(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDynModel.EnvLattice;
}

static inline vector4_t* RefLatticeSize(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDbModel.LattInfo.SizeVec;
}

static inline buffer_t* RefStateBuffer(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.Buffer;
}

static inline byte_t* RefStateBufferAt(const __SCONTEXT_PAR, const int32_t id)
{
    return &RefStateBuffer(SCONTEXT)->Start[id];
}

static inline hdr_state_t* RefStateHeader(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.Header;
}

static inline hdr_info_t* RefStateHeaderData(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->SimState.Header.Data;
}

static inline mta_state_t* RefStateMeta(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.Meta;
}

static inline meta_info_t* RefStateMetaData(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->SimState.Meta.Data;
}

static inline lat_state_t* RefStateLattice(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimState.Lattice;
}

static inline run_info_t* RefModelRunInfo(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDynModel.RunInfo;
}

static inline job_info_t* RefJobInfo(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDbModel.JobInfo;
}

static inline cycle_cnt_t* RefMainCounters(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->CycleState.MainCnts;
}

static inline jump_counts_t* RefJmpDirCountTable(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDbModel.Transition.JumpCountTable;
}

static inline jump_assign_t* RefJmpAssignTable(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDbModel.Transition.JumpAssignTable;
}

static inline jump_dir_t* RefJumpDirAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDbModel.Transition.JumpDirs.Start[id];
}

static inline jump_cols_t* RefJumpCols(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDbModel.Transition.JumpCols;
}

static inline jump_col_t* RefJumpColAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDbModel.Transition.JumpCols.Start[id];
}

static inline env_state_t* RefEnvStateAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDynModel.EnvLattice.Start[id];
}

static inline env_def_t* RefEnvDefAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDbModel.Structure.EnvDefs.Start[id];
}

static inline clu_def_t* RefEnvCluDefAt(const env_state_t* restrict env, const byte_t id)
{
    return (void*) &env->EnvDef->CluDefs.Start[id];
}

static inline pair_def_t* RefEnvPairDefAt(const env_state_t* restrict env, const int32_t id)
{
    return (void*) &env->EnvDef->PairDefs.Start[id];
}

static inline double* RefCluTableEntry(const clu_table_t* restrict table, const byte_t parId, const int32_t codeId)
{
    return (void*) MDA_GET_2(table->EngTable, table->ParToTableId[parId], codeId);
}

static inline double* RefPairTableEntry(const pair_table_t* restrict table, const byte_t id0, const byte_t id1)
{
    return (void*) MDA_GET_2(table->EngTable, id0, id1);
}

static inline double GetCluTableEntry(const clu_table_t* restrict table, const byte_t parId, const int32_t codeId)
{
    return *RefCluTableEntry(table, parId, codeId);
}

static inline double GetPairTableEntry(const pair_table_t* restrict table, const byte_t id0, const byte_t id1)
{
    return *RefPairTableEntry(table, id0, id1);
}

static inline int32_t* RefEnvPoolEntry(const dir_pool_t* restrict dirPool, const int32_t id)
{
    return (void*) &dirPool->EnvPool.Start[id];
}

static inline int32_t GetEnvPoolEntry(const dir_pool_t* restrict dirPool, const int32_t id)
{
    return *RefEnvPoolEntry(dirPool, id);
}

static inline dir_pool_t* RefJmpDirPoolAt(const jump_pool_t* restrict jmpPool, const int32_t id)
{
    return (void*) &jmpPool->DirPools.Start[id];
}

static inline int32_t GetEnvDirPoolId(jump_pool_t* restrict jmpPool, const jump_counts_t* restrict cntTable, const env_state_t* restrict env)
{
    return jmpPool->DirCountToPoolId.Start[*MDA_GET_2(*cntTable, env->PosVector.d, env->ParId)];
}

static inline clu_state_t* RefCluStateAt(const env_state_t* restrict env, const byte_t id)
{
    return (void*) &env->ClusterStates.Start[id];
}

static inline double* RefStateEnvBackupEngAt(const __SCONTEXT_PAR, const byte_t id)
{
    return (void*) &SCONTEXT->CycleState.ActEnvBackup.PathEnergies[id];
}

static inline double GetStateEnvBackupEngAt(const __SCONTEXT_PAR, const byte_t id)
{
    return *RefStateEnvBackupEngAt(SCONTEXT, id);
}

static inline env_link_t* RefEnvLinkByJmpLink(const __SCONTEXT_PAR, const jump_link_t* link)
{
    return (void*) &JUMPPATH[link->PathId]->EnvLinks.Start[link->LinkId];
}

static inline byte_t FindLastEnvParId(env_def_t* restrict envDef)
{
    for(byte_t i = 0;; i++)
    {
        if(envDef->UptParIds[i] == 0)
        {
            return envDef->UptParIds[i-1];
        }
    }
}

static inline bool_t JobInfoHasFlgs(const __SCONTEXT_PAR, const bitmask_t flgs)
{
    return FLG_TRUE(SCONTEXT->SimDbModel.JobInfo.JobFlg, flgs);
}

static inline bool_t JobHeaderHasFlgs(const __SCONTEXT_PAR, const bitmask_t flgs)
{
    return FLG_TRUE(MARSHAL_AS(mmc_header_t, SCONTEXT->SimDbModel.JobInfo.JobHeader)->JobFlg, flgs);
}

static inline bool_t MainStateHasFlags(const __SCONTEXT_PAR, const bitmask_t flgs)
{
    return FLG_TRUE(SCONTEXT->SimState.Header.Data->Flags, flgs);
}

static inline flp_buffer_t* RefMmcAbortBuffer(const __SCONTEXT_PAR)
{
    return (void*) &SCONTEXT->SimDynModel.LatticeEnergyBuffer;
}

static inline int32_t GetTotalPosCount(const __SCONTEXT_PAR)
{
    vector4_t* sizes = RefLatticeSize(SCONTEXT);
    return sizes->a * sizes->b * sizes->c * sizes->d;
}

static inline byte_t GetMaxParId(const __SCONTEXT_PAR)
{
    int32_t dimensions[2];
    GetMdaDimensions((int32_t*)RefJmpDirCountTable(SCONTEXT)->Header, &dimensions[0]);
    return dimensions[0];
}

static inline const int32_t GetNumberOfUnitCells(__SCONTEXT_PAR)
{
    const vector4_t* sizes = Get_LatticeSizeVector(SCONTEXT);
    return sizes->a * sizes->b * sizes->c;
}
