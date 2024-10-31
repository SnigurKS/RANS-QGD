/* Please Write the OpenCL Kernel(s) code here*/
__kernel void VH(__global float* res, __global const float* v, __global float* w)
{
	int k = get_global_id(0);
	float wl = w[k];
	float vl = v[k];
	res[k] = wl * vl;
}
/* Please Write the OpenCL Kernel(s) code here*/
__kernel void VW(__global float* H1, __global const float* v, __global float* w)
{
	int k = get_global_id(0);
	float wl = w[k];
	float vl = v[k];
	float Hl = H1[0];
	w[k] = wl - Hl * vl;

}

