kernel void UV_CV2(const int CV2Length, const int CVWLength, const int width, const float dt, const float rho_w, const float nu_mol, const float tau, const float WSinJ, const float WCosJ,
	global read_only int* OCV, global read_only int* Num, global read_only float* OLx10, global read_only float* OLx32, global read_only float* OLy01, global read_only float* OLy23,
	global read_only float* OSS, global read_only int* OP1, global read_only int* OAreaElems, global read_only float* ONx, global read_only float* ONy, global read_only float* OAlpha,
	global read_only float* OLk, global read_only float* OS0, global read_only float* OCV_Tau, global read_only float* OP, global read_only float* OK, global read_only float* OE,
	global read_only float* OnuT, global float* OU, global float* OV, global float* OS)
{
	int c = get_global_id(0) * width; // NX*NY / width
	//
	float LsummU = 0, LsummV = 0, LsummS = 0; //потоки U, V скорости, S
	int p0, jj, Lv1, Lv2, Lp1, Lt1, Lt2, Lt3, Lz1, Lz2, Lz3;
	float lx10, lx32, ly01, ly23, LS, LUc1, LVc1, LPc1, LSc1, LKc1, LUc2, LVc2, LPc2, LSc2, LKc2, Ls2, Ldudx, Ldudy, Ldvdx, Ldvdy, Ldpdx, Ldpdy, Ldsdx, Ldsdy, Ldkdx, Ldkdy, Lnx, Lny, Lalpha,
		LUcr, LVcr, LPcr, LScr, LLk, LNucr, LKcr, LEcr, LpressU, LconvU, LdiffU, LregU1, LregU2, LregU, LpressV, LconvV, LdiffV, LregV1, LregV2, LregV, LconvS, LdiffS, LregS, wx, wy, wk;
	int k, i, j;
	for (k = 0; k < width; k++)
	{
		i = c + k;
		//
		LsummU = 0;//потоки U скорости
		LsummV = 0;//потоки V скорости
		LsummS = 0;//потоки s концентрации
		//
		p0 = OCV[Num[i]];
		jj = Num[i + 1] - Num[i] - 1; //количество КО, связанных с данным узлом
		for (j = Num[i]; j < Num[i + 1] - 1; j++)
		{
			lx10 = OLx10[j]; lx32 = OLx32[j];
			ly01 = OLy01[j]; ly23 = OLy23[j];
			//площадь
			LS = OSS[j];
			//сосоедние элементы
			Lv1 = OCV[(j - Num[i] + 1) % jj + Num[i] + 1];
			Lv2 = OCV[j + 1];
			//вторая точка общей грани
			Lp1 = OP1[j];
			//находим значения функций в центрах масс 1ого и 2ого треугольника как средние значения по элементу
			Lt1 = OAreaElems[Lv1 * 3]; Lt2 = OAreaElems[Lv1 * 3 + 1]; Lt3 = OAreaElems[Lv1 * 3 + 2];
			LUc1 = (OU[Lt1] + OU[Lt2] + OU[Lt3]) / 3.0f;
			LVc1 = (OV[Lt1] + OV[Lt2] + OV[Lt3]) / 3.0f;
			LPc1 = (OP[Lt1] + OP[Lt2] + OP[Lt3]) / 3.0f;
			LSc1 = (OS[Lt1] + OS[Lt2] + OS[Lt3]) / 3.0f;
			LKc1 = (OK[Lt1] + OK[Lt2] + OK[Lt3]) / 3.0f;
			//
			Lz1 = OAreaElems[Lv2 * 3]; Lz2 = OAreaElems[Lv2 * 3 + 1]; Lz3 = OAreaElems[Lv2 * 3 + 2];
			LUc2 = (OU[Lz1] + OU[Lz2] + OU[Lz3]) / 3.0f;
			LVc2 = (OV[Lz1] + OV[Lz2] + OV[Lz3]) / 3.0f;
			LPc2 = (OP[Lz1] + OP[Lz2] + OP[Lz3]) / 3.0f;
			LSc2 = (OS[Lz1] + OS[Lz2] + OS[Lz3]) / 3.0f;
			LKc2 = (OK[Lz1] + OK[Lz2] + OK[Lz3]) / 3.0f;
			//значения производных в точке пересечения граней
			Ls2 = 2 * LS;
			Ldudx = ((LUc1 - LUc2) * ly01 + (OU[Lp1] - OU[p0]) * ly23) / Ls2;
			Ldudy = ((LUc1 - LUc2) * lx10 + (OU[Lp1] - OU[p0]) * lx32) / Ls2;
			Ldvdx = ((LVc1 - LVc2) * ly01 + (OV[Lp1] - OV[p0]) * ly23) / Ls2;
			Ldvdy = ((LVc1 - LVc2) * lx10 + (OV[Lp1] - OV[p0]) * lx32) / Ls2;
			Ldpdx = ((LPc1 - LPc2) * ly01 + (OP[Lp1] - OP[p0]) * ly23) / Ls2;
			Ldpdy = ((LPc1 - LPc2) * lx10 + (OP[Lp1] - OP[p0]) * lx32) / Ls2;
			Ldsdx = ((LSc1 - LSc2) * ly01 + (OS[Lp1] - OS[p0]) * ly23) / Ls2;
			Ldsdy = ((LSc1 - LSc2) * lx10 + (OS[Lp1] - OS[p0]) * lx32) / Ls2;
			Ldkdx = ((LKc1 - LKc2) * ly01 + (OK[Lp1] - OK[p0]) * ly23) / Ls2;
			Ldkdy = ((LKc1 - LKc2) * lx10 + (OK[Lp1] - OK[p0]) * lx32) / Ls2;
			//внешняя нормаль к грани КО (контуру КО)
			Lnx = ONx[j]; Lny = ONy[j];
			////значение функций в точке пересечения грани КО и основной грани
			Lalpha = OAlpha[j];
			LUcr = Lalpha * OU[p0] + (1 - Lalpha) * OU[Lp1];
			LVcr = Lalpha * OV[p0] + (1 - Lalpha) * OV[Lp1];
			LPcr = Lalpha * OP[p0] + (1 - Lalpha) * OP[Lp1];
			LScr = Lalpha * OS[p0] + (1 - Lalpha) * OS[Lp1];
			LNucr = Lalpha * OnuT[p0] + (1 - Lalpha) * OnuT[Lp1] + nu_mol;
			LKcr = Lalpha * OK[p0] + (1 - Lalpha) * OK[Lp1];
			LEcr = Lalpha * OE[p0] + (1 - Lalpha) * OE[Lp1];
			//
			wx = tau * (LUcr * Ldudx + LVcr * Ldudy + 1.0f / rho_w * Ldpdx + 2.0f / 3.0f * Ldkdx);
			wy = tau * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0f / rho_w * Ldpdy + 2.0f / 3.0f * Ldkdy);
			wk = tau * (LUcr * Ldkdx + LVcr * Ldkdy - (LNucr * (2.0f * Ldudx * Ldudx + (Ldvdx + Ldudy) * (Ldvdx + Ldudy) + 2.0f * Ldvdy * Ldvdy)) - LEcr);
			//длина текущего фрагмента внешнего контура КО
			LLk = OLk[j];
			//
			//расчет потоков
			LpressU = -1.0f / rho_w * LPcr * Lnx - 2.0f / 3.0f * LKcr * Lnx;
			LconvU = -LUcr * LUcr * Lnx - (LUcr * LVcr) * Lny;
			LdiffU = (nu_mol + LNucr) * (2.0f * Ldudx * Lnx - 2.0f / 3.0f * (Ldudx + Ldvdy) * Lnx + Ldudy * Lny + Ldvdx * Lny);
			LregU1 = 2.0f * LUcr * wx * Lnx + 2.0f / 3.0f * wk * Lnx;
			LregU2 = (LVcr * wx + LUcr * wy) * Lny;
			LregU = LregU1 + LregU2;
			LsummU += (LconvU + LdiffU + LregU + LpressU) * LLk;
			//                  
			LpressV = -1.0f / rho_w * LPcr * Lny - 2.0f / 3.0f * LKcr * Lny;
			LconvV = -(LUcr * LVcr) * Lnx - LVcr * LVcr * Lny;
			LdiffV = (nu_mol + LNucr) * (2.0f * Ldvdy * Lny - 2.0f / 3.0f * (Ldudx + Ldvdy) * Lny + Ldvdx * Lnx + Ldudy * Lnx);
			LregV1 = 2.0f * LVcr * wy * Lny + 2.0f / 3.0f * wk * Lny;
			LregV2 = (LVcr * wx + LUcr * wy) * Lnx;
			LregV = LregV1 + LregV2;
			LsummV += (LconvV + LdiffV + LregV + LpressV) * LLk;
			//
			LconvS = - LScr * (LUcr + WSinJ) * Lnx - LScr * (LVcr - WCosJ) * Lny;
			LdiffS = LNucr * (Ldsdx * Lnx + Ldsdy * Lny);
			LregS = LScr * (wx * Lnx + wy * Lny);
			LsummS += (LconvS + LdiffS + LregS) * LLk;
		}
		//
		OU[p0] = OU[p0] + dt / OS0[p0] * LsummU;
		OV[p0] = OV[p0] + dt / OS0[p0] * LsummV;
		OS[p0] = OS[p0] + dt / OS0[p0] * LsummS;

	}
	if (c + width == CV2Length)
	{
		for (k = 0; k < CVWLength; k++)
		{
			i = c + width + k;
			LsummU = 0;//потоки U скорости
			LsummV = 0;//потоки V скорости
			LsummS = 0;//потоки s концентрации
			//
			p0 = OCV[Num[i]];
			jj = Num[i + 1] - Num[i] - 1; //количество КО, связанных с данным узлом
			for (j = Num[i]; j < Num[i + 1] - 1; j++)
			{
				lx10 = OLx10[j]; lx32 = OLx32[j];
				ly01 = OLy01[j]; ly23 = OLy23[j];
				//площадь
				LS = OSS[j];
				//сосоедние элементы
				Lv1 = OCV[(j - Num[i] + 1) % jj + Num[i] + 1];
				Lv2 = OCV[j + 1];
				//вторая точка общей грани
				Lp1 = OP1[j];
				//находим значения функций в центрах масс 1ого и 2ого треугольника как средние значения по элементу
				Lt1 = OAreaElems[Lv1 * 3]; Lt2 = OAreaElems[Lv1 * 3 + 1]; Lt3 = OAreaElems[Lv1 * 3 + 2];
				LUc1 = (OU[Lt1] + OU[Lt2] + OU[Lt3]) / 3.0f;
				LVc1 = (OV[Lt1] + OV[Lt2] + OV[Lt3]) / 3.0f;
				LPc1 = (OP[Lt1] + OP[Lt2] + OP[Lt3]) / 3.0f;
				LSc1 = (OS[Lt1] + OS[Lt2] + OS[Lt3]) / 3.0f;
				LKc1 = (OK[Lt1] + OK[Lt2] + OK[Lt3]) / 3.0f;
				//
				Lz1 = OAreaElems[Lv2 * 3]; Lz2 = OAreaElems[Lv2 * 3 + 1]; Lz3 = OAreaElems[Lv2 * 3 + 2];
				LUc2 = (OU[Lz1] + OU[Lz2] + OU[Lz3]) / 3.0f;
				LVc2 = (OV[Lz1] + OV[Lz2] + OV[Lz3]) / 3.0f;
				LPc2 = (OP[Lz1] + OP[Lz2] + OP[Lz3]) / 3.0f;
				LSc2 = (OS[Lz1] + OS[Lz2] + OS[Lz3]) / 3.0f;
				LKc2 = (OK[Lz1] + OK[Lz2] + OK[Lz3]) / 3.0f;
				//значения производных в точке пересечения граней
				Ls2 = 2 * LS;
				Ldudx = ((LUc1 - LUc2) * ly01 + (OU[Lp1] - OU[p0]) * ly23) / Ls2;
				Ldudy = ((LUc1 - LUc2) * lx10 + (OU[Lp1] - OU[p0]) * lx32) / Ls2;
				Ldvdx = ((LVc1 - LVc2) * ly01 + (OV[Lp1] - OV[p0]) * ly23) / Ls2;
				Ldvdy = ((LVc1 - LVc2) * lx10 + (OV[Lp1] - OV[p0]) * lx32) / Ls2;
				Ldpdx = ((LPc1 - LPc2) * ly01 + (OP[Lp1] - OP[p0]) * ly23) / Ls2;
				Ldpdy = ((LPc1 - LPc2) * lx10 + (OP[Lp1] - OP[p0]) * lx32) / Ls2;
				Ldsdx = ((LSc1 - LSc2) * ly01 + (OS[Lp1] - OS[p0]) * ly23) / Ls2;
				Ldsdy = ((LSc1 - LSc2) * lx10 + (OS[Lp1] - OS[p0]) * lx32) / Ls2;
				Ldkdx = ((LKc1 - LKc2) * ly01 + (OK[Lp1] - OK[p0]) * ly23) / Ls2;
				Ldkdy = ((LKc1 - LKc2) * lx10 + (OK[Lp1] - OK[p0]) * lx32) / Ls2;
				//внешняя нормаль к грани КО (контуру КО)
				Lnx = ONx[j]; Lny = ONy[j];
				////значение функций в точке пересечения грани КО и основной грани
				Lalpha = OAlpha[j];
				LUcr = Lalpha * OU[p0] + (1 - Lalpha) * OU[Lp1];
				LVcr = Lalpha * OV[p0] + (1 - Lalpha) * OV[Lp1];
				LPcr = Lalpha * OP[p0] + (1 - Lalpha) * OP[Lp1];
				LScr = Lalpha * OS[p0] + (1 - Lalpha) * OS[Lp1];
				LNucr = Lalpha * OnuT[p0] + (1 - Lalpha) * OnuT[Lp1] + nu_mol;
				//
				wx = tau * (LUcr * Ldudx + LVcr * Ldudy + 1.0f / rho_w * Ldpdx + 2.0f / 3.0f * Ldkdx);
				wy = tau * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0f / rho_w * Ldpdy + 2.0f / 3.0f * Ldkdy);
				//длина текущего фрагмента внешнего контура КО
				LLk = OLk[j];
				//
				//расчет потоков
				LpressU = -1.0f / rho_w * LPcr * Lnx;
				LconvU = -LUcr * LUcr * Lnx - (LUcr * LVcr) * Lny;
				LdiffU = OCV_Tau[k] / rho_w * Lny - nu_mol * 2.0f / 3.0f * (Ldudx + Ldvdy) * Lnx;
				LregU1 = 2.0f * LUcr * wx * Lnx;
				LregU2 = (LVcr * wx + LUcr * wy) * Lny;
				LregU = LregU1 + LregU2;
				LsummU += (LconvU + LdiffU + LregU + LpressU) * LLk;
				//                  
				LpressV = -1.0f / rho_w * LPcr * Lny;
				LconvV = -(LUcr * LVcr) * Lnx - LVcr * LVcr * Lny;
				LdiffV = OCV_Tau[k] / rho_w * Lnx - nu_mol * 2.0f / 3.0f * (Ldudx + Ldvdy) * Lny;
				LregV1 = 2.0f * LVcr * wy * Lny;
				LregV2 = (LVcr * wx + LUcr * wy) * Lnx;
				LregV = LregV1 + LregV2;
				LsummV += (LconvV + LdiffV + LregV + LpressV) * LLk;
				//
				LconvS = -LScr * (LUcr + WSinJ) * Lnx - LScr * (LVcr - WCosJ) * Lny;
				LdiffS = LNucr * (Ldsdx * Lnx + Ldsdy * Lny);
				LregS = LScr * (wx * Lnx + wy * Lny);
				LsummS += (LconvS + LdiffS + LregS) * LLk;
			}
			//
			OU[p0] = OU[p0] + dt / OS0[p0] * LsummU;
			OV[p0] = OV[p0] + dt / OS0[p0] * LsummV;
			OS[p0] = OS[p0] + dt / OS0[p0] * LsummS;
		}
	}

}

kernel void KE_CV2(const int CV2Length, const int CVWLength, const int width, const float dt, const float rho_w, const float nu_mol, const float tau, const float C_e1, const float C_e2, const float C_m, const float sigma_e, const float sigma_k, const float kappa, const float y_p_0,
	global read_only int* OCV, global read_only int* Num, global read_only float* OLx10, global read_only float* OLx32, global read_only float* OLy01, global read_only float* OLy23,
	global read_only float* OSS, global read_only int* OP1, global read_only int* OAreaElems, global read_only float* ONx, global read_only float* ONy, global read_only float* OAlpha,
	global read_only float* OLk, global read_only float* OS0, global read_only float* CV_WallKnotsDistance, global read_only float* OP, global read_only float* OU, global read_only float* OV,
	global float* OK, global float* OE, global float* OnuT, global float* OPk)
{
	int c = get_global_id(0) * width; // NX*NY / width
	//
	float cm14 = sqrt(sqrt(C_m));
	float LsummK = 0, LsummE = 0, LrightK = 0, LrightE = 0; //потоки K, E и правые части
	int p0, jj, Lv1, Lv2, Lp1, Lt1, Lt2, Lt3, Lz1, Lz2, Lz3;
	float lx10, lx32, ly01, ly23, LS, LUc1, LVc1, LPc1, LEc1, LKc1, LUc2, LVc2, LPc2, LEc2, LKc2, Ls2, Ldudx, Ldudy, Ldvdx, Ldvdy, Ldpdx, Ldpdy, Ldedx, Ldedy, Ldkdx, Ldkdy, Lnx, Lny, Lalpha,
		ldudx, ldudy, ldvdx, ldvdy, LUcr, LVcr, LScr, LLk, LNucr, LKcr, LEcr, LconvK, LdiffK, LregK, LconvE, LdiffE, LregE, wx, wy, wk, we, y_p_plus;
	int k, i, j;
	for (k = 0; k < width; k++)
	{
		i = c + k;
		//
		LsummK = 0;//потоки K скорости
		LsummE = 0;//потоки E скорости
		ldudx = 0; ldudy = 0; ldvdx = 0; ldvdy = 0;
		//
		p0 = OCV[Num[i]];
		jj = Num[i + 1] - Num[i] - 1; //количество КО, связанных с данным узлом
		for (j = Num[i]; j < Num[i + 1] - 1; j++)
		{
			lx10 = OLx10[j]; lx32 = OLx32[j];
			ly01 = OLy01[j]; ly23 = OLy23[j];
			//площадь
			LS = OSS[j];
			//сосоедние элементы
			Lv1 = OCV[(j - Num[i] + 1) % jj + Num[i] + 1];
			Lv2 = OCV[j + 1];
			//вторая точка общей грани
			Lp1 = OP1[j];
			//находим значения функций в центрах масс 1ого и 2ого треугольника как средние значения по элементу
			Lt1 = OAreaElems[Lv1 * 3]; Lt2 = OAreaElems[Lv1 * 3 + 1]; Lt3 = OAreaElems[Lv1 * 3 + 2];
			LUc1 = (OU[Lt1] + OU[Lt2] + OU[Lt3]) / 3.0f;
			LVc1 = (OV[Lt1] + OV[Lt2] + OV[Lt3]) / 3.0f;
			LPc1 = (OP[Lt1] + OP[Lt2] + OP[Lt3]) / 3.0f;
			LKc1 = (OK[Lt1] + OK[Lt2] + OK[Lt3]) / 3.0f;
			LEc1 = (OE[Lt1] + OE[Lt2] + OE[Lt3]) / 3.0f;
			//
			Lz1 = OAreaElems[Lv2 * 3]; Lz2 = OAreaElems[Lv2 * 3 + 1]; Lz3 = OAreaElems[Lv2 * 3 + 2];
			LUc2 = (OU[Lz1] + OU[Lz2] + OU[Lz3]) / 3.0f;
			LVc2 = (OV[Lz1] + OV[Lz2] + OV[Lz3]) / 3.0f;
			LPc2 = (OP[Lz1] + OP[Lz2] + OP[Lz3]) / 3.0f;
			LKc2 = (OK[Lz1] + OK[Lz2] + OK[Lz3]) / 3.0f;
			LEc2 = (OE[Lz1] + OE[Lz2] + OE[Lz3]) / 3.0f;
			//значения производных в точке пересечения граней
			Ls2 = 2 * LS;
			Ldudx = ((LUc1 - LUc2) * ly01 + (OU[Lp1] - OU[p0]) * ly23) / Ls2;
			Ldudy = ((LUc1 - LUc2) * lx10 + (OU[Lp1] - OU[p0]) * lx32) / Ls2;
			Ldvdx = ((LVc1 - LVc2) * ly01 + (OV[Lp1] - OV[p0]) * ly23) / Ls2;
			Ldvdy = ((LVc1 - LVc2) * lx10 + (OV[Lp1] - OV[p0]) * lx32) / Ls2;
			Ldpdx = ((LPc1 - LPc2) * ly01 + (OP[Lp1] - OP[p0]) * ly23) / Ls2;
			Ldpdy = ((LPc1 - LPc2) * lx10 + (OP[Lp1] - OP[p0]) * lx32) / Ls2;
			Ldkdx = ((LKc1 - LKc2) * ly01 + (OK[Lp1] - OK[p0]) * ly23) / Ls2;
			Ldkdy = ((LKc1 - LKc2) * lx10 + (OK[Lp1] - OK[p0]) * lx32) / Ls2;
			Ldedx = ((LEc1 - LEc2) * ly01 + (OE[Lp1] - OE[p0]) * ly23) / Ls2;
			Ldedy = ((LEc1 - LEc2) * lx10 + (OE[Lp1] - OE[p0]) * lx32) / Ls2;
			//внешняя нормаль к грани КО (контуру КО)
			Lnx = ONx[j]; Lny = ONy[j];
			////значение функций в точке пересечения грани КО и основной грани
			Lalpha = OAlpha[j];
			LUcr = Lalpha * OU[p0] + (1 - Lalpha) * OU[Lp1];
			LVcr = Lalpha * OV[p0] + (1 - Lalpha) * OV[Lp1];
			LNucr = Lalpha * OnuT[p0] + (1 - Lalpha) * OnuT[Lp1] + nu_mol;
			LKcr = Lalpha * OK[p0] + (1 - Lalpha) * OK[Lp1];
			LEcr = Lalpha * OE[p0] + (1 - Lalpha) * OE[Lp1];
			//
			wx = tau * (LUcr * Ldudx + LVcr * Ldudy + 1.0f / rho_w * Ldpdx + 2.0f / 3.0f * Ldkdx);
			wy = tau * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0f / rho_w * Ldpdy + 2.0f / 3.0f * Ldkdy);
			wk = tau * (LUcr * Ldkdx + LVcr * Ldkdy - (LNucr * (2.0f * Ldudx * Ldudx + (Ldvdx + Ldudy) * (Ldvdx + Ldudy) + 2.0f * Ldvdy * Ldvdy)) - LEcr);
			we = tau * (LUcr * Ldedx + LVcr * Ldedy - (LEcr / LKcr * (C_e1 * LNucr * (2.0f * Ldudx * Ldudx + (Ldvdx + Ldudy) * (Ldvdx + Ldudy) + 2.0f * Ldvdy * Ldvdy)) - C_e2 * LEcr));
			//длина текущего фрагмента внешнего контура КО
			LLk = OLk[j];
			//
			//расчет потоков
			LconvK = -LUcr * LKcr * Lnx - LVcr * LKcr * Lny;
			LdiffK = (LNucr / sigma_k + nu_mol) * (Ldkdx * Lnx + Ldkdy * Lny);
			LregK = LKcr * wx * Lnx + LKcr * wy * Lny + LUcr * wk * Lnx + LVcr * wk * Lny;
			//
			LsummK += (LconvK + LdiffK + LregK) * LLk;
			//                  
			LconvE = -LUcr * LEcr * Lnx - LVcr * LEcr * Lny;
			LdiffE = (LNucr / sigma_e + nu_mol) * (Ldedx * Lnx + Ldedy * Lny);
			LregE = LEcr * wx * Lnx + LEcr * wy * Lny + LUcr * we * Lnx + LVcr * we * Lny;
			// 
			LsummE += (LconvE + LdiffE + LregE) * LLk;
			//
			// компоненты производных для Pk
			ldudx += LUcr * Lnx * LLk;
			ldudy += LUcr * Lny * LLk;
			ldvdx += LVcr * Lnx * LLk;
			ldvdy += LVcr * Lny * LLk;
		}
		//
		ldudx /= OS0[p0];
		ldudy /= OS0[p0];
		ldvdx /= OS0[p0];
		ldvdy /= OS0[p0];
		//
		OPk[p0] = OnuT[p0] * (2.0f * ldudx * ldudx + (ldvdx + ldudy) * (ldvdx + ldudy) + 2.0f * ldvdy * ldvdy);
		LrightK = (OPk[p0] - OE[p0]);
		LrightE = OE[p0] / OK[p0] * (C_e1 * OPk[p0] - C_e2 * OE[p0]);
		//
		OK[p0] = OK[p0] + dt / OS0[p0] * LsummK + dt * LrightK;
		OE[p0] = OE[p0] + dt / OS0[p0] * LsummE + dt * LrightE;
		OnuT[p0] = C_m * OK[p0] * OK[p0] / (OE[p0] + 1e-26);
		if (OnuT[p0] < nu_mol)
			OnuT[p0] = nu_mol;
	}
	if (c + width == CV2Length)
	{
		for (k = 0; k < CVWLength; k++)
		{
			i = c + width + k;
			LsummK = 0;//потоки K скорости
			LsummE = 0;//потоки E скорости
			ldudx = 0; ldudy = 0; ldvdx = 0; ldvdy = 0;
			//
			p0 = OCV[Num[i]];
			jj = Num[i + 1] - Num[i] - 1; //количество КО, связанных с данным узлом
			for (j = Num[i]; j < Num[i + 1] - 1; j++)
			{
				lx10 = OLx10[j]; lx32 = OLx32[j];
				ly01 = OLy01[j]; ly23 = OLy23[j];
				//площадь
				LS = OSS[j];
				//сосоедние элементы
				Lv1 = OCV[(j - Num[i] + 1) % jj + Num[i] + 1];
				Lv2 = OCV[j + 1];
				//вторая точка общей грани
				Lp1 = OP1[j];
				//находим значения функций в центрах масс 1ого и 2ого треугольника как средние значения по элементу
				Lt1 = OAreaElems[Lv1 * 3]; Lt2 = OAreaElems[Lv1 * 3 + 1]; Lt3 = OAreaElems[Lv1 * 3 + 2];
				LUc1 = (OU[Lt1] + OU[Lt2] + OU[Lt3]) / 3.0f;
				LVc1 = (OV[Lt1] + OV[Lt2] + OV[Lt3]) / 3.0f;
				LPc1 = (OP[Lt1] + OP[Lt2] + OP[Lt3]) / 3.0f;
				LKc1 = (OK[Lt1] + OK[Lt2] + OK[Lt3]) / 3.0f;
				LEc1 = (OE[Lt1] + OE[Lt2] + OE[Lt3]) / 3.0f;
				//
				Lz1 = OAreaElems[Lv2 * 3]; Lz2 = OAreaElems[Lv2 * 3 + 1]; Lz3 = OAreaElems[Lv2 * 3 + 2];
				LUc2 = (OU[Lz1] + OU[Lz2] + OU[Lz3]) / 3.0f;
				LVc2 = (OV[Lz1] + OV[Lz2] + OV[Lz3]) / 3.0f;
				LPc2 = (OP[Lz1] + OP[Lz2] + OP[Lz3]) / 3.0f;
				LKc2 = (OK[Lz1] + OK[Lz2] + OK[Lz3]) / 3.0f;
				LEc2 = (OE[Lz1] + OE[Lz2] + OE[Lz3]) / 3.0f;
				//значения производных в точке пересечения граней
				Ls2 = 2 * LS;
				Ldudx = ((LUc1 - LUc2) * ly01 + (OU[Lp1] - OU[p0]) * ly23) / Ls2;
				Ldudy = ((LUc1 - LUc2) * lx10 + (OU[Lp1] - OU[p0]) * lx32) / Ls2;
				Ldvdx = ((LVc1 - LVc2) * ly01 + (OV[Lp1] - OV[p0]) * ly23) / Ls2;
				Ldvdy = ((LVc1 - LVc2) * lx10 + (OV[Lp1] - OV[p0]) * lx32) / Ls2;
				Ldpdx = ((LPc1 - LPc2) * ly01 + (OP[Lp1] - OP[p0]) * ly23) / Ls2;
				Ldpdy = ((LPc1 - LPc2) * lx10 + (OP[Lp1] - OP[p0]) * lx32) / Ls2;
				Ldkdx = ((LKc1 - LKc2) * ly01 + (OK[Lp1] - OK[p0]) * ly23) / Ls2;
				Ldkdy = ((LKc1 - LKc2) * lx10 + (OK[Lp1] - OK[p0]) * lx32) / Ls2;
				//внешняя нормаль к грани КО (контуру КО)
				Lnx = ONx[j]; Lny = ONy[j];
				////значение функций в точке пересечения грани КО и основной грани
				Lalpha = OAlpha[j];
				LUcr = Lalpha * OU[p0] + (1 - Lalpha) * OU[Lp1];
				LVcr = Lalpha * OV[p0] + (1 - Lalpha) * OV[Lp1];
				LNucr = Lalpha * OnuT[p0] + (1 - Lalpha) * OnuT[Lp1] + nu_mol;
				LKcr = Lalpha * OK[p0] + (1 - Lalpha) * OK[Lp1];
				LEcr = Lalpha * OE[p0] + (1 - Lalpha) * OE[Lp1];
				//
				wx = tau * (LUcr * Ldudx + LVcr * Ldudy + 1.0f / rho_w * Ldpdx + 2.0f / 3.0f * Ldkdx);
				wy = tau * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0f / rho_w * Ldpdy + 2.0f / 3.0f * Ldkdy);
				wk = tau * (LUcr * Ldkdx + LVcr * Ldkdy - (LNucr * (2.0f * Ldudx * Ldudx + (Ldvdx + Ldudy) * (Ldvdx + Ldudy) + 2.0f * Ldvdy * Ldvdy)) - LEcr);
				//длина текущего фрагмента внешнего контура КО
				LLk = OLk[j];
				//
				//расчет потоков
				LconvK = -LUcr * LKcr * Lnx - LVcr * LKcr * Lny;
				LdiffK = (LNucr / sigma_k + nu_mol) * (Ldkdx * Lnx + Ldkdy * Lny);
				LregK = LKcr * wx * Lnx + LKcr * wy * Lny + LUcr * wk * Lnx + LVcr * wk * Lny;
				//
				LsummK += (LconvK + LdiffK + LregK) * LLk;
			}
			//
			y_p_plus = cm14 * sqrt(OK[p0]) * CV_WallKnotsDistance[k] / nu_mol;
			OPk[p0] = 0;
			if (y_p_plus > y_p_0)
			{
				OE[p0] = cm14 * cm14 * cm14 * OK[p0] * sqrt(OK[p0]) / kappa / CV_WallKnotsDistance[k];
				OPk[p0] = OE[p0];
			}
			else
				OE[p0] = 2.0f * OK[p0] / CV_WallKnotsDistance[k] / CV_WallKnotsDistance[k] * nu_mol;
			//
			LrightK = (OPk[p0] - OE[p0]);
			//
			OK[p0] = OK[p0] + dt / OS0[p0] * LsummK + dt * LrightK;
			OnuT[p0] = C_m * OK[p0] * OK[p0] / (OE[p0] + 1e-26);
			if (OnuT[p0] < nu_mol)
				OnuT[p0] = nu_mol;
		}
	}

}