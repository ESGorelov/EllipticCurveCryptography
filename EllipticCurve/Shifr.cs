using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EllipticCurve
{
    class Shifr
    {

        #region Поля
        public EllipticCurvePoint PointA
        {
            get;
            set;
        }
        public EllipticCurvePoint PointB
        {
            get;
            set;
        }      
        public int Correct
        {
            get;
            set;
        }
        #endregion
        public Shifr(EllipticCurvePoint para1,EllipticCurvePoint para2,int correct)
            {
                PointA = para1;       
                PointB = para2; 
                Correct = correct;
            }

        public static string OutShifr(Shifr[] sh)
        {
            StringBuilder outShifr = new StringBuilder();
            outShifr.Append(Convert.ToString(sh.Count(), 2).FillWithZero(24)); // количество шифрограмм 3 байта в начале файла
            EllipticCurvePoint kP = sh[0].PointA; // точка kP
            string kPs = EllipticCurvePoint.CompressPoint(kP);
            outShifr.Append(Convert.ToString(Convert.ToByte(kPs.Count() / 2), 2).FillWithZero(8)); // кол-во байт в сжатой точке 
            outShifr.Append(Additional.FromByteStrToBitStr(kPs));

            for(int i=0;i<sh.Count();i++) // Главный цикл по всем шифрованным блокам
            {             
                StringBuilder tempstr = new StringBuilder();          
                #region данные_о_корректировке      
                tempstr.Append(Convert.ToString(Convert.ToByte(sh[i].Correct), 2).FillWithZero(5));
                #endregion
                #region основная_точка
                string p = EllipticCurvePoint.CompressPoint(sh[i].PointB);
                tempstr.Append(Convert.ToString(Convert.ToByte(p.Count() / 2), 2).FillWithZero(8)); // кол-во байт в сжатой точке 
                tempstr.Append(Additional.FromByteStrToBitStr(p));
                #endregion
                outShifr.Append(Convert.ToString(Convert.ToInt32(tempstr.Length), 2).FillWithZero(16));
                outShifr.Append(tempstr.ToString());
            }
            return outShifr.ToString();
        }
        public static Shifr[] InShifr(string _str, EllipticCurvePoint g)
         {
            StringBuilder str = new StringBuilder(_str);
            int _countSh = Convert.ToInt32(str.ToString(0, 24), 2);
           
            Shifr[] sh = new Shifr[_countSh];

            int count = Convert.ToInt32(str.ToString(24, 8), 2);

            List<byte> pkP = new List<byte>();
            for(int i=0;i<count;i++)
            {
                byte temp = Convert.ToByte(str.ToString(32 + i * 8, 8), 2);
                pkP.Add(temp);
            }
            str = str.Remove(0, count * 8+32);

            byte[] pointkP = pkP.ToArray();
            string point1 = BitConverter.ToString(pointkP, 0).Replace("-", ""); // сжатая точка kP
            EllipticCurvePoint kP = EllipticCurvePoint.UnCompressPoint(point1, g);
            string[] strshifr = new string[_countSh];
            var tasks = new List<Task>();
            int currentchar = 0;
            for (int i = 0; i < _countSh; i++)
            {
                int lenstrcadr = Convert.ToInt32(str.ToString(currentchar, 16), 2);
                strshifr[i] = str.ToString(currentchar + 16, lenstrcadr);
                currentchar += lenstrcadr + 16;    
                var i1 = i;
                tasks.Add(Task.Factory.StartNew(delegate()
                {
                    int correct = 0; // корректировка   
                    correct = Convert.ToInt32(strshifr[i1].Substring(0, 5), 2);

                    int countSecondPoint = Convert.ToInt32(strshifr[i1].Substring(5, 8), 2);

                    var secondPoint = new List<byte>();
                    for (int j = 0; j < countSecondPoint; j++)
                    {
                        byte temp = Convert.ToByte(strshifr[i1].Substring(13 + 8 * j, 8), 2);
                        secondPoint.Add(temp);
                    }
                    string _second = BitConverter.ToString(secondPoint.ToArray(), 0).Replace("-", ""); // сжатая точка 
                    var second = EllipticCurvePoint.UnCompressPoint(_second, g);

                    sh[i1] = new Shifr(kP, second, correct);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return sh.ToArray();

         }
        public static void SaveShifr(string path,string filename, string shifr)
          {
              List<byte> q = new List<byte>();
              q.Add((byte)(shifr.Count() % 8));
              q.AddRange(shifr.ToByteList());
              File.WriteAllBytes(path + filename + ".ecsh", q.ToArray());
          }
        public static string OpenShifr(string path)
          {
              StringBuilder sb = new StringBuilder();
              byte[] inputfile = File.ReadAllBytes(path);
              byte rem = inputfile[0];
              if (rem == (byte)0)
              {
                  for (int i = 1; i < inputfile.Count(); i++)
                  {
                      sb.Append(Convert.ToString(inputfile[i], 2).FillWithZero(8));
                  }

                  return sb.ToString();
              }
              else
              {
                  for (int i = 1; i < inputfile.Count() - 1; i++)
                  {
                      sb.Append(Convert.ToString(inputfile[i], 2).FillWithZero(8));
                  }
                  sb.Append(Convert.ToString(inputfile[inputfile.Count() - 1], 2).FillWithZero(rem));
                  return sb.ToString();
              }
          }            
    }
}
