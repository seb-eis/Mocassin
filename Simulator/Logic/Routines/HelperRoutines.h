
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

#define FLAG_IS_SET(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) == 0

#define FLAG_NOT_SET(__VALUE, __FLAG) ((__VALUE) & (__FLAG)) != 0

#define SET_FLAG(__VALUE, __FLAG) (__VALUE) |= (__VALUE)

#define UNSET_FLAG(__VALUE, __FLAG) (__VALUE) -= ((__VALUE) & (__FLAG))

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


static inline int32_t GetNextCeiledRnd(_SCTPARAM, const int32_t upperLimit)
{
    return (int32_t) (Pcg32Next(&SCT->RnGen) % upperLimit);
}

static inline env_state_t* RefPathEnvAt(const _SCTPARAM, const byte_t id)
{
    return (void*) SCT->CycleState.ActPathEnvs[id];
}

static inline env_state_t** RefActPathArray(const _SCTPARAM)
{
    return (void*) &SCT->CycleState.ActPathEnvs[0];
}

static inline pair_table_t* RefPairTableAt(const _SCTPARAM, const int32_t id)
{
    return (void*) &SCT->SimDbModel.Energy.PairTables.Start[id];
}

static inline clu_table_t* RefCluTableAt(const _SCTPARAM, const int32_t id)
{
    return (void*) &SCT->SimDbModel.Energy.CluTables.Start[id];
}

static inline env_state_t* RefLatticeEnvAt(const _SCTPARAM, const int32_t id)
{
    return (void*) &SCT->SimDynModel.EnvLattice.Start[id];
}

static inline double* RefActStateEngAt(const _SCTPARAM, const byte_t id)
{
    return (void*) &SCT->CycleState.ActWorkEnv->EnergyStates.Start[id];
}

static inline jump_dir_t* RefActJumpDir(const _SCTPARAM)
{
    return (void*) SCT->CycleState.ActJumpDir;
}

static inline jump_col_t* RefActJumpCol(const _SCTPARAM)
{
    return (void*) SCT->CycleState.ActJumpCol;
}

static inline jump_rule_t* RefActJumpRule(const _SCTPARAM)
{
    return (void*) SCT->CycleState.ActJumpRule;
}

static inline roll_info_t* RefActRollInfo(const _SCTPARAM)
{
    return (void*) &SCT->CycleState.ActRollInfo;
}

static inline occode_t* RefActStateCode(const _SCTPARAM)
{
    return (void*) &SCT->CycleState.ActStateCode;
}

static inline occode_t GetActStateCode(const _SCTPARAM)
{
    return *RefActStateCode(SCT);
}

static inline env_state_t* RefActWorkEnv(_SCTPARAM)
{
    return (void*) SCT->CycleState.ActWorkEnv;
}

static inline clu_state_t* RefActWorkClu(_SCTPARAM)
{
    return (void*) SCT->CycleState.ActWorkClu;
}

static inline clu_table_t* RefActCluTable(_SCTPARAM)
{
    return (void*) SCT->CycleState.ActCluTable;
}

static inline pair_table_t* RefActPairTable(_SCTPARAM)
{
    return (void*) SCT->CycleState.ActPairTable;
}

static inline byte_t GetActUpdateIdAt(_SCTPARAM, const byte_t id)
{
    return RefActWorkEnv(SCT)->EnvDef->UptParIds[id];
}

static inline eng_info_t* RefActEngInfo(const _SCTPARAM)
{
    return (void*) &SCT->CycleState.ActEngInfo;
}

static inline cnt_col_t* RefActCounters(const _SCTPARAM)
{
    return (void*) SCT->CycleState.ActCntCol;
}

static inline cnt_col_t* RefStateCountersAt(const _SCTPARAM, const byte_t id)
{
    return (void*) &SCT->SimState.Counters.Start[id];
}

static inline env_lattice_t* RefEnvLattice(const _SCTPARAM)
{
    return (void*) &SCT->SimDynModel.EnvLattice;
}

static inline vector4_t* RefLatticeSize(const _SCTPARAM)
{
    return (void*) &SCT->SimDbModel.LattInfo.SizeVec;
}

static inline buffer_t* RefStateBuffer(const _SCTPARAM)
{
    return (void*) &SCT->SimState.Buffer;
}

static inline hdr_info_t* RefStateHeaderData(const _SCTPARAM)
{
    return (void*) SCT->SimState.Header.Data;
}

static inline meta_info_t* RefStateMetaData(const _SCTPARAM)
{
    return (void*) SCT->SimState.Meta.Data;
}

static inline run_info_t* RefModelRunInfo(const _SCTPARAM)
{
    return (void*) &SCT->SimDynModel.RunInfo;
}

static inline job_info_t* RefJobInfo(const _SCTPARAM)
{
    return (void*) &SCT->SimDbModel.JobInfo;
}

static inline cycle_cnt_t* RefMainCounters(const _SCTPARAM)
{
    return (void*) &SCT->CycleState.MainCnts;
}

static inline jump_counts_t* RefJmpDirCountTable(const _SCTPARAM)
{
    return (void*) &SCT->SimDbModel.Transition.JumpCountTable;
}

static inline jump_assign_t* RefJmpAssignTable(const _SCTPARAM)
{
    return (void*) &SCT->SimDbModel.Transition.JumpAssignTable;
}

static inline jump_dir_t* RefJumpDirAt(const _SCTPARAM, const int32_t id)
{
    return (void*) &SCT->SimDbModel.Transition.JumpDirs.Start[id];
}

static inline jump_col_t* RefJumpColAt(const _SCTPARAM, const int32_t id)
{
    return (void*) &SCT->SimDbModel.Transition.JumpCols.Start[id];
}

static inline clu_def_t* RefEnvCluDefAt(const env_state_t* restrict env, const byte_t id)
{
    return (void*) &env->EnvDef->CluDefs.Start[id];
}

static inline pair_def_t* RefEnvPairDefAt(const env_state_t* restrict env, const int32_t id)
{
    return (void*) &env->EnvDef->PairDefs.Start[id];
}

static inline double* RefCluTableEntry(const clu_table_t* restrict table, const byte_t parId, const int32_t relId)
{
    return (void*) MDA_GET_2(table->EngTable, table->ParToTableId[parId], relId);
}

static inline double* RefPairTableEntry(const pair_table_t* restrict table, const byte_t id0, const byte_t id1)
{
    return (void*) MDA_GET_2(table->EngTable, id0, id1);
}

static inline double GetCluTableEntry(const clu_table_t* restrict table, const byte_t parId, const int32_t relId)
{
    return *RefCluTableEntry(table, parId, relId);
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
