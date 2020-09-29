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
                    addonDisplay.downloadCount = Duck.FormatNum(addon.downloadCount);
                    addonDisplay.thumbnailUrl = addon.attachments.Count > 0 ?
                        addon.attachments.Find(x => x.isDefault) != null ? addon.attachments.Find(x => x.isDefault).thumbnailUrl : "" : "";
                    addonDisplay.thumbnailFile = Duck.GetThumbnailUri(addonDisplay.thumbnailUrl, "addon", addonDisplay.id);


                    list.Add(addonDisplay);
                }
            }
            AddonsDisplay = new ObservableCollection<AddonDisplay>(list);

        }
    }
}
