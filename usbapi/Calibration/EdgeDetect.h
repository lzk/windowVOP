/******************************************************
	File name: 
		EdgeDetect.h

	Description: 
		Canopus sheetfeed image-A side and iamge-B side is different in egdes.
		so we have different methods to detect the edges.

		- The input raw image's requirement h>=600, 24bit.

		- The leading edge detect EdgeDetectColor8() api will detect the leading 
			in x = 1/4 w, 1/2 w and 3/4 w then average them.

		- SideA-leading and SideB-trailing profile have a puls due to the led light. 
			we will find the max white to be the edge.

		- SideA-trailing and SideB-leading will find the edge from inside to outside.
			and monitor the profile from white to dark.

		- For left and right edges, we detect from outside to inside.
			monitor the profile from dark to white.
	
******************************************************/
#ifndef _EDGE_DETECT_
#define _EDGE_DETECT_

#define FLAG_SHEETFEED_A 0
#define FLAG_SHEETFEED_B 1

int EdgeDetectColor8(unsigned char*image, int width, int height, int*leadingInPixel, int *leftEdgeInPixel, int *rightEdgeInPixel, int isSideB);
int EdgeDetectColor8Trailing(unsigned char*image, int width, int height, int*trailingInPixel, int isSideB);

int EdgeDetect8(unsigned char*image, int width, int height, int*leadingInPixel, int *leftEdgeInPixel, int *rightEdgeInPixel, int depth, int isSideB);
int EdgeDetect8Trailing(unsigned char*image, int width, int height, int*trailingInPixel, int depth, int isSideB);

#endif