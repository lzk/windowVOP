/************************************************************************
	File name: 
		EdgeDetect.c

	Description: 
		check the EdgeDetect.h for detail.
	
************************************************************************/

#ifdef WIN32
#pragma pack(1)
#endif

#define _CRT_SECURE_NO_DEPRECATE //stop warning C4996 about fopen.

#include <stdio.h>
#include <math.h>
#include <stdlib.h>     /* malloc, free, rand */
//#include <iostream>
#include "EdgeDetect.h"

#define SCAN_WIDTH 900

//#define DEBUG_OUT //To generate the csv file for edge profile analysis.

float pixel[SCAN_WIDTH]={0};

double _standard_deviation(float data[], int n, double*mean_out)
{
    double mean=0.0, sum_deviation=0.0;
    int i;
    for(i=0; i<n;++i)
    {
        mean+=data[i];
    }
    mean=mean/n;
    for(i=0; i<n;++i)
    sum_deviation+=(data[i]-mean)*(data[i]-mean);
	*mean_out = mean;
    return (double)sqrt(sum_deviation/n);           
}

int _detectDark(float*sample, int length)
{
	int start=100;
	int i=0;
	double mean, std;
	std = _standard_deviation(sample, start, &mean);

	for(i=start; i<length; i++) {
		mean = mean*0.7 + sample[i]*0.3;
		if(sample[i]< mean-3*std) {
			break;
		}
	}
	return length-i;
}

int _detectWhite(float*sample, int length)
{
	int start=5;
	int i=0;
	double mean, std;
	std = _standard_deviation(sample, start, &mean);

	for(i=start; i<length; i++) {
		mean = mean*0.7 + sample[i]*0.3;
		if(sample[i]> mean+3*std) {
			break;
		}
	}
	return i;
}

int _detectMaxWhite(float*sample, int length)
{
	int i, max_i;
	float max=0;

	for(i=0; i<length; i++) {
		if(sample[i]>max) {
			max = sample[i];
			max_i = i;
		}
	}
	return length - max_i;
}

int _detectMaxDiff(float*sample, int length)
{
	int i, j;
	int* diff, max=0, maxIndex=0, avg_mask_size=17;
	float *avg;
	diff = (int*)malloc(sizeof(int)*length);
	avg = (float*)malloc(sizeof(float)*length);

	//smooth filter to remove noise
	for(i=avg_mask_size/2; i<length-(avg_mask_size/2+1); i++){
		avg[i] = 0;
		for(j=-avg_mask_size/2; j<avg_mask_size/2+1; j++) {
			avg[i] += sample[i+j];
		}
		avg[i] /= avg_mask_size;
	}

	//find max diff
	for(i=avg_mask_size/2+1; i<length-(avg_mask_size/2+2); i++) {
		diff[i] = (int)abs(avg[i+1]-avg[i-1]);
		if(diff[i]>max) {
			max = diff[i];
			maxIndex = i;
			//printf("max %d i %d\n", max, maxIndex);
		}
	}

	free(diff);
	free(avg);
	return length-maxIndex;
}

int _detectMedian(float*sample, int length)
{
	int i;
	float *avg;
	float level1=0, level2=0, median;
	avg = (float*)malloc(sizeof(float)*length);

	for(i=1; i<length-1; i++){
		avg[i] = (sample[i-1]+sample[i]+sample[i+1])/3;
	}

	for(i=0; i<64; i++) {
		level1 += avg[i+1];
		level2 += avg[length-2-i];
	}
	level1 /=64; //paper
	level2 /=64; //background
	median = (level1+level2)/2;
	printf("median %f\n",median);

	for(i=1; i<length-1; i++){
		if(avg[length-2-i] > median) {
			break;
		}
	}
	free(avg);
	return i;
}

int _detectEdgeByThreshold(float*sample, int length)
{	
	int i;
	float avg1, avg2;
	float sum;
	float threshold;
	
	sum=0;
	for(i=0; i<100; i++){
		sum += sample[i];
	}
	avg1 = sum/100;

	sum=0;
	for(i=0; i<100; i++){
		sum += sample[length-1-i];
	}
	avg2 = sum/100;

	threshold = (avg1+avg2)/2;

	//we always search from black to white.
	//so we need to know the direction.
	if(avg1>avg2) {
		//white to black input sample case.
		for(i=0; i<length; i++) {
			if(sample[length-1-i]>threshold) {
				break;
			}
		}
	}
	else {
		//black to white input sample case.
		for(i=0; i<length; i++) {
			if(sample[i]>threshold) {
				break;
			}
		}
	}
	return i;
}

//old canopus with brighter background, the ratio is 0.9
//new canopus with darker background, the ratio is 0.4
int _isDarkerBackground(float*sample, int length)
{

	int i, sum1=0, sum2=0;
	float avg1=0, avg2=0, ratio;


	for(i=0; i<64; i++) {
		sum1 += (float)sample[i];
		sum2 += (float)sample[length-1-i];
	}
	avg1 = (float)(sum1>>6);
	avg2 = (float)(sum2>>6);
	
	ratio = avg1>avg2? avg2/avg1: avg1/avg2; 
	//printf("ratio %f\n", ratio);
	return ratio<0.65? 1: 0;
}

int _min(int a, int b) {
	return a<b? a:b;
}

int EdgeDetectColor8(unsigned char*image, int width, int height, int*leadingInPixel, int *leftEdgeInPixel, int *rightEdgeInPixel, int isSideB)
{
	int depth = 3;
	int i, j, tmp, p;
	float k;
	int edge_in_pixel;
	int edge[10];
	int edge_index=0;

#ifdef DEBUG_OUT
	char logFileName[30];
	FILE *fin;
	sprintf(logFileName, "EdProfile_%s.csv", isSideB? 'b':'a');
	fin = fopen(logFileName, "wb");
	fprintf(fin, "w %d h %d\n", width, height);
	printf("w %d h %d d %d\n", width, height, depth);
#endif
	
	//front 
	//We detect x=1/4 w, 1/2 w and 3/4 w place. 
	//from inside to outside. Then average 3 detected edges.
	edge_in_pixel=0;
	//for(k=0.25; k<1; k+=0.25) {
	
	for(k=0.385; k<0.41; k+=0.01) { //984/1036/1088 pixel count @ scan sensor 
		#ifdef DEBUG_OUT
		printf("front\n");
		fprintf(fin, "front\n");
		#endif
		for(j=0, i=SCAN_WIDTH-1; i>=0; i--, j++) {
			p = (int)(width*(i+k)*depth+1);
			pixel[j]=image[p];
			#ifdef DEBUG_OUT
			fprintf(fin, "%d,", image[p]);
			#endif
		}
		#ifdef DEBUG_OUT
		fprintf(fin, "\n");
		#endif
		if(_isDarkerBackground(pixel, SCAN_WIDTH)) {
			//tmp = _detectMaxDiff(pixel, SCAN_WIDTH);
			tmp = _detectEdgeByThreshold(pixel, SCAN_WIDTH);
		}
		else {
			if(isSideB) {
				tmp = _detectDark(pixel, SCAN_WIDTH);
			}
			else {
				tmp = _detectMaxWhite(pixel, SCAN_WIDTH);				
			}
		}
		//printf("%f w Leading %d\n", k, tmp);
		edge_in_pixel += tmp;
		edge[edge_index] = tmp;
		edge_index++;
	}
	//*leadingInPixel = edge_in_pixel/3;
	*leadingInPixel = _min(_min(edge[0], edge[1]), edge[2]);
	#ifdef DEBUG_OUT
	fprintf(fin, "edge %d %d %d\n", edge[0], edge[1], edge[2]);
	fprintf(fin, "leadingInPixel %d\n", *leadingInPixel);
	#endif

	//left
	if(leftEdgeInPixel) {
		#ifdef DEBUG_OUT
		printf("left\n");
		fprintf(fin, "left\n");
		#endif
		tmp = width*depth*(height*7/8);
		for(i=0; i<SCAN_WIDTH; i++) {
			p = tmp + i*depth + 1;
			pixel[i]=image[p];
			#ifdef DEBUG_OUT
			fprintf(fin, "%d,", image[p]);
			#endif
		}
		*leftEdgeInPixel = _detectWhite(pixel, SCAN_WIDTH);
		#ifdef DEBUG_OUT
		fprintf(fin, "\n");
		fprintf(fin, "leftEdgeInPixel %d\n", *leftEdgeInPixel);
		#endif
	}
	
	//right
	if(rightEdgeInPixel) {
		#ifdef DEBUG_OUT
		printf("right\n");
		fprintf(fin, "right\n");
		#endif
		tmp = (width*(height*7/8) - 1)*depth;
		for(i=0; i<SCAN_WIDTH; i++) {
			p = (int)(tmp - i*depth + 1);
			pixel[i]=image[p];
			#ifdef DEBUG_OUT
			fprintf(fin, "%d,", image[p]);
			#endif
		}
		*rightEdgeInPixel = _detectWhite(pixel, SCAN_WIDTH);
	}
	

	#ifdef DEBUG_OUT
	fprintf(fin, "\n");
	fclose(fin);
	#endif

	return 0;
}

int EdgeDetectColor8Trailing(unsigned char*image, int width, int height, int*trailingInPixel, int isSideB)
{
	int depth =3;
	int i, j, tmp, p;
	float k;
	int edge_in_pixel;
	int edge[10];
	int edge_index=0;

#ifdef DEBUG_OUT
	char logFileName[30];
	FILE *fin;
	sprintf(logFileName, "EdProfile_%s.csv", isSideB? 'b':'a');
	fin = fopen(logFileName, "wb");
	fseek(fin, 0, SEEK_END);
	//printf("w %d h %d d %d\n", width, height, depth);
#endif
	

	//back
	edge_in_pixel=0;
	
	for(k=0.385; k<0.41; k+=0.01) { //984/1036/1088 pixel count @ scan sensor 
		#ifdef DEBUG_OUT
		printf("back\n");
		fprintf(fin, "back\n");
		#endif
		for(j=0, i=height-SCAN_WIDTH; i<height; i++, j++) {
			p = (int)(width * (i+k) * depth +1);
			pixel[j]=image[p];
			#ifdef DEBUG_OUT
			fprintf(fin, "%d,", image[p]);
			#endif
		}
		#ifdef DEBUG_OUT
		fprintf(fin, "\n");
		#endif
		if(_isDarkerBackground(pixel, SCAN_WIDTH)) {
			//tmp = _detectMaxDiff(pixel, SCAN_WIDTH);
			tmp = _detectEdgeByThreshold(pixel, SCAN_WIDTH);
		}
		else {
			if(isSideB) {
				tmp = _detectMaxWhite(pixel, SCAN_WIDTH);
			}
			else {
				tmp = _detectDark(pixel, SCAN_WIDTH); 
			}
		}
		edge_in_pixel += tmp;
		edge[edge_index] = tmp;
		edge_index++;
		//printf("%f w trailing %d\n", k, tmp);
	}
	//*trailingInPixel = edge_in_pixel/3;
	*trailingInPixel = _min(_min(edge[0], edge[1]), edge[2]);

	#ifdef DEBUG_OUT
	fprintf(fin, "edge %d %d %d\n", edge[0], edge[1], edge[2]);
	fprintf(fin, "trailingInPixel %d\n", *trailingInPixel);
	fprintf(fin, "\n");
	fclose(fin);
	#endif

	return 0;
}