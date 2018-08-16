
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
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

#define MC_CONST_BLOTZMANN_ELV 8.6173303e-05
#define MC_CONST_JUMPTRACK_MIN 1.0e-05
#define MC_CONST_JUMPTRACK_MAX 1.0e+00
#define MC_CONST_JUMPLIMIT_MIN 0.0e+00
#define MC_CONST_JUMPLIMIT_MAX 1.0e+00

#define FLG_TRUE(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) == 0

#define FLG_FALSE(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) != 0

#define FLG_SET(__VALUE, __FLAG) (__VALUE) |= (__VALUE)

#define FLG_UNSET(__VALUE, __FLAG) (__VALUE) -= ((__VALUE) & (__FLAG))

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

static inline void TrimVector4ToCell(vector4_t* restrict vector, const vector4_t* restrict sizes)
{
    while(vector->a >= sizes->a) vector->a -= sizes->a;
    while(vector->b >= sizes->b) vector->b -= sizes->b;
    while(vector->c >= sizes->c) vector->c -= sizes->c;
}

static inline void AddToLhsAndTrimVector4(vector4_t* restrict lhs, const vector4_t* restrict rhs, const vector4_t* sizes)
{
    AddToLhsVector4(lhs, rhs);
    TrimVector4ToCell(lhs, sizes);
}

static inline env_state_t* GetEnvByVector4(const vector4_t* restrict vec, const env_lattice_t* restrict latt)
{
    return MDA_GET_4(*latt, vec->a, vec->b, vec->c, vec->d);
}

static inline double GetEnergyConvValue(const double temp)
{
    return 1.0 / (temp * MC_CONST_BLOTZMANN_ELV);
}

static inline void ConvEnergyPhysToBoltz(double* restrict value, const double convValue)
{
    *value *= convValue;
}

static inline void ConvEnergyBoltzToPhys(double* restrict value, const double convValue)
{
    *value /= convValue;
}


static inline int32_t GetNextCeiledRnd(__SCONTEXT_PAR, const int32_t upperLimit)
{
    return (int32_t) (Pcg32Next(&SCONTEXT->RnGen) % upperLimit);
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

static inline env_state_t* RefActWorkEnv(__SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActWorkEnv;
}

static inline clu_state_t* RefActWorkClu(__SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActWorkClu;
}

static inline clu_table_t* RefActCluTable(__SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActCluTable;
}

static inline pair_table_t* RefActPairTable(__SCONTEXT_PAR)
{
    return (void*) SCONTEXT->CycleState.ActPairTable;
}

static inline byte_t GetActUpdateIdAt(__SCONTEXT_PAR, const byte_t id)
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

static inline cnt_col_t* RefStateCountersAt(const __SCONTEXT_PAR, const byte_t id)
{
    return (void*) &SCONTEXT->SimState.Counters.Start[id];
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

static inline hdr_info_t* RefStateHeaderData(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->SimState.Header.Data;
}

static inline meta_info_t* RefStateMetaData(const __SCONTEXT_PAR)
{
    return (void*) SCONTEXT->SimState.Meta.Data;
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

static inline jump_col_t* RefJumpColAt(const __SCONTEXT_PAR, const int32_t id)
{
    return (void*) &SCONTEXT->SimDbModel.Transition.JumpCols.Start[id];
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

static inline double* RefStateEnvBackupEngAt(__SCONTEXT_PAR, const byte_t id)
{
    return (void*) &SCONTEXT->CycleState.ActEnvBackup.PathEnergies[id];
}

static inline double GetStateEnvBackupEngAt(__SCONTEXT_PAR, const byte_t id)
{
    return *RefStateEnvBackupEngAt(SCONTEXT, id);
}
