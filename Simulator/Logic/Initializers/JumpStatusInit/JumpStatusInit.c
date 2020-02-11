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
static void AllocateJumpStatusArray(SCONTEXT_PARAM)
{
    let cellSizes = getLatticeSizeVector(SCONTEXT);
    let jumpCountPerCell = (int32_t) span_Length(*getJumpDirections(SCONTEXT));
    JumpStatusArray_t statusArray = new_Array(statusArray,cellSizes->A,cellSizes->B,cellSizes->C,jumpCountPerCell);

    *getJumpStatusArray(SCONTEXT) = statusArray;
}

// Populates the jump path with the passed jump status vector and jump direction to prepare for the jump link search
static error_t PrepareJumpPathForLinkSearch(SCONTEXT_PARAM, const Vector4_t*restrict jumpStatusVector, const JumpDirection_t*restrict jumpDirection)
{
    return_if(jumpStatusVector->D != jumpDirection->ObjectId, ERR_ARGUMENT);

    let latticeSizes = getLatticeSizeVector(SCONTEXT);
    JUMPPATH[0] = getEnvironmentStateByIds(SCONTEXT, jumpStatusVector->A, jumpStatusVector->B, jumpStatusVector->C, jumpDirection->PositionId);
    for (int32_t i = 1; i < jumpDirection->JumpLength; i++)
    {
        let relVector = &span_Get(jumpDirection->JumpSequence, i-1);
        let targetVector = AddAndTrimVector4(&JUMPPATH[0]->PositionVector, relVector, latticeSizes);
        JUMPPATH[i] = getEnvironmentStateByVector4(SCONTEXT, &targetVector);
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
static error_t BufferJumpLinksOfJumpPath(SCONTEXT_PARAM, const int32_t jumpLength, int32_t *restrict outCount, JumpLink_t *restrict outBuffer)
{
    return_if((jumpLength < JUMPS_JUMPLENGTH_MIN)||(jumpLength > JUMPS_JUMPLENGTH_MAX), ERR_ARGUMENT);

    int32_t linkId;
    *outCount = 0;

    for (int32_t receiverPathId = 0; receiverPathId < jumpLength; ++receiverPathId)
    {
        continue_if(!JUMPPATH[receiverPathId]->IsStable);

        let searchEnvId = getEnvironmentStateIdByPointer(SCONTEXT, JUMPPATH[receiverPathId]);
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

    jumpStatus->JumpLinks = new_Span(jumpStatus->JumpLinks, linkCount);
    for (int32_t i = 0; i < linkCount; ++i)
        span_Get(jumpStatus->JumpLinks, i) = buffer[i];

    return ERR_OK;
}

// Finds the jump links that are required for the jump status that can be accessed by the passed status id vector
static error_t BuildJumpStatusByStatusVector(SCONTEXT_PARAM, const Vector4_t *restrict jumpStatusVector, JumpLink_t *restrict linkBuffer)
{
    error_t error;
    int32_t linkCount = 0;
    var jumpStatus = getJumpStatusByVector4(SCONTEXT, jumpStatusVector);
    let jumpDirection = getJumpDirectionAt(SCONTEXT, jumpStatusVector->D);

    error = PrepareJumpPathForLinkSearch(SCONTEXT, jumpStatusVector, jumpDirection);
    return_if(error, error);

    error = BufferJumpLinksOfJumpPath(SCONTEXT, jumpDirection->JumpLength, &linkCount, linkBuffer);
    return_if(error, error);

    error = ConstructJumpStatusFromLinkBuffer(jumpStatus, linkCount, linkBuffer);

    return error;
}

// Constructs all jump status objects into the allocated jump status collection
static error_t ConstructJumpStatusCollection(SCONTEXT_PARAM)
{
    error_t error;
    JumpLink_t linkSearchBuffer[JUMPS_JUMPLINK_LIMIT];
    let jumpDirectionCount = (int32_t) span_Length(*getJumpDirections(SCONTEXT));
    memset(linkSearchBuffer, 0, sizeof(linkSearchBuffer));

    // Generate jump status for each jump direction in each unit cell
    for (int32_t a = 0; a < getLatticeSizeVector(SCONTEXT)->A; ++a)
    {
        for (int32_t b = 0; b < getLatticeSizeVector(SCONTEXT)->B; ++b)
        {
            for (int32_t c = 0; c < getLatticeSizeVector(SCONTEXT)->C; ++c)
            {
                for (int32_t d = 0; d < jumpDirectionCount; ++d)
                {
                    let jumpStatusVector = (Vector4_t) { .A = a, .B = b, .C = c, .D = d };
                    error = BuildJumpStatusByStatusVector(SCONTEXT, &jumpStatusVector, linkSearchBuffer);
                    return_if(error != ERR_OK, error);
                }
            }
        }
    }

    return error;
}

// Builds the KMC jump status collection on an initialized simulation context
void BuildJumpStatusCollection(SCONTEXT_PARAM)
{
    return_if(JobInfoFlagsAreSet(SCONTEXT, INFO_FLG_MMC));

    error_t error;

    AllocateJumpStatusArray(SCONTEXT);
    error = ConstructJumpStatusCollection(SCONTEXT);
    error_assert(error, "Failure during construction of the jump status collection");
}