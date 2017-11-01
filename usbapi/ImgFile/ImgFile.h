#ifndef _ImgFile_h_
#define _ImgFile_h_

#include <stdio.h>
#include "utype.h"


// image format ------------
#define IMG_FMT_RAW I3('RAW')
#define IMG_FMT_JPG I3('JPG')
#define IMG_FMT_TIF I3('TIF')
#define IMG_FMT_BMP I3('BMP')

// image bit ---------------
#define IMG_1_BIT	1
#define IMG_8_BIT	8
#define IMG_16_BIT	16
#define IMG_24_BIT	24
#define IMG_48_BIT	48

// mono source -------------
#define IMG_COLOR			0
#define IMG_MONO_R			1
#define IMG_MONO_G			2
#define IMG_MONO_B			3
#define IMG_1CH_TRUE_MONO   4
#define IMG_3CH_TRUE_MONO   5
#define IMG_MONO_BW			6
#define IMG_MONO_MIX		7

// image optione -------------------
#define IMG_OPT_THUMB        0x0200
#define IMG_OPT_PAGE         0x0100
#define IMG_OPT_JPG_FMT444   0x0080
#define IMG_OPT_JPG_QUALITY  0x007f

//document size-------------------
#define DOC_SIZE_FULL		0
#define DOC_SIZE_A4			1
#define DOC_SIZE_LT			2
#define DOC_SIZE_LG14		3
#define DOC_SIZE_LL			4

#define DOC_K_PREFEED		96
#define DOC_K_PRNU			97
#define DOC_S_PRNU			98
#define DOC_FB_LIFE			99

// image size dots
#define IMG_300_DOT_X		2592
#define IMG_300_DOT_Y		3600

#define IMG_A4_300_DOT_X	2480
#define IMG_A4_300_DOT_Y	3512

#define IMG_LT_300_DOT_X	2536//2552
#define IMG_LT_300_DOT_Y	3296

#define IMG_LL_300_DOT_X	2592//2552
#define IMG_LL_300_DOT_Y	32400//10800

#define IMG_FB_LIFE_300_DOT_X		432
#define IMG_FB_LIFE_300_DOT_Y		3512

//For calibration scan PRNU
#define IMG_K_PRNU_300_DOT_X		2592
#define IMG_K_PRUN_300_DOT_Y		48

//For normal scan PRNU 
#define IMG_S_PRNU_300_DOT_X		IMG_K_PRNU_300_DOT_X
#define IMG_S_PRUN_300_DOT_Y		IMG_K_PRUN_300_DOT_Y*5

//For K prefeed 
#define IMG_K_PREFEED_300_DOT_X		IMG_K_PRNU_300_DOT_X
#define IMG_K_PREFEED_300_DOT_Y		4*300

//image size org dots
#define IMG_300_ORG_X		0
#define IMG_300_ORG_Y		0

#define IMG_A4_300_ORG_X		46
#define IMG_A4_300_ORG_Y		0

#define IMG_LT_300_ORG_X		19
#define IMG_LT_300_ORG_Y		0

#define IMG_LL_300_ORG_X		0
#define IMG_LL_300_ORG_Y		0

typedef struct IMAGE_STRUCT {
	U32 format;	// 'RAW', 'JPG', 'TIF', 'BMP', 'PDF', 'PNG'
	U16	option;	// 0
	U8	bit;	// 1:BW, 8:Gray8, 16:Gray16, 24:Color24, 48:Color48
	U8	mono;	// 0:'MONO', 1:'R', 2:'G', 4:'B', 8:'IR', 7:'NTSC'
	struct{U16 x; U16 y;} dpi;
	struct{U32 x; U32 y;} org;
	U32 width;
	U32 height;
} IMAGE_T;

typedef struct IMAGE_FILE_STRUCT {
	IMAGE_T img;
	FILE *stream;
	int row_size;
	int	row;
	int size;
} IMG_FILE_T;

typedef struct IMAGE_MEMORY_STRUCT {
	IMAGE_T img;
	int row_size;
	int row;
	int size;
} IMG_MEM_T;

int ImgFile_Open(IMG_FILE_T *imgfile, char *filename);
int ImgFile_Write(IMG_FILE_T *imgfile, void *data, int size);
int ImgFile_Close(IMG_FILE_T *imgfile, int lines);

int Jpeg_OpenFile(IMG_FILE_T *imgfile, char *filename);
int Jpeg_WriteFile(IMG_FILE_T *imgfile, void *data, int size);
int Jpeg_CloseFile(IMG_FILE_T *imgfile, int lines);

int Tiff_OpenFile(IMG_FILE_T *file, char *FileName);
int Tiff_WriteFile(IMG_FILE_T *file, void *data, int size);
int Tiff_CloseFile(IMG_FILE_T *file);

int Bmp_OpenFile(IMG_FILE_T *file, char *FileName);
int Bmp_WriteFile(IMG_FILE_T *file, void *data, int size);
int Bmp_CloseFile(IMG_FILE_T *file);

int Raw_OpenFile(IMG_FILE_T *file, char *FileName);
int Raw_WriteFile(IMG_FILE_T *file, void *data, int size);
int Raw_CloseFile(IMG_FILE_T *file);

#endif // _ImgFile_h_
