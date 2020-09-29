using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AddonsDuck2.Models
{
    public class Category
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string slug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dateModified { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? parentGameCategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? rootGameCategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int gameId { get; set; }
    }

    public class CategoryModel : Category
    {
        public Uri avatarFile { get; set; }
        public ObservableCollection<CategoryModel> ChildList { get; set; }
        = new ObservableCollection<CategoryModel>();
    }
}
