__kernel void MultiplicationPackedH(__global const float * M, __global const int * cols, __global const int * pointers, __global const float * v, int h,
	__global float * w)
{
	float sum = 0.0f;
	int i = 10 * get_global_id(0); // row index
	int k = 0, n = 0;
	int Size = 10 * get_global_size(0);
	int p1 = 0;
	int p2 = 0;
	//
	//
	for (n = 0; n < 10; n++)
	{
		p1 = pointers[i];
		p2 = pointers[i + 1];
		sum = 0;
		for (k = p1; k < p2; k++)
		{
			sum += M[k] * v[Size*h + cols[k]];
		}
		w[i] = sum;
		i++;
	}
}
__kernel void HUpdate(__global const float* v, __global const float* w, int i, __global float* res)
{
	int k = 10*get_global_id(0);
	int Size = get_global_size(0)*10;
	private float wH = 0;
	//wH = w[k] * v[i * Size + k];
	for (int n = 0; n < 10; n++)
	{
		wH += w[k + n] * v[i * Size + k + n];
	}
	res[k/10] = wH;
}
__kernel void WUpdare( float H, __global const float* v, __global float* w, int i)
{
	int k = 10 * get_global_id(0);
	int Size = 10 * get_global_size(0);
	int n = 0; float b = 0;
	//
	for (n = 0; n < 10; n++)
	{
		b = w[k + n] - H * v[i * Size + k + n];
		w[k + n] = b;
	}

}
__kernel void VUpdare(__global float* v, __global const float* w, int ip1, float n)
{
	int k = 10 * get_global_id(0);
	int Size = 10 * get_global_size(0);
	for (int m = 0; m < 10; m++)
	{
		v[ip1 * Size + k + m] = w[k + m] / n;
	}
}
__kernel void norm(__global int* w, __global int* w2, __local int* data, int Size)
{
	int tid = get_local_id(0);
	int i = get_global_id(0);
	if(i<Size)
	{ 
		data[tid] = w[i]; // load into shared memory
		barrier(CLK_LOCAL_MEM_FENCE);
		// the first sum of squares
		private int s = get_local_size(0) / 2;
		if (tid < s)
			data[tid] = data[tid] * data[tid] + data[tid + s] * data[tid + s];
		barrier(CLK_LOCAL_MEM_FENCE);
		// just the sum

		for (s = get_local_size(0) / 4; s > 0; s >>= 1)
		{
			if (tid < s)
				data[tid] += data[tid + s];
			barrier(CLK_LOCAL_MEM_FENCE);
		}
		if (tid == 0) //write result of block reduction
			w2[get_group_id(0)] = data[0];
	}
}