kernel void Test1(global float* x, global float* y, global float* ar, global int* N)
{
	float RelaxOrto = ar[0];
	float Tay = ar[1];
	int NX = N[0];
	int NY = N[1];
	int width_i = 4;

	int i_n = get_global_id(0);
	int id = i_n * width_i + NX;
	float xp = 0; float xe = 0; float xw = 0; float xs = 0; float xn = 0;
	float yp = 0; float ye = 0; float yw = 0; float ys = 0; float yn = 0;
	float xen = 0; float xwn = 0; float xes = 0; float xws = 0;
	float yen = 0; float ywn = 0; float yes = 0; float yws = 0;
	float Ap = 0; float Ig = 0; float Alpha = 0; float Betta = 0; float Gamma = 0; float Delta = 0;

	int i;
	for (int k = 0; k < 4; k++)
	{
		// вместо if (((id + k)>NX) & ((id + k) < NX * NY - NX)) - проверка на первую и последнюю строчку
		//i = clamp((id + k), NX-1, NX * NY - NX-1); 	
		i = id + k;
		//
		if ((i % NX != 0) & ((i + 1) % NX != 0))
		{
			xp = x[i];
			xe = x[i + 1];
			xw = x[i - 1];
			xs = x[i - NX];
			xes = x[i - NX + 1];
			xws = x[i - NX - 1];

			xn = x[i + NX];
			xen = x[i + NX + 1];
			xwn = x[i + NX - 1];

			yp = y[i];
			ye = y[i + 1];
			yw = y[i - 1];
			ys = y[i - NX];
			yes = y[i - NX + 1];
			yws = y[i - NX - 1];

			yn = y[i + NX];
			yen = y[i + NX + 1];
			ywn = y[i + NX - 1];

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

			x[i] = (1 - Tay) * x[i] + Tay * xp;
			y[i] = (1 - Tay) * y[i] + Tay * yp;
		}

	}
}