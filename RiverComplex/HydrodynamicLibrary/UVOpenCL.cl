kernel void UV(const int width, const float dt, const float rho_w, const float nu, const float tau, global read_only int* OCV, global read_only int* Num, global read_only float* OLx10, global read_only float* OLx32,
	global read_only float* OLy01, global read_only float* OLy23, global read_only float* OSS, global read_only int* OP1, global read_only int* OAreaElems, global read_only float* ONx,
	global read_only float* ONy, global read_only float* OAlpha, global read_only float* OLk, global read_only float* OS0,
	global read_only float* OP, global float* OU, global float* OV)
{
	int c = get_global_id(0)*width; // NX*NY / width
	//
	float LsummU = 0, LsummV = 0; //потоки U, V скорости
	int p0, jj, Lv1, Lv2, Lp1, Lt1, Lt2, Lt3, Lz1, Lz2, Lz3;
	int Knots1 [3];
	int Knots2 [3];
	float _lx10, _lx32, _ly01, _ly23, LS, LUc1, LVc1, LPc1, LUc2, LVc2, LPc2, Ls2, Ldudx, Ldudy, Ldvdx, Ldvdy, Ldpdx, Ldpdy, Lnx, Lny, Lalpha, LUcr, LVcr, LPcr, LLk, Lpress, LconvU, LdiffU, 
		LregU1, LregU2, LregU, LpressV, LconvV, LdiffV, LregV1, LregV2, LregV;
	int k, i, j;
	for (k = 0; k < width; k++)
	{
		i = c + k;
		//
		LsummU = 0; //потоки U скорости
		LsummV = 0; //потоки V скорости
		//
		p0 = OCV[Num[i]];
		jj = Num[i + 1] - Num[i] - 1; //количество КО, связанных с данным узлом
		//
		for (j = Num[i]; j < Num[i + 1] - 1; j++)
		{
			_lx10 = OLx10[j]; _lx32 = OLx32[j];
			_ly01 = OLy01[j]; _ly23 = OLy23[j];
			//площадь
			LS = OSS[j];
			//сосоедние элементы
			Lv1 = OCV[(j - Num[i] + 1) % jj + Num[i] + 1];
			Lv2 = OCV[j + 1];
			//вторая точка общей грани
			Lp1 = OP1[j];
			//находим значения функций в центрах масс 1ого и 2ого треугольника как средние значения по элементу
			Knots1[0] = OAreaElems[Lv1 * 3]; Knots1[1] = OAreaElems[Lv1 * 3 + 1]; Knots1[2] = OAreaElems[Lv1 * 3 + 2];
			Lt1 = Knots1[0]; Lt2 = Knots1[1]; Lt3 = Knots1[2];
			LUc1 = (OU[Lt1] + OU[Lt2] + OU[Lt3]) / 3.0;
			LVc1 = (OV[Lt1] + OV[Lt2] + OV[Lt3]) / 3.0;
			LPc1 = (OP[Lt1] + OP[Lt2] + OP[Lt3]) / 3.0;
			Knots2[0] = OAreaElems[Lv2 * 3]; Knots2[1] = OAreaElems[Lv2 * 3 + 1]; Knots2[2] = OAreaElems[Lv2 * 3 + 2];
			Lz1 = Knots2[0]; Lz2 = Knots2[1]; Lz3 = Knots2[2];
			LUc2 = (OU[Lz1] + OU[Lz2] + OU[Lz3]) / 3.0;
			LVc2 = (OV[Lz1] + OV[Lz2] + OV[Lz3]) / 3.0;
			LPc2 = (OP[Lz1] + OP[Lz2] + OP[Lz3]) / 3.0;
			//значения производных в точке пересечения граней
			Ls2 = 2 * LS;
			Ldudx = ((LUc1 - LUc2) * _ly01 + (OU[Lp1] - OU[p0]) * _ly23) / Ls2;
			Ldudy = ((LUc1 - LUc2) * _lx10 + (OU[Lp1] - OU[p0]) * _lx32) / Ls2;
			Ldvdx = ((LVc1 - LVc2) * _ly01 + (OV[Lp1] - OV[p0]) * _ly23) / Ls2;
			Ldvdy = ((LVc1 - LVc2) * _lx10 + (OV[Lp1] - OV[p0]) * _lx32) / Ls2;
			Ldpdx = ((LPc1 - LPc2) * _ly01 + (OP[Lp1] - OP[p0]) * _ly23) / Ls2;
			Ldpdy = ((LPc1 - LPc2) * _lx10 + (OP[Lp1] - OP[p0]) * _lx32) / Ls2;
			//внешняя нормаль к грани КО (контуру КО)
			Lnx = ONx[j]; Lny = ONy[j];
			////значение функций в точке пересечения грани КО и основной грани
			Lalpha = OAlpha[j];
			LUcr = Lalpha * OU[p0] + (1 - Lalpha) * OU[Lp1];
			LVcr = Lalpha * OV[p0] + (1 - Lalpha) * OV[Lp1];
			LPcr = Lalpha * OP[p0] + (1 - Lalpha) * OP[Lp1];
			//длина текущего фрагмента внешнего контура КО
			LLk = OLk[j];
			//расчет потоков
			Lpress = -1.0 / rho_w * LPcr * Lnx;
			LconvU = -LUcr * LUcr * Lnx - (LUcr * LVcr) * Lny;
			LdiffU = nu * (2.0 * Ldudx * Lnx - 2.0 / 3.0 * (Ldudx + Ldvdy) * Lnx + Ldudy * Lny + Ldvdx * Lny);
			LregU1 = 2.0 * tau * LUcr * (LUcr * Ldudx + LVcr * Ldudy + 1.0 / rho_w * Ldpdx) * Lnx;
			LregU2 = tau * (LVcr * (LUcr * Ldudx + LVcr * Ldudy + 1.0 / rho_w * Ldpdx) + LUcr * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0 / rho_w * Ldpdy)) * Lny;
			LregU = LregU1 + LregU2;
			LsummU += (LconvU + LdiffU + LregU + Lpress) * LLk;
			//                  
			LpressV = -1.0 / rho_w * LPcr * Lny;
			LconvV = -(LUcr * LVcr) * Lnx - LVcr * LVcr * Lny;
			LdiffV = nu * (2.0 * Ldvdy * Lny - 2.0 / 3.0 * (Ldudx + Ldvdy) * Lny + Ldvdx * Lnx + Ldudy * Lnx);
			LregV1 = 2.0 * tau * LVcr * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0 / rho_w * Ldpdy) * Lny;
			LregV2 = tau * (LVcr * (LUcr * Ldudx + LVcr * Ldudy + 1.0 / rho_w * Ldpdx) + LUcr * (LUcr * Ldvdx + LVcr * Ldvdy + 1.0 / rho_w * Ldpdy)) * Lnx;
			LregV = LregV1 + LregV2;
			LsummV += (LconvV + LdiffV + LregV + LpressV) * LLk;
		}
		//

		OU[p0] = OU[p0] + (dt / OS0[i] * LsummU);
		OV[p0] = OV[p0] + (dt / OS0[i] * LsummV);
		//


	}
}