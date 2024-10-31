// ax=y
__kernel void MultiplicationPacked(__global const float * a, __global const int * cols, __global const int * pointers, __global const float * x,
	__global float * y)
{
	float sum = 0.0f;
	int i = 10*get_global_id(0); // row index
	int k = 0, n = 0;;
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
			sum += a[k] * x[cols[k]];
		}
		y[i] = sum;
		i++;
	}
}
