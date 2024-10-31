kernel void Surch(global int* jMax, global float* SS2, global float* XS, global float* YS )
{
    int j = get_global_id(0); 
	int Nx = jMax[0];
	float S2 = SS2[0];
	float S3 = SS2[1];
    int jMap = Nx - 1;
    float EM1, EM2, EM3, EM4;
    float X2, Y2, X3, Y3;
    float STJM, SJJM;
    float XS2;
    float YS2;
    float XS3;
    float YS3;
    //
    float DXS = XS[3*Nx+j] - XS[0*Nx+j];
    float DYS = YS[3*Nx+j] - YS[0*Nx+j];
    XS[1*Nx+j] = XS[0*Nx+j] + S2 * DXS;
    YS[1*Nx+j] = YS[0*Nx+j] + S2 * DYS;
    XS[2*Nx+j] = XS[0*Nx+j] + S3 * DXS;
    YS[2*Nx+j] = YS[0*Nx+j] + S3 * DYS;
    //
	if ((j>1)&(j<jMap))
    {
        if (fabs(XS[0*Nx+j + 1] - XS[0*Nx+j - 1]) > 0.000001)
            EM1 = (YS[0*Nx+j + 1] - YS[0*Nx+j - 1]) / (XS[0*Nx+j + 1] - XS[0*Nx+j - 1]);
        else
            EM1 = 1.0E+06f * (YS[0*Nx+j + 1] - YS[0*Nx+j - 1]);
        if (fabs(XS[1*Nx+j] - XS[1*Nx+j - 1]) > 0.000001)
            EM2 = (YS[1*Nx+j] - YS[1*Nx+j - 1]) / (XS[1*Nx+j] - XS[1*Nx+j - 1]);
        else
            EM2 = 1000000 * (YS[1*Nx+j] - YS[1*Nx+j - 1]);
        X2 = (EM1 * (YS[0*Nx+j] - YS[1*Nx+j] + EM2 * XS[1*Nx+j]) + XS[0*Nx+j]) / (1 + EM1 * EM2);
        Y2 = YS[1*Nx+j] + EM2 * (X2 - XS[1*Nx+j]);
        STJM = sqrt((X2 - XS[1*Nx+j - 1]) * (X2 - XS[1*Nx+j - 1]) + (Y2 - YS[1*Nx+j - 1]) * (Y2 - YS[1*Nx+j - 1]));
        SJJM = sqrt((XS[1*Nx+j] - XS[1*Nx+j - 1]) * (XS[1*Nx+j] - XS[1*Nx+j - 1]) + (YS[1*Nx+j] - YS[1*Nx+j - 1]) * (YS[1*Nx+j] - YS[1*Nx+j - 1]));
        if (STJM < SJJM)
        {
            XS2 = X2;
            YS2 = Y2;
        }
        else
        {
            if (fabs(XS[1*Nx+j + 1] - XS[1*Nx+j]) > 0.000001)
                EM2 = (YS[1*Nx+j + 1] - YS[1*Nx+j]) / (XS[1*Nx+j + 1] - XS[1*Nx+j]);
            else
                EM2 = 1000000 * (YS[1*Nx+j + 1] - YS[1*Nx+j]);
            X2 = (EM1 * (YS[0*Nx+j] - YS[1*Nx+j] + EM2 * XS[1*Nx+j]) + XS[0*Nx+j]) / (1 + EM1 * EM2);
            Y2 = YS[1*Nx+j] + EM2 * (X2 - XS[1*Nx+j]);
            XS2 = X2;
            YS2 = Y2;
        }
    
        if (fabs(XS[3*Nx+j + 1] - XS[3*Nx+j - 1]) > 0.000001)
            EM4 = (YS[3*Nx+j + 1] - YS[3*Nx+j - 1]) / (XS[3*Nx+j + 1] - XS[3*Nx+j - 1]);
        else
            EM4 = 1000000 * (YS[3*Nx+j + 1] - YS[3*Nx+j - 1]);
        if (fabs(XS[2*Nx+j] - XS[2*Nx+j - 1]) > 0.000001)
            EM3 = (YS[2*Nx+j] - YS[2*Nx+j - 1]) / (XS[2*Nx+j] - XS[2*Nx+j - 1]);
        else
            EM3 = 1000000 * (YS[2*Nx+j] - YS[2*Nx+j - 1]);
        //
        X3 = (EM4 * (YS[3*Nx+j] - YS[2*Nx+j] + EM3 * XS[2*Nx+j]) + XS[3*Nx+j]) / (1 + EM3 * EM4);
        Y3 = YS[2*Nx+j] + EM3 * (X3 - XS[2*Nx+j]);
        STJM = sqrt((X3 - XS[2*Nx+j - 1]) * (X3 - XS[2*Nx+j - 1]) + (Y3 - YS[2*Nx+j - 1]) * (Y3 - YS[2*Nx+j - 1]));
        SJJM = sqrt((XS[2*Nx+j] - XS[2*Nx+j - 1]) * (XS[2*Nx+j] - XS[2*Nx+j - 1]) + (YS[2*Nx+j] - YS[2*Nx+j - 1]) * (YS[2*Nx+j] - YS[2*Nx+j - 1]));
        //
        if (STJM > SJJM)
        {
            if (fabs(XS[2*Nx+j + 1] - XS[2*Nx+j]) > 0.000001)
                EM3 = (YS[2*Nx+j + 1] - YS[2*Nx+j]) / (XS[2*Nx+j + 1] - XS[2*Nx+j]);
            else
                EM3 = 1000000 * (YS[2*Nx+j + 1] - YS[2*Nx+j]);
            X3 = (EM4 * (YS[3*Nx+j] - YS[2*Nx+j] + EM3 * XS[2*Nx+j]) + XS[3*Nx+j]) / (1 + EM3 * EM4);
            Y3 = YS[2*Nx+j] + EM3 * (X3 - XS[2*Nx+j]);
        }
        //
        XS3 = X3;
        YS3 = Y3;
    
        XS[1*Nx+j] = XS2;
        YS[1*Nx+j] = YS2;
        XS[2*Nx+j] = XS3;
        YS[2*Nx+j] = YS3;

    }
        
    //
}