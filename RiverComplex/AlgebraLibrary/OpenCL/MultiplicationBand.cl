__kernel void MultiplicationBand(__global float* M, __global float* V, __global float* X, int FH, int Size)
{
	int i = get_global_id(0);
	private float sum = X[i];
	private int m = 0; private int idx = 0;
	if (Size - i < FH)
	{
		m = Size - i;
		idx = FH * i - (FH - m - 1);
	}
	else
	{
		m = FH;
		idx = FH * i;
	}
	//
	sum += M[idx] * V[i];
	for (int j = 1; j < m; j++)
	{
		sum += M[idx + j] * V[i + j];
		X[i + j] += M[idx + j] * V[i];
	}
	X[i] = sum;
	//
}