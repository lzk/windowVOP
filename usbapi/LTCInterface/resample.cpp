#include "..\stdafx.h"
#include <vector>
#include <algorithm>
using namespace std;
#define wxVector vector
#define wxMin min
#define wxMax max
//#define DB_LINE printf("%s %d\n", __FUNCTION__, __LINE__);
#define DB_LINE 
//bw
int resampleNearest_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBox_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBilinear_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBicubic_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
//color
int resampleNearest(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBox(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBilinear(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBicubic(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
//gray
int resampleNearest_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBox_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBilinear_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBicubic_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
//color48
int resampleNearest16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBox16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBilinear16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBicubic16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
//gray16
int resampleNearest_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBox_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBilinear_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBicubic_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
//bw
int resampleNearest_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBox_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBilinear_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);
int ResampleBicubic_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data);

int resample(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data, int bitsPerPixel, int resample_type)
{ 
	int (*f1[4])(int, int, unsigned char*,int, int, unsigned char*) = { &resampleNearest_bw, &ResampleBox_bw, &ResampleBilinear_bw, &ResampleBicubic_bw};
	int (*f8[4])(int, int, unsigned char*,int, int, unsigned char*) = { &resampleNearest_gray, &ResampleBox_gray, &ResampleBilinear_gray, &ResampleBicubic_gray};
	int (*f16[4])(int, int, unsigned char*,int, int, unsigned char*) = { &resampleNearest_gray16, &ResampleBox_gray16, &ResampleBilinear_gray16, &ResampleBicubic_gray16};
	int (*f24[4])(int, int, unsigned char*,int, int, unsigned char*) = { &resampleNearest, &ResampleBox, &ResampleBilinear, &ResampleBicubic};
	int (*f48[4])(int, int, unsigned char*,int, int, unsigned char*) = { &resampleNearest16, &ResampleBox16, &ResampleBilinear16, &ResampleBicubic16};

	switch(bitsPerPixel) {
	case 1:
		(*f1[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 8:
		(*f8[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 16:
		(*f16[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 24:
		(*f24[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 48:
		(*f48[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	}
	return 0;
}

int resampleNearest(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;

    unsigned char* dest_pixel = target_data;

    long y = 0;
    for ( long j = 0; j < height; j++ )
    {
        const unsigned char* src_line = &source_data[(y>>14)*old_width*3];

        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)*3];
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
        }
		
        y += y_delta;
    }

    return 0;
}

int resampleNearest16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;

    //unsigned char* dest_pixel = target_data;
	unsigned short* dest_pixel = (unsigned short*)target_data;
	unsigned short* src_pixel = (unsigned short*)source_data;

    long y = 0;
    for ( long j = 0; j < height; j++ )
    {
        const unsigned short* src_line = &src_pixel[(y>>14)*old_width*3];

        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)*3];
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
        }

        y += y_delta;
    }

    return 0;
}

int resampleNearest_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;

    unsigned char* dest_pixel = target_data;

    long y = 0;
    for ( long j = 0; j < height; j++ )
    {
        const unsigned char* src_line = &source_data[(y>>14)*old_width];

        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)];
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
        }

        y += y_delta;
    }

    return 0;
}

int resampleNearest_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;

    //unsigned char* dest_pixel = target_data;
	unsigned short* dest_pixel = (unsigned short*)target_data;
	unsigned short* src_pixel = (unsigned short*)source_data;

    long y = 0;
    for ( long j = 0; j < height; j++ )
    {
        const unsigned short* src_line = &src_pixel[(y>>14)*old_width];

        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)];
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
        }

        y += y_delta;
    }

    return 0;
}

int resampleNearestPartial(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    unsigned char* dest_pixel = target_data + *current_y*width*3;
    long y = *current_y_old;
    for ( long j = *current_y; j < height; j++ )
    {	
		if(*current_y_old > *available_y_old) {
			break;
		}
        const unsigned char* src_line = &source_data[(y>>14)*old_width*3];	
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)*3];
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
        }	
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
	//printf("current_y_old %d current_y %d available_y_old %d dx %d dy %d\n", current_y_old>>14, current_y, available_y_old>>14, x_delta>>14, y_delta>>14);
    return 0;
}

int resampleNearestPartial16(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    //unsigned char* dest_pixel = target_data;
	unsigned short* dest_pixel = (unsigned short*)(target_data + *current_y*width*6);
	unsigned short* src_pixel = (unsigned short*)source_data;

    long y = *current_y_old;
    for ( long j = *current_y; j < height; j++ )
    {
		if(*current_y_old > *available_y_old) {
			break;
		}

        const unsigned short* src_line = &src_pixel[(y>>14)*old_width*3];
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)*3];
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
        }
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
    return 0;
}

int resampleNearestPartial_gray(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    unsigned char* dest_pixel = target_data + *current_y*width;
    long y = (long)*current_y_old;
    for ( long j = *current_y; j < height; j++ )
    {
		if(*current_y_old > *available_y_old) {
			break;
		}
        const unsigned char* src_line = &source_data[(y>>14)*old_width];
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)];
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
        }
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
    return 0;
}

int resampleNearestPartial_gray16(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    //unsigned char* dest_pixel = target_data;
	unsigned short* dest_pixel = (unsigned short*)(target_data + *current_y*width*2);
	unsigned short* src_pixel = (unsigned short*)source_data;
    long y = *current_y_old;
    for ( long j = *current_y; j < height; j++ )
    {
		if(*current_y_old > *available_y_old) {
			break;
		}
        const unsigned short* src_line = &src_pixel[(y>>14)*old_width];
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)];
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
        }
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    } 
    return 0;
}

int resampleNearest2Partial(int width, int height, queue<unsigned char*> & target_data, 
						   int old_width, int old_height, queue<unsigned char*> & source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
//printf("%d %d %d %d %d %d %d\n",width,height, old_width,old_height, *current_y, *current_y_old, *available_y_old);
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    //unsigned char* dest_pixel = target_data + *current_y*width*3;
	unsigned char* dest_pixel;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;
	//printf("queue size %d\n", source_data.size());
	if(source_data.size()==0)
		return 1;
	unsigned char* src_line = (unsigned char*)source_data.front();	
	//printf("src_line %d\n", src_line);
	source_data.pop();

    for ( long j = *current_y; j < height; j++ )
    {	
		//printf("j %d %d %d\n", j, *current_y_old,*available_y_old);
		if(*current_y_old >= *available_y_old) {
			break;
		}
		//const unsigned char* src_line = &source_data[(y>>14)*old_width*3];	

		index_old = (y>>14);
		//printf("bass %d index_old %d\n", bass, index_old);
		while(1) {
			if(index_old > bass) {
				src_line = (unsigned char* )source_data.front();	
				source_data.pop();
				bass++;
				if(bass >= index_old) {
					break;
				}
				else {
					free(src_line);
				}
			}
			else {
				break;
			}
		}

		dest_pixel = (unsigned char* )malloc(width*3);
		target_data.push(dest_pixel);
		//printf("src %d dst%d \n", src_line, dest_pixel);
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)*3];
			//printf("i %d src_pixel %d dst %d\n", i, src_pixel, dest_pixel);
			//printf("i %d %d %d\n", i, dest_pixel[0], src_pixel[0]);
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
			
        }	
		free(src_line);
		//DB_LINE
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
	//printf("current_y_old %d current_y %d available_y_old %d dx %d dy %d\n", current_y_old>>14, current_y, available_y_old>>14, x_delta>>14, y_delta>>14);
	//clean the data in Queue to let the image scale correct in Y direction 
	//And to prevent the memory leak issue.
	{
		int q_size = (int)source_data.size();
		if(q_size!=0) {
			for(int i=0; i<q_size; i++) {
				free(source_data.front());
				source_data.pop();
			}
		}
	}
    return 0;
}

int resampleNearest2Partial16(int width, int height, queue<unsigned char*> & target_data, 
						   int old_width, int old_height, queue<unsigned char*> & source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    //unsigned char* dest_pixel = target_data + *current_y*width*3;
	unsigned short* dest_pixel;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;
	if(source_data.size()==0)
		return 1;
	unsigned short* src_line = (unsigned short*)source_data.front();	
	//printf("src_line %d\n", src_line);
	source_data.pop();

    for ( long j = *current_y; j < height; j++ )
    {	
		//printf("j %d %d %d\n", j, *current_y_old,*available_y_old);
		if(*current_y_old >= *available_y_old) {
			break;
		}
		//const unsigned char* src_line = &source_data[(y>>14)*old_width*3];	

		index_old = (y>>14);
		//printf("bass %d index_old %d\n", bass, index_old);
		while(1) {
			if(index_old > bass) {
				src_line = (unsigned short* )source_data.front();	
				source_data.pop();
				bass++;
				if(bass >= index_old) {
					break;
				}
				else {
					free(src_line);
				}
			}
			else {
				break;
			}
		}

		dest_pixel = (unsigned short* )malloc(width*3);
		target_data.push((unsigned char*)dest_pixel);
		//printf("src %d dst%d \n", src_line, dest_pixel);
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)*3];
			//printf("i %d src_pixel %d dst %d\n", i, src_pixel, dest_pixel);
			//printf("i %d %d %d\n", i, dest_pixel[0], src_pixel[0]);
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
			
        }	
		free(src_line);
		//DB_LINE
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
	//printf("current_y_old %d current_y %d available_y_old %d dx %d dy %d\n", current_y_old>>14, current_y, available_y_old>>14, x_delta>>14, y_delta>>14);
    //clean the data in Queue to let the image scale correct in Y direction 
	//And to prevent the memory leak issue.
	{
		int q_size = (int)source_data.size();
		if(q_size!=0) {
			for(int i=0; i<q_size; i++) {
				free(source_data.front());
				source_data.pop();
			}
		}
	}
	return 0;
}

int resampleNearest2Partial_gray(int width, int height, queue<unsigned char*> & target_data, 
						   int old_width, int old_height, queue<unsigned char*> & source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    //unsigned char* dest_pixel = target_data + *current_y*width*3;
	unsigned char* dest_pixel;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;
	if(source_data.size()==0)
		return 1;	
	unsigned char* src_line = (unsigned char*)source_data.front();	
	//printf("src_line %d\n", src_line);
	source_data.pop();

    for ( long j = *current_y; j < height; j++ )
    {	
		//printf("j %d %d %d\n", j, *current_y_old,*available_y_old);
		if(*current_y_old >= *available_y_old) {
			break;
		}
		//const unsigned char* src_line = &source_data[(y>>14)*old_width*3];	

		index_old = (y>>14);
		//printf("bass %d index_old %d\n", bass, index_old);
		while(1) {
			if(index_old > bass) {
				src_line = (unsigned char* )source_data.front();	
				source_data.pop();
				bass++;
				if(bass >= index_old) {
					break;
				}
				else {
					free(src_line);
				}
			}
			else {
				break;
			}
		}

		dest_pixel = (unsigned char* )malloc(width);
		target_data.push(dest_pixel);
		//printf("src %d dst%d \n", src_line, dest_pixel);
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)];
			//printf("i %d src_pixel %d dst %d\n", i, src_pixel, dest_pixel);
			//printf("i %d %d %d\n", i, dest_pixel[0], src_pixel[0]);
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
			
        }	
		free(src_line);
		//DB_LINE
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
	//printf("current_y_old %d current_y %d available_y_old %d dx %d dy %d\n", current_y_old>>14, current_y, available_y_old>>14, x_delta>>14, y_delta>>14);
    //clean the data in Queue to let the image scale correct in Y direction 
	//And to prevent the memory leak issue.
	{
		int q_size = (int)source_data.size();
		if(q_size!=0) {
			for(int i=0; i<q_size; i++) {
				free(source_data.front());
				source_data.pop();
			}
		}
	}
	return 0;
}

int resampleNearest2Partial_gray16(int width, int height, queue<unsigned char*> & target_data, 
						   int old_width, int old_height, queue<unsigned char*> & source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    //unsigned char* dest_pixel = target_data + *current_y*width*3;
	unsigned short* dest_pixel;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;
	if(source_data.size()==0)
		return 1;	
	unsigned short* src_line = (unsigned short*)source_data.front();	
	//printf("src_line %d\n", src_line);
	source_data.pop();

    for ( long j = *current_y; j < height; j++ )
    {	
		//printf("j %d %d %d\n", j, *current_y_old,*available_y_old);
		if(*current_y_old >= *available_y_old) {
			break;
		}
		//const unsigned char* src_line = &source_data[(y>>14)*old_width*3];	

		index_old = (y>>14);
		//printf("bass %d index_old %d\n", bass, index_old);
		while(1) {
			if(index_old > bass) {
				src_line = (unsigned short* )source_data.front();	
				source_data.pop();
				bass++;
				if(bass >= index_old) {
					break;
				}
				else {
					free(src_line);
				}
			}
			else {
				break;
			}
		}

		dest_pixel = (unsigned short* )malloc(width);
		target_data.push((unsigned char*)dest_pixel);
		//printf("src %d dst%d \n", src_line, dest_pixel);
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)];
			//printf("i %d src_pixel %d dst %d\n", i, src_pixel, dest_pixel);
			//printf("i %d %d %d\n", i, dest_pixel[0], src_pixel[0]);
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
			
        }	
		free(src_line);
		//DB_LINE
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
	//printf("current_y_old %d current_y %d available_y_old %d dx %d dy %d\n", current_y_old>>14, current_y, available_y_old>>14, x_delta>>14, y_delta>>14);
    //clean the data in Queue to let the image scale correct in Y direction 
	//And to prevent the memory leak issue.
	{
		int q_size = (int)source_data.size();
		if(q_size!=0) {
			for(int i=0; i<q_size; i++) {
				free(source_data.front());
				source_data.pop();
			}
		}
	}
	return 0;
}

int resampleNearest3Partial(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    unsigned char* dest_pixel = target_data;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;
	
    for ( long j = *current_y; j < height; j++ )
    {	
		if(*current_y_old >= *available_y_old) {
			break;
		}
		index_old = (y>>14);
		index_old -= bass;
        const unsigned char* src_line = &source_data[index_old*old_width*3];
		//DB_LINE
		//if(src_line==NULL) 
			//printf("sre line is null\n");
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)*3];
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
        }	
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
	//printf("current_y_old %d current_y %d available_y_old %d dx %d dy %d\n", *current_y_old>>14, *current_y, *available_y_old>>14, x_delta>>14, y_delta>>14);
    return 0;
}

int resampleNearest3Partial16(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data_in,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
	//unsigned short* target_data = (unsigned short*)target_data_in;
	unsigned short* source_data = (unsigned short*)source_data_in;
	unsigned short* dest_pixel = (unsigned short*)target_data;
	//unsigned short* src_pixel;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;

    for ( long j = *current_y; j < height; j++ )
    {
		if(*current_y_old >= *available_y_old) {
			break;
		}

        index_old = (y>>14);
		index_old -= bass;
        const unsigned short* src_line = &source_data[index_old*old_width*3];
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)*3];
            dest_pixel[0] = src_pixel[0];
            dest_pixel[1] = src_pixel[1];
            dest_pixel[2] = src_pixel[2];
            dest_pixel += 3;
            x += x_delta;
        }
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
    return 0;
}

int resampleNearest3Partial_gray(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
    unsigned char* dest_pixel = target_data;
    long y = (long)*current_y_old;
	int bass = (y>>14);
	int index_old;

    for ( long j = *current_y; j < height; j++ )
    {
		if(*current_y_old >= *available_y_old) {
			break;
		}
        index_old = (y>>14);
		index_old -= bass;
        const unsigned char* src_line = &source_data[index_old*old_width];
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned char* src_pixel = &src_line[(x>>14)];
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
        }
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    }
    return 0;
}

int resampleNearest3Partial_gray16(int width, int height, unsigned char* target_data, 
						   int old_width, int old_height, unsigned char* source_data_in,
						   int* current_y, int* current_y_old, int* available_y_old)
{
DB_LINE
    long x_delta = (old_width<<14) / width;
    long y_delta = (old_height<<14) / height;
	//unsigned short* target_data = (unsigned short*)target_data_in;
	unsigned short* source_data = (unsigned short*)source_data_in;
	unsigned short* dest_pixel = (unsigned short*)target_data;
//	unsigned short* src_pixel;
    long y = *current_y_old;
	int bass = (y>>14);
	int index_old;
    for ( long j = *current_y; j < height; j++ )
    {
		if(*current_y_old >= *available_y_old) {
			break;
		}
        index_old = (y>>14);
		index_old -= bass;
        const unsigned short* src_line = &source_data[index_old*old_width];
        long x = 0;
        for ( long i = 0; i < width; i++ )
        {
            const unsigned short* src_pixel = &src_line[(x>>14)];
            dest_pixel[0] = src_pixel[0];
            dest_pixel += 1;
            x += x_delta;
        }
        y += y_delta;
		*current_y_old = y;
		*current_y = j+1;
    } 
    return 0;
}

namespace
{

struct BoxPrecalc
{
    int boxStart;
    int boxEnd;
};

inline int BoxBetween(int value, int low, int high)
{
    return wxMax(wxMin(value, high), low);
	//return max(min(value, high), low);
}

void ResampleBoxPrecalc(wxVector<BoxPrecalc>& boxes, int oldDim)
{
    const int newDim = (int)boxes.size();
    const double scale_factor_1 = double(oldDim) / newDim;
    const int scale_factor_2 = (int)(scale_factor_1 / 2);

    for ( int dst = 0; dst < newDim; ++dst )
    {
        // Source pixel in the Y direction
        const int src_p = int(dst * scale_factor_1);

        BoxPrecalc& precalc = boxes[dst];
        precalc.boxStart = BoxBetween(int(src_p - scale_factor_1/2.0 + 1),
                                      0, oldDim - 1);
        precalc.boxEnd = BoxBetween(wxMax(precalc.boxStart + 1,
                                          int(src_p + scale_factor_2)),
                                    0, oldDim - 1);
    }
}

} // anonymous namespace

//wxImage wxImage::ResampleBox(int width, int height) const
int ResampleBox(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
    // This function implements a simple pre-blur/box averaging method for
    // downsampling that gives reasonably smooth results To scale the image
    // down we will need to gather a grid of pixels of the size of the scale
    // factor in each direction and then do an averaging of the pixels.
DB_LINE
   
    wxVector<BoxPrecalc> vPrecalcs(height);
    wxVector<BoxPrecalc> hPrecalcs(width);

    ResampleBoxPrecalc(vPrecalcs, old_height);
    ResampleBoxPrecalc(hPrecalcs, old_width);


    const unsigned char* src_data = source_data;
    unsigned char* dst_data = target_data;

    int averaged_pixels, src_pixel_index;
    double sum_r, sum_g, sum_b, sum_a;

    for ( int y = 0; y < height; y++ )         // Destination image - Y direction
    {
        // Source pixel in the Y direction
        const BoxPrecalc& vPrecalc = vPrecalcs[y];

        for ( int x = 0; x < width; x++ )      // Destination image - X direction
        {
            // Source pixel in the X direction
            const BoxPrecalc& hPrecalc = hPrecalcs[x];

            // Box of pixels to average
            averaged_pixels = 0;
            sum_r = sum_g = sum_b = sum_a = 0.0;

            for ( int j = vPrecalc.boxStart; j <= vPrecalc.boxEnd; ++j )
            {
                for ( int i = hPrecalc.boxStart; i <= hPrecalc.boxEnd; ++i )
                {
                    // Calculate the actual index in our source pixels
                    src_pixel_index = j * old_width + i;

                    sum_r += src_data[src_pixel_index * 3 + 0];
                    sum_g += src_data[src_pixel_index * 3 + 1];
                    sum_b += src_data[src_pixel_index * 3 + 2];
                
                    averaged_pixels++;
                }
            }

            // Calculate the average from the sum and number of averaged pixels
            dst_data[0] = (unsigned char)(sum_r / averaged_pixels);
            dst_data[1] = (unsigned char)(sum_g / averaged_pixels);
            dst_data[2] = (unsigned char)(sum_b / averaged_pixels);
            dst_data += 3;
     
        }
    }

    return 0;
}

int ResampleBox16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
    // This function implements a simple pre-blur/box averaging method for
    // downsampling that gives reasonably smooth results To scale the image
    // down we will need to gather a grid of pixels of the size of the scale
    // factor in each direction and then do an averaging of the pixels.
DB_LINE
   
    wxVector<BoxPrecalc> vPrecalcs(height);
    wxVector<BoxPrecalc> hPrecalcs(width);

    ResampleBoxPrecalc(vPrecalcs, old_height);
    ResampleBoxPrecalc(hPrecalcs, old_width);


    const unsigned short* src_data = (unsigned short*)source_data;
    unsigned short* dst_data = (unsigned short*)target_data;

    int averaged_pixels, src_pixel_index;
    double sum_r, sum_g, sum_b, sum_a;

    for ( int y = 0; y < height; y++ )         // Destination image - Y direction
    {
        // Source pixel in the Y direction
        const BoxPrecalc& vPrecalc = vPrecalcs[y];

        for ( int x = 0; x < width; x++ )      // Destination image - X direction
        {
            // Source pixel in the X direction
            const BoxPrecalc& hPrecalc = hPrecalcs[x];

            // Box of pixels to average
            averaged_pixels = 0;
            sum_r = sum_g = sum_b = sum_a = 0.0;

            for ( int j = vPrecalc.boxStart; j <= vPrecalc.boxEnd; ++j )
            {
                for ( int i = hPrecalc.boxStart; i <= hPrecalc.boxEnd; ++i )
                {
                    // Calculate the actual index in our source pixels
                    src_pixel_index = j * old_width + i;

                    sum_r += src_data[src_pixel_index * 3 + 0];
                    sum_g += src_data[src_pixel_index * 3 + 1];
                    sum_b += src_data[src_pixel_index * 3 + 2];
                
                    averaged_pixels++;
                }
            }

            // Calculate the average from the sum and number of averaged pixels
            dst_data[0] = (unsigned short)(sum_r / averaged_pixels);
            dst_data[1] = (unsigned short)(sum_g / averaged_pixels);
            dst_data[2] = (unsigned short)(sum_b / averaged_pixels);
            dst_data += 3;
     
        }
    }

    return 0;
}


int ResampleBox_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
    // This function implements a simple pre-blur/box averaging method for
    // downsampling that gives reasonably smooth results To scale the image
    // down we will need to gather a grid of pixels of the size of the scale
    // factor in each direction and then do an averaging of the pixels.
DB_LINE
   
    wxVector<BoxPrecalc> vPrecalcs(height);
    wxVector<BoxPrecalc> hPrecalcs(width);

    ResampleBoxPrecalc(vPrecalcs, old_height);
    ResampleBoxPrecalc(hPrecalcs, old_width);


    const unsigned char* src_data = source_data;
    unsigned char* dst_data = target_data;

    int averaged_pixels, src_pixel_index;
    double sum_g;

    for ( int y = 0; y < height; y++ )         // Destination image - Y direction
    {
        // Source pixel in the Y direction
        const BoxPrecalc& vPrecalc = vPrecalcs[y];

        for ( int x = 0; x < width; x++ )      // Destination image - X direction
        {
            // Source pixel in the X direction
            const BoxPrecalc& hPrecalc = hPrecalcs[x];

            // Box of pixels to average
            averaged_pixels = 0;
            sum_g = 0.0;

            for ( int j = vPrecalc.boxStart; j <= vPrecalc.boxEnd; ++j )
            {
                for ( int i = hPrecalc.boxStart; i <= hPrecalc.boxEnd; ++i )
                {
                    // Calculate the actual index in our source pixels
                    src_pixel_index = j * old_width + i;
                    sum_g += src_data[src_pixel_index];
                    averaged_pixels++;
                }
            }

            // Calculate the average from the sum and number of averaged pixels
            dst_data[0] = (unsigned char)(sum_g / averaged_pixels);
            dst_data += 1;
     
        }
    }

    return 0;
}

int ResampleBox_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
    // This function implements a simple pre-blur/box averaging method for
    // downsampling that gives reasonably smooth results To scale the image
    // down we will need to gather a grid of pixels of the size of the scale
    // factor in each direction and then do an averaging of the pixels.
DB_LINE
   
    wxVector<BoxPrecalc> vPrecalcs(height);
    wxVector<BoxPrecalc> hPrecalcs(width);

    ResampleBoxPrecalc(vPrecalcs, old_height);
    ResampleBoxPrecalc(hPrecalcs, old_width);


    const unsigned short* src_data = (unsigned short*)source_data;
    unsigned short* dst_data = (unsigned short*)target_data;

    int averaged_pixels, src_pixel_index;
    double sum_g;

    for ( int y = 0; y < height; y++ )         // Destination image - Y direction
    {
        // Source pixel in the Y direction
        const BoxPrecalc& vPrecalc = vPrecalcs[y];

        for ( int x = 0; x < width; x++ )      // Destination image - X direction
        {
            // Source pixel in the X direction
            const BoxPrecalc& hPrecalc = hPrecalcs[x];

            // Box of pixels to average
            averaged_pixels = 0;
            sum_g = 0.0;

            for ( int j = vPrecalc.boxStart; j <= vPrecalc.boxEnd; ++j )
            {
                for ( int i = hPrecalc.boxStart; i <= hPrecalc.boxEnd; ++i )
                {
                    // Calculate the actual index in our source pixels
                    src_pixel_index = j * old_width + i;
                    sum_g += src_data[src_pixel_index];
                    averaged_pixels++;
                }
            }

            // Calculate the average from the sum and number of averaged pixels
            dst_data[0] = (unsigned short)(sum_g / averaged_pixels);
            dst_data += 1;
     
        }
    }

    return 0;
}


namespace
{

struct BilinearPrecalc
{
    int offset1;
    int offset2;
    double dd;
    double dd1;
};

void ResampleBilinearPrecalc(wxVector<BilinearPrecalc>& precalcs, int oldDim)
{
    const int newDim = (int)precalcs.size();
    const double scale_factor = double(oldDim) / newDim;
    const int srcpixmax = oldDim - 1;

    for ( int dsty = 0; dsty < newDim; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        double srcpix = double(dsty) * scale_factor;
        double srcpix1 = int(srcpix);
        double srcpix2 = srcpix1 == srcpixmax ? srcpix1 : srcpix1 + 1.0;

        BilinearPrecalc& precalc = precalcs[dsty];

        precalc.dd = srcpix - (int)srcpix;
        precalc.dd1 = 1.0 - precalc.dd;
        precalc.offset1 = srcpix1 < 0.0
                            ? 0
                            : srcpix1 > srcpixmax
                                ? srcpixmax
                                : (int)srcpix1;
        precalc.offset2 = srcpix2 < 0.0
                            ? 0
                            : srcpix2 > srcpixmax
                                ? srcpixmax
                                : (int)srcpix2;
    }
}

}

int ResampleBilinear(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
    // This function implements a Bilinear algorithm for resampling.
    const unsigned char* src_data = source_data;
    unsigned char* dst_data = target_data;
DB_LINE
    wxVector<BilinearPrecalc> vPrecalcs(height);
    wxVector<BilinearPrecalc> hPrecalcs(width);
    ResampleBilinearPrecalc(vPrecalcs, old_height);
    ResampleBilinearPrecalc(hPrecalcs, old_width);

    // initialize alpha values to avoid g++ warnings about possibly
    // uninitialized variables
    double r1, g1, b1;
    double r2, g2, b2;

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BilinearPrecalc& vPrecalc = vPrecalcs[dsty];
        const int y_offset1 = vPrecalc.offset1;
        const int y_offset2 = vPrecalc.offset2;
        const double dy = vPrecalc.dd;
        const double dy1 = vPrecalc.dd1;


        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BilinearPrecalc& hPrecalc = hPrecalcs[dstx];

            const int x_offset1 = hPrecalc.offset1;
            const int x_offset2 = hPrecalc.offset2;
            const double dx = hPrecalc.dd;
            const double dx1 = hPrecalc.dd1;

            int src_pixel_index00 = y_offset1 * old_width + x_offset1;
            int src_pixel_index01 = y_offset1 * old_width + x_offset2;
            int src_pixel_index10 = y_offset2 * old_width + x_offset1;
            int src_pixel_index11 = y_offset2 * old_width + x_offset2;

            // first line
            r1 = src_data[src_pixel_index00 * 3 + 0] * dx1 + src_data[src_pixel_index01 * 3 + 0] * dx;
            g1 = src_data[src_pixel_index00 * 3 + 1] * dx1 + src_data[src_pixel_index01 * 3 + 1] * dx;
            b1 = src_data[src_pixel_index00 * 3 + 2] * dx1 + src_data[src_pixel_index01 * 3 + 2] * dx;
            
            // second line
            r2 = src_data[src_pixel_index10 * 3 + 0] * dx1 + src_data[src_pixel_index11 * 3 + 0] * dx;
            g2 = src_data[src_pixel_index10 * 3 + 1] * dx1 + src_data[src_pixel_index11 * 3 + 1] * dx;
            b2 = src_data[src_pixel_index10 * 3 + 2] * dx1 + src_data[src_pixel_index11 * 3 + 2] * dx;
          
            // result lines
            dst_data[0] = static_cast<unsigned char>(r1 * dy1 + r2 * dy);
            dst_data[1] = static_cast<unsigned char>(g1 * dy1 + g2 * dy);
            dst_data[2] = static_cast<unsigned char>(b1 * dy1 + b2 * dy);
            dst_data += 3;

        }
    }

    return 0;
}

int ResampleBilinear16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
    // This function implements a Bilinear algorithm for resampling.
    const unsigned short* src_data = (unsigned short*)source_data;
    unsigned short* dst_data = (unsigned short*)target_data;
DB_LINE
    wxVector<BilinearPrecalc> vPrecalcs(height);
    wxVector<BilinearPrecalc> hPrecalcs(width);
    ResampleBilinearPrecalc(vPrecalcs, old_height);
    ResampleBilinearPrecalc(hPrecalcs, old_width);

    // initialize alpha values to avoid g++ warnings about possibly
    // uninitialized variables
    double r1, g1, b1;
    double r2, g2, b2;

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BilinearPrecalc& vPrecalc = vPrecalcs[dsty];
        const int y_offset1 = vPrecalc.offset1;
        const int y_offset2 = vPrecalc.offset2;
        const double dy = vPrecalc.dd;
        const double dy1 = vPrecalc.dd1;


        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BilinearPrecalc& hPrecalc = hPrecalcs[dstx];

            const int x_offset1 = hPrecalc.offset1;
            const int x_offset2 = hPrecalc.offset2;
            const double dx = hPrecalc.dd;
            const double dx1 = hPrecalc.dd1;

            int src_pixel_index00 = y_offset1 * old_width + x_offset1;
            int src_pixel_index01 = y_offset1 * old_width + x_offset2;
            int src_pixel_index10 = y_offset2 * old_width + x_offset1;
            int src_pixel_index11 = y_offset2 * old_width + x_offset2;

            // first line
            r1 = src_data[src_pixel_index00 * 3 + 0] * dx1 + src_data[src_pixel_index01 * 3 + 0] * dx;
            g1 = src_data[src_pixel_index00 * 3 + 1] * dx1 + src_data[src_pixel_index01 * 3 + 1] * dx;
            b1 = src_data[src_pixel_index00 * 3 + 2] * dx1 + src_data[src_pixel_index01 * 3 + 2] * dx;
            
            // second line
            r2 = src_data[src_pixel_index10 * 3 + 0] * dx1 + src_data[src_pixel_index11 * 3 + 0] * dx;
            g2 = src_data[src_pixel_index10 * 3 + 1] * dx1 + src_data[src_pixel_index11 * 3 + 1] * dx;
            b2 = src_data[src_pixel_index10 * 3 + 2] * dx1 + src_data[src_pixel_index11 * 3 + 2] * dx;
          
            // result lines
            dst_data[0] = static_cast<unsigned short>(r1 * dy1 + r2 * dy);
            dst_data[1] = static_cast<unsigned short>(g1 * dy1 + g2 * dy);
            dst_data[2] = static_cast<unsigned short>(b1 * dy1 + b2 * dy);
            dst_data += 3;

        }
    }

    return 0;
}

int ResampleBilinear_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    // This function implements a Bilinear algorithm for resampling.
    const unsigned char* src_data = source_data;
    unsigned char* dst_data = target_data;

    wxVector<BilinearPrecalc> vPrecalcs(height);
    wxVector<BilinearPrecalc> hPrecalcs(width);
    ResampleBilinearPrecalc(vPrecalcs, old_height);
    ResampleBilinearPrecalc(hPrecalcs, old_width);

    // initialize alpha values to avoid g++ warnings about possibly
    // uninitialized variables
    double g1;
    double g2;

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BilinearPrecalc& vPrecalc = vPrecalcs[dsty];
        const int y_offset1 = vPrecalc.offset1;
        const int y_offset2 = vPrecalc.offset2;
        const double dy = vPrecalc.dd;
        const double dy1 = vPrecalc.dd1;


        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BilinearPrecalc& hPrecalc = hPrecalcs[dstx];

            const int x_offset1 = hPrecalc.offset1;
            const int x_offset2 = hPrecalc.offset2;
            const double dx = hPrecalc.dd;
            const double dx1 = hPrecalc.dd1;

            int src_pixel_index00 = y_offset1 * old_width + x_offset1;
            int src_pixel_index01 = y_offset1 * old_width + x_offset2;
            int src_pixel_index10 = y_offset2 * old_width + x_offset1;
            int src_pixel_index11 = y_offset2 * old_width + x_offset2;

            // first line
            g1 = src_data[src_pixel_index00] * dx1 + src_data[src_pixel_index01] * dx;          
          
            // second line
            g2 = src_data[src_pixel_index10] * dx1 + src_data[src_pixel_index11] * dx;
                     
            // result lines
            dst_data[0] = static_cast<unsigned char>(g1 * dy1 + g2 * dy);        
            dst_data += 1;

        }
    }

    return 0;
}

int ResampleBilinear_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    // This function implements a Bilinear algorithm for resampling.
    const unsigned short* src_data = (unsigned short*)source_data;
    unsigned short* dst_data = (unsigned short*)target_data;

    wxVector<BilinearPrecalc> vPrecalcs(height);
    wxVector<BilinearPrecalc> hPrecalcs(width);
    ResampleBilinearPrecalc(vPrecalcs, old_height);
    ResampleBilinearPrecalc(hPrecalcs, old_width);

    // initialize alpha values to avoid g++ warnings about possibly
    // uninitialized variables
    double g1;
    double g2;

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BilinearPrecalc& vPrecalc = vPrecalcs[dsty];
        const int y_offset1 = vPrecalc.offset1;
        const int y_offset2 = vPrecalc.offset2;
        const double dy = vPrecalc.dd;
        const double dy1 = vPrecalc.dd1;


        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BilinearPrecalc& hPrecalc = hPrecalcs[dstx];

            const int x_offset1 = hPrecalc.offset1;
            const int x_offset2 = hPrecalc.offset2;
            const double dx = hPrecalc.dd;
            const double dx1 = hPrecalc.dd1;

            int src_pixel_index00 = y_offset1 * old_width + x_offset1;
            int src_pixel_index01 = y_offset1 * old_width + x_offset2;
            int src_pixel_index10 = y_offset2 * old_width + x_offset1;
            int src_pixel_index11 = y_offset2 * old_width + x_offset2;

            // first line
            g1 = src_data[src_pixel_index00] * dx1 + src_data[src_pixel_index01] * dx;          
          
            // second line
            g2 = src_data[src_pixel_index10] * dx1 + src_data[src_pixel_index11] * dx;
                     
            // result lines
            dst_data[0] = static_cast<unsigned short>(g1 * dy1 + g2 * dy);        
            dst_data += 1;

        }
    }

    return 0;
}



// The following two local functions are for the B-spline weighting of the
// bicubic sampling algorithm
static inline double spline_cube(double value)
{
    return value <= 0.0 ? 0.0 : value * value * value;
}

static inline double spline_weight(double value)
{
    return (spline_cube(value + 2) -
            4 * spline_cube(value + 1) +
            6 * spline_cube(value) -
            4 * spline_cube(value - 1)) / 6;
}


namespace
{

struct BicubicPrecalc
{
    double weight[4];
    int offset[4];
};

void ResampleBicubicPrecalc(wxVector<BicubicPrecalc> &aWeight, int oldDim)
{
    const int newDim = (int)aWeight.size();
    for ( int dstd = 0; dstd < newDim; dstd++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const double srcpixd = static_cast<double>(dstd * oldDim) / newDim;
        const double dd = srcpixd - static_cast<int>(srcpixd);

        BicubicPrecalc &precalc = aWeight[dstd];

        for ( int k = -1; k <= 2; k++ )
        {
            precalc.offset[k + 1] = srcpixd + k < 0.0
                ? 0
                : srcpixd + k >= oldDim
                    ? oldDim - 1
                    : static_cast<int>(srcpixd + k);

            precalc.weight[k + 1] = spline_weight(k - dd);
        }
    }
}

} // anonymous namespace

// This is the bicubic resampling algorithm
int ResampleBicubic(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    // This function implements a Bicubic B-Spline algorithm for resampling.
    // This method is certainly a little slower than wxImage's default pixel
    // replication method, however for most reasonably sized images not being
    // upsampled too much on a fairly average CPU this difference is hardly
    // noticeable and the results are far more pleasing to look at.
    //
    // This particular bicubic algorithm does pixel weighting according to a
    // B-Spline that basically implements a Gaussian bell-like weighting
    // kernel. Because of this method the results may appear a bit blurry when
    // upsampling by large factors.  This is basically because a slight
    // gaussian blur is being performed to get the smooth look of the upsampled
    // image.

    // Edge pixels: 3-4 possible solutions
    // - (Wrap/tile) Wrap the image, take the color value from the opposite
    // side of the image.
    // - (Mirror)    Duplicate edge pixels, so that pixel at coordinate (2, n),
    // where n is nonpositive, will have the value of (2, 1).
    // - (Ignore)    Simply ignore the edge pixels and apply the kernel only to
    // pixels which do have all neighbours.
    // - (Clamp)     Choose the nearest pixel along the border. This takes the
    // border pixels and extends them out to infinity.
    //
    // NOTE: below the y_offset and x_offset variables are being set for edge
    // pixels using the "Mirror" method mentioned above


    const unsigned char* src_data = source_data;
    unsigned char* dst_data = target_data;
   
    // Precalculate weights
    wxVector<BicubicPrecalc> vPrecalcs(height);
    wxVector<BicubicPrecalc> hPrecalcs(width);

    ResampleBicubicPrecalc(vPrecalcs, old_height);
    ResampleBicubicPrecalc(hPrecalcs, old_width);

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BicubicPrecalc& vPrecalc = vPrecalcs[dsty];

        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BicubicPrecalc& hPrecalc = hPrecalcs[dstx];

            // Sums for each color channel
            double sum_r = 0, sum_g = 0, sum_b = 0;

            // Here we actually determine the RGBA values for the destination pixel
            for ( int k = -1; k <= 2; k++ )
            {
                // Y offset
                const int y_offset = vPrecalc.offset[k + 1];

                // Loop across the X axis
                for ( int i = -1; i <= 2; i++ )
                {
                    // X offset
                    const int x_offset = hPrecalc.offset[i + 1];

                    // Calculate the exact position where the source data
                    // should be pulled from based on the x_offset and y_offset
                    int src_pixel_index = y_offset*old_width + x_offset;

                    // Calculate the weight for the specified pixel according
                    // to the bicubic b-spline kernel we're using for
                    // interpolation
                    const double
                        pixel_weight = vPrecalc.weight[k + 1] * hPrecalc.weight[i + 1];

                    // Create a sum of all velues for each color channel
                    // adjusted for the pixel's calculated weight
                    sum_r += src_data[src_pixel_index * 3 + 0] * pixel_weight;
                    sum_g += src_data[src_pixel_index * 3 + 1] * pixel_weight;
                    sum_b += src_data[src_pixel_index * 3 + 2] * pixel_weight;
                }
            }

            // Put the data into the destination image.  The summed values are
            // of double data type and are rounded here for accuracy
            dst_data[0] = (unsigned char)(sum_r + 0.5);
            dst_data[1] = (unsigned char)(sum_g + 0.5);
            dst_data[2] = (unsigned char)(sum_b + 0.5);
            dst_data += 3;
        }
    }

    return 0;
}

int ResampleBicubic16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    // This function implements a Bicubic B-Spline algorithm for resampling.
    // This method is certainly a little slower than wxImage's default pixel
    // replication method, however for most reasonably sized images not being
    // upsampled too much on a fairly average CPU this difference is hardly
    // noticeable and the results are far more pleasing to look at.
    //
    // This particular bicubic algorithm does pixel weighting according to a
    // B-Spline that basically implements a Gaussian bell-like weighting
    // kernel. Because of this method the results may appear a bit blurry when
    // upsampling by large factors.  This is basically because a slight
    // gaussian blur is being performed to get the smooth look of the upsampled
    // image.

    // Edge pixels: 3-4 possible solutions
    // - (Wrap/tile) Wrap the image, take the color value from the opposite
    // side of the image.
    // - (Mirror)    Duplicate edge pixels, so that pixel at coordinate (2, n),
    // where n is nonpositive, will have the value of (2, 1).
    // - (Ignore)    Simply ignore the edge pixels and apply the kernel only to
    // pixels which do have all neighbours.
    // - (Clamp)     Choose the nearest pixel along the border. This takes the
    // border pixels and extends them out to infinity.
    //
    // NOTE: below the y_offset and x_offset variables are being set for edge
    // pixels using the "Mirror" method mentioned above


    const unsigned short* src_data = (unsigned short*)source_data;
    unsigned short* dst_data = (unsigned short*)target_data;
   
    // Precalculate weights
    wxVector<BicubicPrecalc> vPrecalcs(height);
    wxVector<BicubicPrecalc> hPrecalcs(width);

    ResampleBicubicPrecalc(vPrecalcs, old_height);
    ResampleBicubicPrecalc(hPrecalcs, old_width);

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BicubicPrecalc& vPrecalc = vPrecalcs[dsty];

        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BicubicPrecalc& hPrecalc = hPrecalcs[dstx];

            // Sums for each color channel
            double sum_r = 0, sum_g = 0, sum_b = 0 ;

            // Here we actually determine the RGBA values for the destination pixel
            for ( int k = -1; k <= 2; k++ )
            {
                // Y offset
                const int y_offset = vPrecalc.offset[k + 1];

                // Loop across the X axis
                for ( int i = -1; i <= 2; i++ )
                {
                    // X offset
                    const int x_offset = hPrecalc.offset[i + 1];

                    // Calculate the exact position where the source data
                    // should be pulled from based on the x_offset and y_offset
                    int src_pixel_index = y_offset*old_width + x_offset;

                    // Calculate the weight for the specified pixel according
                    // to the bicubic b-spline kernel we're using for
                    // interpolation
                    const double
                        pixel_weight = vPrecalc.weight[k + 1] * hPrecalc.weight[i + 1];

                    // Create a sum of all velues for each color channel
                    // adjusted for the pixel's calculated weight
                    sum_r += src_data[src_pixel_index * 3 + 0] * pixel_weight;
                    sum_g += src_data[src_pixel_index * 3 + 1] * pixel_weight;
                    sum_b += src_data[src_pixel_index * 3 + 2] * pixel_weight;
                }
            }

            // Put the data into the destination image.  The summed values are
            // of double data type and are rounded here for accuracy
            dst_data[0] = (unsigned short)(sum_r + 0.5);
            dst_data[1] = (unsigned short)(sum_g + 0.5);
            dst_data[2] = (unsigned short)(sum_b + 0.5);
            dst_data += 3;
        }
    }

    return 0;
}



int ResampleBicubic_gray(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    // This function implements a Bicubic B-Spline algorithm for resampling.
    // This method is certainly a little slower than wxImage's default pixel
    // replication method, however for most reasonably sized images not being
    // upsampled too much on a fairly average CPU this difference is hardly
    // noticeable and the results are far more pleasing to look at.
    //
    // This particular bicubic algorithm does pixel weighting according to a
    // B-Spline that basically implements a Gaussian bell-like weighting
    // kernel. Because of this method the results may appear a bit blurry when
    // upsampling by large factors.  This is basically because a slight
    // gaussian blur is being performed to get the smooth look of the upsampled
    // image.

    // Edge pixels: 3-4 possible solutions
    // - (Wrap/tile) Wrap the image, take the color value from the opposite
    // side of the image.
    // - (Mirror)    Duplicate edge pixels, so that pixel at coordinate (2, n),
    // where n is nonpositive, will have the value of (2, 1).
    // - (Ignore)    Simply ignore the edge pixels and apply the kernel only to
    // pixels which do have all neighbours.
    // - (Clamp)     Choose the nearest pixel along the border. This takes the
    // border pixels and extends them out to infinity.
    //
    // NOTE: below the y_offset and x_offset variables are being set for edge
    // pixels using the "Mirror" method mentioned above


    const unsigned char* src_data = source_data;
    unsigned char* dst_data = target_data;
   
    // Precalculate weights
    wxVector<BicubicPrecalc> vPrecalcs(height);
    wxVector<BicubicPrecalc> hPrecalcs(width);

    ResampleBicubicPrecalc(vPrecalcs, old_height);
    ResampleBicubicPrecalc(hPrecalcs, old_width);

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BicubicPrecalc& vPrecalc = vPrecalcs[dsty];

        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BicubicPrecalc& hPrecalc = hPrecalcs[dstx];

            // Sums for each color channel
            double sum_g = 0;

            // Here we actually determine the RGBA values for the destination pixel
            for ( int k = -1; k <= 2; k++ )
            {
                // Y offset
                const int y_offset = vPrecalc.offset[k + 1];

                // Loop across the X axis
                for ( int i = -1; i <= 2; i++ )
                {
                    // X offset
                    const int x_offset = hPrecalc.offset[i + 1];

                    // Calculate the exact position where the source data
                    // should be pulled from based on the x_offset and y_offset
                    int src_pixel_index = y_offset*old_width + x_offset;

                    // Calculate the weight for the specified pixel according
                    // to the bicubic b-spline kernel we're using for
                    // interpolation
                    const double
                        pixel_weight = vPrecalc.weight[k + 1] * hPrecalc.weight[i + 1];

                    // Create a sum of all velues for each color channel
                    // adjusted for the pixel's calculated weight
                    sum_g += src_data[src_pixel_index] * pixel_weight;
                   
                }
            }

            // Put the data into the destination image.  The summed values are
            // of double data type and are rounded here for accuracy
            dst_data[0] = (unsigned char)(sum_g + 0.5);           
            dst_data += 1;
        }
    }

    return 0;
}

int ResampleBicubic_gray16(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
    // This function implements a Bicubic B-Spline algorithm for resampling.
    // This method is certainly a little slower than wxImage's default pixel
    // replication method, however for most reasonably sized images not being
    // upsampled too much on a fairly average CPU this difference is hardly
    // noticeable and the results are far more pleasing to look at.
    //
    // This particular bicubic algorithm does pixel weighting according to a
    // B-Spline that basically implements a Gaussian bell-like weighting
    // kernel. Because of this method the results may appear a bit blurry when
    // upsampling by large factors.  This is basically because a slight
    // gaussian blur is being performed to get the smooth look of the upsampled
    // image.

    // Edge pixels: 3-4 possible solutions
    // - (Wrap/tile) Wrap the image, take the color value from the opposite
    // side of the image.
    // - (Mirror)    Duplicate edge pixels, so that pixel at coordinate (2, n),
    // where n is nonpositive, will have the value of (2, 1).
    // - (Ignore)    Simply ignore the edge pixels and apply the kernel only to
    // pixels which do have all neighbours.
    // - (Clamp)     Choose the nearest pixel along the border. This takes the
    // border pixels and extends them out to infinity.
    //
    // NOTE: below the y_offset and x_offset variables are being set for edge
    // pixels using the "Mirror" method mentioned above


    const unsigned short* src_data = (unsigned short*)source_data;
    unsigned short* dst_data = (unsigned short*)target_data;
   
    // Precalculate weights
    wxVector<BicubicPrecalc> vPrecalcs(height);
    wxVector<BicubicPrecalc> hPrecalcs(width);

    ResampleBicubicPrecalc(vPrecalcs, old_height);
    ResampleBicubicPrecalc(hPrecalcs, old_width);

    for ( int dsty = 0; dsty < height; dsty++ )
    {
        // We need to calculate the source pixel to interpolate from - Y-axis
        const BicubicPrecalc& vPrecalc = vPrecalcs[dsty];

        for ( int dstx = 0; dstx < width; dstx++ )
        {
            // X-axis of pixel to interpolate from
            const BicubicPrecalc& hPrecalc = hPrecalcs[dstx];

            // Sums for each color channel
            double sum_g = 0;

            // Here we actually determine the RGBA values for the destination pixel
            for ( int k = -1; k <= 2; k++ )
            {
                // Y offset
                const int y_offset = vPrecalc.offset[k + 1];

                // Loop across the X axis
                for ( int i = -1; i <= 2; i++ )
                {
                    // X offset
                    const int x_offset = hPrecalc.offset[i + 1];

                    // Calculate the exact position where the source data
                    // should be pulled from based on the x_offset and y_offset
                    int src_pixel_index = y_offset*old_width + x_offset;

                    // Calculate the weight for the specified pixel according
                    // to the bicubic b-spline kernel we're using for
                    // interpolation
                    const double
                        pixel_weight = vPrecalc.weight[k + 1] * hPrecalc.weight[i + 1];

                    // Create a sum of all velues for each color channel
                    // adjusted for the pixel's calculated weight
                    sum_g += src_data[src_pixel_index] * pixel_weight;
                   
                }
            }

            // Put the data into the destination image.  The summed values are
            // of double data type and are rounded here for accuracy
            dst_data[0] = (unsigned short)(sum_g + 0.5);           
            dst_data += 1;
        }
    }

    return 0;
}

int bw2gray(int w, int h, unsigned char* bw, unsigned char* gray)
{
	unsigned char *in, *out, ch;
	int i, j, k, w8 = w/8;

	in = bw;
	out = gray;

	for(i=0; i<h; i++) {	
		for(j=0; j<w8; j++) {
			ch = *in;
			for(k=0; k<8; k++) 
				out[7-k] = (ch&(0x01<<k))? 255: 0;
			out+=8;
			in++;
		}
	}

	return 0;
}

int gray2bw(int w, int h, unsigned char* gray, unsigned char* bw)
{
	unsigned char *in, *out, ch;
	int i, j, k, w8 = w/8;

	in = gray;
	out = bw;

	for(i=0; i<h; i++) {	
		for(j=0; j<w8; j++) {
			ch = 0;
			for(k=0; k<8; k++) 
				if(in[7-k]>128) ch|=(0x01<<k);
			*out = ch;
			in+=8;
			out++;
		}
	}
	
	return 0;
}

int resampleNearest_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
	unsigned char *gray_src, *gray_target;
	gray_src = (unsigned char*)malloc(old_width*old_height);
	gray_target = (unsigned char*)malloc(width*height);
	bw2gray(old_width, old_height, source_data, gray_src);
	resampleNearest_gray(width, height, gray_target, old_width, old_height, gray_src);
	gray2bw(width, height, gray_target, target_data);
	free(gray_src);
	free(gray_target);
	
	return 0;
}

int ResampleBox_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
	unsigned char *gray_src, *gray_target;
	gray_src = (unsigned char*)malloc(old_width*old_height);
	gray_target = (unsigned char*)malloc(width*height);
	bw2gray(old_width, old_height, source_data, gray_src);

	ResampleBox_gray(width, height, gray_target, old_width, old_height, gray_src);

	gray2bw(width, height, gray_target, target_data);
	free(gray_src);
	free(gray_target);
	return 0;
}

int ResampleBilinear_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
	unsigned char *gray_src, *gray_target;
	gray_src = (unsigned char*)malloc(old_width*old_height);
	gray_target = (unsigned char*)malloc(width*height);
	bw2gray(old_width, old_height, source_data, gray_src);

	ResampleBilinear_gray(width, height, gray_target, old_width, old_height, gray_src);

	gray2bw(width, height, gray_target, target_data);
	free(gray_src);
	free(gray_target);
	return 0;
}

int ResampleBicubic_bw(int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data)
{
DB_LINE
	unsigned char *gray_src, *gray_target;
	gray_src = (unsigned char*)malloc(old_width*old_height);
	gray_target = (unsigned char*)malloc(width*height);
	bw2gray(old_width, old_height, source_data, gray_src);

	ResampleBicubic_gray(width, height, gray_target, old_width, old_height, gray_src);

	gray2bw(old_width, old_height, gray_target, target_data);
	free(gray_src);
	free(gray_target);
	return 0;
}

int resamplePartialInit(ResampleControlBlock* rscb, int width, int height, unsigned char* target_data, int old_width, int old_height, unsigned char* source_data, int bitsPerPixel, int resample_type)
{
	resample_type = resample_type;
	rscb->current_y = 0;
	rscb->current_y_old = 0;
	rscb->available_y_old = 0;
	rscb->current_width = width;
	rscb->current_height = height;
	rscb->current_bitsPerPixel = bitsPerPixel;
	rscb->current_old_width = old_width;
	rscb->current_old_height = old_height;
	rscb->source_data = source_data;
	rscb->destination_data = target_data;

	//printf("w h ow oh %d %d %d %d %d\n", current_width,current_height,current_old_width,current_old_height, current_bitsPerPixel);
	return 0;
}

int resamplePartialRead(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end)
{
	is_end = is_end;
	rscb->available_y_old = valid_src_line<<14; //one unit is 16384, 14bit.
	switch(rscb->current_bitsPerPixel) {
	case 1:
		//(*f1[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 8:
		resampleNearestPartial_gray(rscb->current_width, rscb->current_height, rscb->destination_data, 
									rscb->current_old_width, rscb->current_old_height, rscb->source_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 16:
		resampleNearestPartial_gray16(rscb->current_width, rscb->current_height, rscb->destination_data, 
									rscb->current_old_width, rscb->current_old_height, rscb->source_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 24:
		resampleNearestPartial(rscb->current_width, rscb->current_height, rscb->destination_data, 
									rscb->current_old_width, rscb->current_old_height, rscb->source_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 48:
		resampleNearestPartial16(rscb->current_width, rscb->current_height, rscb->destination_data, 
									rscb->current_old_width, rscb->current_old_height, rscb->source_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	}
	*valid_dst_line = rscb->current_y;
	return 0;
}

int resamplePartialRead2(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end, queue<unsigned char*> & src_Q, queue<unsigned char*> & dst_Q)
{
	is_end = is_end;
	rscb->available_y_old = valid_src_line<<14; //one unit is 16384, 14bit.
	//printf("valid_src_line %d rscb->available_y_old %d\n",valid_src_line, rscb->available_y_old);
	switch(rscb->current_bitsPerPixel) {
	case 1:
		//(*f1[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 8:
		resampleNearest2Partial_gray(rscb->current_width, rscb->current_height, dst_Q, 
									rscb->current_old_width, rscb->current_old_height, src_Q,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 16:
		resampleNearest2Partial_gray16(rscb->current_width, rscb->current_height, dst_Q, 
									rscb->current_old_width, rscb->current_old_height, src_Q,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 24:
		resampleNearest2Partial(rscb->current_width, rscb->current_height, dst_Q, 
									rscb->current_old_width, rscb->current_old_height, src_Q,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 48:
		resampleNearest2Partial16(rscb->current_width, rscb->current_height, dst_Q, 
									rscb->current_old_width, rscb->current_old_height, src_Q,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	}
	*valid_dst_line = rscb->current_y;
	return 0;
}

int copy2(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end, queue<unsigned char*> & src_Q, queue<unsigned char*> & dst_Q)
{
	rscb = rscb;
	is_end = is_end;
	//rscb->available_y_old = valid_src_line<<14; //one unit is 65536, 16bit.
	*valid_dst_line = valid_src_line;
	//printf("valid_src_line %d rscb->available_y_old %d\n",valid_src_line, rscb->available_y_old);

	int i;
	int size = (int)src_Q.size();
//	unsigned char*p;
	for(i=0; i<size; i++) {
		dst_Q.push(src_Q.front());
		src_Q.pop();
	}

	return 0;
}

int resamplePartialRead3(ResampleControlBlock* rscb, unsigned long* valid_dst_line, int valid_src_line, int is_end, unsigned char* srouce_data, unsigned char* destination_data)
{
	is_end = is_end;
	rscb->available_y_old = valid_src_line<<14; //one unit is 65536, 16bit.
	switch(rscb->current_bitsPerPixel) {
	case 1:
		//(*f1[resample_type])(width, height, target_data, old_width, old_height, source_data);
		break;
	case 8:
		resampleNearest3Partial_gray(rscb->current_width, rscb->current_height, destination_data, 
									rscb->current_old_width, rscb->current_old_height, srouce_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 16:
		resampleNearest3Partial_gray16(rscb->current_width, rscb->current_height, destination_data, 
									rscb->current_old_width, rscb->current_old_height, srouce_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 24:
		resampleNearest3Partial(rscb->current_width, rscb->current_height, destination_data, 
									rscb->current_old_width, rscb->current_old_height, srouce_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	case 48:
		resampleNearest3Partial16(rscb->current_width, rscb->current_height, destination_data, 
									rscb->current_old_width, rscb->current_old_height, srouce_data,
									&rscb->current_y, &rscb->current_y_old, &rscb->available_y_old);
		break;
	}
	*valid_dst_line = rscb->current_y;
	return 0;
}

