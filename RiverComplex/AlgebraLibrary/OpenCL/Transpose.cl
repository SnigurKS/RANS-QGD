//LocalSize = { TRANSPOSEX, TRANSPOSEY };
//GlobalSize = { P, Q };
//
// Константы transpose kernel
#define TRANSPOSEX 4
#define TRANSPOSEY 4
// kernel транспонирования матрицы размером P на Q
__kernel void transpose(const int P, const int Q,
	const __global float* input,
	__global float* output) {

	// Идентификаторы потока
	const int tx = get_local_id(0);
	const int ty = get_local_id(1);
	const int ID0 = get_group_id(0)*TRANSPOSEX + tx; // 0..P
	const int ID1 = get_group_id(1)*TRANSPOSEY + ty; // 0..Q

	// Воспользуемся локальной памятью для быстродействия
	__local float buffer[TRANSPOSEX][TRANSPOSEY];

	// Загрузим в локальную память x и y координаты для выполнения вращения (в локальной системе нумерации)
	if (ID0 < P && ID1 < Q) 
	{
		buffer[ty][tx] = input[ID1*P + ID0];
	}

	// Синхронизация всех потоков
	barrier(CLK_LOCAL_MEM_FENCE);

	// Мы здесь не должны подгружать индексы потоков x и y ,
	// так как это уже сделано в локальной памяти
	const int newID0 = get_group_id(1)*TRANSPOSEY + tx;
	const int newID1 = get_group_id(0)*TRANSPOSEX + ty;

	// Загружаем транспонированную матрицу в глобальный массив
	if (newID0 < Q && newID1 < P) {
		output[newID1*Q + newID0] = buffer[tx][ty];
	}
}
