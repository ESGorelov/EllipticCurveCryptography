using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Diagnostics;


namespace EllipticCurve
{
    public class EllipticCurveCryptoProvider
    {
        public class Encryption
        {
            private EllipticCurvePoint Point;
            private EllipticCurvePoint openKey;
           
            public Encryption(string pathPublicKey)
            {
                #region открытие ключа
                StringBuilder sb = new StringBuilder();
                byte[] inputfile = File.ReadAllBytes(pathPublicKey);
                byte rem = inputfile[0];
                for (int i = 1; i < inputfile.Count() - 1; i++)
                {
                    sb.Append(Convert.ToString(Convert.ToByte(inputfile[i]), 2).FillWithZero(8));
                }
                sb.Append(Convert.ToString(Convert.ToByte(inputfile[inputfile.Count() - 1]), 2).FillWithZero(rem));
                #endregion
                if(sb.ToString(0,1).Equals("1")) //  если кривая заданна в программе
                {
                    EllipticCurvePoint g = new EllipticCurvePoint(192);
                    if(sb.ToString(1,2).Equals("00"))
                    {
                      //  g = new EllipticCurvePoint(192);
                    }
                    else if(sb.ToString(1,2).Equals("01"))
                    {
                        g = new EllipticCurvePoint(256);
                    }
                    else if (sb.ToString(1, 2).Equals("10"))
                    {
                        g = new EllipticCurvePoint(384);
                    }
                    else if (sb.ToString(1, 2).Equals("11"))
                    {
                        g = new EllipticCurvePoint(521);
                    }
                    else
                    {
                        throw new Exception("Файл поврежден. Невозможно определить тип кривой.");
                    }

                    int count = Convert.ToInt32(sb.ToString(3,8), 2);
                    List<byte> point = new List<byte>();
                    for (int j = 0; j < count; j++)
                    {
                        byte temp = Convert.ToByte(sb.ToString(11 + 8 * j, 8), 2);
                        point.Add(temp);
                    }
                    sb = sb.Remove(0, 8 * count + 11);
                    string _second = BitConverter.ToString(point.ToArray(), 0).Replace("-", ""); // сжатая точка 
                    openKey = EllipticCurvePoint.UnCompressPoint(_second, g);

                    count = Convert.ToInt32(sb.ToString(0, 8), 2);
                    point = new List<byte>();
                    for (int j = 0; j < count; j++)
                    {
                        byte temp = Convert.ToByte(sb.ToString(8 + 8 * j, 8), 2);
                        point.Add(temp);
                    }
                    _second = BitConverter.ToString(point.ToArray(), 0).Replace("-", ""); // сжатая точка 
                    Point = EllipticCurvePoint.UnCompressPoint(_second, g);
                }
                else
                {
                    throw new Exception("Файл поврежден");
                }
            }
            
            public void Encrypt(string pathfile, string outshifr)
            {
                StreamWriter sw = new StreamWriter(new FileStream("C:\\1\\отчеты\\шифрование.txt", FileMode.Create));
                Stopwatch st = new Stopwatch();
                FileInfo fi = new FileInfo(pathfile);
                if (Point == null || openKey == null) { throw new Exception("Окрытый ключ не зарегистрированн"); };
                byte[][] message = EllipticCurve.Additional.CutFile(pathfile, Point.BlockSize);
                st.Start();
                Shifr[] shmessage = EllipticCurve.EllipticCryptography.encrypt(message, openKey, Point);
                st.Stop();
                sw.WriteLine("Шифрование данных закончено: " + st.Elapsed);
                st.Reset();
                sw.WriteLine();
                st.Start();
                EllipticCurve.Shifr.SaveShifr(outshifr+"\\",fi.Name, EllipticCurve.Shifr.OutShifr(shmessage));
                st.Stop();
                sw.WriteLine("Ввывод шифра закончен: " + st.Elapsed);
                sw.WriteLine();
                sw.Close();
            }
            public string EncryptStr(string strmessage)
            {
                if (Point == null || openKey == null) { throw new Exception("Окрытый ключ не зарегистрированн"); };
                byte[][] message = EllipticCurve.Additional.StringCut(strmessage, Point.BlockSize);

                Shifr[] shmessage = EllipticCurve.EllipticCryptography.encrypt(message, openKey, Point);

                string shifr = EllipticCurve.Shifr.OutShifr(shmessage);
                return shifr;
            }

        }
        public class Decryption
        {
            private EllipticCurvePoint Point;
            private BigInteger closeKey;

            public Decryption(string pathPrivateKey)
            {
                #region открытие ключа
                StringBuilder sb = new StringBuilder();
                byte[] inputfile = File.ReadAllBytes(pathPrivateKey);
                byte rem = inputfile[0];
                for (int i = 1; i < inputfile.Count() - 1; i++)
                {
                    sb.Append(Convert.ToString(Convert.ToByte(inputfile[i]), 2).FillWithZero(8));
                }
                sb.Append(Convert.ToString(Convert.ToByte(inputfile[inputfile.Count() - 1]), 2).FillWithZero(rem));
                #endregion
                if (sb.ToString(0, 1).Equals("1")) //  если кривая заданна в программе
                {
                    EllipticCurvePoint g = new EllipticCurvePoint(192);
                    if (sb.ToString(1, 2).Equals("00"))
                    {
                        //  g = new EllipticCurvePoint(192);
                    }
                    else if (sb.ToString(1, 2).Equals("01"))
                    {
                        g = new EllipticCurvePoint(256);
                    }
                    else if (sb.ToString(1, 2).Equals("10"))
                    {
                        g = new EllipticCurvePoint(384);
                    }
                    else if (sb.ToString(1, 2).Equals("11"))
                    {
                        g = new EllipticCurvePoint(521);
                    }
                    else
                    {
                        throw new Exception("Файл поврежден. Невозможно определить тип кривой.");
                    }

                    int count = Convert.ToInt32(sb.ToString(3, 8), 2);
                    byte[] _key = new byte[count];
                    for (int j = 0; j < count; j++)
                    {
                        byte temp = Convert.ToByte(sb.ToString(11 + 8 * j, 8), 2);
                        _key[j] = temp;
                    }
                    closeKey = new BigInteger(_key);
                    sb = sb.Remove(0, 8 * count + 11);

                    count = Convert.ToInt32(sb.ToString(0, 8), 2);
                    byte[] point = new byte[count];
                    for (int j = 0; j < count; j++)
                    {
                        byte temp = Convert.ToByte(sb.ToString(8 + 8 * j, 8), 2);
                        point[j] = temp;
                    }
                    string _second = BitConverter.ToString(point, 0).Replace("-", ""); // сжатая точка 
                    Point = EllipticCurvePoint.UnCompressPoint(_second, g);
                }
                else
                {
                    throw new Exception("Файл поврежден");
                }
                
            }

            public void Decrypt(string pathshifr, string outfile)
            {
                StreamWriter sw = new StreamWriter(new FileStream("C:\\1\\отчеты\\дешифрование.txt", FileMode.Create));
                Stopwatch st = new Stopwatch();
                if (Point == null || closeKey == null) { throw new Exception("Окрытый ключ не зарегистрированн"); };
                st.Start();
                Shifr[] shifr = EllipticCurve.Shifr.InShifr(EllipticCurve.Shifr.OpenShifr(pathshifr), Point);
                st.Stop();
                sw.WriteLine("Ввод шифра закончен: " + st.Elapsed);
                sw.WriteLine();
                st.Reset();
                st.Start();
                byte[] mess = EllipticCurve.EllipticCryptography.decrypt(shifr, closeKey);
                st.Stop();
                sw.WriteLine("Расшифровка данных законченна: " + st.Elapsed);
                sw.WriteLine();
                FileInfo f = new FileInfo(pathshifr);
                EllipticCurve.Additional.ByteFile(outfile + "\\" + f.Name.Replace(".ecsh", ""), mess);
                sw.Close();
            }
            public string DecryptStr(string strshifr)
            {
                return this.ToString();
            }
        }
  
        public static void CreateKey(string path,int type)
        {
            EllipticCryptography.createKey(path, type);
        }

    }
}
