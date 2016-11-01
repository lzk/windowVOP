#include "../stdafx.h"
#include "ImgFile.h"

#ifdef WIN32
#pragma pack(1)
#endif


typedef struct BMP_HEADER
{
//== Bitmap File Header ==
	U8	ID[2];
	U32 FileSize;
	U32 Reserved;
	U32 BitmapDataOffset;
//== Bitmap Info Header ==
	U32 BitmapHeaderSize;
	U32 Width;
	U32 Height;
	U16 Planes;
	U16 BitsPerPixel;
	U32 Compression;
	U32 BitmapDataSize;
	U32 HResolution;
	U32 VResolution;
	U32 UsedColors;
	U32 ImportantColors;
//== Palette ==
	//U32 Palette[N];
//== Bitmap Array ==
	//U32 BitmapData[N];
} BH;
BH bh = {
//== Bitmap File Header ==
	{'B','M'},		// ID[2]
	0x00000000,		//FileSize;
	0,				//Reserved;
	sizeof(BH),		//BitmapDataOffset;
//== Bitmap Info Header ==
	0x28,			//BitmapHeaderSize;
	640,			//Width;
	-480,			//Height;
	1,				//Planes;
	24,				//BitsPerPixel;
	0,				//Compression;
	0x00000000,		//BitmapDataSize;
	0x00000000,		//H-Resolution;
	0x00000000,		//V-Resolution;
	0x00000000,		//UsedColors;
	0				//ImportantColors
};

U32 Palette[256];	// 1KB
U32 PreparePalette(U32 bit)
{
	U32 UsedColors;
	if(bit == 1) {
		UsedColors = 2;
		Palette[0] = 0;
		Palette[1] = 0x010101 * 0xff;
	}
	else if(bit == 8) {
		U32 i;
		UsedColors = 256;
		for(i = 0; i < UsedColors; i++)
			Palette[i] = 0x010101 * i;
	}
	else
		UsedColors = 0;
	return UsedColors;
}


U32 PrepareBmpHeader(IMAGE_T *img)
{
	int RowSize = ((img->bit * img->dot.w + 31) & ~31) / 8;
	bh.Width = img->dot.w;
	bh.Height = -(int)img->dot.h;
	bh.BitsPerPixel = img->bit;
	bh.BitmapDataSize = RowSize * img->dot.h;
	bh.HResolution = img->dpi.x * 3937 / 100;
	bh.VResolution = img->dpi.y * 3937 / 100;
	bh.UsedColors = PreparePalette(img->bit);
	bh.BitmapDataOffset = sizeof(BH) + bh.UsedColors*4;
	bh.FileSize = bh.BitmapDataOffset + bh.BitmapDataSize;
	return RowSize;
}

int Bmp_OpenFile(IMG_FILE_T *imgfile, char *filename)
{
	int result = 0;
	imgfile->stream = fopen(filename,"wb");
	if(imgfile->stream) {
		imgfile->row = 0;
                imgfile->size = 0;
		imgfile->row_size = PrepareBmpHeader(&imgfile->img);
		result = fwrite(&bh, 1, sizeof(BH), imgfile->stream);
		if(imgfile->img.bit <= 8)
			result += fwrite(Palette, 1, bh.UsedColors*4, imgfile->stream);
		if(result == 0)		
			fclose(imgfile->stream);
	}
	return result;
}

U8 RowData[0x8000];	// 32KB (maximum size of 8.5" 1200dpi 24-bit color line)
int Bmp_WriteFile(IMG_FILE_T *imgfile, void *data, int size)
{
	IMAGE_T *img = &imgfile->img;
	int imgRowSize = (img->bit * img->dot.w + 7) / 8;
	int fileRowSize = imgfile->row_size;
	int rows = size / imgRowSize;
	int i, r, written, result = 0;
	U8 *s, *t;
	if(img->bit == 24) {
		for(r = 0, result = 0; r < rows; r++) {
			for (i = 0, s = (U8*)data + r * imgRowSize, t = RowData; i < (int)img->dot.w; i++, s += 3, t += 3) {
				t[0] = s[2];
				t[1] = s[1];
				t[2] = s[0];
			}
			for(i = 3*img->dot.w; i < fileRowSize; i++)
				RowData[i] = 0;
			written = fwrite(RowData, 1, fileRowSize, imgfile->stream);
			if(written)
				result += written;
			else
				break;
		}
	}
	else { // (ip->bits == 8 || ip->bits == 1)
		if(imgRowSize == fileRowSize) {
			result = fwrite(data, 1, size, imgfile->stream);
		}
		else if(imgRowSize > fileRowSize) {
			for(r = 0, result = 0; r < rows; r++) {
				s = (U8*)data + r * imgRowSize;
				written = fwrite(s, 1, fileRowSize, imgfile->stream);
				if(written)
					result += written;
				else
					break;
			}
		}
		else { // if(imgRowSize < fileRowSize)
			int padSize = fileRowSize - imgRowSize;
			for(i = 0; i < padSize; i++)
				RowData[i] = 0;
			for(r = 0, result = 0; r < rows; r++) {
				s = (U8*)data + r * imgRowSize;
				written = fwrite(s, 1, imgRowSize, imgfile->stream);
				written += fwrite(RowData, 1, padSize, imgfile->stream);
				if(written)
					result += written;
				else
					break;
			}
		}
	}
	imgfile->size += result;
	imgfile->row = imgfile->size / imgfile->row_size;
	return result;
}

int Bmp_CloseFile(IMG_FILE_T *imgfile)
{
	IMAGE_T *img = &imgfile->img;
	if(imgfile->row != img->dot.h) {
		int FileSize = imgfile->size + sizeof(BH);
		int row = -imgfile->row;
		if(img->bit <= 8)
			FileSize += (4 << img->bit);
		fseek(imgfile->stream, (U32)&((BH*)0)->FileSize, SEEK_SET);
		fwrite(&FileSize, 1, sizeof(U32), imgfile->stream);
		fseek(imgfile->stream, (U32)&((BH*)0)->Height, SEEK_SET);
		fwrite(&row, 1, sizeof(U32), imgfile->stream);
		fseek(imgfile->stream, (U32)&((BH*)0)->BitmapDataSize, SEEK_SET);
		fwrite(&imgfile->size, 1, sizeof(U32), imgfile->stream);
	}
	fclose(imgfile->stream);
	return imgfile->size;
}

#ifdef WIN32
#pragma pack()
#endif
