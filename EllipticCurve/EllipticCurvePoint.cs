using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Data.SQLite;


namespace EllipticCurve
{
    public class EllipticCurvePoint
    {
        #region Поля

        private BigInteger p; // GF(p)
        public BigInteger P
        {
            get { return p; }
        }
        private BigInteger a; // коэфф прямой
        public BigInteger A
        {
            get { return a; }
        }
        private BigInteger b; // коэфф прямой
        public BigInteger B
        {
            get { return b; }
        }
        private BigInteger x; // координата х 
        public BigInteger X
        {
            get { return x; }
            set { x = value; }
        }
        private BigInteger y; // координата y 
        public BigInteger Y
        {
            get { return y; }
            set { y = value; }
        }
        private BigInteger z; // координата z 
        public BigInteger Z
        {
            get { return z; }
            set { z = value; }
        }
        private BigInteger n;// порядок точки G
        public BigInteger N
        {
            get { return n; }
        }
        private int blockSize;  // Размер блока шифрования, (длина ключа -1 ) в байтах
        public int BlockSize
        {
            get { return blockSize; }
        }
        #endregion

        #region Building
        public EllipticCurvePoint(BigInteger P, BigInteger A, BigInteger B, BigInteger N, BigInteger X, BigInteger Y, BigInteger Z, int _blockSize)
        {
            p = P;
            a = A;
            b = B;
            n = N;
            x = X;
            y = Y;
            z = Z;
            blockSize = _blockSize;
        }
        public EllipticCurvePoint(int type)
        {
            DataTable dt = GetCurve(type.ToString());
            DataRow curve = dt.AsEnumerable().FirstOrDefault();
            switch (type)
            {    
                case 192:
                   // DataTable dt = GetCurve("192");
                  //  DataRow curve = dt.AsEnumerable().FirstOrDefault();
                    p = BigInteger.Parse(curve.Field<string>("p").ToString());
                    n = BigInteger.Parse(curve.Field<string>("n").ToString());
                    a = BigInteger.Parse(curve.Field<string>("a").ToString());
                    b = BigInteger.Parse(curve.Field<string>("b").ToString());
                    x = BigInteger.Parse(curve.Field<string>("x").ToString());
                    y = BigInteger.Parse(curve.Field<string>("y").ToString());
                    z = BigInteger.Parse("1");
                    blockSize =Convert.ToInt32(curve.Field<Int64>("block"));
                    break;
                case 256:
				//	DataTable dt = GetCurve("256");
                 //   var curve = dt.AsEnumerable().FirstOrDefault();
                    p = BigInteger.Parse(curve.Field<string>("p").ToString());
                    n = BigInteger.Parse(curve.Field<string>("n").ToString());
                    a = BigInteger.Parse(curve.Field<string>("a").ToString());
                    b = BigInteger.Parse(curve.Field<string>("b").ToString(), System.Globalization.NumberStyles.HexNumber);
                    x = BigInteger.Parse(curve.Field<string>("x").ToString(), System.Globalization.NumberStyles.HexNumber);
                    y = BigInteger.Parse(curve.Field<string>("y").ToString(), System.Globalization.NumberStyles.HexNumber);
                    z = BigInteger.Parse("1");
                    blockSize = Convert.ToInt32(curve.Field<Int64>("block"));
                    break;
                case 384:
                 //   DataTable dt = GetCurve("384");
                 //   var curve = dt.AsEnumerable().FirstOrDefault();
                    p = BigInteger.Parse(curve.Field<string>("p").ToString());
                    n = BigInteger.Parse(curve.Field<string>("n").ToString());
                    a = BigInteger.Parse(curve.Field<string>("a").ToString());
                    b = BigInteger.Parse(curve.Field<string>("b").ToString(), System.Globalization.NumberStyles.HexNumber);
                    x = BigInteger.Parse(curve.Field<string>("x").ToString(), System.Globalization.NumberStyles.HexNumber);
                    y = BigInteger.Parse(curve.Field<string>("y").ToString(), System.Globalization.NumberStyles.HexNumber);
                    z = BigInteger.Parse("1");
                    blockSize = Convert.ToInt32(curve.Field<Int64>("block"));
                    break;
                case 521:
                  //  DataTable dt = GetCurve("521");
                  //  var curve = dt.AsEnumerable().FirstOrDefault();
                    p = BigInteger.Parse(curve.Field<string>("p").ToString());
                    n = BigInteger.Parse(curve.Field<string>("n").ToString());
                    a = BigInteger.Parse(curve.Field<string>("a").ToString());
                    b = BigInteger.Parse(curve.Field<string>("b").ToString(), System.Globalization.NumberStyles.HexNumber);
                    x = BigInteger.Parse(curve.Field<string>("x").ToString(), System.Globalization.NumberStyles.HexNumber);
                    y = BigInteger.Parse(curve.Field<string>("y").ToString(), System.Globalization.NumberStyles.HexNumber);
                    z = BigInteger.Parse("1");
                    blockSize = Convert.ToInt32(curve.Field<Int64>("block"));
                    break;
            }
        }       
        #endregion

        #region Сложение точек
        public static EllipticCurvePoint AddPoint(EllipticCurvePoint pointA, EllipticCurvePoint pointB)
        {
            if ((pointA.X == pointA.Z) && (pointA.X == 0)) { return pointB; } 
            if ((pointB.X == pointB.Z) && (pointB.X == 0)) { return pointA; }       

            BigInteger A = pointB.Y * pointA.Z - pointA.Y * pointB.Z;
            A = BigInteger.Remainder(A, pointA.P);
            if (A < 0) { while (A < 0) { A += pointA.P; } }

            BigInteger B = pointB.X * pointA.Z - pointA.X * pointB.Z;
            B = BigInteger.Remainder(B, pointA.P);
            if (B < 0) { while (B < 0) { B += pointA.P; } }

            BigInteger C = pointB.X * pointA.Z + pointA.X * pointB.Z;
            C = BigInteger.Remainder(C, pointA.P);
            if (C < 0) { while (C < 0) { C += pointA.P; } }

            BigInteger D = pointB.X * pointA.Z + 2 * pointA.X * pointB.Z;
            D = BigInteger.Remainder(D, pointA.P);
            if (D < 0) { while (D < 0) { D += pointA.P; } }

            BigInteger E = pointB.Z * pointA.Z;
            E = BigInteger.Remainder(E, pointA.P);
            if (E < 0) { while (E < 0) { E += pointA.P; } }

            BigInteger X = B * (E * A * A - C * B * B);
            X = BigInteger.Remainder(X, pointA.P);
            if (X < 0) { while (X < 0) { X += pointA.P; } }

            BigInteger Y = A * (B * B * D - A * A * E) - pointA.Y * pointB.Z * B * B * B;
            Y = BigInteger.Remainder(Y, pointA.P);
            if (Y < 0) { while (Y < 0) { Y += pointA.P; } }

            BigInteger Z = B * B * B * E;
            Z = BigInteger.Remainder(Z, pointA.P);
            if (Z < 0) { while (Z < 0) { Z += pointA.P; } }


            if (X == 0 && Z == 0)
            {
                return new EllipticCurvePoint(pointA.P, pointA.A, pointA.B, pointA.N, 0, 1, 0,pointA.BlockSize);
            }

            return new EllipticCurvePoint(pointA.P, pointA.A, pointA.B, pointA.N, X, Y, Z, pointA.BlockSize);
        }
        private static EllipticCurvePoint X2(EllipticCurvePoint point)
        {
            BigInteger A = 3 * point.X * point.X + point.A * point.Z * point.Z;
            A = BigInteger.Remainder(A, point.P);
            if (A < 0) { while (A < 0) { A += point.P; } }


            BigInteger B = 2 * point.Y * point.Z;
            B = BigInteger.Remainder(B, point.P);
            if (B < 0) { while (B < 0) { B += point.P; } }


            BigInteger X = B * (A * A - 4 * point.X * point.Y * B);
            X = BigInteger.Remainder(X, point.P);
            if (X < 0) { while (X < 0) { X += point.P; } }


            BigInteger Y = A * (6 * point.Y * point.X * B - A * A) - 2 * point.Y * point.Y * B * B;
            Y = BigInteger.Remainder(Y, point.P);
            if (Y < 0) { while (Y < 0) { Y += point.P; } }


            BigInteger Z = B * B * B;
            Z = BigInteger.Remainder(Z, point.P);
            if (Z < 0) { while (Z < 0) { Z += point.P; } }
            return new EllipticCurvePoint(point.P, point.A, point.B, point.N, X, Y, Z,point.BlockSize);
        }
        public static EllipticCurvePoint Multiply(BigInteger k, EllipticCurvePoint point)
        {
            EllipticCurvePoint temp = point;
            k--;
            while (k != 0)
            {
                if (BigInteger.Remainder(k, 2) != 0)
                {
                    if ((temp.X == point.X) && (temp.Y == point.Y))
                        temp = X2(temp);
                    else
                        temp = AddPoint(temp, point);
                    k--;
                }
                k = k / 2;
                point = X2(point);
            }
            return temp;
        }
        #endregion

        
        public  EllipticCurvePoint SearchPoint(BigInteger message, out int correct)
        {
            correct = 0;
            BigInteger xcoord = BigInteger.Remainder((message * message * message + A * message + B), P); // Правая часть уравнения кривой
            BigInteger q = BigInteger.ModPow(xcoord, (P - 1) / 2,P); // если q=1 то xcoord квадратичный вычет, иначе нет
            if (q != 1)
            {
                 for (correct = 1; correct < 255; correct++)
                 {
                     BigInteger temptext = message - correct;
                     BigInteger xcoordt = BigInteger.Remainder((temptext * temptext * temptext +A * temptext + B), P);
                     BigInteger lezhandrt = BigInteger.ModPow(xcoordt, (P - 1) / 2, P);
                     if (lezhandrt == 1)
                     {
                         message = message - correct;
                         xcoord = xcoordt;
                         break;
                     }
                 }
                    // максимальная корректировка полученная опытным путем 19
             }
            return new EllipticCurvePoint(P, A, B, N, message, BigInteger.ModPow(xcoord, ((P + 1) / 4), P), 1, BlockSize);
        }   // Возвращает точку на кривой в которой размещено сообщение
        public static string CompressPoint(EllipticCurvePoint point)
        {
            string compress = "";
            BigInteger X = point.X;
            byte[] temp = X.ToByteArray();
            byte[] tochka = new byte[temp.Count() + 1];
            if (BigInteger.Remainder(point.Y,2) == 0) { tochka[0] = 2; }
            else { tochka[0] = 3; }
            for (int i = 1; i < temp.Count() + 1; i++)
            {
                tochka[i] = temp[i - 1];
            }
            Array.Reverse(tochka,1,tochka.Count()-1);
            compress = BitConverter.ToString(tochka, 0).Replace("-", "");

            return compress;
        } // Сжатие точки 
        public static EllipticCurvePoint UnCompressPoint(string compressP, EllipticCurvePoint g)
        {
            BigInteger X, Y = new BigInteger();
            EllipticCurvePoint point; 
            byte[] compressPointX = new byte[compressP.Count() / 2 - 1];

            StringBuilder s = new StringBuilder();
            s.Append(compressP[0]);
            s.Append(compressP[1]);
            compressP = compressP.Remove(0, 2);
            byte y = Convert.ToByte(s.ToString(),16); //байт четности

            X = BigInteger.Parse(compressP, System.Globalization.NumberStyles.HexNumber);
            BigInteger xcord = BigInteger.Remainder((X * X * X + g.A * X + g.B), g.P);
            Y = BigInteger.ModPow(xcord, ((g.P + 1) / 4), g.P);
            if(y==2)
            {
                if(BigInteger.Remainder(Y,2)==0)
                {
                    point = new EllipticCurvePoint(g.P,g.A,g.B,g.N,X,Y,1,g.BlockSize);
                    return point;
                }
                else
                {
                    point = new EllipticCurvePoint(g.P, g.A, g.B, g.N, X, g.P-Y, 1, g.BlockSize);
                    return point;
                }
            }
            else
            {
                if (BigInteger.Remainder(Y, 2) != 0)
                {
                    point = new EllipticCurvePoint(g.P, g.A, g.B, g.N, X, Y, 1, g.BlockSize);
                    return point;
                }
                else
                {
                    point = new EllipticCurvePoint(g.P, g.A, g.B, g.N, X, g.P - Y, 1, g.BlockSize);
                    return point;
                }
            }
        }
        public static EllipticCurvePoint AffineCoords(EllipticCurvePoint p)
        {
            p.X = BigInteger.Remainder(p.X * Additional.Inverse(p.Z, p.P), p.P);
            p.Y = BigInteger.Remainder(p.Y * Additional.Inverse(p.Z, p.P), p.P);
            p.Z = 1;
            return p;
            
        } // Приведение к афинным координатам
        static DataTable GetCurve(string type)
        {
            DataTable dt = new DataTable();
            string conn = @"Data Source= " + string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "CurveParam.db");
            SQLiteDataAdapter dAdapter = new SQLiteDataAdapter("SELECT * FROM curve WHERE type ='" + type + "'", conn);
            dAdapter.Fill(dt);
            return dt;
        }
        public override string ToString()
        {
            string temp = "X= " + X +" Y= " + Y + " Z= " + Z;
            return temp;
        }
    }
}
