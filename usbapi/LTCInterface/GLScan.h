#ifndef _GLScan_h_
#define _GLScan_h_
#include"GLScanStruct.h"
#include"GLUsbio.h"

class CGLDrv
{
public:
	CGLDrv();
	~CGLDrv();
	CGLUsb *m_GLusb;
	CGLNet *m_GLnet;
	BOOL  NetScanReady();
	BYTE _OpenDevice();
	BYTE _OpenDevice(LPCTSTR lpModuleName);
	BYTE _JobCreate();
	BYTE _JobEnd();
	BYTE _parameters();
	BYTE _StartScan();
	BYTE _ReadImage(int dup, int *ImgSize);
	BYTE _ReadImageEX(int dup, int *ImgSize, BYTE* Buffer, int ReadSize);
	int   paperReady();
	int   waitJobFinish(int wait_motor_stop);
	BYTE _info();
	BYTE _stop();
	BYTE _cancel();
	BYTE _CloseDevice();
	//BYTE _SetADFOptions(BYTE Mode,BYTE flowtype);
	BYTE _SetIOHandle(HANDLE dwDevice, WORD wType);
	BYTE _NVRAM_W(unsigned char addr, BYTE *pbData, unsigned char len );
	BYTE _NVRAM_R(unsigned char addr, BYTE *pbData, unsigned char len );
	BYTE _ADFCheck();
	BYTE _matrix(float *Matrix);
	BYTE _SetTime(unsigned int sleep, unsigned int auto_off);
	BYTE _gamma(unsigned int *gammatbl);
	BYTE _GetTime(unsigned int *sleep, unsigned int *auto_offunsigned ,unsigned int *dis_sleep, unsigned int *dis_auto_off);
	BYTE _Get_fw_version(char *version);
	BYTE _ButtonStatusGet(unsigned char *duplex, unsigned char *mode);
	BYTE _ResetScan();
	BYTE _StatusGet();
	BYTE _StatusCheck_Start();
	BYTE _StatusCheck_ADF_Check();
	BYTE _StatusCheck_Scanning();
	BYTE _InitializeScanner();
	BYTE _CheckScanningMode();
	//====
	SC_JOB_T			sc_job_create;
	SC_JOB_T			sc_job_end;
	SC_PAR_T			sc_par;
	SC_JOB_STA_T		job_status;
	SC_PAR_DATA_T		sc_pardata;
	SC_PAR_STA_T		par_status;
	SC_SCAN_T			sc_scan;
	SC_SCAN_STA_T		scan_status;
	SC_STOP_T			sc_stop;
	SC_STOP_STA_T		stop_status;
	SC_INFO_T			sc_info;
	SC_INFO_DATA_T		sc_infodata;
	SC_CNL_T			sc_cancel;
	SC_CNL_STA_T		cancel_status;
	SC_IMG_T			sc_img;
	SC_IMG_STA_T		img_status;
	S_NVRW				NVW ;
	S_NVRW				NVR ;
	S_NVRW_status		NVRW_status;
	SC_ADF_CHECK_T		sc_adf_check;
	SC_ADF_CHECK_STA_T	sc_adf_check_status;
	SC_STATUS_T			sc_status;
	SC_STATUS_DATA_T	sc_status_data;
	fw_version_ack		GFWV;
	fw_version_get		FWV;
	SC_SCAN_T			sc_reset;
	SC_SCAN_STA_T		reset_status;

	


	int				JobID;	
};
#endif