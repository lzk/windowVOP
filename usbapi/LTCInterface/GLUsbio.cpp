#include "../StdAfx.h"

#include <usbscan.h>
#include <tchar.h>
#include "../usbapi.h"
#include "../Global.h"


#ifndef True
#define True 1
#endif
#ifndef False
#define False 0
#endif
#define USBSCANSTRING	  L"\\\\.\\usbscan"
#define MAX_DEVICES       127
#define DLL_NAME_NET L"NetIO"

#define USB_VID 0x05e3
#define USB_PID 0x0118

CGLUsb::CGLUsb():
m_GLUSBDev(INVALID_HANDLE_VALUE)
{
}

CGLUsb::~CGLUsb()
{
}

int CGLUsb::CMDIO_OpenDevice()
{
	TCHAR strPort[32] = { 0 };
	int  iCnt;

	for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++) {
		_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);
		m_hDev = CreateFile(strPort,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_EXISTING,
			FILE_FLAG_OVERLAPPED, NULL);

		if (m_hDev != INVALID_HANDLE_VALUE) {

#ifdef USB_VID
			if (!CMDIO_GetDeviceFeatures()) {
				CloseHandle(m_hDev);
				continue;
			}

//#ifdef USB_VID
			if (CMDIO_VID != USB_VID) {
				CloseHandle(m_hDev);
				continue;
			}
#endif
#ifdef USB_PID
			if (CMDIO_PID != USB_PID) {
				CloseHandle(m_hDev);
				continue;
			}
#endif
			m_hIntrEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
			m_hDevice[0] = m_hDev;
			OpenBulkPipes(m_hDevice, strPort);	// Jason 140408
			MyOutputString(L"OpenDevice OK");
			break;
		}
	}

	return (m_hDev == INVALID_HANDLE_VALUE) ? False : True;
}

typedef enum { USB_10, USB_20, USB_30 } flgUSBType;
int CGLUsb::CMDIO_GetDeviceFeatures()
{
	BOOL bRet;
	DWORD dwIoVal;
	DEVICE_DESCRIPTOR Desc;

	if (!CMDIO_GetConnectionType(&CMDIO_iConnectType))
		return False;
	CMDIO_BulkFiFoSize = ((CMDIO_iConnectType >= USB_20) ? 512 : 64);

	bRet = AsyncDeviceIoCtrl(m_hDev,
		IOCTL_GET_DEVICE_DESCRIPTOR,
		(BYTE*)&Desc,
		sizeof(DEVICE_DESCRIPTOR),
		(BYTE*)&Desc,
		sizeof(DEVICE_DESCRIPTOR),
		&dwIoVal,
		NULL);
	if (!bRet) return False;

	CMDIO_VID = Desc.usVendorId;
	CMDIO_PID = Desc.usProductId;
	CMDIO_BcdDevice = Desc.usBcdDevice;


	return True;
}

int CGLUsb::CMDIO_GetConnectionType(BYTE *nType)
{

	int bRet;
	unsigned char temp;

	//*nType = USB_20;
	//return True;

	bRet = CMDIO_ReadCommand(IOCTL_CONNECT_TYPE, 0, 1, &temp);
	if (!bRet)
		return False;
	//*nType = ( (temp & 0x01) ? USB_10 : USB_20 );
	if (temp == 1)
		*nType = USB_10;
	else if (temp == 2)
		*nType = USB_30;
	else
		*nType = USB_20;

	return True;
}

int CGLUsb::CMDIO_OpenDevice(LPCTSTR lpModuleName)
{
	TCHAR strPort[32];
	int  iCnt=0;
	_tcscpy(strPort,lpModuleName);
	for (iCnt=0;iCnt <= MAX_DEVICES; iCnt++) 
	{
		if(m_GLUSBDev != INVALID_HANDLE_VALUE )
		{
			m_hDev = m_GLUSBDev;
		}
		else
		{
			m_hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);
		}
		if (m_hDev != INVALID_HANDLE_VALUE) 
		{
			m_hIntrEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
			m_hDevice[0] = m_hDev;
			OpenBulkPipes(m_hDevice, strPort);	// Jason 140408
		}
	}
	
	return (m_hDev == INVALID_HANDLE_VALUE) ? False : True ;
}

int CGLUsb::OpenBulkPipes(HANDLE *hPipe, LPCTSTR lpModuleName)
{
	USBSCAN_PIPE_CONFIGURATION PipeConfig;
	DWORD cbRet;
	//-------------------------------------------------------
	//	Get PIPE Configuration with default handle hPipe[0]
	//-------------------------------------------------------
	int result = DeviceIoControl(hPipe[0],
		(DWORD)IOCTL_GET_PIPE_CONFIGURATION, 
		NULL,
		0,
		&PipeConfig,
		sizeof(PipeConfig),
		&cbRet,
		NULL);
	//-------------------------------------------------------
	//	open the first 2 bulk pipes in PIPE configuration
	// (hPipe[1]/hPipe[2] for 2nd bulk-read / bulk-write pipe)
	//-------------------------------------------------------
	int i;
	TCHAR usbscan[32];
	HANDLE handle;
	if(result) {
		for(i = 0, hPipe[1] = hPipe[2] = hPipe[3] = hPipe[4] = 0; i < (int)PipeConfig.NumberOfPipes; i++)
		{
			if(PipeConfig.PipeInfo[i].PipeType  == USBSCAN_PIPE_BULK) {
				_stprintf_s(usbscan, L"%s\\%d", lpModuleName, i);
				handle = CreateFile(usbscan,
					GENERIC_READ | GENERIC_WRITE,
					FILE_SHARE_READ | FILE_SHARE_WRITE,
					NULL,
					OPEN_EXISTING,
					FILE_FLAG_OVERLAPPED, NULL);

				if(PipeConfig.PipeInfo[i].EndpointAddress == 0x81)
					hPipe[1] = handle;
				else if(PipeConfig.PipeInfo[i].EndpointAddress == 0x02)
					hPipe[2] = handle;
				else if(PipeConfig.PipeInfo[i].EndpointAddress == 0x84)
					hPipe[3] = handle;
				else if(PipeConfig.PipeInfo[i].EndpointAddress == 0x05)
					hPipe[4] = handle;
				
				if(hPipe[1] && hPipe[2] && hPipe[3] && hPipe[4])
					break;
			}
		}
	}
	return result;
}

int CGLUsb::CMDIO_WriteCommand(unsigned short nCmd, unsigned short nIdx, unsigned short nCmdLen, unsigned char *pCmdData)
{
	DWORD    cbRet;
	BOOL     bRet;
	IO_BLOCK IoBlock;
	int      iRetry = 0;
#ifdef CTRL_WRITE_RETRY
	if (CMDIO_iConnectType == USB_20) {
		while (iRetry < CTRL_WRITE_RETRY) {
			IoBlock.uOffset = nCmd;
			IoBlock.uLength = nCmdLen;
			IoBlock.pbyData = pCmdData;
			IoBlock.uIndex = nIdx;
			bRet = AsyncDeviceIoCtrl(m_hDev,
				(DWORD)IOCTL_WRITE_REGISTERS,
				(BYTE*)&IoBlock,
				sizeof(IO_BLOCK),
				NULL,
				0,
				&cbRet,
				NULL);
			if (!bRet)
				break;
			bRet = CMDIO_GetCtrlWriteStatus();
			if (bRet)
				break;
			iRetry++;
		}

		if (iRetry >= CTRL_WRITE_RETRY)
			bRet = FALSE;
		return (bRet ? True : False);
	}
#endif  // #ifndef CTRL_WRITE_RETRY
	IoBlock.uOffset = nCmd;
	IoBlock.uLength = nCmdLen;
	IoBlock.pbyData = pCmdData;
	IoBlock.uIndex = nIdx;
	bRet = AsyncDeviceIoCtrl(m_hDev,
		(DWORD)IOCTL_WRITE_REGISTERS,
		(BYTE*)&IoBlock,
		sizeof(IO_BLOCK),
		NULL,
		0,
		&cbRet,
		NULL);
	return (bRet ? True : False);
}

int CGLUsb::CMDIO_ReadCommand(unsigned short nCmd, unsigned short nIdx, unsigned short nCmdLen, unsigned char *pCmdData)
{
	DWORD    cbRet;
	BOOL     bRet;
	IO_BLOCK IoBlock;

	IoBlock.uOffset = nCmd;
	IoBlock.uLength = nCmdLen;
	IoBlock.pbyData = NULL;
	IoBlock.uIndex = nIdx;
	bRet = AsyncDeviceIoCtrl(m_hDev,
		(DWORD)IOCTL_READ_REGISTERS,
		(BYTE*)&IoBlock,
		(DWORD)sizeof(IO_BLOCK),
		(BYTE*)pCmdData,
		(DWORD)nCmdLen * sizeof(BYTE),
		&cbRet,
		NULL);
	return (bRet ? True : False);
}


BOOL CGLUsb::AsyncWriteFile(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, HANDLE hEvent)
{
	OVERLAPPED  ovAnync;  
	BOOL        bRet;
	DWORD       dwErrCode, dwWaitTime; 
	HANDLE      hEventx; 

	hEventx = CreateEvent(NULL,TRUE,FALSE,NULL); 
	ZeroMemory(&ovAnync,sizeof(ovAnync));
	ResetEvent(hEventx); 
	ovAnync.hEvent = hEventx;
   
	dwWaitTime=60000;
	if ((bRet = WriteFile(hFile,Buf,dwBufSize,dwWrite,&ovAnync))==FALSE) {
		dwErrCode = GetLastError();
		if (dwErrCode == ERROR_IO_PENDING) {
			dwErrCode = WaitForSingleObject(hEventx,dwWaitTime);
			bRet = ( (dwErrCode == WAIT_TIMEOUT) ? FALSE : TRUE );
		}
	}
	if (bRet)
		bRet = GetOverlappedResult(hFile,&ovAnync,dwWrite,FALSE);

	CloseHandle(hEventx); 
	return bRet;
}
BOOL CGLUsb::AsyncReadFile(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, HANDLE hEvent)
{
	OVERLAPPED  ovAnync;
	BOOL        bRet;
	DWORD       dwErrCode, dwWaitTime; 
	HANDLE      hEventx; 
	hEvent = hEvent;
	hEventx = CreateEvent(NULL,TRUE,FALSE,NULL); 
	ZeroMemory(&ovAnync,sizeof(ovAnync));
	ResetEvent(hEventx); 
	ovAnync.hEvent = hEventx;

	dwWaitTime = 60000;
	if ((bRet = ReadFile(hFile,Buf,dwBufSize,dwWrite,&ovAnync))==FALSE) {
		dwErrCode = GetLastError();
		if (dwErrCode == ERROR_IO_PENDING) {
			dwErrCode = WaitForSingleObject(hEventx,dwWaitTime);
			bRet = ( (dwErrCode == WAIT_TIMEOUT) ? FALSE : TRUE );
		}
	}
	if (bRet) 
		bRet = GetOverlappedResult(hFile,&ovAnync,dwWrite,FALSE);

	CloseHandle(hEventx); 
	return bRet;
}

BOOL CGLUsb::AsyncDeviceIoCtrl(HANDLE hFile, DWORD dwCtrlCode,
	BYTE *lpInBuffer, DWORD dwInBufSize,
	BYTE *lpOutBuffer, DWORD dwOutBufSize,
	DWORD *dwResult, HANDLE hEvent)
{
	OVERLAPPED  ovAsync;
	BOOL        bRet;
	DWORD       dwErrCode, dwWaitTime;
	HANDLE      hEventx;

	hEventx = CreateEvent(NULL, TRUE, FALSE, NULL);
	ZeroMemory(&ovAsync, sizeof(ovAsync));
	ResetEvent(hEventx);
	ovAsync.hEvent = hEventx;

	dwWaitTime = 60000;
	if ((bRet = DeviceIoControl(hFile, dwCtrlCode, lpInBuffer, dwInBufSize,
		lpOutBuffer, dwOutBufSize, dwResult, &ovAsync)) == FALSE) {
		dwErrCode = GetLastError();
		if (dwErrCode == ERROR_IO_PENDING) {
			dwErrCode = WaitForSingleObject(hEventx, (dwWaitTime));
			bRet = ((dwErrCode == WAIT_TIMEOUT) ? FALSE : TRUE);
		}
	}
	if (bRet)
		bRet = GetOverlappedResult(hFile, &ovAsync, dwResult, FALSE);

	CloseHandle(hEventx);
	return bRet;
}

int CGLUsb::CMDIO_BulkWriteEx(int pipe, void* buffer, unsigned int dwLen)
{
	BOOL	bRet;
	DWORD	dwByteCount;
	bRet = AsyncWriteFile(m_hDevice[pipe*2+2] ,(BYTE*)buffer, dwLen, &dwByteCount,NULL);
	return bRet;
}

int CGLUsb::CMDIO_BulkWriteEx(int pipe, void* buffer, unsigned int dwLen, DWORD	*dwByteCount)
{
	BOOL	bRet;

	if (dwByteCount == NULL)
		return FALSE;

	bRet = AsyncWriteFile(m_hDevice[pipe * 2 + 2], (BYTE*)buffer, dwLen, dwByteCount, NULL);
	return bRet;
}

int CGLUsb::CMDIO_BulkReadEx(int pipe, void *buffer, unsigned int dwLen)
{
	BOOL	bRet;
	DWORD	dwByteCount;
	bRet = AsyncReadFile(m_hDevice[pipe*2+1], (BYTE*)buffer, dwLen, &dwByteCount, NULL);
	if(dwLen!=dwByteCount)
	{
		//printf(" <length mismatch>\n");
		return FALSE;
	}
	return bRet;
}

int CGLUsb::CMDIO_BulkReadEx(int pipe, void *buffer, unsigned int dwLen, DWORD	*dwByteCount)
{
	BOOL	bRet;

	if (dwByteCount == NULL)
		return FALSE;

	bRet = AsyncReadFile(m_hDevice[pipe * 2 + 1], (BYTE*)buffer, dwLen, dwByteCount, NULL);
	if (dwLen != *dwByteCount)
	{
		//printf(" <length mismatch>\n");
		return FALSE;
	}
	return bRet;
}

int CGLUsb::CMDIO_CloseDevice()
{
	int i;
	CloseHandle(m_hDev);
	CloseHandle(m_hIntrEvent);
	for(i=1; i < 5; i++)
		CloseHandle(m_hDevice[i]);
	return True;
}
int CGLUsb::_SetIOHandle(HANDLE dwDevice, WORD wType)
{
	wType = wType;
	m_GLUSBDev = dwDevice;
	return TRUE;
}

CGLNet::CGLNet()
{
	m_hmod = LoadLibrary(DLL_NAME_NET);

	if (m_hmod)
	{
		m_lpfnNetworkConnect = (LPFN_NETWORK_CONNECT)GetProcAddress(m_hmod, "NetworkConnectNonBlock");
		m_lpfnNetworkRead = (LPFN_NETWORK_READ)GetProcAddress(m_hmod, "NetworkRead");
		m_lpfnNetworkWrite = (LPFN_NETWORK_WRITE)GetProcAddress(m_hmod, "NetworkWrite");
		m_lpfnNetworkClose = (LPFN_NETWORK_CLOSE)GetProcAddress(m_hmod, "NetworkClose");
	}

	m_socketId = 0;
}

CGLNet::~CGLNet()
{
	m_lpfnNetworkConnect = NULL;
	m_lpfnNetworkRead = NULL;
	m_lpfnNetworkWrite = NULL;
	m_lpfnNetworkClose = NULL;

	if (m_hmod)
	{
		FreeLibrary(m_hmod);
	}

}

int CGLNet::CMDIO_Connect(const wchar_t* ipAddress)
{
	if (m_hmod && m_lpfnNetworkConnect)
	{
		char szAsciiIP[1024] = { 0 };
		::WideCharToMultiByte(CP_ACP, 0, ipAddress, -1, szAsciiIP, 1024, NULL, NULL);
		m_socketId = m_lpfnNetworkConnect(szAsciiIP, 23010, 1000);

		if(m_socketId == -1)
			return False;

		return True;
	}
	return False;
}

int CGLNet::CMDIO_Connect(const wchar_t* ipAddress, int port)
{
	if (m_hmod && m_lpfnNetworkConnect)
	{
		char szAsciiIP[1024] = { 0 };
		::WideCharToMultiByte(CP_ACP, 0, ipAddress, -1, szAsciiIP, 1024, NULL, NULL);
		m_socketId = m_lpfnNetworkConnect(szAsciiIP, port, 1000);

		if (m_socketId == -1)
			return False;

		return True;
	}
	return False;
}

int CGLNet::CMDIO_Write(void* buffer, unsigned int dwLen)
{
	int cbWrite = 0;
	if (m_hmod && m_lpfnNetworkWrite)
	{
		cbWrite = m_lpfnNetworkWrite(m_socketId, buffer, dwLen);
	}

	if (cbWrite == dwLen)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

int CGLNet::CMDIO_Read(void* buffer, unsigned int dwLen)
{
	int cbRead = 0;
	if (m_hmod && m_lpfnNetworkRead)
	{
		cbRead = m_lpfnNetworkRead(m_socketId, buffer, dwLen);
	}

	if (cbRead == dwLen)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

int CGLNet::CMDIO_Close()
{
	if (m_hmod && m_lpfnNetworkClose)
	{
		m_lpfnNetworkClose(m_socketId);

		return True;
	}

	return False;
}