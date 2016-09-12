#ifndef _LTCDriveApi
#define _LTCDriveApi
#include "jpeg_resize.h"
#include "LTCStructure.h"
#include "GLScan.h"

class IMGInfo
{
public:
	IMGInfo();
	~IMGInfo();
	BYTE JpegResize();
	BYTE JpegResize2();
	BYTE JpegResize2_width();
	void Reset();
	void FreePimg();
	unsigned int	ImgType;
	int				Width;
	int				Height;
	int				Xres;
	int				Yres;
	DWORD			ImgSize;
	int				ImgShfit;
	BYTE			*pimg;
	int				bits;
	BOOL			bEnd;
};

class CLTCDrv
{
public:
	CLTCDrv();
	~CLTCDrv();
	CGLDrv *m_GLDrv;
//Liteon command interface
	BYTE FindScannerEx();
	BYTE FindScannerEx(LPCTSTR lpModuleName);
	BYTE InitializeDriver(VOID);
	BYTE TerminateDriver(VOID);
	BYTE InitializeScanner(VOID);
	BYTE GetADFMode(LPADFPARAMETER pADFParam);
	BYTE GetScannerAbilityEx(LPSCANNERABILITYEX pAbilityEX);
	BYTE SetScanParameter(LPLTCSCANPARAMETER lpScanParam);
	BYTE GetScanParameter(LPLTCSCANPARAMETER lpScanParam);
	BYTE StartScan(VOID);
	BYTE ReadScan(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount);
	BYTE StopScan(VOID);
	BYTE CancelScan(VOID);
	BYTE ReadPushButton(LPREADBUTTONINDEX pIndex);
	BYTE SetSecondGamma(LPBYTE GammaTable, BYTE Color, DWORD Size);
	BYTE ReadSN(BYTE *pSN, WORD len);
	BYTE ReadPageCount(DWORD *page);
	BYTE WritePageCount(DWORD *page);
	BYTE ReadShippingDate(WORD *year, WORD *month, WORD *day);
	BYTE WriteShippingDate(WORD year, WORD month, WORD day);
	BYTE SetScannerSleepTime(int minutes); //change byte to int  snow 14.11.17
	BYTE GetScannerSleepTime(int *minutes);//change byte to int
	BYTE WriteSN(BYTE *pSN, WORD len);
	BYTE SetVendorProductString (BYTE *VendorStr, BYTE *ProductStr);
	BYTE GetLLDVersion(BYTE *Version, BYTE len);
	BYTE EnableButtonEvents(BYTE bEnable);
	BYTE TerminateScanner(VOID);
	BYTE GetFWVersion(char *Version, BYTE len);
	BYTE SetTempFileFolder(WCHAR *Folder);
	BYTE ReadUSBSN(BYTE *pSN, WORD len);
	BYTE WriteUSBSN(BYTE *pSN, WORD len);
	BYTE GetVendorProductString (BYTE *VendorStr, BYTE *ProductStr);
	BYTE GetVidPid(WORD *Vid, WORD *Pid);
	BYTE SetVidPid(WORD Vid, WORD Pid);
	BYTE DoCalibration(BYTE Type, BYTE Mode, WORD Resolution);
	BYTE TestMotor(BYTE Motor, BYTE Dir, DWORD Step, DWORD PPS, BYTE *pStatus);
	BYTE DownloadFW(BYTE *pData, WORD size);
	BYTE GoHome(DWORD Option, DWORD *pStatus);
	BYTE SetAutoOffTime(int minutes);//change byte to int
	BYTE GetAutoOffTime(int *minutes);//change byte to int
	BYTE Test(WORD func1, WORD func2, WORD func3, WORD *pStatus);
	BYTE DisableAutoOffTimer(BYTE Disable);
	BYTE ReadLastCalibration(WORD *pYear, WORD *pMonth, WORD *pDay, DWORD *pPage);
	BYTE WriteLastCalibration(WORD Year, WORD Month, WORD Day, DWORD Page);
	BYTE GetScalingValue(float *pMagCorrectionX);
//=====new function
	BYTE SetIOHandle (HANDLE dwDevice, WORD wType);
	BYTE GetScannerAbility(LPHWCAPABILITY pHWCaps);
	BYTE ReadScanEX(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount,BOOL *EndFlag);
	BYTE ReadScanEX_raw_sim(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount,BOOL *EndFlag);
	BYTE ReadScanEX_raw_dup(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount,BOOL *EndFlag);
	BYTE ADFIsReady(void);
	BYTE SetADFOptions(BYTE Mode,BYTE flowtype);
	BYTE WriteNVRAM (ULONG ulStartLocation, BYTE * pbData, ULONG ulNumBytes);
	BYTE ReadNVRAM (ULONG ulStartLocation, BYTE * pbData, ULONG ulNumBytes);
	int GetSourceRes(int Res);
	int PixelAlignment(int width,int Alignment);
	int PixelAlignment_org(int width,int Alignment);
	BYTE SetCompress(bool Enable);
	BYTE GetCompress();
	BYTE SetRGBMatrix(RGB_MATRIX *pMatrix);
	BYTE SetSleepAutoOffTime(unsigned int sleep, unsigned int auto_off);
	BOOL GetScannerStatus (DWORD *ErrorCode);
	BYTE ButtonStatusGet(unsigned char *duplex, unsigned char *mode);
	void ReleaseImg_ResetVar();
	void ADF_ReleaseImg_ResetVar();
	BYTE ADFEndJob_();
	
	BYTE ErrorMapping_ADF();
	BYTE ErrorMapping_START();

//======Variable
	LTCSCANPARAMETER	ScanParam_LTC;
	float			ScanInch_W;
	IMGInfo			*ImageA;
	IMGInfo			*ImageB;
	IMGInfo			*Image_tmp;
	IMGInfo			*ImageIn;
	IMGInfo			*ImageOut;
	BYTE			byADFMode;
	BYTE		    byOpenRefCount;
	BYTE			byEndDoc;
	BYTE            *pDupImg;
	unsigned char   *jpeg_out_buf;
	unsigned char   *raw_out_buf;
	int				ReadSizeCountB;
	int				ImgBShift;
	SCANINFO		ScanInfo;
	DWORD			dwErrorCode;
	JpegResizeCB	jrcb;
	ResizeCB		rscb;

// ===== flag
	BOOL			ImgBEnd;
	BOOL			ImgBDocEnd;
	BOOL			ReadScan_EndFlag;
	BOOL			bADFOption;
	BOOL			bJobCreatFlag;
	BOOL			bStartScanFlag;
	BOOL			bGetSourceImg;
	BOOL			bCompress;
	BOOL			bScale;
	BOOL			bGamma;
	BOOL			bTonemap;
	BOOL			bColormatrix;
	BOOL			bOverScan;
	BOOL			bReadImg;
	BOOL			bImgAEnd;
	BOOL			bImgBEnd;
	BOOL			bCancel;
	BOOL			bBWmodeScan;
	BOOL            bFirstReadScanEX;
	BOOL			bflag_decode_done;
	BOOL			bStopTimeOutFlag;
	int             end_page[2];
	int				Source_end_page[2];
	int				page_line[2];
//======= image
	BYTE GammaTransLTCtoGL(WORD *pbyRed, WORD *pbyGreen, WORD *pbyBlue,unsigned int *GLGamma);
	BYTE SetGamma(DWORD uSz,WORD *pbyRed, WORD *pbyGreen, WORD *pbyBlue);
	BYTE ReadSourceImage();
	BYTE SendOutputImage(LPBYTE pStatus, LPBYTE pBuffer, DWORD Count,DWORD *RealCount,BOOL *EndFlag);
	BYTE InputImgTransfer_Scale(IMGInfo *Source,IMGInfo *Temp,SCANINFO Info);
	BYTE OutputImgTransfer(IMGInfo *Output,IMGInfo *Temp,SCANINFO Info);
	BYTE GrayImgToBWImg(IMGInfo *Source);
	BYTE gray2bw(int w, int h, unsigned char* gray, unsigned char* bw,int threshold);
	BYTE ReadSourceImage(IMGInfo *Source,DWORD Count,DWORD *RealCount,BOOL Duplex,BOOL *End,BOOL *DocEnd);
};
#define OVER_SCAN_DELTA_Y   0.2 //0.2 inch in 300dpi



#endif _LTCDriveApi