//////////////////////////////////////////
// Project: C Monte Carlo Simulator		//
// File:	DbModel.h        		    //
// Author:	Sebastian Eisele			//
//			Workgroup Martin, IPC       //
//			RWTH Aachen University      //
//			© 2018 Sebastian Eisele     //
// Short:   Db model data types         //
//////////////////////////////////////////

#pragma once
#include "Framework/Errors/McErrors.h"
#include "Framework/Basic/BaseTypes/BaseTypes.h"
#include "Framework/Math/Types/Vector.h"

#define __DBMODEL_PAR db_model_t* restrict dbModel

typedef int64_t occode_t;

DEFINE_MD_ARRAY(eng_table_t, double, 2);

DEFINE_MD_ARRAY(lattice_t, byte_t, 4);

DEFINE_MD_ARRAY(eng_background_t, double, 5);

DEFINE_MD_ARRAY(jump_counts_t, int32_t, 2);

DEFINE_MD_ARRAY(jump_assign_t, int32_t, 3);

typedef struct { int32_t Key; int32_t HeaderSize, BlobSize; byte_t* Buffer; } db_blob_t;

typedef struct { int32_t TrcId; vector4_t Vector; } move_t;

typedef struct { vector4_t RelVector; int32_t TabId; } pair_def_t;

typedef struct { int16_t RelPosIds[8]; int32_t TabId; } clu_def_t;

typedef struct { int32_t Count; pair_def_t * Start, * End; } pair_defs_t;

typedef struct { int32_t Count; clu_def_t * Start, * End; } clu_defs_t;

typedef struct { int32_t ObjId; byte_t PosParIds[64], UptParIds[64]; pair_defs_t PairDefs; clu_defs_t CluDefs; } env_def_t;

typedef struct { int32_t Count; env_def_t * Start, * End; } env_defs_t;

typedef struct { int32_t Count; occode_t* Start, * End; } occ_codes_t;

typedef struct { int32_t ObjId; eng_table_t EngTable; } pair_table_t;

typedef struct { int32_t ObjId; occ_codes_t OccCodes; byte_t ParToTableId[64]; eng_table_t EngTable; } clu_table_t;

typedef struct { int32_t Count; pair_table_t * Start, * End; } pair_tables_t;

typedef struct { int32_t Count; clu_table_t * Start, * End; } clu_tables_t;

typedef struct { byte_t Count; vector4_t * Start, * End; } jump_seq_t;

typedef struct { byte_t PathId; int32_t LinkId; } jump_link_t;

typedef struct { byte_t Count; jump_link_t * Start, * End; } jump_links_t;

typedef struct { byte_t Count; move_t * Start, * End; } move_seq_t;

typedef struct { int32_t ObjId, PosId, ColId; byte_t JumpLength; double FieldProj; jump_seq_t JumpSeq; jump_links_t JumpLinkSeq; move_seq_t LocMoveSeq, GloMoveSeq; } jump_dir_t;

typedef struct { int32_t Count; jump_dir_t* Start, * End; } jump_dirs_t;

typedef struct { occode_t StCode0, StCode1, StCode2; double FrqFactor, FieldFac; byte_t TrcOrderCode[8]; } jump_rule_t;

typedef struct { int32_t Count; jump_rule_t* Start, * End; } jump_rules_t;

typedef struct { int32_t ObjId; bitmask_t ParMask; jump_dirs_t JumpDirs; jump_rules_t JumpRules; } jump_col_t;

typedef struct { int32_t Count; jump_col_t * Start, * End; } jump_cols_t;

typedef struct { vector4_t SizeVec; int32_t MobilesCount, SelectableCount; lattice_t Lattice; eng_background_t EngBackground; } lat_info_t;

typedef struct { int32_t CellTrcCount, GloTrcCount; id_redirect_t PosIdToCellTrcOffset; env_defs_t EnvDefs; } str_model_t;

typedef struct { pair_tables_t PairTables; clu_tables_t CluTables; } eng_model_t;

typedef struct { jump_cols_t JumpCols; jump_dirs_t JumpDirs; jump_counts_t JumpCountTable; jump_assign_t JumpAssignTable; } tra_model_t;

typedef struct { bitmask_t JobFlg, StaFlg; int64_t StateSize, TargetMcsp, TimeLimit; double Temp, MinSuccRate; void * JobHeader; } job_info_t;

typedef struct { bitmask_t JobFlg; double AbortTol; int32_t AbortSeqLen, AbortSmplLen, AbortSmplInt; } mmc_header_t;

typedef struct { bitmask_t JobFlg; int32_t DynTrcCount; double FieldMgn, BaseFrq, FixNormFac; } kmc_header_t;

typedef struct
{
    lat_info_t LattInfo;
    job_info_t JobInfo;
    str_model_t Structure;
    eng_model_t Energy;
    tra_model_t Transition;
} db_model_t;