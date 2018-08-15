//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#include "Simulator/Logic/Routines/MainRoutines.h"
#include "Simulator/Data/Model/State/StateLogic.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Logic/Objects/JumpSelection.h"

error_t LoadCommandLineArgs(_SCTPARAM, int32_t argc, char const* const* argv)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationPlugins(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationModel(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationState(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t ResetContextToDefault(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t PrepareDynamicModel(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t PrepareForMainRoutine(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t StartMainRoutine(_SCTPARAM)
{
    if (FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_STATE_ERROR_FLAG))
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Simulation error. Simulation routine not started due to set state error flag.");
    }

    if (FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_KMC_ROUTINE_FLAG))
    {
        if(FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_PRE_ROUTINE_FLAG))
        {
            if(StartMainKmcRoutine(SCT) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. Prerun execution of main KMC routine returned with an error.");
            }
            if(FinishRoutinePrerun(SCT) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. Finisher for prerun execution of main KMC routine returned with an error.");
            }
        }
        return StartMainKmcRoutine(SCT);
    }

    if (FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_MMC_ROUTINE_FLAG))
    {
        if(FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_PRE_ROUTINE_FLAG))
        {
            if(StartMainKmcRoutine(SCT) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. Prerun execution of main MMC routine returned with an error.");
            }
            if(FinishRoutinePrerun(SCT) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. Finisher for prerun execution of main MMC routine returned with an error.");
            }
        }
        return StartMainMmcRoutine(SCT);
    }
    MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Unexpected simulation error. No routine selection triggered.");
    return -1;
}

error_t FinishRoutinePrerun(_SCTPARAM)
{
    if(SaveSimulationState(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Simulation error. State save after prerun completion failed.")
    }

    if(ResetContextToDefault(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Simulation error. Context reset after prerun completion failed.");
    }

    UNSET_FLAG(RefStateHeaderData(SCT)->Flags, MC_PRE_ROUTINE_FLAG);
    return SCT->ErrorCode;
}

error_t StartMainKmcRoutine(_SCTPARAM)
{
    while(GetKmcAbortCondEval(SCT) == MC_SIM_CONTINUE_FLAG)
    {
        if(DoNextKmcCycleBlock(SCT) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. The KMC routine block execution returned with an error.")
        }
        if(FinishKmcCycleBlock(SCT) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. The finisher for the KMC block routine execution returned with an error.")
        }
    }
    return FinishMainRoutineKmc(SCT);
}

error_t StartMainMmcRoutine(_SCTPARAM)
{
    while(GetMmcAbortCondEval(SCT) == MC_SIM_CONTINUE_FLAG)
    {
        if(DoNextMmcCycleBlock(SCT) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. The MMC routine block execution returned with an error.")
        }
        if(FinishMmcCycleBlock(SCT) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. The finisher for the MMC block routine execution returned with an error.")
        }
    }
    return FinishMainRoutineMmc(SCT);
}

static inline void AdvanceBlockCounters(_SCTPARAM)
{
    RefMainCounters(SCT)->CurTargetMcs += RefMainCounters(SCT)->McsPerBlock;
}

static inline error_t CallOutputPlugin(_SCTPARAM)
{
    if(SCT->Plugins.OnDataOut != NULL)
    {
        SCT->Plugins.OnDataOut(SCT);
        return SCT->ErrorCode;
    }
    return MC_NO_ERROR;
}

error_t DoNextKmcCycleBlock(_SCTPARAM)
{
    while(RefMainCounters(SCT)->CurMcs < RefMainCounters(SCT)->CurTargetMcs)
    {
        for(size_t i = 0; i < RefMainCounters(SCT)->MinCyclesPerBlock; i++)
        {
            SetKmcJumpSelection(SCT);
            SetKmcJumpPathProperties(SCT);

            if(GetKmcJumpRuleEval(SCT))
            {
                SetKmcJumpProperties(SCT);
                SetKmcJumpEvaluation(SCT);
            }
            else
            {
                SetKmcJumpEvalFail(SCT);
            }
        }
    }
    return SCT->ErrorCode;
}

static void SharedCycleBlockFinish(_SCTPARAM)
{
    UNSET_FLAG(RefStateHeaderData(SCT)->Flags, MC_SIM_FIRST_CYCLE_FLAG);

    if(SyncSimulationState(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. Synchronization between simulation data and state object retuned with an error.")
    }

    if(SaveSimulationState(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. State save operation after block completion returned with an error.");
    }
    
    if(CallOutputPlugin(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. Loaded ouput plugin call retuned with an error.");
    }
}

error_t FinishKmcCycleBlock(_SCTPARAM)
{
    AdvanceBlockCounters(SCT);
    SharedCycleBlockFinish(SCT);

    return SCT->ErrorCode;
}

error_t DoNextMmcCycleBlock(_SCTPARAM)
{
    while(RefMainCounters(SCT)->CurMcs < RefMainCounters(SCT)->CurTargetMcs)
    {
        for(size_t i = 0; i < RefMainCounters(SCT)->MinCyclesPerBlock; i++)
        {
            SetMmcJumpSelection(SCT);
            SetMmcJumpPathProperties(SCT);

            if(GetMmcJumpRuleEval(SCT))
            {
                SetMmcJumpProperties(SCT);
                SetMmcJumpEvaluation(SCT);
            }
            else
            {
                SetMmcJumpEvalFail(SCT);
            }
        }
    }
    return SCT->ErrorCode;
}

error_t FinishMmcCycleBlock(_SCTPARAM)
{
    AdvanceBlockCounters(SCT);
    SharedCycleBlockFinish(SCT);

    return SCT->ErrorCode;
}

static inline bool_t GetTimeoutAbortEval(_SCTPARAM)
{
    clock_t newClock = clock();
    RefStateMetaData(SCT)->TimePerBlock = (newClock - RefModelRunInfo(SCT)->LastClock) / CLOCKS_PER_SEC;
    RefModelRunInfo(SCT)->LastClock = newClock;
    RefStateMetaData(SCT)->RunTime = newClock / CLOCKS_PER_SEC;
    int64_t blockEta = RefStateMetaData(SCT)->TimePerBlock + RefStateMetaData(SCT)->RunTime;

    if(RefStateMetaData(SCT)->RunTime >= RefJobInfo(SCT)->TimeLimit)
    {
        return MC_SIM_TIME_ABORT_FLAG;
    }
    if(blockEta > RefJobInfo(SCT)->TimeLimit)
    {
        return MC_SIM_TIME_ABORT_FLAG;
    }
    return MC_SIM_CONTINUE_FLAG;
}

static inline bool_t GetRateAbortEval(_SCTPARAM)
{
    RefStateMetaData(SCT)->SuccessRate = RefMainCounters(SCT)->CurMcs / RefStateMetaData(SCT)->RunTime;
    RefStateMetaData(SCT)->CyleRate = RefMainCounters(SCT)->CurCycles / RefStateMetaData(SCT)->RunTime;

    if(RefStateMetaData(SCT)->CyleRate < RefJobInfo(SCT)->MinSuccRate)
    {
        return true;
    }
    return false;
}

static inline bool_t GetMcsTargetReachedEval(_SCTPARAM)
{
    if(RefMainCounters(SCT)->CurMcs >= RefMainCounters(SCT)->TotTargetMcs)
    {
        return true;
    }
    return false;
}

static error_t GetGeneralAbortCondEval(_SCTPARAM)
{
    if(FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_SIM_FIRST_CYCLE_FLAG))
    {
        return MC_SIM_CONTINUE_FLAG;
    }

    if(GetTimeoutAbortEval(SCT))
    {
        SET_FLAG(RefStateHeaderData(SCT)->Flags, MC_SIM_TIME_ABORT_FLAG);
        return MC_SIM_TIME_ABORT_FLAG;
    }

    if(GetRateAbortEval(SCT))
    {
        SET_FLAG(RefStateHeaderData(SCT)->Flags, MC_SIM_RATE_ABORT_FLAG);
        return MC_SIM_RATE_ABORT_FLAG;
    }

    if(GetMcsTargetReachedEval(SCT))
    {
        SET_FLAG(RefStateHeaderData(SCT)->Flags, MC_SIM_COMPLETED_FLAG);
        return MC_SIM_COMPLETED_FLAG;
    }

    return MC_SIM_CONTINUE_FLAG;
}

error_t GetKmcAbortCondEval(_SCTPARAM)
{
    if(GetGeneralAbortCondEval(SCT) != MC_SIM_CONTINUE_FLAG)
    {
        return MC_SIM_ABORT_FLAG;
    }
    return MC_SIM_CONTINUE_FLAG;
}

error_t GetMmcAbortCondEval(_SCTPARAM)
{
    if(GetGeneralAbortCondEval(SCT) != MC_SIM_CONTINUE_FLAG)
    {
        return MC_SIM_ABORT_FLAG;
    }
    return MC_SIM_CONTINUE_FLAG;
}

error_t SyncSimulationState(_SCTPARAM)
{
    return MC_NO_ERROR;
}

error_t SaveSimulationState(_SCTPARAM)
{
    if(FLAG_IS_SET(RefStateHeaderData(SCT)->Flags, MC_PRE_ROUTINE_FLAG))
    {
        SCT->ErrorCode = SaveWriteBufferToFile(MC_PRE_STATE_FILE, MC_BIN_WRITE_MODE, RefStateBuffer(SCT));
    }
    else
    {
        SCT->ErrorCode = SaveWriteBufferToFile(MC_RUN_STATE_FILE, MC_BIN_WRITE_MODE, RefStateBuffer(SCT));
    }
    return SCT->ErrorCode;
}

static error_t GeneralSimulationFinish(_SCTPARAM)
{
    SET_FLAG(RefStateHeaderData(SCT)->Flags, MC_SIM_COMPLETED_FLAG);
    SCT->ErrorCode = SaveSimulationState(SCT);
    return SCT->ErrorCode;
}

error_t FinishMainRoutineKmc(_SCTPARAM)
{
    if(GeneralSimulationFinish(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. General simulation finisher KMC/MMC returned with an error.")
    }
    return MC_NO_ERROR;
}

error_t FinishMainRoutineMmc(_SCTPARAM)
{    
    if(GeneralSimulationFinish(SCT) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(SCT->ErrorCode, "Fatal error. General simulation finisher KMC/MMC returned with an error.")
    }
    return MC_NO_ERROR;
}

static inline int32_t LookupActJumpId(const _SCTPARAM)
{
        return *MDA_GET_3(*RefJmpAssignTable(SCT), RefPathEnvAt(SCT, 0)->PosVector.d, RefPathEnvAt(SCT, 0)->ParId, RefActRollInfo(SCT)->RelId);
}

static inline void SetActJumpDirAndCol(_SCTPARAM)
{
    RefActRollInfo(SCT)->JmpId = LookupActJumpId(SCT);
    SCT->CycleState.ActJumpDir = RefJumpDirAt(SCT, RefActRollInfo(SCT)->JmpId);
    SCT->CycleState.ActJumpCol = RefJumpColAt(SCT, RefActJumpDir(SCT)->ColId);
}

static inline void SetActPathStartEnv(_SCTPARAM)
{
    RefActPathArray(SCT)[0] = RefLatticeEnvAt(SCT, RefActRollInfo(SCT)->EnvId);
    SetCodeByteAt(RefActStateCode(SCT), 0, RefPathEnvAt(SCT, 0)->ParId);
}

static inline void SetActCounterCol(_SCTPARAM)
{
    SCT->CycleState.ActCntCol = RefStateCountersAt(SCT, RefPathEnvAt(SCT, 0)->ParId);
}

void SetKmcJumpSelection(_SCTPARAM)
{
    RollNextKmcSelect(SCT);
    SCT->CycleState.ActStateCode = 0ULL;

    SetActCounterCol(SCT);
    SetActPathStartEnv(SCT);
    SetActJumpDirAndCol(SCT);
}

void SetKmcJumpPathProperties(_SCTPARAM)
{
    vector4_t actVector = RefPathEnvAt(SCT, 0)->PosVector;
    byte_t index = 0;

    FOR_EACH(vector4_t, relVector, RefActJumpDir(SCT)->JumpSeq)
    {
        AddToLhsAndTrimVector4(&actVector, relVector, RefLatticeSize(SCT));
        RefActPathArray(SCT)[index] = GetEnvByVector4(&actVector, RefEnvLattice(SCT));
        SetCodeByteAt(RefActStateCode(SCT), index, RefPathEnvAt(SCT, index)->ParId);
    }
}

static inline occode_t GetLastPossibleJumpCode(const _SCTPARAM)
{
    return RefActJumpCol(SCT)->JumpRules.End[-1].StCode0;
}

static inline void FindAndSetActJumpRuleLinear(_SCTPARAM)
{
    if(GetLastPossibleJumpCode(SCT) < GetActStateCode(SCT))
    {
        SCT->CycleState.ActJumpRule = NULL;
    }
    else
    {
        SCT->CycleState.ActJumpRule = RefActJumpCol(SCT)->JumpRules.Start;
        while(RefActJumpRule(SCT)->StCode0 < GetActStateCode(SCT))
        {
            SCT->CycleState.ActJumpRule++;
        }
        if(RefActJumpRule(SCT)->StCode0 != GetActStateCode(SCT))
        {
            SCT->CycleState.ActJumpRule = NULL;
        }
    }
}

static inline void FindAndSetActJumpRuleBinary(_SCTPARAM)
{
    
}

bool_t GetKmcJumpRuleEval(_SCTPARAM)
{
    FindAndSetActJumpRuleLinear(SCT);
    return RefActJumpRule(SCT) == NULL;
}

void SetKmcJumpEvalFail(_SCTPARAM)
{
    RefActCounters(SCT)->BlockCnt++;
}

void SetKmcJumpProperties(_SCTPARAM)
{
    SetState0And1EnergiesKmc(SCT);
    CreateLocalJumpDeltaKmc(SCT);
    SetState2EnergyKmc(SCT);
}

void SetKmcJumpProbsDefault(_SCTPARAM)
{
    RefActEngInfo(SCT)->ConfDel = 0.5 * (RefActEngInfo(SCT)->Eng2 - RefActEngInfo(SCT)->Eng0);
    RefActEngInfo(SCT)->Prob0to2 = exp(RefActEngInfo(SCT)->Eng1 + RefActEngInfo(SCT)->ConfDel);
    RefActEngInfo(SCT)->Prob2to0 = (RefActEngInfo(SCT)->ConfDel > RefActEngInfo(SCT)->Eng1) ? INFINITY : 0.0;
}

void SetKmcJumpEvaluation(_SCTPARAM)
{
    SCT->Plugins.OnSetJumpProbs(SCT);

    if(RefActEngInfo(SCT)->Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCT)->ToUnstCnt++;
        RollbackLocalJumpDeltaKmc(SCT);
        return;
    }
    if(RefActEngInfo(SCT)->Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCT)->OnUnstCnt++;
        RollbackLocalJumpDeltaKmc(SCT);
        return;
    }   
    if(Pcg32NextDouble(&SCT->RnGen) < RefActEngInfo(SCT)->Prob0to2)
    {
        RefActCounters(SCT)->StepCnt++;
        CreateFullStateDeltaKmc(SCT);
        return;
    }
    else
    {
        RefActCounters(SCT)->RejectCnt++;
        RollbackLocalJumpDeltaKmc(SCT);
    }
}


void SetMmcJumpSelection(_SCTPARAM)
{
    RollNextMmcSelect(SCT);

    SetActCounterCol(SCT);
    SetActPathStartEnv(SCT);
    SetActJumpDirAndCol(SCT);
}

void SetMmcJumpPathProperties(_SCTPARAM)
{
    RefActPathArray(SCT)[2] = RefLatticeEnvAt(SCT, RefActRollInfo(SCT)->OffId);
    RefActPathArray(SCT)[1] = RefLatticeEnvAt(SCT, 0);

    RefActPathArray(SCT)[1] += RefEnvLattice(SCT)->Header->Blocks[0] * RefPathEnvAt(SCT, 2)->PosVector.a;
    RefActPathArray(SCT)[1] += RefEnvLattice(SCT)->Header->Blocks[1] * RefPathEnvAt(SCT, 2)->PosVector.b;
    RefActPathArray(SCT)[1] += RefEnvLattice(SCT)->Header->Blocks[2] * RefPathEnvAt(SCT, 2)->PosVector.c;
    RefActPathArray(SCT)[1] += RefPathEnvAt(SCT, 0)->PosVector.d;

    SetCodeByteAt(RefActStateCode(SCT), 1, RefPathEnvAt(SCT, 1)->ParId);
}

void SetMmcJumpEvalFail(_SCTPARAM)
{
    RefActCounters(SCT)->BlockCnt++;
}

bool_t GetMmcJumpRuleEval(_SCTPARAM)
{
    FindAndSetActJumpRuleLinear(SCT);
    return SCT->CycleState.ActJumpRule == 0;
}

void SetMmcJumpProperties(_SCTPARAM)
{
    SetState0And1EnergiesMmc(SCT);
    CreateLocalJumpDeltaMmc(SCT);
    SetState2EnergyMmc(SCT);
}

void SetMmcJumpProbsDefault(_SCTPARAM)
{
    RefActEngInfo(SCT)->Prob0to2 = exp(RefActEngInfo(SCT)->Eng2 - RefActEngInfo(SCT)->Eng0);
}

void SetMmcJumpEvaluation(_SCTPARAM)
{
    SCT->Plugins.OnSetJumpProbs(SCT);

    if(RefActEngInfo(SCT)->Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCT)->ToUnstCnt++;
        RollbackLocalJumpDeltaMmc(SCT);
        return;
    }
    if(RefActEngInfo(SCT)->Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCT)->OnUnstCnt++;
        RollbackLocalJumpDeltaMmc(SCT);
        return;
    }   
    if(Pcg32NextDouble(&SCT->RnGen) < RefActEngInfo(SCT)->Prob0to2)
    {
        RefActCounters(SCT)->StepCnt++;
        CreateFullStateDeltaMmc(SCT);
        return;
    }
    else
    {
        RefActCounters(SCT)->RejectCnt++;
        RollbackLocalJumpDeltaMmc(SCT);
    }
}
