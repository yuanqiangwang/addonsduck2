using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddonsDuck2.Models;

namespace AddonsDuck2.Models
{
    public class AddonDisplay
    {
        public int id { get; set; }

        public string name { get; set; }
        public string websiteUrl { get; set; }
        public string summary { get; set; }
        public string downloadCount { get; set; }
        public List<Addon.Module> modules { get; set; }
        public List<Addon.LatestFile> latestFiles { get; set; }
        public string thumbnailUrl { get; set; }
        public Uri thumbnailFile { get; set; }


        public bool isSelected { get; set; }
        /// <summary>
        /// 是否本地插件
        /// </summary>
        public bool isLocal { get; set; } = false;
        /// <summary>
        /// 本地插件版本
        /// </summary>
        public string localVersion { get; set; }
    }
}
