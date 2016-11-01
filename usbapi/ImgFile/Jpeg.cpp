#include "../stdafx.h"
#include "ImgFile.h"

#ifdef WIN32
#pragma pack(1)
#endif



int Jpeg_OpenFile(IMG_FILE_T *imgfile, char *filename)
{
	int result = 0;
	imgfile->stream = fopen(filename,"wb");
	if(imgfile->stream) {
		imgfile->row_size = 0;
		imgfile->row = 0;
		imgfile->size = 0;
	}
	return result;
}

int Jpeg_WriteFile(IMG_FILE_T *imgfile, void *data, int size)
{
	int result = 0;

	if (imgfile->stream)
	{
		result = fwrite(data, 1, size, imgfile->stream);
		if (result) {
			imgfile->size += result;
		}
	}
	
	return result;
}

int Jpeg_CloseFile(IMG_FILE_T *imgfile, int lines)
{
	if (imgfile->stream)
	{
		if (lines >= 8) {
			U8 dpi[5] = { 0x01, 0x01, 0x2c, 0x01, 0x2c };
			U8 height[2];
			dpi[0] = 0x01;
			dpi[1] = (U8)(imgfile->img.dpi.x >> 8);
			dpi[2] = (U8)(imgfile->img.dpi.x);
			dpi[3] = (U8)(imgfile->img.dpi.y >> 8);
			dpi[4] = (U8)imgfile->img.dpi.y;
			lines -= 7;
			height[0] = (U8)(lines >> 8);
			height[1] = (U8)lines;
			fseek(imgfile->stream, 0x0d, SEEK_SET);
			fwrite(dpi, 1, sizeof(dpi), imgfile->stream);
			fseek(imgfile->stream, ((imgfile->img.bit >= 24) ? 0xe6 : 0x64), SEEK_SET);
			fwrite(height, 1, sizeof(height), imgfile->stream);
		}

		fclose(imgfile->stream);
	}
	
	return imgfile->size;
}

#ifdef WIN32
#pragma pack()
#endif
