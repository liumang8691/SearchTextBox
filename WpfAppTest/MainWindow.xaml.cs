using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitData();
            InitializeComponent();
            stb.ItemsSource = listStr;
        }

        public List<User> listStr { get; set; }

        public void InitData()
        {

            listStr = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                listStr.Add(new User() { 
                    id = i,
                    name = "User" + i
                });
            }
        }

        private bool SearchTextbox_OnSearch(SearchArgs args)
        {
            User u = args.Item as User;
            if (u != null)
            {
                return u.name.Contains(args.Text);
            }
            return false;
        }
    }



}
