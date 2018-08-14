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

error_t LoadCommandLineArgs(sim_context_t* restrict simContext, int32_t argc, char const* const* argv)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationPlugins(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationModel(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t LoadSimulationState(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t ResetContextToDefault(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareDynamicModel(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t PrepareForMainRoutine(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t StartMainRoutine(sim_context_t* restrict simContext)
{
    if (FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_STATE_ERROR_FLAG))
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Simulation error. Simulation routine not started due to set state error flag.");
    }

    if (FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_KMC_ROUTINE_FLAG))
    {
        if(FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_PRE_ROUTINE_FLAG))
        {
            if(StartMainKmcRoutine(simContext) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. Prerun execution of main KMC routine returned with an error.");
            }
            if(FinishRoutinePrerun(simContext) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. Finisher for prerun execution of main KMC routine returned with an error.");
            }
        }
        return StartMainKmcRoutine(simContext);
    }

    if (FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_MMC_ROUTINE_FLAG))
    {
        if(FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_PRE_ROUTINE_FLAG))
        {
            if(StartMainKmcRoutine(simContext) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. Prerun execution of main MMC routine returned with an error.");
            }
            if(FinishRoutinePrerun(simContext) != MC_NO_ERROR)
            {
                MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. Finisher for prerun execution of main MMC routine returned with an error.");
            }
        }
        return StartMainMmcRoutine(simContext);
    }
    MC_DUMP_ERROR_AND_EXIT(MC_SIM_ERROR, "Unexpected simulation error. No routine selection triggered.");
    return -1;
}

error_t FinishRoutinePrerun(sim_context_t* restrict simContext)
{
    if(SaveSimulationState(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Simulation error. State save after prerun completion failed.")
    }

    if(ResetContextToDefault(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Simulation error. Context reset after prerun completion failed.");
    }

    UNSET_FLAG(simContext->SimState.Header.Data->Flags, MC_PRE_ROUTINE_FLAG);
    return simContext->ErrorCode;
}

error_t StartMainKmcRoutine(sim_context_t* restrict simContext)
{
    while(GetKmcAbortCondEval(simContext) == MC_SIM_CONTINUE_FLAG)
    {
        if(DoNextKmcCycleBlock(simContext) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. The KMC routine block execution returned with an error.")
        }
        if(FinishKmcCycleBlock(simContext) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. The finisher for the KMC block routine execution returned with an error.")
        }
    }
    return FinishMainRoutineKmc(simContext);
}

error_t StartMainMmcRoutine(sim_context_t* restrict simContext)
{
    while(GetMmcAbortCondEval(simContext) == MC_SIM_CONTINUE_FLAG)
    {
        if(DoNextMmcCycleBlock(simContext) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. The MMC routine block execution returned with an error.")
        }
        if(FinishMmcCycleBlock(simContext) != MC_NO_ERROR)
        {
            MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. The finisher for the MMC block routine execution returned with an error.")
        }
    }
    return FinishMainRoutineMmc(simContext);
}

static inline void AdvanceBlockCounters(sim_context_t* restrict simContext)
{
    simContext->CycleState.MainCnts.CurTargetMcs += simContext->CycleState.MainCnts.McsPerBlock;
}

static inline error_t CallOutputPlugin(sim_context_t* restrict simContext)
{
    if(simContext->Plugins.OnDataOut != NULL)
    {
        simContext->Plugins.OnDataOut(simContext);
        return simContext->ErrorCode;
    }
    return MC_NO_ERROR;
}

error_t DoNextKmcCycleBlock(sim_context_t* restrict simContext)
{
    while(simContext->CycleState.MainCnts.CurMcs < simContext->CycleState.MainCnts.CurTargetMcs)
    {
        for(size_t i = 0; i < simContext->CycleState.MainCnts.MinCyclesPerBlock; i++)
        {
            SetKmcJumpSelection(simContext);
            SetKmcJumpPathProperties(simContext);

            if(GetKmcJumpRuleEval(simContext))
            {
                SetKmcJumpProperties(simContext);
                SetKmcJumpEvaluation(simContext);
            }
            else
            {
                SetKmcJumpEvalFail(simContext);
            }
        }
    }
    return simContext->ErrorCode;
}

static void SharedCycleBlockFinish(sim_context_t* restrict simContext)
{
    UNSET_FLAG(simContext->SimState.Header.Data->Flags, MC_SIM_FIRST_CYCLE_FLAG);

    if(SyncSimulationState(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. Synchronization between simulation data and state object retuned with an error.")
    }

    if(SaveSimulationState(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. State save operation after block completion returned with an error.");
    }
    
    if(CallOutputPlugin(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. Loaded ouput plugin call retuned with an error.");
    }
}

error_t FinishKmcCycleBlock(sim_context_t* restrict simContext)
{
    AdvanceBlockCounters(simContext);
    SharedCycleBlockFinish(simContext);

    return simContext->ErrorCode;
}

error_t DoNextMmcCycleBlock(sim_context_t* restrict simContext)
{
    while(simContext->CycleState.MainCnts.CurMcs < simContext->CycleState.MainCnts.CurTargetMcs)
    {
        for(size_t i = 0; i < simContext->CycleState.MainCnts.MinCyclesPerBlock; i++)
        {
            SetMmcJumpSelection(simContext);
            SetMmcJumpPathProperties(simContext);

            if(GetMmcJumpRuleEval(simContext))
            {
                SetMmcJumpProperties(simContext);
                SetMmcJumpEvaluation(simContext);
            }
            else
            {
                SetMmcJumpEvalFail(simContext);
            }
        }
    }
    return simContext->ErrorCode;
}

error_t FinishMmcCycleBlock(sim_context_t* restrict simContext)
{
    AdvanceBlockCounters(simContext);
    SharedCycleBlockFinish(simContext);

    return simContext->ErrorCode;
}

static inline bool_t GetTimeoutAbortEval(sim_context_t* restrict simContext)
{
    clock_t newClock = clock();
    simContext->SimState.Meta.Data->TimePerBlock = (newClock - simContext->SimDynModel.RunInfo.LastClock) / CLOCKS_PER_SEC;
    simContext->SimDynModel.RunInfo.LastClock = newClock;
    simContext->SimState.Meta.Data->RunTime = newClock / CLOCKS_PER_SEC;
    int64_t blockEta = simContext->SimState.Meta.Data->TimePerBlock + simContext->SimState.Meta.Data->RunTime;

    if(simContext->SimState.Meta.Data->RunTime >= simContext->SimDbModel.JobInfo.TimeLimit)
    {
        return MC_SIM_TIME_ABORT_FLAG;
    }
    if(blockEta > simContext->SimDbModel.JobInfo.TimeLimit)
    {
        return MC_SIM_TIME_ABORT_FLAG;
    }
    return MC_SIM_CONTINUE_FLAG;
}

static inline bool_t GetRateAbortEval(sim_context_t* restrict simContext)
{
    simContext->SimState.Meta.Data->SuccessRate = simContext->CycleState.MainCnts.CurMcs / simContext->SimState.Meta.Data->RunTime;
    simContext->SimState.Meta.Data->CyleRate = simContext->CycleState.MainCnts.CurCycles / simContext->SimState.Meta.Data->RunTime;

    if(simContext->SimState.Meta.Data->CyleRate < simContext->SimDbModel.JobInfo.MinSuccRate)
    {
        return true;
    }
    return false;
}

static inline bool_t GetMcsTargetReachedEval(sim_context_t* restrict simContext)
{
    if(simContext->CycleState.MainCnts.CurMcs >= simContext->CycleState.MainCnts.TotTargetMcs)
    {
        return true;
    }
    return false;
}

static error_t GetGeneralAbortCondEval(sim_context_t* restrict simContext)
{
    if(FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_SIM_FIRST_CYCLE_FLAG))
    {
        return MC_SIM_CONTINUE_FLAG;
    }

    if(GetTimeoutAbortEval(simContext))
    {
        SET_FLAG(simContext->SimState.Header.Data->Flags, MC_SIM_TIME_ABORT_FLAG);
        return MC_SIM_TIME_ABORT_FLAG;
    }

    if(GetRateAbortEval(simContext))
    {
        SET_FLAG(simContext->SimState.Header.Data->Flags, MC_SIM_RATE_ABORT_FLAG);
        return MC_SIM_RATE_ABORT_FLAG;
    }

    if(GetMcsTargetReachedEval(simContext))
    {
        SET_FLAG(simContext->SimState.Header.Data->Flags, MC_SIM_COMPLETED_FLAG);
        return MC_SIM_COMPLETED_FLAG;
    }

    return MC_SIM_CONTINUE_FLAG;
}

error_t GetKmcAbortCondEval(sim_context_t* restrict simContext)
{
    if(GetGeneralAbortCondEval(simContext) != MC_SIM_CONTINUE_FLAG)
    {
        return MC_SIM_ABORT_FLAG;
    }
    return MC_SIM_CONTINUE_FLAG;
}

error_t GetMmcAbortCondEval(sim_context_t* restrict simContext)
{
    if(GetGeneralAbortCondEval(simContext) != MC_SIM_CONTINUE_FLAG)
    {
        return MC_SIM_ABORT_FLAG;
    }
    return MC_SIM_CONTINUE_FLAG;
}

error_t SyncSimulationState(sim_context_t* restrict simContext)
{
    return MC_NO_ERROR;
}

error_t SaveSimulationState(sim_context_t* restrict simContext)
{
    if(FLAG_IS_SET(simContext->SimState.Header.Data->Flags, MC_PRE_ROUTINE_FLAG))
    {
        simContext->ErrorCode = SaveWriteBufferToFile(MC_PRE_STATE_FILE, MC_BIN_WRITE_MODE, &simContext->SimState.Buffer);
    }
    else
    {
        simContext->ErrorCode = SaveWriteBufferToFile(MC_RUN_STATE_FILE, MC_BIN_WRITE_MODE, &simContext->SimState.Buffer);
    }
    return simContext->ErrorCode;
}

static error_t GeneralSimulationFinish(sim_context_t* restrict simContext)
{
    SET_FLAG(simContext->SimState.Header.Data->Flags, MC_SIM_COMPLETED_FLAG);
    simContext->ErrorCode = SaveSimulationState(simContext);
    return simContext->ErrorCode;
}

error_t FinishMainRoutineKmc(sim_context_t* restrict simContext)
{
    if(GeneralSimulationFinish(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. General simulation finisher KMC/MMC returned with an error.")
    }
    return MC_NO_ERROR;
}

error_t FinishMainRoutineMmc(sim_context_t* restrict simContext)
{    
    if(GeneralSimulationFinish(simContext) != MC_NO_ERROR)
    {
        MC_DUMP_ERROR_AND_EXIT(simContext->ErrorCode, "Fatal error. General simulation finisher KMC/MMC returned with an error.")
    }
    return MC_NO_ERROR;
}

static inline int32_t GetActJumpId(const sim_context_t* restrict simContext)
{
        return *MDA_GET_3(
            simContext->SimDbModel.Transition.JumpAssignTable,
            simContext->CycleState.ActPathEnvs[0]->PosVector.d,
            simContext->CycleState.ActPathEnvs[0]->ParId,
            simContext->CycleState.ActRollInfo.RelId);
}

static inline void SetActJumpDirAndCol(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActRollInfo.JmpId = GetActJumpId(simContext);
    simContext->CycleState.ActJumpDir = &simContext->SimDbModel.Transition.JumpDirs.Start[simContext->CycleState.ActRollInfo.JmpId];
    simContext->CycleState.ActJumpCol = &simContext->SimDbModel.Transition.JumpCols.Start[simContext->CycleState.ActJumpDir->ColId];
}

static inline void SetActPathStartEnv(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActPathEnvs[0] = &simContext->SimDynModel.EnvLattice.Start[simContext->CycleState.ActRollInfo.EnvId];
    MARSHAL_AS(byte_t, &simContext->CycleState.ActStateCode)[0] = simContext->CycleState.ActPathEnvs[0]->ParId;
}

static inline void SetActCounterCol(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActCntCol = &simContext->SimState.Counters.Start[simContext->CycleState.ActPathEnvs[0]->ParId];
}

void SetKmcJumpSelection(sim_context_t* restrict simContext)
{
    RollNextKmcSelect(simContext);
    simContext->CycleState.ActStateCode = 0ULL;

    SetActCounterCol(simContext);
    SetActPathStartEnv(simContext);
    SetActJumpDirAndCol(simContext);
}

void SetKmcJumpPathProperties(sim_context_t* restrict simContext)
{
    vector4_t actVector = simContext->CycleState.ActPathEnvs[0]->PosVector;
    size_t index = 0;

    FOR_EACH(vector4_t, relVector, simContext->CycleState.ActJumpDir->JumpSeq)
    {
        AddToLhsAndTrimVector4(&actVector, relVector, &simContext->SimDbModel.LattInfo.SizeVec);
        simContext->CycleState.ActPathEnvs[index] = GetEnvByVector4(&actVector, &simContext->SimDynModel.EnvLattice);
        MARSHAL_AS(byte_t, &simContext->CycleState.ActStateCode)[index] = simContext->CycleState.ActPathEnvs[index]->ParId;
    }
}

static inline void FindAndSetActJumpRuleLinear(sim_context_t* restrict simContext)
{
    if(simContext->CycleState.ActJumpCol->JumpRules.End[-1].StCode0 < simContext->CycleState.ActStateCode)
    {
        simContext->CycleState.ActJumpRule = NULL;
    }
    else
    {
        simContext->CycleState.ActJumpRule = simContext->CycleState.ActJumpCol->JumpRules.Start;
        while(simContext->CycleState.ActJumpRule->StCode0 < simContext->CycleState.ActStateCode)
        {
            simContext->CycleState.ActJumpRule++;
        }
        if(simContext->CycleState.ActJumpRule->StCode0 != simContext->CycleState.ActStateCode)
        {
            simContext->CycleState.ActJumpRule = NULL;
        }
    }
}

static inline void FindAndSetActJumpRuleBinary(sim_context_t* restrict simContext)
{
    
}

bool_t GetKmcJumpRuleEval(sim_context_t* restrict simContext)
{
    FindAndSetActJumpRuleLinear(simContext);
    return simContext->CycleState.ActJumpRule == 0;
}

void SetKmcJumpEvalFail(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActCntCol->BlockCnt++;
}

void SetKmcJumpProperties(sim_context_t* restrict simContext)
{
    SetState0And1EnergiesKmc(simContext);
    CreateLocalJumpDeltaKmc(simContext);
    SetState2EnergyKmc(simContext);
}

void SetKmcJumpProbsDefault(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActEngInfo.ConfDel = 0.5 * (simContext->CycleState.ActEngInfo.Eng2 - simContext->CycleState.ActEngInfo.Eng0);
    simContext->CycleState.ActEngInfo.Prob0to2 = exp(simContext->CycleState.ActEngInfo.Eng1 + simContext->CycleState.ActEngInfo.ConfDel);
    simContext->CycleState.ActEngInfo.Prob2to0 = (simContext->CycleState.ActEngInfo.ConfDel > simContext->CycleState.ActEngInfo.Eng1) ? INFINITY : 0.0;
}

void SetKmcJumpEvaluation(sim_context_t* restrict simContext)
{
    simContext->Plugins.OnSetJumpProbs(simContext);

    if(simContext->CycleState.ActEngInfo.Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        simContext->CycleState.ActCntCol->ToUnstCnt++;
        RollbackLocalJumpDeltaKmc(simContext);
        return;
    }
    if(simContext->CycleState.ActEngInfo.Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        simContext->CycleState.ActCntCol->OnUnstCnt++;
        RollbackLocalJumpDeltaKmc(simContext);
        return;
    }   
    if(Pcg32NextDouble(&simContext->RnGen) < simContext->CycleState.ActEngInfo.Prob0to2)
    {
        simContext->CycleState.ActCntCol->StepCnt++;
        DistributeStateDeltaKmc(simContext);
        return;
    }
    else
    {
        simContext->CycleState.ActCntCol->RejectCnt++;
        RollbackLocalJumpDeltaKmc(simContext);
    }
}


void SetMmcJumpSelection(sim_context_t* restrict simContext)
{
    RollNextMmcSelect(simContext);

    SetActCounterCol(simContext);
    SetActPathStartEnv(simContext);
    SetActJumpDirAndCol(simContext);
}

void SetMmcJumpPathProperties(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActPathEnvs[2] = &simContext->SimDynModel.EnvLattice.Start[simContext->CycleState.ActRollInfo.OffId];
    simContext->CycleState.ActPathEnvs[1] = simContext->SimDynModel.EnvLattice.Start;

    simContext->CycleState.ActPathEnvs[1] += simContext->SimDynModel.EnvLattice.Header->Blocks[0] * simContext->CycleState.ActPathEnvs[2]->PosVector.a;
    simContext->CycleState.ActPathEnvs[1] += simContext->SimDynModel.EnvLattice.Header->Blocks[1] * simContext->CycleState.ActPathEnvs[2]->PosVector.b;
    simContext->CycleState.ActPathEnvs[1] += simContext->SimDynModel.EnvLattice.Header->Blocks[2] * simContext->CycleState.ActPathEnvs[2]->PosVector.c;
    simContext->CycleState.ActPathEnvs[1] += simContext->CycleState.ActPathEnvs[0]->PosVector.d;

    MARSHAL_AS(byte_t, &simContext->CycleState.ActStateCode)[1] = simContext->CycleState.ActPathEnvs[1]->ParId;
}

void SetMmcJumpEvalFail(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActCntCol->BlockCnt++;
}

bool_t GetMmcJumpRuleEval(sim_context_t* restrict simContext)
{
    FindAndSetActJumpRuleLinear(simContext);
    return simContext->CycleState.ActJumpRule == 0;
}

void SetMmcJumpProperties(sim_context_t* restrict simContext)
{
    SetState0And1EnergiesMmc(simContext);
    CreateLocalJumpDeltaMmc(simContext);
    SetState2EnergyMmc(simContext);
}

void SetMmcJumpProbsDefault(sim_context_t* restrict simContext)
{
    simContext->CycleState.ActEngInfo.Prob0to2 = exp(simContext->CycleState.ActEngInfo.Eng2 - simContext->CycleState.ActEngInfo.Eng0);
}

void SetMmcJumpEvaluation(sim_context_t* restrict simContext)
{
    simContext->Plugins.OnSetJumpProbs(simContext);

    if(simContext->CycleState.ActEngInfo.Prob2to0 > MC_CONST_JUMPLIMIT_MAX)
    {
        simContext->CycleState.ActCntCol->ToUnstCnt++;
        RollbackLocalJumpDeltaMmc(simContext);
        return;
    }
    if(simContext->CycleState.ActEngInfo.Prob0to2 > MC_CONST_JUMPLIMIT_MAX)
    {
        simContext->CycleState.ActCntCol->OnUnstCnt++;
        RollbackLocalJumpDeltaMmc(simContext);
        return;
    }   
    if(Pcg32NextDouble(&simContext->RnGen) < simContext->CycleState.ActEngInfo.Prob0to2)
    {
        simContext->CycleState.ActCntCol->StepCnt++;
        DistributeStateDeltaMmc(simContext);
        return;
    }
    else
    {
        simContext->CycleState.ActCntCol->RejectCnt++;
        RollbackLocalJumpDeltaMmc(simContext);
    }
}
