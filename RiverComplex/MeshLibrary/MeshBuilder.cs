using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    [Serializable]
    public class MeshBuilder
    {
        bool surf_flag = false;
        public bool OpecCL = false; 
        /// <summary>
        /// Итоговая сетка
        /// </summary>
        Mesh _FinalMesh;
        public Mesh FinalMesh
        {
            get { return _FinalMesh; }
        }
        /// <summary>
        /// Составная область
        /// </summary>
        Area Area;
        /// <summary>
        /// Массив простых подобластей
        /// </summary>
        SimpleArea[][] areas;
        /// <summary>
        /// метод разбиения сетки:[0 - основная область, 1- верхний подслой, 2 - нижний подслой] 
        /// 0 - алгебраический метод; 1 - дифференциальный,  2 - метод Флетчера, 3 - ортотропный метод;.
        /// </summary>
        //int [] GenMethod;
        /// <summary>
        /// Параметры разбиения области и подобластей
        /// </summary>
        Parameter[] Prs = null;
        /// <summary>
        ///Генератор области 
        /// </summary>
        MeshGenerator[] mg = null;
        //флаг формирования параметров расчета (чтобы не переопредедять параметры для последующего расчета)
        bool flag = true;
        //
        public TimeSpan time = new TimeSpan();
        public TimeSpan timeTransport = new TimeSpan();
        public TimeSpan timeCalculate = new TimeSpan();
        public TimeSpan timeAll = new TimeSpan();
        System.Diagnostics.Stopwatch stopW = new System.Diagnostics.Stopwatch();
        /// <summary>
        /// Флаг расчета сетки на GPU с использованием технологии OpenCL
        /// </summary>
        public bool OpenCL = false;
        /// <summary>
        /// Флаг расчета сетки на GPU с использованием технологии CUDA
        /// </summary>
        public bool Cuda = false; 
        //
        MeshBuilder() { }
        /// <summary>
        /// Метод генерации всей области
        /// </summary>
        /// <param name="subareas">массив подобластей расчетной области</param>
        /// <param name="Method">метод разбиения сетки</param>
        public MeshBuilder(Area area,  Parameter[] Prs, bool surf_flag) 
        {
            Area = area;
            //GenMethod = Method;
            this.Prs = Prs;
            this.surf_flag = surf_flag;
        }
        public void ReturnNx(int Nx)
        {
            for (int i = 0; i < Prs.Length; i++)
                Prs[i].Nx = Nx;
        }
        public void ChangeArea(Area area)
        {
            Area = area;
        }
        public void GenerateMesh(bool structureChanged = true)
        {
            // получение массива простых областей
            areas = GenerateSimpleAreas();
            // формирование массива под сетки простых областей
            Mesh[][] Meshes = new Mesh[areas.Length][];
            for (int i = 0; i < areas.Length; i++)
                Meshes[i] = new Mesh[areas[0].Length];
            //
            if (flag)
            {
                GenerateParameters();
                flag = false;
            }
            for (int i = 0; i < areas.Length; i++)
            {
                mg[i].OpenCL = this.OpecCL;
                //
                for (int j = 0; j < areas[0].Length; j++)
                {
                    Meshes[i][j] = mg[i].Generate(areas[i][j]);
                    //
                    timeCalculate += mg[i].timeCalculate;
                    timeAll += mg[i].timeAll;
                    timeTransport += mg[i].timeTransrort;
                }
            }
            //склейка простых подобластей
            for (int i = 0; i < Meshes.Length; i++)
            {
                for (int j = 1; j < Meshes[0].Length; j++)
                {
                    Meshes[i][0].Add(Meshes[i][j]);
                }
            }
            //
            for (int i = 1; i < Meshes.Length; i++)
            {
                Meshes[0][0].Add(Meshes[i][0]);
            }
            Mesh prevMesh = null;
            if (!structureChanged)
                prevMesh = _FinalMesh;
            //
            _FinalMesh = Meshes[0][0];
            _FinalMesh.Renumberation();
            //
            stopW.Reset();
            stopW.Start();
            //
            if (!structureChanged)
                _FinalMesh.TransportCVstructure(prevMesh);
            _FinalMesh.TriangleGeometryCalculation(structureChanged);
            _FinalMesh.MakeWallFuncStructure(surf_flag);
            //
            time = stopW.Elapsed;
            stopW.Stop();
        }
        //
        public void GenerateMeshWithBackwardStep(double L2, double H2)
        {
            #region Тест
            //--------тест
            //flag = true;
            //_FinalMesh = null;
            //this.Area = new Area(0, 1, 1, 0, 9, 1);
            //L2 = 0.5;
            //H2 = 0.5;
            //Prs[0] = new AlgParameter(9, 9, 0, 1, 1);
            //------ конец теста
            #endregion
            #region генерация основной области
            //сдвижка основной области на L2 вправо
            for (int i = 0; i < this.Area.XBottom.Length; i++)
                this.Area.XBottom[i] += L2;
            for (int i = 0; i < this.Area.XTop.Length; i++)
                this.Area.XTop[i] += L2;
            // получение массива простых областей
            areas = GenerateSimpleAreas();
            // формирование массива под сетки простых областей
            Mesh[][] Meshes = new Mesh[areas.Length][];
            for (int i = 0; i < areas.Length; i++)
                Meshes[i] = new Mesh[areas[0].Length];
            //
            GenerateParameters();
            //
            for (int i = 0; i < areas.Length; i++)
            {
                mg[i].OpenCL = this.OpecCL;
                //
                for (int j = 0; j < areas[0].Length; j++)
                    Meshes[i][j] = mg[i].Generate(areas[i][j]);
            }
            //склейка простых подобластей
            for (int i = 0; i < Meshes.Length; i++)
            {
                for (int j = 1; j < Meshes[0].Length; j++)
                {
                    Meshes[i][0].Add(Meshes[i][j]);
                }
            }
            //
            for (int i = 1; i < Meshes.Length; i++)
            {
                Meshes[0][0].Add(Meshes[i][0]);
            }
            Mesh Main = Meshes[0][0];
            Main.Renumberation();
            
            //
            #endregion
            #region генерация области над ступенькой
            //генерация области над ступенькой
            double dx = (Area.XBottom[Area.Nx - 1] - Area.XBottom[0]) / (Area.Nx - 1);
            int Nx2 = (int)(L2 / dx)+1;
            double[] BX2 = new double[Nx2];
            double[] BY2 = new double[Nx2];
            double[] TX2 = new double[Nx2];
            double[] TY2 = new double[Nx2];
            //
            double dy = (Area.YTop[0]-Area.YBottom[0])/(Main.CountLeft-1);
            int Ny2 = (int)(H2 / dy)+1;
            int Ny1 = Main.CountLeft - Ny2;
            H2 = (Ny2-1) * dy;
            double H = Main.Y[Main.TopKnots[0]] - Main.Y[Main.BottomKnots[0]];
            double H1 = H - H2;
            for (int i = 0; i < Nx2; i++)
            {
                BX2[i] = i * dx;
                TX2[i] = i * dx;
                //
                BY2[i] = H1;
                TY2[i] = H;
            }
            //
            
            //
            SimpleArea StepArea = new MeshLibrary.SimpleArea(TX2, TY2, BX2, BY2);
            AlgParameter p2 = new AlgParameter();
            p2.Nx = Nx2;
            p2.Ny = Ny2;
            p2.P = 1;
            p2.Q = 1;
            //
            MeshGenerator mg2 = new AlgGenerator(p2);
            Mesh Step = mg2.Generate(StepArea);
            Step.Renumberation();
            #endregion
            // соедиение двух областей
            //    Ny2
            //    -------------------------
            // L2 |   Step  |             |
            //    |_________|             |
            //     0        |   Main      |
            //         H2   |             |
            //              |_____________|
            Mesh Final = new Mesh();
            int CountKnots = Step.CountKnots + Main.CountKnots - Step.CountLeft;
            int CountStepKnots = Step.CountKnots - Step.CountLeft;
            int CountStepElems = Step.CountElems;
            //координаты
            Final.X = new double[CountKnots];
            Final.Y = new double[CountKnots];
            for (int i = 0; i < CountStepKnots; i++)
            {
                Final.X[i] = Step.X[i];
                Final.Y[i] = Step.Y[i];
            }
            for (int i = 0; i < Main.CountKnots; i++)
            {
                Final.X[CountStepKnots+i] = Main.X[i];
                Final.Y[CountStepKnots+i] = Main.Y[i];
            }
            // элементы
            Final.AreaElems = new int[Step.CountElems + Main.CountElems][];
            for (int i = 0; i < Step.CountElems; i++)
            {
                Final.AreaElems[i] = new int[3];
                for (int j = 0; j < 3; j++)
                {
                    Final.AreaElems[i][j] = Step.AreaElems[i][j];
                    // замена нумерации для правой границы стуеньки
                    if (Step.RightKnots.Any(array => Final.AreaElems[i][j] == array))
                        Final.AreaElems[i][j] += Ny1;

                }
            }
            for (int i = 0; i < Main.CountElems; i++)
            {
                Final.AreaElems[CountStepElems + i] = new int[3];
                Final.AreaElems[CountStepElems + i][0] = Main.AreaElems[i][0] + CountStepKnots;
                Final.AreaElems[CountStepElems + i][1] = Main.AreaElems[i][1] + CountStepKnots;
                Final.AreaElems[CountStepElems + i][2] = Main.AreaElems[i][2] + CountStepKnots;
            }
            // граничные узлы
            Final.LeftKnots = new int[Step.CountLeft];
            Final.RightKnots = new int[Main.CountRight];
            Final.TopKnots = new int[Step.CountTop + Main.CountTop - 1];
            Final.BottomKnots = new int[Step.CountBottom + Main.CountBottom + (Main.CountLeft - Step.CountLeft) - 1];
            for (int i = 0; i < Step.CountLeft; i++)
                Final.LeftKnots[i] = Step.LeftKnots[i];
            for (int i = 0; i < Main.CountRight; i++)
                Final.RightKnots[i] = Main.RightKnots[i] + CountStepKnots;
            for (int i = 0; i < Main.CountTop; i++)
                Final.TopKnots[i] = Main.TopKnots[i] + CountStepKnots;
            for (int i = 1; i < Step.CountTop; i++)
                Final.TopKnots[Main.CountTop + i - 1] = Step.TopKnots[i];
            for (int i = 0; i < Step.CountBottom; i++)
                Final.BottomKnots[i] = Step.BottomKnots[i];
            int ch = Step.CountBottom-1;
            for (int i = Step.CountLeft-1; i <Main.CountLeft; i++)
                Final.BottomKnots[ch++] = Main.LeftKnots[i] + CountStepKnots;
            for (int i = 1; i < Main.CountBottom; i++)
                Final.BottomKnots[ch++] = Main.BottomKnots[i] + CountStepKnots;
             //
            Final.CountLeft = Final.LeftKnots.Length;
            Final.CountRight = Final.RightKnots.Length;
            Final.CountBottom = Final.BottomKnots.Length;
            Final.CountTop = Final.TopKnots.Length;
            Final.CountElems = Final.AreaElems.Length;
            Final.CountKnots = Final.X.Length;
            _FinalMesh = Final;
            //_FinalMesh = Main;
            _FinalMesh.TriangleGeometryCalculation();
        }

        private void GenerateParameters()
        {
            mg = new MeshGenerator[Prs.Length];
            for (int i = 0; i < areas.Length; i++)
            {

                switch (Prs[i].Method)
                {
                    case 0:
                        {
                            mg[i] = new AlgGenerator((AlgParameter)Prs[i]);
                            break;
                        }
                    case 1:
                        {
                            mg[i] = new DiffGenerator((DiffParameter)Prs[i]);
                            break;
                        }
                    case 2:
                        {
                            mg[i] = new FletcherGenerator((FletcherParameter)Prs[i]);
                            break;
                        }
                    case 3:
                        {
                            mg[i] = new OrthoGenerator((OrthoParameter)Prs[i]);
                            break;
                        }
                        
                }
                mg[i].OpenCL = OpenCL;
                mg[i].Cuda = Cuda;
               
            }
        }

       

        private SimpleArea[][] GenerateSimpleAreas()
        {
            int partsCount = Area.partsCount;
            int layerCount = 1;
            //Если есть нижний слой
            if (Area.bottomLayer != 0)
                layerCount++;
            //Если есть врехний слой
            if (Area.topLayer != 0)
                layerCount++;
            //массив простых областей
            SimpleArea[][] areas = new SimpleArea[layerCount][];
            for (int i = 0; i < layerCount; i++)
                areas[i] = new SimpleArea[partsCount];
            //координаты верхней границы
            int Nx = Area.Nx; int partNx = Nx / partsCount;
            int idx = 0, m = 0;
            double[][] xTop, xBottom, yTop, yBottom;
            xTop = new double[partsCount][];
            xBottom = new double[partsCount][];
            yTop = new double[partsCount][];
            yBottom = new double[partsCount][];
            for (int i = 0; i < partsCount; i++)
            {
                if (i == 0)
                {
                    xTop[i] = new double[partNx];
                    xBottom[i] = new double[partNx];
                    yTop[i] = new double[partNx];
                    yBottom[i] = new double[partNx];
                }
                else
                {
                    xTop[i] = new double[partNx + 1];
                    xBottom[i] = new double[partNx + 1];
                    yTop[i] = new double[partNx + 1];
                    yBottom[i] = new double[partNx + 1];
                }
            }
            //
            idx = 0;
            for (int i = 0; i < partsCount; i++)
            {
                for (int j = 0; j < xTop[i].Length; j++)
                {
                    xTop[i][j] = Area.XTop[idx];
                    yTop[i][j] = Area.YTop[idx];
                    //
                    idx++;
                }
                idx--;
            }
            // если есть верхний слой -  граница X1
            if (Area.X1 != null)
            {
                idx = 0;
                for (int i = 0; i < partsCount; i++)
                {
                    for (int j = 0; j < xBottom[i].Length; j++)
                    {
                        xBottom[i][j] = Area.X1[idx];
                        yBottom[i][j] = Area.Y1[idx];
                        //
                        idx++;
                    }
                    idx--;
                    //
                    areas[m][i] = new SimpleArea(xTop[i], yTop[i], xBottom[i], yBottom[i]);
                }
                //
                m++;
                //если есть нижний слой - граница X2
                if (Area.X2 != null)
                {
                    idx = 0;
                    for (int i = 0; i < partsCount; i++)
                    {
                        for (int j = 0; j < xTop[i].Length; j++)
                        {
                            // поменяла местами массивы верхней и нижней границы (низ одной области - это верх другой)
                            xTop[i][j] = Area.X2[idx];
                            yTop[i][j] = Area.Y2[idx];
                            //
                            idx++;
                        }
                        idx--;
                        //
                        areas[m][i] = new SimpleArea(xBottom[i], yBottom[i], xTop[i], yTop[i]);
                    }
                    //
                    m++;
                    //
                    idx = 0;
                    for (int i = 0; i < partsCount; i++)
                    {
                        for (int j = 0; j < xBottom[i].Length; j++)
                        {
                            xBottom[i][j] = Area.XBottom[idx];
                            yBottom[i][j] = Area.YBottom[idx];
                            //
                            idx++;
                        }
                        idx--;
                        //
                        areas[m][i] = new SimpleArea(xTop[i], yTop[i], xBottom[i], yBottom[i]);
                    }
                    m++;
                }
                    // если нижнего слоя нет - нижняя граница
                else
                {
                    idx = 0;
                    for (int i = 0; i < partsCount; i++)
                    {
                        for (int j = 0; j < xTop[i].Length; j++)
                        {
                            xTop[i][j] = Area.X1[idx];
                            yTop[i][j] = Area.Y1[idx];
                            xBottom[i][j] = Area.XBottom[idx];
                            yBottom[i][j] = Area.YBottom[idx];
                            //
                            idx++;
                        }
                        idx--;
                        //
                        areas[m][i] = new SimpleArea(xTop[i], yTop[i], xBottom[i], yBottom[i]);
                    }
                    m++;
                }
            }
                //если верхнего слоя нет, а нижний есть - граница X2
            else if (Area.X2 != null)
            {
                idx = 0;
                for (int i = 0; i < partsCount; i++)
                {
                    for (int j = 0; j < xBottom[i].Length; j++)
                    {
                        xBottom[i][j] = Area.X2[idx];
                        yBottom[i][j] = Area.Y2[idx];

                        //
                        idx++;
                    }
                    idx--;
                    //
                    areas[m][i] = new SimpleArea(xTop[i], yTop[i], xBottom[i], yBottom[i]);
                }
                m++;
                // нижняя граница
                idx = 0;
                for (int i = 0; i < partsCount; i++)
                {
                    for (int j = 0; j < xTop[i].Length; j++)
                    {
                        xTop[i][j] = Area.XBottom[idx];
                        yTop[i][j] = Area.YBottom[idx];
                        //
                        idx++;
                    }
                    idx--;
                    //
                    areas[m][i] = new SimpleArea(xBottom[i], yBottom[i], xTop[i], yTop[i]);
                }
                m++;
            }
                // если область без подслоев - нижняя граница
            else
            {
                idx = 0;
                for (int i = 0; i < partsCount; i++)
                {
                    for (int j = 0; j < xBottom[i].Length; j++)
                    {
                        xBottom[i][j] = Area.XBottom[idx];
                        yBottom[i][j] = Area.YBottom[idx];
                        //
                        idx++;
                    }
                    idx--;
                    //
                    areas[m][i] = new SimpleArea(xTop[i], yTop[i], xBottom[i], yBottom[i]);
                }
            }
            //
            return areas;
        }
        
    }
}
