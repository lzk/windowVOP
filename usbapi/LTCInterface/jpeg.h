#ifndef _JPEG_H_
#define _JPEG_H_
#include <queue>
//#define DB_LINE printf("%s %d\n", __FUNCTION__, __LINE__);
#include "jpeglib.h"
using namespace std;
typedef struct _LtcImageInfo
{
	unsigned int width;
	unsigned int height;
	unsigned int components;	//color 3, gray 1
	unsigned int x_density;	//default is 96 dpi
	unsigned int y_density; //default is 96 dpi
	unsigned int quality;	//default is 75 [0..100]
	unsigned char* image_buffer;
} LtcImageInfo;


int djpeg(LtcImageInfo* info, char* filename_in);

int cjpeg(LtcImageInfo* info, char* filename_out);

int dmemjpeg(LtcImageInfo* info, unsigned char* inbuffer, unsigned long inlen);

int cmemjpeg(LtcImageInfo* info, unsigned char** outbuffer, unsigned long* outlen);

/********************************
	link list for raw lines

********************************/
typedef struct _LineNode
{
	unsigned char*line;
	struct _LineNode* next;
} LineNode;

LineNode* getOneLineBuffer(LineNode* lastNode, int lineSize);
void releaseLinesBuffer(LineNode* node);
/********************************
	source manager

********************************/

enum DJPEG_STATE {
	DJPEG_STATE_READ_HEADER, 
	DJPEG_STATE_START_DECOMPRESS, 
	DJPEG_STATE_READ_SCANLINE,
	DJPEG_STATE_FINISH,
	DJPEG_STATE_NULL
};

typedef struct _JpegDecodeControlBlock {
	int state;
	struct jpeg_error_mgr jerr;
	jpeg_decompress_struct cinfo;
	JOCTET * in_buffer; //jpeg in
	JOCTET * out_buffer; //raw out
	int total_processed_jpeg_size;
	int total_avaialbe_jpeg_size;
	int total_avaialbe_raw_line;

	LtcImageInfo* imageInfo;
	LineNode* current_avaialbe_lines_list;
	queue<unsigned char*> rawQueue;
}JpegDecodeControlBlock;

typedef struct _JpegDecodeSrcMgr {
    // public fields; must be first in this struct!
    struct jpeg_source_mgr pub;
    JpegDecodeControlBlock* decoder;
}JpegDecodeSrcMgr;

int djpegIoInit(LtcImageInfo* info, JpegDecodeControlBlock* jcb, unsigned char* first_jpegbuffer, unsigned long next_Line, unsigned char **jpeg_buffer, unsigned long *jpeg_size, int page_line);
int djpegIoNext(JpegDecodeControlBlock* jcb, unsigned long next_size, unsigned char **raw, unsigned long *raw_line, int islast, int page_line);



/********************************
	destination manager

********************************/
#define CJPEG_SUSPENSION_SMALL_OUT_BUFFER

#ifdef CJPEG_SUSPENSION_SMALL_OUT_BUFFER
#define DEST_BUF_SIZE 1024*1024
#else
#define DEST_BUF_SIZE 40*1024*1024 //snow modified
#endif

typedef struct _JpegEncodeControlBlock {
	struct jpeg_error_mgr jerr;
	jpeg_compress_struct cinfo;
	JOCTET * in_buffer; //raw in
	JOCTET * out_buffer; //jpeg out
	int out_buffer_size;
	int total_avaialbe_jpeg_size;
	int total_comsumed_jpeg_size;
	int total_avaialbe_raw_line;

	LineNode* current_avaialbe_raw_lines;
	queue<unsigned char*> rawQueue;
}JpegEncodeControlBlock;

typedef struct _JpegEncodeDstMgr {
    // public fields; must be first in this struct!
    struct jpeg_destination_mgr pub;
    JpegEncodeControlBlock* encoder;
}JpegEncodeDstMgr;

void cjpegIoInit(LtcImageInfo* info, JpegEncodeControlBlock* jcb, unsigned char** rawbuffer);
int cjpegIoNext(JpegEncodeControlBlock* jcb, unsigned long next_Line, unsigned char **jpeg_buffer, unsigned long *jpeg_size, int is_last);


#endif