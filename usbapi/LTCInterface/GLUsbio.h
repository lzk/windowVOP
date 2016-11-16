#ifndef _GLUsbio_h_
#define _GLUsbio_h_
#ifndef MAX_NUM_PIPES 
 #define MAX_NUM_PIPES   8 
#endif 

//typedef enum _RAW_PIPE_TYPE { 
//    USBSCAN_PIPE_CONTROL, 
//    USBSCAN_PIPE_ISOCHRONOUS, 
//    USBSCAN_PIPE_BULK, 
//    USBSCAN_PIPE_INTERRUPT 
//} RAW_PIPE_TYPE; 
// 
//typedef struct _USBSCAN_PIPE_INFORMATION { 
//    USHORT          MaximumPacketSize;  // Maximum packet size for this pipe 
//    UCHAR           EndpointAddress;    // 8 bit USB endpoint address (includes direction) 
//    UCHAR           Interval;           // Polling interval in ms if interrupt pipe  
//    RAW_PIPE_TYPE   PipeType;           // PipeType identifies type of transfer valid for this pipe 
//} USBSCAN_PIPE_INFORMATION, *PUSBSCAN_PIPE_INFORMATION; 
// 
//typedef struct _USBSCAN_PIPE_CONFIGURATION { 
//    OUT     ULONG                          NumberOfPipes; 
//    OUT     USBSCAN_PIPE_INFORMATION       PipeInfo[MAX_NUM_PIPES]; 
//} USBSCAN_PIPE_CONFIGURATION, *PUSBSCAN_PIPE_CONFIGURATION; 
// 
// 
//#define FILE_DEVICE_USB_SCAN    0x8000 
//#define IOCTL_INDEX             0x0800 
// 
//#define IOCTL_GET_VERSION               CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX,   METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_CANCEL_IO                 CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+1, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_WAIT_ON_DEVICE_EVENT      CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+2, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_READ_REGISTERS            CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+3, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_WRITE_REGISTERS           CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+4, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_GET_CHANNEL_ALIGN_RQST    CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+5, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_GET_DEVICE_DESCRIPTOR     CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+6, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_RESET_PIPE                CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+7, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_GET_USB_DESCRIPTOR        CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+8, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_SEND_USB_REQUEST          CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+9, METHOD_BUFFERED,FILE_ANY_ACCESS) 
//#define IOCTL_GET_PIPE_CONFIGURATION    CTL_CODE(FILE_DEVICE_USB_SCAN,IOCTL_INDEX+10,METHOD_BUFFERED,FILE_ANY_ACCESS) 
 
class CGLUsb
{
public:
	CGLUsb();
	~CGLUsb();
	int CMDIO_OpenDevice();
	int CMDIO_OpenDevice(LPCTSTR lpModuleName);
	int CMDIO_CloseDevice();
	int OpenBulkPipes(HANDLE *hPipe,  LPCTSTR lpModuleName);

	int CMDIO_BulkWriteEx(int pipe, void* buffer, unsigned int dwLen);
	int CMDIO_BulkWriteEx(int pipe, void* buffer, unsigned int dwLen, DWORD	*dwByteCount);

	int CMDIO_BulkReadEx(int pipe, void *buffer, unsigned int dwLen);
	int CMDIO_BulkReadEx(int pipe, void *buffer, unsigned int dwLen,  DWORD *dwByteCount);

	int _SetIOHandle(HANDLE dwDevice, WORD wType);
	BOOL AsyncWriteFile(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, HANDLE hEvent);
	BOOL AsyncReadFile(HANDLE hFile, BYTE *Buf, DWORD dwBufSize, DWORD *dwWrite, HANDLE hEvent);
	HANDLE m_hDev, m_hIntrEvent;
	HANDLE m_hDevice[5];
	HANDLE m_GLUSBDev;	
	//TCHAR  m_strPort[32];
};

typedef int(*LPFN_NETWORK_CONNECT) (char *server, int port, int timeout);
typedef int(*LPFN_NETWORK_CONNECT_BLOCK) (char *server, int port);
typedef int(*LPFN_NETWORK_READ) (int sd, void* buff, DWORD len);
typedef int(*LPFN_NETWORK_WRITE) (int sd, void* buff, DWORD len);
typedef void(*LPFN_NETWORK_CLOSE) (int sd);

class CGLNet
{
public:
	CGLNet();
	~CGLNet();
	int CMDIO_Connect(const wchar_t* ipAddress);
	int CMDIO_Close();
	int CMDIO_Write(void* buffer, unsigned int dwLen);
	int CMDIO_Read(void *buffer, unsigned int dwLen);
	int m_socketId;
	HMODULE m_hmod;
	LPFN_NETWORK_CONNECT  m_lpfnNetworkConnect;
	LPFN_NETWORK_READ     m_lpfnNetworkRead;
	LPFN_NETWORK_WRITE    m_lpfnNetworkWrite;
	LPFN_NETWORK_CLOSE    m_lpfnNetworkClose;
};
#endif