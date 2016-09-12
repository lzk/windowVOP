#include "../StdAfx.h"

int jpeg_resize_init(JpegResizeCB* jr)
{
	jr->state = JPEG_RESIZE_STATE_INIT;

	return 0;
}

int jpeg_resize_read(JpegResizeCB* jr, unsigned char** jpeg_out, unsigned long* jpeg_out_size, unsigned char* jpeg_in, unsigned long jpeg_in_size, int in_dpi, int out_dpi, int out_width, int page_line)
{

	switch(jr->state) {
		case JPEG_RESIZE_STATE_INIT:

			jr->jd_segment_size=jpeg_in_size; //at least 16k
			jr->tot_jpeg_input_size=0;
			jr->seg_raw_buf=0;
			jr->seg_raw_line=0;
			jr->seg_rs_raw_line;
			jr->tot_raw_line=0;
			jr->tot_rs_raw_line=0;
			//unsigned long jc_input_line;
			jr->seg_jpeg_buf;
			jr->seg_jpeg_size=0;
			jr->tmp=0;
			jr->flag_decode_done=0;
			jr->outfilesize = 0;

			//if(jd_segment_size>filesize)
			//	jd_segment_size = filesize;

			jr->flag_decode_done = djpegIoInit(&jr->info_in, &jr->jdcb, jpeg_in, jr->jd_segment_size, &jr->seg_raw_buf, &jr->seg_raw_line, page_line);
//			printf("(%d) %d %d %d\n",__LINE__, jr->jd_segment_size,  jr->seg_raw_buf, jr->seg_raw_line);
			jr->tot_jpeg_input_size += jr->jd_segment_size;
			jr->tot_raw_line += jr->seg_raw_line;

			jr->info_out = jr->info_in;
			jr->info_out.width = out_width;//jr->info_in.width*out_dpi/in_dpi;
			jr->info_out.height = jr->info_in.height*out_dpi/in_dpi;	
			jr->info_out.x_density = out_dpi;
			jr->info_out.y_density = out_dpi;
			jr->info_out.quality = 95;

			jr->bitPerSample = jr->info_out.components*8;
			//info_out.image_buffer = (unsigned char* )malloc(info_out.width * info_out.height * info_out.components);

			//printf("(%d) w%d h%d w%d h%d\n",__LINE__, info_in.width,  info_in.height, info_out.width, info_out.height);
			resamplePartialInit(&jr->rscb, jr->info_out.width, jr->info_out.height, 
				jr->info_out.image_buffer, jr->info_in.width, jr->info_in.height, jr->info_in.image_buffer, jr->bitPerSample, RESAMPLE_NEAREST);
			cjpegIoInit(&jr->info_out, &jr->jccb, jpeg_out);

			jr->tmp = jr->tot_rs_raw_line;
			//printf("(%d) tot_rs%d tot_raw%d\n",__LINE__, tot_rs_raw_line, tot_raw_line);
			resamplePartialRead2(&jr->rscb, &jr->tot_rs_raw_line, jr->tot_raw_line, 0, jr->jdcb.rawQueue, jr->jccb.rawQueue);
			jr->seg_rs_raw_line = jr->tot_rs_raw_line - jr->tmp;

			//printf("(%d) s_rs%d\n",__LINE__, seg_rs_raw_line);
			cjpegIoNext(&jr->jccb, jr->seg_rs_raw_line, &jr->seg_jpeg_buf, &jr->seg_jpeg_size, jr->flag_decode_done);
			//printf("(%d) js%d\n",__LINE__, seg_jpeg_size);
			jr->outfilesize+=jr->seg_jpeg_size;
			jr->state = JPEG_RESIZE_STATE_NEXT;

			*jpeg_out = jr->seg_jpeg_buf;
			*jpeg_out_size = jr->seg_jpeg_size;
//			printf("tot jpeg in %d raw %d resample_raw %d jpeg out %d end %d\n", jr->tot_jpeg_input_size, jr->tot_raw_line, jr->tot_rs_raw_line, jr->outfilesize, jr->flag_decode_done);
			if(jr->flag_decode_done)
				return 1;
			else 
				return 0;

		case JPEG_RESIZE_STATE_NEXT:

			jr->flag_decode_done = djpegIoNext(&jr->jdcb, jr->jd_segment_size, &jr->seg_raw_buf, &jr->seg_raw_line, 0, page_line);
			jr->tot_jpeg_input_size += jr->jd_segment_size;
			jr->tot_raw_line += jr->seg_raw_line;

			jr->tmp = jr->tot_rs_raw_line;
			//printf("(%d) tot_rs%d tot_raw%d\n",__LINE__, tot_rs_raw_line, tot_raw_line);
			resamplePartialRead2(&jr->rscb, &jr->tot_rs_raw_line, jr->tot_raw_line, 0, jr->jdcb.rawQueue, jr->jccb.rawQueue);
			jr->seg_rs_raw_line = jr->tot_rs_raw_line - jr->tmp;

			//printf("(%d) s_rs%d\n",__LINE__, seg_rs_raw_line);
			cjpegIoNext(&jr->jccb, jr->seg_rs_raw_line, &jr->seg_jpeg_buf, &jr->seg_jpeg_size, jr->flag_decode_done);
			//printf("(%d) js%d\n",__LINE__, seg_jpeg_size);
			jr->outfilesize+=jr->seg_jpeg_size;
			jr->state = JPEG_RESIZE_STATE_NEXT;

			*jpeg_out = jr->seg_jpeg_buf;
			*jpeg_out_size = jr->seg_jpeg_size;

			if(jr->flag_decode_done)
				return 1;
			else 
				return 0;

		default:
			break;
	}
	return 0;
}

int jpeg_resize_free(JpegResizeCB* jr)
{
	free(jr->jccb.out_buffer);
	return 0;
}


int resize_init(ResizeCB* rcb, int width, int height, int old_width, int old_height, int bitsPerPixel)
{
	rcb->seg_rs_raw_line=0;
	rcb->tot_raw_line=0;
	rcb->tot_rs_raw_line=0;
	rcb->tmp=0;
//printf();
	resamplePartialInit(&rcb->rscb, width, height, 
			NULL, old_width, old_height, NULL, bitsPerPixel, RESAMPLE_NEAREST);
	return 0;
}

int resize_read(ResizeCB* rcb, unsigned char* raw_out, unsigned long* out_height, unsigned char*raw_in, unsigned long in_height)
{
//	int i;
//	int bytePerPixle = rcb->rscb.current_bitsPerPixel/8;
	int out_h;
//	int out_stride = rcb->rscb.current_width*bytePerPixle;
//	unsigned char*s,*d;

//	printf("in buf %d  out buf %d\n", raw_in, raw_out);

	rcb->tmp = rcb->tot_rs_raw_line;
	rcb->tot_raw_line += in_height;
//	printf("(%d) tot_rs %d tot_raw %d seg %d\n",__LINE__, rcb->tot_rs_raw_line, rcb->tot_raw_line, rcb->seg_rs_raw_line);

	resamplePartialRead3(&rcb->rscb, &rcb->tot_rs_raw_line, rcb->tot_raw_line, 0, raw_in, raw_out);

	

	rcb->seg_rs_raw_line = rcb->tot_rs_raw_line - rcb->tmp;
	out_h = rcb->seg_rs_raw_line;
	
//	printf("(%d) tot_rs %d tot_raw %d seg %d\n",__LINE__, rcb->tot_rs_raw_line, rcb->tot_raw_line, rcb->seg_rs_raw_line);

	//out_h = rcb->rawQueueOut.size();
//	printf("out_h %d\n", out_h);
	//*raw_out = (unsigned char*)malloc(out_stride*out_h);
	*out_height = (long)out_h;

	
	return 0;
}
