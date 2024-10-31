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

extern "C" {
	// Device code
	__global__ void DiffGrid(float* X, float* Y, int* N)
	{
		int i = blockDim.x * blockIdx.x + threadIdx.x;
		int Nx = N[0];
		int Ny = N[1];		
		
		if ((i > Nx) & (i < Nx * Ny - Nx))
			if ((i % Nx != 0) & ((i + 1) % Nx != 0))
			{
				X[i] = 0.25 * (X[i + 1] + X[i - 1] + X[i - Nx] + X[i + Nx]);
				Y[i] = 0.25 * (Y[i + 1] + Y[i - 1] + Y[i - Nx] + Y[i + Nx]);
			}
	}
}