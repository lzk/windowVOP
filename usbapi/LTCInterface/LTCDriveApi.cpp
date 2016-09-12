#include "../StdAfx.h"

#define JPEG_MODIFY_HEADER 1
#define RAWIMGSIZE  0x8000000
#define JPEGIMGSIZE 0x5000000

#define ACQ_SHADING			 0x01
#define ACQ_GAMMA			(0x01 << 1)
#define ACQ_MIRROR			(0x01 << 2)
#define ACQ_LAMP_OFF		(0x01 << 3)
#define ACQ_START_HOME		(0x01 << 4)
#define ACQ_BACK_TRACK_OFF	(0x01 << 5)
#define ACQ_AUTO_GO_HOME	(0x01 << 6)
#define ACQ_STILL_SCAN		(0x01 << 7)
#define ACQ_STARTSTOP_TEST	(0x01 << 8)
#define ACQ_WRITE_FLASH		(0x01 << 9)  //only for calibration to use
#define ACQ_RODLENS			(0x01 << 10) //for RodLens scan use

CLTCDrv::CLTCDrv()
{
	m_GLDrv			= new CGLDrv;
	ReadScan_EndFlag= 0;
	byOpenRefCount	= 0;
	bADFOption		= 0;
	byADFMode		= 0;
	byEndDoc		= 1;		
	bJobCreatFlag	= FALSE;
	ReadSizeCountB	= 0; 
	ImgBShift		= 0;
	ScanInch_W		= 0;
	dwErrorCode     = 0;
	bCompress		= FALSE;
	bGetSourceImg	= FALSE;
	bOverScan		= FALSE;
	bGamma			= FALSE;
	bTonemap		= FALSE;
	bReadImg		= FALSE; //for cancel
	ImgBEnd			= FALSE;
	ImgBDocEnd		= FALSE;
	bImgAEnd		= FALSE;
	bImgBEnd		= FALSE;
	bStartScanFlag  = FALSE;
	bScale			= FALSE;
	bColormatrix	= FALSE;
	bCancel			= FALSE;
	bBWmodeScan		= FALSE;
	bflag_decode_done=FALSE;
	bStopTimeOutFlag=FALSE;
	bFirstReadScanEX= TRUE;
	Source_end_page[0] = Source_end_page[1] = 0;
	end_page[0] = end_page[1] = 0;
	page_line[0] = page_line[1] = 0;
	pDupImg			= NULL;
	raw_out_buf		= NULL;
	jpeg_out_buf	= NULL;
	ImageA			= new IMGInfo;
	ImageB			= new IMGInfo;
	Image_tmp		= new IMGInfo;
	ImageIn			= new IMGInfo;
	ImageOut		= new IMGInfo;
}

CLTCDrv::~CLTCDrv()
{
	delete m_GLDrv,ImageA,ImageB,Image_tmp,ImageIn,ImageOut;
}

BYTE CLTCDrv::FindScannerEx()
{
	BOOL	bRet = TRUE;
	if (byOpenRefCount == 0)
	{
		//m_pLog->LogPrintf("Lamp is ");
		bRet = m_GLDrv->_OpenDevice();
	}
	if (bRet == TRUE)
	{
		byOpenRefCount++;
	}
	return ((BYTE)bRet);
}

BYTE CLTCDrv::FindScannerEx(LPCTSTR lpModuleName)
{
	BOOL	bRet=TRUE;
	if(byOpenRefCount==0)
	{
		//m_pLog->LogPrintf("Lamp is ");
		bRet=m_GLDrv->_OpenDevice(lpModuleName);
	}
	if(bRet==TRUE)
	{
		byOpenRefCount++;
	}
	return ((BYTE)bRet);
}

BYTE CLTCDrv::InitializeDriver(VOID)
{
	BOOL	bRet=TRUE;
	return ((BYTE)bRet);
}
BYTE CLTCDrv::TerminateDriver(VOID)
{
	if(bStartScanFlag)
	{
		bStartScanFlag = FALSE;
		m_GLDrv->_stop();
	}
	if(bJobCreatFlag)
	{
		bJobCreatFlag = FALSE;
		m_GLDrv->_JobEnd();
	}
	BOOL	bRet = TRUE;
	if(byOpenRefCount <= 0)
		return (TRUE);

	byOpenRefCount--;

	if(byOpenRefCount==0)
	{
		bRet=m_GLDrv->_CloseDevice();
	}

	return ((BYTE)bRet);
}
BYTE CLTCDrv::InitializeScanner(VOID)
{
	m_GLDrv->_InitializeScanner();
	return 1;
}
BYTE CLTCDrv::GetADFMode(LPADFPARAMETER pADFParam)
{
	pADFParam = pADFParam;
	return 0;
}
BYTE CLTCDrv::GetScannerAbilityEx(LPSCANNERABILITYEX pAbilityEX)
{
	pAbilityEX=pAbilityEX;
	return 0;
}
BYTE CLTCDrv::SetScanParameter(LPLTCSCANPARAMETER lpScanParam)
{
	BYTE bRet;
	if(bCancel == TRUE)
	{
		dwErrorCode = ERRCODE_Cancel;
		return FALSE;
	}
	bRet = m_GLDrv->_StatusGet();
	if(!bRet) {
		LTCPrintf("_StatusGet fail!\n");
		dwErrorCode = ERRCODE_ScannerDisconnect;
		return FALSE;
	}
	bRet = m_GLDrv->_StatusCheck_Start();
	if(!bRet) {
		ErrorMapping_START();
		return FALSE;
	}
	m_GLDrv->sc_job_create.mode = (bADFOption) ? I1('A'):I1('F');
	bRet = m_GLDrv->_JobCreate();
	if(bRet)
		bJobCreatFlag = TRUE;
	else
		return FALSE;
	
	ScanInfo.Formate	=  (bCompress)? I3('JPG') : I3('RAW');
	ScanInfo.bShading   = TRUE;
	ScanInfo.OutputXres = lpScanParam->XRes;
	ScanInfo.OutputYres = lpScanParam->YRes;
	ScanInfo.OutputW	= lpScanParam->Width;
	ScanInfo.OutputH	= lpScanParam->Length;
	ScanInfo.SourceXres = GetSourceRes(ScanInfo.OutputXres);
	ScanInfo.SourceYres = GetSourceRes(ScanInfo.OutputYres);
	ScanInfo.BitsPerPixel = lpScanParam->BitsPerPixel;
	if((ScanInfo.SourceXres != ScanInfo.OutputXres)||(ScanInfo.SourceYres!=ScanInfo.OutputYres))
	{
		ScanInfo.SourceW	= (int)(ScanInch_W * ScanInfo.SourceXres);
		ScanInfo.SourceH	= (int)(ScanInfo.OutputH * ((float)ScanInfo.SourceYres/(float)ScanInfo.OutputYres));
		if(lpScanParam->BitsPerPixel == 1)
		{
			ScanInfo.Alignment	= 8;
			ScanInfo.BitsPerPixel = 8;  //using gray img to BW img
			bBWmodeScan = TRUE;
			ScanInfo.Threshold = lpScanParam->Threshold;
		}
		else
			ScanInfo.Alignment	=  (bCompress)? 8 :4;
		ScanInfo.SourceW    = PixelAlignment(ScanInfo.SourceW,ScanInfo.Alignment);
		ScanInfo.SourceLeft = (int)(lpScanParam->Left * ((float)ScanInfo.SourceXres/(float)ScanInfo.OutputXres));
		ScanInfo.SourceTop	= (int)(lpScanParam->Top  * ((float)ScanInfo.SourceYres/(float)ScanInfo.OutputYres));
		bScale=TRUE;
	}
	else
	{
		if(lpScanParam->BitsPerPixel == 1)
		{
			ScanInfo.Alignment	= 8;
			ScanInfo.BitsPerPixel = 8;  //using gray img to BW img
			bBWmodeScan = TRUE;
			ScanInfo.Threshold = lpScanParam->Threshold;
		}
		else
			ScanInfo.Alignment	=  (bCompress)? 8 :4;
		ScanInfo.SourceW	= ScanInfo.OutputW;
		ScanInfo.SourceW    = PixelAlignment(ScanInfo.SourceW,ScanInfo.Alignment);
		ScanInfo.SourceH	= ScanInfo.OutputH ;
		ScanInfo.SourceLeft = lpScanParam->Left;
		ScanInfo.SourceTop	= lpScanParam->Top ;
		if(ScanInfo.SourceW != ScanInfo.OutputW)
			bScale=TRUE;
		else
		{
            bScale=FALSE;
             if(bCompress)
                  bScale=TRUE;
		}

	}
#if _CANOPUS_DEBUG_
	LTCPrintf("ScanInfo.SourceLeft : %d\n",ScanInfo.SourceLeft);
	LTCPrintf("ScanInfo.SourceTop : %d\n",ScanInfo.SourceTop);
	LTCPrintf("ScanInfo.SourceXres : %d\n",ScanInfo.SourceXres);
	LTCPrintf("ScanInfo.SourceYres : %d\n",ScanInfo.SourceYres);
	LTCPrintf("ScanInfo.OutputW : %d\n",ScanInfo.OutputW);
	LTCPrintf("ScanInfo.OutputH : %d\n",ScanInfo.OutputH);
	LTCPrintf("ScanInfo.SourceW : %d\n",ScanInfo.SourceW);
	LTCPrintf("ScanInfo.SourceH : %d\n",ScanInfo.SourceH);
	LTCPrintf("ScanInfo.BitsPerPixel : %d\n",ScanInfo.BitsPerPixel);
	if(bOverScan)
	{
		LTCPrintf("lpScanParam->PreFeed : %d\n",(OVER_SCAN_DELTA_Y*ScanInfo.SourceYres));
		LTCPrintf("lpScanParam->PostFeed : %d\n",(OVER_SCAN_DELTA_Y*ScanInfo.SourceYres));
	}
	LTCPrintf("bGamma :%d  bTonemap : %d\n",bGamma,bTonemap);

#endif
	m_GLDrv->sc_pardata.source	= (bADFOption)?I3('ADF'):I3('FLB');
	m_GLDrv->sc_pardata.duplex	= (bADFOption)?((byADFMode)?3:1):1;
	m_GLDrv->sc_pardata.page	= (bADFOption)?0:1;
	m_GLDrv->sc_pardata.format	= ScanInfo.Formate;
	m_GLDrv->sc_pardata.acquire = ACQ_SHADING|ACQ_AUTO_GO_HOME|ACQ_MIRROR|(bOverScan*ACQ_START_HOME)|((bTonemap||bGamma)*ACQ_GAMMA); 
	m_GLDrv->sc_pardata.bit		= (UINT8)ScanInfo.BitsPerPixel;
	m_GLDrv->sc_pardata.dpi.x	= (UINT16)ScanInfo.SourceXres;
	m_GLDrv->sc_pardata.dpi.y	= (UINT16)ScanInfo.SourceYres;
	if(bOverScan)
	{
		m_GLDrv->sc_pardata.org.x   = (UINT32)(OVER_SCAN_DELTA_Y*ScanInfo.SourceYres);//lpScanParam->PreFeed;
		m_GLDrv->sc_pardata.org.y	= (UINT32)(OVER_SCAN_DELTA_Y*ScanInfo.SourceYres);
	}
	else
	{
		m_GLDrv->sc_pardata.org.x   = ScanInfo.SourceLeft;
		m_GLDrv->sc_pardata.org.y	= ScanInfo.SourceTop ;
	}
	m_GLDrv->sc_pardata.dot.w	= ScanInfo.SourceW;	
	m_GLDrv->sc_pardata.dot.h	= ScanInfo.SourceH;
	if(m_GLDrv->sc_pardata.bit > 16)
		m_GLDrv->sc_pardata.mono = 0;
	else
		m_GLDrv->sc_pardata.mono = 255;

	return m_GLDrv->_parameters();
}
BYTE CLTCDrv::GetScanParameter(LPLTCSCANPARAMETER lpScanParam)
{
	lpScanParam = lpScanParam;
	return 0;
}
BYTE CLTCDrv::StartScan(VOID)
{
	BYTE result=1;
	bStartScanFlag = TRUE;
	if(bCancel == TRUE)
	{
		dwErrorCode = ERRCODE_Cancel;
		return FALSE;
	}
	else if(bADFOption&&byADFMode)
	{
		if(end_page[0]==1 && end_page[1]==0)
		{
#if DEBUG_SCAN_FLOW
			LTCPrintf("StartScan Status 1\n");
#endif 
			return TRUE;
		}
		else if(!byEndDoc)
		{
#if DEBUG_SCAN_FLOW
			LTCPrintf("StartScan Status 2\n");
#endif
			return TRUE;
		}
		else
		{
#if DEBUG_SCAN_FLOW
			LTCPrintf("StartScan Status 3\n");
#endif
			if(bADFOption)
			{
				result = m_GLDrv->_StatusCheck_ADF_Check();
				if(!result)
				{
#if DEBUG_SCAN_FLOW
				LTCPrintf("StartScan Status 3.1\n");
#endif
					result = ErrorMapping_ADF();
					result = m_GLDrv->_info();
					if(!result)
					{
						if(m_GLDrv->sc_infodata.Cancel)
						{
							bCancel = TRUE;
							dwErrorCode = ERRCODE_Cancel;
						}
						else if(m_GLDrv->sc_infodata.Error)
						{
							result = m_GLDrv->_StatusGet();
							if(!result) {
								dwErrorCode = ERRCODE_ScannerDisconnect;
								return FALSE;
							}
							result = ErrorMapping_ADF();
						}
						else
						{
							dwErrorCode = ERRCODE_ScannerDisconnect;
							return FALSE;
						}
						
						return FALSE;
					}
				}
			}
			result = m_GLDrv->_StatusGet();
			if(!result) {
				dwErrorCode = ERRCODE_ScannerDisconnect;
				return FALSE;
			}
			result = m_GLDrv->_StatusCheck_Scanning();
			if(!result) {
				ErrorMapping_START();
				return FALSE;
			}
			result = m_GLDrv->_StartScan();
			if (!result) {

				result = m_GLDrv->_StatusGet();
				if (!result) {
					dwErrorCode = ERRCODE_ScannerDisconnect;
					return FALSE;
				}
				result = m_GLDrv->_StatusCheck_Scanning();
				if (!result) {
					ErrorMapping_START();
					return FALSE;
				}
			}
			else
				return TRUE;
		}

	}
	else if(bADFOption&&(!byEndDoc))
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StartScan Status 4\n");
#endif
		return TRUE;
	}
	else
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StartScan Status 5\n");
#endif
		result = m_GLDrv->_StatusGet();
		if(!result) {
			dwErrorCode = ERRCODE_ScannerDisconnect;
			return FALSE;
		}
		result = m_GLDrv->_StatusCheck_Scanning();
		if(!result) {
			ErrorMapping_START();
			return FALSE;
		}
		result = m_GLDrv->_StartScan();
		if (!result) {

			result = m_GLDrv->_StatusGet();
			if (!result) {
				dwErrorCode = ERRCODE_ScannerDisconnect;
				return FALSE;
			}
			result = m_GLDrv->_StatusCheck_Scanning();
			if (!result) {
				ErrorMapping_START();
				return FALSE;
			}
		}
		else
			return TRUE;
	}
	return TRUE;
}
BYTE CLTCDrv::ReadScan(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount)
{
	BOOL flag;
	return ReadScanEX(pStatus, pBuffer, Count, RealCount, &flag);
}
BYTE CLTCDrv::StopScan(VOID)
{
	BYTE ret=1,time_out_count=0;
	if(bCancel)
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Cancel +\n");
#endif
		dwErrorCode = ERRCODE_Cancel;
		if(!bADFOption)
		{
			ReleaseImg_ResetVar(); 
			bCancel = FALSE;
		}
		if(bStartScanFlag)
		{
			ret = m_GLDrv->_cancel();
			bStartScanFlag = FALSE;
		}
		if(bJobCreatFlag)
		{
			bJobCreatFlag = FALSE;
			m_GLDrv->_JobEnd();
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Cancel -\n");
#endif
		return FALSE;
	}
	else if(dwErrorCode!=0 && dwErrorCode!=14)
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan error stop +\n dwErrorCode = %d\n",dwErrorCode);
#endif
		if(!bADFOption)
		{
			ReleaseImg_ResetVar(); 
			bCancel = FALSE;
		}
		if(bStartScanFlag)
		{
			ret = m_GLDrv->_stop();
			bStartScanFlag = FALSE;
		}
		if(bJobCreatFlag)
		{
			bJobCreatFlag = FALSE;
			m_GLDrv->_JobEnd();
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan error stop -\n");
#endif
		return FALSE;
	}
	else if(byADFMode && end_page[0]==1) // duplex first page stop
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 1\n");
#endif
		return TRUE;
	}
	else if(bADFOption)
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 2\n");
#endif
		while (ret && !byEndDoc)
		{
			time_out_count++;
			if(time_out_count==52)
			{
				bStopTimeOutFlag = TRUE;
				byEndDoc = 1;
				break;
			}
			ret = m_GLDrv->_info();
			if(!ret)
			{
				ret = m_GLDrv->_StatusGet();
				if(!ret) {
					dwErrorCode = ERRCODE_ScannerDisconnect;
				}
				ret = m_GLDrv->_StatusCheck_Scanning();
				if(!ret) {
					ErrorMapping_START();
				}
			}
			if((m_GLDrv->sc_infodata.ValidPageSize[0] > 0) || (m_GLDrv->sc_infodata.ValidPageSize[1] > 0) || (ImgBEnd)) {
				 break;
			}
			else
			{
				if(!(m_GLDrv->_CheckScanningMode()))
				{
					dwErrorCode=ERRCODE_HatchOpen;
				}
			}
			byEndDoc = m_GLDrv->sc_infodata.EndDocument;
			Sleep(200);//為解決在剛開始scan還沒有影像出來時，因SW發info polling FW太頻繁使 key press handle thread 一直沒有優先權執行問題。
			if(m_GLDrv->sc_infodata.Cancel == 1 || bCancel)
			{
				dwErrorCode = ERRCODE_Cancel;
				bCancel = TRUE;
				break;
			}
			if(dwErrorCode!=0 && dwErrorCode!=14)
			{
		#if DEBUG_SCAN_FLOW
				LTCPrintf("StopScan error stop1 +\ndwErrorCode = %d\n",dwErrorCode);
		#endif
				if(!bADFOption)
				{
					ReleaseImg_ResetVar(); 
					bCancel = FALSE;
				}
				if(bStartScanFlag)
				{
					ret = m_GLDrv->_stop();
					bStartScanFlag = FALSE;
				}
				if(bJobCreatFlag)
				{
					bJobCreatFlag = FALSE;
					m_GLDrv->_JobEnd();
				}
		#if DEBUG_SCAN_FLOW
				LTCPrintf("StopScan error stop1 -\n");
		#endif
				return FALSE;
			}
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 2-\n");
#endif
	}
	if(bCancel)
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Cancel +\n");
#endif
		dwErrorCode = ERRCODE_Cancel;
		if(!bADFOption)
		{
		ReleaseImg_ResetVar(); 
		bCancel = FALSE;
		}
		if(bStartScanFlag)
		{
			ret = m_GLDrv->_cancel();
			bStartScanFlag = FALSE;
		}
		if(bJobCreatFlag)
		{
			bJobCreatFlag = FALSE;
			m_GLDrv->_JobEnd();
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Cancel -\n");
#endif
		return FALSE;
	}
	if(bReadImg == 0 && bADFOption && (!byEndDoc))//normal stop
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 1  byEndDoc %d\n",byEndDoc);
#endif
		return TRUE;
	}
	else if(byADFMode && end_page[0]==1) // duplex first page stop
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 4\n");
#endif
		return TRUE;
	}
	else		
	{
		LTCPrintf(" byEndDoc %d\n",byEndDoc);
		ret = m_GLDrv->_StatusGet();
		if(!ret) {
			dwErrorCode = ERRCODE_ScannerDisconnect;
			return FALSE;
		}
		ret = m_GLDrv->_StatusCheck_Scanning();
		if(!ret) {
			ErrorMapping_START();
			return FALSE;
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 5\n");
#endif
		if(!bADFOption)
			ReleaseImg_ResetVar(); // release image buffer
	
		BYTE ret=0;
		if(bStartScanFlag)
		{
			ret = m_GLDrv->_stop();
			bStartScanFlag = FALSE;
		}
		if(bJobCreatFlag)
		{
			bJobCreatFlag = FALSE;
			m_GLDrv->_JobEnd();
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("StopScan Status 5-\n");
#endif
		return ret;
	}
//	return 1;
}
BYTE CLTCDrv::CancelScan(VOID)
{
	return 0;
}
BYTE CLTCDrv::ReadPushButton(LPREADBUTTONINDEX pIndex)
{
	pIndex = pIndex;
	return 0;
}
BYTE CLTCDrv::SetSecondGamma(LPBYTE GammaTable, BYTE Color, DWORD Size)
{
	GammaTable = GammaTable;
	Color = Color;
	Size = Size;
	return 0;
}
BYTE CLTCDrv::ReadSN(BYTE *pSN, WORD len)
{
	pSN = pSN;
	len = len;
	return 0;
}
BYTE CLTCDrv::ReadPageCount(DWORD *page)
{
	page = page;
	return 0;
}
BYTE CLTCDrv::WritePageCount(DWORD *page)
{
	page = page;
	return 0;
}
BYTE CLTCDrv::ReadShippingDate(WORD *year, WORD *month, WORD *day)
{
	year = year;
	month = month;
	day= day;
	return 0;
}
BYTE CLTCDrv::WriteShippingDate(WORD year, WORD month, WORD day)
{
	year = year;
	month = month;
	day= day;
	return 0;
}
BYTE CLTCDrv::SetScannerSleepTime(int minutes)
{

	unsigned int Sleep,AutoOff,dissleep,disauto;
	m_GLDrv->_GetTime(&Sleep,&AutoOff,&dissleep,&disauto);
	if(disauto)
	{
		return m_GLDrv->_SetTime(minutes,0);
	}
	else
	{
		return m_GLDrv->_SetTime(minutes,AutoOff);
	}
	
}
BYTE CLTCDrv::GetScannerSleepTime(int *minutes)
{
	BOOL ret;
	unsigned int Sleep,AutoOff,dissleep,disauto;
	ret = m_GLDrv->_GetTime(&Sleep,&AutoOff,&dissleep,&disauto);
	*minutes = Sleep;
	return (BYTE)ret;
}
BYTE CLTCDrv::WriteSN(BYTE *pSN, WORD len)
{
	pSN = pSN;
	len = len;
	return 0;
}
BYTE CLTCDrv::SetVendorProductString (BYTE *VendorStr, BYTE *ProductStr)
{
	VendorStr = VendorStr;
	ProductStr = ProductStr;
	return 0;
}
BYTE CLTCDrv::GetLLDVersion(BYTE *Version, BYTE len)
{
	Version = Version;
	len = len;
	return 0;
}
BYTE CLTCDrv::EnableButtonEvents(BYTE bEnable)
{
	bEnable = bEnable;
	return 0;
}
BYTE CLTCDrv::TerminateScanner(VOID)
{
	return 1;//m_GLDrv->_JobEnd();
}
BYTE CLTCDrv::GetFWVersion(char *Version, BYTE len)
{
	len = len;
	return m_GLDrv->_Get_fw_version(Version);
}
BYTE CLTCDrv::SetTempFileFolder(WCHAR *Folder)
{
	Folder = Folder;
	return 0;
}
BYTE CLTCDrv::ReadUSBSN(BYTE *pSN, WORD len)
{
	pSN = pSN;
	len = len;
	return 0;
}
BYTE CLTCDrv::WriteUSBSN(BYTE *pSN, WORD len)
{
	pSN = pSN;
	len = len;
	return 0;
}
BYTE CLTCDrv::GetVendorProductString (BYTE *VendorStr, BYTE *ProductStr)
{
	VendorStr = VendorStr;
	ProductStr = ProductStr;
	return 0;
}
BYTE CLTCDrv::GetVidPid(WORD *Vid, WORD *Pid)
{
	Vid = Vid;
	Pid = Pid;
	return 0;
}
BYTE CLTCDrv::SetVidPid(WORD Vid, WORD Pid)
{
	Vid = Vid;
	Pid = Pid;
	return 0;
}
BYTE CLTCDrv::DoCalibration(BYTE Type, BYTE Mode, WORD Resolution)
{
	Type = Type;
	Mode = Mode;
	Resolution = Resolution;
	return 0;
}
BYTE CLTCDrv::TestMotor(BYTE Motor, BYTE Dir, DWORD Step, DWORD PPS, BYTE *pStatus)
{
	Motor = Motor;
	Dir = Dir;
	Step = Step;
	PPS = PPS;
	pStatus = pStatus;
	return 0;
}
BYTE CLTCDrv::DownloadFW(BYTE *pData, WORD size)
{
	pData = pData;
	size = size;
	return 0;
}
BYTE CLTCDrv::GoHome(DWORD Option, DWORD *pStatus)
{
	Option = Option;
	pStatus = pStatus;
	return 0;
}
BYTE CLTCDrv::SetAutoOffTime(int minutes)
{
	unsigned int Sleep,AutoOff,dissleep,disauto;
	m_GLDrv->_GetTime(&Sleep,&AutoOff,&dissleep,&disauto);

	return m_GLDrv->_SetTime(Sleep,minutes);
}
BYTE CLTCDrv::GetAutoOffTime(int *minutes)
{
	BOOL ret;
	unsigned int Sleep,AutoOff,dissleep,disauto;
	ret = m_GLDrv->_GetTime(&Sleep,&AutoOff,&dissleep,&disauto);
	if(disauto)
		*minutes = 0;
	else
	{
		*minutes = AutoOff;
	}
	return (BYTE)ret;
}
BYTE CLTCDrv::Test(WORD func1, WORD func2, WORD func3, WORD *pStatus)
{
	func1 = func2 =func3 = *pStatus;
	return 0;
}
BYTE CLTCDrv::DisableAutoOffTimer(BYTE Disable)
{
	if(Disable)
	{
		unsigned int Sleep,AutoOff,dissleep,disauto;
		m_GLDrv->_GetTime(&Sleep,&AutoOff,&dissleep,&disauto);
		return m_GLDrv->_SetTime(Sleep,0);
	}
	else 
		return 0;
}
BYTE CLTCDrv::ReadLastCalibration(WORD *pYear, WORD *pMonth, WORD *pDay, DWORD *pPage)
{
	pYear=pYear;
	pMonth=pMonth;
	pDay=pDay;
	pPage=pPage;
	return 0;
}
BYTE CLTCDrv::WriteLastCalibration(WORD Year, WORD Month, WORD Day, DWORD Page)
{
	Year=Year;
	Month=Month;
	Day=Day;
	Page=Page;
	return 0;
}
BYTE CLTCDrv::GetScalingValue(float *pMagCorrectionX)
{
	pMagCorrectionX = pMagCorrectionX;
	return 0;
}
//=============new function
BYTE CLTCDrv::SetIOHandle(HANDLE dwDevice, WORD wType)
{
	return m_GLDrv->_SetIOHandle(dwDevice,wType);
}
BYTE CLTCDrv::GetScannerAbility(LPHWCAPABILITY pHWCaps)
{


	//strcpy(pHWCaps->ProductName, "2500"); 
	pHWCaps->ScannerType = 0;	// Clear this member

	pHWCaps->ScannerType |= SCANNERTYPE_ADFSCAN;
	pHWCaps->ScannerType |= SCANNERTYPE_FLATBED;

	pHWCaps->ModeCaps		= MODECAP_COLOR|MODECAP_GRAYSCALE|MODECAP_LINEART;
	pHWCaps->OpticalResol	= 1200;
//	pHWCaps->ResolNum		= RESOLNUM;
//	pHWCaps->lpResolItem	= (WORD *)&ResolItem[0];
	pHWCaps->MaxWidth		= 85 * 30;	//modified to be in 300dpi addressing
	pHWCaps->MinWidth		= 10;
	pHWCaps->MaxHeight		= 117 * 30; //modified to be in 300dpi addressing
	pHWCaps->MinHeight		= 20;
	pHWCaps->HWFuncCaps		= 0;
	m_GLDrv->_Get_fw_version(pHWCaps->VersionName);


	return (TRUE);
}
BYTE CLTCDrv::ReadScanEX(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count, DWORD *RealCount, BOOL *EndFlag)
{
	int result=1;
	DWORD ReadCount=0;
	BOOL ReadEnd = FALSE;
	BOOL DocEnd = FALSE;
	int  raw_line_byte=0;
	unsigned long raw_in_line_max=1000; 
	int raw_in_line=0; 
	unsigned long raw_out_line=0;
	unsigned long jpeg_out_size=0;
	*RealCount = 0;
	if((!bScale) && (!bBWmodeScan)) //native PPI scan
	{
#if JPEG_MODIFY_HEADER
		if(!bADFOption || !bCompress)//fb and raw
		{
			if(byADFMode)
			{
				result = ReadScanEX_raw_dup(pStatus,pBuffer,Count,RealCount,EndFlag);
			}
			else 
			{
				result = ReadScanEX_raw_sim(pStatus,pBuffer,Count,RealCount,EndFlag);
			}
		}
		else
		{
			if(ImageIn->pimg==NULL)
			{
				ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8));
#if _CANOPUS_DEBUG_
				if(ImageIn->pimg==NULL)
					LTCPrintf("ImageIn malloc fail\n");
#endif
				if (ImageIn->pimg == NULL)
				{
					dwErrorCode = ERRCODE_MemoryNotEnough;
					return FALSE;
				}
			}
			while(!ImageIn->bEnd)
			{
				result = ReadSourceImage(ImageIn,Count,&ReadCount,byADFMode,&ReadEnd,&DocEnd);
				if(!result)
				{
					return FALSE;
				}
				if(ImageIn->bEnd)
				{
					ImageIn->Height= page_line[0] ;
					if(!ImgBEnd)
						page_line[0] = page_line[1] = 0;
					ImageIn->Width = ScanInfo.OutputW;
					ImageIn->bits = ScanInfo.BitsPerPixel;
					if((ImageIn->Height < (ScanInfo.OutputH+8))&& (ImageIn->Height>8))
						ImageIn->Height-=7;
					else if(!bOverScan)
					{
						if(ImageIn->Height >= (ScanInfo.OutputH+8))
							ImageIn->Height = ScanInfo.OutputH;
					}
					ImageIn->JpegResize();
					
				}
			}
			if(ImageIn->ImgSize-ImageIn->ImgShfit >= Count)
			{
				memcpy(pBuffer,ImageIn->pimg+ImageIn->ImgShfit,Count);
				ImageIn->ImgShfit += Count;
				*RealCount = Count;
				if((ImageIn->bEnd)&&(ImageIn->ImgShfit == (int)ImageIn->ImgSize))
				{
					goto JPEG_END1;
				}
				else
				{
					*EndFlag = 0;
					byEndDoc = (BYTE)DocEnd;
				}
			}
			else 
			{
				memcpy(pBuffer,ImageIn->pimg+ImageIn->ImgShfit,ImageIn->ImgSize-ImageIn->ImgShfit);
				*RealCount = ImageIn->ImgSize-ImageIn->ImgShfit;
				ImageIn->ImgShfit += ImageIn->ImgSize-ImageIn->ImgShfit;
JPEG_END1:
				byEndDoc = (BYTE)DocEnd;
				*EndFlag = 1;
				//reset varible
				ImageIn->Reset();
				bFirstReadScanEX=TRUE;
				bflag_decode_done = FALSE;
			}
			return (BYTE)result;
	
		}


#else
		if(!byADFMode)//simplex + fb
			result = ReadScanEX_raw_sim(pStatus,pBuffer,Count,RealCount,EndFlag);
		else //duplex
			result = ReadScanEX_raw_dup(pStatus,pBuffer,Count,RealCount,EndFlag);
#endif
	}
#if 0
	else if(bScale || bCompress || bBWmodeScan)
	{
		if(!bGetSourceImg)
		{
			result = ReadSourceImage();
			if(!result)
			{
				return (BYTE)result;
			}
			if(bScale)
			{
				if(bADFOption)
					ScanInfo.OutputH =(int)(ImageA->Height * ((float)ScanInfo.OutputYres/(float)ScanInfo.SourceYres));
				InputImgTransfer_Scale(ImageA,Image_tmp,ScanInfo);
				OutputImgTransfer(ImageA,Image_tmp,ScanInfo);
				if(byADFMode)//duplex
				{
					InputImgTransfer_Scale(ImageB,Image_tmp,ScanInfo);
					OutputImgTransfer(ImageB,Image_tmp,ScanInfo);
				}
			}
			if(bBWmodeScan)
			{
				GrayImgToBWImg(ImageA);
				if(byADFMode)//duplex
				{
					GrayImgToBWImg(ImageB);
				}
			}
		}
		result = SendOutputImage(pStatus,pBuffer,Count,RealCount,EndFlag);
	}
#else
	else if(bScale||bBWmodeScan)
	{
#if 0
		if(ImageIn->pimg==NULL)
		{
			if(bCompress)
			{
				if(ScanInfo.BitsPerPixel>=24)//color
				{
/*					ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8)/7);
					if(ImageIn->pimg==NULL)
						LTCPrintf("ImageIn malloc fail\n");*/
					ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*ScanInfo.OutputW*ScanInfo.OutputH*(ScanInfo.BitsPerPixel/8)/10);
					if(ImageOut->pimg==NULL)
						LTCPrintf("ImageOut malloc fail\n");
				}
				else
				{
					/*ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8)/2);
					if(ImageIn->pimg==NULL)
						LTCPrintf("ImageIn malloc fail\n");*/
					ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*ScanInfo.OutputW*ScanInfo.OutputH*(ScanInfo.BitsPerPixel/8)/2);
					if(ImageOut->pimg==NULL)
						LTCPrintf("ImageOut malloc fail\n");
				}
			}
			else
			{
			/*	ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*(ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8)));
				if(ImageIn->pimg==NULL)
					LTCPrintf("ImageIn malloc fail\n");*/
				ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*(ScanInfo.OutputW*ScanInfo.OutputH*(ScanInfo.BitsPerPixel/8)));
				if(ImageOut->pimg==NULL)
					LTCPrintf("ImageOut malloc fail\n");
				raw_out_buf = (unsigned char*)malloc(raw_in_line_max*(ScanInfo.BitsPerPixel/8)*ScanInfo.SourceW);
				if(raw_out_buf==NULL)
					LTCPrintf("raw_out_buf malloc fail\n");
			}
		}

READSOURCE:
#if JPEG_MODIFY_HEADER
	if(bADFOption)
	{
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count && bflag_decode_done)
		{
			goto SENDIMAGE;
		}
	}
	else
	{
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count)
		{
			goto SENDIMAGE;
		}
	}

#else
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count)
		{
			goto SENDIMAGE;
		}
#endif
		if(ImageIn->bEnd != TRUE)
		{   
			if(ImageIn->pimg==NULL)
			ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*Count*2);
			if(ImageIn->pimg==NULL)
				LTCPrintf("ImageIn malloc fail\n");
			LTCPrintf("ReadSourceImage +\n");
			if(bCompress)
				result = ReadSourceImage(ImageIn,Count,&ReadCount,byADFMode,&ReadEnd,&DocEnd);
			else
			{
				raw_line_byte = ScanInfo.SourceW * (ScanInfo.BitsPerPixel/8) ;
				result = ReadSourceImage(ImageIn,(((int)(Count/raw_line_byte))?((int)(Count/raw_line_byte)):1)*raw_line_byte,&ReadCount,byADFMode,&ReadEnd,&DocEnd);
			}
			if(!result)
			{
				return (BYTE)result;
			}
			LTCPrintf("ReadSourceImage -\n");
		}
		if(bCompress)
		{
			if(bFirstReadScanEX)
			{   
				bFirstReadScanEX=FALSE;
				jpeg_resize_init(&jrcb);
			}
			if(!bflag_decode_done)
			{
				bflag_decode_done = jpeg_resize_read(&jrcb, &jpeg_out_buf, &jpeg_out_size, ImageIn->pimg,ReadCount,ScanInfo.SourceXres , ScanInfo.OutputXres,0);
				if(ImageIn->pimg!=NULL)
				{
					free(ImageIn->pimg);
					ImageIn->pimg=NULL;
					ImageIn->ImgSize=0;
				}
				memcpy(ImageOut->pimg+ImageOut->ImgSize, jpeg_out_buf, jpeg_out_size);
				ImageOut->ImgSize += jpeg_out_size;
			}
#if JPEG_MODIFY_HEADER
			if(bADFOption)
			{
				if(!bflag_decode_done)
					goto READSOURCE;
				else
				{
					ImageOut->Height= (int)(page_line[0] *((float)ScanInfo.OutputYres/(float)ScanInfo.SourceYres)) ;
					ImageOut->bits = ScanInfo.BitsPerPixel;
					ImageOut->JpegResize2();
				}

			}
#endif
		}
		else //raw
		{
			if(bFirstReadScanEX)
			{   
				bFirstReadScanEX=FALSE;
				resize_init(&rscb, ScanInfo.OutputW, ScanInfo.OutputH, ScanInfo.SourceW, ScanInfo.SourceH, ScanInfo.BitsPerPixel);
			}
			raw_line_byte = ScanInfo.SourceW * (ScanInfo.BitsPerPixel/8) ;
			if(raw_line_byte!=0)
			{
				raw_in_line = (int)(ImageIn->ImgSize /raw_line_byte );
				if(raw_in_line > (int)raw_in_line_max)
					raw_in_line = raw_in_line_max;
			}
			
			if(ImageOut->bEnd != TRUE)
			{
				resize_read(&rscb, raw_out_buf, &raw_out_line, (ImageIn->pimg), raw_in_line);
				
				ImageIn->Height+=raw_in_line;
				ImageOut->Height+=raw_out_line;
				if(bBWmodeScan)
				{
					gray2bw(ScanInfo.OutputW,raw_out_line,raw_out_buf,ImageOut->pimg+ImageOut->ImgSize);
					ImageOut->ImgSize += raw_out_line*ScanInfo.OutputW/8;
				}
				else
				{
					memcpy(ImageOut->pimg+ImageOut->ImgSize,raw_out_buf, raw_out_line*(ScanInfo.BitsPerPixel/8)*ScanInfo.OutputW);
					ImageOut->ImgSize += raw_out_line*(ScanInfo.BitsPerPixel/8)*ScanInfo.OutputW;
				}
				if(ImageIn->pimg!=NULL)
				{
					free(ImageIn->pimg);
					ImageIn->pimg=NULL;
					ImageIn->ImgSize=0;
				}
				ImageOut->bEnd = ImageIn->bEnd;
				
			}
		}
SENDIMAGE:
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count)
		{
			memcpy(pBuffer,ImageOut->pimg+ImageOut->ImgShfit,Count);
			ImageOut->ImgShfit += Count;
			*RealCount = Count;
			if((ImageIn->bEnd)&&(ImageOut->ImgShfit == (int)ImageOut->ImgSize))
			{
				if(bCompress)
				{
					goto JPEG_END;
				}
				else 
				{
					goto RAW_END;
				}
			}
			else
			{
				*EndFlag = 0;
				byEndDoc = (BYTE)DocEnd;
			}
		}
		else if(bflag_decode_done) //jpeg
		{
			memcpy(pBuffer,ImageOut->pimg+ImageOut->ImgShfit,ImageOut->ImgSize-ImageOut->ImgShfit);
			*RealCount = ImageOut->ImgSize-ImageOut->ImgShfit;
			ImageOut->ImgShfit += ImageOut->ImgSize-ImageOut->ImgShfit;
JPEG_END:
			byEndDoc = (BYTE)DocEnd;
			*EndFlag = 1;
			jpeg_resize_free(&jrcb);//to free tmp jpeg output files buffer. ~40M.
			//reset varible
			ImageIn->Reset();
			ImageOut->Reset();
			bFirstReadScanEX=TRUE;
			bflag_decode_done = FALSE;
		}
		else if(ImageOut->bEnd) //raw
		{
			memcpy(pBuffer,ImageOut->pimg+ImageOut->ImgShfit,ImageOut->ImgSize-ImageOut->ImgShfit);
			*RealCount = ImageOut->ImgSize-ImageOut->ImgShfit;
			ImageOut->ImgShfit = ImageOut->ImgSize;
RAW_END:
			byEndDoc = (BYTE)DocEnd;
			*EndFlag = 1;
			ImageIn->Reset();
			ImageOut->Reset();
			if(raw_out_buf!=NULL)
			{
				free(raw_out_buf);
				raw_out_buf = NULL;
			}
			bFirstReadScanEX=TRUE;
		}
		else
		{
			goto READSOURCE;
		}
#else
		if(ImageIn->pimg==NULL)
		{
			if(bCompress)
			{
				if(ScanInfo.BitsPerPixel>=24)//color
				{
					ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8));
#if _CANOPUS_DEBUG_
					if(ImageIn->pimg==NULL)
						LTCPrintf("ImageIn malloc fail\n");
#endif
					if (ImageIn->pimg == NULL)
					{
						dwErrorCode = ERRCODE_MemoryNotEnough;
						return FALSE;
					}
					ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*ScanInfo.OutputW*ScanInfo.OutputH*(ScanInfo.BitsPerPixel/8));
#if _CANOPUS_DEBUG_
					if(ImageOut->pimg==NULL)
						LTCPrintf("ImageOut malloc fail\n");
#endif
					if (ImageOut->pimg == NULL)
					{
						dwErrorCode = ERRCODE_MemoryNotEnough;
						return FALSE;
					}
				}
				else
				{
					if(((ScanInfo.OutputW/ScanInfo.OutputXres)>=2)||((ScanInfo.OutputH/ScanInfo.OutputYres)>=2))
					{
						ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8)*3);
#if _CANOPUS_DEBUG_
						if(ImageIn->pimg==NULL)
							LTCPrintf("ImageIn malloc fail\n");
#endif
						if (ImageIn->pimg == NULL)
						{
							dwErrorCode = ERRCODE_MemoryNotEnough;
							return FALSE;
						}
						ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*ScanInfo.OutputW*ScanInfo.OutputH*(ScanInfo.BitsPerPixel/8)*3);
#if _CANOPUS_DEBUG_
						if(ImageOut->pimg==NULL)
							LTCPrintf("ImageOut malloc fail\n");
#endif
						if (ImageOut->pimg == NULL)
						{
							dwErrorCode = ERRCODE_MemoryNotEnough;
							return FALSE;
						}
					}
					else
					{
						ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*(2*ScanInfo.SourceXres)*(2*ScanInfo.SourceYres)*(ScanInfo.BitsPerPixel/8)*3);
						ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*(2*ScanInfo.OutputXres)*(2*ScanInfo.OutputYres)*(ScanInfo.BitsPerPixel/8)*3);
						if ((ImageOut->pimg == NULL) || (ImageIn->pimg == NULL))
						{
							dwErrorCode = ERRCODE_MemoryNotEnough;
							return FALSE;
						}
					}
				}
			}
			else
			{
				ImageIn->pimg = (BYTE*)malloc(sizeof(BYTE)*(ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8)));
#if _CANOPUS_DEBUG_
				if(ImageIn->pimg==NULL)
					LTCPrintf("ImageIn malloc fail\n");
#endif
				ImageOut->pimg =  (BYTE*)malloc(sizeof(BYTE)*(ScanInfo.OutputW*ScanInfo.OutputH*(ScanInfo.BitsPerPixel/8)));
#if _CANOPUS_DEBUG_
				if(ImageOut->pimg==NULL)
					LTCPrintf("ImageOut malloc fail\n");
#endif
				raw_out_buf = (unsigned char*)malloc(raw_in_line_max*(ScanInfo.BitsPerPixel/8)*ScanInfo.SourceW);
#if _CANOPUS_DEBUG_
				if(raw_out_buf==NULL)
					LTCPrintf("raw_out_buf malloc fail\n");
#endif
				if ((ImageOut->pimg == NULL) || (ImageIn->pimg == NULL) || (raw_out_buf == NULL))
				{
					dwErrorCode = ERRCODE_MemoryNotEnough;
					return FALSE;
				}
			}
		}
#if 0
 FILE *fout;
	unsigned long tot_jpeg_in_size=0;
	unsigned long tot_jpeg_out_size=0;
	unsigned char *jpeg_in_buf;
	unsigned char  *outFileBuf;
	unsigned long filesize, outfilesize=0;
	outFileBuf =(unsigned char*)malloc(10*1024*1024);
		result = ReadSourceImage();
		jpeg_in_buf = ImageA->pimg;
	while(flag_decode_done==0) {
		if(tot_jpeg_in_size+jpeg_in_size > ImageA->ImgSize) 
			jpeg_in_size = ImageA->ImgSize - tot_jpeg_in_size;
		
		flag_decode_done = jpeg_resize_read(&jrcb, &jpeg_out_buf, &jpeg_out_size, jpeg_in_buf, jpeg_in_size, 600, 500, 0);
		tot_jpeg_in_size += jpeg_in_size;
		tot_jpeg_out_size += jpeg_out_size;

		jpeg_in_buf += jpeg_in_size;
		LTCPrintf("%d %d %d %d\n",outFileBuf, tot_jpeg_out_size, jpeg_out_size, jpeg_out_buf);
		memcpy(outFileBuf+tot_jpeg_out_size-jpeg_out_size, jpeg_out_buf, jpeg_out_size);
	

		if(flag_decode_done) {
			break;
		}
	}
	jpeg_resize_free(&jrcb);//to free tmp jpeg output files buffer. ~10M.


	outfilesize = tot_jpeg_out_size;
	
	LTCPrintf("out %d size %d\n", outFileBuf, outfilesize);

	fout = fopen("outg.jpg", "wb");
	fwrite(outFileBuf, 1, outfilesize, fout);
	free(outFileBuf);
	fclose(fout);
	LTCPrintf("END~~~~\n");
#endif
READSOURCE:
#if JPEG_MODIFY_HEADER
	if(bADFOption)
	{
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count && bflag_decode_done)
		{
			goto SENDIMAGE;
		}
	}
	else
	{
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count)
		{
			goto SENDIMAGE;
		}
	}

#else
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count)
		{
			goto SENDIMAGE;
		}
#endif
		if(ImageIn->bEnd != TRUE)
		{   
			//LTCPrintf("ReadSourceImage +\n");
			if(bCompress)
				result = ReadSourceImage(ImageIn,Count,&ReadCount,byADFMode,&ReadEnd,&DocEnd);
			else
			{
				raw_line_byte = ScanInfo.SourceW * (ScanInfo.BitsPerPixel/8) ;
				result = ReadSourceImage(ImageIn,(((int)(Count/raw_line_byte))?((int)(Count/raw_line_byte)):1)*raw_line_byte,&ReadCount,byADFMode,&ReadEnd,&DocEnd);
			}
			if(!result)
			{
				return (BYTE)result;
			}
			//LTCPrintf("ReadSourceImage -\n");
		}
		if(bCompress)
		{
			if(bFirstReadScanEX)
			{   
				bFirstReadScanEX=FALSE;
				jpeg_resize_init(&jrcb);
			}
			if(!bflag_decode_done)
			{
				bflag_decode_done = jpeg_resize_read(&jrcb, &jpeg_out_buf, &jpeg_out_size, (ImageIn->pimg+ImageIn->ImgShfit),ReadCount,ScanInfo.SourceXres , ScanInfo.OutputXres,ScanInfo.OutputW, page_line[0]);
				ImageIn->ImgShfit += ReadCount;
				memcpy(ImageOut->pimg+ImageOut->ImgSize, jpeg_out_buf, jpeg_out_size);
				ImageOut->ImgSize += jpeg_out_size;
			}
#if JPEG_MODIFY_HEADER
			if(bADFOption)
			{
				if(!bflag_decode_done)
					goto READSOURCE;
				else
				{
					ImageOut->Height= (int)(page_line[0] *((float)ScanInfo.OutputYres/(float)ScanInfo.SourceYres));
					if(!ImgBEnd)
						page_line[0] = page_line[1] = 0;
					ImageOut->Width = ScanInfo.OutputW;
					ImageOut->bits = ScanInfo.BitsPerPixel;
					if((ImageOut->Height < (ScanInfo.OutputH+8))&& (ImageOut->Height>8))
						ImageOut->Height-=7;
					else if(!bOverScan)
					{
						if(ImageOut->Height >= (ScanInfo.OutputH+8))
							ImageOut->Height = ScanInfo.OutputH;
					}
					ImageOut->JpegResize2();
				}

			}
#endif
		}
		else //raw
		{
			if(bFirstReadScanEX)
			{   
				bFirstReadScanEX=FALSE;
				resize_init(&rscb, ScanInfo.OutputW, ScanInfo.OutputH, ScanInfo.SourceW, ScanInfo.SourceH, ScanInfo.BitsPerPixel);
			}
			raw_line_byte = ScanInfo.SourceW * (ScanInfo.BitsPerPixel/8) ;
			if(raw_line_byte!=0)
			{
				raw_in_line = (int)((ImageIn->ImgSize-ImageIn->ImgShfit) /raw_line_byte );
				if(raw_in_line > (int)raw_in_line_max)
					raw_in_line = raw_in_line_max;
			}
			
			if(ImageOut->bEnd != TRUE)
			{
				resize_read(&rscb, raw_out_buf, &raw_out_line, (ImageIn->pimg+ImageIn->ImgShfit), raw_in_line);
				ImageIn->Height+=raw_in_line;
				ImageOut->Height+=raw_out_line;
				ImageIn->ImgShfit += raw_in_line * raw_line_byte;
				if(bBWmodeScan)
				{
					gray2bw(ScanInfo.OutputW,raw_out_line,raw_out_buf,ImageOut->pimg+ImageOut->ImgSize,ScanInfo.Threshold);
					ImageOut->ImgSize += raw_out_line*(ScanInfo.OutputW/8);
					if(ScanInfo.OutputW%8)
						ImageOut->ImgSize += raw_out_line;
				}
				else
				{
					memcpy(ImageOut->pimg+ImageOut->ImgSize,raw_out_buf, raw_out_line*(ScanInfo.BitsPerPixel/8)*ScanInfo.OutputW);
					ImageOut->ImgSize += raw_out_line*(ScanInfo.BitsPerPixel/8)*ScanInfo.OutputW;
				}
				if(ImageIn->ImgShfit == (int)ImageIn->ImgSize)
				{
					ImageOut->bEnd = ImageIn->bEnd;
				}
				if ((ImageOut->Height == ScanInfo.OutputH) && (!ImageIn->bEnd))
				{
					while (!ImageIn->bEnd)
					{
						raw_line_byte = ScanInfo.SourceW * (ScanInfo.BitsPerPixel / 8);
						result = ReadSourceImage(ImageIn, (((int)(Count / raw_line_byte)) ? ((int)(Count / raw_line_byte)) : 1)*raw_line_byte, &ReadCount, byADFMode, &ReadEnd, &DocEnd);
						if (!result)
						{
							return (BYTE)result;
						}
					}
				}
			}
		}
SENDIMAGE:
		if(ImageOut->ImgSize-ImageOut->ImgShfit >= Count)
		{
			if (!bADFOption && (ImageOut->ImgShfit < (int)Count) && bCompress)
			{
				ImageOut->Width = ScanInfo.OutputW;
				ImageOut->bits = ScanInfo.BitsPerPixel;
				if((ImageOut->Height < (ScanInfo.OutputH+8))&& (ImageOut->Height>8))
					ImageOut->Height-=7;
				else if(!bOverScan)
				{
					if(ImageIn->Height >= (ScanInfo.OutputH+8))
						ImageIn->Height = ScanInfo.OutputH;
				}
				ImageOut->JpegResize2_width();
			}
			memcpy(pBuffer,ImageOut->pimg+ImageOut->ImgShfit,Count);
			ImageOut->ImgShfit += Count;
			*RealCount = Count;
			if((ImageIn->bEnd)&&(ImageOut->ImgShfit == (int)ImageOut->ImgSize))
			{
				if(bCompress)
				{
					goto JPEG_END;
				}
				else 
				{
					goto RAW_END;
				}
			}
			else
			{
				*EndFlag = 0;
				byEndDoc = (BYTE)DocEnd;
			}
		}
		else if(bflag_decode_done) //jpeg
		{
			memcpy(pBuffer,ImageOut->pimg+ImageOut->ImgShfit,ImageOut->ImgSize-ImageOut->ImgShfit);
			*RealCount = ImageOut->ImgSize-ImageOut->ImgShfit;
			ImageOut->ImgShfit += ImageOut->ImgSize-ImageOut->ImgShfit;
JPEG_END:
			byEndDoc = (BYTE)DocEnd;
			*EndFlag = 1;
			jpeg_resize_free(&jrcb);//to free tmp jpeg output files buffer. ~40M.
			//reset varible
			ImageIn->Reset();
			ImageOut->Reset();
			bFirstReadScanEX=TRUE;
			bflag_decode_done = FALSE;
		}
		else if(ImageOut->bEnd) //raw
		{
			memcpy(pBuffer,ImageOut->pimg+ImageOut->ImgShfit,ImageOut->ImgSize-ImageOut->ImgShfit);
			*RealCount = ImageOut->ImgSize-ImageOut->ImgShfit;
			ImageOut->ImgShfit = ImageOut->ImgSize;
RAW_END:
			byEndDoc = (BYTE)DocEnd;
			*EndFlag = 1;
			ImageIn->Reset();
			ImageOut->Reset();
			if(raw_out_buf!=NULL)
			{
				free(raw_out_buf);
				raw_out_buf = NULL;
			}
			bFirstReadScanEX=TRUE;
		}
		else
		{
			goto READSOURCE;
		}
#endif
	}
#endif
	else  
	{
		return FALSE;
	}
	return (BYTE)result;
}
BYTE CLTCDrv::ADFIsReady(void)
{
	BYTE ret=1;
	if(bStopTimeOutFlag) //Scanning mode
	{
		dwErrorCode = ERRCODE_PaperTrayEmpty;
		return FALSE;
	}
	if(byEndDoc)
	{
ADFCHECK:
		ret = m_GLDrv->_StatusGet();
		if(!ret) {
			dwErrorCode = ERRCODE_ScannerDisconnect;
			return FALSE;
		}
		ret = m_GLDrv->_StatusCheck_ADF_Check();
		if(!ret)
		{
#if DEBUG_SCAN_FLOW
		LTCPrintf("ADFIsReady Status 1\n");
#endif
			ret = ErrorMapping_ADF();
			if(dwErrorCode == ERRCODE_PaperTrayEmpty)
			{
				return FALSE;
			}
			else
			{
				//dwErrorCode = 0;
				return TRUE;
			}
			/*ret = m_GLDrv->_info();
			if(!ret)
			{
				if(m_GLDrv->sc_infodata.Cancel)
				{
					bCancel = TRUE;
					dwErrorCode = ERRCODE_Cancel;
				}
				else if(m_GLDrv->sc_infodata.Error)
				{
					ret = m_GLDrv->_StatusGet();
					if(!ret) {
						dwErrorCode = ERRCODE_ScannerDisconnect;
						return FALSE;
					}
					ret = ErrorMapping_ADF();
				}
				else
				{
					dwErrorCode = ERRCODE_ScannerDisconnect;
					return FALSE;
				}
				
				return FALSE;
			}*/
		}
		else
		{
			return ret;
		}
	}
	else if(byADFMode && end_page[0]==1) //duplex page 1 end
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("ADFIsReady Status 2\n");
#endif
		return TRUE;
	}
	else 
	{
		/*while (ret && !byEndDoc)
		{
			ret = m_GLDrv->_info();
			if(m_GLDrv->sc_infodata.Cancel == 1 || bCancel)
			{
				bCancel = TRUE;
				dwErrorCode = ERRCODE_Cancel;
				return FALSE;
			}
			if((m_GLDrv->sc_infodata.ValidPageSize[0] > 0) || (m_GLDrv->sc_infodata.ValidPageSize[1] > 0)) {
			     break;
			}
			byEndDoc = m_GLDrv->sc_infodata.EndDocument;
			Sleep(200);//為解決在剛開始scan還沒有影像出來時，因SW發info polling FW太頻繁使 key press handle thread 一直沒有優先權執行問題。
		}*/
		if(byEndDoc)
		{
			goto ADFCHECK;
		}
		else
		{
#if DEBUG_SCAN_FLOW
			LTCPrintf("ADFIsReady Status 3\n");
#endif
			return TRUE;
		}
	}
//	return FALSE;
}
BYTE CLTCDrv::SetADFOptions(BYTE Mode,BYTE flowtype)
{
	flowtype = flowtype;
	bADFOption = TRUE;
	byADFMode = Mode; //  0:simplex 1:duplex
	return TRUE;
}
BYTE CLTCDrv::WriteNVRAM (ULONG ulStartLocation, BYTE * pbData, ULONG ulNumBytes)
{
	return m_GLDrv->_NVRAM_W((unsigned char)ulStartLocation,pbData,(unsigned char)ulNumBytes);
}
BYTE CLTCDrv::ReadNVRAM (ULONG ulStartLocation, BYTE * pbData, ULONG ulNumBytes)
{
	return m_GLDrv->_NVRAM_R((unsigned char)ulStartLocation,pbData,(unsigned char)ulNumBytes);
}
BYTE CLTCDrv::ReadScanEX_raw_sim(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count, DWORD *RealCount, BOOL *EndFlag)
{

	int result=0,dup=0,ImgSize=0,ReadSizeCount=0,ReadSize,duplex;
	ReadSize = Count;
	duplex = 1;//m_GLDrv->sc_pardata.duplex;
	byEndDoc = 0;
	pStatus = pStatus;
	while(ReadSizeCount<(long)Count)
	{	
		Sleep(5);
	
		m_GLDrv->sc_infodata.ValidPageSize[dup] =0;
		
		result = m_GLDrv->_info();
		bReadImg = TRUE;
		if(!result)
		{
			if(m_GLDrv->sc_infodata.Cancel)
			{
				bCancel = TRUE;
				dwErrorCode = ERRCODE_Cancel;
			}
			else if(m_GLDrv->sc_infodata.Error)
			{
				result = m_GLDrv->_StatusGet();
				{
					if(!result) 
					{
						dwErrorCode = ERRCODE_ScannerDisconnect;
						return FALSE;
					}
				}
				result = ErrorMapping_START();
			}
			else
			{
				dwErrorCode = ERRCODE_ScannerDisconnect;
			}
			
			return FALSE;
		}
		if(bCancel&&(m_GLDrv->sc_infodata.ValidPageSize[dup]>0))
		{
			dwErrorCode = ERRCODE_Cancel;
			return FALSE;
		}
		if(m_GLDrv->sc_infodata.ValidPageSize[dup] > 0)
		{
			if(bCompress)
			{
				result = m_GLDrv->_ReadImageEX(dup,&ImgSize,pBuffer+ReadSizeCount,(ReadSize-(ReadSize%64)));
			}
			else
			{
				result = m_GLDrv->_ReadImageEX(dup,&ImgSize,pBuffer+ReadSizeCount,ReadSize);
			}
			//printf("ImgSize %d ReadSize %d  m_GLDrv->sc_infodata.ValidPageSize[dup] %d\n",ImgSize,ReadSize,m_GLDrv->sc_infodata.ValidPageSize[dup]);
			if(!result)
			{
				LTCPrintf("Get Image Status/Data Error!!\n");
				dwErrorCode = ERRCODE_ScannerDisconnect;
				return FALSE;
			}
			ReadSizeCount += ImgSize;
			ReadSize	  -= ImgSize;
		}
		if(ImgSize >= (int)(m_GLDrv->sc_infodata.ValidPageSize[dup])) {
			end_page[dup] = m_GLDrv->sc_infodata.EndPage[dup];
			if((page_line[dup] == 0) && end_page[dup])
				page_line[dup] = m_GLDrv->sc_infodata.ImageLength[dup];
		}
		if((!(duplex & 1) || (m_GLDrv->sc_infodata.ValidPageSize[0] == 0)) &&  (!(duplex & 2) || (m_GLDrv->sc_infodata.ValidPageSize[1] == 0)))
		{
			Sleep(200);//為解決在剛開始scan還沒有影像出來時，因SW發info polling FW太頻繁使 key press handle thread 一直沒有優先權執行問題。
			if(!(m_GLDrv->_CheckScanningMode()))
			{
				dwErrorCode=ERRCODE_HatchOpen;
				return FALSE;
			}
		}
		if(result && (((duplex & 1) && (end_page[0] == 0)) || ((duplex & 2) && (end_page[1] == 0))))
		{
			continue;
		}
		if((!(duplex & 1) || end_page[0]) &&  (!(duplex & 2) || end_page[1]))
		{
			bImgAEnd = TRUE;
			if(!byEndDoc)
				 m_GLDrv->_info();
			byEndDoc = m_GLDrv->sc_infodata.EndDocument;
			//page_line[0] = page_line[1] = 0;
			end_page[0] = end_page[1] = 0;
			*EndFlag =1;
			bReadImg = FALSE;
			break;
		}
	}
	*RealCount = ReadSizeCount;
	return (BYTE)result;
}
BYTE CLTCDrv::ReadScanEX_raw_dup(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count, DWORD *RealCount, BOOL *EndFlag)
{
	int result=0,dup=0,ImgSize=0,ImgSizeB=0,ReadSizeCount=0,ReadSize,duplex=0;
	ReadSize = Count;
	byEndDoc = 0;
	pStatus = pStatus;
	//LTCPrintf2("%s+\n", __FUNCTION__);
	if(pDupImg == NULL)
	{
		pDupImg	= (BYTE*)malloc(sizeof(BYTE)*ScanInfo.SourceW*ScanInfo.SourceH*(ScanInfo.BitsPerPixel/8));
#if _CANOPUS_DEBUG_
		if(pDupImg ==NULL)
				LTCPrintf("pDupImg malloc fail\n");
#endif
		if(pDupImg ==NULL)
		{
			dwErrorCode = ERRCODE_MemoryNotEnough;
			return FALSE;
		}
	}
	if(end_page[0]==0)
	{
		duplex = 1;
		dup = 0;
	}
	else
	{
		duplex = 2;
		dup = 1;
	}
	while(ReadSizeCount<(long)Count)
	{	
		if(bCancel)
		{
			dwErrorCode = ERRCODE_Cancel;
			return FALSE;
		}
		if(( duplex == 2) && ((ReadSizeCountB-ImgBShift)>0))
		{
			if( (ReadSizeCountB-ImgBShift) >=(long) Count)
			{
				memcpy(pBuffer,pDupImg+ImgBShift,Count);
				ReadSizeCount	+= Count;
				ReadSize		-= Count;
				ImgBShift		+= Count;
			}
			else
			{
#if DEBUG_SCAN_FLOW
				LTCPrintf("Read Image B 2 \n");
#endif
				memcpy(pBuffer,pDupImg+ImgBShift,(ReadSizeCountB-ImgBShift));
				ReadSizeCount += (ReadSizeCountB-ImgBShift);
				ReadSize	  -= (ReadSizeCountB-ImgBShift);
				ImgBShift	  += (ReadSizeCountB-ImgBShift);
			}
			if ((ReadSizeCountB == ImgBShift)&&ImgBEnd)
			{
#if DEBUG_SCAN_FLOW
				LTCPrintf("Read Image B End \n");
#endif
				bImgBEnd = TRUE;
				ImgBEnd=FALSE;
				end_page[1]=1;
				*EndFlag = 1;
				end_page[0] = end_page[1] = 0;
				//page_line[0] = page_line[1] = 0;
				ReadSizeCountB = 0;
				ImgBShift	   = 0;
				byEndDoc = (BYTE)ImgBDocEnd;
				ImgBDocEnd = 0;
				break;
			}
			goto NEXTLOOP;
		}
		result = m_GLDrv->_info();
		//printf("ValidPageSize[0] = %d    ValidPageSize[1] = %d \n",m_GLDrv->sc_infodata.ValidPageSize[0] ,m_GLDrv->sc_infodata.ValidPageSize[1]);
		//bReadImg = TRUE;
		if(!result)
		{
			if(m_GLDrv->sc_infodata.Cancel)
			{
				bCancel = TRUE;
				dwErrorCode = ERRCODE_Cancel;
			}
			else if(m_GLDrv->sc_infodata.Error)
			{
				result = m_GLDrv->_StatusGet();
				{
					if(!result) 
					{
						dwErrorCode = ERRCODE_ScannerDisconnect;
						return FALSE;
					}
				}
				result = ErrorMapping_START();
			}
			else
			{
				dwErrorCode = ERRCODE_ScannerDisconnect;
			}
			
			return FALSE;
		}
	
		if(m_GLDrv->sc_infodata.ValidPageSize[dup] > 0)
		{
			if(bCompress)
			{
				result = m_GLDrv->_ReadImageEX(dup,&ImgSize,pBuffer+ReadSizeCount,(ReadSize-(ReadSize%64)));
			}
			else
			{
				result = m_GLDrv->_ReadImageEX(dup,&ImgSize,pBuffer+ReadSizeCount,ReadSize);
			}
			if(!result)
			{
				LTCPrintf("Get Image Status/Data Error!!\n");
				dwErrorCode = ERRCODE_ScannerDisconnect;
				return FALSE;
			}			
			ReadSizeCount += ImgSize;
			ReadSize	  -= ImgSize;
		}
		if(ImgSize >= (int)(m_GLDrv->sc_infodata.ValidPageSize[dup]) ) {
			end_page[dup] = m_GLDrv->sc_infodata.EndPage[dup];
			if((page_line[dup] == 0) && end_page[dup])
			{
				page_line[0] =page_line[dup] = m_GLDrv->sc_infodata.ImageLength[dup];
			}
		}
		
		if(dup == 0  && !ImgBEnd)
		{
			if(m_GLDrv->sc_infodata.ValidPageSize[1] > 0)
			{
				if(bCompress)
				{
					result = m_GLDrv->_ReadImageEX(1,&ImgSizeB,pDupImg+ReadSizeCountB,(Count-(Count%64)));
				}
				else
				{
					result = m_GLDrv->_ReadImageEX(1,&ImgSizeB,pDupImg+ReadSizeCountB,Count);
				}
				if(!result)
				{
					LTCPrintf("Get Image Status/Data Error!!\n");
					dwErrorCode = ERRCODE_ScannerDisconnect;	
					return FALSE;
				}
				ReadSizeCountB += ImgSizeB;
				if(ImgSizeB >= (int)(m_GLDrv->sc_infodata.ValidPageSize[1]))
				{
					ImgBEnd = m_GLDrv->sc_infodata.EndPage[1];
					if((page_line[1] == 0) && ImgBEnd)
					{
						LTCPrintf("ImgSizeB > ValidPageSize ImgBEnd %d\n ",ImgBEnd);
						page_line[1] = m_GLDrv->sc_infodata.ImageLength[1];
					}
				}
			}
			if((!(duplex & 1) || end_page[0]) &&  (!(duplex & 2) ||ImgBEnd))
			{
				ImgBDocEnd = m_GLDrv->sc_infodata.EndDocument;
			}
		}
		if((!(duplex & 1) || (m_GLDrv->sc_infodata.ValidPageSize[0] == 0)) &&  (!(duplex & 2) || (m_GLDrv->sc_infodata.ValidPageSize[1] == 0)))
		{
			Sleep(200);//為解決在剛開始scan還沒有影像出來時，因SW發info polling FW太頻繁使 key press handle thread 一直沒有優先權執行問題。
			if(!(m_GLDrv->_CheckScanningMode()))
			{
				dwErrorCode=ERRCODE_HatchOpen;
				return FALSE;
			}
		}
NEXTLOOP:
		if(result && (((duplex & 1) && (end_page[0] == 0)) || ((duplex & 2) && (end_page[1] == 0))))
		{
			continue;
		}
		if((!(duplex & 1) || end_page[0]) &&  (!(duplex & 2) || end_page[1]))
		{

			LTCPrintf("duplex = %d end_page[0] %d end_page[1] %d\n",duplex,end_page[0],end_page[1]);
			if(end_page[1])
			{
				end_page[0] = end_page[1] = 0;
				//page_line[0] = page_line[1] = 0;
				ReadSizeCountB = 0; 
				ImgBShift	   = 0;
				bImgBEnd       = TRUE;
				*EndFlag =1;
                if(!byEndDoc)
					m_GLDrv->_info();				
                byEndDoc = m_GLDrv->sc_infodata.EndDocument;
			}
			end_page[1] = 0;
			bImgAEnd = TRUE;
			*EndFlag =1;
			break;
		}
	}
	*RealCount = ReadSizeCount;
	//LTCPrintf2("%s-\n", __FUNCTION__);
	return TRUE;
}
int CLTCDrv::GetSourceRes(int Res)
{
	if(Res<=200)
		return 200;
	else if(Res<=300)
		return 300;
	else if(Res<=600)
		return 600;
	else
		return 1200;
}
int CLTCDrv::PixelAlignment(int width,int Alignment)
{
	int ret;
	ret = (int)(width / Alignment);
	if(width%Alignment)
		ret = (ret+1) * Alignment;
	else
		ret = ret * Alignment;
	return ret;
}
int CLTCDrv::PixelAlignment_org(int width,int Alignment)
{
	int ret;
	ret = (int)(width / Alignment);
	ret = ret * Alignment;
	return ret;
}
BYTE CLTCDrv::SetCompress(bool Enable)
{
	if(Enable)
		bCompress=TRUE;
	else
		bCompress=FALSE;
	return TRUE;
}
BYTE CLTCDrv::GetCompress()
{
	return (BYTE)bCompress;
}
BYTE CLTCDrv::SetRGBMatrix(RGB_MATRIX *pMatrix)
{	
	float matrix[9];
	matrix[0] = (float)pMatrix->dm11;
	matrix[1] = (float)pMatrix->dm12;
	matrix[2] = (float)pMatrix->dm13;
	matrix[3] = (float)pMatrix->dm21;
	matrix[4] = (float)pMatrix->dm22;
	matrix[5] = (float)pMatrix->dm23;
	matrix[6] = (float)pMatrix->dm31;
	matrix[7] = (float)pMatrix->dm32;
	matrix[8] = (float)pMatrix->dm33;
	return m_GLDrv->_matrix(matrix);
}
BYTE CLTCDrv::SetSleepAutoOffTime(unsigned int sleep, unsigned int auto_off)
{
	return m_GLDrv->_SetTime(sleep,auto_off);
}

BOOL CLTCDrv::GetScannerStatus (DWORD *ErrorCode)
{
	*ErrorCode = 0;
	*ErrorCode = dwErrorCode;
	return (TRUE);
}

BYTE CLTCDrv::ButtonStatusGet(unsigned char *duplex, unsigned char *mode)
{
	return m_GLDrv->_ButtonStatusGet(duplex,mode);
}

BYTE CLTCDrv::ReadSourceImage()
{
	int result=1;
	int duplex = 0, dup = 0;
	int ImgSize=0;
	if(bCompress) // 0: raw 1:jpeg
	{
		duplex = 1;
		ImageA->pimg = (BYTE*)malloc(sizeof(BYTE)*JPEGIMGSIZE);
		ImageA->ImgType = I3('JPG');
		if(byADFMode) // 0:simplex 1:duplex
		{
			duplex = 3;
			ImageB->pimg = (BYTE*)malloc(sizeof(BYTE)*JPEGIMGSIZE);
			ImageB->ImgType = I3('JPG');
		}
	}
	else
	{
		duplex = 1;
		ImageA->pimg = (BYTE*)malloc(sizeof(BYTE)*RAWIMGSIZE);
		ImageA->ImgType = I3('RAW');
		if(byADFMode) // 0:simplex 1:duplex
		{
			duplex = 3;
			ImageB->pimg = (BYTE*)malloc(sizeof(BYTE)*RAWIMGSIZE);
			ImageB->ImgType = I3('RAW');
		}
	}


	l_Info_:
	result = m_GLDrv->_info();
	bReadImg = TRUE;
	if(!result)
	{
	/*	printf("Get _info Fail!!\n");
		if(m_GLDrv->sc_infodata.Cancel)
		{
			bCancel = TRUE;
			dwErrorCode = ERRCODE_Cancel;
		}
		else if(m_GLDrv->sc_infodata.PaperJam)
		{
			dwErrorCode = ERRCODE_PaperJam;
		}
		else if(m_GLDrv->sc_infodata.CoverOpen)
		{
			dwErrorCode = ERRCODE_HatchOpen;
		}
		else
		{
			dwErrorCode = ERRCODE_ScannerDisconnect;
		}*/
		return FALSE;
	}
	if((duplex & 1) && (Source_end_page[0] == 0))
	{
		dup = 0;
		ImgSize = 0;

		if(m_GLDrv->sc_infodata.ValidPageSize[dup] > 0) 
		{
			result = m_GLDrv->_ReadImageEX(dup,&ImgSize,ImageA->pimg+ImageA->ImgSize,m_GLDrv->sc_infodata.ValidPageSize[dup]);
			if(!result)
			{
				LTCPrintf("_ReadImageEX A Fail\n");
				LTCPrintf("Get Image Status/Data Error!!\n");
				dwErrorCode = ERRCODE_ScannerDisconnect;
				return FALSE;
			}
			ImageA->ImgSize += ImgSize;
		}
		if(ImgSize >=(int)(m_GLDrv->sc_infodata.ValidPageSize[dup]))
		{
			Source_end_page[dup] = m_GLDrv->sc_infodata.EndPage[dup];
			if((page_line[dup] == 0) && Source_end_page[dup])
			{
				page_line[dup] = m_GLDrv->sc_infodata.ImageLength[dup];
			}
		}
	}
	if((duplex & 2) && (Source_end_page[1] == 0))
	{
		dup = 1;
		ImgSize = 0;
		if(m_GLDrv->sc_infodata.ValidPageSize[dup] > 0)
		{
			result = m_GLDrv->_ReadImageEX(dup,&ImgSize,ImageB->pimg+ImageB->ImgSize,m_GLDrv->sc_infodata.ValidPageSize[dup]);
			if(!result)
			{
				LTCPrintf("_ReadImageEX B Fail\n");
				LTCPrintf("Get Image Status/Data Error!!\n");
				dwErrorCode = ERRCODE_ScannerDisconnect;
				return FALSE;
			}
			ImageB->ImgSize += ImgSize;
		}
		if(ImgSize >= (int)(m_GLDrv->sc_infodata.ValidPageSize[dup])) {
			Source_end_page[dup] = m_GLDrv->sc_infodata.EndPage[dup];
			if((page_line[dup] == 0) && Source_end_page[dup])
				page_line[dup] = m_GLDrv->sc_infodata.ImageLength[dup];
		}
	}
	if((!(duplex & 1) || (m_GLDrv->sc_infodata.ValidPageSize[0] == 0)) &&  (!(duplex & 2) || (m_GLDrv->sc_infodata.ValidPageSize[1] == 0)))
	{
		Sleep(200);//為解決在剛開始scan還沒有影像出來時，因SW發info polling FW太頻繁使 key press handle thread 一直沒有優先權執行問題。
		if(!(m_GLDrv->_CheckScanningMode()))
		{
			dwErrorCode=ERRCODE_HatchOpen;
			return FALSE;
		}
	}
	if(result && (((duplex & 1) && (Source_end_page[0] == 0)) || ((duplex & 2) && (Source_end_page[1] == 0))))
		goto l_Info_;
	
	ImageA->Width  = ScanInfo.SourceW;
	ImageA->Height = page_line[0];
	ImageA->Xres   = ScanInfo.SourceXres;
	ImageA->Yres   = ScanInfo.SourceYres;
	ImageA->bits   = ScanInfo.BitsPerPixel;	
#if _CANOPUS_DEBUG_
	LTCPrintf("ReadSourceImage End  ImageA->Height :%d   ImageA->ImgSize %d\n",ImageA->Height,ImageA->ImgSize);
#endif 
	if(byADFMode)
	{
		ImageB->Width  = ScanInfo.SourceW;
		ImageB->Height = page_line[1];
		ImageB->Xres   = ScanInfo.SourceXres;
		ImageB->Yres   = ScanInfo.SourceYres;
		ImageB->bits   = ScanInfo.BitsPerPixel;	
#if _CANOPUS_DEBUG_
	LTCPrintf("ReadSourceImage End  ImageB->Height :%d ImageB->ImgSize : %d \n",ImageB->Height,ImageB->ImgSize );
#endif 
	}
	if(bCompress)
	{
		ImageA->JpegResize();
		if(byADFMode)
			ImageB->JpegResize();
	}	
	Source_end_page[0] = Source_end_page[1] = 0;
	bGetSourceImg = TRUE;
#if 0 //output img
	FILE *fout;
	fout = fopen("source.raw", "wb");
	if(fout) {
		printf("ImageA->Height : %d \n",ImageA->Height);
		printf("ImageA->Width : %d \n",ImageA->Width);
		printf("ImageA->ImgSize : %d \n",ImageA->ImgSize);
		fwrite(ImageA->pimg, sizeof(unsigned char), ImageA->ImgSize, fout);
		fclose(fout);
	}
#endif
	return TRUE;
}
BYTE CLTCDrv::ReadSourceImage(IMGInfo *Source,DWORD Count,DWORD *RealCount,BOOL Duplex,BOOL *End,BOOL *DocEnd)
{
	BYTE bRet = 0;
	LPBYTE pStatus=0;
	CHECK_HEAP();
	//LTCPrintf2("%s+\n", __FUNCTION__);
	if(!Duplex)
		bRet = ReadScanEX_raw_sim(pStatus,(Source->pimg+Source->ImgSize),Count,RealCount,End);
	else
		bRet = ReadScanEX_raw_dup(pStatus,(Source->pimg+Source->ImgSize),Count,RealCount,End);
	Source->ImgSize += *RealCount;
	Source->bEnd = *End;
	if(Source->bEnd)
	{
		*DocEnd = byEndDoc;
	}
	//LTCPrintf2("%s-\n", __FUNCTION__);
	return bRet;
}
BYTE CLTCDrv::SendOutputImage(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount,BOOL *EndFlag)
{
	int duplex = 0;

	if(byADFMode)
		duplex = 3;
	else
		duplex = 1;
	byEndDoc = FALSE;
	pStatus = pStatus;
	if(byADFMode && (ImageA->ImgSize==0))
	{
		if((end_page[1] == 0)&&(ImageB->ImgSize==0))
		{
			byEndDoc = TRUE;
			end_page[0] = end_page[1] = 0;
			page_line[0] = page_line[1] = 0;
			return FALSE;
		}
		if(ImageB->ImgSize >= Count)
		{
			memcpy(pBuffer,(ImageB->pimg + ImageB->ImgShfit),Count);
			*RealCount		=  Count;
			ImageB->ImgSize -= Count;
			ImageB->ImgShfit+= Count;
			if(ImageB->ImgSize == 0)
			{
				*EndFlag = end_page[1] = 1;
			}
			else
			{
				end_page[1] = 0;
			}
		}
		else
		{
			memcpy(pBuffer,(ImageB->pimg + ImageB->ImgShfit),ImageB->ImgSize);
			*RealCount		=  ImageB->ImgSize;
			ImageB->ImgSize -= ImageB->ImgSize;
			ImageB->ImgShfit+= ImageB->ImgSize;
			*EndFlag = end_page[1] = 1;
		}
	}
	else
	{
		if((end_page[0] == 0)&&(ImageA->ImgSize==0))
		{
			byEndDoc = TRUE;
			end_page[0] = end_page[1] = 0;
			page_line[0] = page_line[1] = 0;
			return FALSE;
		}
		if(ImageA->ImgSize >= Count)
		{
			memcpy(pBuffer,(ImageA->pimg + ImageA->ImgShfit),Count);
			*RealCount		=  Count;
			ImageA->ImgSize -= Count;
			ImageA->ImgShfit+= Count;
			if(ImageA->ImgSize == 0)
			{
				//ImageA->FreePimg();
				*EndFlag = end_page[0] = 1;
			}
			else
			{
				end_page[0] = 0;
			}
		}
		else
		{
			memcpy(pBuffer,(ImageA->pimg + ImageA->ImgShfit),ImageA->ImgSize);
			*RealCount		=  ImageA->ImgSize;
			ImageA->ImgSize -= ImageA->ImgSize;
			ImageA->ImgShfit+= ImageA->ImgSize;
			*EndFlag = end_page[0] = 1;
			ImageA->FreePimg();
		
		}
	}
	if((!(duplex & 1) || end_page[0]) &&  (!(duplex & 2) || end_page[1]))
	{
#if _CANOPUS_DEBUG_
		LTCPrintf("SendOutputImage End \n");
#endif 
		ImageA->Reset();
		if(end_page[1])
		{
			ImageB->Reset();
		}
		if(!byEndDoc)
				 m_GLDrv->_info();
		byEndDoc = m_GLDrv->sc_infodata.EndDocument;
		end_page[0] = end_page[1] = 0;
		page_line[0] = page_line[1] = 0;
		bReadImg	  = FALSE;
		bGetSourceImg = FALSE;
	}
	return TRUE;
}

BYTE CLTCDrv::InputImgTransfer_Scale(IMGInfo *Source,IMGInfo *Temp,SCANINFO Info)
{

	int ret = TRUE;
	Temp->ImgType = Source->ImgType;
	Temp->Height  = Info.OutputH;
	Temp->Width   = Info.OutputW;
	Temp->Xres	  = Info.OutputXres;
	Temp->Yres	  = Info.OutputYres;
	Temp->bits	  = Info.BitsPerPixel;
	if(Info.BitsPerPixel<8)
		Temp->ImgSize = Info.OutputW*Info.OutputH/8;  //BW mode
	else
		Temp->ImgSize = Info.OutputW*Info.OutputH*(Info.BitsPerPixel/8);
	Temp->pimg = (BYTE*)malloc(Temp->ImgSize);

	if(Source->ImgType == I3('RAW'))
	{
		resample(Temp->Width, Temp->Height, Temp->pimg, Source->Width, Source->Height, Source->pimg, Info.BitsPerPixel, RESAMPLE_NEAREST);
	}
	else if(Source->ImgType == I3('JPG'))
	{
		LtcImageInfo info_in;

		// user provide the jpeg file in Source->pimg.
		// this api will decode jpeg to info_in.image_buffer
		// user should release image_buffer later.
		dmemjpeg(&info_in, Source->pimg, Source->ImgSize);
		resample(Temp->Width, Temp->Height, Temp->pimg, info_in.width, info_in.height, info_in.image_buffer, Info.BitsPerPixel, RESAMPLE_NEAREST);
		free(info_in.image_buffer);
	}
#if 0 //output img
	FILE *fout;
	fout = fopen("out.raw", "wb");
	if(fout) {
		printf("Info.BitsPerPixel : %d \n",Info.BitsPerPixel);
		printf("Temp->Height : %d \n",Temp->Height);
		printf("Temp->Width : %d \n",Temp->Width);
		printf("Temp->ImgSize : %d \n",Temp->ImgSize);
		fwrite(Temp->pimg, sizeof(unsigned char), Temp->ImgSize, fout);
		fclose(fout);
	}
#endif
	return (BYTE)ret;
}
BYTE CLTCDrv:: OutputImgTransfer(IMGInfo *Output,IMGInfo *Temp,SCANINFO Info)
{
	int ret = TRUE;
	Output->ImgType	= Temp->ImgType;
	Output->Height  = Temp->Height;
	Output->Width   = Temp->Width;
	Output->Xres	= Temp->Xres;
	Output->Yres	= Temp->Yres;

	if(Temp->ImgType == I3('RAW'))
	{
		Output->ImgSize	= Temp->ImgSize;
		memcpy(Output->pimg,Temp->pimg,Temp->ImgSize);
	}
	else if(Temp->ImgType == I3('JPG'))
	{
		LtcImageInfo info_out;
		unsigned char *outFileBuf;
		info_out.width		= Output->Width;
		info_out.height		= Output->Height;	
		info_out.x_density	= Output->Xres;
		info_out.y_density	= Output->Yres;
		info_out.quality	= 75;
		info_out.components = (Info.BitsPerPixel /8);
		info_out.image_buffer = Temp->pimg;
		// this api will encode info_out.image_buffer to jpeg.
		// save the jpeg to outFileBuf.
		cmemjpeg(&info_out, &outFileBuf, &(Output->ImgSize));
		memcpy(Output->pimg,outFileBuf,Output->ImgSize);
		free(outFileBuf);
	}
	Temp->Reset();
	return (BYTE)ret;
}

BYTE CLTCDrv::GammaTransLTCtoGL(WORD *pbyRed, WORD *pbyGreen, WORD *pbyBlue,unsigned int *GLGamma)
{
	int i;
	for(i=0;i<256;i++)
	{
		GLGamma[i]		= ((*(pbyRed+i*256))&0xffff) + (((*(pbyRed+((i+1)*256)-1))&0xffff)<<16); 
		GLGamma[i+256]	= ((*(pbyGreen+i*256))&0xffff) + (((*(pbyGreen+((i+1)*256)-1))&0xffff)<<16); 
		GLGamma[i+256*2]= ((*(pbyBlue+i*256))&0xffff) + (((*(pbyBlue+((i+1)*256)-1))&0xffff)<<16); 
	}
	return TRUE;
}
BYTE CLTCDrv::SetGamma(DWORD uSz,WORD *pbyRed, WORD *pbyGreen, WORD *pbyBlue)
{
	unsigned int gGammaData[768];
	uSz = uSz;
	GammaTransLTCtoGL(pbyRed,pbyGreen,pbyBlue,gGammaData);
#if 0 //output 8bit gamma
	FILE *gaout;
	int i;
	gaout=fopen("gamma.csv","w");
	for(i=0;i<65536;i=i+256)
	{
		fprintf(gaout,"%d,%d,%d\n",((*(pbyRed+i))>>8),((*(pbyGreen+i))>>8),((*(pbyBlue+i))>>8));
	}
	fclose(gaout);
#endif
#if 0//output 16bit gamma
	FILE *gaout;
	int i;
	gaout=fopen("gamma.csv","w");
	for(i=0;i<65536;i++)
	{
		fprintf(gaout,"%d,%d,%d\n",((*(pbyRed+i))),((*(pbyGreen+i))),((*(pbyBlue+i))));
	}
	fclose(gaout);
#endif
#if 0 // output gl310 gamma
	FILE *gaout;
	int i;
	gaout=fopen("gamma.bin","wb");
	fwrite(gGammaData,sizeof(int)*768,1,gaout);
	fclose(gaout);
#endif
	/*for(i=0;i<256;i++)
		printf("up: %d  low : %d  \n",(((gGammaData[i])>>16)&0xffff),((gGammaData[i])&0xffff));*/
/*	unsigned int gGammaData[768];
	int i;

	for (i=0; i < 256; i++) {
		if(i<=254) {
			gGammaData[i]=(unsigned int)(0xffff-0x0100*i) + ((unsigned int)(0xffff-0x0100*(i+1))<<16);
			gGammaData[i+256]=(unsigned int)(0xffff-0x0100*i) + ((unsigned int)(0xffff-0x0100*(i+1))<<16);
			gGammaData[i+256*2]=(unsigned int)(0xffff-0x0100*i) + ((unsigned int)(0xffff-0x0100*(i+1))<<16);
		} else {
			gGammaData[i]=(unsigned int)(0xffff-0x0100*i) + (0x0000<<16);
			gGammaData[i+256]=(unsigned int)(0xffff-0x0100*i) + (0x0000<<16);
			gGammaData[i+256*2]=(unsigned int)(0xffff-0x0100*i) + (0x0000<<16);
		}
	}*/
	
	return m_GLDrv->_gamma(gGammaData);
}
BYTE CLTCDrv::GrayImgToBWImg(IMGInfo *Source)
{
	unsigned char *bw_target;
	int BWsize = Source->Height*Source->Width/8;
	bw_target = (unsigned char*)malloc(BWsize);
	gray2bw(Source->Width,Source->Height,Source->pimg,bw_target,ScanInfo.Threshold);
	memcpy(Source->pimg,bw_target,BWsize);
	free(bw_target);
	Source->ImgSize = BWsize;
	return TRUE;
}
BYTE CLTCDrv::gray2bw(int w, int h, unsigned char* gray, unsigned char* bw,int threshold)
{
	unsigned char *in, *out, ch;
	int i, j, k, l, w8 = w/8 , w8_r = w%8 ;
	in = gray;
	out = bw;
#if 1
	for(i=0; i<h; i++) {	
		for(j=0; j<w8; j++) {
			ch = 0;
			for(k=0; k<8; k++) 
				if(in[k]>=threshold) ch|=(0x01<<(7-k));
			*out = ch;
			out++;
			if(j==(w8-1)&&w8_r)
			{	
				ch = 0;
				in+=8;
				for(l=0;l<w8_r;l++)
				{
					if(in[l]>=threshold) ch|=(0x01<<(7-l));
				}
				*out = ch;
				in+=w8_r;
				out++;
			}
			else
				in+=8;
		}
	}
#else
	for(i=0; i<h; i++) {	
		for(j=0; j<w8; j++) {
			ch = 0;
			for(k=0; k<8; k++) 
				if(in[7-k]>threshold) ch|=(0x01<<k);
			*out = ch;
			in+=8;
			out++;
		}
	}
#endif
	return 1;
}

void CLTCDrv::ReleaseImg_ResetVar()
{
	LTCPrintf("ReleaseImg_ResetVar\n");
	ImageA->Reset();
	ImageB->Reset();
	Image_tmp->Reset();;
	ImageIn->Reset();
	ImageOut->Reset();
	if(raw_out_buf!=NULL)
	{
		free(raw_out_buf);
		raw_out_buf = NULL;
	}
	Source_end_page[0] = Source_end_page[1] = 0;
	end_page[0] = end_page[1] = 0;
	page_line[0] = page_line[1] = 0;
	byEndDoc		= 1;	
	if(bCompress && !bFirstReadScanEX)
	{
		jpeg_resize_free(&jrcb);
	}
	if(!bADFOption)
	{
		bCancel		= FALSE;
	}
	bCompress		= FALSE;
	bGetSourceImg	= FALSE;
	bOverScan		= FALSE;
	bGamma			= FALSE;
	bReadImg		= FALSE; //for cancel
	ImgBEnd			= FALSE;
	ImgBDocEnd		= FALSE;
	bImgAEnd		= FALSE;
	bImgBEnd		= FALSE;
	ReadSizeCountB	= 0;
	ImgBShift		= 0;
	bScale			= FALSE;
	bColormatrix	= FALSE;
	bBWmodeScan		= FALSE;
	bADFOption		= FALSE;
	byADFMode		= FALSE;
	bFirstReadScanEX=TRUE;
	bflag_decode_done = FALSE;
	bTonemap		= FALSE;
	bStopTimeOutFlag=FALSE;


	if(pDupImg!=NULL)
	{
		free(pDupImg);
		pDupImg=NULL;
	}
}
void CLTCDrv::ADF_ReleaseImg_ResetVar()
{
	LTCPrintf("ADF_ReleaseImg_ResetVar\n");
	ImageA->Reset();
	ImageB->Reset();
	Image_tmp->Reset();;
	ImageIn->Reset();
	ImageOut->Reset();
	if(raw_out_buf!=NULL)
	{
		free(raw_out_buf);
		raw_out_buf = NULL;
	}
	Source_end_page[0] = Source_end_page[1] = 0;
	end_page[0] = end_page[1] = 0;
	page_line[0] = page_line[1] = 0;
	byEndDoc		= 1;	
	if(bCompress && !bFirstReadScanEX)
	{
		jpeg_resize_free(&jrcb);
	}
	bReadImg		= FALSE; //for cancel
	ImgBEnd			= FALSE;
	ImgBDocEnd		= FALSE;
	bImgAEnd		= FALSE;
	bImgBEnd		= FALSE;
	ReadSizeCountB	= 0;
	ImgBShift		= 0;
	bFirstReadScanEX=TRUE;
	bflag_decode_done = FALSE;
	bStopTimeOutFlag=FALSE;
	bCancel=0;
	if(pDupImg!=NULL)
	{
		free(pDupImg);
		pDupImg=NULL;
	}
}
BYTE CLTCDrv::ADFEndJob_()
{
	int ret=TRUE;
	if(bCancel)
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("ADFEndJob Cancel +\n");
#endif
		dwErrorCode = ERRCODE_Cancel;
		if(bStartScanFlag)
		{
			ret = m_GLDrv->_cancel();
			bStartScanFlag = FALSE;
		}
		if(bJobCreatFlag)
		{
			bJobCreatFlag = FALSE;
			m_GLDrv->_JobEnd();
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("ADFEndJob Cancel -\n");
#endif
		return FALSE;
	}
	else
	{
#if DEBUG_SCAN_FLOW
		LTCPrintf("ADFEndJob Stop +\n");
#endif
		dwErrorCode = ERRCODE_Cancel;
		if(bStartScanFlag)
		{
			ret = m_GLDrv->_stop();
			bStartScanFlag = FALSE;
		}
		if(bJobCreatFlag)
		{
			bJobCreatFlag = FALSE;
			m_GLDrv->_JobEnd();
		}
#if DEBUG_SCAN_FLOW
		LTCPrintf("ADFEndJob Stop -\n");
#endif
		return FALSE;
	}
}
BYTE CLTCDrv::ErrorMapping_ADF()
{
	BYTE result=0;
	
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
	if(m_GLDrv->sc_status_data.system & 0x0e) {

		dwErrorCode = ERRCODE_PaperTrayEmpty;
		result = 1;
		
	}
	else if( m_GLDrv->sc_status_data.sensor & 0x02 ) {
		dwErrorCode = ERRCODE_PaperTrayEmpty;
		result = 1;
	}
	else if(  m_GLDrv->sc_status_data.sensor & 0x04 ) {
	#if _GLDEBUG_
			LTCPrintf("Sensor status fail!(scan)\n");
	#endif
		dwErrorCode = ERRCODE_PaperJam;
		result = 1;
	}
	else if( m_GLDrv->sc_status_data.sensor & 0x08 ) {
		dwErrorCode = ERRCODE_HatchOpen;
		result = 1;
	}
	else if(  m_GLDrv->sc_status_data.error & 0x04 ) {
		dwErrorCode = ERRCODE_HatchOpen;
		result = 1;
	}
	else if( m_GLDrv->sc_status_data.error & 0x08 ) {
		dwErrorCode = ERRCODE_PaperJam;
		result = 1;
	}
	else
	{
		dwErrorCode = ERRCODE_ScannerDisconnect;
		result = 1;
	}

	
	return result;
}
BYTE CLTCDrv::ErrorMapping_START()
{
	BYTE result=0;
	
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
	/*if(m_GLDrv->sc_status_data.system & 0x0e) {
		dwErrorCode = ERRCODE_ScannerDisconnect;
		printf("System not ready!\n");
		result = 0;
	}
	else */
	if(  m_GLDrv->sc_status_data.error & 0x04  ) {
		dwErrorCode = ERRCODE_HatchOpen;
		result = 1;
	}
	else if( m_GLDrv->sc_status_data.sensor & 0x08 ) {
		dwErrorCode = ERRCODE_HatchOpen;
		result = 1;
	}
	else if( m_GLDrv->sc_status_data.error & 0x08 ) {
		dwErrorCode = ERRCODE_PaperJam;
		result = 1;
	}
	else
	{
		dwErrorCode = ERRCODE_ScannerDisconnect;
		result = 1;
	}
	
	return result;
}


IMGInfo::IMGInfo()
{
	ImgType	=0;
	Width	=0;
	Height	=0;
	Xres	=0;
	Yres	=0;
	ImgSize	=0;
	ImgShfit=0;
	bits	=0;
	bEnd    =FALSE;
	pimg   =NULL;
}
IMGInfo::~IMGInfo()
{
	if(pimg!=NULL)
	{
		free(pimg);
		pimg = NULL;
	}
}
BYTE IMGInfo::JpegResize()
{
	
	U8 dpi[5] = {0x01, 0x01, 0x2c, 0x01, 0x2c};
	U8 height[2];
	dpi[0] = 0x01;
	dpi[1] = (U8)(Xres >> 8);
	dpi[2] = (U8)(Xres);
	dpi[3] = (U8)(Yres >> 8);
	dpi[4] = (U8)Yres;
	height[0] = (U8)(Height >> 8);
	height[1] = (U8)Height;
	memcpy(pimg+0x0d,dpi,sizeof(dpi));
	if(bits<24)
		memcpy(pimg+0x64,height,sizeof(height));  // bit>=24 ? 0xe6: 0x64
	else
		memcpy(pimg+0xe6,height,sizeof(height));  // bit>=24 ? 0xe6: 0x64
	return TRUE;
}
BYTE IMGInfo::JpegResize2()
{
	
	U8 height[2];
	height[0] = (U8)(Height >> 8);
	height[1] = (U8)Height;
	if(bits<24)
		memcpy(pimg+0x5e,height,sizeof(height));  // 
	else
		memcpy(pimg+0xa3,height,sizeof(height));  // 
	
	JpegResize2_width();
	return TRUE;
}
BYTE IMGInfo::JpegResize2_width()
{
	U8 width[2];
	width[0] = (U8)(Width >> 8);
	width[1] = (U8)Width;
	if(bits<24)
		memcpy(pimg+0x60,width,sizeof(width));  // bit>=24 ? 0xe6: 0x64
	else
		memcpy(pimg+0xa5,width,sizeof(width));  // bit>=24 ? 0xe6: 0x64

	return TRUE;
}
void IMGInfo::Reset()
{
	ImgType	=0;
	Width	=0;
	Height	=0;
	Xres	=0;
	Yres	=0;
	ImgSize	=0;
	ImgShfit=0;
	bEnd	=FALSE;
	if(pimg!=NULL)
	{
		free(pimg);
		pimg = NULL;
	}
}
void IMGInfo::FreePimg()
{
	if(pimg!=NULL)
	{
		free(pimg);
		pimg = NULL;
	}
}