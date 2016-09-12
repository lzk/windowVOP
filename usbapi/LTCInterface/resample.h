#ifndef _RESAMPLE_
#define _RESAMPLE_
enum RESAMPLE_TYPE { 
	RESAMPLE_NEAREST,
	RESAMPLE_BOX,
	RESAMPLE_BILINEAR,
	RESAMPLE_BICUBIC
};

int resample(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data, int bitsPerPixel, int resample_type);

typedef struct _ResampleControlBlock{
	int current_y;
	int current_y_old;
	int available_y_old;
	int current_width;
	int	current_height;
	int current_bitsPerPixel;
	int current_old_width;
	int	current_old_height;
	unsigned char* source_data;
	unsigned char* destination_data;
	LineNode* src_list;
	LineNode* dst_list;
}ResampleControlBlock;

int resamplePartialInit(ResampleControlBlock* rscb, int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data, int bitsPerPixel, int resample_type);

// Need one big buffer to partial resize.
int resamplePartialRead(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end);

// Need two Queues to partial resize.
int resamplePartialRead2(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end, queue<unsigned char*> & src_Q, queue<unsigned char*> & dst_Q);

// Need two small buffer to resize.
int resamplePartialRead3(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end, unsigned char* srouce_data, unsigned char* destination_data);

#endif