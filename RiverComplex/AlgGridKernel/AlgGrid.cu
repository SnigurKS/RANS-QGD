/*
* This code is taken more or less entirely from the NVIDIA CUDA SDK.
* This software contains source code provided by NVIDIA Corporation.
*
*/


//Includes for IntelliSense 
#define _SIZE_T_DEFINED
#ifndef __CUDACC__
#define __CUDACC__
#endif
#ifndef __cplusplus
#define __cplusplus
#endif

#include <cuda.h>
#include <device_launch_parameters.h>
#include <texture_fetch_functions.h>
#include "float.h"
#include <builtin_types.h>
#include <vector_functions.h>
#include <math.h>


extern "C" {
	// Device code
	__global__ void AlgGrid(const double* TopX, const double* BotX, const double* TopY, const double* BotY, double Q, double P, int Nx, int Ny, double* X, double* Y)
	{
		int idx = blockDim.x * blockIdx.x + threadIdx.x;

		int i = idx / Nx; // row number
		int j = idx % Nx; // col number
		// mesh step by vertical
		double DETA = 1.0 / (Ny - 1);
		double TQI = 1.0f / tanhf(Q);
		double ETA = i * DETA;
		double DUM = Q * (1 - ETA);
		DUM = 1 - tanhf(DUM) * TQI;
		double s = P * ETA + (1 - P) * DUM;
		// form functions
		double N0 = s;
		double N1 = 1 -s;

		if (idx < Nx * Ny) {
			X[idx] = N0*BotX[j] + N1*TopX[j];
			Y[idx] = N0*BotY[j] + N1*TopY[j];
		}
	}
}
/*
* This code is taken more or less entirely from the NVIDIA CUDA SDK.
* This software contains source code provided by NVIDIA Corporation.
*
*/


