﻿
#pragma once

#include <stdlib.h>
#include <malloc.h>

#ifdef WIN32
#include <Windows.h>
#else
#endif

#include "hl.h"
#include "hlmodule.h"

typedef void(*real_hl_global_init)(void);

#ifdef _WIN64 or __LP64__
#define X64
#else
#define X86
#endif


#ifdef __cplusplus
#define EXTERNC extern "C"
#else
#define EXTERNC
#endif

typedef unsigned long long hl_value_fat;

typedef void(*hl2cs_callback)(void* table, void* retVal, hl_value_fat* args);

typedef struct
{
	int retType; /* 2: return int64 in x86, 1: return float/double, 0: return ptr/int32/int16/byte/others */
	void* origFunc;
	int enabled;
	int argsCount;
	int argBitMarks; /* bit marks : 1 means the parameter is 8 bytes long, otherwise it is 4 bytes long */
	hl2cs_callback callback;
} hl2c_table;



EXTERNC void* get_ebp();
EXTERNC void* get_esp();

EXTERNC void debug_break();

EXTERNC void* asm_call_bridge_hl_to_cs();

EXTERNC extern void* call_jit_c2hl;
EXTERNC extern void* call_jit_hl2c;
