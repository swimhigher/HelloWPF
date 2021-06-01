using System;
using System.IO;
using System.Net;
using System.Text;

namespace Core.Helper
{
    public class SendPostHelper
    {
        /// <summary>
        /// Post表单提交
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public static string PostWebRequest<T>(string postUrl, T paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                var proInfo = paramData.GetType().GetProperties();
                var PostString = "";
                foreach (var item in proInfo)
                {
                    PostString += $"{item.Name}={Convert.ToString(item.GetValue(paramData))}&";
                }
                PostString = PostString.Substring(0, PostString.Length - 1);
                byte[] byteArray = dataEncode.GetBytes(PostString); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

    }
}
