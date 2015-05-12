#ifndef		_DEVMODE2_H_
#define		_DEVMODE2_H_

#pragma pack(8)

#define	DPI_600		600

#define		STILL_IMAGE_70	0
#if	_WIN32
#define		DM_DRIVERVERSION_40		0x0500
#else
#define		DM_DRIVERVERSION_40		0x0400
#endif

#define		PIXEL_TO_MM10(x)		(int)(((double)x*254/(DPI_600*10)+0.5))//(int)(((DWORD)x*254/(DPI_600*10)))
#define		MM100_TO_PIXEL(x)		(int)(((double)x*DPI_600/254+0.5))
#if FLAG_1200DPI
#define		PIXEL_TO_MM100(x, Resolution)		(int)(((double)x*254/Resolution)+0.5)//(int)(((DWORD)x*254/DPI_600))
#define		MMToPaperSize(mm, Resolution)  ((int)((double)(mm)/254*DPI_600+0.5))
#else
#define		PIXEL_TO_MM100(x, Resolution)		(int)(((double)x*254/DPI_600)+0.5)//(int)(((DWORD)x*254/DPI_600))
#define		MMToPaperSize(mm, Resolution)   ((int)((double)(mm)/254*DPI_600+0.5))
#endif

#define		DEV_NAME_LEN			32
#define		PAPER_STR_LEN			64
//#define		MAX_DEVMODE_SAVE		20
#define		MAX_DEFAULT_NUM			5

#define CPAPER_NAME_MAX_20 20

#ifdef XR6020
#define XEROX
#endif

#ifdef XR6025
#define XEROX
#endif

typedef struct _NTCUSTOMDATA {
	short	Index;
	TCHAR	Name[CPAPER_NAME_MAX_20+1];
	long	Width;  //300 dpi
	long	Height;  //300 dpi
	short	Modify; //0; cannot be modified; 1: can be modified
	short	Type;  //0 : ; 1: Inch
	HGLOBAL handle;
}  NTCUSTOMDATA, FAR *LPNTCUSTOMDATA;

typedef struct _CUSTOMPAPERDATA {
	char	Name[30];
	char	Width[8];
	char	Height[8];
	WORD	type;  /* 0 : MM ; 1: Inch */
	HGLOBAL handle;
}  CUSTOMPAPERDATA, FAR  *LPCUSTOMPAPERDATA;

#define		PAPERSTRLEN_64 64

#define DMPAPER_INDEX_CARD			119 /* PRC Envelope #10 Rotated 3 x 5 in */
#define DMPAPER_POSTCARD			120 /* PRC Envelope #10 Rotated 4 x 6 in */
#define DMPAPER_A6_CARD			    121 /* PRC Envelope #10 Rotated 105 x 148 mm */
#define DMPAPER_HAGAKI				122 /* PRC Envelope #10 Rotated 100 x 148 mm */
#define DMPAPER_CUSTOM_PAPER_SIZE   123
#define DMPAPER_ENVELOPE_6          124
#define DMPAPER_ENVELOPE_9			125
#define DMPAPER_ENVELOPE_10			126
#define	DMPAPER_ENVELOPE_C6			129	
#define	DMPAPER_ENVELOPE_B5			130	
#define	DMPAPER_ENVELOPE_7			131	
#define	DMPAPER_ENVELOPE_A2			132	
#define	DMPAPER_ENVELOPE_CHOKEI3	133		
#define	DMPAPER_ENVELOPE_CHOKEI4	134	
#define	DMPAPER_ENVELOPE_CHOKEI40	135
#define	DMPAPER_ENVELOPE_KAKUGATA3	136	
#define	DMPAPER_ENVELOPE_KAKUGATA4	137	
#define	DMPAPER_ENVELOPE_KAKUGATA5	138               
#define	DMPAPER_ENVELOPE_KAKUGATA6	139	
#define DMPAPER_BANNER_LETTER	    140
#define DMPAPER_BANNER_A4			141
#define DMPAPER_A1                  142
#define DMPAPER_11X15               143
#define DMPAPER_YOUKEI2		        144
#define DMPAPER_YOUKEI3		        145
#define DMPAPER_A5LEF				146
#define DMPAPER_B6LEF				147
#define	DMPAPER_START_257	257
#define DMPAPER_USER1	  257
#define DMPAPER_USER2	  258
#define DMPAPER_USER3	  259
#define DMPAPER_USER4	  260
#define DMPAPER_USER5	  261
#define DMPAPER_USER6	  262
#define DMPAPER_USER7	  263
#define DMPAPER_USER8	  264
#define DMPAPER_USER9	  265
#define DMPAPER_USER10	  266

#define DMPAPER_CUSTOM_MIN DMPAPER_USER1
#define DMPAPER_CUSTOM_MAX DMPAPER_USER10

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

typedef struct{
	short dmPaperSize;
	short dmPaperLength;
	short dmPaperWidth;
	short FtoPaper;
}WM_INFO;

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
	MARGIN_OFFSET  marginsOffset[49];//38
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
/*
typedef struct _GRAPHIC_DATA {//Booklet/Poster/Mixed Document/Rotation
	BYTE	printMode;
	BYTE	ApplyItem;	//0:selected 1;all
	BYTE	bApplyTo;		//apply index 0:text, 1: graphic 3:photo
	short	brightness[4];	//-100~100
	short	contrast[4];	//-100~100
}GRAPHIC_DATA, FAR * LPGRAPHIC_DATA;*/
typedef struct _GRAPHIC_DATA {//Booklet/Poster/Mixed Document/Rotation

			BYTE		printMode;
           BOOL          OutputRGN;                                          //TRUE if Output recognition is checked.

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

typedef struct _HEADER_INFO{
	int  nPos;			//0 top, 1 bottom
	int	 fontHeight;		//string height;
	int	 fontWidth;
}HEADER_INFO, FAR *LPHEADER_INFO;

typedef struct _WMARK_INFO{
	SIZE frame;
	int x, y;
	int originX;
	int originY;
	int borderWidth;
	int borderHeight;
	int nInfactBorderWidth;
}WMARK_INFO, FAR *LPWMARK_INFO;

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
//	BOOL			bAdmin;
} DEVMODEPRIVATE, FAR *LPDEVMODEPRIVATE;

typedef struct {
	DEVMODE			dmPublic; 		  /* standard Device Mode structure		*/
	DEVMODEPRIVATE	dmPrivate;
} PCLDEVMODE, FAR *LPPCLDEVMODE;

typedef struct PirntSettingsData
{
	short m_paperSize;
	short m_paperOrientation;
	short m_mediaType;
	short m_paperOrder;
	short m_printQuality;
	short m_scalingType;
	short m_scalingRatio;
	short m_nupNum;
	short m_typeofPB;
	short m_posterType;
	short m_ADJColorBalance;
	short m_colorBalanceTo;
	int m_densityValue;
	short m_duplexPrint;
	short m_documentStyle;
	short m_reversePrint;
	short m_tonerSaving;
	short m_copies;
	short m_booklet;

} PirntSettingsData, *pPirntSettingsData;

#define PCLDM_PRIV_SIZE (sizeof(PCLDEVMODE) - sizeof(DEVMODE))
#endif	// _DEVMODE2_H_

