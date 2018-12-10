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
static void AllocateJumpStatusArray(__SCONTEXT_PAR)
{
    Vector4_t* cellSizes = getLatticeSizeVector(SCONTEXT);
    size_t numOfJumpsPerCell = span_GetSize(*getJumpCollections(SCONTEXT));
    JumpStatusArray_t statusArray = new_Array(statusArray,cellSizes->a,cellSizes->b,cellSizes->c,numOfJumpsPerCell);

    *getJumpStatusArray(SCONTEXT) = statusArray;
}

// Populates the jump path with the passed jump status vector and jump direction to prepare for the jump link search
static error_t PrepareJumpPathForLinkSearch(__SCONTEXT_PAR, const Vector4_t*restrict jumpStatusVector, const JumpDirection_t*restrict jumpDirection)
{
    return_if(jumpStatusVector->d != jumpDirection->ObjectId, ERR_ARGUMENT);

    JUMPPATH[0] = getEnvironmentStateByIds(SCONTEXT, jumpStatusVector->a, jumpStatusVector->b, jumpStatusVector->c, jumpDirection->PositionId);
    for (int32_t i = 0; i < jumpDirection->JumpLength; ++i)
    {
        Vector4_t targetVector = AddAndTrimVector4(&JUMPPATH[0]->PositionVector, &span_Get(jumpDirection->JumpSequence, i), getLatticeSizeVector(SCONTEXT));
        JUMPPATH[i] = getEnvironmentStateByVector4(SCONTEXT, &targetVector);
    }

    return ERR_OK;
}

// Tries to find the passed environment id in the passed set of environment links and writes the index of the link to the passed buffer if found
static bool_t TryGetEnvironmentLinkId(const EnvironmentLinks_t*restrict environmentLinks, const int32_t searchEnvId, int32_t*restrict outId)
{
    for (int32_t i = 0; i < span_GetSize(*environmentLinks); ++i)
    {
        if (span_Get(*environmentLinks, i).EnvironmentId == searchEnvId)
        {
            *outId = i;
            return true;
        }
    }

    return false;
}

// Determines the jump links that the current jump-path has until the provided jump length and writes the result to the passed buffer and counter
static error_t BufferJumpLinksOfJumppath(__SCONTEXT_PAR, const int32_t jumpLength, int32_t* restrict outCount, JumpLink_t*restrict outBuffer)
{
    return_if((jumpLength < JUMPS_JUMPLENGTH_MIN)||(jumpLength > JUMPS_JUMPLENGTH_MAX), ERR_ARGUMENT);

    *outCount = 0;
    for (int32_t receiverPathId = 0; receiverPathId < jumpLength; ++receiverPathId)
    {
        int32_t searchEnvId = JUMPPATH[receiverPathId]->EnvironmentId;
        for (int32_t senderPathId = 0; senderPathId < jumpLength; ++senderPathId)
        {
            continue_if(receiverPathId == senderPathId);
            int32_t idBuffer;
            if (TryGetEnvironmentLinkId(&JUMPPATH[senderPathId]->EnvironmentLinks, searchEnvId, &idBuffer))
            {
                outBuffer[(*outCount)++] = (JumpLink_t) { .PathId = receiverPathId, .LinkId = idBuffer };
            }
        }
    }

    return ERR_OK;
}

// Constructs the jump status at the provided location from the passed buffer and number of links
static error_t ConstructJumpStatusFromLinkBuffer(JumpStatus_t*restrict jumpStatus, const int32_t numOfLinks, JumpLink_t*restrict buffer)
{
    return_if(jumpStatus == NULL, ERR_NULLPOINTER);
    return_if(numOfLinks == 0, ERR_OK);

    jumpStatus->JumpLinks = new_Span(jumpStatus->JumpLinks, numOfLinks);
    for (int32_t i = 0; i < numOfLinks; ++i)
    {
        span_Get(jumpStatus->JumpLinks, i) = buffer[i];
    }

    return ERR_OK;
}

// Finds the jump links that are required for the jump status that can be accessed by the passed status id vector
static error_t BuildJumpStatusByStatusVector(__SCONTEXT_PAR, const Vector4_t *restrict jumpStatusVector, JumpLink_t *restrict linkBuffer)
{
    error_t error;
    int32_t numOfLinks = 0;
    JumpStatus_t* jumpStatus = getJumpStatusByVector4(SCONTEXT, jumpStatusVector);
    JumpDirection_t* jumpDirection = getJumpDirectionById(SCONTEXT, jumpStatusVector->d);

    error = PrepareJumpPathForLinkSearch(SCONTEXT, jumpStatusVector, jumpDirection);
    return_if(error != ERR_OK, error);

    error = BufferJumpLinksOfJumppath(SCONTEXT, jumpDirection->JumpLength, &numOfLinks, linkBuffer);
    return_if(error != ERR_OK, error);

    error = ConstructJumpStatusFromLinkBuffer(jumpStatus, numOfLinks, linkBuffer);

    return error;
}

// Constructs all jump status objects into the allocated jump status collection
static error_t ConstructJumpStatusCollection(__SCONTEXT_PAR)
{
    error_t error;
    JumpLink_t linkSearchBuffer[JUMPS_JUMPLINK_LIMIT];
    Vector4_t jumpStatusVector = (Vector4_t) { 0, 0, 0, 0 };
    int32_t numOfJumpDirections = span_GetSize(*getJumpDirections(SCONTEXT));

    // Generate jump status for each jump direction in each unit cell
    for (;jumpStatusVector.a < getLatticeSizeVector(SCONTEXT)->a; ++jumpStatusVector.a)
    {
        for (;jumpStatusVector.b < getLatticeSizeVector(SCONTEXT)->b; ++jumpStatusVector.b)
        {
            for (;jumpStatusVector.c < getLatticeSizeVector(SCONTEXT)->c; ++jumpStatusVector.c)
            {
                for (;jumpStatusVector.c < numOfJumpDirections; ++jumpStatusVector.d)
                {
                    error = BuildJumpStatusByStatusVector(SCONTEXT, &jumpStatusVector, linkSearchBuffer);
                    return_if(error != ERR_OK, error);
                }
            }
        }
    }


    return error;
}

// Builds the jump status collection on an initialized simulation context
void BuildJumpStatusCollection(__SCONTEXT_PAR)
{
    error_t error;

    AllocateJumpStatusArray(SCONTEXT);
    error = ConstructJumpStatusCollection(SCONTEXT);
    error_assert(error, "Failure during construction of the jump status collection");
}