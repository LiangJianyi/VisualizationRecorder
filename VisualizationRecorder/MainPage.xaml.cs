using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using VisualizationRecorder.CommonTool;
using Windows.UI.Xaml.Navigation;
using Janyee.Utilty;

namespace VisualizationRecorder {
    using Debug = System.Diagnostics.Debug;
    using TioSalamanca = List<IGrouping<BigInteger, StatistTotalByDateTime>>;

    public sealed partial class MainPage : Page {
        /// <summary>
        /// 获取当前 Window 对象
        /// </summary>
        private Window Window => Window.Current;
        /// <summary>
        /// 保存方块矩阵中日期最古老的方块
        /// </summary>
        private Rectangle _earliestRectangle = null;
        /// <summary>
        /// 注册已经填充颜色的 Rectangle，每个 Rectangle 只能注册一次
        /// </summary>
        private static HashSet<Rectangle> _rectangleRegisteTable = new HashSet<Rectangle>();
        /// <summary>
        /// 暂存当前页面的 StatistTotalByDateTime 集合
        /// </summary>
        private static StatistTotalByDateTimeModel _model = null;
        /// <summary>
        /// 文件的保存模式
        /// </summary>
        private static SaveMode _saveMode = SaveMode.NewFile;
        /// <summary>
        /// 保存从文件选择器选取的文件
        /// </summary>
        private static StorageFile _file = null;

        public MainPage() {
            this.Window.SizeChanged += Current_SizeChanged;
            this.InitializeComponent();
            this._earliestRectangle = this.RectanglesLayout(this.CurrentRectanglesCanvas, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
        }

        /// <summary>
        /// 打开文件选取器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenFileButtonAsync_Click(object sender, RoutedEventArgs e) {
            FileOpenPicker openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".mast");

            _file = await openPicker.PickSingleFileAsync();

            if (_file != null) {
                ResetRectangleAndCanvasLayout();  // 每次选择文件之后都要重置方块颜色和面板布局

                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(_file);
                string text = await FileIO.ReadTextAsync(_file);
                try {
                    if (_file.FileType == ".txt") {
                        IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
                        _model = new StatistTotalByDateTimeModel(lines);
                    }
                    else if (_file.FileType == ".mast") {
                        JymlAST.Cons ast = JymlParser.Parser.GenerateAst(text);
                        Interpreter.SuckerMLInterpreter.Sucker sucker = Interpreter.SuckerMLInterpreter.Eval(ast);
                        List<StatistTotalByDateTime> statistTotalByDateTimes = new List<StatistTotalByDateTime>();
                        foreach (var year in sucker.Years) {
                            foreach (var month in year.Value.Months) {
                                foreach (var day in month.Value.Days) {
                                    statistTotalByDateTimes.Add(new StatistTotalByDateTime() {
                                        DateTime = new DateTime(
                                            year: year.Value.Year.BigIntegerToInt32(),
                                            month: month.Value.Month,
                                            day: day.Value.Day
                                        ),
                                        Total = day.Value.Total
                                    });
                                }
                            }
                        }
                        _model = new StatistTotalByDateTimeModel(statistTotalByDateTimes);
                    }
                    else {
                        throw new FilePickFaildException($"错误的文件类型：{_file.FileType}");
                    }
                    Render(_model, _earliestRectangle);
                }
                catch (ArgumentException err) {
                    PopErrorDialogAsync(err.Message);
                }
                catch (FilePickFaildException err) {
                    PopErrorDialogAsync(err.Message);
                }
                _saveMode = SaveMode.OrginalFile; // 表示当前的操作基于磁盘上已有的文件 
            }
        }

        /// <summary>
        /// 每个方块都要响应鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rect_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            Rectangle rectangle = sender as Rectangle;
            Bubble.CreateBubbleRectangle(
                canvas: (sender as Rectangle).Parent as Canvas,
                hostRect: rectangle,
                bubbleName: VisualEventFrequencyRecorder.Resources["OhhohoRect"] as string,
                zoom: (minWidth: (double)VisualEventFrequencyRecorder.Resources["MinWidth"],
                       minHeight: (double)VisualEventFrequencyRecorder.Resources["MinHeight"],
                       maxWidth: (double)VisualEventFrequencyRecorder.Resources["MaxWidth"],
                       maxHeight: (double)VisualEventFrequencyRecorder.Resources["MaxHeight"])
            );

            /*
             * 气泡动画结束后从 Canvas 移除气泡方块。
             * 把 RectangleBubbleAnimation_Completed 作为局部方法的原因是：
             * 能够让 handler 在事件触发时捕获外部的 sender，让触发动画播放的
             * UI 对象能进入此 handler 的作用域，达到动画播放结束后移除该 UI 对象的效果。
             */
            void RectangleBubbleAnimation_Completed(object animation, object animationEventArg) {
                ((sender as Rectangle).Parent as Canvas).Children.Remove(Bubble._bubble);
            }

            Bubble.CreateBubbleStoryboard(
                hostPosition: (left: Canvas.GetLeft(rectangle), top: Canvas.GetTop(rectangle)),
                storyboard_Completed: RectangleBubbleAnimation_Completed
            );
            // 闪烁动画，提示用户该方块有未保存的变更；
            Blink.PlayBlink(rectangle);
            if (_model != null) {
                // 检测用户点击的方块对应的日期在之前打开的记录表中是否存在。
                // 如果 x.Count() > 0 为 true 证明存在，否则添加新条目。
                // 注意：x.Count() 和 x.First() 可能会导致两次查询，具体详情参见 MSDN
                var x = from entry in _model.Entries
                        where entry.Value.DateTime.ToShortDateString() == rectangle.Name
                        select entry;
                if (x.Count() > 0) { // 点击绿色方块
                    x.First().Value.Total += 1;
#if DEBUG
                    ToolTip toolTip = new ToolTip {
                        Content = rectangle.Name + $"  Level:0  Total:{x.First().Value.Total}  Color:{(rectangle.Fill as SolidColorBrush).Color}"
                    };
                    ToolTipService.SetToolTip(rectangle, toolTip);
#endif
                }
                else {  // 点击灰色方块
                    _model.AddEntry(rectangle.Name);
#if DEBUG
                    ToolTip toolTip = new ToolTip {
                        Content = rectangle.Name + $"  Level:0  Total:1  Color:{(rectangle.Fill as SolidColorBrush).Color}"
                    };
                    ToolTipService.SetToolTip(rectangle, toolTip);
#endif
                }
            }
            else {
                // _model 为 null 证明用户在空白的状态下添加新条目
                _model = new StatistTotalByDateTimeModel(new string[] { rectangle.Name }, DateMode.DateWithSlash);
#if DEBUG
                ToolTip toolTip = new ToolTip {
                    Content = rectangle.Name + $"  Level:0  Total:1  Color:{(rectangle.Fill as SolidColorBrush).Color}"
                };
                ToolTipService.SetToolTip(rectangle, toolTip);
#endif
            }
            // 显示保存按钮，将变更添加到指定文件中
            if (SaveFileButton.Visibility == Visibility.Collapsed) {
                SaveFileButton.Visibility = Visibility.Visible;
            }
            // 显示刷新按钮，根据变更的时间频率对方块重新着色
            if (RefreshButton.Visibility == Visibility.Collapsed) {
                RefreshButton.Visibility = Visibility.Visible;
            }
            // 显示清空按钮
            if (ClearButton.Visibility == Visibility.Collapsed) {
                ClearButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 弹出错误消息框
        /// </summary>
        /// <param name="content"></param>
        private static async void PopErrorDialogAsync(string content) {
            ContentDialog fileOpenFailDialog = new ContentDialog {
                Title = "Error",
                Content = content,
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await fileOpenFailDialog.ShowAsync();
        }

        /// <summary>
        /// 当前窗体大小发生变化时需要更新 UI 布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
#if DEBUG
            Debug.WriteLine($"{this.Window.Bounds.Width} , {this.Window.Bounds.Height}");
#endif
            UpdateMainPageLayout();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveFileButtonAsync_Click(object sender, RoutedEventArgs e) {
            /*
             * 把 DrawRectangleColor(_model?.GroupDateTimesByTotal()) 写进下面两个 Save 异步方法
             * 可以避免用户触发 CloseButtonClick 事件时对方块颜色重新渲染
             */
            async void saveDialog_PrimaryButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args) {
                await SaveOrginalFileAsync();
            }
            async void saveDialog_SecondaryButtonClickAsync(ContentDialog dialog, ContentDialogButtonClickEventArgs args) {
                await SaveNewFileAsync();
            }
            void saveDialog_CloseButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args) {
                dialog.Hide();
            }

            // 在弹出路径选择器之前应渲染一个悬浮表单，让用户选择
            // 保存方式、文件格式、文件名
            // 给用户提供两种保存方式：
            // 1、更新原有文件
            // 2、作为新文件存储
            switch (_saveMode) {
                case SaveMode.NewFile:
                    await SaveNewFileAsync();
                    _saveMode = SaveMode.OrginalFile;
                    break;
                case SaveMode.OrginalFile:
                    ContentDialog saveDialog = new ContentDialog() {
                        Title = "SaveMode",
                        Content = "选择一种保存方式：",
                        PrimaryButtonText = "更新原有文件",
                        SecondaryButtonText = "作为新文件存储",
                        CloseButtonText = "放弃更改"
                    };
                    saveDialog.PrimaryButtonClick += saveDialog_PrimaryButtonClick;
                    saveDialog.SecondaryButtonClick += saveDialog_SecondaryButtonClickAsync;
                    saveDialog.CloseButtonClick += saveDialog_CloseButtonClick;
                    await saveDialog.ShowAsync();
                    break;
                default:
                    throw new InvalidOperationException($"Unknown Error. SaveMode = {_saveMode.ToString()}");
            }
        }


        /// <summary>
        /// 页面加载完成后要对部分控件的视觉状态进行预设
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_Loaded(object sender, RoutedEventArgs e) {
            SaveFileButton.Visibility = Visibility.Collapsed;
            RefreshButton.Visibility = Visibility.Collapsed;
            ClearButton.Visibility = Visibility.Collapsed;
            UpdateMainPageLayout();
        }

        /// <summary>
        /// 刷新方块颜色和方块面板布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e) {
            /*
             * _rectangleRegisteTable 已在 ResetRectangleColor() 内部重新初始化，
             * 这里无需再次执行 _rectangleRegisteTable = new HashSet<Rectangle>()
             */
            ResetRectangleAndCanvasLayout();
            DrawRectangleColor(_model?.GroupDateTimesByTotal(), true);
            Blink.BlinkedRectangles.Clear();
        }

        /// <summary>
        /// 清空所有记录，重置所有状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e) {
            /*
             * _rectangleRegisteTable 已在 ResetRectangleColor() 内部重新初始化，
             * 这里无需再次执行 _rectangleRegisteTable = new HashSet<Rectangle>()
             */
            ResetRectangleAndCanvasLayout();
            _model = null;
            _file = null;
            _saveMode = SaveMode.NewFile;
            Blink.BlinkedRectangles.Clear();
            RefreshButton.Visibility = Visibility.Collapsed;
            SaveFileButton.Visibility = Visibility.Collapsed;
            ClearButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 导航到MainPage页面时会触发该方法
        /// </summary>
        /// <param name="e">从 Source Page 传递过来的参数</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (!(e.Parameter is Configuration res)) {
                TitleTextBlock.Visibility = Visibility.Collapsed;
                AvatarStack.Visibility = Visibility.Collapsed;
            }
            else if (string.IsNullOrEmpty(res.Title)) {
                TitleTextBlock.Visibility = Visibility.Collapsed;
            }
            else {
                TitleTextBlock.Text = res.Title;
                UserName.Text = res.UserName;
                if (res.Avatar != null) {
                    Task avatarTask = Tool.GetAvatarAsync(Avatar, res, Convert.ToInt32(Avatar.Width), Convert.ToInt32(Avatar.Height));
                }
                else {  // 如果 res.Avatar 为空，表明用户还未上传头像，开始使用本地图像
                    Task.WhenAll(Tool.GetDefaultAvatarAsync(res),
                                 Tool.LoadImageFromStreamAsync(Avatar, res.Avatar, Convert.ToInt32(Avatar.Width), Convert.ToInt32(Avatar.Height)));

                }
            }
            base.OnNavigatedTo(e);
        }
    }
}
