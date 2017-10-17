// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

CRITICAL_SECTION g_csCriticalSection;
CRITICAL_SECTION g_csCriticalSection_bonjour;
CRITICAL_SECTION g_csCriticalSection_connect;

CRITICAL_SECTION g_csCriticalSection_UsbTest;
CRITICAL_SECTION g_csCriticalSection_NetWorkTest;
//add by yunying shang 2017-10-12 for BMS1082 and 842
UINT WM_VOPSCAN_PROGRESS;
UINT WM_VOPSCAN_UPLOAD;
UINT WM_VOPSCAN_PAGECOMPLETE;
/////////////////////
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		InitializeCriticalSection(&g_csCriticalSection);
		InitializeCriticalSection(&g_csCriticalSection_bonjour);
		InitializeCriticalSection(&g_csCriticalSection_connect);
		InitializeCriticalSection(&g_csCriticalSection_UsbTest);
		InitializeCriticalSection(&g_csCriticalSection_NetWorkTest);

		//add by yunying shang 2017-10-12 for BMS1082 and 842
		WM_VOPSCAN_PROGRESS = RegisterWindowMessage(L"vop_scan_progress");
		WM_VOPSCAN_UPLOAD = RegisterWindowMessage(L"vop_scan_upload");
		WM_VOPSCAN_PAGECOMPLETE = RegisterWindowMessage(L"vop_scan_pagecomplete");
		//add by yunying shang 2017-10-12 for BMS1082 and 842
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		DeleteCriticalSection(&g_csCriticalSection);
		DeleteCriticalSection(&g_csCriticalSection_bonjour);
		DeleteCriticalSection(&g_csCriticalSection_connect);
		DeleteCriticalSection(&g_csCriticalSection_UsbTest);
		DeleteCriticalSection(&g_csCriticalSection_NetWorkTest);
		break;
	}
	return TRUE;
}

