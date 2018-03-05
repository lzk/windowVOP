////////////////////////////////////////////////////////////////
// File - usbio.c
// Copyright (c) 2011 - Genesys Logic, INC.
////////////////////////////////////////////////////////////////
#include <Windows.h>
//#include <winioctl.h>
#include <stdio.h>
#include "UsbScan.h"
#include "usbio.h"
//#include "gusrdef.h"
#include <tchar.h>

#ifndef True
#define True 1
#endif
#ifndef False
#define False 0
#endif

#define MAX_DEVICES       127
#define USBSCANSTRING    L"\\\\.\\usbscan"
//---------------------------
// modifiable flag
//---------------------------
// remove the following mark if need to do control write check for USB2.0
//#define CTRL_WRITE_RETRY  3
//---------------------------
// remove the following mark if need to check VID/PID
#define USB_VID 0x05e3
#define USB_PID 0x0118
//---------------------------

HANDLE m_hDev, m_hIntrEvent;
HANDLE m_hDevice[5];
OVERLAPPED m_ov[5];

unsigned short CMDIO_VID_, CMDIO_PID_;
unsigned short CMDIO_BcdDevice;
unsigned char CMDIO_iConnectType;
unsigned int  CMDIO_BulkFiFoSize;

BOOL AsyncWriteFile(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, HANDLE hEvent)
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


BOOL AsyncReadFile(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, HANDLE hEvent)
{
  OVERLAPPED  ovAnync;
  BOOL        bRet;
  DWORD       dwErrCode, dwWaitTime; 
  HANDLE      hEventx; 

  hEventx = CreateEvent(NULL,TRUE,FALSE,NULL); 
  ZeroMemory(&ovAnync,sizeof(ovAnync));
  ResetEvent(hEventx); 
  ovAnync.hEvent = hEventx;

  dwWaitTime = 20000;
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



BOOL AsyncDeviceIoCtrl(HANDLE hFile,      DWORD dwCtrlCode, 
                       BYTE *lpInBuffer,  DWORD dwInBufSize,
                       BYTE *lpOutBuffer, DWORD dwOutBufSize,
                       DWORD *dwResult,   HANDLE hEvent)
{
  OVERLAPPED  ovAsync;
  BOOL        bRet;
  DWORD       dwErrCode, dwWaitTime;
  HANDLE      hEventx;

  hEventx = CreateEvent(NULL,TRUE,FALSE,NULL);
  ZeroMemory(&ovAsync,sizeof(ovAsync));
  ResetEvent(hEventx);
  ovAsync.hEvent = hEventx;

  dwWaitTime = 60000;
  if ((bRet = DeviceIoControl(hFile, dwCtrlCode, lpInBuffer, dwInBufSize,
                         lpOutBuffer, dwOutBufSize, dwResult, &ovAsync))==FALSE){
    dwErrCode = GetLastError();
    if (dwErrCode == ERROR_IO_PENDING) {
      dwErrCode = WaitForSingleObject(hEventx,(dwWaitTime));
      bRet = ( (dwErrCode == WAIT_TIMEOUT) ? FALSE : TRUE );
    }
  }
  if (bRet) 
    bRet = GetOverlappedResult(hFile,&ovAsync,dwResult,FALSE);
  
  CloseHandle(hEventx);
  return bRet;
}

// Jason 140408
//-------------------------------------
// input:  hPipe[0]
// ouput:  hPipe[1], hPipe[2]
//-------------------------------------
int OpenBulkPipes(HANDLE *hPipe, int usbscan_num)
{
  USBSCAN_PIPE_CONFIGURATION PipeConfig;
  DWORD cbRet;
  //-------------------------------------------------------
  //  Get PIPE Configuration with default handle hPipe[0]
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
  //  open the first 2 bulk pipes in PIPE configuration
  // (hPipe[1]/hPipe[2] for 2nd bulk-read / bulk-write pipe)
  //-------------------------------------------------------
  int i;
  TCHAR usbscan[32];
  HANDLE handle;
  if(result) {
    for(i = 0, hPipe[1] = hPipe[2] = hPipe[3] = hPipe[4] = 0; i < (int)PipeConfig.NumberOfPipes; i++)
    {
      if(PipeConfig.PipeInfo[i].PipeType  == USBSCAN_PIPE_BULK) {
		  _stprintf_s(usbscan,L"%s%d\\%d",USBSCANSTRING, usbscan_num, i);
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

int AsyncWriteFileEx(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, OVERLAPPED *ov)
{
  BOOL        bRet;
  HANDLE      hEventx; 

  hEventx = CreateEvent(NULL,TRUE,FALSE,NULL); 
  ZeroMemory(ov,sizeof(OVERLAPPED));
  ResetEvent(hEventx); 
  ov->hEvent = hEventx;
  bRet = WriteFile(hFile, Buf, dwBufSize, dwWrite, ov);
  if(!bRet) {
    if(GetLastError() == ERROR_IO_PENDING)
      bRet = TRUE;
  }
  return bRet;
}

int AsyncFileExWait(HANDLE hFile, OVERLAPPED *ov, int wait_ms)
{
  int result = 0;  // 0:pending, -1:error, +:return bytes
  if(wait_ms >= 0) {
    result = (int)WaitForSingleObject(ov->hEvent, wait_ms);
    if(result == WAIT_OBJECT_0) {
      if(!GetOverlappedResult(hFile, ov, (DWORD*)&result, 0))
        result = -1;
    }
    else if(result == WAIT_TIMEOUT)
      result = 0;
    else
      result = -1;
  }
  else {
    //ResetEvent(ov->hEvent);
    result = -1;
  }

  if(result)  // complete / error / cancel
  {
    if(ov->hEvent) {
      CloseHandle(ov->hEvent);
      ov->hEvent = 0;
    }
  }
  return result;
}

BOOL AsyncReadFileEx(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, OVERLAPPED *ov)
{
  BOOL        bRet;
  HANDLE      hEventx; 

  hEventx = CreateEvent(NULL,TRUE,FALSE,NULL); 
  ZeroMemory(ov, sizeof(OVERLAPPED));
  ResetEvent(hEventx); 
  ov->hEvent = hEventx;
  bRet = ReadFile(hFile, Buf, dwBufSize, dwWrite, ov);
  if(!bRet) {
    if(GetLastError() == ERROR_IO_PENDING)
      bRet = TRUE;
  }
  return bRet;
}

int IoCtl_CancelIo(int pipe, int type)
{
  BOOL bRet = FALSE;
  DWORD cbRet = 0;
  HANDLE handle;

  if(type == READ_DATA_PIPE)
    handle = m_hDevice[pipe*2+1];
  else if(type == WRITE_DATA_PIPE)
    handle = m_hDevice[pipe*2+2];
  else // interrupt
    handle = m_hDevice[0];

    bRet = DeviceIoControl(handle,
      (DWORD) IOCTL_CANCEL_IO, 
      (LPVOID)&type,
      sizeof(PIPE_TYPE),
      NULL,
      0,
      &cbRet,
      NULL); 
    return bRet;
}

//===================
// external function
//===================

int CMDIO_OpenDevice()
{ 
	TCHAR strPort[32];
  int  iCnt;

  for (iCnt=0;iCnt <= MAX_DEVICES; iCnt++) {
	  _stprintf_s(strPort,L"%s%d",USBSCANSTRING,iCnt);

    m_hDev = CreateFile(strPort,
      GENERIC_READ | GENERIC_WRITE,
      FILE_SHARE_READ | FILE_SHARE_WRITE,
      NULL,
      OPEN_EXISTING,
      FILE_FLAG_OVERLAPPED, NULL);

    if (m_hDev != INVALID_HANDLE_VALUE) {

#ifdef USB_VID
      if(!CMDIO_GetDeviceFeatures()) {
        CloseHandle(m_hDev);
        continue;
      }

//#ifdef USB_VID
      if(CMDIO_VID_ != USB_VID) {
        CloseHandle(m_hDev);
        continue;
      }
#endif
#ifdef USB_PID
      if (CMDIO_PID_ != USB_PID) {
        CloseHandle(m_hDev);
        continue;
      }
#endif
      m_hIntrEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
      m_hDevice[0] = m_hDev;
      OpenBulkPipes(m_hDevice, iCnt);  // Jason 140408
      break;
    }
  }

  return (m_hDev == INVALID_HANDLE_VALUE) ? False : True ;
}

// Jason 140408
int CMDIO_CloseDevice()
{
  int i;
  CloseHandle(m_hDev);
  CloseHandle(m_hIntrEvent);
  for(i=1; i < 5; i++)
    CloseHandle(m_hDevice[i]);
  return True;
}


int CMDIO_WriteCommand(unsigned short nCmd, unsigned short nIdx, unsigned short nCmdLen, unsigned char *pCmdData)
{
  DWORD    cbRet;
  BOOL     bRet;
  IO_BLOCK IoBlock;
  int      iRetry=0;
#ifdef CTRL_WRITE_RETRY
  if (CMDIO_iConnectType==USB_20) {
    while(iRetry < CTRL_WRITE_RETRY) {
      IoBlock.uOffset = nCmd;
      IoBlock.uLength = nCmdLen;
      IoBlock.pbyData = pCmdData;
      IoBlock.uIndex  = nIdx;
      bRet = AsyncDeviceIoCtrl(m_hDev,
            (DWORD) IOCTL_WRITE_REGISTERS,
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
      iRetry ++;
    }

    if (iRetry >= CTRL_WRITE_RETRY)  
      bRet = FALSE;
    return ( bRet ? True : False );
  }
#endif  // #ifndef CTRL_WRITE_RETRY
  IoBlock.uOffset = nCmd;
  IoBlock.uLength = nCmdLen;
  IoBlock.pbyData = pCmdData;
  IoBlock.uIndex  = nIdx;
  bRet = AsyncDeviceIoCtrl(m_hDev,
                     (DWORD) IOCTL_WRITE_REGISTERS,
                     (BYTE*)&IoBlock,
                     sizeof(IO_BLOCK),             
                     NULL, 
                     0,
                     &cbRet,
           NULL);
  return ( bRet ? True : False );
}

int CMDIO_ReadCommand(unsigned short nCmd, unsigned short nIdx, unsigned short nCmdLen, unsigned char *pCmdData)
{
  DWORD    cbRet;
  BOOL     bRet;
  IO_BLOCK IoBlock;

  IoBlock.uOffset = nCmd;
  IoBlock.uLength = nCmdLen;
  IoBlock.pbyData = NULL;
  IoBlock.uIndex  = nIdx;
  bRet = AsyncDeviceIoCtrl(m_hDev,
                          (DWORD)IOCTL_READ_REGISTERS,
                          (BYTE*)&IoBlock,
                          (DWORD)sizeof(IO_BLOCK),
                          (BYTE*)pCmdData, 
                          (DWORD)nCmdLen * sizeof(BYTE),
                          &cbRet,
                          NULL);
  return ( bRet ? True : False );
}



int CMDIO_BulkWrite(unsigned char *buffer, unsigned int dwLen)
{
  BOOL  bRet=TRUE;
  DWORD  dwByteCount;
  bRet = AsyncWriteFile(m_hDev ,buffer, dwLen, &dwByteCount,NULL);
  if (!bRet)  return False;
  return True;

}

// Jason 140408
// pipe= 0:  bulk write via default endpoint
// pipe= 1: bulk write via additional endpoint
int CMDIO_BulkWriteEx(int pipe, void* buffer, unsigned int dwLen)
{
  BOOL  bRet;
  DWORD  dwByteCount;
  bRet = AsyncWriteFile(m_hDevice[pipe*2+2] ,(BYTE*)buffer, dwLen, &dwByteCount,NULL);
  return bRet;
}

int CMDIO_BulkWriteEx2(int pipe, void* buffer, unsigned int dwLen)
{
  BOOL  bRet;
  DWORD  dwByteCount;
  bRet = AsyncWriteFileEx(m_hDevice[pipe*2+2] , (BYTE*)buffer, dwLen, &dwByteCount, &m_ov[pipe*2+2]);
  return bRet;
}

int CMDIO_BulkWriteEx2Wait(int pipe, int wait_ms)
{
  int ret;
  ret = AsyncFileExWait(m_hDevice[pipe*2+2], &m_ov[pipe*2+2], wait_ms);
  return ret;
}

int CMDIO_BulkRead(unsigned char *buffer, unsigned int dwLen)
{
  BOOL  bRet=TRUE;
  DWORD  dwByteCount;
  bRet = AsyncReadFile(m_hDev, buffer, dwLen, &dwByteCount, NULL);
  if(dwLen!=dwByteCount) {
    printf(" bulk read & return length not match \n");
    //return True;
  }
  if (!bRet)  {
    return False;
  }
  return True;
}

// Jason 140408
// pipe= 0:  bulk read via default endpoint
// pipe= 1: bulk read via additional endpoint
int CMDIO_BulkReadEx(int pipe, void *buffer, unsigned int dwLen)
{
  BOOL  bRet;
  DWORD  dwByteCount;
  bRet = AsyncReadFile(m_hDevice[pipe*2+1], (BYTE*)buffer, dwLen, &dwByteCount, NULL);
  if(dwLen!=dwByteCount)
    printf(" <length mismatch>\n");
  return bRet;
}

int CMDIO_BulkReadEx2(int pipe, void *buffer, unsigned int dwLen)
{
  BOOL  bRet;
  DWORD  dwByteCount;
  bRet = AsyncReadFileEx(m_hDevice[pipe*2+1], (BYTE*)buffer, dwLen, &dwByteCount, &m_ov[pipe*2+1]);
  return bRet;
}

int CMDIO_BulkReadEx2Wait(int pipe, int wait_ms)
{
  int ret;
  ret = AsyncFileExWait(m_hDevice[pipe*2+1], &m_ov[pipe*2+1], wait_ms);
  return ret;
}



// stop DMA to cancel bulk read
int CMDIO_CancelBulkRead(int flush)
{
  unsigned char data=1;

  if(flush)
    FlushFileBuffers(m_hDev);
  return CMDIO_WriteCommand(IOCTL_BUF_ENDACCESS,0,1,&data);
}

int CMDIO_AbortBulkRead(int pipe)
{
  int ret;
  ret = CancelIo(m_hDevice[pipe*2+1]);
  //ret = IoCtl_CancelIo(pipe, READ_DATA_PIPE);
  //ret = FlushFileBuffers(m_hDevice[pipe*2+1]); 
  return ret;
}

int CMDIO_AbortBulkWrite(int pipe)
{
  int ret;
  ret = CancelIo(m_hDevice[pipe*2+2]);
  //ret = IoCtl_CancelIo(pipe, WRITE_DATA_PIPE);
  //ret = FlushFileBuffers(m_hDevice[pipe*2+2]);
  return ret;
}

int CMDIO_InterruptIoCtl(unsigned char *Buf, unsigned int dwLen)
{
  OVERLAPPED  IntOverlapped;
  BOOL        fRet;
  DWORD     m_dwIntRead; //, m_dwIntWait;

  ZeroMemory(&IntOverlapped,sizeof(OVERLAPPED));
  ResetEvent(m_hIntrEvent); 
  IntOverlapped.hEvent = m_hIntrEvent; 
  fRet = DeviceIoControl(m_hDev,
      (DWORD) IOCTL_WAIT_ON_DEVICE_EVENT,
      NULL,
      0,
      Buf,
      dwLen,
      &m_dwIntRead,
      &IntOverlapped);
  if(!fRet) {
    if(GetLastError() == ERROR_IO_PENDING) {
      if(WaitForSingleObject(m_hIntrEvent, 50) == WAIT_OBJECT_0)
        fRet = TRUE;
      else {
        CancelIo(m_hDev);  //about 50ms
        *Buf = 0;
      }
    }
  }
  return fRet;
}

OVERLAPPED  IntOverlapped;

int CMDIO_InterruptRead(unsigned char *Buf, unsigned int dwLen)
{
  BOOL  fRet;
  DWORD  dwError = 0;
  ZeroMemory(&IntOverlapped,sizeof(OVERLAPPED));
  ResetEvent(m_hIntrEvent); 
  IntOverlapped.hEvent = m_hIntrEvent;
  ZeroMemory(Buf, dwLen);
  fRet = DeviceIoControl(m_hDev,
    (DWORD) IOCTL_WAIT_ON_DEVICE_EVENT,
    NULL,
    0,
    Buf,
    dwLen,
    &dwError,
    &IntOverlapped);
  if(!fRet) {
    if(GetLastError() == ERROR_IO_PENDING)
      fRet = TRUE;
  }
  return fRet;
}

int CMDIO_InterruptWait(int wait_ms)
{
  int result = 0;
  if(wait_ms >= 0) {
    result = (int)WaitForSingleObject(m_hIntrEvent, wait_ms);
    if(result == WAIT_OBJECT_0) {
      if(!GetOverlappedResult(m_hDev, &IntOverlapped, (DWORD*)&result, 0))
        result = -1;
    }
    else if(result == WAIT_TIMEOUT)
      result = 0;
    else
      result = -1;
  }
  else {
    ResetEvent(IntOverlapped.hEvent);
    result = -1;
  }
  //if(result)  // complete / error / cancel
  //  CloseHandle(IntOverlapped.hEvent);
  return result;
}

int CMDIO_InterruptCancel()
{
  int ret;
  ret = CancelIo(m_hDevice[0]);
  //ret = IoCtl_CancelIo(0, EVENT_PIPE);
  return ret;
}

int CMDIO_GetDeviceID(unsigned short *vid,unsigned short *pid) 
{
  DWORD dwIoVal;
  DEVICE_DESCRIPTOR Desc;
  BOOL bRet;
  bRet=AsyncDeviceIoCtrl( m_hDev, 
              IOCTL_GET_DEVICE_DESCRIPTOR, 
              (BYTE*)&Desc, 
              sizeof(DEVICE_DESCRIPTOR), 
              (BYTE*)&Desc, 
              sizeof(DEVICE_DESCRIPTOR), 
              &dwIoVal, 
              NULL);
  if (!bRet) return False;

  *vid = Desc.usVendorId;
  *pid = Desc.usProductId;
  return True;
}

typedef enum {USB_10, USB_20, USB_30} flgUSBType;
int CMDIO_GetDeviceFeatures()
{
  BOOL bRet;
  DWORD dwIoVal; 
  DEVICE_DESCRIPTOR Desc;

  if ( !CMDIO_GetConnectionType(&CMDIO_iConnectType) )
    return False;
  CMDIO_BulkFiFoSize = ( (CMDIO_iConnectType>=USB_20) ? 512 : 64 );

  bRet=AsyncDeviceIoCtrl( m_hDev, 
              IOCTL_GET_DEVICE_DESCRIPTOR, 
              (BYTE*)&Desc, 
              sizeof(DEVICE_DESCRIPTOR), 
              (BYTE*)&Desc, 
              sizeof(DEVICE_DESCRIPTOR), 
              &dwIoVal, 
              NULL);
  if (!bRet) return False;

  CMDIO_VID_ = Desc.usVendorId;
  CMDIO_PID_ = Desc.usProductId;
  CMDIO_BcdDevice = Desc.usBcdDevice;


  return True;
}


int CMDIO_GetConnectionType(BYTE *nType)
{

  int bRet;
  unsigned char temp;

  //*nType = USB_20;
  //return True;

  bRet=CMDIO_ReadCommand(IOCTL_CONNECT_TYPE,0,1,&temp);
  if (!bRet) 
    return False;
  //*nType = ( (temp & 0x01) ? USB_10 : USB_20 );
  if(temp == 1)
    *nType = USB_10;
  else if(temp == 2)
    *nType = USB_30;
  else
    *nType = USB_20;

  return True;
}



int CMDIO_GetCtrlWriteStatus()
{
  int bRet;
  unsigned char temp=0x00;

  bRet=CMDIO_ReadCommand(0x8e,0x20,1,&temp);
  if (bRet)
    bRet = ( (temp==0x55) ? True : False );
  return bRet;
}



int CMDIO_IORESET()
{
  BOOL bRet = FALSE;
  DWORD cbRet = 0;
  PIPE_TYPE  pipeType;
  pipeType = READ_DATA_PIPE;

  //::CancelIo(m_hDev);
  bRet = DeviceIoControl(m_hDev,
              (DWORD) IOCTL_CANCEL_IO, 
              (LPVOID)&pipeType,
              sizeof(PIPE_TYPE),
              NULL,
              0,
              &cbRet,
              NULL); 
  return ( bRet ? True : False );
}

int CMDIO_pipeRESET()
{
	DWORD    cbRet;
	BOOL     bRet;
	
	PIPE_TYPE   P_Type;
	
	P_Type = ALL_PIPE;

	bRet = AsyncDeviceIoCtrl(m_hDev,
                          (DWORD)IOCTL_RESET_PIPE,
                          (BYTE*)&P_Type,
                          (DWORD)sizeof(PIPE_TYPE),
                          (BYTE*)NULL, 
                          (DWORD)0,
                          &cbRet,
                          NULL);
	return ( bRet ? True : False );
}

int CMDIO_CancelIO()
{
	DWORD    cbRet;
	BOOL     bRet;
	
	PIPE_TYPE   P_Type;
	
	P_Type = ALL_PIPE;

	bRet = AsyncDeviceIoCtrl(m_hDev,
                          (DWORD)IOCTL_CANCEL_IO,
                          (BYTE*)&P_Type,
                          (DWORD)sizeof(PIPE_TYPE),
                          (BYTE*)NULL, 
                          (DWORD)0,
                          &cbRet,
                          NULL);
	return ( bRet ? True : False );
}