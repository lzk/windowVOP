#ifndef _CMD_h_
#define _CMD_h_

#include "../ImgFile/ImgFile.h"
//-----------------------------
#pragma pack(1)

#define USB_PIPE 0

#define JOB_FLB			'F'
#define JOB_ADF			'A'

#define SCAN_FLB		I3('FLB')
#define SCAN_ADF		I3('ADF')
#define SCAN_A_SIDE		1
#define SCAN_B_SIDE		2
#define SCAN_AB_SIDE	3

// default scan parameters
#define SCAN_SOURCE   SCAN_ADF   //'FLB', 'ADF'
#define SCAN_ACQUIRE  ACQ_MULTI_FEED//(ACQ_PICK_SS|ACQ_MULTI_FEED)//ACQ_NO_SHADING
#define SCAN_OPTION   0
#define SCAN_DUPLEX   SCAN_AB_SIDE   // 1: A_side, 2: B_side, 3: AB_sides
#define SCAN_PAGE     0   // 0: adf sensor detect
#define IMG_FORMAT    IMG_FMT_JPG//I3('JPG')  //'JPG', 'RAW'
#define IMG_BIT       IMG_24_BIT      // 8, 16, 24, 48
#define IMG_MONO      IMG_COLOR
#define IMG_OPTION    IMG_OPT_JPG_FMT444
#define IMG_DPI_X     300
#define IMG_DPI_Y     300
#define IMG_ORG_X     0
#define IMG_ORG_Y     0
#define IMG_WIDTH     IMG_300_DOT_X   // 8.64"
#define IMG_HEIGHT    IMG_300_DOT_Y   // 12"


/*Motor drive port define*/
#define MT_PH	0
#define BMT_PH	1
#define CMT_PH	2

/*State mechine define*/
#define STATE_MECHINE_0		1
#define STATE_MECHINE_1		2
#define STATE_MECHINE_2		4
#define SCAN_STATE_MECHINE		8

#define MTR_DRIV_TAR	0
#define MTR_STAT_MEC	0
#define MTR_SPEED		0
#define MTR_ACC_STEP	0
#define MTR_DIRECT		0
#define MTR_MICRO_STEP  0
#define MTR_CURRENT		0

//---- MOTOR JOB TASK ----
#define JOB_SCAN				1
#define JOB_CALIBRATION			2
#define JOB_ULTRASONIC			3
#define JOB_FLASH_ACCESS		4

#define JOB_ADF_LOAD_PAPER	1 + 10
#define JOB_ADF_EJECT_PAPER	2 + 10
#define JOB_ADF_RESET_HOME	3 + 10
#define JOB_ADF_MOTOR_TEST	4 + 10
#define JOB_ADF_LIFE_TEST	5 + 10

#define JOB_FLB_LOAD_PAPER	1 + 20
#define JOB_FLB_EJECT_PAPER	2 + 20
#define JOB_FLB_RESET_HOME	3 + 20
#define JOB_FLB_LIFE_TEST	4 + 20

//Park test for Taiga
#define JOB_ADF_ROLL_TO_PRNU	1 + 30 
#define JOB_ADF_ROLL_TO_INIT	2 + 30
#define JOB_DUMMY			    (8 << 28)

//Scan error code
#define NO_ERR					0x00
#define ADF_NOT_READY_ERR		0x01
#define DOC_NOT_READY_ERR		0x02
#define HOME_NOT_READY_ERR		0x03
#define SCAN_JAM_ERR			0x04
#define COVER_OPEN_ERR			0x05

#define JOB_ID_ERR				0xff

//Job type define
#define JOB_PULL_SCAN			0x01
#define JOB_PULL_SCAN_BUTTON	0x02
#define JOB_WIFI_SCAN			0x03
#define JOB_PUSH_STORAGE		0x04
#define JOB_PUSH_FTP			0x05
#define JOB_PUSH_SMB			0x06


typedef struct MTR_STRUCT_ {
	U8 drive_target;
	U8 state_mechine;
	U8 direction;
	U8 micro_step;
	U8 currentLV;
	U8 padding1[3];
	U16 speed_pps;
	U16 acc_step;
	U16 pick_ss_step;
	U16 padding2;
} MTR_T_;

typedef struct ME_STRUCT {
  //--Mechanical--
  U16 leading_edge;
  U16 img_gap;
  U16 prefed;
  U16 postfed;
  S16 side_edgeA, side_edgeB;
}  ME_T; //12B


typedef struct ACQUIRE_STRUCT_ {
  U32 source;  // 'ADF'/'FLB'/'POS'/'NEG', 'FW'/'PAR'/'HW', 'HOST', 'WIFI', 'ETH'
  U32 action;
  U16 option;  // 0
  U8  duplex;  // 1:'A', 2:'B', 3:'D'
  U8  page;    // 0 for infinity pages 
  IMAGE_T img;  
} ACQUIRE_T_;

typedef struct SC_PAR_STRUCT_ {
  UINT32	source;   // 'ADF','FLB'
  UINT32	acquire;  // additional reqirement
  UINT16	option;	// 0
  UINT8		duplex;   // 1:side_A, 2:side_B, 3:side_AB
  UINT8		page;		// 0 for infinity pages 
  IMAGE_T	img;
  MTR_T_		mtr[2];
  ME_T		me;
} SC_PAR_T_;

//---------------------------------

typedef struct SC_PAR_STA_STRUCT_ {
	U32	code;
	U8	ack;
	U8	reserved[2];
	union {U8  id; U8 err;};
} SC_PAR_STA_T_;


typedef struct SC_GAMMA_STRUCT {
	U32	code;
	U16	length;
	U8  reserved;
	U8	id;
} SC_GAMMA_T;

//---------------------------------

#define ACQ_NO_MIRROR		(0x02)
#define ACQ_NO_SHADING		(0x04)

#define ACQ_GAMMA			(0x10)
#define ACQ_MULTI_FEED		(0x20)
#define ACQ_COLOR_REGISTER	(0x40)

#define ACQ_CROP_DESKEW		(0x01 << 8)
#define ACQ_OVER_SCAN		(0x02 << 8)

#define ACQ_AUTO_COLOR		(0x08 << 8)
#define ACQ_AUTO_LEVEL		(0x10 << 8)
#define ACQ_DETECT_COLOR	(0x20 << 8)
#define ACQ_DETECT_BLANK	(0x40 << 8)

#define ACQ_MOTOR_OFF		(0x01 << 16)    // scan without moving motor
#define ACQ_NO_PP_SENSOR	(0x02 << 16)    // ADF scan without Doc/ADF sensor detection, Flatbed scan without home sensor detection
#define ACQ_LAMP_OFF		(0x04 << 16)    
#define ACQ_TEST_PATTERN	(0x08 << 16)    // test pattern image

#define ACQ_CALIBRATION		(0x20 << 16)  //Park test for calibration
#define ACQ_SET_MTR			(0x40 << 16)  //Park test for set motor par from console
#define ACQ_SET_ME			(0x80 << 16)  //Park test for set ME par from console

#define ACQ_LIFE_TEST		(0x01 << 24)  //Park life test
#define ACQ_PICK_SS			(0x02 << 24)  //Park pick ss test


//--------------------------------
typedef struct SC_INFO_STRUCT_ {
  UINT32 code;
  UINT16 PageNum[2];
  UINT32 ValidPageSize[2];
  UINT16 ImageWidth[2];
  UINT16 ImageHeight[2];
  //UINT8  EndPage[2];
  //UINT8  EndScan[2];

    struct {
		UINT16 EndPage: 1;
		UINT16 EndScan: 1;
		UINT16 IsColor: 1;
		UINT16 IsBlank: 1;
		UINT16 reserved: 12;
	} ImgStatus[2];

	struct {
		UINT32 cover_open_err: 1;
		UINT32 scan_jam_err: 1;
		UINT32 scan_canceled_err: 1;
		UINT32 scan_timeout_err: 1;
		UINT32 multi_feed_err: 1;
		UINT32 usb_transfer_err: 1;
		UINT32 wifi_transfer_err: 1;
		UINT32 usb_disk_transfer_err: 1;
		UINT32 ftp_transfer_err: 1;
		UINT32 smb_transfer_err: 1;
		UINT32 reserved: 22;
	} ErrorStatus;

	struct {
		UINT32 adf_document_sensor: 1;
		UINT32 fb_home_sensor: 1;
		UINT32 adf_paper_sensor: 1;
		UINT32 cover_sensor: 1;
		UINT32 reserved: 28;
	} SensorStatus;

	struct {
		UINT32 scanning: 1;
		UINT32 sleep_mode: 1;
		UINT32 reserved: 30;
	} SystemStatus;

  UINT8  JobID;

  UINT8  reserved[7];
} SC_INFO_T_;

/*sensor parameter*/
typedef struct SENSOR_CHK_STRUCT_ {
	struct {
		U32 doc:1;		//DOC
		U32 scan:1;		//SCAN
		U32 cover:1;	//COVER
		U32 home:1;		//HOME
		U32 scan2:1;	//SCAN2
		U32 deskew:1;	//DESKEW
		U32 lid:1;		//LID
		U32 x1:1;		//PAPER_X_SIZE_ENCODE1
		U32 x2:1;		//PAPER_X_SIZE_ENCODE2
		U32 x3:1;		//PAPER_X_SIZE_ENCODE3
		U32 y2:1;		//PAPER_Y_SIZE_2
		U32 y3:1;		//PAPER_Y_SIZE_3
		U32 reserved:20;
	} val;
}SENSOR_CHK_T_;


/****Calibration****/
typedef struct CCD_CAP_STRUCT_ {
  U32  type; 
  U16  dpi, dot;
  U32  lperiod, exp_max, exp_def, exp_min;
  U8   mono, line_diff, line_stag, mirror;
} CCD_CAP_T_; // 28 Byte

typedef struct AFE_CAP_STRUCT_ {
  S16 offset_max, offset_def, offset_min;
  U16 gain_max, gain_def, gain_min;
} AFE_CAP_T_; // 16 Byte

typedef struct ME_CAP_STRUCT_ {
	U16 prefeed, postfeed;
} ME_CAP_T_; // 8 Byte

/*
typedef struct CALIBRATION_CAP_STRUCT {
  U32 id;             // I4('CDAT')
  CCD_CAP_T_ ccd[2];   // 28x2
  AFE_CAP_T_ afe[2];   // 12x2
} CALIBRATION_CAP_T; // 84 Byte
*/

typedef struct CALIBRATION_CAP_STRUCT_ {
  U32 id;             // I4('CDAT')
  CCD_CAP_T_ ccd[2];   // 28x2
  AFE_CAP_T_ afe[2];   // 12x2
  ME_CAP_T_ me;		  // 8
} CALIBRATION_CAP_T_; // 92 Byte

typedef struct CCD_SET_STRUCT_ {
  U32 lperiod;
  U32 exp[3];
} CCD_SET_T_;  // 16B

typedef struct AFE_SET_STRUCT {
  S16 offset[6];
  U16 gain[6];
} AFE_SET_T;  // 24B

typedef struct SHD_SET_STRUCT {
  U8  mono, gain_base;  // mono: same as the image.h definition; 0:'RGB', 1:'R', 2:'G', 3:'B', 4:'IR', 7:'NTSC', 8:'MONO'
  U8  dark_shift, dark_digit;
} SHD_SET_T;  // 4B

typedef struct ME_SET_STRUCT {
	U16 prefeed, postfeed;
} ME_SET_T; // 4 Byte

/*
typedef struct CALIBRATION_SET_STRUCT {
  CCD_SET_T ccd[2];
  AFE_SET_T afe[2];
  SHD_SET_T shd[2];
} CALIBRATION_SET_T;  // 88B
*/

typedef struct CALIBRATION_SET_STRUCT_ {
  CCD_SET_T_ ccd[2];
  AFE_SET_T afe[2];
  SHD_SET_T shd[2];
  ME_SET_T me;
} CALIBRATION_SET_T_;  // 92B

typedef struct FLASH_ME_STRUCT {
  U16 prefeed, postfeed;
} FLASH_ME_T; // 4B

/*
typedef struct CALIBRATION_FLASH_STRUCT {
  U8 str[12];
  CALIBRATION_CAP_T cap;  // 84B
  CALIBRATION_SET_T set;  // 88B
} CALIBRATION_FLASH_T; // 84+88+12=184B
*/
typedef struct FLASH_SHD_STRUCT {
  U8  source;     // Adf, Flb, Pos, Neg..
  U8  duplex;     // duplex = 1 or 3
  U8  bit;        // bit=16 or 48
  U8  bank_num;   // 1 or 3 or 6
  U32 bank_size;
  U16 dot;
  U16 dpi;
  U32 exp[2][3];
  S16 offset[2][6];
  U16 gain[2][6];
  U16 sector_idx;
  U16 sector_num;  // 4KB/sector
} FLASH_SHD_T; // 88B

/*
typedef struct FLASH_SHD_STRUCT {
  U8  source;     // Adf, Flb, Pos, Neg..
  U8  duplex;     // duplex = 1 or 3
  U8  bit;        // bit=16 or 48
  U8  bank_num;   // 1 or 3 or 6
  U32 bank_size;
  U16 dot;
  U16 dpi;
  U32 exp[2][3];
  S16 offset[2][6];
  U16 gain[2][6];
  U16 prefeed;
  U16 postfeed;
  U32 reserve;
  U16 sector_idx;
  U16 sector_num;  // 4KB/sector
} FLASH_SHD_T; // 92B
*/
/****Scan flow command****/
int _JobCreate(char job);
int _JobEnd();
int _parameters(SC_PAR_T_ *par);
int _StartScan();
int _stop();
int _info(SC_INFO_T_ *info);
int _cancel();
int _imgRead(int dup, U8 *buf, int *length);
void _show_err_msg(char *title, int err_code);

#pragma pack()

#endif//_CMD_h_

