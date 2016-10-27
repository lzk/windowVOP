// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

//#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <winsock2.h>
#include <windows.h>
#include <winioctl.h>

/***** ADD 03/07/07 - Establish Endian setting for byte order / byte swapping *****/
#ifdef WIN32
#define LITTLE
#elif defined __linux__
#include <endian.h>
#if __BYTE_ORDER == __LITTLE_ENDIAN
#define LITTLE
#endif
#if __BYTE_ORDER == __BIG_ENDIAN
#define BIG
#endif
#else  // MAC
#ifdef __LITTLE_ENDIAN__
#define LITTLE
#else
#define BIG
#endif
#endif
/***** ADD 03/07/07 *****/


// ADD 03/07/07  #ifndef __MAC__
#ifdef WIN32  // ADD 03/07/07
#if !defined(AFX_STDAFX_H__D8E73815_8EB2_43B4_91D1_2329348E936A__INCLUDED_)
#define AFX_STDAFX_H__D8E73815_8EB2_43B4_91D1_2329348E936A__INCLUDED_
#if !defined(_LEGO_DLL) // don't use this if we are building the DLL version of toolkit

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define STRICT
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0501  // XP and later
#endif
#define _ATL_APARTMENT_THREADED

#define _CANOPUS_
#ifdef _CANOPUS_
#define _CANOPUS_DEBUG_ 0
#define DEBUG_SCAN_FLOW 0
#endif

#include "hpgtblues.h"
#include "LTCInterface\LTCDriveApi.h"
#include "LTCInterface\GLScan.h"
#include "LTCInterface\GLUsbio.h"
#include "LTCInterface\jpeg.h"
#include "LTCInterface\resample.h"
#include "LTCInterface\jpeglib.h"
#include "LTCInterface\jpeg_resize.h"
#include "LTCInterface\LTCPrintf.h"

#include <assert.h>

//
//// {F2A5E1D0-F075-4a3b-9D2F-908B3BFE2DD2}
//extern const GUID CATID_HP_SCANNER_INTERFACE;
//
//// {A2D2C384-2021-4105-B2A5-13313FEC1789}
//extern const GUID CATID_HP_BUTTON_INTERFACE;
//
//extern const GUID CATID_HP_TULIPLOG_INTERFACE;

// {{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(_LEGO_DLL)
#endif // !defined(AFX_STDAFX_H__D8E73815_8EB2_43B4_91D1_2329348E936A__INCLUDED)
#else
#include "hpgtbluesmac.h"
#include "HRESULT.h"
#include "Log.h"
#include "LogSettings.h"
#include "ScannerSession.h"
#include "legostruct.h"
#endif // __MAC__

/* ADD 03/07/07 */
#if !(defined(LITTLE) || defined(BIG))
#error LITTLE or BIG must be defined to set the "Endian-ness" for this architecture!
#endif

#if (defined(LITTLE) && defined(BIG))
#error BIG and LITTLE are both defined for this architecture ?
#endif


extern long CreateSafeArrayFromBSTRArray(BSTR* pBSTRArray,ULONG ulArraySize,SAFEARRAY** ppSafeArrayReceiver);
extern wchar_t g_ipAddress[256];
extern BOOL g_connectMode_usb;