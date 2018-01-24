using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace EllipticCurve
{
    class ParallelEncoding
    {
        private BigInteger Key { get; set; }
        private byte[][] Message { get; set; }
        private EllipticCurvePoint PublicB { get; set; }
        private EllipticCurvePoint PointG { get; set; }
        private EllipticCurvePoint kP { get; set; }
        private EllipticCurvePoint kPublicB { get; set; }
        public Shifr[] Shifr { get; set; }
        private int lengthmess;

        public ParallelEncoding(BigInteger key, byte[][] mess, EllipticCurvePoint _publicB, EllipticCurvePoint g)
        {
            Key = key;
            Message = mess;
            PublicB = _publicB;
            PointG = g;
            lengthmess = mess.GetLength(0);
            Shifr = new Shifr[lengthmess];
            // вычисление общей части шифрограммы
            kP = EllipticCurvePoint.Multiply(key, PointG);
            kP = EllipticCurvePoint.AffineCoords(kP);
            //Вычисление общего ключа
            kPublicB = EllipticCurvePoint.Multiply(Key, PublicB);
        
        }

        public void Func0()
        {
            for (int i = 0; i < lengthmess; i += 4 )
            {
                int correct = 0;
                BigInteger bigtext = new BigInteger(Message[i]); 
                EllipticCurvePoint pointtext = PointG.SearchPoint(bigtext, out correct);
                EllipticCurvePoint point2 = EllipticCurvePoint.AffineCoords(EllipticCurvePoint.AddPoint(pointtext, kPublicB));
                Shifr[i] = new Shifr(kP, point2, correct);
            }

        }
        public void Func1()
        {
            for (int i = 1; i < lengthmess; i += 4)
            {
                int correct = 0;
                BigInteger bigtext = new BigInteger(Message[i]);                       
                EllipticCurvePoint pointtext = PointG.SearchPoint(bigtext, out correct);
                EllipticCurvePoint point2 = EllipticCurvePoint.AffineCoords(EllipticCurvePoint.AddPoint(pointtext, kPublicB));
                Shifr[i] = new Shifr(kP, point2, correct);
            }

        }
        public void Func2()
        {
            for (int i = 2; i < lengthmess; i += 4)
            {
                int correct = 0;
                BigInteger bigtext = new BigInteger(Message[i]);     
                EllipticCurvePoint pointtext = PointG.SearchPoint(bigtext, out correct);
                EllipticCurvePoint point2 = EllipticCurvePoint.AffineCoords(EllipticCurvePoint.AddPoint(pointtext, kPublicB));
                Shifr[i] = new Shifr(kP, point2, correct);
            }
        }
        public void Func3()
        {
            for (int i = 3; i < lengthmess; i += 4)
            {
                int correct = 0;
                BigInteger bigtext = new BigInteger(Message[i]);     
                EllipticCurvePoint pointtext = PointG.SearchPoint(bigtext, out correct);
                EllipticCurvePoint point2 = EllipticCurvePoint.AffineCoords(EllipticCurvePoint.AddPoint(pointtext, kPublicB));
                Shifr[i] = new Shifr(kP, point2, correct);
            }

        }
        public void FuncNoParallel()
        {
            for (int i = 0; i < lengthmess; i++)
            {
                int correct = 0;
                BigInteger bigtext = new BigInteger(Message[i]);
                EllipticCurvePoint pointtext = PointG.SearchPoint(bigtext, out correct);
                EllipticCurvePoint point2 = EllipticCurvePoint.AffineCoords(EllipticCurvePoint.AddPoint(pointtext, kPublicB));
                Shifr[i] = new Shifr(kP, point2, correct);
            }

        }
    }
}
