using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.IO;
using SevenZip.Compression.LZMA;

namespace EllipticCurve
{
    public static class Additional
    {
        public static BigInteger Inverse(BigInteger ch, BigInteger n)
        {
            BigInteger a = ch;
            BigInteger b = n, x = BigInteger.Zero, d = BigInteger.One;
            while (a.CompareTo(BigInteger.Zero) == 1)//a>0
            {
                BigInteger q = BigInteger.Divide(b, a);
                BigInteger y = a;
                a = BigInteger.Remainder(b, a);
                b = y;
                y = d;
                d = BigInteger.Subtract(x, BigInteger.Multiply(q, d));
                x = y;
            }
            x = BigInteger.Remainder(x, n);
            if (x.CompareTo(BigInteger.Zero) == -1)//x<0
            {
                x = BigInteger.Remainder(BigInteger.Add(x, n), n);
            }
            return x;
        } // обратный элемент в поле
        public static byte[][] StringCut(string a, int oneBlock)
        {
            byte[] y = Encoding.Default.GetBytes(a);
            int r = y.Count() / oneBlock;
            int r2 = y.Count() % oneBlock;
            byte[][] y2 = new byte[r + 1][];
            for (int i = 0; i < r; i++)
            {
                y2[i] = new byte[oneBlock];
                for (int j = 0; j < oneBlock; j++)
                {
                    y2[i][j] = y[i * oneBlock + j];

                }
            }
            y2[r] = new byte[r2];
            for (int i = 0; i < r2; i++)
            {
                // if (i >= r2) { y2[r][i] = (byte)0; }
                y2[r][i] = y[r * oneBlock + i];
            }
            return y2;
        }   // делит строку на блоки байт заданного размера
        public static string FillWithZero(this string value, int len)
        {
            while (value.Length < len)
            {
                value = "0" + value;
            }

            return value;
        } // Дополняет битовую строку до заданной длинны нулями
        public static string FromByteStrToBitStr(string bytestr)
        {
            StringBuilder bit = new StringBuilder();
            while (bytestr.Count() > 0)
            {
                StringBuilder t = new StringBuilder();
                t.Append(bytestr[0]);
                t.Append(bytestr[1]);
                bytestr = bytestr.Remove(0, 2);
                byte f = Byte.Parse(t.ToString(), System.Globalization.NumberStyles.HexNumber);
                bit.Append(Convert.ToString(Convert.ToByte(f), 2).FillWithZero(8));
            }
            return bit.ToString();
        } // возвращает битовую строку из байтовой строки
        public static string FromByteStrToBitStr(byte[] bytearray)
        {
            StringBuilder bit = new StringBuilder();
            for (int i = 0; i < bytearray.Count();i++ )
            {
                bit.Append(Convert.ToString(bytearray[i], 2).FillWithZero(8));
            }            
            return bit.ToString();
        } // возвращает битовую строку из массива байт
        public static List<byte> ToByteList(this string value)
        {
            List<byte> l = new List<byte>();

            int i = 0;
            for (i = 0; i < value.Length; i += 8)
            {
                string bs = "";
                if (i + 8 <= value.Length)
                {
                    bs = value.Substring(i, 8);
                }
                else
                {
                    bs = value.Substring(i, value.Length - i);
                }

                byte b = Convert.ToByte(bs, 2);

                l.Add(b);
            }

            return l;
        }  // преобразует битовую строку в список байт
        public static byte[][] CutFile(string path, int oneBlock)
        {
            //  byte[] temp = File.ReadAllBytes(path);
            //Сжатие LZMA(7-zip)
            var temp = SevenZipAdapter.Compress(File.ReadAllBytes(path));
            int lengthPadding = 0;
            int countBlock = (temp.Count() / oneBlock) + 1; 
            int rem = temp.Count() % oneBlock;
            if ((rem + 1) != oneBlock) 
            { 
                lengthPadding = oneBlock - (rem + 1);
            } 
            byte[][] mess = new byte[countBlock][];

            for (int i = 0; i < countBlock-1 ; i++)
            {
                mess[i] = new byte[oneBlock+1];
                for (int j = 0; j < oneBlock; j++)
                {
                    mess[i][j] = temp[i * oneBlock + j];
                }
                mess[i][oneBlock] = (byte)0;
            }
            mess[countBlock-1] = new byte[oneBlock+1];
            for (int j = 0; j < rem; j++)
            {
                 mess[countBlock-1][j]  = temp[(countBlock-1) * oneBlock + j];

            }
            for (int i = rem; i < oneBlock;i++ )
            {
                mess[countBlock - 1][ i] = (byte)lengthPadding;
            }
            mess[countBlock - 1][oneBlock] = (byte)0;
                return mess;
            
        } // Файл -> массив байт по блокам
        public static void ByteFile(string name, byte[] file)
        {
            //распаковка LZMA (7-zip)
            var temp = SevenZipAdapter.Decompress(file);
            File.WriteAllBytes(name, temp); 
        } // Массив -> файл
        public static void DeletePadding(ref byte[] block,int defaultblocksize)
        {
            var len = block.ToList();
            if(block.Count() == defaultblocksize)
            {
                byte lenpad = len[len.Count() - 1];
                len.RemoveRange(len.Count() - lenpad - 1, lenpad + 1);
                block = len.ToArray();
            } // Почему так? преобразование из byte[] в bigintger и обратно, если старшие байты равны 0 то они не раскодируются 
        } // Удаляет дополнение
        public static void Recovery(ref byte[][] blocks,int defaultblocksize)
        {
            int count = blocks.GetLength(0)-1; // последний блок не надо поверять
            for(int i = 0; i < count; i++)
            {
                if(blocks[i].Count()!= defaultblocksize)
                {
                    if(blocks[i].Count() > defaultblocksize)
                    {
                        var b = blocks[i].ToList();
                        b.RemoveAt(b.Count - 1);
                        blocks[i] = b.ToArray();
                    }
                    else if(blocks[i].Count() < defaultblocksize)
                    {
                        byte[] t = new byte[defaultblocksize];
                        for(int j=0;j<blocks[i].Count();j++)
                        {
                            t[j] = blocks[i][j];
                        }
                        blocks[i] = t;
                    }
                }
            }
        } // востановление размера блоков сообщения
    }
}
