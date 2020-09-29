using Prism.Mvvm;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddonsDuck2.Models;
using Newtonsoft.Json;
using Prism.Common;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;


namespace AddonsDuck2.ViewModels
{
    class AddonsViewModel : BindableBase
    {
        string baseUrl = "https://addons-ecs.forgesvc.net";

        private ObservableCollection<AddonDisplay> _addonsDisplay;
        public ObservableCollection<AddonDisplay> AddonsDisplay
        {
            get { return _addonsDisplay; }
            set { SetProperty(ref _addonsDisplay, value); }
        }

        private string _gameVersionFlavor = "wow_classic";

        public AddonsViewModel()
        {
            GetData();
        }

        public async void GetData()
        {
            string json = await AddonSearchAsync(0, 0, _gameVersionFlavor, 20);
            FilterAddons(json);
        }


        private async Task<string> AddonSearchAsync(int categoryId, int _index, string gameVersionFlavor, int pagesize = 0, string searchFilter = "")
        {
            // index 是 索引起始位置
            // pagesize 是长度 
            var client = new RestClient(baseUrl);
            client.Timeout = -1;
            string resource = string.Format("/api/v2/addon/search?categoryId={0}&gameId=1&index={1}", categoryId, _index);
            if (!string.IsNullOrEmpty(searchFilter))
            {
                resource += string.Format("&searchFilter={0}", searchFilter);
            }

            if (pagesize != 0)//检索不分页
            {
                resource += string.Format("&pageSize={0}", pagesize);
            }

            if (gameVersionFlavor != "")
            {
                resource += string.Format("&gameVersionFlavor={0}", gameVersionFlavor);
            }
            var request = new RestRequest(resource, Method.GET);
            var response = await client.ExecuteGetAsync(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                return null;
            }
        }


        private void FilterAddons(string addonsJson)
        {
            List<AddonDisplay> list = new List<AddonDisplay>();
            List<Addon> addons = JsonConvert.DeserializeObject<List<Addon>>(addonsJson);
            foreach (Addon addon in addons)
            {
                Addon.LatestFile latestFile = addon.latestFiles.FindAll(x => x.releaseType == 1)
                      .Find(x => x.gameVersionFlavor == _gameVersionFlavor);
                if (latestFile != null)
                {
                    AddonDisplay addonDisplay = new AddonDisplay();
                    addonDisplay.id = addon.id;
                    addonDisplay.name = addon.name;
                    addonDisplay.websiteUrl = addon.websiteUrl;
                    addonDisplay.summary = addon.summary;
                    addonDisplay.downloadCount = FormatNum(addon.downloadCount);
                    addonDisplay.thumbnailUrl = addon.attachments.Count > 0 ?
                        addon.attachments.Find(x => x.isDefault) != null ? addon.attachments.Find(x => x.isDefault).thumbnailUrl : "" : "";

                    string filepath= GetThumbnailFile(addonDisplay.thumbnailUrl, "addon", addonDisplay.id);
                    addonDisplay.thumbnailFile = new Uri("pack://SiteOfOrigin:,,,/" + filepath);


                    list.Add(addonDisplay);
                }
            }


            AddonsDisplay = new ObservableCollection<AddonDisplay>(list);

        }


        string GetThumbnailFile(string url,string type,int uid)
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


            return path + uid.ToString();
        }

        async void SaveThumbnail(string url, string path, int uid)
        {

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (System.IO.File.Exists(path + uid))
            {
                return;
            }
            using (WebClient wc=new WebClient())
            {
              await  wc.DownloadFileTaskAsync(new Uri( url), path + uid.ToString());
            }


        }




        private string FormatNum(decimal longnum)
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
