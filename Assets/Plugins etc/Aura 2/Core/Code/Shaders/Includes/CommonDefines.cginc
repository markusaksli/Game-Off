
/***************************************************************************
*                                                                          *
*  Copyright (c) Rapha�l Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Rapha�l Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Rapha�l Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

// Global define helpers
#define CONCATENATE_DEFINES(a, b) a##b

// Floating point precision
#define PRECISION_HALF

#if defined(PRECISION_SIMPLE)
	#define FP float
#elif defined(PRECISION_HALF)
	#define FP half
#endif

#define FP2		CONCATENATE_DEFINES(FP, 2)
#define FP3		CONCATENATE_DEFINES(FP, 3)
#define FP4		CONCATENATE_DEFINES(FP, 4)
#define FP2x2	CONCATENATE_DEFINES(FP2, x2)
#define FP3x3	CONCATENATE_DEFINES(FP3, x3)
#define FP4x4	CONCATENATE_DEFINES(FP4, x4)

// Compute shaders dispatch threads dimensions
#define NUM_THREAD_X 8
#define NUM_THREAD_Y 8
#define NUM_THREAD_Z 8

#define VISIBILITY_GROUPS_SIZE_X NUM_THREAD_X
#define VISIBILITY_GROUPS_SIZE_Y NUM_THREAD_Y
#define VISIBILITY_GROUPS_SIZE_Z NUM_THREAD_Z
#define VISIBILITY_GROUPS_SIZE uint3(VISIBILITY_GROUPS_SIZE_X, VISIBILITY_GROUPS_SIZE_Y, VISIBILITY_GROUPS_SIZE_Z)