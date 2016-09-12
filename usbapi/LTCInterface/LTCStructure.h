//
// Structures.h:
//
//////////////////////////////////////////////////////////////////////
#define ERRCODE_None								0
#define ERRCODE_Cancel								15
#pragma once

typedef struct tagADFParameter
{
	DWORD StructSize;
	BYTE Connect;
	BYTE Status;
	BYTE AutoFeed;
	BYTE ADFPriority;
	BYTE Reserved[16]; 
} ADFPARAMETER;
typedef ADFPARAMETER FAR* LPADFPARAMETER;

typedef struct tagScannerAbilityEX
{
	DWORD StructSize;
	BYTE VendorName[12];           
	BYTE ModelName[20];            
	BYTE FirmwareVersion[8];     
	BYTE PortType[12];          
	BYTE PortAddress[8];        
	BYTE SensorType [12];       
	DWORD ScannerCapability;	
	WORD FlatbedMaxWidth;       
	WORD FlatbedMaxLength;  
	WORD ADFMinWidth;  
	WORD ADFMaxWidth;   
	WORD ADFMinLength;
	WORD ADFMaxLength;	        
	WORD OpticalRes;            
	WORD ADFOpticalRes;         
	WORD MaxGrayXRes;           
	WORD MaxGrayYRes;           
	WORD MaxColorXRes;          
	WORD MaxColorYRes;          
	WORD ImageType;             
	WORD ScanMethod;            
	BYTE  BitsPerChannel;       
	DWORD MaxOptResX;           
	DWORD MaxOptResY;           
	DWORD MaxCustomRes;         
	DWORD MinCustomRes;         
	DWORD ModelRes[128];        
	DWORD ADFModelRes[128];     
	float   MaxScanWidth;       
	float   MaxScanHeight;      
	WORD PreviewRes;            
	WORD ADFPreviewRes;         
	BYTE  ADFMode;              
	WORD   PreFeed;
	WORD   PostFeed;
	WORD FlatbedMinWidth;
	WORD FlatbedMinLength;
	BYTE Reserved[512]; //06/21/2010

} SCANNERABILITYEX;
typedef SCANNERABILITYEX FAR* LPSCANNERABILITYEX;

	

typedef struct tagScanParameter
{	
	DWORD StructSize;
	BYTE PreScanMode;
	WORD XRes;
	WORD YRes;
	WORD Left;
	WORD Top;
	WORD Width;
	WORD Length;
	WORD PixelNum; //(N/A)
	WORD LineNum; //(N/A)
	BYTE ScanMode ;
	BYTE ScanMethod;
	BYTE BitsPerPixel;
	BYTE ScanSpeed;     //(N/A)
	BYTE Contrast;      //(N/A)
	BYTE Brightness;    //(N/A)
	BYTE HTPatternNo;   //(N/A)
	BYTE Highlight;     //(N/A)
	BYTE Shadow;        //(N/A)
	BYTE ColorFilter;   //(N/A)
	BYTE Invert;        //(N/A)
	BYTE ReadStatus;    //(N/A)
	WORD ExtScanParam;  //(N/A)
	BYTE Threshold;     //(N/A)
	BYTE Option;
	BYTE RetryPreview;  //(N/A)
	BYTE UseAGData;
	BYTE ReduceMoire;   //(N/A)
	WORD PreFeed;      
	WORD PostFeed;
	BYTE  MultiOption;
	DWORD MultiFeedLength;			
	BYTE AutoADF;
	BYTE Background;
	DWORD MaxLength;
	DWORD PagesToScan;
	BYTE Reserved[128]; //06/21/2010
} LTCSCANPARAMETER;
typedef LTCSCANPARAMETER FAR* LPLTCSCANPARAMETER;


/*
typedef struct tagsButtonInfo
{
	DWORD StructSize;
	DWORD Ability1;
	WORD ButtonNum;
	WORD MapIndex;
	BYTE Reserved[16];
} BUTTONINFO;
typedef BUTTONINFO FAR* LPBUTTONINFO;
*/

typedef struct tagsReadButtonIndex
{
	DWORD StructSize;
	BYTE PressState;
	BYTE ButtonIndex[5];
	BYTE Reserved[10];
} READBUTTONINDEX;
typedef READBUTTONINDEX FAR* LPREADBUTTONINDEX;


//new


typedef struct 
{
	unsigned int	Formate;
	int				SourceW;
	int				SourceH;
	int				SourceXres;
	int				SourceYres;
	int				SourceLeft;
	int				SourceTop;
	int				OutputW;
	int				OutputH;
	int				OutputXres;
	int				OutputYres;
	int             Alignment;
	int				BitsPerPixel;
	int             Threshold;
	BOOL			bShading;
}SCANINFO;
typedef	struct {
	double dm11;
	double dm12;
	double dm13;
	double dm21;
	double dm22;
	double dm23;
	double dm31;
	double dm32;
	double dm33;
} MATRIX_RGB;


typedef struct {
	WORD			ScannerType;
	char			DrvSourceName[128];
	char			ProductName[128];
	char			VersionName[128];
//	char			VersionDate[128];
	char			VersionDate[126];	// SS: 2/22/02 Steal from this for CCDType & Reserve
	BYTE			CCDType;
	BYTE			Reserve;
	BYTE			ModeCaps;
	WORD			OpticalResol;
	WORD			ResolNum;
	WORD			*lpResolItem;
	DWORD			MaxWidth;
	DWORD			MinWidth;
	DWORD			MaxHeight;
	DWORD			MinHeight;
	DWORD			HWFuncCaps;
} HWCAPABILITY, *LPHWCAPABILITY;

typedef struct {
	DWORD			PixelLeft;
	DWORD			PixelTop;
	DWORD			PixelWidth;
	DWORD			PixelHeight;
	DWORD			BytesPerLine;
	WORD			ScanMode;
	WORD			ScanResol;
	BYTE			RedGamma[256];
	BYTE			GreenGamma[256];
	BYTE			BlueGamma[256];
/* --- Modified for Adjustable Exposure Ratio by Terry Lin in 11.23.2000 --- *
	BYTE			HalftonePat[64];
 * ------------------------------------------------------------------------- */
	WORD			RedExposureRatio;
	WORD			GrnExposureRatio;
	WORD			BluExposureRatio;
	BYTE			HalftonePat[58];		/// HalftonePat[64];
/* --------------------------------- END ----------------------------------- */
	BYTE			GrayChnel;
	DWORD			RGB_order;
	BYTE			ScanMethod;
}SCANPARAM, *LPSCANPARAM;

#define SCANNERTYPE_FLATBED				0x0001
#define SCANNERTYPE_ADFSCAN				0x0002


#define LEGO_GAMMA_ENTRIES_PER_CHANNEL	65536  // 16 x 16 tonemap.
#define LEGO_GAMMA_MAX_ENTRY_VALUE		65535 // 0 - 65535
#define LEGO_GAMMA_BYTES_PER_ENTRY		2		// WORD in length



#define ERRCODE_None								0
#define ERRCODE_ScannerNotFound						1
#define ERRCODE_MemoryNotEnough						2
#define ERRCODE_CarriageCanNotGoHome				3
#define ERRCODE_IllegalScanParameter				4
#define ERRCODE_ScannerDisconnect					5
#define ERRCODE_WhiteStripNotFound					6
#define ERRCODE_LampOff								7
#define ERRCODE_BeginLineNotFound					8
#define ERRCODE_BeginPixelNotFound					9
#define ERRCODE_CreateEventIsFailed					10
#define ERRCODE_CreateThreadIsFailed				11
#define ERRCODE_TimeOut								12
#define ERRCODE_PaperJam							13
#define ERRCODE_PaperTrayEmpty						14
#define	ERRCODE_Cancel								15
#define ERRCODE_LampNotStable						16
#define ERRCODE_Others								17
#define ERRCODE_WriteNVRAMError						18
#define	ERRCODE_HatchOpen							19
#define ERRCODE_HardwareError						20
#define ERRCODE_InvalidCommand						21
#define ERRCODE_OpenFileError						22
#define ERRCODE_LampFailure							23
#define ERRCODE_APFPaperJam							24
#define ERRCODE_ScannerLampFailed					25
#define ERRCODE_ADF_DUPLEX_PAGE_TOO_LONG			26
#define ERRCODE_DIAG_MOVE_FAILED					27
#define ERRCODE_DIAG_NO_ADF							28

#define	MODECAP_COLOR					0x01
#define	MODECAP_GRAYSCALE				0x02
#define MODECAP_LINEART					0x04