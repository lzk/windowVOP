//B001 yunying 2006-04-28	for dcs 1194 word app bug, if UI setting change, and directly change the devmode public, the ap is diordered, 
//so to process the specifal case. First judge the ap, second if word, not setting public devmode.  
#ifndef		_DM2OEM_H_
#define		_DM2OEM_H_

#pragma pack(8)

#define UI_VERSION	"1, 0, 2, 8"

#ifdef GRANDE2
#define		ABOUT_VERSION	" 0.001 (2010/06/10) "
#endif

#ifdef MANGO
#define		ABOUT_VERSION	" 1.0.1.8 "
#define		OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Dell\\Form")
#define		OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Dell")
#define		INSTALL_DIR					TEXT("\\Dell\\Config\\")
#endif

#ifdef CM215B
#define		ABOUT_VERSION	" 1.0.0.6 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Fuji Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Fuji Xerox")
#define		INSTALL_DIR					TEXT("\\Fuji Xerox\\Config\\")
#endif

#ifdef XR6020
#define		ABOUT_VERSION	"1.0.1.7 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Xerox")
#define	INSTALL_DIR					TEXT("\\Xerox\\Config\\")
#define FAVORITE_COMPANY		TEXT("\\Xerox")
#endif

#ifdef XR6025
#define		ABOUT_VERSION	"1.0.1.8 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Xerox")
#define	INSTALL_DIR					TEXT("\\Xerox\\Config\\")
#define FAVORITE_COMPANY		TEXT("\\Xerox")
#endif

#ifdef CP115W
#define		ABOUT_VERSION	"1.0.1.5 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Fuji Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Fuji Xerox")
#define		INSTALL_DIR					TEXT("\\Fuji Xerox\\Config\\")
#define FAVORITE_COMPANY		TEXT("\\Fuji Xerox")
#endif

#ifdef CM115W
#define		ABOUT_VERSION	"1.0.1.9 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Fuji Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Fuji Xerox")
#define		INSTALL_DIR					TEXT("\\Fuji Xerox\\Config\\")
#define FAVORITE_COMPANY		TEXT("\\Fuji Xerox")
#endif

#ifdef LENOVOABC3IN1
#define		ABOUT_VERSION	"1.0.1.0"
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Lenovo\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Lenovo")
#define		INSTALL_DIR					TEXT("\\Lenovo\\Config\\")
#define FAVORITE_COMPANY		TEXT("\\Lenovo")
#endif

#ifdef LENOVOP001
#define		ABOUT_VERSION	"1.0.1.1"
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Lenovo\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Lenovo")
#define		INSTALL_DIR					TEXT("\\Lenovo\\Config\\")
#define FAVORITE_COMPANY		TEXT("\\Lenovo")
#endif

#ifdef CM215F
#define		ABOUT_VERSION	" 1.0.0.9 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Fuji Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Fuji Xerox")
#define		INSTALL_DIR					TEXT("\\Fuji Xerox\\Config\\")
#endif

#ifdef CM205F
#define		ABOUT_VERSION	" 0.0.0.1 "
#define	OVERLAY_DEFAULT_FOLDER		(LPTSTR)TEXT("\\Fuji Xerox\\Form")
#define	OVERLAY_DEFAULT_FOLDER1		(LPTSTR)TEXT("\\Fuji Xerox")
#define		INSTALL_DIR					TEXT("\\Fuji Xerox\\Config\\")
#endif

#ifdef SWIFT_P105B
#define		ABOUT_VERSION	" 0.018 (2010/08/20) "
#endif

#ifdef SWIFT_P205B
#define		ABOUT_VERSION	" 0.018 (2010/08/20) "
#endif

#ifdef SWIFT_M105B
#define		ABOUT_VERSION	" 0.018 (2010/08/20) "
#endif

#ifdef SWIFT_M205B
#define		ABOUT_VERSION	" 0.018 (2010/08/20) "
#endif

#ifdef SWIFT_M105F
#define		ABOUT_VERSION	" 0.013 (2010/08/03) "
#endif

#ifdef SWIFT_M205F
#define		ABOUT_VERSION	" 0.013 (2010/08/03) "
#endif

#ifdef GRANDE2_31
#define		ABOUT_VERSION	" 0.003 (2010/08/16) "
#endif

#ifdef GRANDE2_41
#define		ABOUT_VERSION	" 0.003 (2010/08/16) "
#endif

#define		ABOUT_DATE		"May 25 2009"

typedef struct _COMPILER_CHECK{
	WORD	dwDummy;
	BYTE	byDummy;
} COMPILER_CHECK;

/*printer quality*/
#define DMQUALITY_QUICK	  0
#define DMQUALITY_NORMAL  1
#define DMQUALITY_BETTER  2
#define DMQUALITY_BEST    3

//==========Added by Margaret=====================

//N-up:ndg
#define		NUPORDER_UPL2R_0	0
#define		NUPORDER_LEFTU2D_1	1
#define		NUPORDER_UPR2L_2	2
#define		NUPORDER_RIGHTU2D_3	3

#define		NUPORDER_DOWNL2R_4	4
#define		NUPORDER_LEFTD2U_5	5
#define		NUPORDER_RIGHTD2U_6	6
#define		NUPORDER_DOWNR2L_7	7

#define		NUPF2_ACTIVATE				0
#define		NUPF2_BORDER_3D				1
#define		NUPF2_BORDER_LOGPAGEHP		2
#define		NUPF2_BORDER_PHYSPAGE		3

//add by Sunny 2003/01/02=>
typedef struct _NUPDLG{
	DWORD	bNupFlags;
	BYTE	bNupNum;
	BYTE	bNupPageOrder;
}NUPDLG, FAR * LPNUPDLG;


//Advance:adv
#define		TTRASTER_AUTO			0	// UI: must match with screen position
#define		TTRASTER_ASOUTLINE		1
#define		TTRASTER_ASBMP			2
typedef struct _ADVANCE_DLG{
 	BYTE	bAdvanceTrueType;			// 0: Auto   1:Outline  2:Bitmap	
	int		nAdvancePrintMode;  		// We do *NOT* modify dmPublic.dmColor
}ADVANCE_DLG, FAR * LPADVANCE_DLG;


//scaling:sfp
#define		ISF_DISABLE		0
#define		ISF_SCALING		1
#define		ISF_FITTOPAPER	2

typedef struct _SCAL_DLG{
   BYTE		ISFSet;
   short	SRatio;
   short	FtoPaper;
   BYTE		byFullPage;
}SCAL_DLG, FAR * LPSCAL_DLG;

//Overlays:ovy
#define		OVERLAY_DEFAULT_FILENAME	(LPTSTR)TEXT("form.dfd")

#define		OVERLAY_NONE_0		0
#define		OVERLAY_PRINT_1		1
#define		OVERLAY_CREATE_2	2
#define		WM_CREATE_4			4
#define		WM_PRINT_5			5
typedef struct _OVERLAY_DLG{
	BYTE	bOverlay;	// 0:Off	1:Print Overlay	 2:Create Overlay
	TCHAR	folder[256];
	TCHAR	OverlayFile[256];	
}OVERLAY_DATA, FAR * LPOVERLAY_DATA;

//Job Handling:job
#define		JOBHANDLE_OFF_0				0
#define		JOBHANDLE_PROOF_1			1
#define		JOBHANDLE_CONFIDENTIAL_2	2
#define		JOBHANDLE_SECURITY_3		3
typedef struct _JOB_HANDLE_DLG{
	BYTE	byJobHandle;		//0: off 1: Proof 2: Confidential 3: Security
	TCHAR	cPin[11];		//Pin number
}JOB_HANDLE_DLG, FAR * LPJOB_HANDLE_DLG;

//Paper:par
  #define		MEDIATYPE_PLAIN_0			0
  #define		MEDIATYPE_PLAIN_THICK_1		1
  #define		MEDIATYPE_COVERS_2			2
  #define		MEDIATYPE_COATED_3			3
  #define		MEDIATYPE_ENVELOPE_4		4
  #define		MEDIATYPE_LABEL_5			5
  #define		MEDIATYPE_RECYCLED_6		6
  #define		MEDIATYPE_PLAIN_SIDE2_7		7
  #define		MEDIATYPE_PLAIN_THICK_SIDE2_8			8
  #define		MEDIATYPE_COVERS_SIDE2_9	9
  #define		MEDIATYPE_COATED_SIDE2_10	10
  #define		MEDIATYPE_RECYCLED_SIDE2_11	11
  #define		MEDIATYPE_GLOSSY_12			12
  #define		MEDIATYPE_TRANSPARENCY_13	13
  #define		MEDIATYPE_IRONON_14			14
  #define		MEDIATYPE_CARDSTOCK_15		15  
  #define		MEDIATYPE_PLAIN_16			16
  #define		MEDIATYPE_ENVELOPE_17		17
  #define		MEDIATYPE_BANNER_18			18
  #define		MEDIATYPE_COATEDPAPER_19	19
  #define		MEDIATYPE_GLOSSY_20			20
  #define		MEDIATYPE_TRANSPARENCY_21	21
  #define		MEDIATYPE_IRONON_22			22
  #define		MEDIATYPE_CARDSTOCK_23		23  

typedef struct _PAPER_DLG{
	short   dmPaperSize;		//save current paper size;
	BYTE	dmOrientation;		//B001 save current UI orientation;//add by yunying 2006-04-28 for dcs 1194
	WORD	wMediaType;		    // 0: Plain  1: Envelope 2: Banner 3: Coated paper
			                    // 4. Photo/Glossy 5. Transparency 6. Iron-on Transfer
			                    // 7. Card Stock
							
}PAPER_DLG, FAR * LPPAPER_DLG;

//Different Paper Source:dif
#define	PAPERSOURCE_GENERAL_0		0
#define	PAPERSOURCE_DIFFIRST_1		1
#define	PAPERSOURCE_DIFOTHER_2		2
typedef struct _DIFF_DLG{
	int		nDiffPSource;		//0: General 1: Diff first page 2: Diff last page
								//3: Diff for special
	int		nAddPSource;		//the same as the paper source
	int		wFromPage;			//1~999
	int		wToPage;			//1~999
}DIFF_DLG, FAR * LPDIFF_DLG;

//Transparency:tra
typedef struct _TRANS_DLG{
	BYTE	bTransInsert;		// 0: off 1: on
	DWORD	dwPSource;		//paper source
}TRANS_DLG, FAR * LPTRANS_DLG;

// DEBUG_DLG:dbg_dlg
#define		DEBUGF2_PRINTTIME		0
#define		DEBUGF2_PRINTTEXT		1
#define		DEBUGF2_PRINTGRAPHICS	2
#define		DEBUGF2_PRINTIMAGE		3
#define		DEBUGF2_SHOWTEXTCLIP	4
#define		DEBUGF2_SHOWIMAGECLIP	5
#define		DEBUGF2_SHOWGXCLIP		6
#define		DEBUGF2_DUMPUISETTING	7
#define		DEBUGF2_DUMPTX			8
#define		DEBUGF2_DUMPERRMSG		9
#define		DEBUGF2_DUMPIMG			11

typedef struct _DEBUG_DLG
{
	DWORD	dwDbgFlags;
	WORD	wFontSize;
	WORD	wEditA;
	WORD	wEditB;
	TCHAR	UIDumpFile[_MAX_FNAME];
	TCHAR	TXDumpFolder[_MAX_FNAME];
}DEBUG_DLG, FAR *LPDEBUG_DLG;

// PERFORM_DLG::perf_dlg
#define		PERF2_CLRFONTCACHBYPAGE	0
#define		PERF2_REMOVECLIPS		1
#define		PERF2_COMPRESSIMAGE		2
#define		PERF2_STEALIMGOFF		3
#define		PERF2_STEALIMGFIXED		4
#define		PERF2_STEALIMGAUTO		5
#define		PERF2_STEALIMGDEBUG		6
#define		PERF2_LITEONFIRMWARE	7

typedef struct _PERFORM_DLG
{
	DWORD	dwPrmFlags;
	int		nImgScaleX;	// 1~100
	int		nImgScaleY;	// 1~100
	int		nThreshold;
} PERFORM_DLG, FAR *LPPERFORM_DLG;

//==========Added by Margaret=====================

//.typedef struct _PAPERSIZE{
//.	short dmPaperSize1;				// paper size
//.	short dmPaperLength1;			// paper length, overrides dmPaperSize
//.	short dmPaperWidth1;			 // paper width, overrides dmPaperSize
//.} PAPERSIZE, FAR *LPPAPERSIZE;

typedef struct _POSTERSIZE{
	int 	iIdsPosterSize;
}POSTERSIZE, FAR * LPPOSTERSIZE;

typedef struct _MEDIATYPE{
	int 	iIdsMediaType;	// Media Type ID
}MEDIATYPE, FAR * LPMEDIATYPE;

typedef struct _BOOKLET{
	int 	iIdsBooklet;
}BOOKLET, FAR * LPBOOKLET;

typedef struct _OUTFACE{
	int 	iIdsOutFace;
}OUTFACE, FAR * LPOUTFACE;

typedef struct _OUTTRAY{
	int 	iIdsOutTray;
}OUTTRAY, FAR * LPOUTTRAY;

typedef struct _DIFFPS{
	int 	iIdsDiffPs;
}DIFFPS, FAR * LPDIFFPS;

typedef struct _NUPSTR{
	int 	iIdsNups;
}NUPSTR, FAR * LPNUPSTR;

typedef struct _NUPORDER{
	int 	iIdsNupsPOrder;
}NUPORDER, FAR * LPNUPORDER;

typedef struct _HALFTONE{
	int 	iIdsHalftone;
}HALF_TONE, FAR * LPHALF_TONE;

typedef struct __COLORMAP{
	int 	iIdsColorMap;
}COLOR_MAP, FAR * LPCOLOR_MAP;

//#if	DPI_600 == 600
//	#define		CPAPER_OFFSET_150	150
//#else
	#define		CPAPER_OFFSET_150	MARGIN_VALUE
//#endif

// Albert test 5/20/03 ---->
#define		IMAGEAREA_X(x)		((x)->xPhys - (x)->xOffset - (x)->xrOffset)
#define		IMAGEAREA_Y(x)		((x)->yPhys - (x)->yOffset - (x)->ybOffset)
// <----

typedef struct _PAPERFORMAT{
	short xPhys;		/* physical paper width */
	short yPhys;		/* physical paper height */
	short xOffset;  	/* printing offset in x direction */
	short yOffset;  	/* printing offset in y direction */
	// Albert test 5/20/03 ---->
	short xrOffset;     /* printing right offset in x direction */
	short ybOffset;     /* printing bottom offset in y direction */
} PAPERFORMAT, *PPAPERFORMAT ,FAR * LPPAPERFORMAT;

typedef struct _DMPAPER
{
	short			dmPaperSize;	// DM value
	int 			iIdsPaperName;	// Paper String ID
	LPPAPERFORMAT   lpPortPaperMetric;
	LPPAPERFORMAT   lpLandPaperMetric;
} DMPAPER, FAR * LPDMPAPER;

typedef struct _CONFIG{
	int 	iIdsConfig;
}CONFIG, FAR * LPCONFIG;

#define NAME_LEN 32

#define GetTrayIDS(a)       ((a)->iIdsTrayName)
#define BINSTRLEN 24
typedef struct _TRAYDEFS
{
	short   bin;
	int		iIdsTrayName;   // ID of tray name string resource
} TRAYDEFS, FAR * LPTRAYDEFS;

typedef struct _MODELDATA
{
#define		MD_GetCaps(prtIndex)			ModelData[prtIndex].iCaps
#define		MD_GetCaps2(prtIndex)			ModelData[prtIndex].iCaps2
#define		MD_GetRomInd(prtIndex)			ModelData[prtIndex].iRomInd
#define		MD_GetRomCount(prtIndex)		ModelData[prtIndex].iRomCount
	int		 iCaps;		// Bit field of printer capabilities
	int		 iCaps2;		 // Bit field of printer capabilities
	int		 iRomInd;		// Index to first device font in list
	int		 iRomCount;	// Number of device fonts in list

	//Paper window
#define 	MD_GetNumPapers(prtIndex)			ModelData[prtIndex].nNoOfPaperSize
#define 	MD_GetPaperList(prtIndex)			ModelData[prtIndex].lpPaperSize
#define 	MD_GetNumOutputPapers(prtIndex)		ModelData[prtIndex].nNoOfOutputPaperSize
#define 	MD_GetOutputPaperList(prtIndex)		ModelData[prtIndex].lpOutputPaperSize
#define		MD_GetNumTrays(prtIndex)			ModelData[prtIndex].sNumTrays
#define 	MD_GetTrayList(prtIndex)			ModelData[prtIndex].lpTrayDefs
#define		MD_GetOutFaceList(prtIndex)			ModelData[prtIndex].lpOutFace
#define		MD_GetNumOutFaces(prtIndex)			ModelData[prtIndex].sNumOutFace
#define		MD_GetOutTrayList(prtIndex)			ModelData[prtIndex].lpOutTray
#define		MD_GetNumOutTray(prtIndex)			ModelData[prtIndex].sNumOutTray
	short		nNoOfPaperSize;	 // Number of papers in list
	LPDMPAPER	lpPaperSize;	// Pointer to Paper List
	short		nNoOfOutputPaperSize;	 // Number of output papers in list
	LPDMPAPER	lpOutputPaperSize;	// Pointer to output Paper List
	short		sNumTrays;	// Number of Paper Sources in list
	LPTRAYDEFS	lpTrayDefs;	 // Point to tray definitions
//.	short		sMaxCopies;	 // Maxium number of copies allowed on printer
	LPOUTFACE   lpOutFace;	//pointer to Output Face
	short		sNumOutFace;
	LPOUTTRAY	lpOutTray;	//pointer to Output Tray
	short		sNumOutTray;


	//Color Matching
#define		MD_ColorMatchList(prtIndex)			ModelData[prtIndex].lpColorMatch
#define		MD_GetNumColorMatch(prtIndex)		ModelData[prtIndex].sNumColorMatch
#define		MD_GetHalftoneList(prtIndex)		ModelData[prtIndex].lpHalftone
#define		MD_GetNumHalftone(prtIndex)			ModelData[prtIndex].sNumHalftone
	LPCOLOR_MAP	lpColorMatch;
	short		sNumColorMatch;
	LPHALF_TONE	lpHalftone;
	short		sNumHalftone;
	
	//main-N-up
#define		MD_GetNupList(prtIndex)				ModelData[prtIndex].lpNup
#define		MD_GetNumNup(prtIndex)				ModelData[prtIndex].sNumNup
#define		MD_GetNupPageOrderList(prtIndex)	ModelData[prtIndex].lpNupPageOrder
#define		MD_GetNumNupPageOrder(prtIndex)		ModelData[prtIndex].sNumPageOrder
	LPNUPSTR	lpNup;
	short		sNumNup;
	LPNUPORDER  lpNupPageOrder;	
	short		sNumPageOrder;	
	//main-poster
#define 	MD_GetPosterSizeList(prtIndex)		ModelData[prtIndex].lpPosterSize
#define 	MD_GetNumPosterSizeList(prtIndex)	ModelData[prtIndex].sPosterSize	
	LPPOSTERSIZE	lpPosterSize;
	short		    sPosterSize;
	//PaperSetting Window
#define		MD_GetBookletList(prtIndex)			ModelData[prtIndex].lpBooklet
#define		MD_GetNumBooklet(prtIndex)			ModelData[prtIndex].sNumBooklet
#define		MD_GetMediaTypeList(prtIndex)		ModelData[prtIndex].lpMediaType
#define		MD_GetNumDMediaType(prtIndex)		ModelData[prtIndex].sMediaType
	LPBOOKLET 	lpBooklet;		//pointer to booklet list
	short		sNumBooklet;	//Number of items in booklet list	
	LPMEDIATYPE	lpMediaType;	//pointer to Media Type
	short		sMediaType;		//Number of Media Type List

#define		MD_GetDiffList(prtIndex)			ModelData[prtIndex].lpDiffPs
#define		MD_GetNumDiffs(prtIndex)			ModelData[prtIndex].sNumDiffPs
	LPDIFFPS	lpDiffPs;		//pointer to different Pager source
	short		sNumDiffPs;
				
	//Quality window
//.	BYTE		bQualityFineArt;			// 0=Off   1=Light  2=Medium  3=Dark
//.#define 	MD_GetDefaultResolution(prtIndex)	ModelData[prtIndex].sQualityResolution
//.	short		sQualityResolution; // Resolution   600 | 300 dpi

	//Custom Paper
#define 	MD_GetCusMinExtent(prtIndex)		ModelData[prtIndex].pCusMinExtent
#define 	MD_GetCusMaxExtent(prtIndex)		ModelData[prtIndex].pCusMaxExtent
	POINT	pCusMinExtent;	 // Small physical paper size at 300dpi
	POINT	pCusMaxExtent;	 // Largest physical paper size at 300dpi
} MODELDATA, FAR * LPMODELDATA;

#define		WMARK_USRINDEX_5			5
#define		WMARK_ITEM_MAX_20 			20
#define 	WMARK_NAMELEN_30			30
#define 	WMARK_STRINGLEN_255			255
#define		WMARK_NTESC_GETSETTING			0x5001
#define		WMARK_NTESC_BEGIN				0x5002
#define		WMARK_NTESC_PRINT				0x5003
#define		WMARK_NTESC_END					0x5004
#define		HEADFOOTER_NTESC_GETSETTING		0x5005
#define		HEADFOOTER_NTESC_BEGIN			0x5006
#define		HEADFOOTER_NTESC_END			0x5007
#define		HEADFOOTER_NTESC_PRINT			0x5008
#define		NTESC_GETPAGENUM				0x5009
#define		NTESC_GETDOCUMENTNAME			0x500A
#define		CONFIG_NTESC_GETSETTING			0x500B
#define		ORIENT_NTESC_GETSETTING			0x500C
#define		PAPERSIZE_NTESC_GETSETTING		0x500D
#define		NUPNUMBER_NTESC_GETSETTING		0x500E
#define		OVERLAY_NTESC_GETSETTING		0x500F
#define		SYSTEM_DISK_STATUS				0x5010 //Devid added for not enough disk space.
#define		DUPLEX_MODE						0x5011 
#define		OVERLAY_MODE					0x5012 
#define		STARTPAGE						0x5013 //Allan 2013-10-19 added for BMS 40777
#define		PAGE_STATUS						0x5014 //Allan 2013-10-19 added for BMS 40777
#define		HFOVERLAY_NTESC_BEGIN			0x5016//add by yunying shang 2014-04-23 for BMS 48442 48450
#define		HFOVERLAY_NTESC_END				0x5017//add by yunying shang 2014-04-23 for BMS 48442 48450
#define		HFOVERLAY_NTESC_PRINT			0x5018//add by yunying shang 2014-04-23 for BMS 48442 48450
#define		SET_PROCESSOR_NAME				0X5019
#define		WMARK_NTESC_ENABLE				0x501A
// Model Data Structures
//.typedef DWORD COLORREF ;
#define EZ_ATTR_BOLD		1
#define EZ_ATTR_ITALIC		2
#define EZ_ATTR_UNDERLINE	 4
#define EZ_ATTR_STRIKEOUT	 8

#define JACK_INI_FILENAME	TEXT("\\falap.INI")

#define		VD_SCALE_25		25
#define		VD_SCALE_400	400

#define		VD_COPIES_1		1
#define		VD_COPIES_999	999		//modified by yunying 2004-12-07

#define		VD_WMARK_HORIZ_MIN	-30
#define		VD_WMARK_HORIZ_MAX	30
#define		VD_WMARK_VERT_MIN	-30
#define		VD_WMARK_VERT_MAX	30
#define		VD_WMARK_ANGLE_MIN	-90
#define		VD_WMARK_ANGLE_MAX	90

// for custom paper
#define		CPAPER_ITEM_NAME		TEXT("Name")
#define		CPAPER_ITEM_MODIFY		TEXT("Modify")
#define		CPAPER_ITEM_MITERTYPE	TEXT("MiterType")
/*
#if	_WIN32
	#define HELP_FILENAME   "\\3\\dlnaphhe.chm\0"
#else
	#define HELP_FILENAME   "\\3\\dlnaphhe.chm\0"
#endif*/


#define		CPAPER_SECTION			TEXT("CUSTOM PAPER SIZE")
#define		CPAPER_TOTALITEMS		TEXT("Total")

#define		CPAPER_SECTION_NAME		TEXT("CPS%d")
#define		CPAPER_ITEM_NAME		TEXT("Name")
#define		CPAPER_ITEM_MODIFY		TEXT("Modify")
#define		CPAPER_ITEM_MITERTYPE	TEXT("MiterType")
#define		CPAPER_ITEM_INWIDTH		TEXT("inWidth")
#define		CPAPER_ITEM_INHEIGHT	TEXT("inHeight")
#define		CPAPER_ITEM_MMWIDTH		TEXT("mmWidth")
#define		CPAPER_ITEM_MMHEIGHT	TEXT("mmHeight")

#define		TENTHINCHTOMM_10(x)		(int)((x*2.54+0.5))		
#define		TENTHINCHTOMM_100(x)	(int)((x*25.4)+0.5)
#define		_10_MMTOTENTHINCH(x)	(int)((x/2.54+0.5))		 

#define		CPAPERTYPE_INCH		1
#define		CPAPERTYPE_MM		0

#define		CPAPER_NAME_LEN		24
#define		CPAPER_NAME_MAX_20	20

#define		MARGIN_VALUE		99

typedef struct _CPAPERSIZE {
	int		width;
	int		height;
	int		cp_MiterType;  /* 0 : MM ; 1: Inch */
	TCHAR	cp_szName[CPAPER_NAME_LEN + 1];
	BOOL	cp_Modify;
	short	paperSizeID;
}  CPAPERSIZE, FAR * LPCPAPERSIZE;

typedef struct _COLORTRACK {
	int		nType;
	TCHAR	cPassWord[13];
}  COLORTRACK, FAR * LPCOLORTRACK;

typedef struct _CPAPER_DLG{
	WORD	cpd_Enabled;				// 0 for standard, 1 for custom
	int		cpd_CustomCount;			//user defined size number
	int		cpd_dmPaperSize;			// paper size
	int		cpd_dmPaperLength;			// paper length, overrides dmPaperSize
	int		cpd_dmPaperWidth;			// paper width, overrides dmPaperSize
	CPAPERSIZE		cCustomPaperSize[CPAPER_NAME_MAX_20];		//save user defined custom paper size
} CPAPER_DLG, FAR *LPCPAPER_DLG;

enum 
{
	PJL_RESOLUTION_600 = 0,
	PJL_RESOLUTION_1200 = 0
};
typedef struct _RESOLTBL
{
	LPTSTR	lpUiStr;
	LPSTR	lpPjlStr;
} RESOLTBL, *LPRESOLTBL;

typedef struct _VARC {
	LPTSTR lpszUnit;
	struct _RANGE
	{
		int		min;
		int		max;
	} range[2];
} VARC;

VOID	GetNupPos(
	BYTE	bNupPageOrder,
	BYTE	bOrient,
	BYTE	rotate180,
	WORD	wLogPageCount,
	int		Nup_xCount,
	int		Nup_yCount,
	LPINT	lpxPos,
	LPINT	lpyPos
);
int findFitPaperType(int paperSizeID);
int findPaperTypeID(UINT mediaStrId);
#endif	// _DM2OEM_H_
