#include "..\stdafx.h"
#include "ImgFile.h"
#ifdef WIN32
#pragma pack(1)
#endif



typedef struct TIFF_HEADER
{
	struct{U16 order; U16 version; U32 offset;}		Header;
	U16 EntryCount;
	struct{U16 x0fe; U16 type; U32 n; U32 SubFile;}	NewSubFileType;
	struct{U16 x100; U16 type; U32 n; U32 width;}	ImageWidth;
	struct{U16 x101; U16 type; U32 n; U32 length;}	ImageLength;
	struct{U16 x102; U16 type; U32 n; U32 pBits;}	BitsPerSample;	// ->
	struct{U16 x103; U16 type; U32 n; U32 method;}	Compression;
	struct{U16 x106; U16 type; U32 n; U32 mode;}	PhotoMetric;
	struct{U16 x111; U16 type; U32 n; U32 offset;}	StripOffset;	// ->
	struct{U16 x112; U16 type; U32 n; U32 orient;}	Orientation;
	struct{U16 x115; U16 type; U32 n; U32 samples;}	SamplesPerPixel;
	struct{U16 x116; U16 type; U32 n; U32 rows;}	RowsPerStrip;
	struct{U16 x117; U16 type; U32 n; U32 bytes;}	StripByteCounts;
	struct{U16 x11a; U16 type; U32 n; U32 pXDPI;}	XResolution;	// ->
	struct{U16 x11b; U16 type; U32 n; U32 pYDPI;}	YResolution;	// ->
	struct{U16 x11c; U16 type; U32 n; U32 config;}	PlanarConfig;
	struct{U16 x128; U16 type; U32 n; U32 unit;}	ResolutionUnit;
	U32 NextIFD;
	struct{U16 r; U16 g; U16 b;}					pBitsPerSample;
	struct{U32 fraction; U32 denominator;}			pXResolution;
	struct{U32 fraction; U32 denominator;}			pYResolution;
} TH;
TH th = {
	{'II', 0x2a, 8},			// Header
	15,							// EntryCount
	{0x0fe, 4, 1, 0},			// NewSubFileType
	{0x100, 3, 1, 640},			// ImageWidth
	{0x101, 3, 1, 480},			// ImageLength
	{0x102, 3, 3, (U32)&((TH*)0)->pBitsPerSample}, // BitsPerSample
	{0x103, 3, 1, 1},			// Compression
	{0x106, 3, 0, 2},			// PhotoMetric (RGB)
	{0x111, 3, 1, sizeof(TH)},	// StripOffset
	{0x112, 3, 1, 1},			// Orientation
	{0x115, 3, 1, 3},			// SamplesPerPixel
	{0x116, 3, 1, 480},			// RowsPerStrip
	{0x117, 4, 1, 3*640*480},	// StripByteCounts
	{0x11a, 5, 1, (U32)&((TH*)0)->pXResolution}, // XResolution
	{0x11b, 5, 1, (U32)&((TH*)0)->pYResolution}, // YResolution
	{0x11c, 3, 1, 1},			// PlanarConfig
	{0x128, 3, 1, 2},			// ResolutionUnit (inch)
	0,							// NextIFD
	{8, 8, 8},					// <- BitsPerSample
	{300, 1},					// <- XResolution
	{300, 1},					// <- YResolution
};

int PrepareTiffHeader(IMAGE_T *img)
{
	int RowSize;
	th.ImageWidth.width = img->width;
	th.ImageLength.length = img->height;
	
	if(img->bit <= 16) {
		th.BitsPerSample.n = 1;
		th.BitsPerSample.pBits = img->bit;
		th.SamplesPerPixel.samples = 1;
	}
	else {
		th.BitsPerSample.n = 3;
		th.BitsPerSample.pBits = (U32)&((TH*)0)->pBitsPerSample;
		if(img->bit <= 24)
			th.pBitsPerSample.r = th.pBitsPerSample.g = th.pBitsPerSample.b = 8;
		else
			th.pBitsPerSample.r = th.pBitsPerSample.g = th.pBitsPerSample.b = 16;
		th.SamplesPerPixel.samples = 3;
	}
	
	if(img->bit == 1)
		th.PhotoMetric.mode = 1;
	else if(img->bit <= 16)
		th.PhotoMetric.mode = 1;
	else
		th.PhotoMetric.mode = 2;

	RowSize = (img->width * img->bit + 7) / 8;
	th.RowsPerStrip.rows = img->height;
	th.StripByteCounts.bytes = img->height * RowSize;
	th.pXResolution.fraction = img->dpi.x;
	th.pYResolution.fraction = img->dpi.y;
	return RowSize;
}

int Tiff_OpenFile(IMG_FILE_T *imgfile, char *filename)
{
	int result = 0;
	imgfile->stream = fopen(filename,"wb");
	if(imgfile->stream) {
		imgfile->row = imgfile->size = 0;
		imgfile->row_size = PrepareTiffHeader(&imgfile->img);
		result = fwrite(&th, 1, sizeof(TH), imgfile->stream);
		if(result == 0)
			fclose(imgfile->stream);
	}
	return result;
}

int Raw_OpenFile(IMG_FILE_T *imgfile, char *filename)
{
	int result = 0;
	imgfile->stream = fopen(filename,"wb");
	//if(imgfile->stream) {
		//imgfile->row = imgfile->size = 0;
		//imgfile->row_size = PrepareTiffHeader(&imgfile->img);
		//result = fwrite(&th, 1, sizeof(TH), imgfile->stream);
		//if(result == 0)
		//	fclose(imgfile->stream);
	//}
	return result;
}

int Tiff_WriteFile(IMG_FILE_T *imgfile, void *data, int size)
{
	int result;
	result = fwrite(data, 1, size, imgfile->stream);
	imgfile->size += result;
	imgfile->row = imgfile->size / imgfile->row_size;
	return result;
}

int Raw_WriteFile(IMG_FILE_T *imgfile, void *data, int size)
{
	int result;
	result = fwrite(data, 1, size, imgfile->stream);
	if(result) {
		imgfile->size += result;
	}
	return result;
}

int Tiff_CloseFile(IMG_FILE_T *imgfile)
{
	if(imgfile->row != imgfile->img.height) {
		fseek(imgfile->stream, (U32)&((TH*)0)->ImageLength.length, SEEK_SET);
		fwrite(&imgfile->row, 1, sizeof(U32), imgfile->stream);
		fseek(imgfile->stream, (U32)&((TH*)0)->RowsPerStrip.rows, SEEK_SET);
		fwrite(&imgfile->row, 1, sizeof(U32), imgfile->stream);
		fseek(imgfile->stream, (U32)&((TH*)0)->StripByteCounts.bytes, SEEK_SET);
		fwrite(&imgfile->size, 1, sizeof(U32), imgfile->stream);
	}
	fclose(imgfile->stream);
	return imgfile->size;
}

int Raw_CloseFile(IMG_FILE_T *imgfile)
{
	fclose(imgfile->stream);
	return imgfile->size;
}

#ifdef WIN32
#pragma pack()
#endif
