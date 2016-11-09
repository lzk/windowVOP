// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

CRITICAL_SECTION g_csCriticalSection;
CRITICAL_SECTION g_csCriticalSection_bonjour;
CRITICAL_SECTION g_csCriticalSection_connect;

CRITICAL_SECTION g_csCriticalSection_UsbTest;
CRITICAL_SECTION g_csCriticalSection_NetWorkTest;

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

