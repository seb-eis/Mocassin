//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.c        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Main simulation routines    //
//////////////////////////////////////////

#include "Simulator/Logic/Constants/Constants.h"
#include "Simulator/Data/Model/SimContext/ContextAccess.h"
#include "Simulator/Logic/Routines/MainRoutines.h"
#include "Framework/Basic/FileIO/FileIO.h"
#include "Simulator/Logic/Objects/JumpSelection.h"

error_t PrepareForMainRoutine(__SCONTEXT_PAR)
{
    return ERR_NOTIMPLEMENTED;
}

error_t ResetContextAfterPrerun(__SCONTEXT_PAR)
{
    return ERR_NOTIMPLEMENTED;
}

error_t StartMainRoutine(__SCONTEXT_PAR)
{
    RUNTIME_ASSERT(MainStateHasFlags(SCONTEXT, FLG_STATEERROR), SIMERROR, "Cannot start main simulation routine, state error flag is set.")

    if (MainStateHasFlags(SCONTEXT, FLG_KMC))
    {
        if (MainStateHasFlags(SCONTEXT, FLG_PRERUN))
        {
            SIMERROR = StartMainKmcRoutine(SCONTEXT);
            ASSERT_ERROR(SIMERROR, "Prerun execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePrerun(SCONTEXT);
            ASSERT_ERROR(SIMERROR, "Prerun finish of KMC main routine failed.")
        }
        
        return StartMainKmcRoutine(SCONTEXT);
    }

    if (MainStateHasFlags(SCONTEXT, FLG_MMC))
    {
        if (MainStateHasFlags(SCONTEXT, FLG_PRERUN))
        {
            SIMERROR = StartMainMmcRoutine(SCONTEXT);
            ASSERT_ERROR(SIMERROR, "Prerun execution of main KMC routine aborted with an error");

            SIMERROR = FinishRoutinePrerun(SCONTEXT);
            ASSERT_ERROR(SIMERROR, "Prerun finish of KMC main routine failed.")
        }
        
        return StartMainKmcRoutine(SCONTEXT);
    }

    ASSERT_ERROR(ERR_UNKNOWN, "Main routine starter skipped selection process. Neither MMC nor KMC flags is set.");
    return -1; // GCC [-Wall] expects return value, even with exit(..) statement
}

error_t FinishRoutinePrerun(__SCONTEXT_PAR)
{
    SIMERROR = SaveSimulationState(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "State save after prerun completion failed.");

    SIMERROR = ResetContextAfterPrerun(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "Context reset after prerun completion failed.");

    UnSet_MainStateFlags(SCONTEXT, FLG_PRERUN);
    return SIMERROR;
}

error_t StartMainKmcRoutine(__SCONTEXT_PAR)
{
    while (GetKmcAbortCondEval(SCONTEXT) == FLG_CONTINUE)
    {
        SIMERROR = DoNextKmcCycleBlock(SCONTEXT);
        ASSERT_ERROR(SIMERROR, "Simulation abort due to error in KMC cycle block execution.");

        SIMERROR = FinishKmcCycleBlock(SCONTEXT);
        ASSERT_ERROR(SIMERROR, "Simulation abort due to error in KMC cycle block finisher execution.");
    }

    return FinishMainRoutineKmc(SCONTEXT);
}

error_t StartMainMmcRoutine(__SCONTEXT_PAR)
{
    while (GetMmcAbortCondEval(SCONTEXT) == FLG_CONTINUE)
    {
        SIMERROR = DoNextMmcCycleBlock(SCONTEXT);
        ASSERT_ERROR(SIMERROR, "Simulation abort due to error in MMC cycle block execution.");

        SIMERROR = FinishMmcCycleBlock(SCONTEXT);
        ASSERT_ERROR(SIMERROR, "Simulation abort due to error in MMC cycle block finisher execution.");
    }

    return FinishMainRoutineMmc(SCONTEXT);
}

static inline error_t CallOutputPlugin(__SCONTEXT_PAR)
{
    return_if(SCONTEXT->Plugins.OnDataOut == NULL, ERR_OK);
    SCONTEXT->Plugins.OnDataOut(SCONTEXT);
    return SIMERROR;
}

static inline void AdvanceBlockCounters(__SCONTEXT_PAR)
{
    Get_MainCycleCounters(SCONTEXT)->CurTargetMcs += Get_MainCycleCounters(SCONTEXT)->McsPerBlock;
}

error_t DoNextKmcCycleBlock(__SCONTEXT_PAR)
{
    while (Get_MainCycleCounters(SCONTEXT)->CurMcs < Get_MainCycleCounters(SCONTEXT)->CurTargetMcs)
    {
        for (size_t i = 0; i < Get_MainCycleCounters(SCONTEXT)->MinCyclesPerBlock; i++)
        {
            SetKmcJumpSelection(SCONTEXT);
            SetKmcJumpPathProperties(SCONTEXT);

            if (GetKmcJumpRuleEval(SCONTEXT))
            {
                SetKmcJumpProperties(SCONTEXT);
                SetKmcJumpEvaluation(SCONTEXT);
            }
            else
            {
                SetKmcJumpEvalFail(SCONTEXT);
            }
        }
    }
    return SIMERROR;
}

static void SharedCycleBlockFinish(__SCONTEXT_PAR)
{
    UnSet_MainStateFlags(SCONTEXT, FLG_FIRSTCYCLE);

    SIMERROR = SyncSimulationState(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "Simulation aborted due to failed sycnhronization between dynamic model and state object.");

    SIMERROR = SaveSimulationState(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "Simulation aborted due to error during serialization of the state object.");

    SIMERROR = CallOutputPlugin(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "Simulation aborted due to error in the external output plugin.");
}

error_t FinishKmcCycleBlock(__SCONTEXT_PAR)
{
    AdvanceBlockCounters(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

error_t DoNextMmcCycleBlock(__SCONTEXT_PAR)
{
    while (Get_MainCycleCounters(SCONTEXT)->CurMcs < Get_MainCycleCounters(SCONTEXT)->CurTargetMcs)
    {
        for (size_t i = 0; i < Get_MainCycleCounters(SCONTEXT)->MinCyclesPerBlock; i++)
        {
            SetMmcJumpSelection(SCONTEXT);
            SetMmcJumpPathProperties(SCONTEXT);

            if (GetMmcJumpRuleEval(SCONTEXT))
            {
                SetMmcJumpProperties(SCONTEXT);
                SetMmcJumpEvaluation(SCONTEXT);
            }
            else
            {
                SetMmcJumpEvalFail(SCONTEXT);
            }
        }
    }
    return SIMERROR;
}

error_t FinishMmcCycleBlock(__SCONTEXT_PAR)
{
    AdvanceBlockCounters(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

static inline bitmask_t GetTimeoutAbortEval(__SCONTEXT_PAR)
{
    clock_t newClock = clock();
    clock_t lastClock = Get_RuntimeInformation(SCONTEXT)->LastClock;

    Get_MainStateMetaData(SCONTEXT)->TimePerBlock = (newClock - lastClock) / CLOCKS_PER_SEC;
    Get_MainStateMetaData(SCONTEXT)->RunTime += (newClock - lastClock) / CLOCKS_PER_SEC;
    int64_t blockEta = Get_MainStateMetaData(SCONTEXT)->TimePerBlock + Get_MainStateMetaData(SCONTEXT)->RunTime;

    bool_t isTimeout = (Get_MainStateMetaData(SCONTEXT)->RunTime >= Get_JobInformation(SCONTEXT)->TimeLimit) || (blockEta > Get_JobInformation(SCONTEXT)->TimeLimit);
    return (isTimeout) ? FLG_TIMEOUT : FLG_CONTINUE;
}

static inline bool_t GetRateAbortEval(__SCONTEXT_PAR)
{
    Get_MainStateMetaData(SCONTEXT)->SuccessRate = Get_MainCycleCounters(SCONTEXT)->CurMcs / Get_MainStateMetaData(SCONTEXT)->RunTime;
    Get_MainStateMetaData(SCONTEXT)->CyleRate = Get_MainCycleCounters(SCONTEXT)->CurCycles / Get_MainStateMetaData(SCONTEXT)->RunTime;

    if (Get_MainStateMetaData(SCONTEXT)->CyleRate < Get_JobInformation(SCONTEXT)->MinSuccRate)
    {
        return true;
    }
    return false;
}

static inline bool_t GetMcsTargetReachedEval(__SCONTEXT_PAR)
{
    if (Get_MainCycleCounters(SCONTEXT)->CurMcs >= Get_MainCycleCounters(SCONTEXT)->TotTargetMcs)
    {
        return true;
    }
    return false;
}

static error_t GetGeneralAbortCondEval(__SCONTEXT_PAR)
{
    if (MainStateHasFlags(SCONTEXT, FLG_FIRSTCYCLE))
    {
        return FLG_CONTINUE;
    }
    if (GetTimeoutAbortEval(SCONTEXT))
    {
        Set_MainStateFlags(SCONTEXT, FLG_TIMEOUT);
        return FLG_TIMEOUT;
    }
    if (GetRateAbortEval(SCONTEXT))
    {
        Set_MainStateFlags(SCONTEXT, FLG_RATELIMIT);
        return FLG_RATELIMIT;
    }
    if (GetMcsTargetReachedEval(SCONTEXT))
    {
        Set_MainStateFlags(SCONTEXT, FLG_COMPLETED);
        return FLG_COMPLETED;
    }

    return FLG_CONTINUE;
}

error_t GetKmcAbortCondEval(__SCONTEXT_PAR)
{
    if (GetGeneralAbortCondEval(SCONTEXT) != FLG_CONTINUE)
    {
        return FLG_ABORTCONDITION;
    }
    return FLG_CONTINUE;
}

error_t GetMmcAbortCondEval(__SCONTEXT_PAR)
{
    if (GetGeneralAbortCondEval(SCONTEXT) != FLG_CONTINUE)
    {
        return FLG_ABORTCONDITION;
    }
    return FLG_CONTINUE;
}

error_t SyncSimulationState(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t SaveSimulationState(__SCONTEXT_PAR)
{
    if (MainStateHasFlags(SCONTEXT, FLG_PRERUN))
    {
        SIMERROR = SaveWriteBufferToFile(FILE_PRERSTATE, FMODE_BINARY_W, Get_MainStateBuffer(SCONTEXT));
    }
    else
    {
        SIMERROR = SaveWriteBufferToFile(FILE_MAINSTATE, FMODE_BINARY_W, Get_MainStateBuffer(SCONTEXT));
    }
    return SIMERROR;
}

static error_t GeneralSimulationFinish(__SCONTEXT_PAR)
{
    Set_MainStateFlags(SCONTEXT, FLG_COMPLETED);
    SIMERROR = SaveSimulationState(SCONTEXT);
    return SIMERROR;
}

error_t FinishMainRoutineKmc(__SCONTEXT_PAR)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "Simulation aborted due to error in general simulation finisher routine exceution.");
    return SIMERROR;
}

error_t FinishMainRoutineMmc(__SCONTEXT_PAR)
{
    SIMERROR = GeneralSimulationFinish(SCONTEXT);
    ASSERT_ERROR(SIMERROR, "Simulation aborted due to error in general simulation finisher routine exceution.");
    return SIMERROR;
}

static inline int32_t LookupActJumpId(__SCONTEXT_PAR)
{
    return *MDA_GET_3(*Get_JumpIdToPositionsAssignmentTable(SCONTEXT), JUMPPATH[0]->PosVector.d, JUMPPATH[0]->ParId, Get_JumpSelectionInfo(SCONTEXT)->RelId);
}

static inline void SetActJumpDirAndCol(__SCONTEXT_PAR)
{
    Get_JumpSelectionInfo(SCONTEXT)->JmpId = LookupActJumpId(SCONTEXT);
    Get_CycleState(SCONTEXT)->ActJumpDir = Get_JumpDirectionById(SCONTEXT, Get_JumpSelectionInfo(SCONTEXT)->JmpId);
    Get_CycleState(SCONTEXT)->ActJumpCol = Get_JumpCollectionById(SCONTEXT, Get_ActiveJumpDirection(SCONTEXT)->ColId);
}

static inline void SetActPathStartEnv(__SCONTEXT_PAR)
{
    JUMPPATH[0] = Get_EnvironmentStateById(SCONTEXT, Get_JumpSelectionInfo(SCONTEXT)->EnvId);
    SetCodeByteAt(&Get_CycleState(SCONTEXT)->ActStateCode, 0, JUMPPATH[0]->ParId);
    JUMPPATH[0]->PathId = 0;
}

static inline void SetActCounterCol(__SCONTEXT_PAR)
{
    Get_CycleState(SCONTEXT)->ActCntCol = Get_MainStateCounterById(SCONTEXT, JUMPPATH[0]->ParId);
}

void SetKmcJumpSelection(__SCONTEXT_PAR)
{
    RollNextKmcSelect(SCONTEXT);
    Get_CycleState(SCONTEXT)->ActStateCode = 0ULL;

    SetActCounterCol(SCONTEXT);
    SetActPathStartEnv(SCONTEXT);
    SetActJumpDirAndCol(SCONTEXT);
}

void SetKmcJumpPathProperties(__SCONTEXT_PAR)
{
    vector4_t actVector = JUMPPATH[0]->PosVector;
    byte_t index = 0;

    FOR_EACH(vector4_t, relVector, Get_ActiveJumpDirection(SCONTEXT)->JumpSeq)
    {
        AddToLhsAndTrimVector4(&actVector, relVector, Get_LatticeSizeVector(SCONTEXT));
        JUMPPATH[index] = Get_EnvironmentStateByVector4(SCONTEXT, &actVector);

        SetCodeByteAt(&Get_CycleState(SCONTEXT)->ActStateCode, index, JUMPPATH[index]->ParId);
        JUMPPATH[index]->PathId = index;
    }
}

static inline occode_t GetLastPossibleJumpCode(__SCONTEXT_PAR)
{
    return Get_ActiveJumpCollection(SCONTEXT)->JumpRules.End[-1].StCode0;
}

static inline void LinearJumpRuleLookup(__SCONTEXT_PAR)
{
    if (GetLastPossibleJumpCode(SCONTEXT) < Get_ActiveStateCode(SCONTEXT))
    {
        Get_CycleState(SCONTEXT)->ActJumpRule = NULL;
    }
    else
    {
        Get_CycleState(SCONTEXT)->ActJumpRule = Get_ActiveJumpCollection(SCONTEXT)->JumpRules.Start;
        while (Get_ActiveJumpRule(SCONTEXT)->StCode0 < Get_ActiveStateCode(SCONTEXT))
        {
            Get_CycleState(SCONTEXT)->ActJumpRule++;
        }
        if (Get_ActiveJumpRule(SCONTEXT)->StCode0 != Get_ActiveStateCode(SCONTEXT))
        {
            Get_CycleState(SCONTEXT)->ActJumpRule = NULL;
        }
    }
}

static inline void BinaryJumpRuleLookup(__SCONTEXT_PAR)
{
    // Possible implementation on optimization
}

static inline void LookupAndSetActJumpRule(__SCONTEXT_PAR)
{
    LinearJumpRuleLookup(SCONTEXT);
}

bool_t GetKmcJumpRuleEval(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return Get_ActiveJumpRule(SCONTEXT) == NULL;
}

void SetKmcJumpEvalFail(__SCONTEXT_PAR)
{
    Get_ActiveCounters(SCONTEXT)->BlockCnt++;
}

void SetKmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesKmc(SCONTEXT);
}

void SetKmcJumpProbsDefault(__SCONTEXT_PAR)
{
    eng_info_t* energyInfo = Get_JumpEnergyInfo(SCONTEXT);

    energyInfo->ConfDel = 0.5 * (energyInfo->Eng2 - energyInfo->Eng0);
    energyInfo->Prob0to2 = exp(energyInfo->Eng1 + energyInfo->ConfDel);
    energyInfo->Prob2to0 = (energyInfo->ConfDel > energyInfo->Eng1) ? INFINITY : 0.0;
}

void SetKmcJumpEvaluation(__SCONTEXT_PAR)
{
    SCONTEXT->Plugins.OnSetJumpProbs(SCONTEXT);

    if (Get_JumpEnergyInfo(SCONTEXT)->Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        Get_ActiveCounters(SCONTEXT)->ToUnstCnt++;
        return;
    }
    if (Get_JumpEnergyInfo(SCONTEXT)->Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        Get_ActiveCounters(SCONTEXT)->OnUnstCnt++;
        return;
    }
    if (GetNextCompareDouble(SCONTEXT) < Get_JumpEnergyInfo(SCONTEXT)->Prob0to2)
    {
        Get_ActiveCounters(SCONTEXT)->StepCnt++;
        AdvanceKmcSystemToState2(SCONTEXT);
        MakeJumpPoolUpdateKmc(SCONTEXT);
        return;
    }
    else
    {
        Get_ActiveCounters(SCONTEXT)->RejectCnt++;
    }
}

void SetMmcJumpSelection(__SCONTEXT_PAR)
{
    RollNextMmcSelect(SCONTEXT);

    SetActCounterCol(SCONTEXT);
    SetActPathStartEnv(SCONTEXT);
    SetActJumpDirAndCol(SCONTEXT);
}

void SetMmcJumpPathProperties(__SCONTEXT_PAR)
{
    JUMPPATH[2] = Get_EnvironmentStateById(SCONTEXT, Get_JumpSelectionInfo(SCONTEXT)->OffId);
    JUMPPATH[1] = Get_EnvironmentStateById(SCONTEXT, 0);

    JUMPPATH[1] += Get_EnvironmentLattice(SCONTEXT)->Header->Blocks[0] * JUMPPATH[2]->PosVector.a;
    JUMPPATH[1] += Get_EnvironmentLattice(SCONTEXT)->Header->Blocks[1] * JUMPPATH[2]->PosVector.b;
    JUMPPATH[1] += Get_EnvironmentLattice(SCONTEXT)->Header->Blocks[2] * JUMPPATH[2]->PosVector.c;
    JUMPPATH[1] += JUMPPATH[0]->PosVector.d;

    SetCodeByteAt(&Get_CycleState(SCONTEXT)->ActStateCode, 1, JUMPPATH[1]->ParId);
    JUMPPATH[1]->PathId = 1;
}

void SetMmcJumpEvalFail(__SCONTEXT_PAR)
{
    Get_ActiveCounters(SCONTEXT)->BlockCnt++;
}

bool_t GetMmcJumpRuleEval(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return Get_CycleState(SCONTEXT)->ActJumpRule == 0;
}

void SetMmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesMmc(SCONTEXT);
}

void SetMmcJumpProbsDefault(__SCONTEXT_PAR)
{
    Get_JumpEnergyInfo(SCONTEXT)->Prob0to2 = exp(Get_JumpEnergyInfo(SCONTEXT)->Eng2 - Get_JumpEnergyInfo(SCONTEXT)->Eng0);
}

void SetMmcJumpEvaluation(__SCONTEXT_PAR)
{
    SCONTEXT->Plugins.OnSetJumpProbs(SCONTEXT);

    if (Get_JumpEnergyInfo(SCONTEXT)->Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        Get_ActiveCounters(SCONTEXT)->ToUnstCnt++;
        return;
    }
    if (Get_JumpEnergyInfo(SCONTEXT)->Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        Get_ActiveCounters(SCONTEXT)->OnUnstCnt++;
        return;
    }
    if (GetNextCompareDouble(SCONTEXT) < Get_JumpEnergyInfo(SCONTEXT)->Prob0to2)
    {
        Get_ActiveCounters(SCONTEXT)->StepCnt++;
        AdvanceMmcSystemToState2(SCONTEXT);
        MakeJumpPoolUpdateMmc(SCONTEXT);
        return;
    }
    else
    {
        Get_ActiveCounters(SCONTEXT)->RejectCnt++;
    }
}
