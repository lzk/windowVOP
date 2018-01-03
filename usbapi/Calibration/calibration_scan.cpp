
#include <Windows.h>
#include <stdio.h>
#include <math.h>
#include <conio.h>
#include "usbio.h"
#include "ScanCMD.h"
#include "model.h"

extern int ImgFile_Close(IMG_FILE_T *imgfile, int lines);

SC_PAR_T_ k_scan_par;

#ifdef Taiga
	#define K_ADF_ROLLER  //For Taiga
#endif

//-------------------------
extern int ScanMenuItem[];
//-------------------------
U8 JobID = 0;
//--Scan-----------------------

static IMG_FILE_T ImgFile[2] = {{k_scan_par.img,0},{k_scan_par.img,0}};
static int bFiling[2] = {0, 0};
static SC_INFO_T_ Info;
static U8 ScanBuf[0x80000];  // 512MB(?) //Park:512KB	//For 存放 A or B 面一次 bulk 傳來的區段影像
static int ScanBufSize=sizeof(ScanBuf);
static char ImgFileName[64];

int K_BatchNum = 0;
int K_PageNum = 0;
int bSaveFile = 1;

int ini_get_paper_layout(int *x, int *y, int *width, int *height);

int Scan_EnableSaveFile(int enable);

int ini_get_jpeg_quality();
int ini_get_jpeg_page_mode();
int paper_layout(U32 paper, int dpi, int *pos, int *width, int *height);
U32 ini_get_paper();
int Scan_Info();
int Scan_OpenFile(int dup, char *filename);
int Scan_WriteFile(int dup, char*buf, int length);
int Scan_CloseFile(int dup, int lines, int width);
int cal_img_buf_store(int dup, void *img, int size);
int job_LoadPaper(U32 source, int length);
int job_EjectPaper(U32 source, int length);
int job_ResetHome(U32 source, int length);
int job_Calibration(SC_PAR_T_ *_par);



//--------------------
int Scan_OpenDevice()
{
  return CMDIO_OpenDevice();
}
//-------------------
int Scan_CloseDevice()
{
  return CMDIO_CloseDevice();
}
//-------------------
int Scan_JobCreate(char job)
{
  int result;
  U8 cmd[8] = {'J','O','B',0, job,0,0,0};
  U8 status[8];

  cmd[7] = JOB_PULL_SCAN;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  if(result) {
    
  }
  else {
    printf("Job create fail!!\n");
  }

  return result;
}

//-------------------
int Scan_JobEnd()
{
  int result;
  U8 cmd[8] = {'J','O','B',0, 'E',0,0,JobID};
  U8 status[8];

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  JobID = 0;

  if(!result)
    _show_err_msg("Job End", status[7]);

  return result;
}

int Scan_FW_Version()
{
  int result;
  U8 cmd[8] = {'C','A','P',0, 0,0,0,0x10|JobID};
  char ver_str[24];

  M32(&cmd[4]) |= sizeof(ver_str);

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, ver_str, sizeof(ver_str)) &&
      (M32(&ver_str[0])==I3('CDAT'));

//  if(result)
  //  MessageBox(NULL, &ver_str[4], "FW Version", MB_OK);

  return result;
}

void _get_flash_tag_string(U8 *tag_string)
{
  int duplex_char = (k_scan_par.duplex == 3)? 'D': ('A'+k_scan_par.duplex-1);
  IMAGE_T *img = &k_scan_par.img;
  
  sprintf((char*)tag_string, "%c%c%d%c", (U8)k_scan_par.source, (img->bit>=24)?'C':'G', img->dpi.x, duplex_char);
}

extern int Load_CalibrationParameter(SC_PAR_T_ *par);

U32 user_param(U32 action)
{
  k_scan_par.acquire = action;
  Load_CalibrationParameter(&k_scan_par);
  
  return TRUE;
}

int Scan_Param(void)
{
  int result;

  result = _parameters(&k_scan_par);

  return result;
}
//---------------

int Scan_Start()
{
  int result;
  U8 cmd[8]= {'S','C','A','N', 0,0,0,JobID};
  U8 status[8];
  K_PageNum++;
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  //OutputDebugString("Scan_Start()\n");
  return result;
}

int Scan_Stop()
{
  int result;
  U8 cmd[8]= {'S','T','O','P', 0,0,0,JobID};
  U8 status[8];
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4] =='A');
  return result;
}

int Scan_Cancel()
{
  int result;
  U8 cmd[8]= {'C','A','N','C', 0,0,0,JobID};
  U8 status[8];
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}

int Scan_Info()
{
  int result;
  U8 cmd[8]= {'I','N','F','O', sizeof(Info),0,0,0}; // JobID = 0

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, &Info, sizeof(Info)) &&
      (Info.code == I4('IDAT'));
      //&& !Info.Cancel;

  //printf("Info.ValidPageSize[0] = %d\n", Info.ValidPageSize[0]);
  //printf("Info.ValidPageSize[1] = %d\n", Info.ValidPageSize[1]);

  return result;
}

int Scan_Img(int dup, int *length)
{
  int result;
  U8 cmd[8]= {'I','M','G',0, 0,0,0,0};
  U8 status[8];

  M32(&cmd[4])= *length & 0xffffff;
  cmd[7] = (U8)dup;

  // Jason debug
  return CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd));
  
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  if(result)
    *length = M32(&status[4]) >> 8;
  return result;
}

int Scan_Read(void *buf, int length)
{
  int result = CMDIO_BulkReadEx(0, buf, length);
  return result;
}

#if 0
void _gamma(int x, int y, int gamma10, U16 *buf)
{
  int i;
  double g = (double)10/(double)gamma10;  // 1.0 / (gamma10 / 10.0)
  double k = (double)y / pow((double)x, g);
  U16 data = 0;

  for(i = 1; i <= x; i++) {
    *buf++ = data;
    data = (U16)(k * pow((double)i, g) + 0.5);  // round by 0.5
    *buf++ = data;
  }
  buf[-1] = (U16)(y-1);
}

int Scan_Gamma(int gamma)
{
  int result;
  U8 code = 0x04 + k_scan_par.duplex;
  U8 cmd[8]= {'P','A','R',0, 0,0,code,JobID};
  U8 status[8];
  U16 *buf = (U16*)ScanBuf;
  int i;
  U32 size = 2*512*3;  //3KB
  M16(&cmd[4]) = (U16)size;

  for(i = 0; i < 3; i++)
    _gamma(256, 0x10000, gamma, buf+512*i);

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, buf, size) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}
#endif

int Scan_ColorMatrix(int *color_matrix)
{
  int result;
  U8 code = 0x08 + k_scan_par.duplex;
  U8 cmd[8]= {'P','A','R',0, 0,0,code,JobID};
  U8 status[8];
  
  U32 size = 9 * sizeof(int);  // 36 Byte
  M16(&cmd[4]) = (U16)size;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, color_matrix, size) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

int Scan_Convolution(int *convolution_table)
{
  int result;
  U8 code = 0x0c + k_scan_par.duplex; 
  U8 cmd[8]= {'P','A','R',0, 0,0,code,JobID};
  U8 status[8];
  
  U32 size = 15 * sizeof(int);  // 60 Byte
  M16(&cmd[4]) = size;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, convolution_table, size) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

int Scan_Pwr_PowerOff(int time)
{
  int result;
  U8 cmd[8]= {'P','W','R',0, 0,0,1,JobID};
  U8 status[8];
  
  M16(&cmd[4]) = (U16)time;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

int Scan_Pwr_Sleep(int time)
{
  int result;
  U8 cmd[8]= {'P','W','R',0, 0,0,2,JobID};
  U8 status[8];
  
  M16(&cmd[4]) = (U16)time;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

int Scan_Pwr_Wakeup(int time)
{
  int result;
  U8 cmd[8]= {'P','W','R',0, 0,0,3,JobID};
  U8 status[8];
  
  M16(&cmd[4]) = (U16)time;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

/*To FW "ui_set_calibration()"*/
int Scan_Shad_Calibration(CALIBRATION_SET_T_ *set)
{
  int result;
  U8 code = 0x8 << 4;
  U8 cmd[8]= {'S','H','A','D', 0,0,0,code+JobID};
  U8 status[8];
  M32(&cmd[4]) += (sizeof(CALIBRATION_SET_T_) & 0x00ffffff);
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, set, sizeof(CALIBRATION_SET_T_)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}

/*To FW "ui_set_shading()"*/
int Scan_Shad_Shading(int side, int channel, void *buf, int length)
{
  int result;
  U8 code = ((side << 2) + channel) << 4;
  U8 cmd[8]= {'S','H','A','D', 0,0,0,code+JobID};
  U8 status[8];
  M32(&cmd[4]) += (length & 0x00ffffff);
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, buf, length) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}

/*To FW "ui_set_shading_flash()"*/
int Scan_Shad_Flash(void *buf, int length)
{
  int result;
  U8 code = 0xf << 4;
  U8 cmd[8]= {'S','H','A','D', 0,0,0,code+JobID};
  U8 status[8];
  M32(&cmd[4]) += (length & 0x00ffffff);
  cmd[7] = 0xf0 + (JobID & 0x0f);
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, buf, length) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
    return result;
}

/*To FW "ui_set_ME_flash()"*/
int Scan_ME_Flash(void *buf, int length)
{
  int result;
  U8 code = 0x9 << 4;
  U8 cmd[8]= {'S','H','A','D', 0,0,0,code+JobID};
  U8 status[8];
  M32(&cmd[4]) += (length & 0x00ffffff);
  //cmd[7] = 0xf0 + (JobID & 0x0f);
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkWriteEx(USB_PIPE, buf, length) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
    return result;
}

int Scan_Cap_Fw_Version(char *fw_ver_str)
{
  int result;
  U8 cmd[8]= {'C','A','P',0, 0,0,0x01,JobID};
  char cap_fw_ver[28] = {0};
  int size = sizeof(cap_fw_ver);
  M16(&cmd[4]) = (U16)size;
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, cap_fw_ver, size) &&
      (M32(cap_fw_ver) == I4('CDAT'));
  if(result) {
    result = strlen(&cap_fw_ver[4]);
    strcpy(fw_ver_str, &cap_fw_ver[4]);
  }

  return result;
}


int Scan_Cap_Calibration(CALIBRATION_CAP_T_ *cap)
{
  int result;
  U8 cmd[8]= {'C','A','P',0, 0,0,0x08,JobID};
  M16(&cmd[4]) = sizeof(CALIBRATION_CAP_T_);
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, cap, sizeof(CALIBRATION_CAP_T_)) &&
      (cap->id == I4('CDAT'));

  return result;
}

int Scan_Set_Calibration(CALIBRATION_CAP_T_ *cap)
{
  int result;
  U8 cmd[8]= {'C','A','P',0, 0,0,0x08,JobID};
  M16(&cmd[4]) = sizeof(CALIBRATION_CAP_T_);
  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, cap, sizeof(CALIBRATION_CAP_T_)) &&
      (cap->id == I4('CDAT'));

  return result;
}

/*
#define JOB_WAIT_TIMEOUT  5000
int job_Wait(int job, int wait_motor_stop)
{
  U32 tick = GetTickCount();
  while((GetTickCount() - tick) < JOB_WAIT_TIMEOUT) {
    if(!Scan_Info())
      break;
    if(!(Info.JobState & job) && (!wait_motor_stop || !Info.MotorMove))
      return TRUE;
    Sleep(100);
  }
  return FALSE;
}
*/

int _scan_start()
{
  ImgFile[0].stream = ImgFile[1].stream = 0;
  return Scan_Start();
}

int _scan_info()
{
  
  if(!Scan_Info())
    return -1;

  
  if((!(k_scan_par.duplex & 1) || Info.ImgStatus[0].EndScan) &&
    (!(k_scan_par.duplex & 2) || Info.ImgStatus[1].EndScan))
    return -1;
 
  return 1;
}

int _scan_image(void)
{
  ACQUIRE_T_ *acq = (ACQUIRE_T_*)&k_scan_par;
  IMAGE_T *img = &k_scan_par.img;
  int dup, length, PageStart, PageEnd;
  //ScanBufSize=((img->format==I3('JPG')) && !(img->option & IMG_OPT_JPG_PAGE))? sizeof(ScanBuf)/8: sizeof(ScanBuf);
  int lineSize = (img->format!=I3('JPG'))? ((img->bit * img->width + 7)/8): 0;
  ScanBufSize=sizeof(ScanBuf);
  for(dup = 0; dup < 2; dup++) {
    if(!(acq->duplex & (1<<dup)) || !Info.ValidPageSize[dup])
      continue;
    length = min(ScanBufSize, (int)Info.ValidPageSize[dup]);
    if(lineSize)
      length -= (length % lineSize);
    if(Scan_Img(dup, &length) && Scan_Read(ScanBuf, length)) {
      //cal_img_buf_store(dup, ScanBuf, length);  // Jason debug

      PageStart = 0, PageEnd = 0;
      if(!bFiling[dup]) {
        bFiling[dup]++;
        sprintf(ImgFileName, "%02d_%c%c%d_%02d%c.%s", K_BatchNum, (U8)acq->source, (img->bit>=24)?'C':'G', img->dpi.x, /*Info.PageNum[0]+1*/K_PageNum, 'A'+dup, img->format==I3('JPG')? "JPG":"TIF");
        Scan_OpenFile(dup, ImgFileName);
        PageStart++;
      }
      Scan_WriteFile(dup, (char*)ScanBuf, length);
      if((length >= (int)Info.ValidPageSize[dup]) && Info.ImgStatus[dup].EndPage) {
	    //printf("File close ");
		//printf("%c\n", dup? 'B': 'A');
        Scan_CloseFile(dup, Info.ImageHeight[dup], 0);
        PageEnd = Info.ImageHeight[dup];
        bFiling[dup]--;
      }
      
      cal_img_buf_store(dup, ScanBuf, length);  // Jason debug
    }
  }
  return TRUE;
}

int _scan_stop()
{
  if(ImgFile[0].stream)
    Scan_CloseFile(1, Info.ImageHeight[0], 0);
  if(ImgFile[1].stream)
    Scan_CloseFile(2, Info.ImageHeight[1], 0);
  return Scan_Stop();
}

int job_Scan(void)
{
  int data_ready;
  if(!_scan_start())
    goto EXIT;
LOOP:
  data_ready = _scan_info();
  if(data_ready < 0)
    goto EXIT;
  if(data_ready == 0)
    goto LOOP;
  _scan_image();
  goto LOOP;
EXIT:
  return _scan_stop();
}

int Scan_EnableSaveFile(int enable)
{
  int old_setting = bSaveFile;
  bSaveFile = enable? 1: 0;
  return old_setting;
}

int Scan_CloseFile(int dup, int lines, int width)
{
#ifdef K_ADF_ROLLER
	if(bSaveFile) {
		if( (k_scan_par.source == I3('FLB')) && (dup == 0) )
			return ImgFile_Close(&ImgFile[dup], lines);

		if( (k_scan_par.source == I3('ADF')) && (dup == 1) )
			return ImgFile_Close(&ImgFile[dup], lines);
	}
#else
	if(bSaveFile)
		return ImgFile_Close(&ImgFile[dup], lines);
#endif
	return TRUE;
}

int Scan_OpenFile(int dup, char *filename)
{
#ifdef K_ADF_ROLLER
	if(bSaveFile) {
		if( (k_scan_par.source == I3('FLB')) && (dup == 0) )
			return ImgFile_Open(&ImgFile[dup], filename);

		if( (k_scan_par.source == I3('ADF')) && (dup == 1) )
			return ImgFile_Open(&ImgFile[dup], filename);
	}
	
#else
	if(bSaveFile)
		return ImgFile_Open(&ImgFile[dup], filename);
#endif
	return TRUE;
}

int Scan_WriteFile(int dup, char*buf, int length)
{
#ifdef K_ADF_ROLLER
	if(bSaveFile) {
		if( (k_scan_par.source == I3('FLB')) && (dup == 0) )
			return ImgFile_Write(&ImgFile[dup], buf, length);

		if( (k_scan_par.source == I3('ADF')) && (dup == 1) )
			return ImgFile_Write(&ImgFile[dup], buf, length);
	}
#else
	if(bSaveFile)
		return ImgFile_Write(&ImgFile[dup], buf, length);
#endif
	return TRUE;
}

//-------------------------
