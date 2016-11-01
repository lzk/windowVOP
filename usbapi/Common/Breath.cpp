#include "../stdafx.h"
#include <windows.h>
#include <stdio.h>
#include <conio.h>

int Wait_100ms(int dtick, int ms)
{
	if(dtick < ms) {
		dtick = ms - dtick;
		if(dtick > 100) {
			Sleep(100);
			return 0;
		}
		Sleep(dtick);
	}
	return 1;
}

#define TICK_CYCLE	1000
#define TICK_1		125
#define TICK_2		125
#define TICK_3		500
#define TICK_4		125

int BreathWaitHitKey(int ms)
{
	static int i = 0;
	static int tick = 0;
	int now, hit, wait_start, wait_step, show = 1;

	if(ms == -1) {
		i = 0;
		return 0;
	}
	else if(ms < 0) {
		ms = -ms;
		show = 0;
	}

	if(i == 0) {
		tick = GetTickCount();
		if(show)
			printf(".");
		i = 1;
	}

	wait_start = wait_step = GetTickCount();

	do {
		now = GetTickCount();
		hit = _kbhit();
		if(i == 1) {		// '.'
			if(Wait_100ms(now - wait_step, TICK_1)) {
				wait_step = GetTickCount();
				if(show)
					printf("\bo");
				i++;
			}
		}
		else if(i == 2) {	// 'o'
			if(Wait_100ms(now - wait_step, TICK_2)) {
				wait_step = GetTickCount();
				if(show)
					printf("\bO");
				i++;
			}
		}
		else if(i == 3) {	// 'O'
			if(Wait_100ms(now - wait_step, TICK_3)) {
				wait_step = GetTickCount();
				if(show)
					printf("\bo");
				i++;
			}
		}
		else if(i == 4) {	// 'o'
			if(Wait_100ms(now - wait_step, 248)) {
				wait_step = GetTickCount();
				if(show)
					printf("\b.");
				i = 1;
			}
		}
	} while(!hit && ((now - wait_start) < ms));
	if(hit) {
		if(show)
			printf("\b");
		hit = _getch();
	}
	return hit;
}

int PrintDigits(int digit)
{
	int p;
	for(p = 1; digit >= p; p*=10)
		printf("\b");
	printf("%d", digit);
	return digit;
}

int DigitWaitHitKey(int ms)
{
	static int i = 0;
	static int tick = 0;
	static int digit = 1;
	int now, hit, wait_start, wait_step, show = 1;

	if(ms == -1) {
		i = 0;
		return 0;
	}
	else if(ms < 0) {
		ms = -ms;
		show = 0;
	}

	if(i == 0) {
		tick = GetTickCount();
		digit = 1;
		if(show)
			printf(".");
		i = 1;
	}

	wait_start = wait_step = GetTickCount();
	do {
		hit = _kbhit();
		now = GetTickCount();
		if(i == 1) {		// '.'
			if(Wait_100ms(now - wait_step, TICK_1)) {
				wait_step = GetTickCount();
				if(show)
					printf("\bo");
				i++;
			}
		}
		else if(i == 2) {	// 'o'
			if(Wait_100ms(now - wait_step, TICK_2)) {
				wait_step = GetTickCount();
				if(show) {
					if(digit % 10 == 0)
						PrintDigits(digit);
					else
						printf("\b%d", digit % 10);
					digit++;
				}
				i++;
			}
		}
		else if(i == 3) {	// 'O'
			if(Wait_100ms(now - wait_step, TICK_3)) {
				wait_step = GetTickCount();
				if(show)
					printf("\bo");
				i++;
			}
		}
		else if(i == 4) {	// 'o'
			if(Wait_100ms(now - wait_step, TICK_4)) {
				wait_step = GetTickCount();
				printf("\b.");
				i = 1;
			}
		}
	} while(!hit && ((now - wait_start) < ms));
	if(hit) {
		if(show)
			printf("\b\b\b\b\b");
		hit = _getch();
	}
	return hit;
}
