using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace EllipticCurve
{
    class ParallelDecoding
    {
        private BigInteger Key;
        public byte[][] Message { get; set; }
        public Shifr[] Sh { get; set; }
        private EllipticCurvePoint KeyPara1;
        private int lengtshifr;

        public ParallelDecoding(BigInteger key,Shifr[] shifr)
        {
            Key=key;
            Sh = shifr;
            lengtshifr = shifr.Count();
            Message = new byte[lengtshifr][];

            // Вычисляем точку Key*K*pointG
            KeyPara1 = EllipticCurvePoint.Multiply(Key, shifr[0].PointA);
            // привод к аффиным координатам и смена знака координаты Y(Для вычитания)
            KeyPara1 = EllipticCurvePoint.AffineCoords(KeyPara1);
            KeyPara1.Y = -KeyPara1.Y;
            // конец
        }

        public void Func0()
        {
            for (int i = 0; i < lengtshifr; i+=4)
            {
                Shifr _sh = Sh[i];         
                EllipticCurvePoint para2 = _sh.PointB;
                EllipticCurvePoint t = EllipticCurvePoint.AddPoint(para2, KeyPara1);
                BigInteger q = t.X * Additional.Inverse(t.Z, t.P);
                q = BigInteger.Remainder(q, t.P);
                if (_sh.Correct != 0)
                {
                    q += _sh.Correct;
                }
   
                Message[i] = q.ToByteArray(); 
            }
        }
        public void Func1()
        {
            for (int i = 1; i < lengtshifr; i += 4)
            {
                Shifr _sh = Sh[i];
                EllipticCurvePoint para2 = _sh.PointB;
                EllipticCurvePoint t = EllipticCurvePoint.AddPoint(para2, KeyPara1);
                BigInteger q = t.X * Additional.Inverse(t.Z, t.P);
                q = BigInteger.Remainder(q, t.P);
                if (_sh.Correct != 0)
                {
                    q += _sh.Correct;
                }

                Message[i] = q.ToByteArray();
            }
        }
        public void Func2()
        {
            for (int i = 2; i < lengtshifr; i += 4)
            {
                Shifr _sh = Sh[i];
                EllipticCurvePoint para2 = _sh.PointB;
                EllipticCurvePoint t = EllipticCurvePoint.AddPoint(para2, KeyPara1);
                BigInteger q = t.X * Additional.Inverse(t.Z, t.P);
                q = BigInteger.Remainder(q, t.P);
                if (_sh.Correct != 0)
                {
                    q += _sh.Correct;
                }

                Message[i] = q.ToByteArray();
            }
        }
        public void Func3()
        {
            for (int i = 3; i < lengtshifr; i += 4)
            {
                Shifr _sh = Sh[i];
                EllipticCurvePoint para2 = _sh.PointB;
                EllipticCurvePoint t = EllipticCurvePoint.AddPoint(para2, KeyPara1);
                BigInteger q = t.X * Additional.Inverse(t.Z, t.P);
                q = BigInteger.Remainder(q, t.P);
                if (_sh.Correct != 0)
                {
                    q += _sh.Correct;
                }

                Message[i] = q.ToByteArray();
            }
        }

        public void FuncNoParalel()
        {
            for (int i = 0; i < lengtshifr; i++)
            {
                Shifr _sh = Sh[i];
                EllipticCurvePoint para2 = _sh.PointB;
                EllipticCurvePoint t = EllipticCurvePoint.AddPoint(para2, KeyPara1);
                BigInteger q = t.X * Additional.Inverse(t.Z, t.P);
                q = BigInteger.Remainder(q, t.P);
                if (_sh.Correct != 0)
                {
                    q += _sh.Correct;
                }

                Message[i] = q.ToByteArray();
            }
        }
    }
}
