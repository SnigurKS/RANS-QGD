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
	__global__ void OrthoGrid(float* X, float* Y, float* ar, int* N)
	{
		int i = blockDim.x * blockIdx.x + threadIdx.x;
		float RelaxOrto = ar[0];
		float Tay = ar[1];
		int Nx = N[0];
		int Ny = N[1];
		
				
		if ((i > Nx) & (i < Nx * Ny - Nx))
			if ((i % Nx != 0) & ((i + 1) % Nx != 0))
			{
				float xp = 0; float xe = 0; float xw = 0; float xs = 0; float xn = 0;
				float yp = 0; float ye = 0; float yw = 0; float ys = 0; float yn = 0;
				float xen = 0; float xwn = 0; float xes = 0; float xws = 0;
				float yen = 0; float ywn = 0; float yes = 0; float yws = 0;
				float Ap = 0; float Ig = 0; float Alpha = 0; float Betta = 0; float Gamma = 0; float Delta = 0;

				xp = X[i];
				xe = X[i + 1];
				xw = X[i - 1];
				xs = X[i - Nx];
				xes = X[i - Nx + 1];
				xws = X[i - Nx - 1];

				xn = X[i + Nx];
				xen = X[i + Nx + 1];
				xwn = X[i + Nx - 1];

				yp = Y[i];
				ye = Y[i + 1];
				yw = Y[i - 1];
				ys = Y[i - Nx];
				yes = Y[i - Nx + 1];
				yws = Y[i - Nx - 1];

				yn = Y[i + Nx];
				yen = Y[i + Nx + 1];
				ywn = Y[i + Nx - 1];

				/// g22
				Alpha = 0.25 * ((xn - xs) * (xn - xs) + (yn - ys) * (yn - ys));
				/// g12
				Betta = RelaxOrto * 0.25 * ((xe - xw) * (xn - xs) + (ye - yw) * (yn - ys));
				/// g11
				Gamma = 0.25 * ((xe - xw) * (xe - xw) + (ye - yw) * (ye - yw));
				/// чтобы не вылетать за float
				if ((Alpha + Gamma) < 0.000001)
				{
					Alpha = 1;
					Gamma = 1;
					Betta = 0;
				}
				//
				Ig = Alpha + Gamma;
				Ap = 1.0 / (2 * Ig);

				xp = Ap * (Alpha * (xw + xe) + Gamma * (xn + xs) - 0.5 * Betta * (xen - xwn - xes + xws));

				yp = Ap * (Alpha * (yw + ye) + Gamma * (yn + ys) - 0.5 * Betta * (yen - ywn - yes + yws));

				X[i] = (1 - Tay) * X[i] + Tay * xp;
				Y[i] = (1 - Tay) * Y[i] + Tay * yp;
			} //*/
	}
}