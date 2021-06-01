using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Core.Helper
{
    public class BaiDuFanyi
    {
        private const string appid = "20200529000475784";
        private const string password = "sPeuEMIX_VzJ926Fct6o";
        private const string url = "https://fanyi-api.baidu.com/api/trans/vip/translate";//http://api.fanyi.baidu.com/api/trans/vip/translate

        public static TranOutput Run(string q, string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                from = "auto";
            }
            Random r = new Random();
            var salt = r.Next(99999).ToString();
            //appid+q+salt+密钥 的MD5值
            var sign = DesHelper.MD5_32(appid + q + salt + password);

            var q_coding = HttpUtility.UrlEncode(q, Encoding.UTF8);
            TranPostInput input = new TranPostInput()
            {
                q = q_coding,
                from = from,
                to = to,
                appid = appid,
                sign = sign,
                salt = salt

            };

            var reslutStr = SendPostHelper.PostWebRequest(url, input, Encoding.UTF8);
            var result = JsonSerializer.Deserialize<TranOutput>(reslutStr);
            if (!string.IsNullOrWhiteSpace(result.error_code) && result.error_code != "52000")
            {
                result.ErrorMessage = GetErrorMessage(result.error_code);

            }
            return result;
        }

        private static string GetErrorMessage(string code)
        {
            switch (code)
            {
                case "52000": return "成功";
                case "52001": return "请求超时 重试";
                case "52002": return "系统错误 重试";
                case "52003": return "未授权用户 检查您的 appid 是否正确，或者服务是否开通";
                case "54000": return "必填参数为空 检查是否少传参数";
                case "54001": return "签名错误 请检查您的签名生成方法";
                case "54003": return "访问频率受限 请降低您的调用频率";
                case "54004": return "账户余额不足 请前往管理控制台为账户充值";
                case "54005": return "长query请求频繁 请降低长query的发送频率，3s后再试";
                case "58000": return "客户端IP非法 检查个人资料里填写的 IP地址 是否正确可前往管理控制平台修改IP限制，IP可留空";
                case "58001": return "译文语言方向不支持 检查译文语言是否在语言列表里";
                case "58002": return "服务当前已关闭 请前往管理控制台开启服务";
                case "90107": return "认证未通过或未生效 请前往我的认证查看认证进度";
                default:
                    return "";

            }


        }
    }

    public class TranPostInput
    {
        public string q { get; set; } //TEXT    Y 请求翻译query   UTF-8编码
        public string from { get; set; }  //TEXT Y   翻译源语言 语言列表(可设置为auto)
        public string to { get; set; } //TEXT    Y 译文语言    语言列表(不可设置为auto)
        public string appid { get; set; } //TEXT    Y APP ID 可在管理控制台查看
        public string salt { get; set; } //TEXT    Y 随机数
        public string sign { get; set; } //TEXT    Y 签名  appid+q+salt+密钥 的MD5值
        //以下字段仅开通了词典、TTS者需填写
        public string tts { get; set; } //STRING N   是否显示语音合成资源 tts = 0显示，tts=1不显示

        public string dict { get; set; }   // STRING N   是否显示词典资源 dict = 0显示，dict=1不显示
                                           //以下字段仅开通“我的术语库”用户需填写
        public string action { get; set; }
    }

    public class TranOutput
    {
        public string from { get; set; } //TEXT    翻译源语言 返回用户指定的语言，或自动检测的语言（源语言设为auto时）
        public string to { get; set; } //TEXT    译文语言 返回用户指定的目标语言
        public List<TransResutl> trans_result { get; set; }//MIXED LIST 翻译结果    返回翻译结果，包含src 和 dst 字段。
        public string src { get; set; } //TEXT    原文
        public string dst { get; set; }//TEXT 译文
        public string error_code { get; set; } //Int32   错误码 仅当出现错误时显示
                                               //以下字段仅开通词典、TTS资源者可见
        public string src_tts { get; set; }    //原文tts mp3格式，暂时无法指定发音

        public string dst_tts { get; set; }    //译文tts mp3格式，暂时无法指定发音
        public string dict { get; set; }       //中英词典资源 返回中文或英文词典资源，包含音标、简明释义等内容
        public string ErrorMessage { get; set; }
    }
    public class TransResutl
    {
        public string src { get; set; }
        public string dst { get; set; }
    }

}