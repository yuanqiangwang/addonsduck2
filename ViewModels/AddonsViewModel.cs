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
using Prism.Commands;
using AddonsDuck2.Duck;
using Prism.Events;
using System.Text.RegularExpressions;
using static AddonsDuck2.Models.Addon;
using System.Collections.Concurrent;

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

        private int _progress;

        public int Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }

        }

        private string _tipsString;
        public string TipsString
        {
            get { return _tipsString; }
            set { SetProperty(ref _tipsString, value); }

        }


        /// <summary>
        /// 依赖注入接收
        /// </summary>
        //IApplicationCommands _applicationCommands;

        IEventAggregator _ea;
        public DelegateCommand<object> ReloadAddonsCommand { get; private set; }

        public DelegateCommand<AddonDisplay> DownloadAddonCammond { get; private set; }

        private ObservableQueue<AddonDisplay> _addonQueue;
        public ObservableQueue<AddonDisplay> AddonQueue
        {
            get { return _addonQueue; }
            set { SetProperty(ref _addonQueue, value); }

        }

        private string _gameVersionFlavor = "wow_classic";
        string addonPath = @"D:\Entertainment\Game\World of Warcraft\_classic_\Interface\AddOns";

        public AddonsViewModel(IEventAggregator ea)//依赖注入 全局command
        {
            _ea = ea;
            _ea.GetEvent<MessageSentEvent>().Subscribe(GetData);
            //_ = DisplayLocal();

            DownloadAddonCammond = new DelegateCommand<AddonDisplay>(AddToQueue);
            AddonQueue = new ObservableQueue<AddonDisplay>();
        }

        void AddToQueue(AddonDisplay addon)
        {

            AddonQueue.Enqueue(addon);

            
        }

        public async void GetData(object obj)
        {
            List<AddonDisplay> addonDisplays = new List<AddonDisplay>();
            CategoryModel categoryModel = obj as CategoryModel;
            string json = await AddonSearchAsync(categoryModel.id == 1 ? 0 : categoryModel.id, 0, _gameVersionFlavor, 20);
            FilterAddons(json, addonDisplays);
            AddonsDisplay = new ObservableCollection<AddonDisplay>(addonDisplays);
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

            if (pagesize != 0)//检索不分页，不分页最大返回结果数量是500
            {
                resource += string.Format("&pageSize={0}", pagesize);
            }

            if (gameVersionFlavor != "")
            {
                resource += string.Format("&gameVersionFlavor={0}", gameVersionFlavor);
            }

            //categoryId={categoryID}&gameId={gameId}&gameVersion={gameVersion}&index={index}&pageSize={pageSize}5&searchFilter={searchFilter}
            string key = "0&1&wow_classic&1&20";

            string addonsJson = "";
            addonsJson = Duck.Tools.GetCatcheData("Addons", key);

            if (string.IsNullOrEmpty(addonsJson))
            {
                var request = new RestRequest(resource, Method.GET);
                var response = await client.ExecuteGetAsync(request);
                addonsJson = response.IsSuccessful ? response.Content : string.Empty;
                Duck.Tools.SaveData("Addons", "Catche", key);
            }
            return addonsJson;
        }

        private void FilterAddons(string addonsJson, List<AddonDisplay> addonDisplays, string localVersion = "")
        {
            List<Addon> addons = JsonConvert.DeserializeObject<List<Addon>>(addonsJson);
            foreach (Addon addon in addons)
            {
                Addon.LatestFile latestFile = addon.latestFiles.FindAll(x => x.releaseType == 1)
                      .Find(x => x.gameVersionFlavor == _gameVersionFlavor);
                if (latestFile != null)
                {

                    if (addonDisplays != null && addonDisplays.Find(x => x.id == addon.id) != null)
                    {
                        continue;
                    }
                    AddonDisplay display = new AddonDisplay
                    {
                        id = addon.id,
                        name = addon.name,
                        websiteUrl = addon.websiteUrl,
                        summary = addon.summary,
                        downloadCount = Tools.FormatNum(addon.downloadCount),
                        dateCreated = addon.dateCreated,
                        dateModified = addon.dateModified,
                        dateReleased = addon.dateReleased,
                        thumbnailUrl = addon.attachments.Count > 0 ?
                        addon.attachments.Find(x => x.isDefault) != null ? addon.attachments.Find(x => x.isDefault).thumbnailUrl : "" : ""
                    };
                    display.thumbnailFile = Tools.GetThumbnailUri(display.thumbnailUrl, "addon", display.id);
                    display.isLocal = !string.IsNullOrEmpty(localVersion);
                    display.localVersion = localVersion;

                    addonDisplays.Add(display);
                }
            }
        }

        private async Task<bool> DisplayLocal()
        {
            List<AddonDisplay> addonDisplays = new List<AddonDisplay>();
            List<string> localAddons = ScanLocalAddonsModule(addonPath);

            foreach (string item in localAddons)
            {
                string addonmodule = item.Split('|')[0];
                string version = item.Split('|')[1];
                Progress = localAddons.IndexOf(item) * 100 / localAddons.Count;
                TipsString = addonmodule;
                string json = await AddonSearchAsync(0, 0, _gameVersionFlavor, 1, addonmodule);
                FilterAddons(json, addonDisplays, version);
            }
            Progress = 0;
            TipsString = "";


            AddonsDisplay = new ObservableCollection<AddonDisplay>(addonDisplays);
            return true;
        }

        private List<string> ScanLocalAddonsModule(string addonPath)
        {
            List<string> localAddons = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(addonPath);
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                foreach (FileInfo file in directory.GetFiles())
                {
                    if (file.Extension == ".toc")
                    {
                        if (file.Name == "DBM-StatusBarTimers")
                        {
                            continue;
                        }
                        string filepath = file.FullName;
                        string strContent = File.ReadAllText(filepath);
                        string patternDependencies = "## Dependencies:(?'key'.*)\\r?\\n?";
                        string patternRequiredDeps = "## RequiredDeps:(?'key'.*)\\r?\\n?";

                        string patternVersion = "## Version:(?'key'.*)\\r?\\n?";

                        string Version = Regex.Match(strContent, patternVersion, RegexOptions.IgnoreCase).Success ?
                                Regex.Match(strContent, patternVersion, RegexOptions.IgnoreCase).Groups["key"].Value : "";
                        //## RequiredDeps: DBM-Core
                        //## Dependencies: Details
                        string requiredDepsOrDependencies =
    Regex.Match(strContent, patternDependencies, RegexOptions.IgnoreCase).Success ?
    Regex.Match(strContent, patternDependencies, RegexOptions.IgnoreCase).Groups["key"].Value :
    Regex.Match(strContent, patternRequiredDeps, RegexOptions.IgnoreCase).Success ?
    Regex.Match(strContent, patternRequiredDeps, RegexOptions.IgnoreCase).Groups["key"].Value : "";
                        requiredDepsOrDependencies = requiredDepsOrDependencies.Trim();


                        string localaddoninfo = "";

                        if (requiredDepsOrDependencies == "" || file.Directory.Name == "DBM-Core")
                        {
                            localaddoninfo = file.Directory.Name + "|" + Version;
                        }

                        if (!localAddons.Contains(localaddoninfo)
                            && !string.IsNullOrEmpty(localaddoninfo))
                        {
                            localAddons.Add(localaddoninfo);
                        }

                    }
                }
            }
            return localAddons;
        }
    }
}
