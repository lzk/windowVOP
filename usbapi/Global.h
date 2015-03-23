
#ifndef _GLOBAL_H_
#define _GLOBAL_H_
#include <tchar.h>
#include <stdlib.h>
#include <stdio.h>
#include <ostream>
#include <fstream>
#include <strsafe.h>
#include <direct.h>
#include <stdarg.h>
#include <time.h> 

using namespace std;

inline void U2A(wchar_t* lpwszSrc, char* lpszDest)
{
	if(lpwszSrc == NULL || lpszDest == NULL)
		return ;
	int len = ::WideCharToMultiByte(CP_ACP, NULL, lpwszSrc, wcslen(lpwszSrc), NULL, 0, NULL, NULL);

	char* szAnsi = new char[len + 1];

	::memset(szAnsi, 0, len + 1);

	::WideCharToMultiByte(CP_ACP, NULL, lpwszSrc, wcslen(lpwszSrc), szAnsi, len, NULL, NULL);

	szAnsi[len] = '\0';

	StringCbCopyA(lpszDest, len + 1, szAnsi);

	delete []szAnsi;
	szAnsi = NULL;
}

inline void A2U(char* lpszDest, wchar_t* lpwszSrc)
{
	if(lpwszSrc == NULL || lpszDest == NULL)
		return ;

	int len  = ::MultiByteToWideChar(CP_ACP, 0, lpszDest, strlen(lpszDest), lpwszSrc, 0);

	int nBufsize = len * sizeof(wchar_t) + 1 * sizeof(wchar_t);
	wchar_t* wszUnicode = new wchar_t[nBufsize];

	::memset(wszUnicode, 0, nBufsize);

	::MultiByteToWideChar(CP_ACP, 0, lpszDest, strlen(lpszDest), wszUnicode, nBufsize - sizeof(wchar_t));

	wszUnicode[nBufsize - sizeof(wchar_t)] = _T('\0');

	StringCbCopy(lpwszSrc, nBufsize, wszUnicode);

	delete []wszUnicode;
	wszUnicode = NULL;
}

USBAPI_API int OutputDebugStringToFileA(const char *_lpFormat, ...);
USBAPI_API int __stdcall OutputDebugStringToFile(TCHAR *_lpFormat, ...);
USBAPI_API int __stdcall OutputDebugStringToFile_(TCHAR *_lpString);
int OutputDebugStringToFileA_(LPCSTR _lpFileName, char *_lpFormat, ...);

#endif