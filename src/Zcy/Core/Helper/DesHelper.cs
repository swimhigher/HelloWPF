using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helper
{
    public class DesHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(string Text, string sKey = "zcyzdygj")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            ASCIIEncoding encoding = new ASCIIEncoding();
            MD5 md5 = MD5.Create();
            des.Key = encoding.GetBytes(BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(sKey))).Replace("-", null).Substring(0, 8));
            des.IV = des.Key;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>

        public static string Decrypt(string Text, string sKey = "zcyzdygj")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            MD5 md5 = MD5.Create();
            des.Key = encoding.GetBytes(BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(sKey))).Replace("-", null).Substring(0, 8));
            des.IV = des.Key;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// 32位MD5加密算法
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的32位MD5值</returns>
        public static string MD5_32(string str)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
    }
}
