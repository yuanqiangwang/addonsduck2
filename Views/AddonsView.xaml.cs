using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AddonsDuck2.Views
{
    /// <summary>
    /// AddonsView.xaml 的交互逻辑
    /// </summary>
    public partial class AddonsView : UserControl
    {
        public AddonsView()
        {
            InitializeComponent();
           
        }

        private void progressbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (e.NewValue==0)
            {
                progressbar.Visibility = Visibility.Collapsed;
            }
            else
            {
                progressbar.Visibility = Visibility.Visible;
            }
        }

    }
}
