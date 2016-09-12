//=======================================
//	File: debug.h
//	Note: debug message definition
//	Date: 2014.07.01
//=======================================
#ifndef	_DEBUG_H_
#define	_DEBUG_H_
#ifdef __cplusplus
extern "C" {
#endif//__cplusplus
//=======================================
#include <stdio.h>

#ifdef DEBUG
#   define dprintf(...)     printf(__VA_ARGS__)
#else
#   define dprintf(...)
#endif
//=====================================
#ifdef __cplusplus
}
#endif//__cplusplus
#endif	// _DEBUG_H_
//=====================================
