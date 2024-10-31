/*
        for (int k = 0; k < Size; k++)
            Hl += w[k] *  v[i * Size + k];
		//
		H[i * m + j] = Hl;
        //
		  
        for (int k = 0; k < Size; k++)
            w[k] += Hl * v[i * Size + k];
		
    //}
}
*/
__kernel void FirstOperation1(__global const float* v, __global const float* w, int i, __global float* res)
{
	int k = get_global_id(0);
	int Size = get_global_size(0)*4;
	private float wH = 0;
	//wH = w[k] * v[i * Size + k];
	wH = w[4*k] * v[i * Size + 4*k];
	wH += w[4 * k + 1] * v[i * Size + 4 * k + 1];
	wH += w[4 * k + 2] * v[i * Size + 4 * k + 2];
	wH += w[4 * k + 3] * v[i * Size + 4 * k + 3];
	res[k] = wH;
}
__kernel void FirstOperation2 (__global float* H, __global const float* v, __global float* w, int i)
{
	int k = get_global_id(0);
	int Size = get_global_size(0);
	//
	w[k] -= H[0] * v[i * Size + k];

}
