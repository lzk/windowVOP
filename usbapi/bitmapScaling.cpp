#include <windows.h>
#include "bitmapScaling.h"

static void ScalineLine(
        unsigned char* pLine,
        int            nWidth,
        int            nBitCount,
        double         rate
        );

static void ScalingBlock(
        unsigned char* pBlock,
        int            nWidth,
        int            nHeight,
        int            nBitCount,
        double         rate,
        unsigned char* pOutput 
        );


/*****************************************************************************
 * Function
 *      Scaling bitmap.
 *
 * Parameters
 *      pszInput [in] : Full path of input bitmap file.
 *      pszOutput[in] : Full path of output bitmap file.
 *      rate     [in] : Scaling rate.
 *
 * Return value
 *      None.
 *
 * Remarks
 *      Only support scaling rate between ( 0, 1 ]. Ignore fraction part of
 *      width and height; 
 *      NOTE: DON'T SUPPORT 1 BIT BITMAP!
 *
*****************************************************************************/
extern void ScalingBitmap( 
        const wchar_t* pszInput,
        const wchar_t* pszOutput,
        double rate 
        )
{
    if ( 1-rate < 0.0001 )
    {
        CopyFile( pszInput, pszOutput, FALSE );
    }
    else if ( rate > 0 )
    {
        HANDLE hInput = CreateFile( pszInput, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL); 
        if ( INVALID_HANDLE_VALUE != hInput )
        {
            DWORD dwActualUsed = 0;
            BITMAPFILEHEADER stFileHeader;
            ReadFile( hInput, &stFileHeader, sizeof(stFileHeader), &dwActualUsed, NULL);

            if ( stFileHeader.bfType == ((WORD)('M'<< 8)|'B') )
            {
                HANDLE hOutput = CreateFile( pszOutput, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, 0, NULL); 
                if ( INVALID_HANDLE_VALUE != hOutput )
                {
                    // copy file header
                    char* pHeadSrc = new char[stFileHeader.bfOffBits];
                    char* pHeadDst = new char[stFileHeader.bfOffBits];

                    BITMAPINFO* pBmpInfoSrc = reinterpret_cast<BITMAPINFO*>(pHeadSrc+sizeof( BITMAPFILEHEADER ));
                    BITMAPINFO* pBmpInfoDst = reinterpret_cast<BITMAPINFO*>(pHeadDst+sizeof( BITMAPFILEHEADER ));
                    BITMAPFILEHEADER* pBmpHeaderDst = reinterpret_cast<BITMAPFILEHEADER*>(pHeadDst);

                    SetFilePointer( hInput, 0, NULL, FILE_BEGIN );
                    ReadFile( hInput, pHeadSrc, stFileHeader.bfOffBits, &dwActualUsed, NULL);

                    memcpy( pHeadDst, pHeadSrc, stFileHeader.bfOffBits );

                    pBmpInfoDst->bmiHeader.biWidth         = static_cast<LONG>(rate*pBmpInfoSrc->bmiHeader.biWidth        );
                    pBmpInfoDst->bmiHeader.biHeight        = static_cast<LONG>(rate*pBmpInfoSrc->bmiHeader.biHeight       );
                    pBmpInfoDst->bmiHeader.biXPelsPerMeter = static_cast<LONG>(rate*pBmpInfoSrc->bmiHeader.biXPelsPerMeter);
                    pBmpInfoDst->bmiHeader.biYPelsPerMeter = static_cast<LONG>(rate*pBmpInfoSrc->bmiHeader.biYPelsPerMeter);

                    int cbStride = ( pBmpInfoSrc->bmiHeader.biWidth * pBmpInfoSrc->bmiHeader.biBitCount + 31 )/32 * 4; 
                    int cbStrideScaled = ( pBmpInfoDst->bmiHeader.biWidth * pBmpInfoSrc->bmiHeader.biBitCount + 31 )/32 * 4; 

                    pBmpInfoDst->bmiHeader.biSizeImage = cbStrideScaled * pBmpInfoDst->bmiHeader.biHeight;
                    pBmpHeaderDst->bfSize = pBmpInfoDst->bmiHeader.biSizeImage + pBmpHeaderDst->bfOffBits;

                    WriteFile( hOutput , pHeadDst, stFileHeader.bfOffBits, &dwActualUsed , NULL);

                    int nBlockLine = static_cast<int>(1.0/rate)+1;  // max line number of one block

                    unsigned char* pStrideBlock  = new unsigned char[cbStride*nBlockLine];
                    unsigned char* pStrideScaled = new unsigned char[cbStride];

                    int nCntBlkRow = 0; // counter for lines of block
                    int nCntDstRow = 0; // counter for lines of scaling bitmap

                    for ( int i=0; i<pBmpInfoSrc->bmiHeader.biHeight; i++ )
                    {
                        ReadFile( hInput, pStrideBlock+nCntBlkRow*cbStride, cbStride, &dwActualUsed, NULL);

                        nCntBlkRow++;

                        int n = static_cast<int>(i*rate);
                        if ( nCntDstRow != n || i == pBmpInfoSrc->bmiHeader.biHeight-1 )
                        {
                            ScalingBlock(
                                    pStrideBlock,
                                    pBmpInfoSrc->bmiHeader.biWidth,
                                    nCntBlkRow,
                                    pBmpInfoSrc->bmiHeader.biBitCount,
                                    rate,
                                    pStrideScaled
                                    );

                            WriteFile( hOutput , pStrideScaled, cbStrideScaled, &dwActualUsed , NULL);
                            nCntBlkRow = 0;
                            nCntDstRow = n;
                        }
                    }

                    // free buffer
                    if ( pStrideBlock )
                    {
                        delete[] pStrideBlock;
                        pStrideBlock = NULL;
                    }

                    if ( pStrideScaled )
                    {
                        delete[] pStrideScaled;
                        pStrideScaled = NULL;
                    }

                    if ( pHeadSrc )
                    {
                        delete[] pHeadSrc;
                        pHeadSrc = NULL;
                    }

                    if ( pHeadDst )
                    {
                        delete[] pHeadDst;
                        pHeadDst = NULL;
                    }

                }

                CloseHandle( hOutput );
            }

        }

        CloseHandle( hInput );


    }
}

/*****************************************************************************
 * Function
 *      Scaling several lines.
 *
 * Parameters
 *      pBlock    [in] : 
 *      nWidth    [in] : Pixel numbers in width.
 *      nHeight   [in] : Pixel numbers in height.
 *      nBitCount [in] : Bits pers pixel, support value:  8, 24
 *      rate      [in] : Scaling rate.
 *      pOutput   [out]: Store the scaling result.
 *
 * Return value
 *      None.
*****************************************************************************/
static void ScalingBlock(
        unsigned char* pBlock,
        int            nWidth,
        int            nHeight,
        int            nBitCount,
        double         rate,
        unsigned char* pOutput 
        )
{
    int cbBytePerLine = 0;

    if ( 8 == nBitCount )
        cbBytePerLine = nWidth;
    else if ( 24 == nBitCount )
        cbBytePerLine = nWidth*3;

    if ( 8 == nBitCount )
    {
        for ( int i=0; i<cbBytePerLine; i++ ) 
        {
            int nSum = 0;
            for ( int j=0; j<nHeight; j++ ) 
            {
                nSum += *(pBlock+cbBytePerLine*j+i); 
            }

            pOutput[i] = static_cast<unsigned char>(nSum/nHeight);
        }
    }
    else if (24 == nBitCount )
    {
        memcpy( pOutput, pBlock, cbBytePerLine );   // don't destructure RGB value
    }

    ScalineLine( pOutput, nWidth, nBitCount, rate );
}

/*****************************************************************************
 * Function
 *      Scaling one line.
 *
 * Parameters
 *      pLine     [in,out] : The byte array need to be scaled.
 *      nWidth    [in]     : With of data in pixel.
 *      nBitCount [in]     : Bits pers pixel, support value:  8, 24.
 *      rate      [in]     : 
 *
 * Return value
 *      None.
*****************************************************************************/
static void ScalineLine(
        unsigned char* pLine,
        int            nWidth,
        int            nBitCount,
        double         rate
        )
{
    int nSum = 0;

    int nCntBlkCol = 0; // counter for columns of block
    int nCntDstCol = 0; // counter for columns of scaling data

    for ( int i=0; i<nWidth; i++ )
    {
        nCntBlkCol++;
        if ( 8 == nBitCount )
        {
            nSum += pLine[i];
        }
        else if ( 24 == nBitCount )
        {
            // do nothing
        }

        // calculate range [n, n+1)
        int n = static_cast<int>(i*rate);
        if ( nCntDstCol != n || i == nWidth-1 )
        {
            if ( 8 == nBitCount )
            {
                pLine[nCntDstCol] = nSum/nCntBlkCol;
            }
            else if ( 24 == nBitCount )
            {
                pLine[3*nCntDstCol]   = pLine[3*i];
                pLine[3*nCntDstCol+1] = pLine[3*i+1];
                pLine[3*nCntDstCol+2] = pLine[3*i+2];
            }

            nSum = 0;
            nCntBlkCol = 0;
            nCntDstCol = n;
        }
    }
}
