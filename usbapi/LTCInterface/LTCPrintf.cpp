#include "..\StdAfx.h"
#include <windows.h>
#include <time.h>
#include <stdio.h>
#include <stdarg.h>
#include <queue>
#include <deque>

#if defined(_WIN32) || defined(_WIN64) 
#pragma warning (disable : 4996)
#endif

#if _CANOPUS_DEBUG_
#define LOG_FILE_NAME "c:\\canopus_driver_log.txt"
#define LOG_FILE_CONFIG "c:\\canopus_driver_log_configure.txt"
int _ltc_log_level1=0; //normal debug
int _ltc_log_level2=0; //very detail debug
int _ltc_log_first_time_flag=1;

#ifdef LTCPrintf_ByThread
/*****************************************************************
	LTCPrinf put log in ram and the other thread write it to file.
******************************************************************/
#define LOG_BUF_SIZE 16384
typedef struct _QMSG
{
	char *str;
}QMSG;
char _ltc_log_buf[LOG_BUF_SIZE+128];
char *_ltc_log_buf_current=0;
typedef deque<QMSG*, allocator<QMSG*> > QMSG_DEQEUE;
typedef queue<QMSG*,QMSG_DEQEUE> QMSG_QUEUE;
HANDLE _ltc_Sem;
HANDLE _ltc_Thread;
QMSG_QUEUE _ltc_q;

//wait for fprintf thread finish, max=3sec
void LTCPrintfWaitLogThreadDone(void)
{
	int i=30;

	if(_ltc_log_level1) {
		while(i--){
			if(_ltc_q.size()==0) {
				break;
			}
			Sleep(100);
		}
	}
}

DWORD WINAPI LTCPrintfThreadProc( LPVOID lpParam )
{
	UNREFERENCED_PARAMETER(lpParam);
	QMSG *msg;
	FILE *fp;

	while(1) {
		WaitForSingleObject(_ltc_Sem, INFINITE); 
		msg = _ltc_q.front(); 
		_ltc_q.pop();

		fp = fopen(LOG_FILE_NAME, "a");
		if(fp) {
			fprintf(fp, "%s", msg->str);
			fclose(fp);
		}
		
		delete msg;
	}

	return TRUE;
}

void LTCPrintf_Thread_Init(void)
{
	DWORD ThreadID;

    _ltc_Sem = CreateSemaphore( 
        NULL,		// default security attributes
        0,			// initial count
        256,		// maximum count
        NULL);		// unnamed semaphore
    if (_ltc_Sem == NULL) 
    {
        printf("CreateSemaphore error: %d\n", GetLastError());
        //return 1;
    }

	_ltc_Thread = CreateThread( 
		NULL,       // default security attributes
		0,          // default stack size
		(LPTHREAD_START_ROUTINE) LTCPrintfThreadProc, 
		NULL,       // no thread function arguments
		0,          // default creation flags
		&ThreadID); // receive thread identifier

	if( _ltc_Thread == NULL )
    {
        printf("CreateThread error: %d\n", GetLastError());
        //return 1;
	}
	
	//To inital the log buffer pointer.
	_ltc_log_buf_current=_ltc_log_buf;
}
#endif

void LTCPrintf_Capability_Check(void)
{
	FILE *fp;
	char buf[64];

	if(_ltc_log_first_time_flag==0) {
		return;
	}
	else {
		_ltc_log_first_time_flag=0;
#ifdef  LTCPrintf_do_not_save_file 
		//don't check the log file, then it will not write to file.
		return;
#endif
	}

	fp = fopen(LOG_FILE_NAME, "r");
	if(fp) {
		_ltc_log_level1=1;
		fclose(fp);
#ifdef LTCPrintf_ByThread
		LTCPrintf_Thread_Init();
#endif
	}

	fp = fopen(LOG_FILE_CONFIG, "r");
	if(fp) {
		if(fgets (buf , 64 , fp) != NULL ) {
			if(strcmp(buf, "level2=1")==0){
				_ltc_log_level2=1;
			}
		}
		fclose(fp);
	}
}

void LTCPrintf(char* format, ...)
{
	SYSTEMTIME st;
	va_list arg;
	int tl=0;
#ifdef LTCPrintf_ByThread 
	char *buf;
#else
	char buf[192];
#endif
	
	LTCPrintf_Capability_Check();
#ifdef LTCPrintf_ByThread 
	buf = _ltc_log_buf_current;
#endif
	if(_ltc_log_level1==0) {
		va_start(arg, format);
		vsprintf(buf, format, arg);
		va_end(arg);
		printf("%s", buf);
		//cannot find this file.
		return;
	}
 
	GetLocalTime(&st);//GetSystemTime(&st);
	tl = sprintf(buf, "%d/%d/%d %d:%d:%d.%03d ", st.wMonth, st.wDay, st.wYear, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);

    va_start(arg, format);
    vsprintf(buf+tl, format, arg);
    va_end(arg);

	printf("%s", buf+tl);

#ifdef LTCPrintf_ByThread
	QMSG *msg = new QMSG;
	msg->str = buf;
	_ltc_q.push(msg);
	ReleaseSemaphore(_ltc_Sem, 1, NULL);
	
	_ltc_log_buf_current += strlen(buf)+1;
	if(_ltc_log_buf_current > (_ltc_log_buf+LOG_BUF_SIZE))
		_ltc_log_buf_current = _ltc_log_buf;
#else
	FILE *fp;
	fp = fopen(LOG_FILE_NAME, "a");
	if(fp) {
		fprintf(fp, "%s", buf);
		fclose(fp);
	}
#endif
}

void LTCPrintf2(char* format, ...)
{
	va_list arg;
	char buf[192];
	
	LTCPrintf_Capability_Check();
	if(_ltc_log_level2==0) {
		return;
	}

	va_start(arg, format);
	vsprintf(buf, format, arg);
	va_end(arg);
	LTCPrintf("%s", buf);
}

#endif

#ifdef _CANOPUS_DEBUG_HEAP
//http://www.flounder.com/bugincrtmemcheckpoint.htm
unsigned int  CountWalk()
{
	int HeapStatus;
	BOOL running = TRUE;
	_HEAPINFO info;
	info._pentry = NULL;
	UINT UsedBytes = 0;
     
	while(running) { /* scan heap */
		HeapStatus = _heapwalk(&info);
		switch(HeapStatus) { /* check status */
		case _HEAPOK:
			break;
		case _HEAPEND:
			running = FALSE;
			break;
		default:
			//ASSERT(FALSE);
			running = FALSE;
			continue;
		} /* check status */

		if(info._useflag == _USEDENTRY) { /* used block */
			UsedBytes += (UINT)info._size;
		} /* used block */
	} /* scan heap */

	return UsedBytes;
} // CountWalk
#endif