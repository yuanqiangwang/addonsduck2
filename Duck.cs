using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AddonsDuck2
{
    public static class Duck
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

            if (!File.Exists(path + uid.ToString()))
            {
                SaveThumbnail(url, path, uid);
            }


            return new Uri("pack://SiteOfOrigin:,,,/"+ path + uid.ToString());
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




        public static string FormatNum(decimal longnum)
        {
            string formatednum;
            if (longnum > 1000000)
                formatednum = Math.Round(longnum / 1000000, 1).ToString() + "百万";
            else if (longnum > 10000)
                formatednum = Math.Round(longnum / 1000, 1).ToString() + "万";
            else
                formatednum = longnum.ToString();


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
}
