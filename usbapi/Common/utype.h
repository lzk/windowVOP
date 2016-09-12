//=======================================
//	File: utype.h
//	Note: most Used data type definition
//	Date: 2014.07.01
//=======================================
#ifndef _utype_h_
#define _utype_h_
#ifdef __cplusplus
extern "C" {
#endif//__cplusplus
//=======================================

//#ifndef	UINT8
//#define UINT8	unsigned char
//#endif
//#ifndef UINT16
//#define UINT16	unsigned short
//#endif
//#ifndef UINT32
//#define UINT32	unsigned int
//#endif
//#ifndef	BYTE
//#define BYTE	unsigned char
//#endif
//#ifndef WORD
//#define WORD	unsigned short
//#endif
//#ifndef DWORD
//#define DWORD	unsigned int
//#endif

//----- Data Type Access -------
#ifndef U32
#define U32	unsigned int
#endif
#ifndef U16
#define U16	unsigned short
#endif
#ifndef	U8
#define U8	unsigned char
#endif
#ifndef S32 
#define S32 int 
#endif 
#ifndef S16 
#define S16 short 
#endif 
#ifndef S8 
#define	S8  char 
#endif 
	//------ Char ID/Tag Access ---------
#ifndef I4
#define	I4(i) ((((i) & 0xff)<<24)+(((i) & 0xff00)<<8)+(((i) & 0xff0000)>>8)+(((i) & 0xff000000)>>24))
#endif
#ifndef I3
#define I3(i) ((((i) & 0xff)<<16)+(((i) & 0xff00))+(((i) & 0xff0000)>>16))
#endif
#ifndef I2
#define	I2(i) ((((i) & 0xff)<<8)+(((i) & 0xff00)>>8))
#endif
#ifndef I1
#define	I1(i) ((i) & 0xff)
#endif
//----- Memory Data Access ------------
#ifdef ARM //--------------------------
#ifndef M8
#define M8(addr)	(*(volatile U8*)(addr))
#endif
#ifndef M16
#define M16(addr)	(*(volatile U16*)(addr))
#endif
#ifndef M32
#define M32(addr)	(*(volatile U32*)(addr))
#endif
#else //-------------------------------
#ifndef M8
#define M8(addr)	(*(U8*)(addr))
#endif
#ifndef M16
#define M16(addr)	(*(U16*)(addr))
#endif
#ifndef M32
#define M32(addr)	(*(U32*)(addr))
#endif
#endif //------------------------------
//----- Logical Access-----------------
#ifndef TRUE 
#define TRUE 	1 
#define FALSE 	0 
#endif 
#ifndef OK
#define OK		1 
#define FAIL	0
#endif 
//----- Reference --------
#if 0
// 1. ... ²»¶¨…¢”µ
#ifdef DEBUG
#define debug_printf(str, ...)     do {         printf(str, __VA_ARGS__);     } while (0)
#else
#define debug_printf(str, ...)
#endif
// 2. # ×Ö´® " "
#define print_var(var)     do {         printf("%s: %s\n", #var, var);     } while (0)
// 3. ## ßB½Y
#define print_three_var(var)
do {
	print_var(var);
	print_var(var##2);
	print_var(var##3);
} while (0)
// 4. #@ ×Ö·û ' '
#endif

//=====================================
#ifdef __cplusplus
}
#endif//__cplusplus
#endif //_u_type_
//=====================================
