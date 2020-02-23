//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	JumpStatusInit.c       		//
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Jump status initializer     //
//////////////////////////////////////////

#include "Simulator/Logic/Initializers/JumpStatusInit/JumpStatusInit.h"
#include "Simulator/Logic/Routines/Helper/HelperRoutines.h"

// Allocates the memory for the jump status collection span
static void AllocateJumpStatusArray(SCONTEXT_PARAMETER)
{
    let cellSizes = getLatticeSizeVector(simContext);
    let jumpCountPerCell = (int32_t) span_Length(*getJumpDirections(simContext));
    JumpStatusArray_t statusArray = array_New(statusArray, cellSizes->A, cellSizes->B, cellSizes->C, jumpCountPerCell);

    *getJumpStatusArray(simContext) = statusArray;
}

// Populates the jump path with the passed jump status vector and jump direction to prepare for the jump link search
static error_t PrepareJumpPathForLinkSearch(SCONTEXT_PARAMETER, const Vector4_t*restrict jumpStatusVector, const JumpDirection_t*restrict jumpDirection)
{
    return_if(jumpStatusVector->D != jumpDirection->ObjectId, ERR_ARGUMENT);
    let latticeSizes = getLatticeSizeVector(simContext);

    memset(JUMPPATH, 0, sizeof(JUMPPATH));
    JUMPPATH[0] = getEnvironmentStateByIds(simContext, jumpStatusVector->A, jumpStatusVector->B, jumpStatusVector->C, jumpDirection->PositionId);
    for (int32_t i = 1; i < jumpDirection->JumpLength; i++)
    {
        let relVector = &span_Get(jumpDirection->JumpSequence, i-1);
        let targetVector = AddAndTrimVector4(&JUMPPATH[0]->LatticeVector, relVector, latticeSizes);
        JUMPPATH[i] = getEnvironmentStateByVector4(simContext, &targetVector);
    }

    return ERR_OK;
}

// Tries to find the passed environment id in the passed set of environment links and writes the index of the link to the passed buffer if found
static bool_t TryGetEnvironmentLinkId(const EnvironmentLinks_t*restrict environmentLinks, const int32_t searchEnvId, int32_t*restrict outId)
{
    for (int32_t i = 0; i < span_Length(*environmentLinks); ++i)
    {
        if (span_Get(*environmentLinks, i).TargetEnvironmentId == searchEnvId)
        {
            *outId = i;
            return true;
        }
    }

    return false;
}

// Determines the jump links that the current jump-path has until the provided jump length and writes the result to the passed buffer and counter
static error_t BufferJumpLinksOfJumpPath(SCONTEXT_PARAMETER, const int32_t jumpLength, int32_t *restrict outCount, JumpLink_t *restrict outBuffer)
{
    return_if((jumpLength < JUMPS_JUMPLENGTH_MIN)||(jumpLength > JUMPS_JUMPLENGTH_MAX), ERR_ARGUMENT);

    int32_t linkId;
    *outCount = 0;

    for (int32_t receiverPathId = 0; receiverPathId < jumpLength; ++receiverPathId)
    {
        continue_if(!JUMPPATH[receiverPathId]->IsStable);

        let searchEnvId = getEnvironmentStateIdByPointer(simContext, JUMPPATH[receiverPathId]);
        for (int32_t senderPathId = 0; senderPathId < jumpLength; ++senderPathId)
        {
            continue_if(receiverPathId == senderPathId || !JUMPPATH[senderPathId]->IsStable);
            if (TryGetEnvironmentLinkId(&JUMPPATH[senderPathId]->EnvironmentLinks, searchEnvId, &linkId))
                outBuffer[(*outCount)++] = (JumpLink_t) { .SenderPathId = senderPathId, .LinkId = linkId };
        }
    }

    return ERR_OK;
}

// Constructs the jump status at the provided location from the passed buffer and number of links
static error_t ConstructJumpStatusFromLinkBuffer(JumpStatus_t*restrict jumpStatus, const int32_t linkCount, JumpLink_t*restrict buffer)
{
    return_if(jumpStatus == NULL, ERR_NULLPOINTER);
    return_if(linkCount == 0, ERR_OK);

    jumpStatus->JumpLinks = span_New(jumpStatus->JumpLinks, linkCount);
    for (int32_t i = 0; i < linkCount; ++i)
        span_Get(jumpStatus->JumpLinks, i) = buffer[i];

    return ERR_OK;
}

// Finds the jump links that are required for the jump status that can be accessed by the passed status id vector
static error_t BuildJumpStatusByStatusVector(SCONTEXT_PARAMETER, const Vector4_t *restrict jumpStatusVector, JumpLink_t *restrict linkBuffer)
{
    error_t error;
    int32_t linkCount = 0;
    var jumpStatus = getJumpStatusByVector4(simContext, jumpStatusVector);
    let jumpDirection = getJumpDirectionAt(simContext, jumpStatusVector->D);

    error = PrepareJumpPathForLinkSearch(simContext, jumpStatusVector, jumpDirection);
    return_if(error, error);

    error = BufferJumpLinksOfJumpPath(simContext, jumpDirection->JumpLength, &linkCount, linkBuffer);
    return_if(error, error);

    error = ConstructJumpStatusFromLinkBuffer(jumpStatus, linkCount, linkBuffer);
    return error;
}

// Constructs all jump status objects into the allocated jump status collection
static error_t ConstructJumpStatusCollection(SCONTEXT_PARAMETER)
{
    error_t error = ERR_OK;
    JumpLink_t linkSearchBuffer[JUMPS_JUMPLINK_LIMIT];
    let jumpDirectionCount = (int32_t) span_Length(*getJumpDirections(simContext));
    memset(linkSearchBuffer, 0, sizeof(linkSearchBuffer));

    // Generate jump status for each jump direction in each unit cell
    let latticeSize = getLatticeSizeVector(simContext);
    int64_t totalCount = jumpDirectionCount * latticeSize->A * latticeSize->B * latticeSize->C;
    for (int32_t a = 0; a < latticeSize->A; ++a)
    {
        for (int32_t b = 0; b < latticeSize->B; ++b)
        {
            for (int32_t c = 0; c < latticeSize->C; ++c)
            {
                for (int32_t d = 0; d < jumpDirectionCount; ++d)
                {
                    let jumpStatusVector = (Vector4_t) { .A = a, .B = b, .C = c, .D = d };
                    error = BuildJumpStatusByStatusVector(simContext, &jumpStatusVector, linkSearchBuffer);
                    return_if(error != ERR_OK, error);
                }
            }
        }
    }
    printf("[Init-Info]: Constructed transition status cache [TRANSITION_COUNT=" FORMAT_I64() "]\n", totalCount);
    return error;
}

//  Tries to calculate a constant virtual jump energy correction in [eV] for a jump rule of a jump collection and sets the value on the out parameter
static bool_t TryFindConstVirtualJumpCorrection(SCONTEXT_PARAMETER, JumpCollection_t*restrict jumpCollection, JumpRule_t*restrict jumpRule, double* outValue)
{
    // Note: Due to symmetry it is enough to check the first direction, the others have to be the same by definition
    let jumpDirection = &span_Get(jumpCollection->JumpDirections, 0);
    let jumpStatusVector = (Vector4_t) {.A = 0, .B = 0, .C = 0, .D = jumpDirection->ObjectId};
    let jumpStatus = getJumpStatusByVector4(simContext, &jumpStatusVector);
    PrepareJumpPathForLinkSearch(simContext, &jumpStatusVector, jumpDirection);
    var energy = 0.0;

    cpp_foreach(jumpLink, jumpStatus->JumpLinks)
    {
        let environmentLink = getEnvLinkByJumpLink(simContext, jumpLink);
        return_if(span_Length(environmentLink->ClusterLinks) != 0, false); // Todo: Check if the cluster changes actually affect energy

        // Determine which path id is the actual target
        let environment = getEnvironmentStateAt(simContext, environmentLink->TargetEnvironmentId)->EnvironmentDefinition;
        var receiverId = 0;
        c_foreach(item, JUMPPATH)
        {
            if (getEnvironmentStateIdByPointer(simContext, *item) == environmentLink->TargetEnvironmentId) break;
            ++receiverId;
        }
        if (receiverId >= JUMPS_JUMPLENGTH_MAX)
        {
            SIMERROR = ERR_DATACONSISTENCY;
            return false;
        }

        // Get the pair contribution from the table of the receiver
        let pairInteraction = span_Get(environment->PairInteractions, environmentLink->TargetPairId);
        let pairTable = getPairEnergyTableAt(simContext, pairInteraction.EnergyTableId);
        let receiverParticleId = GetOccupationCodeByteAt(&jumpRule->StateCode0, receiverId);
        let oldSenderParticle = GetOccupationCodeByteAt(&jumpRule->StateCode0, jumpLink->SenderPathId);
        let newSenderParticle = GetOccupationCodeByteAt(&jumpRule->StateCode2, jumpLink->SenderPathId);
        energy += getPairEnergyAt(pairTable, receiverParticleId, newSenderParticle) - getPairEnergyAt(pairTable, receiverParticleId, oldSenderParticle);
    }
    *outValue = -energy;
    return *outValue != NAN;
}

//  Compares the jump status collection with available jumprules and sets static correction values where possible
static error_t AssignPossibleStaticCorrectionValuesToRules(SCONTEXT_PARAMETER)
{
    var jumpCollections = getJumpCollections(simContext);
    var physicalFactors = getPhysicalFactors(simContext);
    cpp_foreach(jumpCollection, *jumpCollections)
    {
        cpp_foreach(jumpRule, jumpCollection->JumpRules)
        {
            double energy;
            let isConst = TryFindConstVirtualJumpCorrection(simContext, jumpCollection, jumpRule, &energy);
            return_if(SIMERROR != ERR_OK, ERR_DATACONSISTENCY);
            jumpRule->StaticVirtualJumpEnergyCorrection = isConst ? energy : NAN;
            if (isConst)
            {
                printf("[Init-Info]: Optimized transition event [TRANSITION_ID=%i, STATE_ENCODING="FORMAT_I64()"-"FORMAT_I64()"-"FORMAT_I64()"] => Path bias S0->S2 is context independent [%f eV].\n",
                       jumpCollection->ObjectId, jumpRule->StateCode0.Value, jumpRule->StateCode1.Value, jumpRule->StateCode2.Value, energy * physicalFactors->EnergyFactorKtToEv);
            }
            else
            {
                printf("[Init-Info]: Cannot optimize transition event [TRANSITION_ID=%i, STATE_ENCODING="FORMAT_I64()"-"FORMAT_I64()"-"FORMAT_I64()"] => Path bias S0->S2 is context dependent. \n",
                       jumpCollection->ObjectId, jumpRule->StateCode0.Value, jumpRule->StateCode1.Value, jumpRule->StateCode2.Value);
            }
        }
    }

    return ERR_OK;
}

void BuildJumpStatusCollection(SCONTEXT_PARAMETER)
{
    return_if(JobInfoFlagsAreSet(simContext, INFO_FLG_MMC));

    error_t error;

    AllocateJumpStatusArray(simContext);
    error = ConstructJumpStatusCollection(simContext);
    assert_success(error, "Fatal error during construction of the jump status collection.");

    error = AssignPossibleStaticCorrectionValuesToRules(simContext);
    assert_success(error, "Fatal error during attempt to determine virtual jump bias optimization.");
}
