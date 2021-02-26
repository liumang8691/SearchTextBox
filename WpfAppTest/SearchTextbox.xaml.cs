using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppTest
{
    /// <summary>
    /// SearchTextbox.xaml 的交互逻辑
    /// </summary>
    public partial class SearchTextbox : UserControl
    {
        public SearchTextbox()
        {
            InitializeComponent();
            //注册事件
            _TextBox.LostFocus += (s, e) => { OpenTip( false); };
            _TextBox.TextChanged += tb_TextChanged;
            _TextBox.GotFocus += (s, e) => {tb_TextChanged(null, null);};
            this.LostFocus += (s, e) => { OpenTip(false); };
            this.Loaded += (s, e) => {
                PopopHelper.SetPopupPlacementTarget(_Popup, this);
            };
        }

        #region 依赖属性

        public double TextBoxWidth
        {
            get { return (double)GetValue(TextBoxWidthProperty); }
            set { SetValue(TextBoxWidthProperty, value); }
        }

        public static readonly DependencyProperty TextBoxWidthProperty =
            DependencyProperty.Register("TextBoxWidth", typeof(double),
                typeof(SearchTextbox), new FrameworkPropertyMetadata(120.0) { BindsTwoWayByDefault = true });

        public double TextBoxHeight
        {
            get { return (double)GetValue(TextBoxHeightProperty); }
            set { SetValue(TextBoxHeightProperty, value); }
        }

        public static readonly DependencyProperty TextBoxHeightProperty =
            DependencyProperty.Register("TextBoxHeight", typeof(double),
                typeof(SearchTextbox), new FrameworkPropertyMetadata(30.0) { BindsTwoWayByDefault = true });



        protected IEnumerable ListItemsSource { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(SearchTextbox), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });


        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string),
                typeof(SearchTextbox), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string),
                typeof(SearchTextbox), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public string SearchMemberPath
        {
            get { return (string)GetValue(SearchMemberPathProperty); }
            set { SetValue(SearchMemberPathProperty, value); }
        }

        public static readonly DependencyProperty SearchMemberPathProperty =
            DependencyProperty.Register("SearchMemberPath", typeof(string),
                typeof(SearchTextbox), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });



        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
                SetText(value);
                if (this.Selected != null)
                {
                    this.Selected(this, value);
                }
            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object),
                typeof(SearchTextbox), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        /// <summary>
        /// 搜索结果数量
        /// </summary>
        public int SearchCount
        {
            get { return (int)GetValue(SearchCountProperty); }
            set { SetValue(SearchCountProperty, value); }
        }

        public static readonly DependencyProperty SearchCountProperty =
            DependencyProperty.Register("SearchCount", typeof(int),
                typeof(SearchTextbox), new FrameworkPropertyMetadata(10) { BindsTwoWayByDefault = true });

        #endregion

        #region 事件

        public delegate bool Search(SearchArgs args);
        public event Search OnSearch;

        public event EventHandler<object> Selected;

        #endregion

        #region 方法


        /// <summary>
        /// 文本更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_TextBox.Text))
            {
                this.SelectedItem = null;
                SetItemSource(this.ItemsSource);
            }
            else
            {
                if (this.ItemsSource != null)
                {
                    ArrayList array = new ArrayList();
                    foreach (object item in this.ItemsSource)
                    {
                        SearchArgs args = new SearchArgs()
                        {
                            Text = this._TextBox.Text,
                            Item = item
                        };
                        bool IsContains = false;

                        if (OnSearch == null)
                        {
                            IsContains = Contain(args);
                        }
                        else
                        {
                            IsContains = OnSearch(args);
                        }

                        if (IsContains)
                        {
                            array.Add(item);
                            if (array.Count >= this.SearchCount)
                            {
                                break;
                            }
                        }
                    }

                    SetItemSource(array);
                }
            }
        }
        /// <summary>
        /// 搜索函数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Contain(SearchArgs args)
        {
            string content = args.Item.GetType().GetProperty(this.SearchMemberPath).GetValue(args.Item).ToString();
            return content.Contains(args.Text);
        }
        /// <summary>
        /// 选择选项事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            this.SelectedItem = _ListBox.SelectedItem;
            OpenTip(false);
        }
        /// <summary>
        /// 设置搜索结果
        /// </summary>
        /// <param name="enumerable"></param>
        void SetItemSource(IEnumerable enumerable)
        {
            _ListBox.ItemsSource = enumerable;
            if (_ListBox.Items.Count <= 0)
            {
                OpenTip(false);
            }
            else
            {
                OpenTip(true);
            }
        }
        /// <summary>
        /// 设置显示文本
        /// </summary>
        /// <param name="item"></param>
        void SetText(object item)
        {
            lock (this)
            {
                _TextBox.TextChanged -= tb_TextChanged;
                if (item == null)
                {
                    this._TextBox.Text = null;
                }
                else
                {
                    this._TextBox.Text = item.GetType()
                        .GetProperty(this.DisplayMemberPath)
                        .GetValue(item)
                        .ToString();
                }
                _TextBox.TextChanged += tb_TextChanged;
            }
        }

        /// <summary>
        /// 打开搜索选项
        /// </summary>
        /// <param name="isOpen"></param>
        void OpenTip(bool isOpen)
        {
            this._Popup.IsOpen = isOpen;
        }

        #endregion
    }

    public class SearchArgs : EventArgs
    {
        public string Text { get; set; }
        public Object Item { get; set; }
    }
}
