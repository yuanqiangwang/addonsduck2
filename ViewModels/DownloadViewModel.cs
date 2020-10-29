using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using AddonsDuck2.Models;
using AddonsDuck2.Duck;
using System.Net;
using System.IO;

namespace AddonsDuck2.ViewModels
{
    class DownloadViewModel : BindableBase
    {
        IEventAggregator _ea;
        private ObservableQueue<AddonDisplay> _addonQueue;
        public ObservableQueue<AddonDisplay> AddonQueue
        {
            get { return _addonQueue; }
            set { SetProperty(ref _addonQueue, value); }
        }
        public DownloadViewModel(IEventAggregator ea)//依赖注入 全局command
        {
            _ea = ea;
            _ea.GetEvent<DownloadAddedEvent>().Subscribe(Add);
            AddonQueue = new ObservableQueue<AddonDisplay>();
        }

        private void Add(AddonDisplay obj)
        {
            if (AddonQueue.Count==0)
            {
                Start();
            }


            if (AddonQueue.Contains(obj))
            {
                return;
            }
            AddonQueue.Enqueue(obj);
        }

        private void Start()
        {
            while (AddonQueue.Count!=0)
            {
                //doanload
            }
        }

        //private async Task<bool> DownLoad(AddonDisplay addon)
        //{
            
        //}

    }
}
