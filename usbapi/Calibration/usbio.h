////////////////////////////////////////////////////////////////
// File - usbio.h
// Copyright (c) 2011 - Genesys Logic, INC.
//
// define the export function and global variable for usbio.c 
//
////////////////////////////////////////////////////////////////

#ifndef _usbio_h
#define _usbio_h


#define  IOCTL_REG_ADDR             0x83
#define  IOCTL_REG_WRITE            0x85
#define  IOCTL_REG_READ             0x84
#define  IOCTL_BUF_ACCESS           0x82
#define  IOCTL_BUF_ENDACCESS        0x8D 
#define  IOCTL_CTL_SPPCTL           0x87
#define  IOCTL_CTL_WRITECTRL        0x87
#define  IOCTL_CTL_READSTATUS       0x86
#define  IOCTL_USB_WRITEREG         0x8C
#define  IOCTL_CONNECT_TYPE         0x8E
#define  IOCTL_GPIO_DIRECTION       0x89
#define  IOCTL_GPIO_READ            0x8A
#define  IOCTL_GPIO_WRITE           0x8B

union SetupCmd{
  unsigned char buf[8];
  unsigned int val[2];
  unsigned short  wval[4];
};

int CMDIO_WriteCommand(unsigned short nCmd, unsigned short nIdx, unsigned short nCmdLen, unsigned char *pCmdData); 
int CMDIO_ReadCommand(unsigned short nCmd, unsigned short nIdx, unsigned short nCmdLen, unsigned char *pCmdData);
int CMDIO_BulkRead(unsigned char *buffer, unsigned int dwLen);
int CMDIO_BulkWrite(unsigned char *buffer, unsigned int dwLen);
int CMDIO_GetDeviceFeatures();
int CMDIO_GetCtrlWriteStatus();

int CMDIO_GetDeviceID(unsigned short *vid,unsigned short *pid); //DEVICE_DESCRIPTOR *pDesc);
int CMDIO_GetConnectionType(unsigned char *nType);
int CMDIO_InterruptIoCtl(unsigned char *Buf, unsigned int dwLen);
int CMDIO_CancelBulkRead(int flush);
int CMDIO_IORESET();

int CMDIO_OpenDevice();
int CMDIO_CloseDevice();
int CMDIO_BulkWriteEx(int pipe, void *buffer, unsigned int dwLen);
int CMDIO_BulkReadEx(int pipe, void *buffer, unsigned int dwLen);
int CMDIO_InterruptRead(unsigned char *Buf, unsigned int dwLen);
int CMDIO_InterruptWait(int wait_ms);
int CMDIO_InterruptCancel();
int CMDIO_BulkReadEx2(int pipe, void *buffer, unsigned int dwLen);
int CMDIO_BulkReadEx2Wait(int pipe, int wait_ms);
int CMDIO_BulkWriteEx2(int pipe, void* buffer, unsigned int dwLen);
int CMDIO_BulkWriteEx2Wait(int pipe, int wait_ms);
int CMDIO_AbortBulkRead(int pipe);
int CMDIO_AbortBulkWrite(int pipe);

//unsigned short CMDIO_VID_,CMDIO_PID_;
//unsigned short CMDIO_BcdDevice;
//unsigned char CMDIO_iConnectType;
//unsigned int  CMDIO_BulkFiFoSize;
//int CMDIO_doCtrlCheck;

#endif // _usbio_h


