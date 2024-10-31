using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
     [Serializable]
    public abstract class Algorythm
    {
        public bool OpenCL = false;
        public double Discrepancy = 0.00001;
        public string error = "";
         //
        public abstract string Name
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="Array"></param>
        /// <param name="flag"></param>
        public abstract void Solve(SSquare A);
        //
        public abstract void Solve(SBand A);
        //
        public abstract void Solve(SRowPacked A);
        //
        public virtual void Forwarding(SSquare A, int[] BoundAdresses)
        { }
        public virtual void BackSolve(SSquare A)
        { }
        public virtual void Forwarding(SBand A, int[] BoundAdresses)
        { }
        public virtual void BackSolve(SBand A)
        { }
        public virtual void Forwarding(SRowPacked A, int[] BoundAdresses)
        { }
        public virtual void BackSolve(SRowPacked A)
        { }
    }
}
