#ifndef _LTCPrintf
#define _LTCPrintf

//#define LTCPrintf_do_not_save_file 
//#define LTCPrintf_ByThread

#if _CANOPUS_DEBUG_
	void LTCPrintf(char* format, ...);//printf + write to c:\canopus_driver_log.txt
	void LTCPrintf2(char* format, ...);
	#ifdef LTCPrintf_ByThread
		void LTCPrintfWaitLogThreadDone(void);
	#else
		#define LTCPrintfWaitLogThreadDone()
	#endif
#else
#define LTCPrintf(...)
#define LTCPrintf2(...)
#define LTCPrintfWaitLogThreadDone()
#endif

//-------------------------------------------
//to debug heap
//#define _CANOPUS_DEBUG_HEAP

#ifdef _CANOPUS_DEBUG_HEAP
unsigned int CountWalk();
#define CHECK_HEAP() LTCPrintf("%s %d, heap %d\n", __FUNCTION__, __LINE__, CountWalk());
#else
#define CHECK_HEAP()
#endif
//-------------------------------------------

#endif//#ifndef _LTCPrintf
