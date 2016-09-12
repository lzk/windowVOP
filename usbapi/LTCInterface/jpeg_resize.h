#ifndef _JPEG_RESIZE_
#define _JPEG_RESIZE_

#include "jpeg.h"
#include "resample.h"

enum JPEG_RESIZE_STATE {
	JPEG_RESIZE_STATE_INIT, 
	JPEG_RESIZE_STATE_NEXT
};

typedef struct _JpegResizeControlBlock 
{
	//info
	int state;
	LtcImageInfo info_in;
	LtcImageInfo info_out;
	int bitPerSample; //24 or 8
	//flow
	JpegDecodeControlBlock jdcb;
	ResampleControlBlock rscb;
	JpegEncodeControlBlock jccb;
	unsigned long outfilesize;
	unsigned long jd_segment_size; //at least 16k
	unsigned long tot_jpeg_input_size;
	unsigned char *seg_raw_buf;
	unsigned long seg_raw_line;
	unsigned long seg_rs_raw_line;
	unsigned long tot_raw_line;
	unsigned long tot_rs_raw_line;
	unsigned char *seg_jpeg_buf;
	unsigned long seg_jpeg_size;
	unsigned long tmp;
	int  flag_decode_done;

}JpegResizeCB;

int jpeg_resize_init(JpegResizeCB* jr);
int jpeg_resize_read(JpegResizeCB* jr, unsigned char** jpeg_out, unsigned long* jpeg_out_size, unsigned char* jpeg_in, unsigned long jpeg_in_size, int in_dpi, int out_dpi, int out_width, int page_line);
int jpeg_resize_free(JpegResizeCB* jr);

typedef struct _ResizeControlBlock 
{
	queue<unsigned char*> rawQueueIn;
	queue<unsigned char*> rawQueueOut;
	ResampleControlBlock rscb;

	unsigned long tot_raw_line;
	unsigned long tot_rs_raw_line;
	unsigned long seg_rs_raw_line;
	unsigned long tmp;
}ResizeCB;

int resize_init(ResizeCB* rcb, int width, int height, int old_width, int old_height, int bitsPerPixel);
int resize_read(ResizeCB* rcb, unsigned char* raw_out, unsigned long* out_height, unsigned char*raw_in, unsigned long in_height);

#endif