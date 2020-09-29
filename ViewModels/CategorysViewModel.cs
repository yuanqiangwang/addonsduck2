using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using AddonsDuck2.Models;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using System.Windows.Controls;
using Prism.Commands;

namespace AddonsDuck2.ViewModels
{
    class CategorysViewModel : BindableBase
    {
        private ObservableCollection<CategoryModel> _categories;
        public ObservableCollection<CategoryModel> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }

        }


        public DelegateCommand<object> SelectedItemChangedCommand { get; private set; }
        string baseUrl = "https://addons-ecs.forgesvc.net";

        public CategorysViewModel()
        {
            SelectedItemChangedCommand = new DelegateCommand<object>(SelectedItemChanged);

            LoadCategoryAsync();
        }
        void SelectedItemChanged(object obj)
        {
            CategoryModel model = obj as CategoryModel;
        }
        void LoadCategoryAsync()
        {
            var client = new RestClient(baseUrl)
            {
                Timeout = -1
            };
            var request = new RestRequest("/api/v2/category", Method.GET);
            IRestResponse response = client.Execute(request);
            string categoryJson = response.Content;
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson).FindAll(x => x.gameId == 1); ;
            Categories = new ObservableCollection<CategoryModel>();
            Categories = PrepareData(categories, null);
        }
        private ObservableCollection<CategoryModel> PrepareData(List<Category> categories, int? id)
        {

            var _categories = categories.FindAll(t => t.parentGameCategoryId == id).OrderBy(t => t.id);

            List<CategoryModel> categoryModels = new List<CategoryModel>();


            foreach (Category item in _categories)
            {
                CategoryModel categoryModel = new CategoryModel();
                categoryModel.id = item.id;
                categoryModel.name = item.name;
                categoryModel.avatarUrl = item.avatarUrl;
                categoryModel.ChildList = PrepareData(categories, item.id);

                categoryModels.Add(categoryModel);
            }


            return new ObservableCollection<CategoryModel>(categoryModels);


        }
    }
}
