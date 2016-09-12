#ifndef _ImgFile_h_
#define _ImgFile_h_

#include <stdio.h>
#include "utype.h"

typedef struct IMAGE_STRUCT {
	U32 format;	// 'RAW', 'JPG', 'TIF', 'BMP', 'PDF', 'PNG'
	U16	option;	// 0
	U8	bit;	// 1:BW, 8:Gray8, 16:Gray16, 24:Color24, 48:Color48
	U8	mono;	// 0:'MONO', 1:'R', 2:'G', 4:'B', 8:'IR', 7:'NTSC'
	struct{U16 x; U16 y;} dpi;
	struct{U32 x; U32 y;} org;
	struct{U32 w; U32 h;} dot;
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
