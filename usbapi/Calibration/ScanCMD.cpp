#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include "usbio.h"
#include "ScanCMD.h"

int BatchNum = 0;

void _show_err_msg(char *title, int err_code)
{
  printf("%s - error(0x%02x)\n", title, err_code);
  switch(err_code) {
	  case JOB_ID_ERR:
		  printf("JOB_ID_ERR\n");
		  break;

	  case ADF_NOT_READY_ERR:
		  printf("ADF_NOT_READY_ERR\n");
		  break;
	  case DOC_NOT_READY_ERR:
		  printf("DOC_NOT_READY_ERR\n");
		  break;
	  case HOME_NOT_READY_ERR:
		  printf("HOME_NOT_READY_ERR\n");
		  break;
	  case SCAN_JAM_ERR:
		  printf("SCAN_JAM_ERR\n");
		  break;
	  case COVER_OPEN_ERR:
		  printf("COVER_OPEN_ERR\n");
		  break;

  }
}


int _JobCreate(char job)
{
  int JobID = 0;
  U8 cmd[8] = {'J','O','B',0,job,0,0,0};
  U8 status[8];

  cmd[7] = JOB_PULL_SCAN;

  if(CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
     CMDIO_BulkReadEx(0, status, sizeof(status)) &&
	 (M32(&status[0])==I3('STA')) && (status[4]=='A')) {
     return TRUE;
  }
  else {
    printf("JOB create fail!!");
	return FALSE;
  }
}


//-----------------------------------------------
int _JobEnd()
{
  int result;
  U8 cmd[8] = {'J','O','B',0,'E',0,0,0};
  U8 status[8];

  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkReadEx(0, status, sizeof(status)) &&
          (M32(&status[0])==I3('STA')) && (status[4]=='A');
  if(result) {
  }
  else {
    printf("Job end fail!!");
  }

  return result;
}
//-----------------------------------------------
int _parameters(SC_PAR_T_ *par)
{
  int result;
  U8 cmd[8]= {'P','A','R',0, 0,0,0,0};
  U8 status[8];

  M16(&cmd[4]) = sizeof(SC_PAR_T_);

  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkWriteEx(0, par, sizeof(SC_PAR_T_)) &&
          CMDIO_BulkReadEx(0, status, sizeof(status)) &&
          (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

//----------------------------------------------
int _MotorParameters(int JobID, int code, MTR_T_ *par)
{
  int result;
  U8 cmd[8]= {'P','A','R',0, 0,0,0,(U8)JobID};
  U8 status[8];

  M16(&cmd[4]) = sizeof(MTR_T_);

  cmd[6] = code;

  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkWriteEx(0, par, sizeof(MTR_T_)) &&
          CMDIO_BulkReadEx(0, status, sizeof(status)) &&
          (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

//-----------------------------------------------
int _StartScan()
{
  int result;
  U8 cmd[8]= {'S','C','A','N', 0,0,0,0};
  U8 status[8];
  BatchNum++;
  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkReadEx(0, status, sizeof(status)) &&
          (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}
//-----------------------------------------------
int _stop()
{
  int result;
  U8 cmd[8]= {'S','T','O','P', 0,0,0,0};
  U8 status[8];
  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkReadEx(0, status, sizeof(status)) &&
          (M32(&status[0])==I3('STA')) && (status[4] =='A');
  return result;
}
//-----------------------------------------------
int _info(SC_INFO_T_ *info)
{
  int result;
  U8 cmd[8]= {'I','N','F','O', sizeof(SC_INFO_T_),0,0,0};
  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(0, info, sizeof(SC_INFO_T_)) &&
      (info->code == I4('IDAT'));
  return result;
}
//-----------------------------------------------
int _cancel()
{
  int result;
  U8 cmd[8]= {'C','A','N','C', 0,0,0,0};
  U8 status[8];
  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(0, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}
//-----------------------------------------------
int _imgRead(int dup, U8 *buf, int *length)
{
  int result;
  U8 cmd[8]= {'I','M','G',0, 0,0,0,0};
  //U8 status[8];

  M32(&cmd[4])= *length & 0xffffff;
  cmd[7] = (U8)dup;

  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkReadEx(0, buf, *length);
  //result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd));
  //      result = CMDIO_BulkReadEx(0, buf, *length);
  return result;
}
//-------------------------------------------------
int _buf(int dup, U8 *buf, int *length)
{
  int result;
  U8 cmd[8]= {'B','U','F',0, 0,0,0,0};
  //U8 status[8];

  M32(&cmd[4])= *length & 0xffffff;
  cmd[7] = (U8)dup;

  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkReadEx(0, buf, *length);
  return result;
}
//-------------------------------------------------
int _ResetScan(void)
{
  int result;
  U8 cmd[8]= {'R','S','E','T', 0,0,0,0};
  U8 status[8];

  result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
          CMDIO_BulkReadEx(0, status, sizeof(status)) &&
          (M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}

extern int GammaTransLTCtoGL(unsigned int *pbyRed, unsigned int *pbyGreen, unsigned int *pbyBlue, unsigned int *GLGamma);

int _gamma()
{
	int result;
	U32 size;
	U8 cmd[8]= {'G','A','M','A', 0,0,0,0}; 
	U8 status[8];
	SC_PAR_STA_T_	sc_gamma_status;
	int i, numread;
	unsigned int gGammaData[768];
	U32 up,down;
	double gamma=-1;
	unsigned int Red[65536];
	unsigned int Green[65536];
	unsigned int Blue[65536];
	unsigned int *pbyRed=Red;
	unsigned int *pbyGreen=Green;
	unsigned int *pbyBlue=Blue;

	
	//unsigned int *gGammaData;	
	for(i=0;i<65536;i++){
		Red[i]=(unsigned int)(65536-i); 
		Green[i]=(unsigned int)(65536-i) ;
        Blue[i]=(unsigned int)(65536-i) ;
	}

	GammaTransLTCtoGL(pbyRed,pbyGreen,pbyBlue,gGammaData);

	 M16(&cmd[4]) = sizeof(gGammaData);

	result = CMDIO_BulkWriteEx(0, cmd, sizeof(cmd)) &&
			CMDIO_BulkWriteEx(0, gGammaData, sizeof(gGammaData)) &&
			CMDIO_BulkReadEx(0, status, sizeof(status)) &&
			(M32(&status[0])==I3('STA')) && (status[4]=='A');
  return result;
}


