#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include "usbio.h"
#include "ScanCMD.h"
#include "model.h"
#include "Strsafe.h"
#include "string.h"
#include "../ImgFile/ImgFile.h"
#include <tchar.h>

//#define IMG_NAME_MODE_ONLY


//-- scan parameter -----------------------------------------------
SC_PAR_T_ sc_par = {SCAN_SOURCE, SCAN_ACQUIRE, SCAN_OPTION, SCAN_DUPLEX, SCAN_PAGE,
                  {IMG_FORMAT, IMG_OPTION, IMG_BIT, IMG_MONO,
                  {IMG_DPI_X, IMG_DPI_Y}, {IMG_ORG_X, IMG_ORG_Y}, IMG_WIDTH, IMG_HEIGHT},
				  //{{MTR_DRIV_TAR, MTR_STAT_MEC, MTR_DIRECT, MTR_MICRO_STEP, MTR_CURRENT, MTR_SPEED, MTR_ACC_STEP, 0},
                  //{MTR_DRIV_TAR, MTR_STAT_MEC, MTR_DIRECT, MTR_MICRO_STEP, MTR_CURRENT, MTR_SPEED, MTR_ACC_STEP, 0}}
				  {{0},
				  {0}}
				  ,{0}};

//IMG_FILE_T ImgFile[2] = {{&sc_par.img}, {&sc_par.img}};
IMG_FILE_T ImgFile[2];
IMAGE_T ImgTemp[2];

int time;

char filename[24];
U8  scanBuf[0x100000];
//-----------------------------------------------
SC_INFO_T_ sc_info;
//-----------------------------------------------

extern char IniFile[256], Profile[64];

extern int BatchNum;

extern U8 SCAN_DOC_SIZE;

int WritePrivateProfileInt(LPCTSTR   lpAppName, LPCTSTR   lpKeyName, INT   Value, LPCSTR   lpFileName)
{
	TCHAR   ValBuf[16];

	StringCbPrintf(ValBuf, sizeof(ValBuf), TEXT("%i"), Value);

	TCHAR fileNamce[256];
	StringCbPrintf(fileNamce, sizeof(fileNamce), TEXT("%s"), lpFileName);

	return(WritePrivateProfileString(lpAppName, lpKeyName, ValBuf, fileNamce));
}



U32 write_to_ini(void)
{

	GetModuleFileNameA(NULL, IniFile, sizeof(IniFile));
	strcpy(strrchr(IniFile, '.'), "_calibration.ini");

	WritePrivateProfileStringA("OPTION", "SOURCE", (LPSTR)(sc_par.source == I3('FLB') ? "FLB" : "ADF"), IniFile);
	WritePrivateProfileStringA("OPTION", "FORMAT", (LPSTR)(sc_par.img.format == I3('JPG') ? "JPG" : "TIF"), IniFile);
	WritePrivateProfileInt(L"OPTION", L"COLOR_MODE", sc_par.img.mono, IniFile);
	WritePrivateProfileInt(L"OPTION", L"PIXEL_DEPTH", sc_par.img.bit, IniFile);
	WritePrivateProfileInt(L"OPTION", L"MAIN_SOLU", sc_par.img.dpi.x, IniFile);
	WritePrivateProfileInt(L"OPTION", L"SUB_SOLU", sc_par.img.dpi.y, IniFile);
	WritePrivateProfileInt(L"OPTION", L"SCAN_SIDE", sc_par.duplex, IniFile);
	if ((SCAN_DOC_SIZE != DOC_S_PRNU) && (SCAN_DOC_SIZE != DOC_K_PRNU))
		WritePrivateProfileInt(L"OPTION", L"SCAN_SIZE", SCAN_DOC_SIZE, IniFile);


	return TRUE;
}

U32 read_from_ini(void)
{
	GetModuleFileNameA(NULL, IniFile, sizeof(IniFile));
	strcpy(strrchr(IniFile, '.') , "_calibration.ini");

	GetPrivateProfileStringA("OPTION", "SOURCE", "ADF", (LPSTR)&sc_par.source, 4, IniFile);
	//sc_par.acquire = 0;
	//if(GetPrivateProfileInt(Profile, "SCAN_TEST_PATTERN", 0, IniFile))
	//  par->acquire |= ACQ_TEST_PATTERN;
	//par->option = 0;
	//par->duplex = GetPrivateProfileInt(Profile, "SCAN_DUPLEX", 1, IniFile);
	//par->page = GetPrivateProfileInt(Profile, "SCAN_PAGE", 0, IniFile);

	GetPrivateProfileStringA("OPTION", "FORMAT", "JPG", (LPSTR)&sc_par.img.format, 4, IniFile);
	if (sc_par.img.format != I3('JPG'))
		sc_par.img.format = I3('RAW');

	sc_par.img.mono = GetPrivateProfileIntA("OPTION", "COLOR_MODE", IMG_MONO, IniFile);
	sc_par.img.bit = GetPrivateProfileIntA("OPTION", "PIXEL_DEPTH", IMG_BIT, IniFile);
	sc_par.img.dpi.x = GetPrivateProfileIntA("OPTION", "MAIN_SOLU", IMG_DPI_X, IniFile);
	sc_par.img.dpi.y = GetPrivateProfileIntA("OPTION", "SUB_SOLU", IMG_DPI_Y, IniFile);
	SCAN_DOC_SIZE = GetPrivateProfileIntA("OPTION", "SCAN_SIZE", DOC_SIZE_FULL, IniFile);
	sc_par.duplex = GetPrivateProfileIntA("OPTION", "SCAN_SIDE", SCAN_AB_SIDE, IniFile);
	return TRUE;
}

int Load_ScanParameter(SC_PAR_T_ *par)
{
	IMAGE_T *img = (IMAGE_T *)&par->img;

/***************************IMAGE_T**********************************/
	par->option = 0;

	if(par->source == SCAN_ADF) {
		//par->duplex = SCAN_AB_SIDE;
		par->duplex = par->duplex;
	}
	else {
		par->duplex = SCAN_A_SIDE;
	}
        
	//par->page = 0;

	if(img->format == I3('JPG')) {
		img->option = IMG_OPT_JPG_FMT444;
	}


	switch(SCAN_DOC_SIZE) {
		case DOC_SIZE_FULL:
			img->width = IMG_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_300_ORG_X * img->dpi.x/300;

			img->height = IMG_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_SIZE_A4:
			img->width = IMG_A4_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_A4_300_ORG_X * img->dpi.x/300;

			img->height = IMG_A4_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_SIZE_LT:
			img->width = IMG_LT_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_LT_300_ORG_X * img->dpi.x/300;

			img->height = IMG_LT_300_DOT_Y *img->dpi.y/300;
			break;

                case DOC_SIZE_LG14:
			img->width = IMG_LG14_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_LG14_300_ORG_X * img->dpi.x/300;

			img->height = IMG_LG14_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_SIZE_LL:
			img->width = IMG_LL_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_LL_300_ORG_X * img->dpi.x/300;

			img->height = IMG_LL_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_FB_LIFE:
			img->width = IMG_FB_LIFE_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_FB_LIFE_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_S_PRNU:
			img->width = IMG_S_PRNU_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_S_PRUN_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_K_PRNU:
			img->width = IMG_K_PRNU_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_K_PRUN_300_DOT_Y *img->dpi.y/300;
			break;

	}
	img->width -= (img->width%8);
	img->height -= (img->height%8);



/***************************MTR_T**********************************/
	if(par->acquire & ACQ_SET_MTR) {

		if(par->source == I3('FLB')) {
			/*FB scan*/
			//par->mtr[0].drive_target = MT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			switch(par->img.dpi.x) {
				case 300:
					par->mtr[0].speed_pps = GetPrivateProfileIntA("FB300x300M", "PPS", 0, IniFile);
					par->mtr[0].direction = GetPrivateProfileIntA("FB300x300M", "DIR", 0, IniFile);
					par->mtr[0].micro_step = GetPrivateProfileIntA("FB300x300M", "MS", 0, IniFile);
					par->mtr[0].currentLV = GetPrivateProfileIntA("FB300x300M", "CLV", 0, IniFile);
                    break;
				case 600:
					par->mtr[0].speed_pps = GetPrivateProfileIntA("FB600x600", "PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("FB600x600", "DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("FB600x600", "MS", 0, IniFile); 
					par->mtr[0].currentLV =  GetPrivateProfileIntA("FB600x600", "CLV", 0, IniFile);
					break;
			}
		}
		else {
			/*ADF scan*/
			par->mtr[1].pick_ss_step = GetPrivateProfileIntA("PICK_SS_STEP", "STEP", 0, IniFile);

			//par->mtr[0].drive_target = CMT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			//par->mtr[1].drive_target = BMT_PH;
			//par->mtr[1].state_mechine = STATE_MECHINE_1;
			if(par->img.bit < 24) {
				/*Mono scan*/
				par->mtr[1].speed_pps = GetPrivateProfileIntA("ADFgray", "PICK_PPS", 0, IniFile);
				par->mtr[1].direction = GetPrivateProfileIntA("ADFgray", "PICK_DIR", 0, IniFile);
				par->mtr[1].micro_step = GetPrivateProfileIntA("ADFgray", "PICK_MS", 0, IniFile);
				par->mtr[1].currentLV = GetPrivateProfileIntA("ADFgray", "PICK_CLV", 0, IniFile);

				par->mtr[0].speed_pps = GetPrivateProfileIntA("ADFgray", "FEED_PPS", 0, IniFile); 
				par->mtr[0].direction = GetPrivateProfileIntA("ADFgray", "FEED_DIR", 0, IniFile); 
				par->mtr[0].micro_step = GetPrivateProfileIntA("ADFgray", "FEED_MS", 0, IniFile); 
				par->mtr[0].currentLV = GetPrivateProfileIntA("ADFgray", "FEED_CLV", 0, IniFile); 
			}
			else {
				/*Color scan*/
				if((par->img.dpi.x == 300) && (par->img.dpi.y == 300)) {
					/*300x300DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF300x300color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF300x300color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF300x300color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF300x300color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF300x300color", "FEED_PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("ADF300x300color", "FEED_DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF300x300color", "FEED_MS", 0, IniFile); 
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF300x300color", "FEED_CLV", 0, IniFile); 
				}
				else if((par->img.dpi.x == 300) && (par->img.dpi.y == 600)) {
					/*300x600DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF300x600color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF300x600color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF300x600color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF300x600color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF300x600color", "FEED_PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("ADF300x600color", "FEED_DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF300x600color", "FEED_MS", 0, IniFile); 
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF300x600color", "FEED_CLV", 0, IniFile);	
				}
				else {
					/*600DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF600x600color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF600x600color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF600x600color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF600x600color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF600x600color", "FEED_PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("ADF600x600color", "FEED_DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF600x600color", "FEED_MS", 0, IniFile); 
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF600x600color", "FEED_CLV", 0, IniFile); 
				}

			}
		}

		switch(par->mtr[0].currentLV) {
			case 1:
				par->mtr[0].currentLV = 0;
				break;
			case 2:
				par->mtr[0].currentLV = 2;
				break;
			case 3:
				par->mtr[0].currentLV = 1;
				break;
			case 4:
				par->mtr[0].currentLV = 3;
				break;
		}

		switch(par->mtr[1].currentLV) {
			case 1:
				par->mtr[1].currentLV = 0;
				break;
			case 2:
				par->mtr[1].currentLV = 2;
				break;
			case 3:
				par->mtr[1].currentLV = 1;
				break;
			case 4:
				par->mtr[1].currentLV = 3;
				break;
		}
		
	}
/***************************ME_T**********************************/
	if(par->acquire & ACQ_SET_ME) {
		par->me.leading_edge = GetPrivateProfileIntA("FB_LEADING", "FB_leading", 0, IniFile);
		par->me.img_gap = GetPrivateProfileIntA("CIS_GAP", "cis_gap", 0, IniFile);
		par->me.prefed = GetPrivateProfileIntA("PREFEED", "prefeed", 0, IniFile);
		par->me.postfed = GetPrivateProfileIntA("POSTFEED", "postfeed", 0, IniFile);
		par->me.side_edgeA = GetPrivateProfileIntA("START_PIXEL_A", "start_pixel_A", 0, IniFile);
		par->me.side_edgeB = GetPrivateProfileIntA("START_PIXEL_B", "start_pixel_B", 0, IniFile);
	}

	memcpy(&ImgTemp[0], &sc_par.img, sizeof(IMAGE_T));
	memcpy(&ImgTemp[1], &sc_par.img, sizeof(IMAGE_T));

	ImgFile[0].img = ImgTemp[0];
	ImgFile[1].img = ImgTemp[1];


	write_to_ini();

	//sc_par.acquire |= ACQ_NO_SHADING;
	//sc_par.acquire |= ACQ_TEST_PATTERN;

	return TRUE;
}

int Load_CalibrationParameter(SC_PAR_T_ *par)
{
	IMAGE_T *img = (IMAGE_T *)&par->img;

/***************************IMAGE_T**********************************/
	par->option = 0;

	if(img->format == I3('JPG')) {
		img->option = IMG_OPT_JPG_FMT444;
	}


	switch(SCAN_DOC_SIZE) {
		case DOC_SIZE_FULL:
			img->width = IMG_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_300_ORG_X * img->dpi.x/300;

			img->height = IMG_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_SIZE_A4:
			img->width = IMG_A4_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_A4_300_ORG_X * img->dpi.x/300;

			img->height = IMG_A4_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_SIZE_LT:
			img->width = IMG_LT_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_LT_300_ORG_X * img->dpi.x/300;

			img->height = IMG_LT_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_SIZE_LL:
			img->width = IMG_LL_300_DOT_X * img->dpi.x/300;
			img->org.x = IMG_LL_300_ORG_X * img->dpi.x/300;

			img->height = IMG_LL_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_FB_LIFE:
			img->width = IMG_FB_LIFE_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_FB_LIFE_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_S_PRNU:
			img->width = IMG_S_PRNU_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_S_PRUN_300_DOT_Y *img->dpi.y/300;
			break;

		case DOC_K_PRNU:
			img->width = IMG_K_PRNU_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_K_PRUN_300_DOT_Y *img->dpi.y/300;
			//img->height = IMG_K_PRUN_300_DOT_Y *300/img->dpi.y; //Image buf issue
			break;

		case DOC_K_PREFEED:
			img->width = IMG_K_PREFEED_300_DOT_X * img->dpi.x/300;
			img->org.x = 0 * img->dpi.x/300;

			img->height = IMG_K_PREFEED_300_DOT_Y *img->dpi.y/300;
			break;
	}
	img->width -= (img->width%8);
	img->height -= (img->height%8);



/***************************MTR_T**********************************/
	if(par->acquire & ACQ_SET_MTR) {

		if(par->source == I3('FLB')) {
			/*FB scan*/
			//par->mtr[0].drive_target = MT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			switch(par->img.dpi.x) {
				case 300:
					par->mtr[0].speed_pps = GetPrivateProfileIntA("FB300x300M", "PPS", 0, IniFile);
					par->mtr[0].direction = GetPrivateProfileIntA("FB300x300M", "DIR", 0, IniFile);
					par->mtr[0].micro_step = GetPrivateProfileIntA("FB300x300M", "MS", 0, IniFile);
					par->mtr[0].currentLV = GetPrivateProfileIntA("FB300x300M", "CLV", 0, IniFile);
                    break;
				case 600:
					par->mtr[0].speed_pps = GetPrivateProfileIntA("FB600x600", "PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("FB600x600", "DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("FB600x600", "MS", 0, IniFile); 
					par->mtr[0].currentLV =  GetPrivateProfileIntA("FB600x600", "CLV", 0, IniFile);
					break;
			}
		}
		else {
			/*ADF scan*/
			par->mtr[1].pick_ss_step = GetPrivateProfileIntA("PICK_SS_STEP", "STEP", 0, IniFile);

			//par->mtr[0].drive_target = CMT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			//par->mtr[1].drive_target = BMT_PH;
			//par->mtr[1].state_mechine = STATE_MECHINE_1;
			if(par->img.bit < 24) {
				/*Mono scan*/
				par->mtr[1].speed_pps = GetPrivateProfileIntA("ADFgray", "PICK_PPS", 0, IniFile);
				par->mtr[1].direction = GetPrivateProfileIntA("ADFgray", "PICK_DIR", 0, IniFile);
				par->mtr[1].micro_step = GetPrivateProfileIntA("ADFgray", "PICK_MS", 0, IniFile);
				par->mtr[1].currentLV = GetPrivateProfileIntA("ADFgray", "PICK_CLV", 0, IniFile);

				par->mtr[0].speed_pps = GetPrivateProfileIntA("ADFgray", "FEED_PPS", 0, IniFile); 
				par->mtr[0].direction = GetPrivateProfileIntA("ADFgray", "FEED_DIR", 0, IniFile); 
				par->mtr[0].micro_step = GetPrivateProfileIntA("ADFgray", "FEED_MS", 0, IniFile); 
				par->mtr[0].currentLV = GetPrivateProfileIntA("ADFgray", "FEED_CLV", 0, IniFile); 
			}
			else {
				/*Color scan*/
				if((par->img.dpi.x == 300) && (par->img.dpi.y == 300)) {
					/*300x300DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF300x300color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF300x300color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF300x300color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF300x300color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF300x300color", "FEED_PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("ADF300x300color", "FEED_DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF300x300color", "FEED_MS", 0, IniFile); 
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF300x300color", "FEED_CLV", 0, IniFile); 
				}
				else if((par->img.dpi.x == 300) && (par->img.dpi.y == 600)) {
					/*300x600DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF300x600color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF300x600color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF300x600color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF300x600color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF300x600color", "FEED_PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("ADF300x600color", "FEED_DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF300x600color", "FEED_MS", 0, IniFile); 
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF300x600color", "FEED_CLV", 0, IniFile);	
				}
				else {
					/*600DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF600x600color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF600x600color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF600x600color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF600x600color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF600x600color", "FEED_PPS", 0, IniFile); 
					par->mtr[0].direction = GetPrivateProfileIntA("ADF600x600color", "FEED_DIR", 0, IniFile); 
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF600x600color", "FEED_MS", 0, IniFile); 
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF600x600color", "FEED_CLV", 0, IniFile); 
				}

			}
		}

		switch(par->mtr[0].currentLV) {
			case 1:
				par->mtr[0].currentLV = 0;
				break;
			case 2:
				par->mtr[0].currentLV = 2;
				break;
			case 3:
				par->mtr[0].currentLV = 1;
				break;
			case 4:
				par->mtr[0].currentLV = 3;
				break;
		}

		switch(par->mtr[1].currentLV) {
			case 1:
				par->mtr[1].currentLV = 0;
				break;
			case 2:
				par->mtr[1].currentLV = 2;
				break;
			case 3:
				par->mtr[1].currentLV = 1;
				break;
			case 4:
				par->mtr[1].currentLV = 3;
				break;
		}
		
	}
/***************************ME_T**********************************/
	if(par->acquire & ACQ_SET_ME) {
		par->me.leading_edge = GetPrivateProfileIntA("FB_LEADING", "FB_leading", 0, IniFile);
		par->me.img_gap = GetPrivateProfileIntA("CIS_GAP", "cis_gap", 0, IniFile);
		par->me.prefed = GetPrivateProfileIntA("PREFEED", "prefeed", 0, IniFile);
		par->me.postfed = GetPrivateProfileIntA("POSTFEED", "postfeed", 0, IniFile);
		par->me.side_edgeA = GetPrivateProfileIntA("START_PIXEL_A", "start_pixel_A", 0, IniFile);
		par->me.side_edgeB = GetPrivateProfileIntA("START_PIXEL_B", "start_pixel_B", 0, IniFile);
	}


	write_to_ini();

	return TRUE;
}


int ResetScanFlow(void)
{
	_cancel();
	_JobEnd();

	return TRUE;
}

/*
#define JOB_WAIT_TIMEOUT  5000
int waitJobFinish(int job, int wait_motor_stop)
{
  U32 tick = GetTickCount();
  while((GetTickCount() - tick) < JOB_WAIT_TIMEOUT) {
    if(!_info(&sc_info))
      break;
    if(!(sc_info.JobState & job) && (!wait_motor_stop || !sc_info.MotorMove))
      return TRUE;
    Sleep(100);
  }
  return FALSE;
}
*/
//-------------------------------------------------
//int GammaTransLTCtoGL(unsigned int *pbyRed, unsigned int *pbyGreen,unsigned int *pbyBlue,unsigned int *GLGamma)
//{
//    int i;
//    for(i=0;i<256;i++)
//    {
//		if(i<255){
//		   GLGamma[i]    = ((unsigned int)(*(pbyRed+i*256))&0xffff) + (unsigned int)(((*(pbyRed+((i+1)*256)))&0xffff)<<16); 
//		   GLGamma[i+256] = ((unsigned int)(*(pbyGreen+i*256))&0xffff) + (unsigned int)(((*(pbyGreen+((i+1)*256)))&0xffff)<<16); 
//		   GLGamma[i+256*2]= ((unsigned int)(*(pbyBlue+i*256))&0xffff) + (unsigned int)(((*(pbyBlue+((i+1)*256)))&0xffff)<<16); 
//		}
//		else{
//			GLGamma[i]    =(unsigned int)(*(pbyRed+i*256))&0xffff|((unsigned int)(0xffff)<<16); 
//			GLGamma[i+256] = (unsigned int)(*(pbyGreen+i*256))&0xffff|((unsigned int)(0xffff)<<16); 
//			GLGamma[i+256*2]= (unsigned int)(*(pbyBlue+i*256))&0xffff|((unsigned int)(0xffff)<<16); 
//		}
//
//
//    }
//    return 1;
//}

#define START_STAGE					0x1
#define SCANNING_STAGE				0x2
#define PUSH_TRANSFER_STAGE		0x3

int ScannerStatusCheck(char stage)
{
	int result = TRUE;
	if(!_info(&sc_info)) {
		printf("INFO command error!!");
		result = FALSE;
	}
	else {
		if(sc_info.JobID || sc_info.SystemStatus.scanning) {
			if(stage == START_STAGE) {
				printf("Last job not finish!!\n");
				result = FALSE;
			}
		}
		else {
			if(stage == SCANNING_STAGE) {
				printf("Scan job missing!!\n");
				result = FALSE;
			}
		}

			
		if(sc_info.ErrorStatus.cover_open_err) {
			if(stage != PUSH_TRANSFER_STAGE) {
				printf("COVER_OPEN_ERR\n");
				result = FALSE;
			}
		}
		if(sc_info.ErrorStatus.scan_jam_err) {
			printf("SCAN_JAM_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.scan_canceled_err) {
			printf("SCAN_CANCELED_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.scan_timeout_err) {
			printf("SCAN_TIMEOUT_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.multi_feed_err) {
			printf("MULTI_FEED_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.usb_transfer_err) {
			printf("USB_TRANSFER_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.wifi_transfer_err) {
			printf("WiFi_TRANSFER_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.usb_disk_transfer_err) {
			printf("USBDISK_TRANSFER_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.ftp_transfer_err) {
			printf("FTP_TRANSFER_ERR\n");
			result = FALSE;
		}
		if(sc_info.ErrorStatus.smb_transfer_err) {
			printf("SMB_TRANSFER_ERR\n");
			result = FALSE;
		}

		
		if(stage == START_STAGE) {
			if(sc_info.SensorStatus.adf_document_sensor) {
				printf("ADF document not ready.\n");
				result = FALSE;
			}
			if(sc_info.SensorStatus.adf_paper_sensor) {
				printf("ADF path not ready.\n");
				result = FALSE;
			}
			if(sc_info.SensorStatus.cover_sensor) {
				printf("ADF cover not ready.\n");
				result = FALSE;
			}
		}
	}

	return result;
}


TCHAR   ValBuf[16];
extern int _gamma();

//-------------------------------------------------
int SCAN_FLOW(void)
{
  int result=1;
  int duplex, dup;
  IMAGE_T *img = &sc_par.img;
  FILE *fp;
  char ImgFileName[64];
  int bFiling[2] = {0, 0};
  int length, cancel, lineSize;
  unsigned int PageNum[2]={0,0};
  

  Load_ScanParameter(&sc_par);

  if(!ScannerStatusCheck(START_STAGE)) {
    system("PAUSE");
    return FALSE;
  }
  
  /*if( (sc_par.acquire & ACQ_MOTOR_OFF) ||
	(sc_par.acquire & ACQ_NO_PP_SENSOR) ||
	(sc_par.acquire & ACQ_TEST_PATTERN) ) {
	 
  }
  else*/ {
	  if(sc_par.source == I3('ADF')) {
		  if(!_JobCreate(JOB_ADF))
			return FALSE;
	  }
	  else {
		  if(!_JobCreate(JOB_FLB))
			return FALSE;
	  }
  }

	if(!_parameters(&sc_par)) {
		result = FALSE;
		goto JobEnd;
	}

	if(sc_par.acquire & ACQ_GAMMA) {
		if(!_gamma()) {
			result = FALSE;
			goto JobEnd;
		}
	}

	if(_StartScan()) {
		time = GetTickCount();

    duplex = sc_par.duplex;
    cancel = FALSE;
    lineSize = (img->format != I3('JPG'))? ((img->bit*img->width+7)/8): 0;
    while(!cancel) {

      //_info(&sc_info);
		if(!ScannerStatusCheck(SCANNING_STAGE)) {
			break;
		}

#if 0
      if(_kbhit()) {
        _getch();
		_cancel();
		_JobEnd();
		printf("SCAN_CANCELED_ERR\n");
        return TRUE;
      }
#endif

      if((!(duplex & 1) || sc_info.ImgStatus[0].EndScan) && (!(duplex & 2) || sc_info.ImgStatus[1].EndScan))
        break;

	  //if(!error_check(&sc_info)) {
		  //cancel = TRUE;
		//  break;
	  //}

#if 1 //define 0 for bypass image receive
      for (dup = 0; dup < 2; dup++) {
		if((duplex & (1<<dup)) && sc_info.ValidPageSize[dup]) {
          length = min(sizeof(scanBuf), sc_info.ValidPageSize[dup]);
          if(lineSize)
            length -= (length % lineSize);
          if(_imgRead(dup, scanBuf, &length)) {
            if(!bFiling[dup]) {
              bFiling[dup]++;
			  PageNum[dup]++;

			  sprintf(ImgFileName, "Batch%d_%s_%s_%dx%d_%d_%c.%s", BatchNum, ((U8)sc_par.source == 'A' ? "ADF":"FB"), sc_info.ImgStatus[dup].IsBlank ? "Blank" : (sc_info.ImgStatus[dup].IsColor ? "Color":"Gray"), img->dpi.x, img->dpi.y, PageNum[dup]/*sc_info.PageNum[dup]+1*/, 'A'+dup, img->format==I3('JPG')? "JPG":"TIF");
				  
				if(sc_par.acquire & ACQ_DETECT_COLOR) {
					if(sc_par.img.bit >= 24) {
						if(sc_info.ImgStatus[dup].IsColor == 0) {
							ImgFile[dup].img.bit = sc_par.img.bit/3;
						}
						else {
							ImgFile[dup].img.bit = sc_par.img.bit;
						}
					}
				}
              ImgFile_Open(&ImgFile[dup], ImgFileName);
			  printf("\n%s\n", ImgFileName);
            }
			ImgFile_Write(&ImgFile[dup], scanBuf, length);
			printf("%c", dup? 'B': 'A');
			
            if((length >= (int)sc_info.ValidPageSize[dup]) && sc_info.ImgStatus[dup].EndPage) {
			  //printf("\nSide %c CLOSED\n",  dup? 'B': 'A');
				/*if(sc_par.acquire & ACQ_DETECT_COLOR) {
					if(sc_par.img.bit >= 24) {
						if(sc_info.ImgStatus[dup].IsColor == 0) {
							ImgFile[dup].img->bit = sc_par.img.bit/3;
						}
						else {
							ImgFile[dup].img->bit = sc_par.img.bit;
						}
					}
				}*/
				ImgFile_Close(&ImgFile[dup], sc_info.ImageHeight[dup]);
              bFiling[dup]--;
			  printf("\n");
            }
			else if(cancel && bFiling[dup]) {
			  //printf("\nSide %c cancel CLOSED\n",  dup? 'B': 'A');
				/*if(sc_par.acquire & ACQ_DETECT_COLOR) {
					if(sc_par.img.bit >= 24) {
						if(sc_info.ImgStatus[dup].IsColor == 0) {
							ImgFile[dup].img->bit = sc_par.img.bit/3;
						}
						else {
							ImgFile[dup].img->bit = sc_par.img.bit;
						}
					}
				}*/
				ImgFile_Close(&ImgFile[dup], sc_info.ImageHeight[dup]);
			  bFiling[dup] = 0;
			}
          }
        }
		/*
        if(cancel && bFiling[dup]) {
		  printf("\nSide %c cancel CLOSED\n",  dup? 'B': 'A');
          ImgFile_Close(&ImgFile[dup], sc_info.ImageHeight[dup]);
          bFiling[dup] = 0;
        }
		*/
	  }//for
#endif
    }//while
    //time = GetTickCount() - time;


	
	//if(sc_info.Cancel || sc_info.CoverOpen || sc_info.PaperJam || sc_info.UltraSonic) {
		//_cancel();
	//}
	//else {
		_stop();
	//}
    //waitJobFinish(1, 0);
	time = GetTickCount() - time;

  }

JobEnd:
  
  _JobEnd();

  return result;
}

