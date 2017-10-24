#include "stdafx.h"
#include "usbapi.h"
#include "Global.h"

USBAPI_API int __stdcall OutputDebugStringToFile(TCHAR *_lpFormat, ...)
{
#if 1
	fstream file;
	char szFile[MAX_PATH] = {0};

	WIN32_FIND_DATAA wfd;
	
	GetModuleFileNameA(NULL, szFile, MAX_PATH);
	strrchr(szFile, '\\')[0] = '\0';
	StringCbCatA(szFile, MAX_PATH, "\\vopdbg.txt");

	HANDLE hFind = FindFirstFileA(szFile, &wfd);
	if(hFind == INVALID_HANDLE_VALUE)
	{
		GetTempPathA(MAX_PATH, szFile);
		strrchr(szFile, '\\')[0] = '\0';
		StringCbCatA(szFile, MAX_PATH, "\\vopdbg.txt");
	}

	file.open(szFile, ios_base::out | ios_base::app | ios_base::ate);
	
	if(!file.good())
		return -1;

	TCHAR szDebug[MAX_PATH * 10] = {0};
	TCHAR szTemp[MAX_PATH * 10] = {0};
	va_list parg;
	va_start(parg, _lpFormat);

	for(int i = 0; i < MAX_PATH * 2; i++)
	{
		if(*_lpFormat == '\0')
		{
			szDebug[i] = *_lpFormat;
			va_end(parg);
			break;
		}
		if(*_lpFormat != _T('%'))
		{
			szDebug[i] = *_lpFormat;
		}
		else
		{
			switch(*(++_lpFormat))
			{
			case _T('d'):
				{
					int temp = va_arg(parg, int);
					wsprintf(szTemp, _T("%d"), temp);
					lstrcat(szDebug, szTemp);
					i += lstrlen(szTemp) - 1;
				}
				break;
			case _T('s'):
				{
					TCHAR *s = va_arg( parg, TCHAR* );
					wsprintf(szTemp, _T("%s"), s);
					lstrcat(szDebug, szTemp);
					i += lstrlen(szTemp) - 1;

				}
				break;
			case _T('x'):
				{
					DWORD temp = va_arg(parg, DWORD);
					wsprintf(szTemp, _T("%x"), temp);
					lstrcat(szDebug, szTemp);
					i += lstrlen(szTemp) - 1;
				}
				break;
			case _T('u'):
				{
					DWORD temp = va_arg(parg, int);
					wsprintf(szTemp, _T("%u"), temp);
					lstrcat(szDebug, szTemp);
					i += lstrlen(szTemp) - 1;
				}
				break;
			default:
				{
					file << _T("the format not support!") << endl;

					file.close();
				}
					return 0;
			}
			
		}
		_lpFormat++;
	}

	time_t t = time(0);
	struct tm newtime;

	char tmp[64] = {0};
	char szInfo[MAX_PATH * 10] = { 0 };
	_localtime64_s(&newtime, &t);
	strftime(tmp, sizeof(tmp), "%y-%m-%d %H:%M:%S\t\t", &newtime);

#ifdef _UNICODE
	U2A(szDebug, szInfo);
#else
	StringCbCopy(szInfo, MAX_PATH * 2, szDebug);
#endif
	
	file << tmp << szInfo << endl;

	file.close();
#endif
	return 0;
}

USBAPI_API int OutputDebugStringToFileA(const char *_lpFormat, ...)
{
#if 0
	fstream file;
	char szFile[MAX_PATH] = {0};

	WIN32_FIND_DATAA wfd;
	
	GetModuleFileNameA(NULL, szFile, MAX_PATH);
	strrchr(szFile, '\\')[0] = '\0';
	StringCbCatA(szFile, MAX_PATH, "\\vopdbg.txt");

	HANDLE hFind = FindFirstFileA(szFile, &wfd);
	if(hFind == INVALID_HANDLE_VALUE)
	{
		GetTempPathA(MAX_PATH, szFile);
		strrchr(szFile, '\\')[0] = '\0';
		StringCbCatA(szFile, MAX_PATH, "\\vopdbg.txt");
	}

	file.open(szFile, ios_base::out | ios_base::app | ios_base::ate);
	
	if(!file.good())
		return -1;

	char szDebug[MAX_PATH * 10] = {0};
	char szTemp[MAX_PATH * 10] = {0};
	va_list parg;
	va_start(parg, _lpFormat);

	for(int i = 0; i < MAX_PATH * 2; i++)
	{
		if(*_lpFormat == '\0')
		{
			szDebug[i] = *_lpFormat;
			va_end(parg);
			break;
		}
		if(*_lpFormat != _T('%'))
		{
			szDebug[i] = *_lpFormat;
		}
		else
		{
			switch(*(++_lpFormat))
			{
			case _T('d'):
				{
					int temp = va_arg(parg, int);
					sprintf(szTemp, ("%d"), temp);
					strcat(szDebug, szTemp);
					i += strlen(szTemp) - 1;
				}
				break;
			case _T('s'):
				{
					char *s = va_arg( parg, char* );
					sprintf(szTemp, ("%s"), s);
					strcat(szDebug, szTemp);
					i += strlen(szTemp) - 1;
				}
				break;
			case _T('x'):
				{
					DWORD temp = va_arg(parg, DWORD);
					sprintf(szTemp, ("%x"), temp);
					strcat(szDebug, szTemp);
					i += strlen(szTemp) - 1;
				}
				break;
			case _T('u'):
				{
					DWORD temp = va_arg(parg, int);
					sprintf(szTemp, ("%u"), temp);
					strcat(szDebug, szTemp);
					i += strlen(szTemp) - 1;
				}
				break;
			default:
				{
					file << _T("the format not support!") << endl;

					file.close();
				}
					return 0;
			}
			
		}
		_lpFormat++;
	}

	time_t t = time(0);
	struct tm newtime;

	char tmp[64] = { 0 };
	char szInfo[MAX_PATH * 10] = { 0 };
	_localtime64_s(&newtime, &t);
	strftime(tmp, sizeof(tmp), "%y-%m-%d %H:%M:%S\t\t", &newtime);


	StringCbCopyA(szInfo, MAX_PATH * 10, szDebug);
	
	file << tmp << szInfo << endl;

	file.close();
#endif
	return 0;
}

int OutputDebugStringToFileA_(LPCSTR _lpFileName, char *_lpFormat, ...)
{
#if 0
	fstream file;
	char szFile[MAX_PATH];

	WIN32_FIND_DATAA wfd;

	GetModuleFileNameA(NULL, szFile, MAX_PATH);
	strrchr(szFile, '\\')[0] = '\0';
	StringCbCatA(szFile, MAX_PATH, _lpFileName);

	HANDLE hFind = FindFirstFileA(szFile, &wfd);
	if (hFind == INVALID_HANDLE_VALUE)
	{
		GetTempPathA(MAX_PATH, szFile);
//		strrchr(szFile, '\\')[0] = '\0';
		StringCbCatA(szFile, MAX_PATH, _lpFileName);
	}

	file.open(szFile, ios_base::out | ios_base::app | ios_base::ate);

	if (!file.good())
		return -1;

	char szDebug[MAX_PATH * 10] = { 0 };
	char szTemp[MAX_PATH * 10] = { 0 };
	va_list parg;
	va_start(parg, _lpFormat);

	for (int i = 0; i < MAX_PATH * 2; i++)
	{
		if (*_lpFormat == '\0')
		{
			szDebug[i] = *_lpFormat;
			va_end(parg);
			break;
		}
		if (*_lpFormat != _T('%'))
		{
			szDebug[i] = *_lpFormat;
		}
		else
		{
			switch (*(++_lpFormat))
			{
			case _T('d'):
			{
							int temp = va_arg(parg, int);
							sprintf(szTemp, ("%d"), temp);
							strcat(szDebug, szTemp);
							i += strlen(szTemp) - 1;
			}
				break;
			case _T('s'):
			{
							char *s = va_arg(parg, char*);
							sprintf(szTemp, ("%s"), s);
							strcat(szDebug, szTemp);
							i += strlen(szTemp) - 1;
			}
				break;
			case _T('x'):
			{
							DWORD temp = va_arg(parg, DWORD);
							sprintf(szTemp, ("%x"), temp);
							strcat(szDebug, szTemp);
							i += strlen(szTemp) - 1;
			}
				break;
			case _T('u'):
			{
							DWORD temp = va_arg(parg, int);
							sprintf(szTemp, ("%u"), temp);
							strcat(szDebug, szTemp);
							i += strlen(szTemp) - 1;
			}
				break;
			default:
			{
					   file << _T("the format not support!") << endl;

					   file.close();
			}
				return 0;
			}

		}
		_lpFormat++;
	}

	time_t t = time(0);
	struct tm newtime;

	char tmp[64];
	char szInfo[MAX_PATH * 10];
	_localtime64_s(&newtime, &t);
	strftime(tmp, sizeof(tmp), "%y-%m-%d %H:%M:%S\t\t", &newtime);


	StringCbCopyA(szInfo, MAX_PATH * 10, szDebug);

	file << tmp << szInfo << endl;

	file.close();
#endif
	return 0;
}

USBAPI_API int __stdcall OutputDebugStringToFile_(TCHAR *_lpString)
{
	return OutputDebugStringToFile(_lpString);
}