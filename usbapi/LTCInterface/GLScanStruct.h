#ifndef _GLScanStruct_h_
#define _GLScanStruct_h_
//#pragma pack(2)

#include "GLUtype.h"

typedef struct SC_JOB_STRUCT {
	U32	code;
	U8	request;
	U8	mode; //ADF = 'A', FLB = 'F'
	U8  reserved;
	U8	id;
} SC_JOB_T;
typedef struct SC_JOB_STA_STRUCT {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} SC_JOB_STA_T;

//---------2015/01/19 add

typedef struct SC_STATUS_STRUCT {
	U32	code;
	U8	request;
	U8  reserved[2];
	U8	id;
} SC_STATUS_T;

typedef struct SC_STATUS_DATA_STRUCT {
	U8 system;
	U8 sensor;
	U8 error;
} SC_STATUS_DATA_T;

//---------2015/01/19 end
//------------------------------
typedef struct SC_PAR_STRUCT {
	U32	code;
	U16	length;
	U8  reserved;
	U8	id;
} SC_PAR_T;
typedef struct SC_PAR_DATA_STRUCT {
	//- ACQUIRE --
	UINT32 source;	// 'ADF'/'FLB'/'POS'/'NEG', 'FW'/'PAR'/'HW', 'HOST', 'WIFI', 'ETH'
	UINT32 acquire;//bit1: Prefeed enable¡A bit2: 0:white 1:dark  scan¡A bit3: 0:normal scan 1:stillmode scan¡A  bit4: 0:continue calibration  1:calibration over autogohome = 1
	UINT16 acq_opt;// 0
	UINT8	duplex;	// 1:'A', 2:'B', 3:'D'
	UINT8	page;	// 0 for infinity pages 
	//- IMAGE ----
	UINT32 format;	// 'RAW', 'JPG', 'TIF', 'BMP', 'PDF', 'PNG'
	UINT16	img_opt;// 0
	UINT8	bit;	// 1:BW, 8:Gray8, 16:Gray16, 24:Color24, 48:Color48
	UINT8	mono;	// 0:'MONO', 1:'R', 2:'G', 4:'B', 8:'IR', 7:'NTSC'
	struct{UINT16 x; UINT16 y;} dpi;
	struct{UINT32 x; UINT32 y;} org;
	struct{UINT32 w; UINT32 h;} dot;
	//- shading ---
//	UINT16	AFE_OffsetCode[2][3];
//	UINT16	AFE_GainCode[2][3];
//	float AFE_OffsetValue[2][3];
//	float AFE_GainValue[2][3];
//	UINT32 shading_size;
} SC_PAR_DATA_T;

//#define ACQ_AUTO_SCAN		0x01
//#define ACQ_SHADING			0x02
//#define ACQ_GAMMA			0x04
//#define ACQ_MIRROR			0x80
//#define ACQ_START_HOME		(0x01 << 8)
//#define ACQ_BACK_TRACK		(0x02 << 8)
//#define ACQ_AUTO_GO_HOME	(0x04 << 8)
//#define ACQ_EJECT_PAPER		(0x10 << 8)
//#define ACQ_PICKUP_HOME		(0x20 << 8)
//
//#define ACQ_STILL_SCAN		(0x01 << 16)
//#define ACQ_TEST_PATTERN	(0x02 << 16)
//#define ACQ_RUNIN_IMAGE		(0x04 << 16)
//#define ACQ_RUNIN			(0x08 << 16)

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
#define ACQ_DUSTBKG			(0x01 << 11) //for Dust bkg Scan, don't need to pass to fw.

typedef struct SC_PAR_STA_STRUCT {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} SC_PAR_STA_T;
//---------------------------------
typedef struct SC_SCAN_STRUCT {
	U32	code;
	U8  reserved[3];
	U8	id;
} SC_SCAN_T;
typedef struct SC_SCAN_STA_STRUCT {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} SC_SCAN_STA_T;
//--------------------------------
typedef struct SC_INFO_STRUCT {
	U32	code;
	U8  reserved[3];
	U8	id;
} SC_INFO_T;
typedef struct SC_INFO_DATA_STRUCT {
	U32	code;
	U16 ValidPage[2];
	U32 ValidPageSize[2];
	U16 ImageWidth[2];
	U16 ImageLength[2];
	U8	PageNo[2];
	U8	EndPage[2];
	U8	EndDocument;
	U8	Cancel; 
	U8	Error;
	U8	reserved;
/*	U32	code;
	U16 ValidPage[2];
	U32 ValidPageSize[2];
	U16 ImageWidth[2];
	U16 ImageLength[2];
	U8	PageNo[2];
	U8	EndPage[2];
	U8	UltraSonic; U8 PaperJam; U8 CoverOpen; U8 Cancel; U8 key; 
	U8	EndDocument;
	U8	reserved[14+16];*/
} SC_INFO_DATA_T;
//-----------------------------
typedef struct SC_IMG_STRUCT {
	U32	code;
	U32 length: 24;
	U32 side:	8;
	//U8	length[3];
	//U8	side;
} SC_IMG_T;
typedef struct SC_IMG_STA_STRUCT {
	U32	code;
	U32 ack: 8;
	U32 length: 24;
	//U8	ack;
	//U8	length[2];
	//U8	err;
} SC_IMG_STA_T;
#define LENGTH
//-----------------------------
typedef struct SC_CNL_STRUCT {
	U32	code;
	U8	reserved[3];
	U8  id;
} SC_CNL_T;
typedef struct SC_CNL_STA_STRUCT {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} SC_CNL_STA_T;
//-----------------------------
typedef struct SC_STOP_STRUCT {
	U32 code;
	U8	reserved[3];
	U8	id;
}SC_STOP_T;
typedef struct SC_STOP_STA_STRUCT {
	U32 code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
}SC_STOP_STA_T;
//-----------------------------
#define OPT_STILL_IMG	0x0
// 0: 'SCAN', 'STIL': Still scan, 'MOVE': motor Move only, 'CALI': Calibration scan
// request
// type (b31~b24):		0=image, 1=still-image, 2=motor moving, 3=FW update, 4=table update
// request(b23~b16):	ShadingTable, bit22: Gamma, GammaTable, bit21: contrast, bit20: brightness, 
//						Auto_Level, Auto_Color, 
//------------------------------
typedef struct SC_AFE_STRUCT {
	U32	code;		//'AFE'
	U16	length;		// 36
	U8  zero[2];	// 0, 0
} SC_AFE_T;
typedef struct SC_AFE_DATA_STRUCT {
	U16 GainCode[2][3];
	U16 OffsetCode[2][3];
	U16 ShutterPixel[2][3];
} SC_AFE_DATA_T;

typedef struct SC_AFE_STA_STRUCT {
	U32	code;		// 'STA'
	U8	ack;		// 'A' or 'E'
	U8	zero[3];	// 0, 0, 0
} SC_AFE_STA_T;
//---------------------------------
typedef struct SC_SHAD_STRUCT {
	U32	code;		//'SHAD'
	U32 type: 8;	// 0:(default), 16:(16+16), 12:(12+4), 10:(10+6), 8:(8+8)
	U32 length: 24;	// 3 channel shading data size
} SC_SHAD_T;
typedef struct SC_SHAD_STA_STRUCT {
	U32	code;		// 'STA'
	U8	ack;		// 'A' or 'E'
	U8	zero[3];	// 0, 0, 0
} SC_SHAD_STA_T;
//------------------------------
typedef struct SC_MOTO_STRUCT {
	U32	code;		//'MOTO'
	U8	StepType;	// 0:(Full Step), 1:(Half Step), 2:(Quarter Step), 3:(1/8 step), 4:(1/16 step)
	U8	CurrentSel;	// 0, 1, 2, 3
	U8  Dir;		// 1: forward, 2: backward, 3: forward and backward
	U8	MotorID;	// 'F': flatbed, 'A': ADF, 'P': pickup
} SC_MOTO_T;
typedef struct SC_MOTO_DATA_STRUCT {
	U32 forward_step;	U32 backward_step;
} SC_MOTO_DATA_T;

typedef struct SC_MOTO_STA_STRUCT {
	U32	code;		// 'STA'
	U8	ack;		// 'A' or 'E'
	U8	zero[3];	// 0, 0, 0
} SC_MOTO_STA_T;
//---------------------------------

typedef struct REGBIT_STRUCT {
	U32 addr; U32 clear; U32 set;
} REGBIT_T;

typedef struct AFEREG_STRUCT {
	U16 addr; U16 data;
} AFEREG_T;
#endif
typedef struct NVRW_STRUCT {
	U32	code;  //'NVR', 'NVW'
	U8	straddr;
	U8	length;
	U8	reserved[2];
} S_NVRW;

typedef struct NVW_data_STRUCT {
	U8  *data;
} S_NVW_data;


typedef struct NVR_data_STRUCT {//can be change format, total size, etc...
	U8	*data;
} S_NVR_data;


typedef struct NVRW_status_STRUCT {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} S_NVRW_status;
typedef struct SC_ADF_CHECK_STRUCT {
	U32	code;
	U8	request;
	U8  reserved[2];
	U8	id;
} SC_ADF_CHECK_T;

typedef struct SC_ADF_CHECK_STA_STRUCT {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} SC_ADF_CHECK_STA_T;
typedef struct fw_version_ack_STRUCT {
	U32	code;  
	U8	reserved[4];
} fw_version_ack;


typedef struct fw_version_get_STRUCT {
	U32	version;
	U8  check;
	U8  length;
	U8	reserved[2];
} fw_version_get;

