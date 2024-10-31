kernel void AlgGrid(global int* NXYF, global float* BottomX, global float* TopX, global float* BottomY, global float* TopY, float P1, float Q1, global write_only float* X, global write_only float* Y)
{
	int width_i = 4;
	int NX = NXYF[0];
	int NY = NXYF[1];
	float P = P1;
	float Q = Q1;
	int i = get_global_id(0);// NY
	int j = get_global_id(1);// NX
	private float2 CX;
	private float2 CY;
	//
	int id = i * width_i;
	//
	CX.x = BottomX[j];
	CX.y = TopX[j];
	//
	CY.x = BottomY[j];
	CY.y = TopY[j];
	//
	float s[4]; // width_i
	float DETA = 1.0 / (NY - 1);
	float TQI = 1.0 / tanh(Q);
	float DUM, ETA;
	for (int t = 0; t < width_i; t++)
	{
		ETA = (id + t) * DETA;
		DUM = Q * (1 - ETA);
		DUM = 1 - tanh(DUM) * TQI;
		s[t] = P * ETA + (1 - P) * DUM;
	}
	//float eta;
	float2 N; int k, l, n;
	//
	for (k = 0; k < width_i; k++)
	{
		//вместо if ((id + k) < NY) используется функуция min
		l = min((id + k), NY - 1);
		n = l - id;
		//eta = 1 - d_eta * l;
		//N.x = 0.5 * (1 - eta[n]);
		//N.y = 0.5 * (1 + eta[n]);
		N.x = s[n];
		N.y = 1 - s[n];
		X[l * NX + j] = dot(CX, N);// вместо N0 * CX0 + N1 * CX1 ;
		Y[l * NX + j] = dot(CY, N);// вместо N0 * CX0 + N1 * CX1 ;
	}

}


/*kernel void AlgGrid(global int* NXYF, global float* Bottom, global float* Top, float P1, float Q1, global write_only float* X)
{
int width_i = 4;
int NX = NXYF[0];
int NY = NXYF[1];
float P = P1;
float Q = Q1;
int i = get_global_id(0);// NY
int j = get_global_id(1);// NX
int flagFirstX = NXYF[2];
private float4 CX;
float d_zeta = 2.0 / (NX - 1);
float d_eta = 2.0 / (NY - 1);
//
int id = i * width_i;
float zeta = d_zeta * j - 1;
//
if ((flagFirstX == 0) || (j == 0))
{
CX.x = Bottom[0];
CX.y = Bottom[NX - 1];
CX.z = Top[NX - 1];
CX.w = Top[0];
}
else
{
CX.x = Bottom[j - 1];
CX.y = Bottom[j];
CX.z = Top[j];
CX.w = Top[j - 1];
}
float eta [4];
float DETA = 1.0 / (NY - 1);
float TQI = 1.0 / tanh(Q);
float AL, DUM, ETA;
for (int t = 0; t < width_i; t++)
{
AL = id + t;
ETA = AL * DETA;
DUM = Q * (1 - ETA);
DUM = 1 - tanh(DUM) * TQI;
eta[t] =1 - 2 * ( P * ETA + (1 - P) * DUM );
}
//float eta;
float4 N; int k, l, n;
//
for (k = 0; k < width_i; k++)
{
//вместо if ((id + k) < NY) используется функуция min
l = min((id + k), NY - 1);
n = l - id;
//eta = 1 - d_eta * l;
N.x = 0.25 * (1 - zeta) * (1 - eta[n]);
N.y = 0.25 * (1 + zeta) * (1 - eta[n]);
N.z = 0.25 * (1 + zeta) * (1 + eta[n]);
N.w = 0.25 * (1 - zeta) * (1 + eta[n]);
X[l * NX + j] = dot(CX, N);// вместо N0 * CX0 + N1 * CX1 + N2 * CX2 + N3 * CX3;
}

}
*/

/*
kernel void AlgGrid(global int* NXYF, global float* Bottom, global float* Top, global write_only float* X)
{
int width_i = 4;
int NX = NXYF[0];
int NY = NXYF[1];
int i = get_global_id(0);// NY
int j = get_global_id(1);// NX
int flagFirstX = NXYF[2];
private float4 CX;
float d_zeta = 2.0 / (NX - 1);
float d_eta = 2.0 / (NY - 1);
//
int id = i * width_i;
float zeta =  d_zeta * j - 1;
//
if ((flagFirstX==0)||(j==0))
{
CX.x=Bottom[0];
CX.y=Bottom[NX - 1];
CX.z=Top[NX - 1];
CX.w=Top[0];
}
else
{
CX.x=Bottom[j - 1];
CX.y=Bottom[j];
CX.z=Top[j];
CX.w=Top[j - 1];
}
float eta; float4 N; int k, l;
//
for (k = 0; k < width_i; k++)
{
//вместо if ((id + k) < NY) используется функуция min
l = min((id + k), NY-1);
eta = 1 - d_eta * l;
N.x = 0.25 * (1 - zeta) * (1 - eta);
N.y = 0.25 * (1 + zeta) * (1 - eta);
N.z = 0.25 * (1 + zeta) * (1 + eta);
N.w = 0.25 * (1 - zeta) * (1 + eta);
X[l * NX + j] = dot(CX,N);// вместо N0 * CX0 + N1 * CX1 + N2 * CX2 + N3 * CX3;
}

}
*/

/*
kernel void AlgGrid(global int* NXYF, global float* Bottom, global float* Top, global write_only float* X)
{
float x;
int NX = NXYF[0];
int NY = NXYF[1];
int flagFirstX = NXYF[2];
private float CX0, CX1, CX2, CX3;
float d_zeta = 2.0 / (NX - 1);
float d_eta = 2.0 / (NY - 1);
int i = get_global_id(0);//NY
int j = get_global_id(1);//NX
int width = 4;
float eta = 1 - d_eta * i;
float zeta =  d_zeta * j - 1;
float N0 = 0.25 * (1 - zeta) * (1 - eta);
float N1 = 0.25 * (1 + zeta) * (1 - eta);
float N2 = 0.25 * (1 + zeta) * (1 + eta);
float N3 = 0.25 * (1 - zeta) * (1 + eta);
//
if ((flagFirstX==0)||(j==0))
{
CX0=Bottom[0];
CX1=Bottom[NX - 1];
CX2=Top[NX - 1];
CX3=Top[0];
x= N0 * CX0 + N1 * CX1 + N2 * CX2 + N3 * CX3;
}
else
{
CX0=Bottom[j - 1];
CX1=Bottom[j];
CX2=Top[j];
CX3=Top[j - 1];
x= N0 * CX0+ N1 * CX1 + N2 * CX2 + N3 * CX3;
}

X[i*NX+j] = x;
}
*/