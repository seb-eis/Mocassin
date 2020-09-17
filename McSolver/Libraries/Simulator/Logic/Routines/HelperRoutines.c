//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	EnvRoutines.h        		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Set of helper routines      //
//////////////////////////////////////////

#include "HelperRoutines.h"

bool_t CheckPairEnergyTableIsConstant(SCONTEXT_PARAMETER, const PairTable_t *table)
{
    return_if(span_Length(table->EnergyTable) == 0, true);
    let compValue = span_Get(table->EnergyTable, 0);
    cpp_offset_foreach(value,table->EnergyTable, 1)
    {
        let isSignificantDelta = fabs(compValue - *value) > MC_CONST_FLP_TOLERANCE;
        return_if (isSignificantDelta, false);
    }

    return true;
}

bool_t CheckClusterEnergyTableIsConstant(SCONTEXT_PARAMETER, const ClusterTable_t *table)
{
    return_if(span_Length(table->EnergyTable) == 0, true);
    let compValue = span_Get(table->EnergyTable, 0);
    cpp_offset_foreach(value,table->EnergyTable, 1)
        return_if (fabs(compValue - *value) > MC_CONST_FLP_TOLERANCE, false);

    return true;
}

bool_t CheckPairInteractionIsLinkIrrelevantByIndex(SCONTEXT_PARAMETER, const EnvironmentDefinition_t *restrict environment, const int32_t pairId)
{
    let pairInteraction = span_Get(environment->PairInteractions, pairId);
    let pairTable = getPairEnergyTableAt(simContext, pairInteraction.EnergyTableId);
    let isFixedTable = flagsAreTrue(pairTable->Padding, ENERGY_FLG_CONST_TABLE);
    return_if(!isFixedTable, false);
    return_if(isFixedTable && span_Length(environment->ClusterInteractions) == 0, true);
    cpp_foreach(clusterInteraction, environment->ClusterInteractions)
    {
        bool_t isDependent = false;
        c_foreach(value, clusterInteraction->PairInteractionIds)
        {
            isDependent |= (bool_t) (*value == pairId);
            break_if(isDependent);
        }
        let clusterTable = getClusterEnergyTableAt(simContext, clusterInteraction->EnergyTableId);
        let isFixedClusterTable = flagsAreTrue(clusterTable->Padding, ENERGY_FLG_CONST_TABLE);
        return_if(!isFixedClusterTable, false);
    }
    return true;
}