/*	Reference

1. Simple example to read/write jpeg file.
	jpeg-8a/example.c

2. Example to write jpeg file.
	http://www.andrewewhite.net/wordpress/2010/04/07/simple-cc-jpeg-writer-part-2-write-to-buffer-in-memory/

3. Example to write jpeg to memory
	http://stackoverflow.com/questions/1443390/compressing-iplimage-to-jpeg-using-libjpeg-in-opencv
*/

//#define _CRT_SECURE_NO_DEPRECATE //<-- http://stackoverflow.com/questions/14386/fopen-deprecated-warning
//#include <stdio.h>
//#include <iostream>
//#include <exception>
//#include <stdexcept>
//#include <vector>
//#include <setjmp.h>
#include "../StdAfx.h"
#define DB_LINE
using namespace std;
#if 0
int djpeg(LtcImageInfo* info, char* filename_in)
{
	jpeg_decompress_struct cinfo;
	struct jpeg_error_mgr jerr;

	FILE* pFile = fopen(filename_in, "rb");
	if (!pFile)
		return NULL;

	cinfo.err = jpeg_std_error(&jerr);
	jpeg_create_decompress(&cinfo);
	jpeg_stdio_src(&cinfo, pFile);
	jpeg_read_header(&cinfo, TRUE);
	jpeg_start_decompress(&cinfo);

	JOCTET * out_buffer   = new JOCTET[cinfo.image_width * cinfo.image_height * cinfo.num_components];
	info->width = cinfo.image_width;
	info->height = cinfo.image_height;
	info->components = cinfo.num_components;
	info->image_buffer = out_buffer;
	
	while(cinfo.output_scanline < cinfo.image_height)
	{
		JOCTET* p = out_buffer + cinfo.output_scanline*cinfo.image_width*cinfo.num_components;
		jpeg_read_scanlines(&cinfo, &p, 1);
	}

	jpeg_finish_decompress(&cinfo);
	jpeg_destroy_decompress(&cinfo);
	fclose(pFile);
	return 0;
}

int cjpeg(LtcImageInfo* info, char* filename_out)
{
	unsigned char* image = info->image_buffer;
	unsigned int width = info->width;
	unsigned int height = info->height;
	unsigned int components = info->components;

	struct jpeg_compress_struct cinfo;
	struct jpeg_error_mgr jerr;
	
	FILE* outfile = fopen(filename_out, "wb");
 
	cinfo.err = jpeg_std_error(&jerr);
	jpeg_create_compress(&cinfo);
	jpeg_stdio_dest(&cinfo, outfile);
	 
	cinfo.image_width      = width;
	cinfo.image_height     = height;
	cinfo.input_components = components;
	cinfo.in_color_space   = components>1? JCS_RGB: JCS_GRAYSCALE;
	//printf("density %d %d %d\n", cinfo.density_unit, cinfo.X_density, cinfo.Y_density);

	jpeg_set_defaults(&cinfo);
	cinfo.density_unit=1;
	cinfo.X_density=info->x_density;
	cinfo.Y_density=info->y_density;

	jpeg_set_quality (&cinfo, info->quality, true);
	jpeg_start_compress(&cinfo, true);
	JSAMPROW row_pointer;          /* pointer to a single row */
 
	while (cinfo.next_scanline < cinfo.image_height) {
		row_pointer = (JSAMPROW) &image[cinfo.next_scanline * 3 * width];
		jpeg_write_scanlines(&cinfo, &row_pointer, 1);
	}

	jpeg_finish_compress(&cinfo);  
	fclose(outfile);
	jpeg_destroy_compress(&cinfo);

	return 0;
}
#endif
int dmemjpeg(LtcImageInfo* info, unsigned char* inbuffer, unsigned long inlen)
{
	unsigned char** image = &(info->image_buffer);
	unsigned int* width = &(info->width);
	unsigned int* height = &(info->height);
	unsigned int* components = &(info->components);

	jpeg_decompress_struct cinfo;
	struct jpeg_error_mgr jerr;

	cinfo.err = jpeg_std_error(&jerr);
	jpeg_create_decompress(&cinfo);
	jpeg_mem_src(&cinfo, inbuffer, inlen);
	jpeg_read_header(&cinfo, TRUE);
	jpeg_start_decompress(&cinfo);
	
	JOCTET * out_buffer   = new JOCTET[cinfo.image_width * cinfo.image_height * cinfo.num_components];
	*width = cinfo.image_width;
	*height = cinfo.image_height;
	*components = cinfo.num_components;
	*image =  out_buffer;
	//printf("density %d %d %d\n", cinfo.density_unit, cinfo.X_density, cinfo.Y_density);

	while(cinfo.output_scanline < cinfo.image_height)
	{
		JOCTET* p = out_buffer + cinfo.output_scanline*cinfo.image_width*cinfo.num_components;
		jpeg_read_scanlines(&cinfo, &p, 1);
	}

	jpeg_finish_decompress(&cinfo);
	jpeg_destroy_decompress(&cinfo);

	return 0;
}

int cmemjpeg(LtcImageInfo* info, unsigned char**outbuffer, unsigned long*outlen)
{
	unsigned char* image = info->image_buffer;
	unsigned int width = info->width;
	unsigned int height = info->height;
	unsigned int components = info->components;
	
	struct jpeg_compress_struct cinfo;
	struct jpeg_error_mgr jerr;

	*outbuffer = NULL;
	*outlen = 0;
	int row_stride;

	cinfo.err = jpeg_std_error(&jerr);
	jpeg_create_compress(&cinfo);
	jpeg_mem_dest(&cinfo, outbuffer, outlen);
	 
	cinfo.image_width      = width;
	cinfo.image_height     = height;
	cinfo.input_components = components;
	cinfo.in_color_space   = components>1? JCS_RGB: JCS_GRAYSCALE;

	jpeg_set_defaults(&cinfo);
	cinfo.density_unit=1;
	cinfo.X_density=(UINT16)info->x_density;
	cinfo.Y_density=(UINT16)info->y_density;

	jpeg_set_quality (&cinfo, info->quality, true);
	jpeg_start_compress(&cinfo, true);

	JSAMPROW row_pointer;          /* pointer to a single row */
	row_stride = components*width;

	while (cinfo.next_scanline < cinfo.image_height) {
		row_pointer = (JSAMPROW) &image[cinfo.next_scanline * row_stride];
		jpeg_write_scanlines(&cinfo, &row_pointer, 1);
	}
	
	jpeg_finish_compress(&cinfo);  
	jpeg_destroy_compress(&cinfo);

	return 0;
}

/* ------------------------------------------------------------- */
/* MEMORY DESTINATION INTERFACE METHODS */
/* ------------------------------------------------------------- */

LineNode* getOneLineBuffer(LineNode* lastNode, int size)
{
	LineNode *thisNode = (LineNode *)malloc(sizeof(LineNode));
	thisNode->line = (unsigned char*)malloc(size);
	thisNode->next = NULL;

	if(lastNode != NULL) {
		lastNode->next = thisNode;
	}
	return thisNode;
}

void releaseLinesBuffer(LineNode* headNode)
{
	LineNode* lastNode = headNode;
	LineNode* tmp;
	while(lastNode !=NULL) {
		tmp = lastNode;
		lastNode = lastNode->next;

		free(tmp->line);
		free(tmp);
	}
}

/* ------------------------------------------------------------- */
/* MEMORY DESTINATION INTERFACE METHODS */
/* ------------------------------------------------------------- */

/* Read JPEG image from a memory segment */
void liteon_init_source (j_decompress_ptr cinfo) 
{
	cinfo = cinfo;
	DB_LINE
}

boolean liteon_fill_input_buffer (j_decompress_ptr cinfo)
{
	//DB_LINE
	JpegDecodeSrcMgr* src = (JpegDecodeSrcMgr*)cinfo->src;
	//printf("total_avaialbe_jpeg_size %d \n", src->decoder->total_avaialbe_jpeg_size);
	//printf("total_processed_jpeg_size %d \n", src->decoder->total_processed_jpeg_size);

	if(src->decoder->total_avaialbe_jpeg_size <= src->decoder->total_processed_jpeg_size)
		return FALSE;

	src->pub.next_input_byte = src->decoder->in_buffer + src->decoder->total_processed_jpeg_size;
	src->pub.bytes_in_buffer = src->decoder->total_avaialbe_jpeg_size - src->decoder->total_processed_jpeg_size;
    src->decoder->total_processed_jpeg_size = src->decoder->total_avaialbe_jpeg_size;
	//src->decoder->total_avaialbe_jpeg_size=0;
	//printf(" next_input_byte %d  bytes_in_buffer %d\n", src->pub.next_input_byte, src->pub.bytes_in_buffer);
	return TRUE;
}

void liteon_skip_input_data (j_decompress_ptr cinfo, long num_bytes)
{
	struct jpeg_source_mgr* src = (struct jpeg_source_mgr*) cinfo->src;
	//printf("%s num byte %d (%d, %d)\n", __FUNCTION__, num_bytes, src->next_input_byte, src->bytes_in_buffer);
    if (num_bytes > 0) {
        if ((size_t)num_bytes > src->bytes_in_buffer) {
			src->next_input_byte = 0; /* no buffer byte */
			src->bytes_in_buffer = 0; /* no input left */
		} else {
			src->next_input_byte += (size_t)num_bytes;
			src->bytes_in_buffer -= (size_t)num_bytes;
		}	
    }
}

void liteon_term_source (j_decompress_ptr cinfo) 
{
	cinfo = cinfo;
	DB_LINE
}

void liteon_jpeg_mem_src (JpegDecodeControlBlock* jcb, j_decompress_ptr cinfo, void* buffer, long nbytes)
{
	nbytes = nbytes;
	buffer=buffer;
	JpegDecodeSrcMgr* src = 0; 
	//printf("sizeof(JpegDecodeSrcMgr) size %d\n", sizeof(JpegDecodeSrcMgr));
	//src = (JpegDecodeSrcMgr*)malloc(sizeof(JpegDecodeSrcMgr));
	src = new JpegDecodeSrcMgr();
	
	cinfo->src = (jpeg_source_mgr*)src;

	// Set up callback functions.
	src->pub.init_source = liteon_init_source;
	src->pub.fill_input_buffer = liteon_fill_input_buffer;
	src->pub.skip_input_data = liteon_skip_input_data;
	src->pub.resync_to_restart = jpeg_resync_to_restart;
	src->pub.term_source = liteon_term_source;
	src->pub.next_input_byte = NULL;
	src->pub.bytes_in_buffer = 0;
	src->decoder = jcb;
}

int djpegIoInit(LtcImageInfo* info, JpegDecodeControlBlock* jcb, unsigned char* first_jpegbuffer, unsigned long next_size, unsigned char **raw, unsigned long *raw_line, int page_line)
{
	jcb->state = DJPEG_STATE_READ_HEADER;
	jcb->imageInfo = info;

	jcb->cinfo.err = jpeg_std_error(&jcb->jerr);
	jpeg_create_decompress(&jcb->cinfo);
	liteon_jpeg_mem_src(jcb, &jcb->cinfo, NULL, 0);
	jcb->in_buffer = first_jpegbuffer;
	jcb->total_processed_jpeg_size=0;
	jcb->total_avaialbe_jpeg_size=0;
	jcb->total_avaialbe_raw_line=0;
	return djpegIoNext(jcb, next_size, raw, raw_line, 0, page_line);
	//return 0;
}

int djpegIoNext(JpegDecodeControlBlock* jcb, unsigned long next_size, unsigned char **raw, unsigned long *raw_line, int islast, int page_line)
{
//	int line=0;
//	LineNode* lastNode;
	islast = islast;
	int last_output_scanline;
	unsigned int target_decode_line;
	//JOCTET* p;
	//printf("djpegIoNext -s \n");
	//printf("next size %d\n", next_size);
	//JOCTET * raw_buf;
	jcb->total_avaialbe_jpeg_size += next_size;
	

	switch(jcb->state) {
	case DJPEG_STATE_READ_HEADER:
		//printf("jpeg_read_header -s \n");
		if(jpeg_read_header(&jcb->cinfo, TRUE)== JPEG_SUSPENDED){
			//printf("jpeg_read_header JPEG_SUSPENDED \n");
			return 0;
		}
		//printf("jpeg_read_header -e \n");
		jcb->state = DJPEG_STATE_START_DECOMPRESS;
		//printf("DJPEG_STATE_READ_HEADER\n");

	case DJPEG_STATE_START_DECOMPRESS:
		//printf("jpeg_start_decompress -s \n");
		if(jpeg_start_decompress(&jcb->cinfo)== JPEG_SUSPENDED){
			return 0;
		}
		//printf("jpeg_start_decompress -e \n");
		//jcb->out_buffer = new JOCTET[jcb->cinfo.image_width * jcb->cinfo.image_height * jcb->cinfo.num_components];
		//fill info
		jcb->imageInfo->width = jcb->cinfo.image_width;
		jcb->imageInfo->height = jcb->cinfo.image_height;
		jcb->imageInfo->components = jcb->cinfo.num_components;
		jcb->imageInfo->image_buffer =  jcb->out_buffer;

		jcb->state = DJPEG_STATE_READ_SCANLINE;
		//printf("DJPEG_STATE_START_DECOMPRESS\n");
	
	case DJPEG_STATE_READ_SCANLINE:
		//printf("jpeg_read_scanlines -s \n");
		//jcb->current_avaialbe_lines_list = getOneLineBuffer(NULL, jcb->cinfo.image_width*jcb->cinfo.num_components);
		//lastNode = jcb->current_avaialbe_lines_list;
		if(page_line) {
			target_decode_line = page_line+64;
			if(target_decode_line>jcb->cinfo.image_height) {
				target_decode_line = jcb->cinfo.image_height;
			}
		}
		else {
			target_decode_line = jcb->cinfo.image_height;
		}
		while(jcb->cinfo.output_scanline < target_decode_line)
		{
			//JOCTET* p = jcb->out_buffer + jcb->cinfo.output_scanline*jcb->cinfo.image_width*jcb->cinfo.num_components;
			//JOCTET* p = lastNode->line;
			last_output_scanline = jcb->cinfo.output_scanline;
			JOCTET* p = (JOCTET*)malloc(jcb->cinfo.image_width*jcb->cinfo.num_components);
			if(jpeg_read_scanlines(&jcb->cinfo, &p, 1)==JPEG_SUSPENDED) {
				*raw_line = jcb->cinfo.output_scanline - jcb->total_avaialbe_raw_line;
				*raw = jcb->out_buffer + jcb->total_avaialbe_raw_line*jcb->cinfo.image_width*jcb->cinfo.num_components;
				jcb->total_avaialbe_raw_line = jcb->cinfo.output_scanline;
				//put the valid raw line into queue.
				if(last_output_scanline != (int)jcb->cinfo.output_scanline){
					jcb->rawQueue.push(p);
				}
				else {
					free(p);
				}
				//printf("p %d\n", p);
				//printf("total_avaialbe_raw_line %d\n", jcb->cinfo.output_scanline);
				return 0;
			}
			if(last_output_scanline != (int)jcb->cinfo.output_scanline){
				jcb->rawQueue.push(p);
			}
			else {
				free(p);
			}
			//printf("p %d\n", p);
			//lastNode = getOneLineBuffer(lastNode, jcb->cinfo.image_width*jcb->cinfo.num_components);
			//printf("jcb scan_line %d\n", jcb->cinfo.output_scanline);
		}
		//put the valid raw line into queue.
		//printf("jpeg_read_scanlines -e \n");
		*raw_line = jcb->cinfo.output_scanline - jcb->total_avaialbe_raw_line;
		*raw = jcb->out_buffer + jcb->total_avaialbe_raw_line*jcb->cinfo.image_width*jcb->cinfo.num_components;
		jcb->total_avaialbe_raw_line = jcb->cinfo.output_scanline;
		jcb->state = DJPEG_STATE_FINISH;

	//	printf("total_avaialbe_raw_line %d %d\n",jcb->total_avaialbe_raw_line, jcb->cinfo.output_scanline);
		//printf("DJPEG_STATE_READ_SCANLINE\n");

	case DJPEG_STATE_FINISH:
		//printf("DJPEG_STATE_FINISH -s \n");
		//delete jcb->out_buffer;
		//delete jcb->cinfo.src;
		//jcb->out_buffer=NULL;
		if(jcb->cinfo.output_scanline == jcb->cinfo.image_height) {
			//if it is not normal case, this api will have error.
			jpeg_finish_decompress(&jcb->cinfo);
		}
		delete jcb->cinfo.src;
		jpeg_destroy_decompress(&jcb->cinfo);
		jcb->state = DJPEG_STATE_NULL;
		//printf("DJPEG_STATE_FINISH -e \n");
		return 1;

	case DJPEG_STATE_NULL:
		break;;
	}
	return 0;
	//printf("djpegIoNext -e \n");
}


/* ------------------------------------------------------------- */
/* MEMORY DESTINATION INTERFACE METHODS */
/* ------------------------------------------------------------- */

/* This function is called by the library before any data gets written */
void liteon_init_destination (j_compress_ptr cinfo)
{
	cinfo = cinfo;
	DB_LINE
}

boolean liteon_empty_output_buffer (j_compress_ptr cinfo)
{
	cinfo = cinfo;
	DB_LINE
	return FALSE;
}

void liteon_term_destination (j_compress_ptr cinfo)
{
	cinfo = cinfo;
	DB_LINE
}

void liteon_jpeg_memory_dest(j_compress_ptr cinfo, JOCTET* buffer, int bufsize, int* outsize)
{
	JpegEncodeDstMgr* dest = new JpegEncodeDstMgr();
	outsize = outsize;
	cinfo->dest = (jpeg_destination_mgr*)dest;
	/* set method callbacks */
	dest->pub.init_destination = liteon_init_destination;
	dest->pub.empty_output_buffer = liteon_empty_output_buffer;
	dest->pub.term_destination = liteon_term_destination;
	dest->pub.next_output_byte = buffer; /* set destination buffer */
	dest->pub.free_in_buffer = bufsize;
}

void cjpegIoInit(LtcImageInfo* info, JpegEncodeControlBlock* jcb, unsigned char** jpeg_buffer)
{
	DB_LINE
	jcb->in_buffer = info->image_buffer;
	jcb->total_avaialbe_raw_line = 0;
	jcb->total_avaialbe_jpeg_size = 0;
	jcb->total_comsumed_jpeg_size = 0;
	jcb->out_buffer_size = DEST_BUF_SIZE;
	jcb->out_buffer = (JOCTET *)malloc(DEST_BUF_SIZE);
	*jpeg_buffer = jcb->out_buffer;

	jcb->cinfo.err = jpeg_std_error(&jcb->jerr);
	jpeg_create_compress(&jcb->cinfo);
	liteon_jpeg_memory_dest(&jcb->cinfo, jcb->out_buffer, jcb->out_buffer_size, NULL);
	 
	jcb->cinfo.image_width      = info->width;
	jcb->cinfo.image_height     = info->height;
	jcb->cinfo.input_components = info->components;
	jcb->cinfo.in_color_space   = info->components>1? JCS_RGB: JCS_GRAYSCALE;

	jpeg_set_defaults(&jcb->cinfo);
	jcb->cinfo.density_unit=1;
	jcb->cinfo.X_density=(UINT16)info->x_density;
	jcb->cinfo.Y_density=(UINT16)info->y_density;

	jpeg_set_quality (&jcb->cinfo, info->quality, true);
	jpeg_start_compress(&jcb->cinfo, true);
}

int cjpegIoNext(JpegEncodeControlBlock* jcb, unsigned long next_Line, unsigned char **jpeg_buffer, unsigned long *jpeg_size, int is_last)
{
	//DB_LINE
//printf("next_scanline %d nl %d\n", jcb->cinfo.next_scanline, next_Line);
	JSAMPROW row_pointer;          /* pointer to a single row */
	unsigned long target_line = jcb->cinfo.next_scanline + next_Line;
//	int row_stride = jcb->cinfo.input_components*jcb->cinfo.image_width;		
	
	jcb->total_avaialbe_raw_line += next_Line;
	
	
	while(jcb->cinfo.next_scanline < target_line) {		
		//row_pointer = (JSAMPROW) jcb->in_buffer + jcb->cinfo.next_scanline * row_stride;
		//if(jcb->rawQueue.front()
		row_pointer = (JSAMPROW) jcb->rawQueue.front();
		jcb->rawQueue.pop();
		if(row_pointer) {
			jpeg_write_scanlines(&jcb->cinfo, &row_pointer, 1);
			free(row_pointer);
		}
		//printf("%d/%d row_pointer %d \n", jcb->cinfo.next_scanline, target_line, row_pointer);
	}
	//printf("jcb->cinfo.next_scanline %d end %d\n", jcb->cinfo.next_scanline, is_last);
	//printf("%d free %d av %d com %d\n",__LINE__, jcb->cinfo.dest->free_in_buffer, jcb->total_avaialbe_jpeg_size, jcb->total_comsumed_jpeg_size);
	if(is_last && (jcb->cinfo.next_scanline == jcb->cinfo.jpeg_height)) {
		jpeg_finish_compress(&jcb->cinfo); 
	}
#ifdef CJPEG_SUSPENSION_SMALL_OUT_BUFFER
	*jpeg_size = (unsigned long)(jcb->out_buffer_size - (int)jcb->cinfo.dest->free_in_buffer);
	*jpeg_buffer = jcb->out_buffer;
	jcb->total_avaialbe_jpeg_size += (int)(*jpeg_size);
	jcb->total_comsumed_jpeg_size = jcb->total_avaialbe_jpeg_size;

	//reset the buf pointer and free size for next usage.
	jcb->cinfo.dest->next_output_byte = jcb->out_buffer;
	jcb->cinfo.dest->free_in_buffer = jcb->out_buffer_size;
#else 
	jcb->total_avaialbe_jpeg_size = jcb->out_buffer_size - (int)jcb->cinfo.dest->free_in_buffer;
	*jpeg_size = (unsigned long)(jcb->total_avaialbe_jpeg_size - jcb->total_comsumed_jpeg_size);
	*jpeg_buffer = jcb->out_buffer + jcb->total_comsumed_jpeg_size;
	jcb->total_comsumed_jpeg_size = jcb->total_avaialbe_jpeg_size;
#endif
	if(is_last) {
		jpeg_destroy_compress(&jcb->cinfo);
	}

#if 0
	if(is_last) {
		jpeg_finish_compress(&jcb->cinfo); 
		printf("%d free %d av %d com %d\n",__LINE__, jcb->cinfo.dest->free_in_buffer, jcb->total_avaialbe_jpeg_size, jcb->total_comsumed_jpeg_size);
		//delete jcb->cinfo.dest;
		jpeg_destroy_compress(&jcb->cinfo);
	}
	else {
	}
#endif
	return 0;
}



