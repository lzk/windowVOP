/*******************************************************************

   Copyright (C), 2006, LiteON

   File name: WiaDevice.cpp

   Author: James Yu   Version: 1.0   Date: 2008-09-23

   Description: 

   History: 
      James Yu  2008-09-23   1.0   build this module
      
*******************************************************************/

#include <windows.h>
#include <stdio.h>
#include <tchar.h>
#include "Scanner.h"
#include <math.h>



//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CScanner::CScanner()
{
	m_hLLD = NULL;

	m_dwTotalLinesRead = 0;
	m_dwTotalLinesToRead = 0;
	m_dwTotalLinesToScan = 0;
	m_dwBytesPerLine = 0;
	m_dwPixelsPerLine = 0;
	m_dwBitsPerPixel = 0;
	m_dwResolution = 0;
	m_dwBytesPerLineFromDevice = 0;

	m_bScannerLocked = false;
	m_bDeviceOpened = false;

	//m_hMutex = NULL;
}

CScanner::~CScanner()
{
	if (m_hLLD != NULL)
	{
		Close();

		::FreeLibrary(m_hLLD);
		m_hLLD = NULL;
	}
}

int CScanner::Initialize(LPTSTR lptszDevicePath)
{
	if (m_hLLD != NULL)
	{
		Close();

		::FreeLibrary(m_hLLD);
		m_hLLD = NULL;
	}
    
	m_hLLD = ::LoadLibrary(lptszDevicePath);
	if (m_hLLD == NULL)
	{
		//OutputDebugString(lptszDevicePath);
		OutputDebugString(L"can not init Devmon interfaces");
		return DEVMON_ERROR_OPEN_FAILED;
	}

	//UsbInitializeMFP = (LPFUNC_UsbInitializeMFP)::GetProcAddress(m_hLLD, "UsbInitializeMFP");
	DevMon_Open = (LPFUNC_DevMon_Open)::GetProcAddress(m_hLLD, "DevMon_Open");    
	DevMon_WriteCommand = (LPFUNC_DevMon_WriteCommand)::GetProcAddress(m_hLLD, "DevMon_WriteCommand");
	DevMon_SetScanParameter = (LPFUNC_DevMon_SetScanParameter)::GetProcAddress(m_hLLD, "DevMon_SetScanParameter");
	DevMon_SetScanParameterAdj = (LPFUNC_DevMon_SetScanParameterAdj)::GetProcAddress(m_hLLD, "DevMon_SetScanParameterAdj");
	DevMon_GetScanParameter = (LPFUNC_DevMon_GetScanParameter)::GetProcAddress(m_hLLD, "DevMon_GetScanParameter");
	DevMon_StartScan = (LPFUNC_DevMon_StartScan)::GetProcAddress(m_hLLD, "DevMon_StartScan");
	DevMon_ReadScanData = (LPFUNC_DevMon_ReadScanData)::GetProcAddress(m_hLLD, "DevMon_ReadScanData");
	DevMon_AbortScan = (LPFUNC_DevMon_AbortScan)::GetProcAddress(m_hLLD, "DevMon_AbortScan");
	DevMon_AbortScanEx = (LPFUNC_DevMon_AbortScanEx)::GetProcAddress(m_hLLD, "DevMon_AbortScanEx");
	DevMon_Close = (LPFUNC_DevMon_Close)::GetProcAddress(m_hLLD, "DevMon_Close");
	//ReadPushButton = (LPFUNC_ReadPushButton)::GetProcAddress(m_hLLD, "ReadPushButton");
	//writeESCCommand = (LPFUNC_writeESCCommand)::GetProcAddress(m_hLLD, "writeESCCommand");
	DevMon_BroadcastMFP = (LPFUNC_DevMon_BroadcastMFP)::GetProcAddress(m_hLLD, "BroadcastMFP");
	DevMon_DetectNetMFP = (LPFUNC_DevMon_DetectNetMFP)::GetProcAddress(m_hLLD, "DetectNetMFP");
	DevMon_LocateMFP = (LPFUNC_DevMon_LocateMFP)::GetProcAddress(m_hLLD, "LocateMFP");

	if (
		DevMon_Open == NULL || 
		DevMon_WriteCommand == NULL || 
		DevMon_SetScanParameter == NULL || 
		DevMon_SetScanParameterAdj == NULL || 
		DevMon_GetScanParameter == NULL || 
		DevMon_StartScan == NULL || 
		DevMon_ReadScanData == NULL || 
		DevMon_AbortScan == NULL || 
		DevMon_AbortScanEx == NULL ||        
		DevMon_Close == NULL ||
		DevMon_BroadcastMFP == NULL ||        
		DevMon_DetectNetMFP == NULL ||
		DevMon_LocateMFP == NULL
	)
	{
		::FreeLibrary(m_hLLD);
		m_hLLD = NULL;

		return DEVMON_ERROR_OPEN_FAILED;
	}
	else
	{
		return DEVMON_STATUS_OK;
	}
}

bool CScanner::FindDevice(bool bLocal, char **ppszDevicePathList)
{
	if (bLocal)
	{
	}
	else
	{
		// char szDevIP[100][64];
		//DevMon_BroadcastMFP(, );
	}

	return false;
}

bool CScanner::FindNetworkDevice(int *DevNum, char **DevName)
{
	return (DEVMON_STATUS_OK == DevMon_BroadcastMFP(DevNum, DevName));
}

int CScanner::Open(LPSTR szDevicePath, const char* szIP, bool bPushScan, int nCapability, int nPushScanFlag )
{
	int nRet, nVersion = 0, nModelCode = 0;

	if (bPushScan)
	{
		nPushScanFlag = 1;
	}

	//char szDevicePath[MAX_PATH];

	if (m_hLLD == NULL)
	{
		OutputDebugString(L"error:m_hLLD = NULL");
		return DEVMON_ERROR;
	}

	if (m_bDeviceOpened)
	{
		Close();
	}

	if ( szIP && szIP[0] )
	{
		// network
	 	nRet = DevMon_DetectNetMFP( "public" );
	 	if (nRet != DEVMON_STATUS_OK)
	 	{
	 		return nRet;
	 	}

		 nRet = DevMon_LocateMFP( szIP );
		 if (nRet != DEVMON_STATUS_OK)
		 {
		 	return nRet;
		 }
	}

	/*CString strDevicePath, strTemp = szDevicePath;
	int index = strTemp.ReverseFind(_T('\\'));
	strDevicePath = strTemp.Right(strTemp.GetLength() - index - 1);
	m_hMutex = ::CreateEvent(NULL, TRUE, FALSE, strDevicePath);
	if (m_hMutex != NULL && ERROR_ALREADY_EXISTS == GetLastError())
	{
		m_bDeviceOpened = false;
		return DEVMON_ERROR_IN_USE;
	}*/
	//memset(szDevicePath, 0, sizeof(szDevicePath));	
	//WideCharToMultiByte(CP_ACP, 0, wszDevicePath, -1, szDevicePath, MAX_PATH, NULL, NULL);
	//OutputDebugStringA(szDevicePath);


	nRet = DevMon_Open(&nVersion, &nCapability, &nPushScanFlag, &nModelCode, szDevicePath);
	if (nRet != DEVMON_STATUS_OK)
	{
		DevMon_Close();

		m_bDeviceOpened = false;
	}
	else
	{
		m_bDeviceOpened = true;
	}

	m_dwTotalLinesRead = 0;
	m_dwTotalLinesToRead = 0;
	m_dwTotalLinesToScan = 0;
	m_dwBytesPerLine = 0;
	m_dwPixelsPerLine = 0;
	m_dwBitsPerPixel = 0;
	m_dwResolution = 0;

	m_dwBytesPerLineFromDevice = 0;

	return nRet;
}

int CScanner::Close()
{
	if (m_hLLD != NULL && m_bDeviceOpened)
	{
		DevMon_Close();
	}

	m_bDeviceOpened = false;

	/*if (m_hMutex != NULL)
	{
		CloseHandle(m_hMutex);
		m_hMutex = NULL;
	}*/

	return DEVMON_STATUS_OK;
}

int CScanner::StartScan()
{
	int nRet;

	if (m_hLLD == NULL || !m_bDeviceOpened)
	{
		// LLD not be loaded
		return DEVMON_ERROR_OPEN_FAILED;
	}

	// Lock
	nRet = LockScanner();
	return nRet;
}

int CScanner::SetScanParameterEx(LPSCANPARAMETER lpScanParam)
{
	int nRet;

	if (m_hLLD == NULL || !m_bDeviceOpened)
	{
		// LLD not be loaded
		return DEVMON_ERROR_OPEN_FAILED;
	}

	// set parameters fro scanning
	nRet = SetScanParameter(lpScanParam);
	if (DEVMON_STATUS_OK != nRet)
	{
		DevMon_AbortScan();         
		UnLockScanner();
		return nRet;
	}

	// get PixelPerLine & TotalLines
	nRet = GetScanParameter(&lpScanParam->PixelPerLine, &lpScanParam->TotalLines);
	if (DEVMON_STATUS_OK != nRet)
	{
		DevMon_AbortScan();         
		UnLockScanner();
		return nRet;
	}

	if (lpScanParam->ScanMode == ScanMode_Mono)
	{
		m_dwBitsPerPixel = 1;
	}
	else if (lpScanParam->ScanMode == ScanMode_Gray)
	{
		m_dwBitsPerPixel = 8;
	}
	else if (lpScanParam->ScanMode == ScanMode_Color)
	{
		m_dwBitsPerPixel = 24;
	}
	else
	{
		// WIA not support
		DevMon_AbortScan();         
		UnLockScanner();

		return DEVMON_ERROR;
	}

	m_dwTotalLinesRead = 0;

	m_dwTotalLinesToRead = lpScanParam->Height * lpScanParam->YRes / 1000; //lpScanParam->TotalLines;
	m_dwTotalLinesToScan = lpScanParam->TotalLines;

	char szText[200];
	sprintf(szText, "Height = %d, YRes = %d", lpScanParam->YRes, lpScanParam->Height);
	//OutputDebugStringA(szText);//*/

	//if (m_dwTotalLinesToRead > lpScanParam->TotalLines)
	//{
	//	/*char szText[200];
	//	sprintf(szText, "DevMon: m_dwTotalLinesToRead = %d, TotalLines(From DevMON) = %d", 
	//	m_dwTotalLinesToRead, lpScanParam->TotalLines);
	//	OutputDebugStringA(szText);*/

	//	m_dwTotalLinesToRead = lpScanParam->TotalLines;
	//	//OutputDebugString(_T("m_dwTotalLinesToRead is larger than TotalLines that get from DevMon"));
	//}

	m_dwPixelsPerLine = lpScanParam->Width * lpScanParam->XRes / 1000; // lpScanParam->PixelPerLine;
	if (m_dwPixelsPerLine > lpScanParam->PixelPerLine)
	{
		/*char szText[200];
		sprintf(szText, "DevMon: m_dwPixelsPerLine = %d, PixelPerLine(From DevMON) = %d", 
		m_dwPixelsPerLine, lpScanParam->PixelPerLine);
		OutputDebugStringA(szText);//*/

		m_dwPixelsPerLine = lpScanParam->PixelPerLine;
		//OutputDebugString(_T("m_dwPixelsPerLine is larger than PixelPerLine that get from DevMon"));
	}

	m_dwBytesPerLine = (m_dwPixelsPerLine * m_dwBitsPerPixel + 31) / 32 * 4;

	m_dwBytesPerLineFromDevice = (lpScanParam->PixelPerLine * m_dwBitsPerPixel+7) / 8;

	m_dwResolution = lpScanParam->XRes;

//	char szText[200];
	//sprintf(szText, "m_dwPixelsPerLine = %d, PixelPerLine = %d", 
	//	m_dwPixelsPerLine, lpScanParam->PixelPerLine);
	//OutputDebugStringA(szText);//*/


	m_dwMarginTopBottom = lpScanParam->MarginTopBottom;
	m_dwMarginLeftRight = lpScanParam->MarginLeftRight;
	m_dwMarginMiddle = lpScanParam->MarginMiddle;

	nRet = DevMon_StartScan();

	return DEVMON_STATUS_OK;
}

int CScanner::ReadBitmapInfo(BYTE *pBuffer, ULONG ulBufferSize, ULONG *pulBytesRead)
{

	// Convert the color format to a count of bits. 
    WORD cClrBits = (WORD)(1 * m_dwBitsPerPixel); 
    if (cClrBits == 1) 
        cClrBits = 1; 
    else if (cClrBits <= 4) 
        cClrBits = 4; 
    else if (cClrBits <= 8) 
        cClrBits = 8; 
    else if (cClrBits <= 16) 
        cClrBits = 16; 
    else if (cClrBits <= 24) 
        cClrBits = 24; 
    else cClrBits = 32; 
	
	// Initialize the fields in the BITMAPINFO structure. 
	BITMAPINFO bmi;
	memset(&bmi, 0, sizeof(BITMAPINFO));

    bmi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER); 
    bmi.bmiHeader.biWidth = m_dwPixelsPerLine; 
    bmi.bmiHeader.biHeight = 0 - m_dwTotalLinesToRead; 
    bmi.bmiHeader.biPlanes = 1; 
    bmi.bmiHeader.biBitCount = (WORD)m_dwBitsPerPixel; 
    if (cClrBits < 24) 
        bmi.bmiHeader.biClrUsed = (1<<cClrBits); 

    // If the bitmap is not compressed, set the BI_RGB flag. 
    bmi.bmiHeader.biCompression = BI_RGB; 
   
    bmi.bmiHeader.biSizeImage = m_dwBytesPerLine * m_dwTotalLinesToRead; 

    // Set biClrImportant to 0, indicating that all of the 
    // device colors are important. 
	bmi.bmiHeader.biClrImportant = 0; 

	bmi.bmiHeader.biXPelsPerMeter = (DWORD)((double)m_dwResolution * 39.37);
	bmi.bmiHeader.biYPelsPerMeter = bmi.bmiHeader.biXPelsPerMeter;//m_dwResolution;
	

	BITMAPFILEHEADER   bmfh;
	memset(&bmfh, 0, sizeof(bmfh));

	bmfh.bfType    = ((WORD) ('M' << 8) | 'B');
	 // Compute the size of the entire file. 
    bmfh.bfSize = (DWORD) (sizeof(BITMAPFILEHEADER) + 
                 bmi.bmiHeader.biSize + bmi.bmiHeader.biClrUsed 
                 * sizeof(RGBQUAD) + bmi.bmiHeader.biSizeImage);     

    // Compute the offset to the array of color indices. 
    bmfh.bfOffBits = (DWORD) sizeof(BITMAPFILEHEADER) + 
                    bmi.bmiHeader.biSize + bmi.bmiHeader.biClrUsed 
                    * sizeof (RGBQUAD); 

	
	memcpy(pBuffer, &bmfh, sizeof(bmfh));
	DWORD dwOffset = sizeof(bmfh);

	memcpy(pBuffer + dwOffset, &bmi, sizeof(BITMAPINFOHEADER));
	dwOffset += sizeof(BITMAPINFOHEADER);
	
	ULONG ulHeaderSize = dwOffset;

	if (bmi.bmiHeader.biClrUsed > 0)
	{
		RGBQUAD* pColorTable = (RGBQUAD*)(pBuffer + dwOffset);

		BYTE bClrVal;
		DWORD dwTemp;

		DWORD dwClrNum = bmi.bmiHeader.biClrUsed;

	
		for (DWORD i = 0; i < dwClrNum; i++)
		{
			dwTemp = 255 * i / (dwClrNum-1);
			bClrVal = (BYTE)dwTemp;

			pColorTable[i].rgbBlue = bClrVal;
			pColorTable[i].rgbGreen = bClrVal;
			pColorTable[i].rgbRed = bClrVal;
			pColorTable[i].rgbReserved = 0;
		}

		ulHeaderSize += (dwClrNum * sizeof(RGBQUAD));
	}

	*pulBytesRead = ulHeaderSize;

	return DEVMON_STATUS_OK;
}

int CScanner::ReadData(BYTE *pBuffer,ULONG ulBufferSize,ULONG *pulBytesRead, LONG *plPercentComplete)
{
	HRESULT hr = S_OK;

	if ((m_hLLD == NULL) || !m_bDeviceOpened || (pBuffer == NULL) || (pulBytesRead == NULL) || (plPercentComplete == NULL) || (ulBufferSize <= 0))
	{
		return DEVMON_ERROR;;
	}

	//
	// iScanline contains the number of bytes to copy from each scanline
	//
	//INT iScanline = (m_dwPixelsPerLine * m_dwBitsPerPixel + 31) / 32 * 4;

	*pulBytesRead       = 0;
	*plPercentComplete  = 0;

	if (m_dwTotalLinesRead >= m_dwTotalLinesToRead)
	{
		OutputDebugString(_T("WIA_STATUS_END_OF_MEDIA "));
		DevMon_WriteCommand(DevMon_STOPSCAN);
		return DEVMON_STATUS_END_OF_MEDIA;
	}

	DWORD dwNumBytesInBuffer = (ulBufferSize - (ulBufferSize % m_dwBytesPerLine));
	char szText2[200];
	sprintf(szText2, "ulBufferSize = %d, dwNumBytesInBuffer = %d, m_dwBytesPerLine = %d", 
		ulBufferSize, dwNumBytesInBuffer, m_dwBytesPerLine);

	//OutputDebugStringA(szText2);//*/


	DWORD dwNumBytesLeftToRead = (m_dwTotalLinesToRead - m_dwTotalLinesRead) * m_dwBytesPerLine;

	if(dwNumBytesLeftToRead < dwNumBytesInBuffer)
	{
		dwNumBytesInBuffer = dwNumBytesLeftToRead;
	}
	if (dwNumBytesInBuffer <= 0)
	{
		DevMon_AbortScan();         
		UnLockScanner();
		m_dwTotalLinesRead = 0;
		m_dwTotalLinesToRead = 0;
		m_dwTotalLinesToScan = 0;
		m_dwBytesPerLine = 0;
		m_dwPixelsPerLine = 0;
		m_dwBitsPerPixel = 0;
		m_dwResolution = 0;
		m_dwBytesPerLineFromDevice = 0;

		return DEVMON_ERROR;
	}
	DWORD dwNumLinesToRead = dwNumBytesInBuffer / m_dwBytesPerLine;

	//char szText1[200];
	//sprintf(szText1, "dwNumBytesInBuffer = %d, m_dwBytesPerLine = %d, dwNumLinesToRead = %d", 
	//	dwNumBytesInBuffer, m_dwBytesPerLine, dwNumLinesToRead);

	//OutputDebugStringA(szText1);//*/


	int nRet;
	DWORD dwBytesRead = 0;
	int nImageLength = m_dwTotalLinesToRead;

	DWORD dwOffset = 0;

	LPBYTE lpTemp = new BYTE[m_dwBytesPerLineFromDevice];

	for (DWORD i = 0; i < dwNumLinesToRead; i++)
	{
		if((m_dwTotalLinesRead + i) >= m_dwTotalLinesToScan)
		{
			memset(lpTemp, 255, m_dwBytesPerLineFromDevice);
			nRet = DEVMON_STATUS_OK;
		}
		else
		{
			nRet = DevMon_ReadScanData(lpTemp, m_dwBytesPerLineFromDevice, &dwBytesRead, nImageLength);
		}
		if(nRet != DEVMON_STATUS_OK)
		{
			/*char szText[100];
			sprintf(szText, "nRet = %d, m_dwTotalLinesRead = %d, m_dwTotalLinesToRead = %d", nRet, m_dwTotalLinesRead, m_dwTotalLinesToRead);
			OutputDebugStringA(szText);
			OutputDebugString(_T("DevMon_ReadScanData failed!"));*/
			DevMon_AbortScan();         
			UnLockScanner();

			m_dwTotalLinesRead = 0;
			m_dwTotalLinesToRead = 0;
			m_dwTotalLinesToScan = 0;
			m_dwBytesPerLine = 0;
			m_dwPixelsPerLine = 0;
			m_dwBitsPerPixel = 0;
			m_dwResolution = 0;
			m_dwBytesPerLineFromDevice = 0;

			delete []lpTemp;
			lpTemp = NULL;

			return nRet;
		}
		//EdgesErase(lpTemp);
		memcpy(pBuffer + dwOffset, lpTemp, m_dwBytesPerLine );

		dwOffset += m_dwBytesPerLine;
	}
	
	delete []lpTemp;
	lpTemp = NULL;

	*pulBytesRead = dwNumBytesInBuffer;

	m_dwTotalLinesRead += dwNumLinesToRead;

	if (m_dwBitsPerPixel == 24)
	{
		SwapRToBFor24BitsData(pBuffer, dwNumLinesToRead, m_dwBytesPerLine);
	}
	*plPercentComplete = (LONG)((((float)(m_dwTotalLinesRead)/(float)(m_dwTotalLinesToRead))) * 100.0f);

	//char szText[200];
	//sprintf(szText, "m_dwTotalLinesRead = %d, m_dwTotalLinesToRead = %d, plPercentComplete = %d", 
	//	m_dwTotalLinesRead, m_dwTotalLinesToRead, plPercentComplete);

	//OutputDebugStringA(szText);//*/


	return DEVMON_STATUS_OK;
}

int CScanner::StopScan()
{
	int nRet = 0;

	if (m_hLLD == NULL || !m_bDeviceOpened)
	{
		return DEVMON_ERROR_OPEN_FAILED;
	}

	if (m_dwTotalLinesRead < m_dwTotalLinesToScan)
	{
		nRet = DevMon_AbortScanEx();
	}
	else
	{
		nRet = DevMon_AbortScan();
	}
	UnLockScanner();

	m_dwTotalLinesRead = 0;
	m_dwTotalLinesToRead = 0;
	m_dwTotalLinesToScan = 0;
	m_dwBytesPerLine = 0;
	m_dwPixelsPerLine = 0;
	m_dwBitsPerPixel = 0;
	m_dwResolution = 0;
	m_dwBytesPerLineFromDevice = 0;

	return nRet;
}

int CScanner::GetADFStatus()
{
	int nRet;
	bool bLockedHere = false;

	if (m_hLLD == NULL || !m_bDeviceOpened)
	{
		return DEVMON_ERROR_OPEN_FAILED;
	}

	if (!m_bScannerLocked)
	{
		nRet = LockScanner();
		if (nRet != DEVMON_STATUS_OK)
		{
			return nRet;
		}

		bLockedHere = true;
	}

	nRet = DevMon_WriteCommand(DevMon_ADFSTATUS);

	if (bLockedHere)
	{
		UnLockScanner();
	}

	return nRet;
}

int CScanner::LockScanner()
{
	int nRet = 0;
	int nRetry = 0;

	if (m_bScannerLocked == true)
	{
		return DEVMON_STATUS_OK;
	}

	if (m_hLLD == NULL || !m_bDeviceOpened)
	{
		return DEVMON_ERROR;
	}

	while(nRetry < 3)
	{
		nRet = DevMon_WriteCommand(DevMon_LOCKSCANNER);
		if (nRet == DEVMON_STATUS_OK)                //LOCK SCANNER
		{
			break;
		}
		if (nRet == DEVMON_ERROR_SCAN_STATUS_STOP)   //cancel the task on scanner
		{
			break;
		}

		nRetry++;
		Sleep(2000);
	}

	if (nRet != DEVMON_STATUS_OK)
	{
		//DevMon_WriteCommand(DevMon_UNLOCKSCANNER);
	}
	else
	{
		m_bScannerLocked = true;
	}

	return nRet;
}

int CScanner::UnLockScanner()
{
	if (m_hLLD == NULL || !m_bDeviceOpened)
	{
		return DEVMON_ERROR;
	}

	m_bScannerLocked = false;

	return DevMon_WriteCommand(DevMon_UNLOCKSCANNER);;
}

int CScanner::SetScanParameter(LPSCANPARAMETER lpScanParam)
{
	int nRet;

	if (lpScanParam == NULL || m_hLLD == NULL || !m_bDeviceOpened)
	{
		return DEVMON_ERROR;
	}

	
		ADJUSTSTR AdjustStr;
		memset(&AdjustStr, 0, sizeof(AdjustStr));

        // Set adjust parameter, such as contrast/brightness
        for(int i=0; i<4; i++)
        {
            AdjustStr.Brightness[i] = lpScanParam->Brightness;
            AdjustStr.Contrast[i] = lpScanParam->Contrast;                
            AdjustStr.Chroma[i]=0;
        }

		AdjustStr.Flag = 4; //Brightness, Contrast and Chroma

		//  Purpose  : Protocols of setting scan adjust parameters into FW
		//  Parameters : MediaFlag: 0:Standard scan; 1:Copy scan; 2:Positive scan; 3:Nataive scan; 16:Vivid Scan
		//							10:for disable ICM
		//                      FilterFlag: 0:skip, 1:Normal, 2:Height,
		//                      DescrnFlag: 0:skip, 1:Fine Press, 2:Magazine, 3:News Paper
		//                      AdjustInput: Adjustment struct define at NTDCMS.h(Same as coloradjust.h)
		DevMon_SetScanParameterAdj(10, 0, 0, &AdjustStr);
	

	DWORD dwStartX, dwStartY, dwEndX, dwEndY;

	dwStartX = lpScanParam->Left*1200/1000;
	dwStartY = lpScanParam->Top*1200/1000;

	int iWidth = lpScanParam->Width;
	int iHeight = lpScanParam->Height;

	if(lpScanParam->ScanSource == ScanSource_Flatbed)
	{
		if(iWidth > 8500)
			iWidth = 8500;

		if(iHeight > 11690)
			iHeight = 11690;
	}

	dwEndX = dwStartX + iWidth*1200/1000;
	dwEndY = dwStartY + iHeight*1200/1000;

	nRet = DevMon_SetScanParameter(
						lpScanParam->XRes, 
						lpScanParam->YRes, 
						lpScanParam->ScanMode, 
						dwStartX, 
						dwStartY, 
						dwEndX,
						dwEndY,
						lpScanParam->Threshold,
						lpScanParam->ScanSource
						);

	return nRet;
}

int CScanner::GetScanParameter(DWORD *pPixelPerLine, DWORD *pTotalLines)
{
	return DevMon_GetScanParameter(pPixelPerLine, pTotalLines);
}

bool CScanner::SwapRToBFor24BitsData(BYTE *pBuffer, DWORD dwNumOfLine, DWORD dwNumBytesPerLine)
{
	LPBYTE lpTemp1 = NULL;      // point to first byte of line
	LPBYTE lpTemp2 = NULL;

	BYTE bTemp;

	if (pBuffer == NULL || dwNumOfLine == 0 || dwNumBytesPerLine == 0)
	{
		return false;
	}

	lpTemp1 = pBuffer;

	for (DWORD i = 0; i < dwNumOfLine; i++)
	{
		lpTemp2 = lpTemp1;

		for (DWORD j = 0; j < dwNumBytesPerLine-2; j+= 3)
		{
			bTemp = lpTemp2[2];
			lpTemp2[2] = lpTemp2[0];
			lpTemp2[0] = bTemp;

			lpTemp2 += 3;
		}

		lpTemp1 += dwNumBytesPerLine; 
	}

	return true;
}

BOOL CScanner::EdgesErase(LPBYTE pData)
{	
	DWORD nLeft = m_dwMarginLeftRight;
	DWORD nRight = m_dwPixelsPerLine - nLeft;

	switch(m_dwBitsPerPixel)
	{
	case 1:	//BW
		{
			int nLength = nLeft/8;
			int nRemain = 8 - (nLeft%8);
			memset(pData, 0xFF, nLength);
			pData[nLength] |= 0xFF<<nRemain;
			int nPos = (nRight+7)/8;
			memset(&pData[nPos], 0xFF, m_dwBytesPerLine - nPos);
			nRemain = nRight%8;
			pData[nPos-1] |= 0xFF>>nRemain;
		}
		break;
	case 8:	//Gray
		memset(pData, 0xFF, nLeft);
		memset(&pData[nRight], 0xFF, nLeft);
		break;
	case 24:	//24bit Color
		memset(pData, 0xFF, nLeft*3);
		memset(&pData[nRight*3], 0xFF, nLeft*3);
		break;
	}

	return TRUE;
}

