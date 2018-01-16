
#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include "usbio.h"
#include "ScanCMD.h"
#include "model.h"
#include "EdgeDetect.h"
#include <time.h>

#define Show_Time_Cost
#define Save_LED_AFE_Profile
#define Save_Shading_Profile
#define Save_IMG_CHK_Profile

#define DARK_DATA_ORGANIZE_NEW
#define LED_SHUTTER_FAST_CAL

#ifdef Taiga

	#define K_ADF_ROLLER  //For Taiga

#else

	#define K_PRE_POST_FEED

#endif


//#define HT82V38
//#define WM8234
#ifdef Faroe
#define CALIBRATION_ALL
#endif
extern SC_PAR_T_ k_scan_par;
extern U8 SCAN_DOC_SIZE;
extern int bSaveFile;
extern int K_BatchNum;
extern int K_PageNum;


int bCalibrationMode = 1; //0: Shading only, 1:Full(AFE+LED+Shading)
int bCalibration = 0;
U16 *K_img[2];  // pointer to K_img_buf image buffer
//U8 K_img_buf[2][0x140000];  // 2 x 1.25 MB  // 300dpi: 80lines, 600dpi: 40 lines, 1200dpi: 20 lines
//U8 K_img_buf[2][0x500000];
U8 K_img_buf[2][0x3200000];		//2 x 50 MB for prefeed/postfeed scan buf 
U32 K_img_size[2];	//Currently K image data size
U8 K_shad16_data[2][184*1024]; // 2 x 184KB  // 1200dpi one line (13" max)
U8 K_shad_data[2][184*1024];
#define K_LINES 48
U32 K_lines;

CALIBRATION_CAP_T_ K_Cap;
CALIBRATION_SET_T_ K_Set;

#define CALIBRATION_16BIT_DARK

//-------- Calibration target define------------
//--------AFE offset cal---------
#define CAL_AFE_DARK		(8 * 0x100)	//color level
#define CAL_AFE_DARK_THD	(2 * 0x100)	//color level
#define CAL_AFE_DARK_ABORT	10	//loop counts

//--------LED exposure time cal---------
#define CAL_EXP_WHITE		(175 * 0x100)	//color level
#define CAL_EXP_THD			(5  * 0x100)	//color level
#define CAL_EXP_ABORT		30	//loop counts
#define CAL_EXP_MINUS_C		15	//Pixel count
#define CAL_EXP_MINUS_G		5


#define CAL_AFE_WHITE		(175 * 0x100)	//color level


#ifdef WM8234
#define AFE_MINUS			0.3	// V/V
#endif

#ifdef HT82V38
//Special, not linear
#endif


#define DARK_DROP			IMG_K_PRUN_300_DOT_Y/4		
#define WHITE_DROP			IMG_K_PRUN_300_DOT_Y/4	

#if K_ADF_ROLLER

#define CAL_SHADING_WHITE_A_R	(233 * 0x100)
#define CAL_SHADING_WHITE_A_G	(242 * 0x100)
#define CAL_SHADING_WHITE_A_B	(243 * 0x100)  //20161122

#define CAL_SHADING_WHITE_B_R	(233 * 0x100)
#define CAL_SHADING_WHITE_B_G	(242 * 0x100)
#define CAL_SHADING_WHITE_B_B	(243 * 0x100)

#else
/*
#define CAL_SHADING_WHITE_A_R	(200 * 0x100)
#define CAL_SHADING_WHITE_A_G	(209 * 0x100)
#define CAL_SHADING_WHITE_A_B	(210 * 0x100)

#define CAL_SHADING_WHITE_B_R	(200 * 0x100)
#define CAL_SHADING_WHITE_B_G	(209 * 0x100)
#define CAL_SHADING_WHITE_B_B	(210 * 0x100)
*/
#define SHD_WHITE_TARGET_A_R_TYPE1	(233 * 1.078 * 0x100)
#define SHD_WHITE_TARGET_A_G_TYPE1	(230 * 1.078 * 0x100)
#define SHD_WHITE_TARGET_A_B_TYPE1	(223 * 1.12 * 0x100)

#define SHD_WHITE_TARGET_B_R_TYPE1	(233 * 1.078 * 0x100)
#define SHD_WHITE_TARGET_B_G_TYPE1	(230 * 1.078 * 0x100)
#define SHD_WHITE_TARGET_B_B_TYPE1	(223 * 1.12 * 0x100)

#define SHD_WHITE_TARGET_A_R_TYPE2	(200 * 0x100)
#define SHD_WHITE_TARGET_A_G_TYPE2	(209 * 0x100)
#define SHD_WHITE_TARGET_A_B_TYPE2	(210 * 0x100)

#define SHD_WHITE_TARGET_B_R_TYPE2	(200 * 0x100)
#define SHD_WHITE_TARGET_B_G_TYPE2	(209 * 0x100)
#define SHD_WHITE_TARGET_B_B_TYPE2	(210 * 0x100)

#endif

//--------------------
U16 SHD_WHITE_TARGET[2][3]; //For A/B side white shading target R/G/B


//-------------------
U8 AFE_OFFSET_ABORT=0;
U8 EXP_ABORT=0;


U32 user_param(U32 action);
int Scan_Param(void);
int Scan_Cap_Calibration(CALIBRATION_CAP_T_ *cap);
int Scan_Shad_Calibration(CALIBRATION_SET_T_ *set);
int job_Scan(void);
//int job_Wait(int job, int wait_motor_stop);
int Scan_Shad_Shading(int side, int channel, void *buf, int length);
int Scan_Shad_Flash(void *buf, int length);
int job_LoadPaper(U32 source, int length);
int job_EjectPaper(U32 source, int length);
int job_ResetHome(U32 source, int length);
int job_Calibration(SC_PAR_T_ *_par);
//int ini_get_shading_dump();

void Delete_Cal_Files(void)
{
	remove("*.csv");
}

void Save_LED_AFE(U8 data_type, SC_PAR_T_ *par, U32 *data, U8 dup)
{
	FILE *fcsv;
	char fcsvName[80];

	sprintf(fcsvName, "%c%c%d%c_led_afe.csv", (U8)par->source, (par->img.bit>=24)?'C':'G', par->img.dpi.x, 'A'+dup);
	fcsv = fopen(fcsvName, "a");
	//fcsv = fopen(fcsvName, "wb");

	if(!fcsv)
		printf("can't open file shading csv!!\n");
	else{
		switch(data_type) {
			case 0:
				//Save abort log
				fprintf(fcsv, "ABORT\n");
				break;

			case 1:
				//Save LED data
				fprintf(fcsv, "led_r, led_g, led_b\n");
				fprintf(fcsv, "%d, %d, %d\n", data[0], data[1], data[2]);
				break;

			case 2:
				//Save AFE offset data
				{
				S16 *tmp_data = (S16*)data;
				fprintf(fcsv, "afe_offset_1, afe_offset_2, afe_offset_3, afe_offset_4, afe_offset_5, afe_offset_6\n");
				fprintf(fcsv, "%d, %d, %d, %d, %d, %d\n", tmp_data[0], tmp_data[1], tmp_data[2], tmp_data[3], tmp_data[4], tmp_data[5]);
				}
				break;

			case 3:
				//Save AFE gain data
				{
				U16 *tmp_data = (U16*)data;
				fprintf(fcsv, "afe_gain_1, afe_gain_2, afe_gain_3, afe_gain_4, afe_gain_5, afe_gain_6\n");
				fprintf(fcsv, "%d, %d, %d, %d, %d, %d\n", tmp_data[0], tmp_data[1], tmp_data[2], tmp_data[3], tmp_data[4], tmp_data[5]);
				}
				break;
		}

		fclose(fcsv);
	}
}

void Save_Shading(SC_PAR_T_ *par, U16 *img_buf, U32 *shd_buf, U32 gain, U8 dup)
{
	FILE *fcsv;
	char fcsvName[80];
	U32 i,dot_x;
	U8 color_loop = (par->img.mono == IMG_COLOR) ? 3 : 1;

	dot_x = par->img.width;

	sprintf(fcsvName, "%c%c%d%c_shd_%s.csv", (U8)par->source, (par->img.bit>=24)?'C':'G', par->img.dpi.x, 'A'+dup, (par->acquire & ACQ_LAMP_OFF) ? "offset" : "gain" );
	fcsv = fopen(fcsvName, "wb");

	if(!fcsv)
		printf("can't open file shading csv!!\n");
	else{
		if(par->acquire & ACQ_LAMP_OFF) {
#ifdef DARK_DATA_ORGANIZE_NEW
			/*Offset*/
			fprintf(fcsv, "dg, offset_r, offset_g, offset_b\n");
			for(i=0;i<dot_x;i++) {
				if(par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d, %d\n",
							img_buf[i], shd_buf[i]-gain, shd_buf[i+dot_x]-gain, shd_buf[i+dot_x*2]-gain
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d, %d\n", img_buf[i], shd_buf[i]-gain);
				}
			}
#else
			/*Offset*/
			fprintf(fcsv, "dr, dg, db, offset_r, offset_g, offset_b\n");
			for(i=0;i<dot_x;i++) {
				if(par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d, %d, %d, %d\n",
							img_buf[i*3], img_buf[i*3+1], img_buf[i*3+2],
							shd_buf[i]-gain, shd_buf[i+dot_x]-gain, shd_buf[i+dot_x*2]-gain
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d, %d\n", img_buf[i], shd_buf[i]-gain);
				}
			}
#endif
		}
		else {
			/*Gain*/
			fprintf(fcsv, "wr, wg, wb, gainr, gaing, gainb\n");
			for(i=0;i<dot_x;i++) {
				if(par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d, %f, %f, %f\n",
							img_buf[i*3], img_buf[i*3+1], img_buf[i*3+2],
							((float)(shd_buf[i] >> 16)/((float)gain)), 
							((float)(shd_buf[i+dot_x] >> 16)/(float)gain), 
							((float)(shd_buf[i+dot_x*2] >> 16)/(float)gain)
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d, %f\n", img_buf[i], ((float)(shd_buf[i] >> 16)/(float)gain));
				}
			}
		}

		fclose(fcsv);
	}
}


void Save_img(SC_PAR_T_ *par, U16 *img_buf, U8 dup)
{
	FILE *fcsv;
	char fcsvName[80];
	U32 i,dot_x;
	U8 color_loop = (par->img.mono == IMG_COLOR) ? 3 : 1;

	dot_x = par->img.width;

	sprintf(fcsvName, "%c%c%d%c_img_%s.csv", (U8)par->source, (par->img.bit>=24)?'C':'G', par->img.dpi.x, 'A'+dup, (par->acquire & ACQ_LAMP_OFF) ? "offset" : "gain" );
	fcsv = fopen(fcsvName, "wb");

	if(!fcsv)
		printf("can't open file shading csv!!\n");
	else{
		if(par->acquire & ACQ_LAMP_OFF) {
			/*Offset*/
#ifdef DARK_DATA_ORGANIZE_NEW
			fprintf(fcsv, "dg\n");
			for(i=0;i<dot_x;i++) {
				if(par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d\n", img_buf[i]);
				}
				else {
					//Mono
					fprintf(fcsv, "%d\n", img_buf[i]);
				}
			}
#else
			fprintf(fcsv, "dr, dg, db\n");
			for(i=0;i<dot_x;i++) {
				if(par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d\n",
							img_buf[i*3], img_buf[i*3+1], img_buf[i*3+2]
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d\n", img_buf[i]);
				}
			}
#endif
		}
		else {
			/*Gain*/
			fprintf(fcsv, "wr, wg, wb\n");
			for(i=0;i<dot_x;i++) {
				if(par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d\n",
							img_buf[i*3], img_buf[i*3+1], img_buf[i*3+2]
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d\n", img_buf[i]);
				}
			}
		}

		
		fclose(fcsv);
	}
}

#if 0
int cal_img_buf_create(unsigned int byte_size)
{
	K_img_buf[0] = (U8*)malloc(byte_size);
	if(K_img_buf[0] == NULL) {
		printf("K_img_buf[0] buffer create fail!\n");
		return FALSE;
	}

	K_img_buf[1] = (U8*)malloc(byte_size);
	if(K_img_buf[1] == NULL) {
		printf("K_img_buf[1] buffer create fail!\n");
		return FALSE;
	}

	return TRUE;
}

int cal_img_buf_free(void)
{
	free(K_img_buf[0]);
	free(K_img_buf[1]);
	return TRUE;
}
#endif

int cal_img_buf_store(int dup, void *img, int size)
{
  if(!bCalibration)
    return FALSE;
  if(img) {
    int buf_size = sizeof(K_img_buf)/2 - ((U32)K_img[dup] - (U32)K_img_buf[dup]);
    if(size > buf_size) {
      printf("Calibration buffer too small.\n");
      return FALSE;
    }
    memcpy(K_img[dup], img, size);
    K_img[dup] += (size/sizeof(U16));
	K_img_size[dup] += size;
  }
  else {  // reset memory pointer
    K_img[0] = (U16*)K_img_buf[0];
    K_img[1] = (U16*)K_img_buf[1];
	K_img_size[0] = 0;
	K_img_size[1] = 0;
  }
  return TRUE;
}

__inline U16 _cal_average_data(U16 *data, int next, int num)
{
  U16 *last_data = data + next*num;
  U32 sum;
  for(sum = 0; data < last_data; data += next)
    sum += *data;
  return (U16)(sum/num);
}

__inline U16 _cal_min_data(U16 *data, int next, int num)
{
  U16 *last_data = data + next*num;
  U16 min;
  for(min = 0xffff; data < last_data; data += next) {
    if(min > *data)
      min = *data;
  }
  return min;
}

//計算後仍維持 raw data R,G,B 順序
void _cal_average_iterate(U16 *data, int num_x, int num_y)
{
  U16 *last_data;
  for(last_data = data + num_x; data < last_data; data++)
    *data = _cal_average_data(data, num_x, num_y);
}

void _cal_average_iterate2(U16 *data, int num_x, int num_y, int channel)
{
  U32 i;
  U32 j = (channel == 0) ? 1:3;
  U32 k; 
  U32 offset = (channel < 2) ? 0: (channel-1);

  //for(i = 0; i < num_x; i+=j, data++)
  //  *data = _cal_min_data(&data[offset + i], num_x, num_y);
  for(i = 0, k = 0; i < num_x; i+=j, k++)
    data[k] = _cal_average_data(&data[offset + i], num_x, num_y);
}

void _cal_min_iterate(U16 *data, int num_x, int num_y, int channel)
{
#if 0
  U32 i;
  U32 j = (channel == 0) ? 1:2;
  U32 offset = (channel < 2) ? 0: (channel-1);

  for(i = offset; i < num_x; i+=j, data++)
    *data = _cal_min_data(data + i, num_x, num_y);
#else
  U32 i;
  U32 j = (channel == 0) ? 1:3;
  U32 k; 
  U32 offset = (channel < 2) ? 0: (channel-1);

  //for(i = 0; i < num_x; i+=j, data++)
  //  *data = _cal_min_data(&data[offset + i], num_x, num_y);
  for(i = 0, k = 0; i < num_x; i+=j, k++)
    data[k] = _cal_min_data(&data[offset + i], num_x, num_y);
#endif
}


//============================
static int data[256];
//---------------------------------
void quicksort(int left, int right)
{ 
    int i, j, s, temp; 

    if(left < right) { 
        s = data[(left+right)/2]; 
        i = left - 1; 
        j = right + 1; 

        while(1) { 
            while(data[++i] < s) ;
            while(data[--j] > s) ;
            if(i >= j)
                break;
      temp = data[i];
      data[i] = data[j];
      data[j] = temp;
        } 

        quicksort(left, i-1);
        quicksort(j+1, right);
    } 
} 
//---------------------------------
int average_quicksort(unsigned short *input, int left, int right, int number, int offset)
{
  int i, result = 0;
  //offset /= sizeof(unsigned short);
  for(i = 0; i < number; i++, input+=offset)
    data[i] = *input;
  quicksort(0, number-1);
  for(i = left, number-=right; i < number; i++)  
    result += data[i];
  result /= (number-left);
  return result;
}
//============================

void _cal_ave_sort_iterate(U16 *data, int num_x, int num_y)
{
  U16 *last_data;
  //int left = num_y / 4;
  //int right = num_y / 4;
  int left = DARK_DROP;
  int right = WHITE_DROP;
  for(last_data = data + num_x; data < last_data; data++)
    *data = average_quicksort(data, left, right, num_y, num_x);
}


/*find min of *data*/
U16 _cal_find_min(U16 *data, int next, int num)
{
  U16 *last_data = data + next*num;
  U16 min;
  for(min = 0xffff; data < last_data; data += next) {
    if(min > *data)
      min = *data;
  }
  return min;
}


U16 _cal_find_max(U16 *data, int next, int num)
{
  U16 *last_data = data + next*num;
  U16 max;
  for(max = 0; data < last_data; data += next) {
    if(max < *data)
      max = *data;
  }
  return max;
}

void _cal_check_offset(S16 *offset, int channel, int max, int min)
{
  int i;
  for(i = 0; i < channel; i++) {
    if(offset[i] > max)
      offset[i] = max;
    else if(offset[i] < min)
      offset[i] = min;
  }
}

// calibration step 0: retrieve the capablility of CCD and AFE
int cal_set_def(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i;
  U32 *exp;
  S16 *offset;
  U16 *gain;
  U32 exp_def;
  S16 offset_def;
  U16 gain_def;  

  user_param(ACQ_CALIBRATION|ACQ_MOTOR_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);  //Park test
 
  if(!Scan_Param())
    return FALSE;
  if(!Scan_Cap_Calibration(cap))
    return FALSE;  

  //memset(set, 0, sizeof(CALIBRATION_SET_T_));
  //memset(set, 0, (sizeof(CALIBRATION_SET_T_) - sizeof(ME_SET_T))); //Not to write ME par
  for(i = 0; i < 2; i++) {
    //exp_def = cap->ccd[i].exp_max; //cap->ccd[i].exp_def;
    exp_def = cap->ccd[i].exp_def;
    exp = set->ccd[i].exp;

    exp[0] = exp[1] = exp[2] = exp_def;

    //offset_def = cap->afe[i].offset_max/2; //cap->afe[i].offset_def;
	offset_def = cap->afe[i].offset_def;
    offset = set->afe[i].offset;
    offset[0] = offset[1] = offset[2] = offset[3] = offset[4] = offset[5] = offset_def;
    //gain_def = cap->afe[i].gain_min; //cap->afe[i].gain_def;
	gain_def = cap->afe[i].gain_def;
    if(gain_def < 1000)
      gain_def = 1000;
    gain = set->afe[i].gain;
    gain[0] = gain[1] = gain[2] = gain[3] = gain[4] = gain[5] = gain_def;
	set->shd[i].mono = (k_scan_par.img.mono? (cap->ccd[i].mono): 0);
  }

  return TRUE;
}

extern int Scan_Set_Calibration(CALIBRATION_CAP_T_ *cap);
int cal_set_def_shading_only(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i;
  U32 *exp;
  S16 *offset;
  U16 *gain;
  U32 exp_def;
  S16 offset_def;
  U16 gain_def;  

  user_param(ACQ_MOTOR_OFF|ACQ_NO_MIRROR);  //Park test
 
  if(!Scan_Param())
    return FALSE;
  if(!Scan_Cap_Calibration(cap))
    return FALSE; 
  if(!Scan_Set_Calibration((CALIBRATION_CAP_T_ *)set))
    return FALSE;  


  //memset(set, 0, sizeof(CALIBRATION_SET_T_));

  for(i = 0; i < 2; i++) {
    //exp_def = cap->ccd[i].exp_max; //cap->ccd[i].exp_def;
    exp_def = cap->ccd[i].exp_def;
    exp = set->ccd[i].exp;
    exp[0] = exp[1] = exp[2] = exp_def;
    //offset_def = cap->afe[i].offset_max/2; //cap->afe[i].offset_def;
	offset_def = cap->afe[i].offset_def;
    offset = set->afe[i].offset;
    offset[0] = offset[1] = offset[2] = offset[3] = offset[4] = offset[5] = offset_def;
    //gain_def = cap->afe[i].gain_min; //cap->afe[i].gain_def;
	gain_def = cap->afe[i].gain_def;
    if(gain_def < 1000)
      gain_def = 1000;
    gain = set->afe[i].gain;
    gain[0] = gain[1] = gain[2] = gain[3] = gain[4] = gain[5] = gain_def;
	set->shd[i].mono = (k_scan_par.img.mono? (cap->ccd[i].mono): 0);
  }

  return TRUE;
}


int cal_AFE_offset(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j, seg;
  int color_loop = (k_scan_par.img.mono == 4) ? 1 : 3;
  U32 dot, seg_dot;
  U16 SEG_AFE_DARK;
  S16 *offset;
  U16 *gain, *buf;
  U16 one_channel_cal = 2; //0:Color, 1:R, 2:G, 3:B
  U8 SIDE_K[2]={0, 0};
  U8 CYCLE_COUNT=0;
  U8 TMP_NOT_OK=0;


  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;


  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_MOTOR_OFF|ACQ_LAMP_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);

AFE_OFFSET_CHK:

  if(!Scan_Param())
    return FALSE;
  
  cal_img_buf_store(0, 0, 0);  // reset image buffer pointer
  if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;

  for(i = 0; i < 2; i++) {

	if(SIDE_K[i] == 0)
		continue;

	//Save AFE offset profile
#ifdef Save_LED_AFE_Profile
	Save_LED_AFE(2, &k_scan_par, (unsigned int*)set->afe[i].offset, i);
#endif

    seg = (cap->ccd[i].type == I4('CIS6'))? 6: ((cap->ccd[i].type == I4('CIS3'))? 3: 1);
    buf = (U16*)K_img_buf[i];
    dot = cap->ccd[i].dot;
    seg_dot = dot / seg;
    offset = set->afe[i].offset;
    gain = set->afe[i].gain;

#ifdef DARK_DATA_ORGANIZE_NEW
	_cal_min_iterate(buf, dot * color_loop, k_scan_par.img.height, (color_loop==3) ? one_channel_cal : 0);
#else
	_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);
#endif

		//Image profile
#ifdef Save_IMG_CHK_Profile
	Save_img(&k_scan_par, buf, i);
#endif
	
    if(seg > 1) {
	  TMP_NOT_OK = 0;
      for(j = 0; j < seg; j++) {
#ifdef DARK_DATA_ORGANIZE_NEW
		  if(one_channel_cal)
			SEG_AFE_DARK = _cal_find_min(&buf[seg_dot*j], 1, seg_dot);
		  else
		    SEG_AFE_DARK = _cal_find_min(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop);
#else
		  SEG_AFE_DARK = _cal_find_min(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop);
#endif
		  if((SEG_AFE_DARK < (CAL_AFE_DARK - CAL_AFE_DARK_THD)) 
			  || (SEG_AFE_DARK > (CAL_AFE_DARK + CAL_AFE_DARK_THD))) {
			TMP_NOT_OK = 1;
			offset[j] += 1000*((S16)CAL_AFE_DARK - SEG_AFE_DARK)/gain[j];
		  }
      }
	  if(!TMP_NOT_OK) {
		  SIDE_K[i] = 0;
	  }
	  else {
		CYCLE_COUNT++;
	  }
      
    }
    else {
      for(j = 0; j < color_loop; j++)
        offset[j] -= 1000*_cal_average_data(&buf[j], color_loop, dot)/gain[j];
      for(; j < 3; j++)
        offset[j] = offset[0] * gain[0] / gain[j];
    }
    _cal_check_offset(offset, j, cap->afe[i].offset_max, cap->afe[i].offset_min);
  }

  if( !(SIDE_K[0]|SIDE_K[1]) || (CYCLE_COUNT == (CAL_AFE_DARK_ABORT+1))) {
	if(CYCLE_COUNT == (CAL_AFE_DARK_ABORT+1)) {
	  AFE_OFFSET_ABORT = 1;
	  //return FALSE;
		return TRUE;
	}
	goto AFE_OFFSET_OK;
  }
  else {
	goto AFE_OFFSET_CHK;
  }


AFE_OFFSET_OK:

  return TRUE;
}



void _cal_check_exposure_time(U32 *exp, int color, int max, int min)
{
  int i;
  for (i = 0; i < color; i++) {
    if((int)exp[i] > max)
      exp[i] = max;
    else if((int)exp[i] < min)
      exp[i] = min;
  }
}

#if 0	//Origin

int cal_exposure_time(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j, k;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U32 *exp, dot;
  U16 white[3], *buf;
  U16 white_min[2]={0, 0};

  U8 SIDE_K[2]={0, 0};
  U8 CYCLE_COUNT[2]={0, 0};
  U8 TMP_NOT_OK=0;

  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  user_param(ACQ_CALIBRATION|ACQ_MOTOR_OFF|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);  //Park test

EXP_CHK:

	  if(!Scan_Param())
	  return FALSE;
	  //Sleep(200);
	  cal_img_buf_store(0, 0, 0);
	  if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
		return FALSE;


	  for (i = 0; i < 2; i++) {
		if(SIDE_K[i] == 0)
			continue;

		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		exp = set->ccd[i].exp;

		//_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);
		_cal_ave_sort_iterate(buf, dot * color_loop, k_scan_par.img.height);

		for(j = 0; j < color_loop; j++) {
		  white[j] = _cal_find_max(&buf[j], color_loop, dot);
		  //white[j] = _cal_find_max(&buf[j], color_loop, dot*k_scan_par.img.height);
		  //white[j] = _cal_average_data(&buf[j], color_loop, dot);
		  //printf("white[%d] = %d\n", j, white[j]/256);
		}

		if(CYCLE_COUNT[i] == 0) {
			white_min[i] = _cal_find_min(white, 1, color_loop);

			white_min[i] = (white_min[i] > CAL_EXP_WHITE) ? CAL_EXP_WHITE:white_min[i];
		}

		TMP_NOT_OK = 0;
		for(j = 0; j < color_loop; j++) {

			if(white[j] < (white_min[i] - CAL_EXP_THD)) {
				TMP_NOT_OK = 1;
				//exp[j] = exp[j] * white_min / white[j];
				if(k_scan_par.img.mono)
					exp[j] += CAL_EXP_MINUS_G; //park test
					//exp[j] += (CAL_EXP_MINUS*5);
				else
					exp[j] += CAL_EXP_MINUS_C;
			}
			else if(white[j] > (white_min[i] + CAL_EXP_THD)) {
				TMP_NOT_OK = 1;
				if(k_scan_par.img.mono)
					exp[j] -= CAL_EXP_MINUS_G; //park test
					//exp[j] -= (CAL_EXP_MINUS*5);
				else
					exp[j] -= CAL_EXP_MINUS_C;
			}
		}

		if(!TMP_NOT_OK) {
			SIDE_K[i] = 0;
		}
		else {
			CYCLE_COUNT[i]++;
		}

		for(;j < 3; j++)
			  exp[j] = exp[0];


		_cal_check_exposure_time(exp, j, cap->ccd[i].exp_max, cap->ccd[i].exp_min);

	  }

 

  if( !(SIDE_K[0]|SIDE_K[1]) || (CYCLE_COUNT[0] == (CAL_EXP_ABORT+1)) || (CYCLE_COUNT[1] == (CAL_EXP_ABORT+1))) {
	if((CYCLE_COUNT[0] == (CAL_EXP_ABORT+1)) || CYCLE_COUNT[1] == (CAL_EXP_ABORT+1)) {
	  EXP_ABORT = 1;
	}
	goto EXP_OK;
  }
  else {
	goto EXP_CHK;
  }

EXP_OK:

  return TRUE;
}

#else	//New 20170222

int cal_exposure_time(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j, k;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U32 *exp, dot;
  U32 exp_max, exp_min, exp_tmp, exp_last[2][3] = {cap->ccd[0].exp_def};
  U16 white[3], white_cross[2][3][2] = {0}, *buf;
  U16 white_target = CAL_EXP_WHITE;
  U16 white_target_thd = CAL_EXP_THD;
  U16 white_min[2]={0, 0};

  U8 SIDE_K[2]={0, 0};
  U8 CYCLE_COUNT[2]={0, 0};
  U8 TMP_NOT_OK=0;

  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  user_param(ACQ_CALIBRATION|ACQ_MOTOR_OFF|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);  //Park test


EXP_CHK:
		
	  if(!Scan_Param())
	  return FALSE;
	  //Sleep(200);
	  cal_img_buf_store(0, 0, 0);
	  if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
		return FALSE;


	  for (i = 0; i < 2; i++) {
		if(SIDE_K[i] == 0)
			continue;

	#ifdef Save_LED_AFE_Profile
		Save_LED_AFE(1, &k_scan_par, set->ccd[i].exp, i);
	#endif

		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		exp = set->ccd[i].exp;
		exp_max = cap->ccd[i].exp_max;
		exp_min = cap->ccd[i].exp_min;

		_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);

		for(j = 0; j < color_loop; j++) {
		  //white[j] = _cal_find_max(&buf[j], color_loop, dot);
		  white[j] = _cal_average_data(&buf[j], color_loop, dot);
		}

		TMP_NOT_OK = 0;
		for(j = 0; j < color_loop; j++) {

			exp_tmp = exp[j];

			if(white[j] < (white_target - white_target_thd)) {
				TMP_NOT_OK = 1;

				if(exp[j] >= exp_max) {
					exp[j] = exp_max;
					white_target = white[j];
					continue;
				}
				
#ifdef LED_SHUTTER_FAST_CAL
				white_cross[i][j][0]++;

				if((white_cross[i][j][0] < 2) || (white_cross[i][j][1] < 2)) {
					if(exp_last[i][j] > exp[j]) {
						if(k_scan_par.img.mono == 0) {
							exp[j] += (exp_last[i][j] - exp[j])/2;
						}
						else {
							exp[j] += (exp_last[i][j] - exp[j])/2/3;
						}
					}
					else if(exp_last[i][j] <= exp[j]) {
						if(k_scan_par.img.mono == 0) {
							exp[j] += (exp_max - exp[j])/2;
						}
						else {
							exp[j] += (exp_max - exp[j])/2/3;
						}
					}

					exp_last[i][j] = exp_tmp;
				}
				else {
					if(k_scan_par.img.mono)
						exp[j] += CAL_EXP_MINUS_G;
					else
						exp[j] += CAL_EXP_MINUS_C;
				}



#else
				if(k_scan_par.img.mono)
					exp[j] += CAL_EXP_MINUS_G; //park test
				else
					exp[j] += CAL_EXP_MINUS_C;

#endif

				if(exp[j] >= exp_max) {
					exp[j] = exp_max;
				}

				_cal_check_exposure_time(exp, j, cap->ccd[i].exp_max, cap->ccd[i].exp_min);
			}
			else if(white[j] > (white_target + white_target_thd)) {
				TMP_NOT_OK = 1;

				if(exp[j] <= 0) {
					printf("Shutter time fail: Exposure minimum > white target.\n");
					return FALSE;
				}

#ifdef LED_SHUTTER_FAST_CAL
				white_cross[i][j][1]++;
				
				if((white_cross[i][j][0] < 2) || (white_cross[i][j][1] < 2)) {
					if(exp_last[i][j] >= exp[j]) {
						if(k_scan_par.img.mono == 0) {
							exp[j] -= (exp[j] - exp_min)/2;
						}
						else {
							exp[j] -= (exp[j] - exp_min)/2/3;
						}
					}
					else if(exp_last[i][j] < exp[j]) {
						if(k_scan_par.img.mono == 0) {
							exp[j] -= (exp[j] - exp_last[i][j])/2;
						}
						else {
							exp[j] -= (exp[j] - exp_last[i][j])/2/3;
						}
					}

					exp_last[i][j] = exp_tmp;
				}
				else {
					if(k_scan_par.img.mono)
						exp[j] -= CAL_EXP_MINUS_G;
					else
						exp[j] -= CAL_EXP_MINUS_C;
				}
			
#else
				if(k_scan_par.img.mono)
					exp[j] -= CAL_EXP_MINUS_G; //park test
				else
					exp[j] -= CAL_EXP_MINUS_C;

				if(exp[j] <= cap->ccd[i].exp_min) {
					exp[j] = cap->ccd[i].exp_min;
					printf("Shutter time fail: Exposure minimum > white target.\n");
					return FALSE;
				}
#endif

				if(exp[j] <= 0) {
					printf("Shutter time fail: Exposure minimum > white target.\n");
					return FALSE;
				}

				_cal_check_exposure_time(exp, j, cap->ccd[i].exp_max, /*cap->ccd[i].exp_min*/0);
			}
			else {
				//Current shutter time OK

			}
		}

		if(!TMP_NOT_OK) {
			SIDE_K[i] = 0;
		}
		else {
			CYCLE_COUNT[i]++;
		}

		for(;j < 3; j++)
			  exp[j] = exp[0];

	  }

 

  if( !(SIDE_K[0]|SIDE_K[1]) || (CYCLE_COUNT[0] == (CAL_EXP_ABORT+1)) || (CYCLE_COUNT[1] == (CAL_EXP_ABORT+1))) {
	if((CYCLE_COUNT[0] == (CAL_EXP_ABORT+1)) || CYCLE_COUNT[1] == (CAL_EXP_ABORT+1)) {
	  EXP_ABORT = 1;
	  //return FALSE;
		return TRUE;
	}
	goto EXP_OK;
  }
  else {
	goto EXP_CHK;
  }

EXP_OK:

  return TRUE;
}

#endif

int cal_exposure_balance(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j, k;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U32 *exp, dot;
  U16 white[3], *buf;
  U16 white_min[2]={0, 0};

  U8 SIDE_K[2]={0, 0};
  U8 CYCLE_COUNT[2]={0, 0};
  U8 TMP_NOT_OK=0;

  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  user_param(ACQ_CALIBRATION|ACQ_MOTOR_OFF|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);  //Park test

EXP_CHK:

	  if(!Scan_Param())
	  return FALSE;
	  Sleep(200);
	  cal_img_buf_store(0, 0, 0);
	  if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
		return FALSE;


	  for (i = 0; i < 2; i++) {
		if(SIDE_K[i]==0)
			continue;

	#ifdef Save_LED_AFE_Profile
		Save_LED_AFE(1, &k_scan_par, set->ccd[i].exp, i);
	#endif

		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		exp = set->ccd[i].exp;

		_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);

		for(j = 0; j < color_loop; j++)
		  //white[j] = _cal_find_max(&buf[j], color, dot);
		  white[j] = _cal_average_data(&buf[j], color_loop, dot);

		if(CYCLE_COUNT[i] == 0) {
			white_min[i] = _cal_find_min(white, 1, color_loop);
	  }

		TMP_NOT_OK = 0;
		for(j = 0; j < color_loop; j++) {
			if(white[j] < (white_min[i] - CAL_EXP_THD)) {
				TMP_NOT_OK = 1;

				if(k_scan_par.img.mono)
					exp[j] += CAL_EXP_MINUS_G;
				else
					exp[j] += CAL_EXP_MINUS_C;

			}
			else if(white[j] > (white_min[i] + CAL_EXP_THD)) {
				TMP_NOT_OK = 1;

				if(k_scan_par.img.mono)
					exp[j] -= CAL_EXP_MINUS_G;
				else
					exp[j] -= CAL_EXP_MINUS_C;

			}
		}

		if(!TMP_NOT_OK) {
			SIDE_K[i] = 0;
		}
		else {
			CYCLE_COUNT[i]++;
		}

		for(;j < 3; j++)
			  exp[j] = exp[0];


		_cal_check_exposure_time(exp, j, cap->ccd[i].exp_max, cap->ccd[i].exp_min);
	  }
 

  if( !(SIDE_K[0]|SIDE_K[1]) || (CYCLE_COUNT[0] == (CAL_EXP_ABORT+1)) || (CYCLE_COUNT[1] == (CAL_EXP_ABORT+1))) {
	if((CYCLE_COUNT[0] == (CAL_EXP_ABORT+1)) || CYCLE_COUNT[1] == (CAL_EXP_ABORT+1)) {
	  EXP_ABORT = 1;
	  //return FALSE;
		return TRUE;
	}
	goto EXP_OK;
  }
  else {
	goto EXP_CHK;
  }

EXP_OK:

  return TRUE;
}



void _cal_check_gain(U16 *gain, int channel, int max, int min)
{
  int i;
  for(i = 0; i < channel; i++) {
    if(gain[i] > max)
      gain[i] = max;
    else if(gain[i] < min)
      gain[i] = min;
  }
}

int cal_prefeed(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
	U8 i = 0;
	U8 TMP_DOC_SIZE = 0;
	SC_PAR_T_ tmp_scan_par;
	U32 width=0, height=0;
	U16 *buf;
	int leadingEdge = 0, leftEdge = 0, rightEdge = 0, isSideB = 0;

	TMP_DOC_SIZE = SCAN_DOC_SIZE;
	memcpy(&tmp_scan_par, &k_scan_par, sizeof(SC_PAR_T_));


	SCAN_DOC_SIZE = DOC_K_PREFEED;

	k_scan_par.source = I3('ADF');
	k_scan_par.duplex = 3;
	k_scan_par.img.format = I3('RAW');
	k_scan_par.img.bit = IMG_24_BIT;
	k_scan_par.img.mono = 0;
	k_scan_par.img.dpi.x = 300;
	k_scan_par.img.dpi.y =300;


	cal_set_def(cap, set);

	for(i = 0; i < 2; i++) {
		set->ccd[i].exp[0] = set->ccd[i].exp[1] = set->ccd[i].exp[2] = cap->ccd[i].exp_max;
		set->afe[i].gain[0] = set->afe[i].gain[1] = set->afe[i].gain[2] = set->afe[i].gain[3] = set->afe[i].gain[4] = set->afe[i].gain[5] = cap->afe[i].gain_max;
	}

	set->me.prefeed = cap->me.prefeed;

#ifndef K_ADF_ROLLER

	user_param(ACQ_CALIBRATION|ACQ_NO_MIRROR|ACQ_NO_SHADING);
	if(!Scan_Param())
		return FALSE;

	cal_img_buf_store(0, 0, 0);
	if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
		return FALSE;

	width = k_scan_par.img.width;

	buf = (U16*)K_img_buf[0]; //Calculate for side A
	height = K_img_size[0]/(k_scan_par.img.mono == IMG_COLOR ? 3 : 1)/k_scan_par.img.width;

	//EdgeDetectColor8(buf, IMG_K_PREFEED_300_DOT_X, IMG_K_PREFEED_300_DOT_Y, &leadingEdge, &leftEdge, &rightEdge, isSideB);
	EdgeDetect8((unsigned char *)buf, width, height, &leadingEdge, &leftEdge, &rightEdge, (k_scan_par.img.mono == IMG_COLOR) ? 3:1, isSideB);


	set->me.prefeed = 100*leadingEdge/k_scan_par.img.dpi.y;

#endif

	SCAN_DOC_SIZE = TMP_DOC_SIZE;
	memcpy(&k_scan_par, &tmp_scan_par, sizeof(SC_PAR_T_));

	for(i = 0; i < 2; i++) {
		set->ccd[i].exp[0] = set->ccd[i].exp[1] = set->ccd[i].exp[2] = cap->ccd[i].exp_def;
		set->afe[i].gain[0] = set->afe[i].gain[1] = set->afe[i].gain[2] = set->afe[i].gain[3] = set->afe[i].gain[4] = set->afe[i].gain[5] = cap->afe[i].gain_def;
	}

	return TRUE;
}


int cal_postfeed(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
	int i, j;
	U16 *shad_data;
	U32 dot, dark_digit;
	U16 *buf;
	int trailingEdge = 0, isSideB = 0;

	U8 TMP_DOC_SIZE = 0;
	SC_PAR_T_ tmp_scan_par;

	TMP_DOC_SIZE = SCAN_DOC_SIZE;
	memcpy(&tmp_scan_par, &k_scan_par, sizeof(SC_PAR_T_));


	SCAN_DOC_SIZE = DOC_SIZE_FULL;
	
	k_scan_par.source = I3('ADF');
	k_scan_par.duplex = 3;
	k_scan_par.img.format = I3('RAW');
	k_scan_par.img.bit = IMG_24_BIT;
	k_scan_par.img.mono = 0;
	k_scan_par.img.dpi.x = 300;
	k_scan_par.img.dpi.y =300;

	cal_set_def(cap, set);

	for(i = 0; i < 2; i++) {
		set->ccd[i].exp[0] = set->ccd[i].exp[1] = set->ccd[i].exp[2] = cap->ccd[i].exp_max;
		set->afe[i].gain[0] = set->afe[i].gain[1] = set->afe[i].gain[2] = set->afe[i].gain[3] = set->afe[i].gain[4] = set->afe[i].gain[5] = cap->afe[i].gain_max;
	}

	set->me.postfeed = cap->me.postfeed;

#ifndef K_ADF_ROLLER

	user_param(ACQ_CALIBRATION|ACQ_NO_MIRROR|ACQ_NO_SHADING);
	
	if(!Scan_Param())  //Park test
    return FALSE;

	cal_img_buf_store(0, 0, 0);
	if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
		return FALSE;

	buf = (U16*)K_img_buf[0]; //Calculate for side A
	
	//printf("K_img_size[0] = %d\n", K_img_size[0]);
	//printf("K_img_size[0]/3/IMG_K_PREFEED_300_DOT_X = %d\n", K_img_size[0]/3/IMG_K_PREFEED_300_DOT_X);

	//EdgeDetectColor8Trailing(buf, IMG_K_PREFEED_300_DOT_X, K_img_size[0]/3/IMG_K_PREFEED_300_DOT_X, &trailingEdge, isSideB);
	EdgeDetect8Trailing((unsigned char *)buf, IMG_K_PREFEED_300_DOT_X, K_img_size[0]/3/IMG_K_PREFEED_300_DOT_X, &trailingEdge, (k_scan_par.img.mono == IMG_COLOR) ? 3:1, isSideB);

	//printf("trailingEdge = %d\n", trailingEdge);

	set->me.postfeed = cap->me.postfeed - 100*trailingEdge/k_scan_par.img.dpi.y;

#endif

	SCAN_DOC_SIZE = TMP_DOC_SIZE;
	memcpy(&k_scan_par, &tmp_scan_par, sizeof(SC_PAR_T_));

	return TRUE;
}


int cal_AFE_gain(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j, seg;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U32 dot, seg_dot;
  U16 *buf, *gain;
  float gain_cl=0;
  U16 tmp_max=0;

  U8 SIDE_K[2]={0, 0};


  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  user_param(ACQ_CALIBRATION|ACQ_MOTOR_OFF|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);  //Park test
  if(!Scan_Param())
    return FALSE;

  cal_img_buf_store(0, 0, 0);
  if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;

  for (i = 0; i < 2; i++) {
	if(SIDE_K[i] == 0)
		continue;

    seg = (cap->ccd[i].type == I4('CIS6'))? 6: ((cap->ccd[i].type == I4('CIS3'))? 3: 1);
    buf = (U16*)K_img_buf[i];
    dot = cap->ccd[i].dot;
    seg_dot = dot/seg;
    gain = set->afe[i].gain;
    _cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);
    if(seg > 1) {
      U32 new_gain = 0;
      for(j = 0; j < seg; j++) {
		  tmp_max = _cal_find_max(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop);
		  //gain[j] = gain[j]*( (float)CAL_AFE_WHITE/_cal_average_data(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop) );
		  //gain[j] = gain[j]*( (float)CAL_AFE_WHITE/_cal_find_max(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop) );
		  gain[j] = gain[j]*( (float)CAL_AFE_WHITE/tmp_max );
		  //printf("tmp_max = %d\n", tmp_max/256);
		  //printf("gain[%d] = %d\n", j, gain[j]);
      }
    }
    else {
	  for(j = 0; j < color_loop; j++) {
		//gain[j] = gain[j]*CAL_AFE_WHITE/_cal_average_data(&buf[j], color_loop, dot);
		  gain[j] = gain[j]*CAL_AFE_WHITE/_cal_find_max(&buf[j], color_loop, dot);
	  }
      for(; j < 3; j++)
        gain[j] = gain[0];
    }

	//Save LED profile
#ifdef Save_LED_AFE_Profile
	Save_LED_AFE(3, &k_scan_par, (unsigned int *)gain, i);
#endif

    _cal_check_gain(gain, j, cap->afe[i].gain_max, cap->afe[i].gain_min);
  }

  if(bSaveFile) {
	//#if 1  //Scan for final image check(debug)
    Scan_Param();
    cal_img_buf_store(0, 0, 0);
    if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
      return FALSE;
  //#endif
  }

  return TRUE;
}

void _cal_construct_dark16(U16 *data, U32 *shad, int next_data, int next_shad, int num, U32 gain)
{
  U16 *last_data = data + next_data*num;
  while(data < last_data) {
    *shad = gain + *data;
    data += next_data;
    shad += next_shad;
  }
}

#if 0
int _cal_set_dark_shift(SHD_SET_T *set, U32 dark_max, U32 dark_min)
{
  // dark_digit: 4, 6, 8, 16
  // dark_shift: 0 ~ 15

  int dark_shift, dark_digit, dark_range;

  for(dark_shift = 0; dark_min > 1; dark_shift++)
    dark_min >>= 1;
  for(dark_range = 0; dark_max > 0; dark_range++)
    dark_max >>= 1;
  dark_digit = dark_range - dark_shift;
  if(dark_digit <= 4) {
    dark_shift -= (4 - dark_digit);
    dark_digit = 4;
  }
  else if(dark_digit <= 6) {
    dark_shift -= (6 - dark_digit);
    dark_digit = 6;
  }
  else if(dark_digit <= 8)
  {
    dark_shift -= (8 - dark_digit);
    dark_digit = 8;
  }
  else {
    dark_shift = 0;
    dark_digit = 16;
  }
  set->dark_shift = (U8)dark_shift;
  set->dark_digit = (U8)dark_digit;
  return dark_shift;
}
#endif

void _cal_do_shift_dark(U32 *src, U16 *dst, int num, int dark_digit, int dark_shift)
{
  U32 *last_src;
  U32 data;
  U32 gain_mask = (0xffff >> dark_digit) << dark_digit;
  U32 dark_mask = (gain_mask ^ 0xffff) << dark_shift;

  if(dark_digit == 16) {
    if((U32)src != (U32)dst)
      memcpy(dst, src, num*4);
  }
  else {  
    for(last_src = src+num; src < last_src; src++, dst++) {
      data = *src;
      *dst = (U16)((data & dark_mask) >> dark_shift) | ((data >> 16) & gain_mask);
    }
  }
}



int cal_dark_shading(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U32 dot;
  U16 *buf, *shad_data/*, dark_min, dark_max*/;
  U32 gain, *dark_buf, dark_shift, dark_digit;
  U8 SIDE_K[2]={0, 0};
  U16 one_channel_cal = 2; //0:Color, 1:R, 2:G, 3:B


  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  if(k_scan_par.source == I3('ADF')) { //Park test
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_LAMP_OFF|ACQ_MOTOR_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);
  }
  else {
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_LAMP_OFF|ACQ_MOTOR_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);
	  //user_param(ACQ_CALIBRATION|ACQ_LAMP_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);
  }
  if(!Scan_Param())
    return FALSE;
  cal_img_buf_store(0, 0, 0);

  if(!Scan_Shad_Calibration(set) || !job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;

  for (i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
	  continue;

    buf = (U16*)K_img_buf[i];
    dot = cap->ccd[i].dot;
    dark_buf = (U32*)K_shad16_data[i];
    set->shd[i].gain_base = 8;  // default digital gain base
    gain = (0x10000 / set->shd[i].gain_base) << 16;

#ifdef DARK_DATA_ORGANIZE_NEW
	//_cal_min_iterate(buf, dot*color_loop, k_scan_par.img.height, one_channel_cal);
	_cal_average_iterate2(buf, dot*color_loop, k_scan_par.img.height, one_channel_cal);
#else
    _cal_average_iterate(buf, dot*color_loop, k_scan_par.img.height); 
#endif

#if 1	//Dark min protect check
	#ifdef DARK_DATA_ORGANIZE_NEW
	//if(SIDE_K[i] == 1) {
	if(_cal_find_min(buf, 1, (one_channel_cal == 0) ? (dot * color_loop) : dot ) == 0) {
				printf("Dark shading fail: Image dark min = 0!!!\n");
			//return FALSE;
			}
	//}
	#else
	//if(SIDE_K[i] == 1) {
		if(_cal_find_min(buf, 1, dot * color_loop) == 0) {
			printf("Dark shading fail: Image dark min = 0!!!\n");
			//return FALSE;
		}
	//}
	#endif

		#endif

	for(j = 0; j < color_loop; j++) {
#ifdef DARK_DATA_ORGANIZE_NEW
		_cal_construct_dark16(buf, &dark_buf[j*dot], 1, 1, dot, gain);
#else
		_cal_construct_dark16(&buf[j], &dark_buf[j*dot], color_loop, 1, dot, gain);
#endif
	}

	//Save shading profile
	#ifdef Save_Shading_Profile
		Save_Shading(&k_scan_par, buf, dark_buf, gain, i);
	#endif

    dark_shift = set->shd[i].dark_shift = 0;
    dark_digit = set->shd[i].dark_digit = 16;
    _cal_do_shift_dark(dark_buf, (U16*)K_shad_data[i], dot*color_loop, dark_digit, dark_shift);
  }
  

  if(bSaveFile) {
	//#if 1  //Scan for final image check(debug)
    Scan_Param();
    cal_img_buf_store(0, 0, 0);
    if(!Scan_Shad_Calibration(set))
      return FALSE;

    for(i = 0; i < 2; i++) {
		  if(SIDE_K[i] == 0)
            continue;
      dark_digit = set->shd[i].dark_digit;
      dot = cap->ccd[i].dot * ((dark_digit==16)?2:1);
      shad_data = (U16*)K_shad_data[i];
	  if(k_scan_par.img.mono)
        //Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
		Scan_Shad_Shading(i, 1, shad_data, dot*2);  //Park test 
      else {
        for(j = 0; j < 3; j++)
          Scan_Shad_Shading(i, j+1, &shad_data[j*dot], dot*2);
      }
    }

    if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
      return FALSE;
	//#endif
	}

  return TRUE;
}


int cal_dark_shading_only(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U32 dot;
  U16 *buf, *shad_data/*, dark_min, dark_max*/;
  U32 gain, *dark_buf, dark_shift, dark_digit;
  U8 SIDE_K[2]={0, 0};


  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  if(k_scan_par.source == I3('ADF')) { //Park test
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_LAMP_OFF|ACQ_MOTOR_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);
  }
  else {
	user_param(ACQ_CALIBRATION|ACQ_LAMP_OFF|ACQ_NO_MIRROR|ACQ_NO_SHADING);
  }
  if(!Scan_Param())
    return FALSE;
  cal_img_buf_store(0, 0, 0);

  if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;

  for (i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
	  continue;
    buf = (U16*)K_img_buf[i];
    dot = cap->ccd[i].dot;
    dark_buf = (U32*)K_shad16_data[i];
    set->shd[i].gain_base = 8;  // default digital gain base
    gain = (0x10000 / set->shd[i].gain_base) << 16;
    _cal_average_iterate(buf, dot*color_loop, k_scan_par.img.height);      

	for(j = 0; j < color_loop; j++) {
	  
      #if 1	//Dark min protect check

	    //if(SIDE_K[i] == 1) {
		  if(_cal_find_min(&buf[j], color_loop, dot) == 0) {
		    printf("Dark shading fail: Image dark min = 0!!!\n");
			return FALSE;
		  }
	    //}

	#endif

      _cal_construct_dark16(&buf[j], &dark_buf[j*dot], color_loop, 1, dot, gain);
	}

    dark_shift = set->shd[i].dark_shift = 0;
    dark_digit = set->shd[i].dark_digit = 16;
    _cal_do_shift_dark(dark_buf, (U16*)K_shad_data[i], dot*color_loop, dark_digit, dark_shift);
  }
  
#if 1  //Scan for final image check(debug)
    Scan_Param();
    cal_img_buf_store(0, 0, 0);

    for(i = 0; i < 2; i++) {
	  if(SIDE_K[i] == 0)
	    continue;
      dark_digit = set->shd[i].dark_digit;
      dot = cap->ccd[i].dot * ((dark_digit==16)?2:1);
      shad_data = (U16*)K_shad_data[i];
	  if(k_scan_par.img.mono)
        //Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
		Scan_Shad_Shading(i, 1, shad_data, dot*2);  //Park test 
      else {
        for(j = 0; j < 3; j++)
          Scan_Shad_Shading(i, j+1, &shad_data[j*dot], dot*2);
      }
    }

    if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
      return FALSE;
#endif

  return TRUE;
}


__inline void _cal_construct_white16(U16 *data, U32 *shad, int next_data, int next_shad, int num, U32 gain_base, U16 white_target)
{
  U32 white_gain, white;
  U16 *last_data = data + next_data*num;
  while(data < last_data) {
    white = *data;
    if(white > 0) {
      white_gain = white_target * gain_base / white;
      if(white_gain > 0xffff)
        white_gain = 0xffff;
    }
    else {
      white_gain = 0xffff;
    }
    *shad = (*shad & 0xffff) + (white_gain << 16);
    data += next_data;
    shad += next_shad;
  }
}

U32 _cal_set_white_gain(SHD_SET_T *set, U32 white_min)
{
  U32 gain_base;
  #ifdef GL310
  if(white_min == 0) {
    gain_base = 0x2000;
    set->gain_base = 8;
  }
  else {  
    gain_base = CAL_SHADING_WHITE * 0x4000 / white_min;
    if(gain_base < 0x10000) {
      gain_base = 0x4000;
      set->gain_base = 4;
    }
    else {
      gain_base = 0x2000;
      set->gain_base = 8;
    }
  }
  #else
    gain_base = 0x2000;
    set->gain_base = 8;    
  #endif
  return gain_base;
}


int cal_white_shading(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U16 *buf, *shad_data, white_min;
  U32 dot, *white_buf, gain_base, dark_digit;
  U16 white_target[2][3];
  U8 SIDE_K[2]={0, 0};


  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  if(k_scan_par.source == I3('ADF')) { //Park test
#ifdef K_ADF_ROLLER
	  //user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR);	//20170220
#else
	  //user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR);	//20170220
#endif
  }
  else {
	user_param(ACQ_CALIBRATION|ACQ_NO_MIRROR|ACQ_NO_SHADING);
  }
  
	if(!Scan_Param()) //Park test
    return FALSE;

  cal_img_buf_store(0, 0, 0);


  if(!Scan_Shad_Calibration(set))
    return FALSE;

  for(i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
      continue;
    dark_digit = set->shd[i].dark_digit;
    dot = cap->ccd[i].dot * ((dark_digit==16)?2:1);
    shad_data = (U16*)K_shad_data[i];
    if(k_scan_par.img.mono) {
      //Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
		Scan_Shad_Shading(i, 1, shad_data, dot*2);  //Park test 
    }
    else {
      for(j = 0; j < 3; j++)
        Scan_Shad_Shading(i, j+1, &shad_data[j*dot], dot*2);
    }
  }
  
  if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;
/*
  if(k_scan_par.source == I3('ADF')) {
	white_target[0] = CAL_SHADING_WHITE_B_R;
	white_target[1] = CAL_SHADING_WHITE_B_G;
	white_target[2] = CAL_SHADING_WHITE_B_B;
  }
  else {
	white_target[0] = CAL_SHADING_WHITE_A_R;
	white_target[1] = CAL_SHADING_WHITE_A_G;
	white_target[2] = CAL_SHADING_WHITE_A_B;
  }
*/

  white_target[0][0] = SHD_WHITE_TARGET[0][0];
  white_target[0][1] = SHD_WHITE_TARGET[0][1];
  white_target[0][2] = SHD_WHITE_TARGET[0][2];

  white_target[1][0] = SHD_WHITE_TARGET[1][0];
  white_target[1][1] = SHD_WHITE_TARGET[1][1];
  white_target[1][2] = SHD_WHITE_TARGET[1][2];


  for (i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
      continue;

    buf = (U16*)K_img_buf[i];
    dot = cap->ccd[i].dot;
    white_buf = (U32*)K_shad16_data[i];

    _cal_ave_sort_iterate(buf, dot*color_loop, k_scan_par.img.height);
    white_min = _cal_find_min(buf, 1, dot*color_loop);
    gain_base = _cal_set_white_gain(&set->shd[i], white_min);
    
	for(j = 0; j < color_loop; j++) {
      _cal_construct_white16(&buf[j], &white_buf[j*dot], color_loop, 1, dot, gain_base, white_target[i][j]);
	}

	//Save shading profile
	#ifdef Save_Shading_Profile
		Save_Shading(&k_scan_par, buf, white_buf, gain_base, i);
	#endif

    _cal_do_shift_dark(white_buf, (U16*)K_shad_data[i], dot*color_loop, 16, 0);
  }

#ifdef K_ADF_ROLLER
  if(k_scan_par.source == I3('ADF')) {
	job_RollerToINIT(0); //Park test
  }
#endif

if(bSaveFile) {
//#if 1  //Scan for final image check(debug)

  if(k_scan_par.source == I3('ADF')) { //Park test
	  
#ifdef K_ADF_ROLLER
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);
#else
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR|ACQ_NO_SHADING);
#endif
  }
  else {
	user_param(ACQ_CALIBRATION|ACQ_NO_MIRROR|ACQ_NO_SHADING);
  }

  if(!Scan_Param())  //Park test
    return FALSE;

  cal_img_buf_store(0, 0, 0);
  if(!Scan_Shad_Calibration(set))
    return FALSE;

  for(i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
      continue;
    dark_digit = 16; //set->shd[i].dark_digit;
    dot = cap->ccd[i].dot * ((dark_digit==16)?2:1);
    shad_data = (U16*)K_shad_data[i];
	if(k_scan_par.img.mono) {
      //Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
		Scan_Shad_Shading(i, 1, shad_data, dot*2);  //Park test 
    }
    else {
      for(j = 0; j < 3; j++)
        Scan_Shad_Shading(i, j+1, &shad_data[j*dot], dot*2);
    }
  }

  if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;
  
//#endif
}

  return TRUE;
}


int cal_white_shading_only(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  int i, j;
  int color_loop = (k_scan_par.img.mono == 4) ? 1:3;
  U16 *buf, *shad_data, white_min;
  U32 dot, *white_buf, gain_base, dark_digit;
  U16 white_target[2][3];
  U8 SIDE_K[2]={0, 0};


  SIDE_K[0] = k_scan_par.duplex & SCAN_A_SIDE;

  SIDE_K[1] = (k_scan_par.duplex & SCAN_B_SIDE) >> 1;

  if(k_scan_par.source == I3('ADF')) { //Park test
#ifdef K_ADF_ROLLER
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR);
#else
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR);
#endif
  }
  else {
	user_param(ACQ_CALIBRATION|ACQ_NO_MIRROR);
  }
  
	if(!Scan_Param()) //Park test
    return FALSE;

  cal_img_buf_store(0, 0, 0);


  for(i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
	  continue;
    dark_digit = set->shd[i].dark_digit;
    dot = cap->ccd[i].dot * ((dark_digit==16)?2:1);
    shad_data = (U16*)K_shad_data[i];
    if(k_scan_par.img.mono) {
      //Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
		Scan_Shad_Shading(i, 1, shad_data, dot*2);  //Park test 
    }
    else {
      for(j = 0; j < 3; j++)
        Scan_Shad_Shading(i, j+1, &shad_data[j*dot], dot*2);
    }
  }
  
  if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;
/*
  if(k_scan_par.source == I3('ADF')) {
	white_target[0] = CAL_SHADING_WHITE_B_R;
	white_target[1] = CAL_SHADING_WHITE_B_G;
	white_target[2] = CAL_SHADING_WHITE_B_B;
  }
  else {
	white_target[0] = CAL_SHADING_WHITE_A_R;
	white_target[1] = CAL_SHADING_WHITE_A_G;
	white_target[2] = CAL_SHADING_WHITE_A_B;
  }
*/

  white_target[0][0] = SHD_WHITE_TARGET[0][0];
  white_target[0][1] = SHD_WHITE_TARGET[0][1];
  white_target[0][2] = SHD_WHITE_TARGET[0][2];

  white_target[1][0] = SHD_WHITE_TARGET[1][0];
  white_target[1][1] = SHD_WHITE_TARGET[1][1];
  white_target[1][2] = SHD_WHITE_TARGET[1][2];

  for (i = 0; i < 2; i++) {
    buf = (U16*)K_img_buf[i];
    dot = cap->ccd[i].dot;
    white_buf = (U32*)K_shad16_data[i];

    _cal_ave_sort_iterate(buf, dot*color_loop, k_scan_par.img.height);
    white_min = _cal_find_min(buf, 1, dot*color_loop);
    gain_base = _cal_set_white_gain(&set->shd[i], white_min);
    
	for(j = 0; j < color_loop; j++) {
      _cal_construct_white16(&buf[j], &white_buf[j*dot], color_loop, 1, dot, gain_base, white_target[i][j]);
	}
    _cal_do_shift_dark(white_buf, (U16*)K_shad_data[i], dot*color_loop, 16, 0);
  }

#ifdef K_ADF_ROLLER
  if(k_scan_par.source == I3('ADF')) {
	job_RollerToINIT(0); //Park test
  }
#endif

#if 1  //Scan for final image check(debug)

  if(k_scan_par.source == I3('ADF')) { //Park test
	  
#ifdef K_ADF_ROLLER
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR);
#else
	  user_param(ACQ_CALIBRATION|ACQ_NO_PP_SENSOR|ACQ_NO_MIRROR);
#endif
  }
  else {
	user_param(ACQ_CALIBRATION|ACQ_NO_MIRROR);
  }

  if(!Scan_Param())  //Park test
    return FALSE;

  cal_img_buf_store(0, 0, 0);

  for(i = 0; i < 2; i++) {
    if(SIDE_K[i] == 0)
	  continue;
    dark_digit = 16; //set->shd[i].dark_digit;
    dot = cap->ccd[i].dot * ((dark_digit==16)?2:1);
    shad_data = (U16*)K_shad_data[i];
	if(k_scan_par.img.mono) {
      //Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
		Scan_Shad_Shading(i, 1, shad_data, dot*2);  //Park test 
    }
    else {
      for(j = 0; j < 3; j++)
        Scan_Shad_Shading(i, j+1, &shad_data[j*dot], dot*2);
    }
  }

  if(!job_Scan()/* || !job_Wait(JOB_SCAN, 1)*/)
    return FALSE;
  
#endif
  return TRUE;
}


int save_shd_flash(CALIBRATION_CAP_T_ *cap, CALIBRATION_SET_T_ *set)
{
  FLASH_SHD_T *shd = (FLASH_SHD_T*)K_img_buf;
  U8 *src, *dst;
  int i, j, channel, len;

  shd->source = (U8)k_scan_par.source;

  //shd->duplex = k_scan_par.duplex;
#ifdef K_ADF_ROLLER
  shd->duplex = SCAN_AB_SIDE;
#else
  shd->duplex = k_scan_par.duplex;
#endif

  shd->bit = k_scan_par.img.bit;
  shd->dot = cap->ccd[0].dot;
  shd->dpi = cap->ccd[0].dpi;
  shd->bank_num = ((shd->bit < 24) || (cap->ccd[0].type == I3('CCD')))? 1: 3;
  shd->bank_size = shd->dot * (shd->bit/8) / shd->bank_num * 2;
  for(i = 0, dst=(U8*)shd+sizeof(FLASH_SHD_T); i < 2; i++) {
    for(j = 0; j < 3; j++)
      shd->exp[i][j] = set->ccd[i].exp[j];
    channel = (cap->ccd[i].type == I4('CIS6'))? 6: 3;
    for(j = 0; j < channel; j++) {
      shd->offset[i][j] = set->afe[i].offset[j];
      shd->gain[i][j] = set->afe[i].gain[j];
    }
    if(shd->duplex & (1<<i)) {
      src = K_shad_data[i];
      memcpy(dst, src, shd->bank_num*shd->bank_size);
      dst += (shd->bank_num*shd->bank_size);
    }
  }

  len = (int)dst - (int)shd;
  Scan_Shad_Flash(shd, len);

  return TRUE;
}

extern int Scan_ME_Flash(void *buf, int length);

int cal_save_me_flash(CALIBRATION_SET_T_ *set)
{
  FLASH_ME_T me;
  U8 *src, *dst;
  int i, j, channel, len;

  me.prefeed = set->me.prefeed;
  me.postfeed = set->me.postfeed;

  Scan_ME_Flash(&me, sizeof(FLASH_ME_T));
  return TRUE;
}

extern int Scan_JobCreate(char job);
extern int Scan_JobEnd();

#if 1
int job_Calibration(SC_PAR_T_ *_par)
{
  int i;
#ifdef CALIBRATION_ALL
  int CalibrationMode[] = {300,600};
  int CalibrationTimes= sizeof(CalibrationMode) / sizeof(int);
#endif
#ifdef Show_Time_Cost
  //SYSTEMTIME st0, st1, lt, ct;
  unsigned long st0, st1, lt, ct;
#endif

  bCalibration = TRUE;

  memset(&k_scan_par, 0, sizeof(SC_PAR_T_));

  k_scan_par.source = _par->source;
  k_scan_par.duplex = 3;
  //k_scan_par.page = 1;
  k_scan_par.img.format = I3('RAW');
  k_scan_par.img.bit = (_par->img.bit >= 24)? 48: 16;
  k_scan_par.img.mono = (_par->img.mono == 4) ? 4 : 0;
#ifndef CALIBRATION_ALL
  k_scan_par.img.dpi.x = (_par->img.dpi.x >= 400) ? 600 : 300;
  k_scan_par.img.dpi.y = (_par->img.dpi.x >= 400) ? 600 : 300;
#endif
  SCAN_DOC_SIZE = DOC_K_PRNU;

  if(!Scan_JobCreate(JOB_ADF))
	goto NG;

  K_BatchNum++;
  K_PageNum=0;

  printf("Calibration processing...\n");
  
#ifdef Show_Time_Cost
	//GetSystemTime(&st0);
	st0 = clock();	
#endif

#ifdef K_ADF_ROLLER

#else
	if(k_scan_par.source == I3('ADF')/* && (k_scan_par.img.dpi.x == 300)*/) {
		//ADF prefeed calibration
		if(!cal_prefeed(&K_Cap, &K_Set))
		  goto NG;

		#ifdef Show_Time_Cost
		  //GetSystemTime(&lt);
		  //printf("Prefeed time cost %.3f s\n", (double)time_spend(&st0, &lt)/10000000);
			lt = clock();
			printf("Prefeed time cost %.3f s\n", (float)(lt - st0)/1000);
		#endif
	}
#endif

#ifdef CALIBRATION_ALL
for(i=0 ; i<CalibrationTimes;i++)
{
	k_scan_par.img.dpi.x = CalibrationMode[i];
	k_scan_par.img.dpi.y = CalibrationMode[i];

	printf("%c%c%d calibration start...\n", (U8)k_scan_par.source, (k_scan_par.img.bit>=24)?'C':'G', k_scan_par.img.dpi.x);
	#ifdef Show_Time_Cost
	  //GetSystemTime(&st1);
		st1 = clock();
	#endif

#endif
if(bCalibrationMode) {
	#ifdef K_ADF_ROLLER
	  if(k_scan_par.source == I3('ADF')){
		 job_RollerToINIT(0);
		 if(bCalibrationMode)
			job_RollerToPRNU(0);
	  }
	  else {
		if(bCalibrationMode)
			job_LoadPaper(k_scan_par.source, 0);
	  }
	#else
	  //if(!job_LoadPaper(k_scan_par.source, 0))
	  //  goto NG;
	#endif

	////Full calibration

	//Get AFE / Calibration capability
    if(!cal_set_def(&K_Cap, &K_Set))
      goto NG;

    //AFE offset
    if(!cal_AFE_offset(&K_Cap, &K_Set))
      goto NG;

	#ifdef Show_Time_Cost
		//GetSystemTime(&lt);
		//printf("AFE offset time cost %.3f s\n", (double)time_spend(&st1, &lt)/10000000);
		lt = clock();
		printf("AFE offset time cost %.3f s\n", (double)(lt - st1)/1000);
	#endif
    
    //exposure time
    if(!cal_exposure_time(&K_Cap, &K_Set))
      goto NG;

	#ifdef Show_Time_Cost
		ct = lt;
		//GetSystemTime(&lt);
		//printf("LED time cost %.3f s\n", (double)time_spend(&ct, &lt)/10000000);
		lt = clock();
		printf("LED time cost %.3f s\n", (double)(lt - ct)/1000);
	#endif

    //AFE gain
    if(!cal_AFE_gain(&K_Cap, &K_Set))
      goto NG;

	#ifdef Show_Time_Cost
		ct = lt;
		//GetSystemTime(&lt);
		//printf("AFE gain time cost %.3f s\n", (double)time_spend(&ct, &lt)/10000000);
		lt = clock();
		printf("AFE gain time cost %.3f s\n", (double)(lt - ct)/1000);
	#endif

	//exposure balance
    if(!cal_exposure_balance(&K_Cap, &K_Set))
      goto NG;

	#ifdef Show_Time_Cost
		ct = lt;
		//GetSystemTime(&lt);
		//printf("LED balance time cost %.3f s\n", (double)time_spend(&ct, &lt)/10000000);
		lt = clock();
		printf("LED balance time cost %.3f s\n", (double)(lt - ct)/1000);
	#endif

    //dark shading
    if(!cal_dark_shading(&K_Cap, &K_Set))
      goto NG;

	#ifdef Show_Time_Cost
		ct = lt;
		//GetSystemTime(&lt);
		//printf("Dark shading time cost %.3f s\n", (double)time_spend(&ct, &lt)/10000000);
		lt = clock();
		printf("Dark shading time cost %.3f s\n", (double)(lt - ct)/1000);
	#endif

#ifdef K_ADF_ROLLER
	  if(k_scan_par.source == I3('ADF')){
		 job_RollerToINIT(0);
	  }
#endif
    //white shading
    if(!cal_white_shading(&K_Cap, &K_Set))
      goto NG;

    #ifdef Show_Time_Cost
		ct = lt;
		//GetSystemTime(&lt);
		//printf("White shading time cost %.3f s\n", (double)time_spend(&ct, &lt)/10000000);
		lt = clock();
		printf("White shading time cost %.3f s\n", (double)(lt - ct)/1000);
	#endif
}
else {
	//Get AFE / Calibration capability
    if(!cal_set_def_shading_only(&K_Cap, &K_Set))
      goto NG;

	//Shading calibration only
    //dark shading
    if(!cal_dark_shading_only(&K_Cap, &K_Set))
      goto NG;

#ifdef K_ADF_ROLLER
	  if(k_scan_par.source == I3('ADF')){
		 job_RollerToINIT(0);
	  }
#endif
    //white shading
    if(!cal_white_shading_only(&K_Cap, &K_Set))
      goto NG;
} 

	//save to flash
	save_shd_flash(&K_Cap, &K_Set);

	printf("%c%c%d calibration finish.\n", (U8)k_scan_par.source, (k_scan_par.img.bit>=24)?'C':'G', k_scan_par.img.dpi.x);

	#ifdef Show_Time_Cost
		//GetSystemTime(&lt);
		//printf("Current mode time cost %.3f s\n\n", (double)time_spend(&st1, &lt)/10000000);
		lt = clock();
		printf("Current mode time cost %.3f s\n\n", (double)(lt - st1)/1000);
	#endif

#ifdef CALIBRATION_ALL
		
}//end for(i=0 ; i<CalibrationTimes;i++)
Sleep(1000);

#endif

#ifdef K_ADF_ROLLER

#else
	if(k_scan_par.source == I3('ADF')/* && (k_scan_par.img.dpi.x == 300)*/) {
		//ADF postfeed calibration
		if(!cal_postfeed(&K_Cap, &K_Set))
		  goto NG;

		#ifdef Show_Time_Cost
			ct = lt;
			//GetSystemTime(&lt);
			//printf("Postfeed time cost %.3f s\n", (double)time_spend(&ct, &lt)/10000000);
			lt = clock();
			printf("Postfeed time cost %.3f s\n", (double)(lt - ct)/1000);
		#endif
	}

	Sleep(1000); //Do next sace flash command while previous ADF motor reset process will cause bug. 

	cal_save_me_flash(&K_Set);
	
#endif


K_OK:
  bCalibration = TRUE;
  printf("Calibration success.");

#ifdef Show_Time_Cost
	//GetSystemTime(&lt);
	//printf("Total time cost %.3f s\n", (double)time_spend(&st0, &lt)/10000000);
	lt = clock();
	printf("Total time cost %.3f s\n", (double)(lt - st0 )/1000);
#endif
  
  Scan_JobEnd();

  if (k_scan_par.source == I3('ADF')) {
#ifdef K_ADF_ROLLER

#else
#if 1
	  // 0. Eject Paper
	  //	  if(!job_EjectPaper(k_scan_par.source, 0))
	  //		goto NG;

	  // 0. Pickup Home
	  if (!job_ResetHome(k_scan_par.source, 0))
		  goto NG;
#endif
#endif
  }

  return TRUE;
NG:
  bCalibration = FALSE;
  printf("Calibration fail.");
  return FALSE;
}
#endif

void job_Set_Calibration_Par(unsigned char type, SC_PAR_T_ *_par)
{
	switch(type) {
		case 1:
			SHD_WHITE_TARGET[0][0] = SHD_WHITE_TARGET_A_R_TYPE1;
			SHD_WHITE_TARGET[0][1] = SHD_WHITE_TARGET_A_G_TYPE1;
			SHD_WHITE_TARGET[0][2] = SHD_WHITE_TARGET_A_B_TYPE1;

			SHD_WHITE_TARGET[1][0] = SHD_WHITE_TARGET_B_R_TYPE1;
			SHD_WHITE_TARGET[1][1] = SHD_WHITE_TARGET_B_G_TYPE1;
			SHD_WHITE_TARGET[1][2] = SHD_WHITE_TARGET_B_B_TYPE1;

			break;

		case 2:
		default:
			SHD_WHITE_TARGET[0][0] = SHD_WHITE_TARGET_A_R_TYPE2;
			SHD_WHITE_TARGET[0][1] = SHD_WHITE_TARGET_A_G_TYPE2;
			SHD_WHITE_TARGET[0][2] = SHD_WHITE_TARGET_A_B_TYPE2;

			SHD_WHITE_TARGET[1][0] = SHD_WHITE_TARGET_B_R_TYPE2;
			SHD_WHITE_TARGET[1][1] = SHD_WHITE_TARGET_B_G_TYPE2;
			SHD_WHITE_TARGET[1][2] = SHD_WHITE_TARGET_B_B_TYPE2;
			break;
	}
}
