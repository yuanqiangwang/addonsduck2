using AddonsDuck2.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AddonsDuck2.Duck
{
    public static class Tools
    {

        public static Uri GetThumbnailUri(string url, string type, int uid)
        {
            string path = @"Thumbnail";

            switch (type)
            {
                case "addon":
                    path += @"\addon\";

                    break;
                case "category":
                    path += @"\category\";

                    break;
                default:
                    break;
            }



            if (string.IsNullOrEmpty(url))
            {

                return new Uri("pack://SiteOfOrigin:,,,/" + path + "1");
            }
            if (!File.Exists(path + uid.ToString()))
            {
                SaveThumbnail(url, path, uid);
            }


            return new Uri("pack://SiteOfOrigin:,,,/" + path + uid.ToString());
        }

        public static async void SaveThumbnail(string url, string path, int uid)
        {

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (System.IO.File.Exists(path + uid))
            {
                return;
            }
            using (WebClient wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(new Uri(url), path + uid.ToString());
            }
        }


        public static string GetCatcheData(string type ,string key)
        {
            if (!System.IO.File.Exists(@"Catche\" + key))
                return string.Empty;

            string jsondata = File.ReadAllText(@"Catche\" + key);
            if (type == "Categorys")
            {
                JSchema schema = new JSchemaGenerator().Generate(typeof(CategoryModel));
                JToken token = JToken.Parse(jsondata);
                if (token.IsValid(schema))
                    jsondata = string.Empty;
            }

            if (type == "Addons")
            {
                JSchema schema = new JSchemaGenerator().Generate(typeof(Addon));
                JToken token = JToken.Parse(jsondata);
                if (token.IsValid(schema))
                    jsondata = string.Empty;
            }


            return jsondata;

        }
        public static void SaveData(string str, string path, string name)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            using (StreamWriter sw = new StreamWriter(path+@"\"+name, false))
            {
                sw.Write(str);
            }
        }

        public static string FormatNum(decimal longnum)
        {
            string formatednum;
            if (longnum > 100000000)
                formatednum = Math.Round(longnum / 100000000, 1).ToString() + "亿";
            else if (longnum > 10000)
                formatednum = Math.Round(longnum / 10000, 0).ToString() + "万";
            else
                formatednum = Math.Round(longnum, 0).ToString();


            return formatednum;
        }

        public static string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                return "很久以前";
            }
            else if (span.TotalDays > 30)
            {
                return "1个月前";
            }
            else if (span.TotalDays > 14)
            {
                return "2周前";
            }
            else if (span.TotalDays > 7)
            {
                return "1周前";
            }
            else if (span.TotalDays > 1)
            {
                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
            }
            else if (span.TotalHours > 1)
            {
                return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                return "1秒前";
            }
        }

    }


    public class MessageSentEvent : PubSubEvent<object>
    {
    }

}
