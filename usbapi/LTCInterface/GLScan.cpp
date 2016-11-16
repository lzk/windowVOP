#include "../stdafx.h"
#include "GLUtype.h"
#include "../usbapi.h"
#include "../Global.h"

#if _CANOPUS_DEBUG_
#define _GLDEBUG_ 1
#endif

U8	buf[0x100000];

CGLDrv::CGLDrv()
{
	m_GLusb					= new CGLUsb;
	m_GLnet					= new CGLNet();

	JobID = 0;

	sc_job_create = {0};
	sc_job_create.code		= I3('JOB');
	sc_job_create.request	= ('C');

	sc_job_end = { 0 };
	sc_job_end.code			= I3('JOB');
	sc_job_end.request		= ('E');

	sc_par = { 0 };
	sc_par.code				= I3('PAR');
	sc_par.length			= sizeof(SC_PAR_DATA_T);

	sc_pardata = { 0 };
	sc_pardata			    = { SCAN_SOURCE, SCAN_ACQUIRE, SCAN_OPTION, SCAN_DUPLEX, SCAN_PAGE,
							{ IMG_FORMAT, IMG_OPTION, IMG_BIT, IMG_MONO,
							{ IMG_DPI_X, IMG_DPI_Y },{ IMG_ORG_X, IMG_ORG_Y }, IMG_WIDTH, IMG_HEIGHT },
								//{{MTR_DRIV_TAR, MTR_STAT_MEC, MTR_DIRECT, MTR_MICRO_STEP, MTR_CURRENT, MTR_SPEED, MTR_ACC_STEP, 0},
								//{MTR_DRIV_TAR, MTR_STAT_MEC, MTR_DIRECT, MTR_MICRO_STEP, MTR_CURRENT, MTR_SPEED, MTR_ACC_STEP, 0}}
							{ { 0 },
							{ 0 } }
							};
	sc_scan = { 0 };
	sc_scan.code			= I4('SCAN');

	sc_stop = { 0 };
	sc_stop.code			= I4('STOP');

	sc_info = { 0 };
	sc_info.code			= I4('INFO');
	sc_info.length			= sizeof(SC_INFO_DATA_T);

	sc_cancel = { 0 };
	sc_cancel.code			= I4('CANC');

	sc_img = { 0 };
	sc_img.code				= I3('IMG');

	NVW.code				= I3('NVW');
	NVR.code				= I3('NVR');
	sc_adf_check.code		= I3('ADF');
	GFWV.code				= I4('VERN');
	sc_status.code			= I4('STAS');
	sc_reset.code			= I4('RSET');
}
CGLDrv::~CGLDrv()
{
	/*if (m_GLusb != NULL)
		delete m_GLusb;*/

	if (m_GLnet != NULL)
		delete(m_GLnet);
}

BYTE CGLDrv::_OpenDevice()
{
	if (g_connectMode_usb == TRUE)
	{
		return (BYTE)(m_GLusb->CMDIO_OpenDevice());
	}
	else
	{
		return (BYTE)(m_GLnet->CMDIO_Connect(g_ipAddress));
	}
}

BYTE CGLDrv::_OpenDevice(LPCTSTR lpModuleName)
{
	if (g_connectMode_usb == TRUE)
	{
		return (BYTE)(m_GLusb->CMDIO_OpenDevice(lpModuleName));
	}
	else
	{
		return (BYTE)(m_GLnet->CMDIO_Connect(lpModuleName));
	}

}

BYTE CGLDrv::_JobCreate()
{
	int result;
	U8 cmd[8] = { 'J','O','B',0,'C',0,0,0 };
	U8 status[8];
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_job_create, sizeof(sc_job_create)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &job_status, sizeof(job_status));

		/*result = (m_GLusb->CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
			m_GLusb->CMDIO_BulkReadEx(0, status, sizeof(status)) &&
			(M32(&status[0]) == I3('STA')) && (status[4] == 'A'));*/
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_job_create, sizeof(sc_job_create)) &&
			m_GLnet->CMDIO_Read(&job_status, sizeof(job_status));
		
	}

	if(!result || job_status.ack == 'E') {
	MyOutputString(L"Job create error", job_status.err);
#if _GLDEBUG_
		LTCPrintf("Job create error (#%d)\n", job_status.err);
#endif
		result = 0;
		goto exit_JobCraete;
	}
	JobID = job_status.id;
#if _GLDEBUG_
	LTCPrintf("Job create OK. ID(%d)\n", JobID);
#endif
exit_JobCraete:
	return (BYTE)result;
}
BYTE CGLDrv::_JobEnd()
{
	int result;
	sc_job_end.id = (unsigned char)JobID;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_job_end, sizeof(sc_job_end)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &job_status, sizeof(job_status));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_job_end, sizeof(sc_job_end)) &&
			m_GLnet->CMDIO_Read(&job_status, sizeof(job_status));
	}

	if(!result || job_status.ack == 'E' || job_status.id != JobID) {
		MyOutputString(L"Job end error", job_status.err);
#if _GLDEBUG_
		LTCPrintf("Job end error. err(%d), ID(%d)\n", job_status.err, job_status.id);
#endif
		result = 0;
		goto exit_JobEnd;
	}
#if _GLDEBUG_
	LTCPrintf("Job end OK.\n");
#endif
exit_JobEnd:
	return (BYTE)result;
}
BYTE CGLDrv::_parameters()
{
	int result;
	sc_par.id = (unsigned char)JobID;
	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_par, sizeof(sc_par)) &&
			m_GLusb->CMDIO_BulkWriteEx(0, &sc_pardata, sizeof(sc_pardata)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &par_status, sizeof(par_status));

	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_par, sizeof(sc_par)) &&
			m_GLnet->CMDIO_Write(&sc_pardata, sizeof(sc_pardata)) &&
			m_GLnet->CMDIO_Read(&par_status, sizeof(par_status));
	}

	if(!result || par_status.ack == 'E' || par_status.id != JobID) {
		MyOutputString(L"Set parameter error", par_status.err);
#if _GLDEBUG_
		LTCPrintf("Set parameter error. err(%d), ID(%d)\n", par_status.err, par_status.id);
#endif
		result = 0;
		goto exit_par;
	}
#if _GLDEBUG_
	LTCPrintf("Set parameter OK.\n");
#endif
exit_par:
	return (BYTE)result;
}
BYTE CGLDrv::_StartScan()
{
	int result;
	sc_scan.id = (unsigned char)JobID;
	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_scan, sizeof(sc_scan)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &scan_status, sizeof(scan_status));

	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_scan, sizeof(sc_scan)) &&
			m_GLnet->CMDIO_Read(&scan_status, sizeof(scan_status));
	}

	if(!result || scan_status.ack == 'E' || scan_status.id != JobID) {
		MyOutputString(L"Start scan error", scan_status.err);
#if _GLDEBUG_
		LTCPrintf("Start scan error. err(%d), ID(%d)\n", scan_status.err, scan_status.id);
#endif
		result = 0;
		goto exit_StartScan;
	}
#if _GLDEBUG_
	LTCPrintf("Start scan OK.\n");
#endif
exit_StartScan:
	return (BYTE)result;
}
BYTE CGLDrv::_ReadImage(int dup, int *ImgSize) //not work
{
	int result;

	sc_img.side = dup;
	sc_img.length = sc_infodata.ValidPageSize[dup];
	if (sizeof(buf) < sc_infodata.ValidPageSize[dup]) {
		sc_img.length = sizeof(buf);
	}

	result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_img, sizeof(sc_img)) &&
		m_GLusb->CMDIO_BulkReadEx(0, &img_status, sizeof(img_status));
	if(!result || img_status.ack == 'E') {
		printf("Get image status error.\n");
		result = 0;
		return FALSE;
	}

	//printf("img_status.length = %d\n",img_status.length);

	result = m_GLusb->CMDIO_BulkReadEx(0, buf, img_status.length/*sizeof(buf)*/);
	if(!result) {
		printf("Get image data error.\n");
		result = 0;
		return FALSE;
	}
	*ImgSize = img_status.length;
	printf("%c", 'A' + dup);
	return result;
}
BYTE CGLDrv::_stop()
{
	int result;
	sc_stop.id = (unsigned char)JobID;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_stop, sizeof(sc_stop)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &stop_status, sizeof(stop_status));

	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_stop, sizeof(sc_stop)) &&
			m_GLnet->CMDIO_Read(&stop_status, sizeof(stop_status));
	}

	if(!result || stop_status.ack == 'E' || stop_status.id != JobID) {
		MyOutputString(L"Stop scan error", stop_status.err);
#if _GLDEBUG_
		LTCPrintf("Stop scan error. err(%d), ID(%d)\n", stop_status.err, stop_status.id);
#endif
		result = 0;
		goto exit_stop;
	}
#if _GLDEBUG_
	LTCPrintf("Stop scan OK.\n");
#endif
exit_stop:
	return (BYTE)result;
}

int CGLDrv::paperReady()
{
	int ready = TRUE;
	if (sc_pardata.source == I3('ADF') && !(sc_pardata.acquire & (ACQ_NO_PP_SENSOR + ACQ_MOTOR_OFF + ACQ_PSEUDO_SENSOR))) {
		if (!_info() || !sc_infodata.DocSensor) {
			ready = FALSE;
		}
	}
	return ready;
}

#define JOB_WAIT_TIMEOUT  5000
int CGLDrv::waitJobFinish(int wait_motor_stop)
{
	U32 tick = GetTickCount();
	while ((GetTickCount() - tick) < JOB_WAIT_TIMEOUT) {
		if (!_info())
			break;
		if (!(sc_infodata.JobState & 1) && (!wait_motor_stop || !sc_infodata.MotorMove))
			return TRUE;
		Sleep(100);
	}
	return FALSE;
}

BYTE CGLDrv::_info()
{
	int result;
	sc_info.id = (unsigned char)JobID;
	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_info, sizeof(sc_info));
		if (!result)
		{
			result = 0;
			goto exit_info;
		}
		//Sleep(3);
		result = result && m_GLusb->CMDIO_BulkReadEx(0, &sc_infodata, sizeof(sc_infodata));

	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_info, sizeof(sc_info));
		if (!result)
		{
			result = 0;
			goto exit_info;
		}
		//Sleep(3);
		result = result && m_GLnet->CMDIO_Read(&sc_infodata, sizeof(sc_infodata));
	}

	if (start_cancel) {
		sc_infodata.Cancel = 1;
	}

	/*if(sc_infodata.CoverOpen || sc_infodata.PaperJam || sc_infodata.Cancel)
	{
#if _GLDEBUG_
		if(sc_infodata.CoverOpen)
			printf("Cover open!\n");
		if(sc_infodata.PaperJam)
			printf("Parer jam!\n");
		if(sc_infodata.Cancel)
			printf("Job cancel!\n");
#endif
		result = 0;
		goto exit_info;
	}
	else if(!result)
	{
		result = 0;
		goto exit_info;
	}*/
	if(!result || sc_infodata.code != I4('IDAT') || sc_infodata.Cancel) {
		//MyOutputString(L"Scan info error", sc_infodata.Error);
#if _GLDEBUG_
		if(sc_infodata.Error)
			LTCPrintf("Status error!\n");
		if(sc_infodata.Cancel)
			LTCPrintf("Job cancel!\n");
#endif
		result = 0;
		goto exit_info;
	}

exit_info:
	/*LTCPrintf2("GLScan::_info vp %d/%d size %d/%d ep %d/%d ed %d\n", 
		sc_infodata.ValidPage[0], sc_infodata.ValidPage[1], 
		sc_infodata.ValidPageSize[0], sc_infodata.ValidPageSize[1], 
		sc_infodata.EndPage[0], sc_infodata.EndPage[1],
		sc_infodata.EndDocument);*/
	return (BYTE)result;
}
BYTE CGLDrv::_cancel()
{
	int result;
#if _GLDEBUG_
	LTCPrintf("\n\tCancel Scan...");
#endif

	sc_cancel.id = (unsigned char)JobID;
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_cancel, sizeof(sc_cancel)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &cancel_status, sizeof(cancel_status));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_cancel, sizeof(sc_cancel)) &&
			m_GLnet->CMDIO_Read(&cancel_status, sizeof(cancel_status));
	}


	if(!result || cancel_status.ack == 'E' || cancel_status.id != JobID) {
		MyOutputString(L"Scan cancel error", cancel_status.err);
#if _GLDEBUG_
		LTCPrintf("Fail\n");
#endif
		result = 0;
		goto exit_cancel;
	}
#if _GLDEBUG_
	LTCPrintf("OK\n");
#endif
exit_cancel:
	return (BYTE)result;
}
BYTE CGLDrv::_CloseDevice()
{
	if (g_connectMode_usb == TRUE)
	{
		return (BYTE)(m_GLusb->CMDIO_CloseDevice());
	}
	else
	{
		return (BYTE)(m_GLnet->CMDIO_Close());
	}
	
}
BYTE CGLDrv::_ReadImageEX(int dup, int *ImgSize,BYTE* Buffer,int ReadSize)
{
	int result;
	sc_img.side = dup;
	sc_img.length = ReadSize;
	if(sc_img.length > 0x100000) //for GL cmd
		sc_img.length = 0x100000;
	if(sc_img.length > sc_infodata.ValidPageSize[dup])
	{
		sc_img.length = sc_infodata.ValidPageSize[dup];
	}
	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_img, sizeof(sc_img));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_img, sizeof(sc_img));
	}

	if(!result)
	{
		MyOutputString(L"Get image status error");
#if _GLDEBUG_
		LTCPrintf("Get image status error.\n");
#endif
		result = 0;
		return FALSE;
	}
	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkReadEx(0, Buffer, sc_img.length);
	}
	else
	{
		result = m_GLnet->CMDIO_Read(Buffer, sc_img.length);
	}

	if(!result) {
		MyOutputString(L"Get image data error");
#if _GLDEBUG_
		LTCPrintf("Get image data error.\n");
#endif
		result = 0;
		return FALSE;
	} 
	*ImgSize = sc_img.length;
	//printf("%c", 'A'+dup);
	//LTCPrintf2("GLScan::_ReadImageEX dup %d length %d\n", dup, img_status.length);
	return (BYTE)result;
}
BYTE CGLDrv::_SetIOHandle(HANDLE dwDevice, WORD wType)
{
	return (BYTE)(m_GLusb->_SetIOHandle(dwDevice,wType));
}
BYTE CGLDrv::_NVRAM_W(unsigned char addr, BYTE *pbData, unsigned char len )
{
	int result;

	NVW.straddr = addr;
	NVW.length = len;


	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &NVW, sizeof(S_NVRW)) &&
			m_GLusb->CMDIO_BulkWriteEx(0, pbData, NVW.length) &&
			m_GLusb->CMDIO_BulkReadEx(0, &NVRW_status, sizeof(NVRW_status));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&NVW, sizeof(S_NVRW)) &&
			m_GLnet->CMDIO_Write(pbData, NVW.length) &&
			m_GLnet->CMDIO_Read(&NVRW_status, sizeof(NVRW_status));
	}

	if(!result || NVRW_status.ack == 'E') {
		LTCPrintf("NVW test error (#%d)\n", NVRW_status.err);
		result = 0;
		goto exit_NVTEST_W;
	}
#if _GLDEBUG_
	LTCPrintf("NVW test OK.\n");
#endif
exit_NVTEST_W:
	return (BYTE)result;
}
BYTE CGLDrv::_NVRAM_R(unsigned char addr, BYTE *pbData, unsigned char len )
{
	int result;
	
	
	NVR.straddr = addr;
	NVR.length = len;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &NVR, sizeof(S_NVRW)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &NVRW_status, sizeof(NVRW_status));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&NVR, sizeof(S_NVRW)) &&
			m_GLnet->CMDIO_Read(&NVRW_status, sizeof(NVRW_status));
	}

	if(!result || NVRW_status.ack == 'E') {
		LTCPrintf("NVR test error (#%d)\n", NVRW_status.err);
		result = 0;
		goto exit_NVTEST_R;
	}


	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkReadEx(0, pbData, NVR.length);
	}
	else
	{
		result = m_GLnet->CMDIO_Read(pbData, NVR.length);
	}

#if _GLDEBUG_
	LTCPrintf("NVR test OK.\n");
#endif

exit_NVTEST_R:

	return (BYTE)result;
}
BYTE CGLDrv::_ADFCheck()
{
	int result;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_adf_check, sizeof(SC_ADF_CHECK_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_adf_check_status, sizeof(SC_ADF_CHECK_STA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_adf_check, sizeof(SC_ADF_CHECK_T)) &&
			m_GLnet->CMDIO_Read(&sc_adf_check_status, sizeof(SC_ADF_CHECK_STA_T));
	}

	if(!result ||  sc_adf_check_status.ack == 'E') {
		MyOutputString(L"ADF check error", sc_adf_check_status.err);
#if _GLDEBUG_
		LTCPrintf("ADF check error (#%d)\n", sc_adf_check_status.err);
#endif
		result = 0;
		goto exit_ADFCheck;
	}
exit_ADFCheck:
	return (BYTE)result;
}
BYTE CGLDrv::_matrix(float *Matrix)
{
	SC_PAR_T		sc_matrix =		{I4('MATX')};
	SC_PAR_STA_T	sc_matrix_status = {0};
	int result;
	int i;
	float ColorMatrix[9];
	sc_matrix.id = (unsigned char)JobID;
	sc_matrix.length = sizeof(ColorMatrix);
	for(i=0;i<9;i++)
		ColorMatrix[i]= *(Matrix+i);


	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_matrix, sizeof(SC_PAR_T)) &&
			m_GLusb->CMDIO_BulkWriteEx(0, ColorMatrix, sc_matrix.length) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_matrix_status, sizeof(SC_PAR_STA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_matrix, sizeof(SC_PAR_T)) &&
			m_GLnet->CMDIO_Write(ColorMatrix, sc_matrix.length) &&
			m_GLnet->CMDIO_Read(&sc_matrix_status, sizeof(SC_PAR_STA_T));
	}

	if(!result || sc_matrix_status.ack == 'E' || sc_matrix_status.id != JobID) {
#if _GLDEBUG_
		LTCPrintf("Load matrix table error. err(%d), ID(%d)\n", sc_matrix_status.err, sc_matrix_status.id);
#endif
		result = 0;
		goto exit_matrix;
	}
#if _GLDEBUG_
	LTCPrintf("Load matrix table OK.\n");
#endif
exit_matrix:
	return (BYTE)result;
}
BYTE CGLDrv::_SetTime(unsigned int sleep, unsigned int auto_off)
{

	SC_PAR_T		sc_time =		{I4('TIME')};
	SC_PAR_STA_T	sc_time_status = {0};
	int result;
	unsigned int time_minsec[2];

	time_minsec[0] = sleep*60*1000;
	time_minsec[1] = auto_off*60*1000;
	//=====================================
	sc_time.reserved = 1;   //For set time
	//=====================================

	sc_time.length = sizeof(time_minsec);

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_time, sizeof(SC_PAR_T)) &&
			m_GLusb->CMDIO_BulkWriteEx(0, &time_minsec, sc_time.length) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_time_status, sizeof(SC_PAR_STA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_time, sizeof(SC_PAR_T)) &&
			m_GLnet->CMDIO_Write(&time_minsec, sc_time.length) &&
			m_GLnet->CMDIO_Read(&sc_time_status, sizeof(SC_PAR_STA_T));
	}

	if(!result || sc_time_status.ack == 'E') {
#if _GLDEBUG_
		LTCPrintf("Time set error.\n");
#endif
		result = 0;
		goto exit;
	}
#if _GLDEBUG_
	LTCPrintf("Time set OK.\n");
#endif

exit:
	return (BYTE)result;
}
BYTE CGLDrv::_gamma(unsigned int *gammatbl)
{
	SC_PAR_T		sc_gamma =		{I4('GAMA')};
	SC_PAR_STA_T	sc_gamma_status = {0};
	int result;

	sc_gamma.id = (unsigned char)JobID;
	sc_gamma.length = sizeof(int)*768;

	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_gamma, sizeof(SC_PAR_T)) &&
			m_GLusb->CMDIO_BulkWriteEx(0, gammatbl, sc_gamma.length) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_gamma_status, sizeof(SC_PAR_STA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_gamma, sizeof(SC_PAR_T)) &&
			m_GLnet->CMDIO_Write(gammatbl, sc_gamma.length) &&
			m_GLnet->CMDIO_Read(&sc_gamma_status, sizeof(SC_PAR_STA_T));
	}

	if(!result || sc_gamma_status.ack == 'E' || sc_gamma_status.id != JobID) {
#if _GLDEBUG_
		LTCPrintf("Load gamma table error. err(%d), ID(%d)\n", sc_gamma_status.err, sc_gamma_status.id);
#endif
		result = 0;
		goto exit_gamma;
	}
#if _GLDEBUG_
	LTCPrintf("Load gamma table OK.\n");
#endif
exit_gamma:
	return (BYTE)result;
}
BYTE CGLDrv::_GetTime(unsigned int *sleep, unsigned int *auto_off ,unsigned int *dis_sleep, unsigned int *dis_auto_off)
{
	SC_PAR_T		sc_time =		{I4('TIME')};
	SC_PAR_STA_T	sc_time_status= {0};
	int result;
	
	unsigned int time_minsec[4];

	//=====================================
	sc_time.reserved = 0;   //For get time
	//=====================================
	sc_time.length = sizeof(time_minsec);
	

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_time, sizeof(SC_PAR_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_time_status, sizeof(SC_PAR_STA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_time, sizeof(SC_PAR_T)) &&
			m_GLnet->CMDIO_Read(&sc_time_status, sizeof(SC_PAR_STA_T));
	}
	
	if(!result || sc_time_status.ack == 'E') {
#if _GLDEBUG_
		LTCPrintf("Get time error (#%d)\n", sc_time_status.err);
#endif
		result = 0;
		goto exit;
	}

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkReadEx(0, &time_minsec, sc_time.length);
	}
	else
	{
		result = m_GLnet->CMDIO_Read(&time_minsec, sc_time.length);
	}
			
	*sleep = time_minsec[0]/1000/60;
	*auto_off = time_minsec[1]/1000/60;
	*dis_sleep = time_minsec[2];
	*dis_auto_off = time_minsec[3];

#if _GLDEBUG_
	LTCPrintf("Get time OK.\n");
#endif

/*
	sc_time.length = sizeof(time_minsec);
	
	result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_time, sizeof(SC_PAR_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_time_status, sizeof(SC_PAR_STA_T));
	
	if(!result || sc_time_status.ack == 'E') {
#if _GLDEBUG_
		printf("Get time error (#%d)\n", sc_time_status.err);
#endif
		result = 0;
		goto exit;
	}

	result = m_GLusb->CMDIO_BulkReadEx(0, &time_minsec, sc_time.length);
			
	*sleep = time_minsec[0]/1000/60;
	*auto_off = time_minsec[1]/1000/60;*/
#if _GLDEBUG_
	printf("Get time OK.\n");
#endif

exit:
	return (BYTE)result;
}
BYTE CGLDrv::_Get_fw_version(char *version)
{
	int result;
	
	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &GFWV, sizeof(fw_version_ack)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &FWV, sizeof(fw_version_get));

		result = m_GLusb->CMDIO_BulkReadEx(0, version, FWV.length);
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&GFWV, sizeof(fw_version_ack)) &&
			m_GLnet->CMDIO_Read(&FWV, sizeof(fw_version_get));

		result = m_GLnet->CMDIO_Read(version, FWV.length);
	}

	version[FWV.length]='\0';
	if(!result || (FWV.check == 'E')) {
#if _GLDEBUG_
		LTCPrintf("Get FW version error.\n");
#endif
		result = 0;
		goto exit_Get_fw_version;
	}
#if _GLDEBUG_
	LTCPrintf("FW version = %s\n", version);
#endif

exit_Get_fw_version:
	return (BYTE)result;
}

BYTE CGLDrv::_ButtonStatusGet(unsigned char *duplex, unsigned char *mode)
{
	SC_PAR_T get_button = {I4('BTON')};
	int result;
	unsigned char button_status[4];


	if (g_connectMode_usb == TRUE)
	{

		result = m_GLusb->CMDIO_BulkWriteEx(0, &get_button, sizeof(SC_PAR_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, button_status, sizeof(button_status));
	}
	else
	{

		result = m_GLnet->CMDIO_Write(&get_button, sizeof(SC_PAR_T)) &&
			m_GLnet->CMDIO_Read(button_status, sizeof(button_status));
	}

     *duplex = button_status[0];
     *mode = button_status[1];
    // *scan = button_status[2];  //for at 
    // *cancel = button_status[3]; //for at

     //printf("Duplex button status= %d\n", button_status[0]);
     //printf("Mode button status= %d\n", button_status[1]);
  

	return (BYTE)result;
}
BYTE CGLDrv::_ResetScan()
{
     int result;
   
	 if (g_connectMode_usb == TRUE)
	 {

		 result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_reset, sizeof(sc_reset)) &&
			 m_GLusb->CMDIO_BulkReadEx(0, &reset_status, sizeof(reset_status));
	 }
	 else
	 {

		 result = m_GLnet->CMDIO_Write(&sc_reset, sizeof(sc_reset)) &&
			 m_GLnet->CMDIO_Read(&reset_status, sizeof(reset_status));
	 }


     if(!result || reset_status.ack == 'E') {
		 MyOutputString(L"Reset scan flow error", reset_status.err);
#if _GLDEBUG_
          printf("Reset scan flow error. err(%d), ID(%d)\n", reset_status.err, reset_status.id);
#endif
          result = 0;
          goto exit_StartScan;
     }
#if _GLDEBUG_
     printf("Reset scan flow OK.\n");
#endif
exit_StartScan:
     return (BYTE)result;
}
BYTE CGLDrv::_InitializeScanner()
{
	BYTE result=1;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_status, sizeof(SC_STATUS_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_status_data, sizeof(SC_STATUS_DATA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_status, sizeof(SC_STATUS_T)) &&
			m_GLnet->CMDIO_Read(&sc_status_data, sizeof(SC_STATUS_DATA_T));
	}
	
	if(!result) {
#if _GLDEBUG_
		LTCPrintf("Get status error!\n");
#endif
		result = 0;
		goto exit__InitializeScanner;
	}
	if((sc_status_data.system & 0x10)&&(sc_status_data.system & 0x01)) { // scanning mode and time out
          printf("Last scan flow timeout!\n");
          result = _ResetScan();
          if(!result)
               goto exit__InitializeScanner;
     }

exit__InitializeScanner:
	return result;
}
BYTE CGLDrv::_CheckScanningMode()
{
	BYTE result=1;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_status, sizeof(SC_STATUS_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_status_data, sizeof(SC_STATUS_DATA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_status, sizeof(SC_STATUS_T)) &&
			m_GLnet->CMDIO_Read(&sc_status_data, sizeof(SC_STATUS_DATA_T));
	}

	
	if(!result) {
#if _GLDEBUG_
		LTCPrintf("Get status error!\n");
#endif
		result = 0;
	}
	
	return (sc_status_data.system & 0x01);
}
BYTE CGLDrv::_StatusGet()
{
	BYTE result=1;

	if (g_connectMode_usb == TRUE)
	{
		result = m_GLusb->CMDIO_BulkWriteEx(0, &sc_status, sizeof(SC_STATUS_T)) &&
			m_GLusb->CMDIO_BulkReadEx(0, &sc_status_data, sizeof(SC_STATUS_DATA_T));
	}
	else
	{
		result = m_GLnet->CMDIO_Write(&sc_status, sizeof(SC_STATUS_T)) &&
			m_GLnet->CMDIO_Read(&sc_status_data, sizeof(SC_STATUS_DATA_T));
	}

	
	if(!result) {
		MyOutputString(L"Get status error");
#if _GLDEBUG_
		LTCPrintf("Get status error!\n");
#endif
		result = 0;
		goto exit_StatusGet;
	}
exit_StatusGet:
	return result;
}

BYTE CGLDrv::_StatusCheck_Start()
{
	BYTE result=1;
	
	/*=====================================
		system:
		bit(1,2): (0,0) =  idle  , (0,1) = scanning  ,  (1,0) = initializing  ,  (1,1) = sleeping
		bit3: flash under read/write
		bit4: scaner under error status
	=====================================*/

	/*=====================================
		sensor:  0 means OK, 1 is not OK 
		bit1: FLB home sensor
		bit2: ADF DOC sensor
		bit3: ADF scan sensor
		bit4: ADF cover sensor
	=====================================*/

	/*=====================================
		error:
		bit1: FLB home initial fail
		bit2: ADF initial fail
		bit3: Cover open
		bit4: Parer jam
	=====================================*/
	/*
	if(sc_status_data.system & 0x04) {
#if _GLDEBUG_
		printf("System not ready!\n");
#endif
		result = 0;
	}*/
	if(sc_status_data.system & 0x01) {
#if _GLDEBUG_
		LTCPrintf("scanning status!\n");
#endif
		result = 0;
	}

/*	if( sc_status_data.error & 0x01 ) {
#if _GLDEBUG_
		printf("FLB home initial fail!\n");
#endif
		result = 0;
	}*/
	/*if( sc_status_data.error & 0x02 ) {
#if _GLDEBUG_
		printf("ADF initial fail!\n");
#endif
		result = 0;
	}*/
	/*if( sc_status_data.error & 0x04 ) {
#if _GLDEBUG_
		printf("Cover open!\n");
#endif
		result = 0;
	}*/
	if( sc_status_data.error & 0x08 ) {
#if _GLDEBUG_
		LTCPrintf("Paper jam!\n");
#endif
		result = 0;
	}
	if(  sc_status_data.error & 0x04   ) {
#if _GLDEBUG_
		LTCPrintf("Cover Open Error!\n");
#endif
		result = 0;
	}

	if(!result) {
		//printf("Status check fail!\n");
		goto exit_StatusCheck;
	}
exit_StatusCheck:
	return result;
}

BYTE CGLDrv::_StatusCheck_ADF_Check()
{
	BYTE result=1;
	
	/*=====================================
		system:
		bit(1,2): (0,0) =  idle  , (0,1) = scanning  ,  (1,0) = initializing  ,  (1,1) = sleeping
		bit3: flash under read/write
		bit4: scaner under error status
	=====================================*/

	/*=====================================
		sensor:
		bit1: FLB home sensor
		bit2: ADF DOC sensor
		bit3: ADF scan sensor
		bit4: ADF cover sensor
	=====================================*/

	/*=====================================
		error:
		bit1: FLB home initial fail
		bit2: ADF initial fail
		bit3: Cover open
		bit4: Parer jam
	=====================================*/
	
	if(sc_status_data.system & 0x02) {
#if _GLDEBUG_
		LTCPrintf("System not ready!\n");
#endif
		result = 0;
	}
	if( sc_status_data.sensor & 0x02 ) {
#if _GLDEBUG_
		LTCPrintf("Sensor status fail!(doc)\n");
#endif
		result = 0;
	}
	if( sc_status_data.sensor & 0x04 ) {
#if _GLDEBUG_
		LTCPrintf("Sensor status fail!(scan)\n");
#endif
		result = 0;
	}
	if( sc_status_data.sensor & 0x08 ) {
#if _GLDEBUG_
		LTCPrintf("Sensor status fail!(cover)\n");
#endif
		result = 0;
	}
	if(  sc_status_data.error & 0x04  ) {
#if _GLDEBUG_
		LTCPrintf("Cover Open Error!\n");
#endif
		result = 0;
	}
	if( sc_status_data.error & 0x08 ) {
#if _GLDEBUG_
		LTCPrintf("Paper jam!\n");
#endif
		result = 0;
	}
	/*printf("sc_status_data.system = 0x%02x\n", sc_status_data.system);
	printf("sc_status_data.sensor = 0x%02x\n", sc_status_data.sensor);
	printf("sc_status_data.error = 0x%02x\n", sc_status_data.error);*/
	if(!result) {
		//printf("Status check fail!\n");
		goto exit_StatusCheck;
	}
exit_StatusCheck:
	return result;
}

BYTE CGLDrv::_StatusCheck_Scanning()
{
	BYTE result=1;
	
	/*=====================================
		system:
		bit(1,2): (0,0) =  idle  , (0,1) = scanning  ,  (1,0) = initializing  ,  (1,1) = sleeping
		bit3: flash under read/write
		bit4: scaner under error status
	=====================================*/

	/*=====================================
		sensor:  0 means OK, 1 is not OK 
		bit1: FLB home sensor
		bit2: ADF DOC sensor
		bit3: ADF scan sensor
		bit4: ADF cover sensor
	=====================================*/

	/*=====================================
		error:
		bit1: FLB home initial fail
		bit2: ADF initial fail
		bit3: Cover open
		bit4: Parer jam
	=====================================*/
	
	if(sc_status_data.system & 0x08) {
		//printf("System in error!\n");
	//	result = 0;
	}

	//if( (sc_status_data.sensor & 0x08) && (sc_pardata.source != I2('FK'))  && (sc_pardata.source != I3('FLB')) ) {
/*	if( (sc_status_data.sensor & 0x08) ) {
		printf("Cover open!\n");
		result = 0;
	}
*/
	if( sc_status_data.error & 0x04  ) {
#if _GLDEBUG_
		LTCPrintf("Cover open!\n");
#endif
		result = 0;
	}
	if( sc_status_data.error & 0x08 ) {
#if _GLDEBUG_
		LTCPrintf("Paper jam!\n");
#endif
		result = 0;
	}

	/*printf("sc_status_data.system = 0x%02x\n", sc_status_data.system);
	printf("sc_status_data.sensor = 0x%02x\n", sc_status_data.sensor);
	printf("sc_status_data.error = 0x%02x\n", sc_status_data.error);*/
	if(!result) {
		//printf("Status check fail!\n");
		goto exit_StatusCheck;
	}
exit_StatusCheck:
	return result;
}

