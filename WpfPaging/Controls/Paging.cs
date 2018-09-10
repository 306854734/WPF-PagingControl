﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfPaging.Controls
{
    /// <summary>
    /// 分页控件
    /// </summary>
    [TemplatePart(Name = PART_PREVIOUSPAGE, Type = typeof(Button))]
    [TemplatePart(Name = PART_NEXTPAGE, Type = typeof(Button))]
    [TemplatePart(Name = PART_FIRSTPAGE, Type = typeof(Button))]
    [TemplatePart(Name = PART_LASTPAGE, Type = typeof(Button))]
    [TemplatePart(Name = PART_PAGESIZE, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_CONTENT, Type = typeof(StackPanel))]
    //[TemplatePart(Name = PART_PAGECOUNT, Type = typeof(Run))]
    //[TemplatePart(Name = PART_CURREENTPAGE, Type = typeof(Run))]
    [TemplatePart(Name = PART_TOTALNUM, Type = typeof(Run))]
    [TemplatePart(Name = PART_GOTOPAGE, Type = typeof(Button))]


    public class Paging : Control
    {
        #region 字段 
        private const String PART_PREVIOUSPAGE = "PART_PreviousPage";
        private const String PART_NEXTPAGE = "PART_NextPage";
        private const String PART_FIRSTPAGE = "PART_FirstPage";
        private const String PART_LASTPAGE = "PART_LastPage";
        private const String PART_PAGESIZE = "PART_PageSize";
        private const String PART_CONTENT = "PART_Content";
        private const String PART_PAGECOUNT = "PART_PageCount";
        private const String PART_CURREENTPAGE = "PART_CurrentPage";
        private const String PART_TOTALNUM = "PART_TotalNum";
        private const String PART_GOTOPAGE = "PART_GotoPage";
        private const String PART_GOTOPAGENUM = "PART_GotoPageNum";

        private Button PART_Previouspage;
        private Button PART_Nextpage;
        private Button PART_FirstPage;
        private Button PART_LastPage;
        private ComboBox PART_PageSize;
        private StackPanel PART_Content;
        private Button PART_GotoPage;
        private TextBox PART_GotoPageNum;

        private PagerType mPagerType = PagerType.Default;  //当前分页控件类型，复杂、默认
        private List<int> mCurrentPagers = new List<int>(); //当前分页控件显示的页码索引

        #endregion

        #region DependecyProperty
        public static readonly DependencyProperty CurrentPageProperty;
        public static readonly DependencyProperty PageCountProperty;
        public static readonly DependencyProperty TotalNumProperty;
        public static readonly DependencyProperty PageSizeProperty;
        public static readonly DependencyProperty GotoPageNumProperty;


        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }
        public int TotalNum
        {
            get { return (int)GetValue(TotalNumProperty); }
            set { SetValue(TotalNumProperty, value); }
        }
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public int GotoPageNum
        {
            get { return (int)GetValue(GotoPageNumProperty); }
            set { SetValue(GotoPageNumProperty, value); }
        }

        #endregion

        #region RoutedEvent

        public static readonly RoutedEvent FirstPageEvent;
        public static readonly RoutedEvent PreviousPageEvent;
        public static readonly RoutedEvent NextPageEvent;
        public static readonly RoutedEvent LastPageEvent;
        public static readonly RoutedEvent GotoPageEvent;
        public static readonly RoutedEvent PageChangedEvent;
        public static readonly RoutedEvent PageSizeChangedEvent;

        public event RoutedEventHandler FirstPage
        {
            add { AddHandler(FirstPageEvent, value); }
            remove { RemoveHandler(FirstPageEvent, value); }
        }

        public event RoutedEventHandler PreviousPage
        {
            add { AddHandler(PreviousPageEvent, value); }
            remove { RemoveHandler(PreviousPageEvent, value); }
        }

        public event RoutedEventHandler NextPage
        {
            add { AddHandler(NextPageEvent, value); }
            remove { RemoveHandler(NextPageEvent, value); }
        }

        public event RoutedEventHandler LastPage
        {
            add { AddHandler(LastPageEvent, value); }
            remove { RemoveHandler(LastPageEvent, value); }
        }
        public event RoutedEventHandler GotoPage
        {
            add { AddHandler(GotoPageEvent, value); }
            remove { RemoveHandler(GotoPageEvent, value); }
        }
        public event RoutedEventHandler PageChanged
        {
            add { AddHandler(PageChangedEvent, value); }
            remove { RemoveHandler(PageChangedEvent, value); }
        }
        public event RoutedEventHandler PageSizeChanged
        {
            add { AddHandler(PageSizeChangedEvent, value); }
            remove { RemoveHandler(PageSizeChangedEvent, value); }
        }
        #endregion

        static Paging()
        {
            FirstPageEvent = EventManager.RegisterRoutedEvent("FirstPage", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            PreviousPageEvent = EventManager.RegisterRoutedEvent("PreviousPage", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            NextPageEvent = EventManager.RegisterRoutedEvent("NextPage", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            LastPageEvent = EventManager.RegisterRoutedEvent("LastPage", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            GotoPageEvent = EventManager.RegisterRoutedEvent("GotoPage", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            PageChangedEvent = EventManager.RegisterRoutedEvent("PageChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            PageSizeChangedEvent = EventManager.RegisterRoutedEvent("PageSizeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Paging));
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), typeof(Paging), new PropertyMetadata(1, new PropertyChangedCallback(OnCurrentPageChanged)));
            PageCountProperty = DependencyProperty.Register("PageCount", typeof(int), typeof(Paging), new PropertyMetadata(1, new PropertyChangedCallback(OnPageCountChanged)));
            TotalNumProperty = DependencyProperty.Register("TotalNum", typeof(int), typeof(Paging), new PropertyMetadata(0));
            PageSizeProperty = DependencyProperty.Register("PageSize", typeof(int), typeof(Paging), new PropertyMetadata(0));
            GotoPageNumProperty = DependencyProperty.Register("GotoPageNum", typeof(int), typeof(Paging), new PropertyMetadata(1));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Previouspage = this.GetTemplateChild(PART_PREVIOUSPAGE) as Button;
            this.PART_Nextpage = this.GetTemplateChild(PART_NEXTPAGE) as Button;
            this.PART_FirstPage = this.GetTemplateChild(PART_FIRSTPAGE) as Button;
            this.PART_LastPage = this.GetTemplateChild(PART_LASTPAGE) as Button;
            this.PART_PageSize = this.GetTemplateChild(PART_PAGESIZE) as ComboBox;
            this.PART_Content = this.GetTemplateChild(PART_CONTENT) as StackPanel;
            this.PART_GotoPage = this.GetTemplateChild(PART_GOTOPAGE) as Button;
            this.PART_GotoPageNum = this.GetTemplateChild(PART_GOTOPAGENUM) as TextBox;

            this.PART_Previouspage.Click += PART_Previouspage_Click;
            this.PART_Nextpage.Click += PART_Nextpage_Click;
            this.PART_FirstPage.Click += PART_FirstPage_Click;
            this.PART_LastPage.Click += PART_LastPage_Click;
            this.PART_PageSize.Loaded += PART_PageSize_Loaded;
            this.PART_PageSize.SelectionChanged += PART_PageSize_SelectionChanged;
            this.PART_GotoPage.Click += PART_GotoPage_Click;
            this.PART_GotoPageNum.TextChanged += PART_GotoPageNum_TextChanged;
            this.PART_GotoPageNum.PreviewTextInput += PART_GotoPageNum_PreviewTextInput;
            InitPART_Content();
        }

        #region DependecyPropertyMethod

        private static void OnPageCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Paging)d).IsLoaded)
                ((Paging)d).InitPART_Content();
        }

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Paging)d).IsLoaded)
                ((Paging)d).UpdateGotoPageNum();
        }
        private void UpdateGotoPageNum()
        {
            GotoPageNum = CurrentPage;
        }

        #endregion

        #region EventMothod

        /// <summary>
        /// Goto LastPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_LastPage_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage = this.PageCount;
            SetNextpageAndPreviouspageState();
            int _index = this.CalculationCurrentSelectPagerButtonWithIndex();
            if (this.mPagerType == PagerType.Complex)
            {
                this._RefreshFirstOrLastPager(AddSubtract.Add);
                this._MaintainCurrentPagersFirstOrLast(AddSubtract.Add);
                this.SetLinkButtonFocus(6);
            }
            else
            {
                this.SetLinkButtonFocus(_index);
            }
            OnPageChanged(LastPageEvent);
        }

        /// <summary>
        /// Goto FirstPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_FirstPage_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage = 1;
            SetNextpageAndPreviouspageState();
            int _index = this.CalculationCurrentSelectPagerButtonWithIndex();
            if (this.mPagerType == PagerType.Complex)
            {
                this._RefreshFirstOrLastPager(AddSubtract.subtract);
                this._MaintainCurrentPagersFirstOrLast(AddSubtract.subtract);
                this.SetLinkButtonFocus(0);
            }
            else
            {
                this.SetLinkButtonFocus(_index);
            }
            OnPageChanged(FirstPageEvent);
        }

        /// <summary>
        /// NextPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Nextpage_Click(object sender, RoutedEventArgs e)
        {
            if (PageCount > CurrentPage)
                this.CurrentPage++;
            SetNextpageAndPreviouspageState();
            int _index = this.CalculationCurrentSelectPagerButtonWithIndex();
            if (this.mPagerType == PagerType.Complex)
            {
                if (this.CurrentPage == this.PageCount)
                {
                    this.SetLinkButtonFocus(_index);
                    OnPageChanged(NextPageEvent);
                    return;
                }
                if (_index == 6)
                {
                    if (this.CurrentPage == this.PageCount - 1)
                    {
                        SetNextpageAndPreviouspageState();
                    }
                    //Refresh UI
                    this._RefreshSinglePager(AddSubtract.Add);
                    this._MaintainCurrentPagersSingle(AddSubtract.Add);
                }
                else
                {
                    this.SetLinkButtonFocus(_index);
                }
            }
            else
            {
                this.SetLinkButtonFocus(_index);
            }
            OnPageChanged(NextPageEvent);
        }

        /// <summary>
        /// PreviousPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Previouspage_Click(object sender, RoutedEventArgs e)
        {
            if (1 < CurrentPage)
                this.CurrentPage--;
            SetNextpageAndPreviouspageState();
            //The index displayed by the current PageIndex on the interface, used to determine the control point 
            int _index = this.CalculationCurrentSelectPagerButtonWithIndex();
            if (this.mPagerType == PagerType.Complex)
            {
                if (this.CurrentPage == 1)
                {
                    this.SetLinkButtonFocus(_index);
                    OnPageChanged(PreviousPageEvent);
                    return;
                }
                if (_index == 0)
                {
                    //Refresh UI
                    this._RefreshSinglePager(AddSubtract.subtract);
                    this._MaintainCurrentPagersSingle(AddSubtract.subtract);
                }
                else
                {
                    this.SetLinkButtonFocus(_index);
                }
            }
            else
            {
                this.SetLinkButtonFocus(_index);
            }
            OnPageChanged(PreviousPageEvent);
        }

        /// <summary>
        /// Page jump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_GotoPage_Click(object sender, RoutedEventArgs e)
        {
            if (1 <= GotoPageNum && GotoPageNum <= PageCount)
            {
                CurrentPage = GotoPageNum;
            }
            else if (1 > GotoPageNum)
            {
                CurrentPage = 1;
            }
            else if (GotoPageNum > PageCount)
            {
                CurrentPage = PageCount;
            }
            SetNextpageAndPreviouspageState();
            int _index = this.CalculationCurrentSelectPagerButtonWithIndex();
            int _firstIndex = this.mCurrentPagers.First();
            int _lastIndex = this.mCurrentPagers.Last();
            if (this.mPagerType == PagerType.Complex)
            {
                if (_firstIndex < GotoPageNum && GotoPageNum < _lastIndex)
                {
                    //Do not refresh page number
                    this.SetLinkButtonFocus(_index);
                }
                else if (_firstIndex >= GotoPageNum)
                {
                    //Refresh page number up
                    if (GotoPageNum > 1)
                    {
                        this._RefreshGotoPager(AddSubtract.subtract);
                        this._MaintainCurrentPagersGoto(AddSubtract.subtract);
                        int _afterIndex = this.CalculationCurrentSelectPagerButtonWithIndex();
                        this.SetLinkButtonFocus(_afterIndex);
                    }
                    else if (GotoPageNum == 1)
                    {
                        this._RefreshFirstOrLastPager(AddSubtract.subtract);
                        this._MaintainCurrentPagersFirstOrLast(AddSubtract.subtract);
                        this.SetLinkButtonFocus(0);
                    }
                }
                else if (_lastIndex <= GotoPageNum)
                {
                    //Refresh page number down
                    if (GotoPageNum < PageCount)
                    {
                        this._RefreshGotoPager(AddSubtract.Add);
                        this._MaintainCurrentPagersGoto(AddSubtract.Add);
                        int _afterIndex = this.CalculationCurrentSelectPagerButtonWithIndex();
                        this.SetLinkButtonFocus(_afterIndex);
                    }
                    else if (GotoPageNum == PageCount)
                    {
                        this._RefreshFirstOrLastPager(AddSubtract.Add);
                        this._MaintainCurrentPagersFirstOrLast(AddSubtract.Add);
                        this.SetLinkButtonFocus(6);
                    }
                }
            }
            else
            {
                this.SetLinkButtonFocus(_index);
            }
            OnPageChanged(GotoPageEvent);
        }

        private void PagenumBtn_Click(object sender, RoutedEventArgs e)
        {
            //Get the page number of the current click
            var pagenumBtn = sender as Button;
            this.CurrentPage = Convert.ToInt32(pagenumBtn.Content);
            this.SetNextpageAndPreviouspageState();
            int _index = CalculationCurrentSelectPagerButtonWithIndex();
            //Handling complex controls
            if (this.mPagerType == PagerType.Complex)
            {
                this._RefreshPagerBtn(pagenumBtn);
                if (_index == 0 && this.CurrentPage > 1)
                    SetLinkButtonFocus(_index + 1);
                else if (_index == 6 && this.CurrentPage < PageCount)
                    SetLinkButtonFocus(_index - 1);
                else
                    SetLinkButtonFocus(_index);
            }
            else
            {
                SetLinkButtonFocus(_index);
            }

            OnPageChanged(PageChangedEvent);
        }

        private void PART_PageSize_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox combobox)
            {
                var item = combobox.SelectedItem as ComboBoxItem;
                this.PageSize = Convert.ToInt32(item.Content.ToString());
                this.GotoPageNum = CurrentPage;
                OnPageSizeChanged();
            }
        }

        /// <summary>
        /// Select the number of displays per page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_PageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combobox)
            {
                var item = combobox.SelectedItem as ComboBoxItem;
                this.PageSize = Convert.ToInt32(item.Content.ToString());    
                InitPART_Content();
                OnPageSizeChanged();
            }
        }

        private void PART_GotoPageNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                Regex re = new Regex(@"[^0-9]+");
                if (re.IsMatch(textBox.Text) || textBox.Text == "0")
                {
                    this.GotoPageNum = 0;
                    this.GotoPageNum = CurrentPage;
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        if (int.Parse(textBox.Text) > PageCount)
                        {
                            this.GotoPageNum = 0;
                            this.GotoPageNum = PageCount;
                        }
                        else
                        {
                            this.GotoPageNum = int.Parse(textBox.Text);
                        }
                    }
                }
            }   
        }
        private void PART_GotoPageNum_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.SelectionStart == 0 && e.Text == "0")
                {
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region OtherMethod
        /// <summary>
        /// Add Page
        /// </summary>
        private void InitPART_Content()
        {
            this.CurrentPage = 1;
            SetNextpageAndPreviouspageState();
            this.PART_Content.Children.RemoveRange(0, PART_Content.Children.Count);
            this.mCurrentPagers.RemoveRange(0, mCurrentPagers.Count);
            if ((this.PageCount <= 7))
            {
                this.mPagerType = PagerType.Default;
                for (int i = 0; i < this.PageCount; i++)
                {
                    var pagenumBtn = new Button()
                    {
                        Content = (i + 1).ToString(),
                        Width = double.NaN,
                        Margin = new Thickness(3, 0, 3, 0),
                        BorderThickness = new Thickness(1, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        Style = Application.Current.FindResource("PageNumberButton") as Style
                    };
                    this.mCurrentPagers.Add((i + 1));
                    pagenumBtn.Click += PagenumBtn_Click;
                    if (this.PART_Content != null)
                    {
                        this.PART_Content.Children.Add(pagenumBtn);
                    }
                }
            }
            else
            {
                this.mPagerType = PagerType.Complex;
                for (int i = 0; i < 7; i++)
                {
                    var pagenumBtn = new Button()
                    {
                        Content = (i + 1).ToString(),
                        Width = double.NaN,
                        Margin = new Thickness(3, 0, 3, 0),
                        BorderThickness = new Thickness(1, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        Style = Application.Current.FindResource("PageNumberButton") as Style
                    };
                    pagenumBtn.Click += PagenumBtn_Click;
                    if (i.Equals(0)) pagenumBtn.Tag = 1;  //Set the left control point
                    if (i.Equals(6)) pagenumBtn.Tag = 7;  //Set the right control point
                    this.mCurrentPagers.Add((i + 1));
                    if (this.PART_Content != null)
                    {
                        this.PART_Content.Children.Add(pagenumBtn);
                    }
                }
            }
            SetLinkButtonFocus(0);
        }

        /// <summary>
        /// Maintain page number data displayed by current paging control - single page switching
        /// </summary>
        /// <param name="addSubtract"></param>
        private void _MaintainCurrentPagersSingle(AddSubtract addSubtract)
        {
            if (addSubtract == AddSubtract.Add && CurrentPage < PageCount)
            {
                for (int i = 0; i < this.mCurrentPagers.Count; i++)
                {
                    this.mCurrentPagers[i] = this.mCurrentPagers[i] + 1;
                }
            }
            if (addSubtract == AddSubtract.subtract && CurrentPage > 1)
            {
                for (int i = 0; i < this.mCurrentPagers.Count; i++)
                {
                    this.mCurrentPagers[i] = this.mCurrentPagers[i] - 1;
                }
            }
        }

        /// <summary>
        /// Maintain page number data displayed by the current paging control - first and last page switching
        /// </summary>
        /// <param name="addSubtract"></param>
        private void _MaintainCurrentPagersFirstOrLast(AddSubtract addSubtract)
        {
            int _firstIndex = this.mCurrentPagers.First();
            int _lastIndex = this.mCurrentPagers.Last();
            if (addSubtract == AddSubtract.Add && CurrentPage <= PageCount)
            {
                for (int i = 0; i < this.mCurrentPagers.Count; i++)
                {
                    this.mCurrentPagers[i] = this.mCurrentPagers[i] + PageCount - _lastIndex;
                }
            }
            if (addSubtract == AddSubtract.subtract && CurrentPage >= 1)
            {
                for (int i = 0; i < this.mCurrentPagers.Count; i++)
                {
                    this.mCurrentPagers[i] = this.mCurrentPagers[i] - _lastIndex + 7;
                }
            }
        }

        /// <summary>
        /// Maintain the page number data displayed by the current paging control - jump page switch
        /// </summary>
        /// <param name="addSubtract"></param>
        private void _MaintainCurrentPagersGoto(AddSubtract addSubtract)
        {
            int _firstIndex = this.mCurrentPagers.First();
            int _lastIndex = this.mCurrentPagers.Last();
            if (addSubtract == AddSubtract.Add && CurrentPage <= PageCount)
            {
                for (int i = 0; i < this.mCurrentPagers.Count; i++)
                {
                    this.mCurrentPagers[i] = this.mCurrentPagers[i] + GotoPageNum - _lastIndex + 1;
                }
            }
            if (addSubtract == AddSubtract.subtract && CurrentPage >= 1)
            {
                for (int i = 0; i < this.mCurrentPagers.Count; i++)
                {
                    this.mCurrentPagers[i] = this.mCurrentPagers[i] - _lastIndex + GotoPageNum + 7 - 2;
                }
            }
        }

        /// <summary>
        /// Sets the status of the page change button
        /// </summary>
        private void SetNextpageAndPreviouspageState()
        {
            if (PageCount == 1 || PageCount == 0)
            {
                this.PART_FirstPage.IsEnabled = false;
                this.PART_Previouspage.IsEnabled = false;
                this.PART_LastPage.IsEnabled = false;
                this.PART_Nextpage.IsEnabled = false;
            }
            else if (CurrentPage == 1 && PageCount > 1)
            {
                this.PART_FirstPage.IsEnabled = false;
                this.PART_Previouspage.IsEnabled = false;
                this.PART_LastPage.IsEnabled = true;
                this.PART_Nextpage.IsEnabled = true;
            }
            else if (CurrentPage == PageCount)
            {
                this.PART_FirstPage.IsEnabled = true;
                this.PART_Previouspage.IsEnabled = true;
                this.PART_LastPage.IsEnabled = false;
                this.PART_Nextpage.IsEnabled = false;
            }
            else if (CurrentPage > 1 && PageCount > 2)
            {
                this.PART_FirstPage.IsEnabled = true;
                this.PART_Previouspage.IsEnabled = true;
                this.PART_LastPage.IsEnabled = true;
                this.PART_Nextpage.IsEnabled = true;
            }
        }

        /// <summary>
        /// Evaluates the index of the currently selected paging button
        /// </summary>
        private int CalculationCurrentSelectPagerButtonWithIndex()
        {
            //The page number set displayed by the current control
            return this.mCurrentPagers.FindIndex((o) => { return o == this.CurrentPage; });
        }

        private void _RefreshSinglePager(AddSubtract addSubtract)
        {
            if (this.PART_Content.Children.Count > 0)
            {
                int _index = 0;  //
                int _contentCount = this.PART_Content.Children.Count;
                for (int i = 0; i < _contentCount; i++)
                {
                    var pageNumBtn = this.PART_Content.Children[_index] as Button;
                    if (pageNumBtn != null)
                    {
                        if (PageCount > CurrentPage)
                        {
                            pageNumBtn.Content = addSubtract == AddSubtract.Add ? (Convert.ToInt32(pageNumBtn.Content) + 1).ToString() : (Convert.ToInt32(pageNumBtn.Content) - 1).ToString();
                        }
                    }
                    _index++;
                }
            }
        }

        private void _RefreshFirstOrLastPager(AddSubtract addSubtract)
        {
            if (this.PART_Content.Children.Count > 0)
            {
                int _index = 0;
                int _contentCount = this.PART_Content.Children.Count;
                int _lastIndex = this.mCurrentPagers.Last();
                for (int i = 0; i < _contentCount; i++)
                {
                    var pageNumBtn = this.PART_Content.Children[_index] as Button;
                    if (pageNumBtn != null)
                    {
                        if (PageCount >= CurrentPage)
                        {
                            pageNumBtn.Content = addSubtract == AddSubtract.Add ? (Convert.ToInt32(pageNumBtn.Content) + PageCount - _lastIndex).ToString() : (Convert.ToInt32(pageNumBtn.Content) - _lastIndex + 7).ToString();
                        }
                    }
                    _index++;
                }
            }
        }

        private void _RefreshGotoPager(AddSubtract addSubtract)
        {
            if (this.PART_Content.Children.Count > 0)
            {
                int _index = 0;
                int _contentCount = this.PART_Content.Children.Count;
                int _lastIndex = this.mCurrentPagers.Last();
                for (int i = 0; i < _contentCount; i++)
                {
                    var pageNumBtn = this.PART_Content.Children[_index] as Button;
                    if (pageNumBtn != null)
                    {
                        if (PageCount >= CurrentPage)
                        {
                            pageNumBtn.Content = addSubtract == AddSubtract.Add ? (Convert.ToInt32(pageNumBtn.Content) + GotoPageNum - _lastIndex + 1).ToString() : (Convert.ToInt32(pageNumBtn.Content) - _lastIndex + GotoPageNum + 7 - 2).ToString();
                        }
                    }
                    _index++;
                }
            }
        }
        private void _RefreshPagerBtn(Button pagenumBtn)
        {
            if (pagenumBtn.Tag != null)
            {
                if (pagenumBtn.Tag.Equals(1))
                {
                    if (this.CurrentPage > 1)
                    {
                        //Refresh UI
                        this._RefreshSinglePager(AddSubtract.subtract);
                        this._MaintainCurrentPagersSingle(AddSubtract.subtract);
                    }
                }
                if (pagenumBtn.Tag.Equals(7))
                {
                    if (this.CurrentPage < PageCount)
                    {
                        //Refresh UI
                        this._RefreshSinglePager(AddSubtract.Add);
                        this._MaintainCurrentPagersSingle(AddSubtract.Add);
                    }
                }
            }
        }

        /// <summary>
        /// Set the focus of the page number button
        /// </summary>
        /// <param name="_index"></param>
        private void SetLinkButtonFocus(int _index)
        {
            if (this.mPagerType == PagerType.Complex)
            {
                for (int i = 0; i < 7; i++)
                {
                    var beforepagenumBtn = this.PART_Content.Children[i] as Button;
                    beforepagenumBtn.Foreground = Brushes.Black;    //Set page number button color
                    if (i == _index)
                    {
                        //this.PART_Content.Children[_index].Focus();
                        var pagenumBtn = this.PART_Content.Children[_index] as Button;
                        pagenumBtn.Foreground = Brushes.Red;    //Set the page number button color of the current page
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.PageCount; i++)
                {
                    var beforepagenumBtn = this.PART_Content.Children[i] as Button;
                    beforepagenumBtn.Foreground = Brushes.Black;
                    if (i == _index)
                    {
                        // this.PART_Content.Children[_index].Focus();
                        var pagenumBtn = this.PART_Content.Children[_index] as Button;
                        pagenumBtn.Foreground = Brushes.Red;
                    }
                }
            }
        }

        protected virtual void OnPageChanged(RoutedEvent routed)
        {
            var eventArgs = new PageChangedEventArgs(this.CurrentPage) { RoutedEvent = routed, Source = this };
            this.RaiseEvent(eventArgs);
        }
        protected virtual void OnPageSizeChanged()
        {
            var eventArgs = new PageSizeChangedEventArgs(PageSize) { RoutedEvent = PageSizeChangedEvent, Source = this };
            this.RaiseEvent(eventArgs);
        }

        #endregion
    }

    /// <summary>
    /// Paging Control Type
    /// </summary>
    public enum PagerType
    {
        Default,
        Complex
    }
    public enum AddSubtract
    {
        Add, subtract
    }
    public class PageChangedEventArgs : RoutedEventArgs
    {
        public int PageIndex { get; set; }
        public PageChangedEventArgs(int pageIndex) : base()
        {
            PageIndex = pageIndex;
        }
    }
    public class PageSizeChangedEventArgs : RoutedEventArgs
    {
        public int PageSize { get; set; }
        public PageSizeChangedEventArgs(int pageSize) : base()
        {
            PageSize = pageSize;
        }
    }
}