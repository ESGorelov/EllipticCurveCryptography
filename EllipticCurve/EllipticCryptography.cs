using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using System.Security.Cryptography;
using System.IO;

namespace EllipticCurve
{
    class EllipticCryptography
    {
        public static Shifr[] encrypt(byte[][] message, EllipticCurvePoint publicB, EllipticCurvePoint _pointG)
        {            
            Shifr[] sh = new Shifr[message.GetLength(0)];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] key = new byte[15];
            rng.GetBytes(key);
            BigInteger k = new BigInteger(key);
            if (k < 0) { k = -k; }; 

            ParallelEncoding pe = new ParallelEncoding(k, message, publicB, _pointG);

            Task t1 = new Task(pe.Func0);
            Task t2 = new Task(pe.Func1);
            Task t3 = new Task(pe.Func2);
            t1.Start();
            t2.Start();
            t3.Start();
            pe.Func3();
            Task.WaitAll(t1,t2,t3);
          
            return pe.Shifr;
        }
        public static byte[] decrypt(Shifr[] sh, BigInteger secret)
        {
            int k = sh.Count();
            int sizeblock = sh[0].PointA.BlockSize;
            byte[][] mess;

            ParallelDecoding pd = new ParallelDecoding(secret, sh);
            Task t1 = new Task(pd.Func0);
            Task t2 = new Task(pd.Func1);
            Task t3 = new Task(pd.Func2);
            t1.Start();
            t2.Start();
            t3.Start();
            pd.Func3();
            Task.WaitAll(t1,t2,t3);
            mess = pd.Message;

            Additional.DeletePadding(ref mess[mess.Count() - 1],sizeblock);
            Additional.Recovery(ref mess, sizeblock);

            byte[] mess2 = new byte[sizeblock * (k - 1) + mess[k - 1].GetLength(0)];
            for (int i = 0; i < k - 1; i++)
            {
                for (int j = 0; j < sizeblock; j++)
                {
                    mess2[i * sizeblock + j] = mess[i][j];
                }
            }
            for (int i = 0; i < mess[k - 1].GetLength(0); i++)
            {
                mess2[(k - 1) * sizeblock + i] = mess[k - 1][i];

            }
            return mess2;
        }
        public static void createKey(string path, int typePoint)
        {
            EllipticCurvePoint g = new EllipticCurvePoint(typePoint);
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] _key = new byte[g.BlockSize];
            rng.GetBytes(_key);
            BigInteger key = new BigInteger(_key); // секрет 
            if (key < 0) { key = -key; };

            rng.GetBytes(_key);
            BigInteger t = new BigInteger(_key); //Случайное число, для нахождения случайной точки 
            if (t < 0) { t = -t; };
            int b;
            EllipticCurvePoint point = g.SearchPoint(t,out b);
            EllipticCurvePoint publicPoint = EllipticCurvePoint.Multiply(key, point); // публичная точка
            #region openkey
            StringBuilder openkey = new StringBuilder();
            // определение типа кривой
            openkey.Append('1');
            if(typePoint == 192)
            {
                openkey.Append("00");
            }
            else if(typePoint == 256)
            {
                openkey.Append("01");
            }
            else if (typePoint == 384)
            {
                openkey.Append("10");
            }
            else if (typePoint == 521)
            {
                openkey.Append("11");
            }
            // cохранение publicPoint
            publicPoint = EllipticCurvePoint.AffineCoords(publicPoint);
            string pointstr = EllipticCurvePoint.CompressPoint(publicPoint);
            openkey.Append(Convert.ToString(Convert.ToByte(pointstr.Count() / 2), 2).FillWithZero(8)); // кол-во байт в сжатой точке 
            openkey.Append(Additional.FromByteStrToBitStr(pointstr));
            // Сохранение point
            pointstr = EllipticCurvePoint.CompressPoint(point);
            openkey.Append(Convert.ToString(Convert.ToByte(pointstr.Count() / 2), 2).FillWithZero(8)); // кол-во байт в сжатой точке 
            openkey.Append(Additional.FromByteStrToBitStr(pointstr));
            // Вывод piblicKey
            List<byte> q = new List<byte>();
            string s = openkey.ToString();
            q.Add((byte)(s.Count() % 8));
            q.AddRange(s.ToByteList());
            File.WriteAllBytes(path + "\\" + Environment.MachineName + "_" + typePoint.ToString() + ".publickey", q.ToArray());
            #endregion
            #region closekey
            StringBuilder closekey = new StringBuilder();
            // определение типа кривой
            closekey.Append('1');
            if (typePoint == 192)
            {
                closekey.Append("00");
            }
            else if (typePoint == 256)
            {
                closekey.Append("01");
            }
            else if (typePoint == 384)
            {
                closekey.Append("10");
            }
            else if (typePoint == 521)
            {
                closekey.Append("11");
            }
            // сохранение ключа
            closekey.Append(Convert.ToString(Convert.ToByte(key.ToByteArray().Count()), 2).FillWithZero(8)); // кол-во байт в в ключе 
            closekey.Append(Additional.FromByteStrToBitStr(key.ToByteArray()));
            // Сохранение point
            pointstr = EllipticCurvePoint.CompressPoint(point);
            closekey.Append(Convert.ToString(Convert.ToByte(pointstr.Count() / 2), 2).FillWithZero(8)); // кол-во байт в сжатой точке 
            closekey.Append(Additional.FromByteStrToBitStr(pointstr));
            // Вывод piblicKey
            q = new List<byte>();
            s = closekey.ToString();
            q.Add((byte)(s.Count() % 8));
            q.AddRange(s.ToByteList());
            File.WriteAllBytes(path + "\\" + Environment.MachineName + "_" + typePoint.ToString() + ".privatekey", q.ToArray());
            #endregion
        }

        
    }
}
