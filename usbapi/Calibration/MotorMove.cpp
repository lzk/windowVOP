
#include <Windows.h>
#include <stdio.h>
#include <conio.h>
#include "usbio.h"
#include "ScanCMD.h"

extern SC_PAR_T_ k_scan_par;
extern U8 JobID;  // defined in scan.c

int job_Wait(int job, int wait_motor_stop);

//-------------------

int Motor_LoadPaper(U8 code, int length)
{
  int result;
  U8 cmd[8] = {'M','O','T','O', 0,0,code,JobID};
  U8 status[8];

  M16(&cmd[4]) = (U16)length;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}
//-------------------
int Motor_EjectPaper(U8 code, int length)
{
  int result;
  U8 cmd[8] = {'M','O','T','O', 0,0,code,JobID};
  U8 status[8];

  M16(&cmd[4]) = (U16)length;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}

//-------------------
int Motor_ResetHome(U8 code, int length)
{
  int result;
  U8 cmd[8] = {'M','O','T','O', 0,0,code,JobID};
  U8 status[8];

  M16(&cmd[4]) = (U16)length;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}

int Motor_RollerToPRNU(U8 code, int length)
{
  int result;
  U8 cmd[8] = {'M','O','T','O', 0,0,code,JobID};
  U8 status[8];

  M16(&cmd[4]) = (U16)length;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}

int Motor_RollerToINIT(U8 code, int length)
{
  int result;
  U8 cmd[8] = {'M','O','T','O', 0,0,code,JobID};
  U8 status[8];

  M16(&cmd[4]) = (U16)length;

  result = CMDIO_BulkWriteEx(USB_PIPE, cmd, sizeof(cmd)) &&
      CMDIO_BulkReadEx(USB_PIPE, status, sizeof(status)) &&
      (M32(&status[0])==I3('STA')) && (M8(&status[4])=='A');
  return result;
}


//-----------------------
int job_LoadPaper(U32 source, int length)
{
  U32 job = (source == I3('ADF'))? JOB_ADF_LOAD_PAPER: JOB_FLB_LOAD_PAPER;
  if(!Motor_LoadPaper(job, length))
    return FALSE;
  
  return TRUE;//job_Wait(job, 1);
}

int job_EjectPaper(U32 source, int length)
{
  U32 job = (source == I3('ADF'))? JOB_ADF_EJECT_PAPER: JOB_FLB_EJECT_PAPER;
  if(!Motor_EjectPaper(job, length))
    return FALSE;
  return TRUE;//job_Wait(job, 1);
}

int job_ResetHome(U32 source, int length)
{
  U32 job = (source == I3('ADF'))? JOB_ADF_RESET_HOME: JOB_FLB_RESET_HOME;
  if(!Motor_ResetHome(job, length))
    return FALSE;
  return TRUE;//job_Wait(job, 1);
}

int job_RollerToPRNU(int length)  //For Taiga
{
  U32 job = JOB_ADF_ROLL_TO_PRNU;
  if(!Motor_RollerToPRNU(job, length))
    return FALSE;

  Sleep(500); //Park test
  
  return TRUE;//job_Wait(job, 1);
}

int job_RollerToINIT(int length)  //For Taiga
{
  U32 job = JOB_ADF_ROLL_TO_INIT;
  if(!Motor_RollerToINIT(job, length))
    return FALSE;

  Sleep(500); //Park test
  
  return TRUE;//job_Wait(job, 1);
}

