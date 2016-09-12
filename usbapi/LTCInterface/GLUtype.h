//=======================================
//	File: utype.h
//	Note: most Used data type definition
//	Date: 2014.07.01
//=======================================
#ifndef _GLutype_h_
#define _GLutype_h_

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
	
#endif