#define ROW_DIM 0 //измерение от 0 до m
#define COL_DIM 1//измерение от 0 до P
// P threads per row compute 1/P-th of each dot product.
// WORK has n/P entries.
// A*x = y
//
//   ________
//	|		 |
//  m	A	 |
//	|		 |
//	|___n____|
// Марица записывается не построчно, а постолбцово!!!
//  <-p-><-p->
// Матрица делится по горизонтали на P/n кусочков, P должно быть кратное n
__kernel void MatrixToVector(__global const float * A, __global const float * x,
	__global float * y,
	__local float * work,
	int m, int n)
{
	int ncols = n / get_global_size(COL_DIM); // Количество столбцов в каждой подматрице pxm
	int col0 = ncols * get_global_id(COL_DIM); // Первое значение (левое верхнее) в каждой подматрице
	// Копируется часть вектора X в WORK с использованием всех доступных потоков
	// (если work-group мало, но в них много thread, то get_local_id даст инкремент,
	// если work-group много, но в каждом по 1 therad, то k даст инкремент )
	for (int k = 0; k<ncols; k += get_local_size(ROW_DIM))
	{
		int col = k + get_local_id(ROW_DIM);
		if (col < ncols) 
			work[col] = x[col0 + col];
	}
	// синхронизация - для каждой workgroup сформирован свой вектор правой части
	barrier(CLK_LOCAL_MEM_FENCE); 
	//--
	float sum = (float)0;
	// Для каждой строки каждой подматрицы выполняется умножение A на правую часть work
	for (int k = 0; k<ncols; k++)
	{
		sum += A[get_global_id(ROW_DIM) + m*(col0 + k)] * work[k];
	}
	// Сохранение sum в Y для кадой подматрицы (P столбцов в строчке)
	y[get_global_id(ROW_DIM) + m*get_global_id(COL_DIM)] = sum;
}
// Приведение вектора Y(шириной P) для подматриц 
// К вектору решения всей матрицы A (шириной 1)
__kernel void reduce_rows(__global float * y, int m, int p)
{
	int row = get_global_id(0);// 0..m
	float sum = (float)0;
	//Вычисляется сумма всех элементов строки вектора Y (шириной P) 
	// и записывется в первый столбец вектора Y
	for (int col = 0; col<p; col++) 
		sum += y[row + m*col];
	y[row] = sum;
}
