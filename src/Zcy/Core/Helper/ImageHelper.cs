using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Core.Helper
{
    public class ImageHelper
    {
        /// <summary>
        /// Base64转图片
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static Image Base64StringToImage(string base64)
        {
            byte[] buffer = Convert.FromBase64String(base64);
            return BytesToImage(buffer);
        }
        /// <summary>
        /// 二进制转图片
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = Image.FromStream(ms);
            return image;
        }
        /// Image转byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        /// <summary>
        /// Image转Base64字符串
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ImageToBase64String(Image image)
        {
            return Convert.ToBase64String(ImageToBytes(image));
        }
        /// <summary>
        /// 根据文件路径获取图片Base64
        /// </summary>
        /// <param name="imagefile"></param>
        /// <returns></returns>
        public static string GetBase64FromImage(string imagefile)
        {
            string strbaser64 = "";
            try
            {
                Bitmap bmp = new Bitmap(imagefile);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                strbaser64 = Convert.ToBase64String(arr);
            }
            catch (Exception)
            {
                return "";
                // throw new Exception("Something wrong during convert!");
            }
            return strbaser64;
        }

    }
}
