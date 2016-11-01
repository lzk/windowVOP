#include "../stdafx.h"
#include "ImgFile.h"
#ifdef WIN32
#pragma pack(1)
#endif


int ImgFile_Open(IMG_FILE_T *imgfile, char *filename)
{
	int result = FALSE;
	switch(imgfile->img.format) {
		case I3('JPG'):
			result = Jpeg_OpenFile(imgfile, filename);
			break;
		case I3('TIF'):
			result = Tiff_OpenFile(imgfile, filename);
			break;
		case I3('RAW'):
			result = Raw_OpenFile(imgfile, filename);
			break;
		case I3('BMP'):
			result = Bmp_OpenFile(imgfile, filename);
			break;
	}
	return result;
}

int ImgFile_Write(IMG_FILE_T *imgfile, void *data, int size)
{
	int result = FALSE;
	switch(imgfile->img.format) {
		case I3('JPG'):
			result = Jpeg_WriteFile(imgfile, data, size);
			break;
		case I3('TIF'):
			result = Tiff_WriteFile(imgfile, data, size);
			break;
		case I3('RAW'):
			result = Raw_WriteFile(imgfile, data, size);
			break;
		case I3('BMP'):
			result = Bmp_WriteFile(imgfile, data, size);
			break;
	}
	return result;
}

int ImgFile_Close(IMG_FILE_T *imgfile, int lines)
{
	int result = FALSE;
	switch(imgfile->img.format) {
		case I3('JPG'):
			result = Jpeg_CloseFile(imgfile, lines);
			break;
		case I3('TIF'):
			result = Tiff_CloseFile(imgfile);
			break;
		case I3('RAW'):
			result = Raw_CloseFile(imgfile);
			break;
		case I3('BMP'):
			result = Bmp_CloseFile(imgfile);
			break;
	}
	return result;
}

#ifdef WIN32
#pragma pack()
#endif
