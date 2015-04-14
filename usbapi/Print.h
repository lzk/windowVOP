
#pragma pack(1)

typedef struct _NUPDLG{
	DWORD	bNupFlags;
	BYTE	bNupNum;
	BYTE	bNupPageOrder;
}NUPDLG, FAR * LPNUPDLG;

#define DMPAPER_A5LEF 146
#define DMPAPER_B6LEF 147

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



#define DM_SIGNATURE 0x56495250

typedef struct _POSTER_DLG{
	WORD	 wPosterType;	      // ID_2x2:0, ID_3x3:1, ID_4x4:2
	BOOL     bCropMarks	;						//1:On,0:off	
}POSTER_DLG, FAR * LPPOSTER_DLG;

typedef struct _COLORADJ_DLG{
	int		wBrightness;	
	int		wContrast;		
	int		wSaturation;	
	int		wRedStrength;	
	int		wGreenStrength; 
	int		wBlueStrength;  
	int		wGamma;//add by Sunny 2003-12-30
}COLORADJ_DLG, FAR * LPCOLORADJ_DLG;

typedef struct {
   WORD		 index;
   TCHAR	 strName[MAX_PATH+1];
   TCHAR	 strText[MAX_PATH+1];
   COLORREF  colorRef;
   COLORREF  CustomColor;
   BYTE		 colorIndex;
   TCHAR	 typeFace[32];
   short	 fontSize;					     // User defined font size
   long      fontWidth;
   short	 vert;
   short	 hori;
   short	 postion;
   short	 angle;
   BYTE		 isImage;						//image flag
   BYTE		 canModify;		 			         // 0: can't modify   1: can modify
   BYTE      charSet;
   BYTE 	 firstPageOnly;			//not be used
   BYTE		 italic;
   BYTE		 bold;
   BYTE	     transparent;  
   BYTE		 wBorder; //0:no Border,1:Circle Border,2:Square Border
   BOOL		 wRepeat;//0:don't repeat,1:repeat
   TCHAR     tcFileName[MAX_PATH];  // Watermark file (mono PDL)
   TCHAR     LtcFileName[MAX_PATH];  // Watermark file (mono PDL)
   TCHAR     tcColorFile[MAX_PATH]; // Color watermark file (color PDL)
   TCHAR     LtcColorFile[MAX_PATH]; // Color watermark file (color PDL)
}WMARK_DATA, FAR *LPWMARK_DATA;


typedef struct {
	short xOffset;  	/* printing offset in x direction */
	short yOffset;  	/* printing offset in y direction */
	short xrOffset;     /* printing right offset in x direction */
	short ybOffset;     /* printing bottom offset in y direction */
}MARGIN_OFFSET, *PMARGIN_OFFSET ,FAR * LPMARGIN_OFFSET;

typedef struct 
{	//Margin Shift
	WORD shiftPosition;
	BYTE autoFit;
	WORD side1;
	WORD side2;

	//Print Position
	WORD printPositon;
	BYTE adjustPositon;
	int leftRightShift;
	int upDownShift;

	//Margins
	BYTE userMargin;
	WORD leftMargin;
	WORD upMargin;
	WORD RightMargin;
	WORD DownMargin;

#ifdef SWIFT
	MARGIN_OFFSET  marginsOffset[49];
#else
#ifdef XEROX
	MARGIN_OFFSET  marginsOffset[31];
#else
	MARGIN_OFFSET  marginsOffset[38];
#endif
#endif
}MARGIN_DATA, FAR *LPMARGIN_DATA;

typedef struct _OPTIONS_DATA{
	BYTE item;
	BYTE tray;
	BYTE select[6];
}OPTION_DATA, *POPTION_DATA;

typedef struct _BOOKLET_DATA	{	
			BOOL BookletSetting;//0 for left bind/top bind.1 for Right/bottom bind.
			BYTE Subset;//0 fo no,1 for auto,2 for custom
			BYTE Sheets;//1-15
			BOOL Gutter;//0 :no Gutter,1 have Gutter.
			BOOL Autofit;
			int  Margin;//0-50 mm or 0.0-1.9 inch.
			BYTE Unit;//inches :1,mm :0.
}BOOKLET_DATA,FAR LPBOOKLET_DATA;

typedef struct _BPMR_DATA {//Booklet/Poster/Mixed Document/Rotation
	BYTE	TypeofPB;						// 0 off,1 Poster,2 Booklet Creation
	BYTE	ImgRotate;
	BYTE  Mixorientation;
}BPMR_DATA, FAR * LPBPMR_DATA;

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



typedef struct _TRAY_DATA{
	BYTE tray1;	//0 not availiable 1:  250 feed 2: 500 feed
	WORD PaperSRC;
//	BOOL isInsert;//Insert Paper between Transparencies
//	BYTE PrintOnSep;//Print On Separators
//	BYTE FeedSepFrom;//Feed Separators from :BYTE
	BYTE trayOrient;	//0: landscape, 1: portrait
	BYTE bEnalbeFeed;	//0: disable 1:enable
}TRAY_DATA,FAR *LPTRAY_DATA;

typedef struct _PRINT_ITEMS{
	BOOL	print;		//0: off 1: print
	BYTE	position;	//0: top left, 1: top center 2: top right 
						//3: bottom left 4: bottom center 5: bottom right
}PRINT_ITEMS, FAR *LPPRINT_ITEMS;

#define PRINT_ITEMS_MAX	6

typedef struct _DEVICEFONT_DATA{
	TCHAR		strSoftFontName[256];
	short		sUseFont;	//0 softfont 1: device font
	TCHAR		strDeviceFontName[256];
}DEVICEFONT, *LPDEVICEFONT;

typedef struct _FONT_DATA{
	LPDEVICEFONT	lpDeviceFont;
	DWORD			dwFont;
}FONT_DATA, *LPFONT_DATA;

typedef struct _HEADER_FOOTER_DATA{
	TCHAR	typeface[512];	
	LONG	fontSize;	
	COLORREF colorref;	 //0: red, 1: orange, 2: yellow 3: green, 4: blue 5: purple, 6:black
						//default: 6:black
	BOOL	bold;		
	BOOL	italic;
	PRINT_ITEMS	printItems[PRINT_ITEMS_MAX];
	int	index;
	int bEnable; //yunying shang for lenovo
}HEADER_DATA, FAR *LPHEADER_DATA;
typedef struct _GRAPHIC_DATA {//Booklet/Poster/Mixed Document/Rotation

			BYTE		printMode;
           BOOL          OutputRGN;               //TRUE if Output recognition is checked.

                                                                                                //FALSE otherwise.                                                   
		   BYTE bImgType[3];		//index for different image adjustment mode
           BYTE ImgADJ;                                                 //This specifies image adjust mode:

                                                                                                //0:     Recommended Text&Photo

                                                                                                //1:     Recommended Photo

                                                                                                //2:     Recommended Graphics

                                                                                                //3:     ICM - Vividness

                                                                                                //4:     ICM - Contrast

                                                                                                //5:     ICM - Colorimetric

                                                                                                //6:     CMS

                                                                                                //7:     Complementary Color Conversion

           //For Image Setting

           BYTE  ApplyItem;                                                     //Apply to All Elements/Apply to Selected Element

                                                                                                           //TRUE for all element ;False select element(TEXT,Graphic,Photo)         
		   BYTE	 bApplyTo;			//0:text 1: text 2:graphi
           signed char  DocElementIndex[4][3];                                                   

                                                                                                //DocElementIndex[Element][IMGPROC]

                                                                                                //Where       Element = 0 for    text

                                                                                                //                   Element = 1 for    Graphic

                                                                                                //                   Element = 2 for    Photo

                                                                                                //                   Element = 3 for    All Element

                                                                                                //                   IMGPROC = 0 for Brightness

                                                                                                //                   IMGPROC = 1 for Contrast

                                                                                                //                   IMGPROC = 2 for Chorma

           //For Color Balance

           BOOL  isADJColorBalance;            //TRUE if this option is checked. 

                                                                                                //FALSE otherwise.

 
			BYTE	bColorBalanceTo;			//0:black 1:cyan 2:magenta 3:yellow
           signed char ColorBalanceIndex[4][3];//ColorBalanceIndex[TARGET_COLOR][DENSITY]

                                                                                                //Where       TARGET_COLOR = 0 for         Black

                                                                                                //                   TARGET_COLOR = 1 for         Cyan

                                                                                                //                   TARGET_COLOR = 2 for         Magenta

                                                                                                //                   TARGET_COLOR = 3 for         Yellow

                                                                                                //                   DENSITY = 0 for Low density

                                                                                                //                   DENSITY = 1 for medium density

                                                                                                //                   DENSITY = 2 for high density

            

 

           //Porfile Setting

 

           BYTE  Profile;                                                           //0:No profile,

                                                                                                     //1:Color Temperature/Gama Correction 

                                                                                                            //2:ICC Profile

           BYTE  ColorTemperature;                                     //0:5000k,

                                                                                                            //1:6500k,

                                                                                                            //2:9300k

           BYTE  GamaCorrection;                                        //0;1.0;

                                                                                                            //1:1.4;

                                                                                                            //2:1.8,

                                                                                                            //3:2.2,

                                                                                                            //4:2.6

           //ICC Profile:

           char Monitor[256];
		   BYTE imgIndex;	
           char InputImg[256];   
		   BYTE bEnhance;
		   BYTE bScreen; // 0: Auto, 1: Normal, 2: Fineness, 3: Gradation
		   BYTE	TonerSaving;
		   BYTE  barcodeMode;
}GRAPHIC_DATA, FAR * LPGRAPHIC_DATA;

#define JOBOWNER_MAX_LEN  33
typedef struct _CONFIG_DATA{
	short	sAccountMode;		
	short	sJobOwnerMode[2];
	TCHAR	strLoginName[2][MAX_PATH];
	TCHAR	strJobOwnerName[2][33];
	TCHAR	strSpecifyName[2][33];
	short	sRestrict;
	short	sEnableColorTrack;
	/*
	BYTE	UserMode;	//0:Adm,1:user
	BYTE	JobOwnerKind;	//0:using log name.1:Specify
	TCHAR	JobOwner[JOBOWNER_MAX_LEN];	//specify user name
	TCHAR	specifyName[JOBOWNER_MAX_LEN];
	BOOL	restrict;	//0:all,1:ADM
	TCHAR	strLoginName[MAX_PATH];
	*/
}CONFIG_DATA,FAR *LPCONFIG_DATA;

typedef struct _NODE_INFO {
	char		ip[80];
	char		hostname[48];
}NODE_INFO, FAR * LPNODE_INFO;

typedef struct SHAREDWIZDATA {
	BOOL	specify;				//0: broad cast 1:specify IP address
	BOOL	specifyBroad;			//0: not specify 1:specify broadcast ip address
	BOOL	tcpip;					//1:tcpip 0:ipx
	char	ipAddr[25];				//default "255.255.255.255"
	char	ip6Addr[129];			//ipv6 address
	char	community[129];			//community name
	int		prnNum;					//the number of printers
	NODE_INFO nodeInfo[32];			//the result found
	BOOL	isEnableIP6;			//0: IP6 disable 1: ip6 enable
	BOOL	useIP6;					//0: use IP4, 1: use IP6
}SHAREDWIZDATA, FAR *LPSHAREDWIZDATA;


#define DEVMODENAMELEN 49
#define MAX_DEVMODE_SAVE  25

typedef struct _DEVMODEPRIVATE
{
	// For Basic Tab data
	short			sEnableColorTrack;
	short			sPaperMismatchError; //0: disable; 1: enabled
	short			dmColor;
	short			sImprovePrint;		
	short			sSkipBlankPage;
	short			sPrintTextAsBlack;
	BYTE		    bDocumentStyle;				// 0 : 1-Sided, 1: 2-Sided(Book), 2-Sided(Tablet)
	BYTE			bEnableBooklet;				//0: not print booklet, 1: print booklet
	BYTE 			firstPageOnly;				//add by yunying for dcs 946
	BYTE			bEnableWM;				//0 : disable watermark 1: enable watermark
	BYTE			bPaperReverseOrder;		
	short			prtIndex;				// index DEVNAME in string table prtIndex>MAX_DEVMODE_SAVE can't change;
	DWORD			dwJobId;
	DWORD			dmSignature;				// private data id 	
	TCHAR			dataName[DEVMODENAMELEN+1];	
	NUPDLG		    nup;
    POSTER_DLG      poster;
	MARGIN_DATA     margins;
	SCAL_DLG		sfp;
	PAPER_DLG		par;						//paper tab
	CPAPER_DLG		cpd;						//custom paper size	
	BOOKLET_DATA    booklet;
	BPMR_DATA		bpmrdata;					//Booklet/Poster/Mixed Document/Rotation
	TRAY_DATA		tray;	
	OVERLAY_DATA	ovy;
	GRAPHIC_DATA    graphics;
	WMARK_DATA		waterMarkData;				//watermark data
	HEADER_DATA		headerData;					//header dialog data
	CONFIG_DATA     config;					//configuration 
	SHAREDWIZDATA	wizdata;
	OPTION_DATA		options;
	JOB_HANDLE_DLG  job;
	
    // For engineer usage
	DEBUG_DLG		dbg_dlg;
	PERFORM_DLG		perf_dlg;	
	BYTE			PrintQuality;	//0: 600dpi, 1: 1200dpi
	BYTE			byReverse[1024];
} DEVMODEPRIVATE, FAR *LPDEVMODEPRIVATE;

typedef struct {
	DEVMODE			dmPublic; 		  /* standard Device Mode structure		*/
	DEVMODEPRIVATE	dmPrivate;
} PCLDEVMODE, FAR *LPPCLDEVMODE;
