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
	__global__ void Surch(int* jMax, float* SS2, float* XS, float* YS)
	{
		int j = blockDim.x * blockIdx.x + threadIdx.x;
		int Nx = jMax[0];
		if (j >= Nx) return;
		float S2 = SS2[0];
		float S3 = SS2[1];
		int jMap = Nx - 1;
		float EM1, EM2, EM3, EM4;
		float X2, Y2, X3, Y3;
		float STJM, SJJM;
		float XS2;
		float YS2;
		float XS3;
		float YS3;
		//
		float DXS = XS[3 * Nx + j] - XS[0 * Nx + j];
		float DYS = YS[3 * Nx + j] - YS[0 * Nx + j];
		XS[1 * Nx + j] = XS[0 * Nx + j] + S2 * DXS;
		YS[1 * Nx + j] = YS[0 * Nx + j] + S2 * DYS;
		XS[2 * Nx + j] = XS[0 * Nx + j] + S3 * DXS;
		YS[2 * Nx + j] = YS[0 * Nx + j] + S3 * DYS;
		//
		if ((j>1)&(j<jMap))
		{
			if (fabsf(XS[0 * Nx + j + 1] - XS[0 * Nx + j - 1]) > 0.000001)
				EM1 = (YS[0 * Nx + j + 1] - YS[0 * Nx + j - 1]) / (XS[0 * Nx + j + 1] - XS[0 * Nx + j - 1]);
			else
				EM1 = 1.0E+06f * (YS[0 * Nx + j + 1] - YS[0 * Nx + j - 1]);
			if (fabsf(XS[1 * Nx + j] - XS[1 * Nx + j - 1]) > 0.000001)
				EM2 = (YS[1 * Nx + j] - YS[1 * Nx + j - 1]) / (XS[1 * Nx + j] - XS[1 * Nx + j - 1]);
			else
				EM2 = 1000000 * (YS[1 * Nx + j] - YS[1 * Nx + j - 1]);
			X2 = (EM1 * (YS[0 * Nx + j] - YS[1 * Nx + j] + EM2 * XS[1 * Nx + j]) + XS[0 * Nx + j]) / (1 + EM1 * EM2);
			Y2 = YS[1 * Nx + j] + EM2 * (X2 - XS[1 * Nx + j]);
			STJM = sqrtf((X2 - XS[1 * Nx + j - 1]) * (X2 - XS[1 * Nx + j - 1]) + (Y2 - YS[1 * Nx + j - 1]) * (Y2 - YS[1 * Nx + j - 1]));
			SJJM = sqrtf((XS[1 * Nx + j] - XS[1 * Nx + j - 1]) * (XS[1 * Nx + j] - XS[1 * Nx + j - 1]) + (YS[1 * Nx + j] - YS[1 * Nx + j - 1]) * (YS[1 * Nx + j] - YS[1 * Nx + j - 1]));
			if (STJM < SJJM)
			{
				XS2 = X2;
				YS2 = Y2;
			}
			else
			{
				if (fabsf(XS[1 * Nx + j + 1] - XS[1 * Nx + j]) > 0.000001)
					EM2 = (YS[1 * Nx + j + 1] - YS[1 * Nx + j]) / (XS[1 * Nx + j + 1] - XS[1 * Nx + j]);
				else
					EM2 = 1000000 * (YS[1 * Nx + j + 1] - YS[1 * Nx + j]);
				X2 = (EM1 * (YS[0 * Nx + j] - YS[1 * Nx + j] + EM2 * XS[1 * Nx + j]) + XS[0 * Nx + j]) / (1 + EM1 * EM2);
				Y2 = YS[1 * Nx + j] + EM2 * (X2 - XS[1 * Nx + j]);
				XS2 = X2;
				YS2 = Y2;
			}

			if (fabsf(XS[3 * Nx + j + 1] - XS[3 * Nx + j - 1]) > 0.000001)
				EM4 = (YS[3 * Nx + j + 1] - YS[3 * Nx + j - 1]) / (XS[3 * Nx + j + 1] - XS[3 * Nx + j - 1]);
			else
				EM4 = 1000000 * (YS[3 * Nx + j + 1] - YS[3 * Nx + j - 1]);
			if (fabsf(XS[2 * Nx + j] - XS[2 * Nx + j - 1]) > 0.000001)
				EM3 = (YS[2 * Nx + j] - YS[2 * Nx + j - 1]) / (XS[2 * Nx + j] - XS[2 * Nx + j - 1]);
			else
				EM3 = 1000000 * (YS[2 * Nx + j] - YS[2 * Nx + j - 1]);
			//
			X3 = (EM4 * (YS[3 * Nx + j] - YS[2 * Nx + j] + EM3 * XS[2 * Nx + j]) + XS[3 * Nx + j]) / (1 + EM3 * EM4);
			Y3 = YS[2 * Nx + j] + EM3 * (X3 - XS[2 * Nx + j]);
			STJM = sqrtf((X3 - XS[2 * Nx + j - 1]) * (X3 - XS[2 * Nx + j - 1]) + (Y3 - YS[2 * Nx + j - 1]) * (Y3 - YS[2 * Nx + j - 1]));
			SJJM = sqrtf((XS[2 * Nx + j] - XS[2 * Nx + j - 1]) * (XS[2 * Nx + j] - XS[2 * Nx + j - 1]) + (YS[2 * Nx + j] - YS[2 * Nx + j - 1]) * (YS[2 * Nx + j] - YS[2 * Nx + j - 1]));
			//
			if (STJM > SJJM)
			{
				if (fabsf(XS[2 * Nx + j + 1] - XS[2 * Nx + j]) > 0.000001)
					EM3 = (YS[2 * Nx + j + 1] - YS[2 * Nx + j]) / (XS[2 * Nx + j + 1] - XS[2 * Nx + j]);
				else
					EM3 = 1000000 * (YS[2 * Nx + j + 1] - YS[2 * Nx + j]);
				X3 = (EM4 * (YS[3 * Nx + j] - YS[2 * Nx + j] + EM3 * XS[2 * Nx + j]) + XS[3 * Nx + j]) / (1 + EM3 * EM4);
				Y3 = YS[2 * Nx + j] + EM3 * (X3 - XS[2 * Nx + j]);
			}
			//
			XS3 = X3;
			YS3 = Y3;

			XS[1 * Nx + j] = XS2;
			YS[1 * Nx + j] = YS2;
			XS[2 * Nx + j] = XS3;
			YS[2 * Nx + j] = YS3;

		}
	}

	__global__ void InternalGrid(float* X, float* Y, float* XS, float* YS, float* sCD, float* sAF, float* Aw, int* N)
	{
		//int width_j = 4;
		int idx = blockDim.x * blockIdx.x + threadIdx.x;
		int Nx = N[0];
		int Ny = N[1];
		int j = idx % Nx;
		int k = idx / Nx;
		//int idj = j * width_j;
		//
		float AW = Aw[0];		
		float A1 = 2.0f / (3.0f * AW - 1);
		float A2 = 2.0f / (3.0f * (1 - AW) - 1);
		float AJM = Nx - 1;
		float DZI = 1.0f / AJM;
		//
		float AJ, ZI, S;
		float sH[4];
		int l;
		
		// вместо if ((idj+1)<Nx)
		// l = fminf(j, Nx - 1);
		if (idx < Nx * Ny) {
			AJ = j - 1;
			ZI = AJ * DZI;
			S = sAF[k] + ZI * (sCD[k] - sAF[k]);
			//
			sH[0] = (1 - S) * (1 - S) * (1 - A1 * S);
			sH[1] = (1 - S) * (1 - S) * S * (A1 + 2);
			sH[2] = (1 - S) * S * S * (A2 + 2);
			sH[3] = S * S * (1 - A2 * (1 - S));
			//поправка координат
			for (int L = 0; L < 4; L++)
			{
				X[j + k*Nx] = X[j + k*Nx] + sH[L] * XS[L * Nx + j];
				Y[j + k*Nx] = Y[j + k*Nx] + sH[L] * YS[L * Nx + j];
			}
		}	
		
	}
}