kernel void Test(global float* x, global float* y, global int* ar)
{
	//Получаем текущий номер потока
	int i = get_global_id(0);
	int width = 4;
	int NX = ar[0];
	int NY = ar[1];

	int id = width*i;
	int cid;
	///
	for (int k = 0; k < width; k++)
	{
		// вместо if ((cid>NX) & (cid < NX * NY - NX)) - проверка на первую и последнюю строчку
		cid = min(max((id + k), NX - 1), NX * NY - NX - 1);
		//
		if ((cid % NX != 0) & ((cid + 1) % NX != 0))
		{
			x[cid] = 0.25 * (x[cid + 1] + x[cid - 1] + x[cid - NX] + x[cid + NX]);
			y[cid] = 0.25 * (y[cid + 1] + y[cid - 1] + y[cid - NX] + y[cid + NX]);
		}
	}
}
/*
kernel void Test(global float* x, global int* ar)
{
//Получаем текущий номер потока
int i = get_global_id(0);
int width = 4;
int NX = ar[0];
int NY = ar[1];

int id = width*i;
int cid;
///
for (int k = 0; k < width; k++)
{
// вместо if ((cid>NX) & (cid < NX * NY - NX)) - проверка на первую и последнюю строчку
cid = min(max((id + k), NX-1),NX * NY - NX-1);
//
if((cid % NX != 0) & ((cid + 1) % NX != 0))
{
x[cid] = 0.25 * (x[cid + 1] + x[cid - 1] + x[cid - NX] + x[cid + NX]);
}
}
}
*/