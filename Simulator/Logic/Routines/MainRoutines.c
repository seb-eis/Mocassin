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

error_t LoadSimulationModel(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t LoadSimulationState(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t ResetContextToDefault(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t PrepareDynamicModel(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t PrepareForMainRoutine(__SCONTEXT_PAR)
{
    return ERR_OK;
}

error_t StartMainRoutine(__SCONTEXT_PAR)
{
    if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_STATEERROR))
    {
        MC_ERROREXIT(SIMERROR, "Simulation error. Simulation routine not started due to set state error flag.");
    }

    if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_KMC))
    {
        if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_PRERUN))
        {
            if (StartMainKmcRoutine(SCONTEXT) != ERR_OK)
            {
                MC_ERROREXIT(SIMERROR, "Fatal error. Prerun execution of main KMC routine returned with an error.");
            }
            if (FinishRoutinePrerun(SCONTEXT) != ERR_OK)
            {
                MC_ERROREXIT(SIMERROR, "Fatal error. Finisher for prerun execution of main KMC routine returned with an error.");
            }
        }
        return StartMainKmcRoutine(SCONTEXT);
    }

    if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_MMC))
    {
        if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_PRERUN))
        {
            if (StartMainKmcRoutine(SCONTEXT) != ERR_OK)
            {
                MC_ERROREXIT(SIMERROR, "Fatal error. Prerun execution of main MMC routine returned with an error.");
            }
            if (FinishRoutinePrerun(SCONTEXT) != ERR_OK)
            {
                MC_ERROREXIT(SIMERROR, "Fatal error. Finisher for prerun execution of main MMC routine returned with an error.");
            }
        }
        return StartMainMmcRoutine(SCONTEXT);
    }
    MC_ERROREXIT(ERR_UNKNOWN, "Unexpected simulation error. No routine selection triggered.");
    return -1;
}

error_t FinishRoutinePrerun(__SCONTEXT_PAR)
{
    if (SaveSimulationState(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Simulation error. State save after prerun completion failed.")
    }

    if (ResetContextToDefault(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Simulation error. Context reset after prerun completion failed.");
    }

    FLG_UNSET(RefStateHeaderData(SCONTEXT)->Flags, FLG_PRERUN);
    return SIMERROR;
}

error_t StartMainKmcRoutine(__SCONTEXT_PAR)
{
    while (GetKmcAbortCondEval(SCONTEXT) == FLG_CONTINUE)
    {
        if (DoNextKmcCycleBlock(SCONTEXT) != ERR_OK)
        {
            MC_ERROREXIT(SIMERROR, "Fatal error. The KMC routine block execution returned with an error.")
        }
        if (FinishKmcCycleBlock(SCONTEXT) != ERR_OK)
        {
            MC_ERROREXIT(SIMERROR, "Fatal error. The finisher for the KMC block routine execution returned with an error.")
        }
    }
    return FinishMainRoutineKmc(SCONTEXT);
}

error_t StartMainMmcRoutine(__SCONTEXT_PAR)
{
    while (GetMmcAbortCondEval(SCONTEXT) == FLG_CONTINUE)
    {
        if (DoNextMmcCycleBlock(SCONTEXT) != ERR_OK)
        {
            MC_ERROREXIT(SIMERROR, "Fatal error. The MMC routine block execution returned with an error.")
        }
        if (FinishMmcCycleBlock(SCONTEXT) != ERR_OK)
        {
            MC_ERROREXIT(SIMERROR, "Fatal error. The finisher for the MMC block routine execution returned with an error.")
        }
    }
    return FinishMainRoutineMmc(SCONTEXT);
}

static inline error_t CallOutputPlugin(__SCONTEXT_PAR)
{
    if (SCONTEXT->Plugins.OnDataOut != NULL)
    {
        SCONTEXT->Plugins.OnDataOut(SCONTEXT);
        return SIMERROR;
    }
    return ERR_OK;
}

static inline void AdvanceBlockCounters(__SCONTEXT_PAR)
{
    Get_MainCycleCounters(SCONTEXT)->CurTargetMcs += Get_MainCycleCounters(SCONTEXT)->McsPerBlock;
}

error_t DoNextKmcCycleBlock(__SCONTEXT_PAR)
{
    while (RefMainCounters(SCONTEXT)->CurMcs < RefMainCounters(SCONTEXT)->CurTargetMcs)
    {
        for (size_t i = 0; i < RefMainCounters(SCONTEXT)->MinCyclesPerBlock; i++)
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
    FLG_UNSET(RefStateHeaderData(SCONTEXT)->Flags, FLG_FIRSTCYCLE);

    if (SyncSimulationState(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Fatal error. Synchronization between simulation data and state object retuned with an error.")
    }

    if (SaveSimulationState(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Fatal error. State save operation after block completion returned with an error.");
    }

    if (CallOutputPlugin(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Fatal error. Loaded ouput plugin call retuned with an error.");
    }
}

error_t FinishKmcCycleBlock(__SCONTEXT_PAR)
{
    AdvanceBlockCounters(SCONTEXT);
    SharedCycleBlockFinish(SCONTEXT);

    return SIMERROR;
}

error_t DoNextMmcCycleBlock(__SCONTEXT_PAR)
{
    while (RefMainCounters(SCONTEXT)->CurMcs < RefMainCounters(SCONTEXT)->CurTargetMcs)
    {
        for (size_t i = 0; i < RefMainCounters(SCONTEXT)->MinCyclesPerBlock; i++)
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

static inline bool_t GetTimeoutAbortEval(__SCONTEXT_PAR)
{
    clock_t newClock = clock();
    RefStateMetaData(SCONTEXT)->TimePerBlock = (newClock - RefModelRunInfo(SCONTEXT)->LastClock) / CLOCKS_PER_SEC;
    RefModelRunInfo(SCONTEXT)->LastClock = newClock;
    RefStateMetaData(SCONTEXT)->RunTime = newClock / CLOCKS_PER_SEC;
    int64_t blockEta = RefStateMetaData(SCONTEXT)->TimePerBlock + RefStateMetaData(SCONTEXT)->RunTime;

    if (RefStateMetaData(SCONTEXT)->RunTime >= RefJobInfo(SCONTEXT)->TimeLimit)
    {
        return FLG_TIMEOUT;
    }
    if (blockEta > RefJobInfo(SCONTEXT)->TimeLimit)
    {
        return FLG_TIMEOUT;
    }
    return FLG_CONTINUE;
}

static inline bool_t GetRateAbortEval(__SCONTEXT_PAR)
{
    RefStateMetaData(SCONTEXT)->SuccessRate = RefMainCounters(SCONTEXT)->CurMcs / RefStateMetaData(SCONTEXT)->RunTime;
    RefStateMetaData(SCONTEXT)->CyleRate = RefMainCounters(SCONTEXT)->CurCycles / RefStateMetaData(SCONTEXT)->RunTime;

    if (RefStateMetaData(SCONTEXT)->CyleRate < RefJobInfo(SCONTEXT)->MinSuccRate)
    {
        return true;
    }
    return false;
}

static inline bool_t GetMcsTargetReachedEval(__SCONTEXT_PAR)
{
    if (RefMainCounters(SCONTEXT)->CurMcs >= RefMainCounters(SCONTEXT)->TotTargetMcs)
    {
        return true;
    }
    return false;
}

static error_t GetGeneralAbortCondEval(__SCONTEXT_PAR)
{
    if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_FIRSTCYCLE))
    {
        return FLG_CONTINUE;
    }

    if (GetTimeoutAbortEval(SCONTEXT))
    {
        FLG_SET(RefStateHeaderData(SCONTEXT)->Flags, FLG_TIMEOUT);
        return FLG_TIMEOUT;
    }

    if (GetRateAbortEval(SCONTEXT))
    {
        FLG_SET(RefStateHeaderData(SCONTEXT)->Flags, FLG_RATELIMIT);
        return FLG_RATELIMIT;
    }

    if (GetMcsTargetReachedEval(SCONTEXT))
    {
        FLG_SET(RefStateHeaderData(SCONTEXT)->Flags, FLG_COMPLETED);
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
    if (FLG_TRUE(RefStateHeaderData(SCONTEXT)->Flags, FLG_PRERUN))
    {
        SIMERROR = SaveWriteBufferToFile(FILE_PRERSTATE, FMODE_BINARY_W, RefStateBuffer(SCONTEXT));
    }
    else
    {
        SIMERROR = SaveWriteBufferToFile(FILE_MAINSTATE, FMODE_BINARY_W, RefStateBuffer(SCONTEXT));
    }
    return SIMERROR;
}

static error_t GeneralSimulationFinish(__SCONTEXT_PAR)
{
    FLG_SET(RefStateHeaderData(SCONTEXT)->Flags, FLG_COMPLETED);
    SIMERROR = SaveSimulationState(SCONTEXT);
    return SIMERROR;
}

error_t FinishMainRoutineKmc(__SCONTEXT_PAR)
{
    if (GeneralSimulationFinish(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Fatal error. General simulation finisher KMC/MMC returned with an error.")
    }
    return ERR_OK;
}

error_t FinishMainRoutineMmc(__SCONTEXT_PAR)
{
    if (GeneralSimulationFinish(SCONTEXT) != ERR_OK)
    {
        MC_ERROREXIT(SIMERROR, "Fatal error. General simulation finisher KMC/MMC returned with an error.")
    }
    return ERR_OK;
}

static inline int32_t LookupActJumpId(const __SCONTEXT_PAR)
{
    return *MDA_GET_3(*RefJmpAssignTable(SCONTEXT), JUMPPATH[0]->PosVector.d, JUMPPATH[0]->ParId, RefActRollInfo(SCONTEXT)->RelId);
}

static inline void SetActJumpDirAndCol(__SCONTEXT_PAR)
{
    RefActRollInfo(SCONTEXT)->JmpId = LookupActJumpId(SCONTEXT);
    SCONTEXT->CycleState.ActJumpDir = RefJumpDirAt(SCONTEXT, RefActRollInfo(SCONTEXT)->JmpId);
    SCONTEXT->CycleState.ActJumpCol = RefJumpColAt(SCONTEXT, RefActJumpDir(SCONTEXT)->ColId);
}

static inline void SetActPathStartEnv(__SCONTEXT_PAR)
{
    JUMPPATH[0] = RefLatticeEnvAt(SCONTEXT, RefActRollInfo(SCONTEXT)->EnvId);
    SetCodeByteAt(RefActStateCode(SCONTEXT), 0, JUMPPATH[0]->ParId);
    JUMPPATH[0]->PathId = 0;
}

static inline void SetActCounterCol(__SCONTEXT_PAR)
{
    SCONTEXT->CycleState.ActCntCol = RefStateCountersAt(SCONTEXT, JUMPPATH[0]->ParId);
}

void SetKmcJumpSelection(__SCONTEXT_PAR)
{
    RollNextKmcSelect(SCONTEXT);
    SCONTEXT->CycleState.ActStateCode = 0ULL;

    SetActCounterCol(SCONTEXT);
    SetActPathStartEnv(SCONTEXT);
    SetActJumpDirAndCol(SCONTEXT);
}

void SetKmcJumpPathProperties(__SCONTEXT_PAR)
{
    vector4_t actVector = JUMPPATH[0]->PosVector;
    byte_t index = 0;

    FOR_EACH(vector4_t, relVector, RefActJumpDir(SCONTEXT)->JumpSeq)
    {
        AddToLhsAndTrimVector4(&actVector, relVector, RefLatticeSize(SCONTEXT));
        JUMPPATH[index] = GetEnvByVector4(&actVector, RefEnvLattice(SCONTEXT));
        SetCodeByteAt(RefActStateCode(SCONTEXT), index, JUMPPATH[index]->ParId);
        JUMPPATH[index]->PathId = index;
    }
}

static inline occode_t GetLastPossibleJumpCode(const __SCONTEXT_PAR)
{
    return RefActJumpCol(SCONTEXT)->JumpRules.End[-1].StCode0;
}

static inline void LinearJumpRuleLookup(__SCONTEXT_PAR)
{
    if (GetLastPossibleJumpCode(SCONTEXT) < GetActStateCode(SCONTEXT))
    {
        SCONTEXT->CycleState.ActJumpRule = NULL;
    }
    else
    {
        SCONTEXT->CycleState.ActJumpRule = RefActJumpCol(SCONTEXT)->JumpRules.Start;
        while (RefActJumpRule(SCONTEXT)->StCode0 < GetActStateCode(SCONTEXT))
        {
            SCONTEXT->CycleState.ActJumpRule++;
        }
        if (RefActJumpRule(SCONTEXT)->StCode0 != GetActStateCode(SCONTEXT))
        {
            SCONTEXT->CycleState.ActJumpRule = NULL;
        }
    }
}

static inline void BinaryJumpRuleLookup(__SCONTEXT_PAR)
{
}

static inline void LookupAndSetActJumpRule(__SCONTEXT_PAR)
{
    LinearJumpRuleLookup(SCONTEXT);
}

bool_t GetKmcJumpRuleEval(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return RefActJumpRule(SCONTEXT) == NULL;
}

void SetKmcJumpEvalFail(__SCONTEXT_PAR)
{
    RefActCounters(SCONTEXT)->BlockCnt++;
}

void SetKmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesKmc(SCONTEXT);
}

void SetKmcJumpProbsDefault(__SCONTEXT_PAR)
{
    RefActEngInfo(SCONTEXT)->ConfDel = 0.5 * (RefActEngInfo(SCONTEXT)->Eng2 - RefActEngInfo(SCONTEXT)->Eng0);
    RefActEngInfo(SCONTEXT)->Prob0to2 = exp(RefActEngInfo(SCONTEXT)->Eng1 + RefActEngInfo(SCONTEXT)->ConfDel);
    RefActEngInfo(SCONTEXT)->Prob2to0 = (RefActEngInfo(SCONTEXT)->ConfDel > RefActEngInfo(SCONTEXT)->Eng1) ? INFINITY : 0.0;
}

void SetKmcJumpEvaluation(__SCONTEXT_PAR)
{
    SCONTEXT->Plugins.OnSetJumpProbs(SCONTEXT);

    if (RefActEngInfo(SCONTEXT)->Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCONTEXT)->ToUnstCnt++;
        return;
    }
    if (RefActEngInfo(SCONTEXT)->Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCONTEXT)->OnUnstCnt++;
        return;
    }
    if (Pcg32NextDouble(&SCONTEXT->RnGen) < RefActEngInfo(SCONTEXT)->Prob0to2)
    {
        RefActCounters(SCONTEXT)->StepCnt++;
        AdvanceKmcSystemToState2(SCONTEXT);
        MakeJumpPoolUpdateKmc(SCONTEXT);
        return;
    }
    else
    {
        RefActCounters(SCONTEXT)->RejectCnt++;
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
    JUMPPATH[2] = RefLatticeEnvAt(SCONTEXT, RefActRollInfo(SCONTEXT)->OffId);
    JUMPPATH[1] = RefLatticeEnvAt(SCONTEXT, 0);

    JUMPPATH[1] += RefEnvLattice(SCONTEXT)->Header->Blocks[0] * JUMPPATH[2]->PosVector.a;
    JUMPPATH[1] += RefEnvLattice(SCONTEXT)->Header->Blocks[1] * JUMPPATH[2]->PosVector.b;
    JUMPPATH[1] += RefEnvLattice(SCONTEXT)->Header->Blocks[2] * JUMPPATH[2]->PosVector.c;
    JUMPPATH[1] += JUMPPATH[0]->PosVector.d;

    SetCodeByteAt(RefActStateCode(SCONTEXT), 1, JUMPPATH[1]->ParId);
    JUMPPATH[1]->PathId = 1;
}

void SetMmcJumpEvalFail(__SCONTEXT_PAR)
{
    RefActCounters(SCONTEXT)->BlockCnt++;
}

bool_t GetMmcJumpRuleEval(__SCONTEXT_PAR)
{
    LookupAndSetActJumpRule(SCONTEXT);
    return SCONTEXT->CycleState.ActJumpRule == 0;
}

void SetMmcJumpProperties(__SCONTEXT_PAR)
{
    SetAllStateEnergiesMmc(SCONTEXT);
}

void SetMmcJumpProbsDefault(__SCONTEXT_PAR)
{
    RefActEngInfo(SCONTEXT)->Prob0to2 = exp(RefActEngInfo(SCONTEXT)->Eng2 - RefActEngInfo(SCONTEXT)->Eng0);
}

void SetMmcJumpEvaluation(__SCONTEXT_PAR)
{
    SCONTEXT->Plugins.OnSetJumpProbs(SCONTEXT);

    if (RefActEngInfo(SCONTEXT)->Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCONTEXT)->ToUnstCnt++;
        return;
    }
    if (RefActEngInfo(SCONTEXT)->Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        RefActCounters(SCONTEXT)->OnUnstCnt++;
        return;
    }
    if (Pcg32NextDouble(&SCONTEXT->RnGen) < RefActEngInfo(SCONTEXT)->Prob0to2)
    {
        RefActCounters(SCONTEXT)->StepCnt++;
        AdvanceMmcSystemToState2(SCONTEXT);
        MakeJumpPoolUpdateMmc(SCONTEXT);
        return;
    }
    else
    {
        RefActCounters(SCONTEXT)->RejectCnt++;
    }
}
