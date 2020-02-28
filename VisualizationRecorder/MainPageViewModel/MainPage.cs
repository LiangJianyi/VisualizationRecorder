using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Janyee.Utilty;
using VisualizationRecorder.CommonTool;
using Windows.UI.Xaml.Media.Animation;

namespace VisualizationRecorder {
    using Debug = System.Diagnostics.Debug;
    using TioSalamanca = List<IGrouping<BigInteger, StatistTotalByDateTime>>;


    /// <summary>
    /// 控制 MainPage.xaml 的视图逻辑
    /// </summary>
    public sealed partial class MainPage {
        private static Windows.UI.Color LightGray => new Windows.UI.Color() { A = 255, R = 214, G = 218, B = 215 };
        private static Windows.UI.Color OneLevelColor => new Windows.UI.Color() { A = 255, R = 203, G = 229, B = 146 };
        private static Windows.UI.Color TwoLevelColor => new Windows.UI.Color() { A = 255, R = 142, G = 230, B = 107 };
        private static Windows.UI.Color ThreeLevelColor => new Windows.UI.Color() { A = 255, R = 78, G = 154, B = 67 };
        private static Windows.UI.Color FourLevelColor => new Windows.UI.Color() { A = 255, R = 11, G = 110, B = 0 };
        private static Windows.UI.Color FiveLevelColor => new Windows.UI.Color() { A = 255, R = 0, G = 58, B = 6 };

        private static IDictionary<int, SolidColorBrush> ClassifyColorByLevelScore(int groups) {
            switch (groups) {
                case 0:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) }
                            };
                case 1:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(FiveLevelColor) }
                            };
                case 2:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(FourLevelColor) },
                                { 2, new SolidColorBrush(FiveLevelColor) }
                            };
                case 3:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(ThreeLevelColor) },
                                { 2, new SolidColorBrush(FourLevelColor) },
                                { 3, new SolidColorBrush(FiveLevelColor) }
                            };
                case 4:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(TwoLevelColor) },
                                { 2, new SolidColorBrush(ThreeLevelColor) },
                                { 3, new SolidColorBrush(FourLevelColor) },
                                { 4, new SolidColorBrush(FiveLevelColor) }
                            };
                case 5:
                    return new Dictionary<int, SolidColorBrush>() {
                                { 0, new SolidColorBrush(LightGray) },
                                { 1, new SolidColorBrush(OneLevelColor) },
                                { 2, new SolidColorBrush(TwoLevelColor) },
                                { 3, new SolidColorBrush(ThreeLevelColor) },
                                { 4, new SolidColorBrush(FourLevelColor) },
                                { 5, new SolidColorBrush(FiveLevelColor) }
                            };
                default:
                    throw new System.ArgumentOutOfRangeException($"levelRange out of range: {groups}");
            }
        }

        /// <summary>
        /// 给相应的 Rectangle 标记上日期和星期数
        /// </summary>
        /// <param name="canvas"></param>
        private static void DateTag(Canvas canvas) {
            /*
             * 下面的查询语句用来提取画布中最左侧的一列方块(res1)和最上侧的一行方块(res2)，
             * 然后将这些方块标记上星期数和月份
             */
            IEnumerable<IGrouping<double, UIElement>> leftGroup = from rect in canvas.Children
                                                                  group rect by Canvas.GetLeft(rect);
            IEnumerable<IGrouping<double, UIElement>> topGroup = from rect in canvas.Children
                                                                 group rect by Canvas.GetTop(rect);
            double minLeftGroupKey = leftGroup.Min(group => group.Key);
            double minTopGroupKey = topGroup.Min(group => group.Key);
            IGrouping<double, UIElement> res1 = (from g in leftGroup
                                                 where g.Key == minLeftGroupKey
                                                 select g).First();
            IGrouping<double, UIElement> res2 = (from g in topGroup
                                                 where g.Key == minTopGroupKey
                                                 select g).First();

            /*
             * 给周一、周三、周五的方块打上标记 Mon、Wed、Fri
             */
            foreach (Rectangle rect in res1) {
                if (DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == DayOfWeek.Monday ||
                    DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == DayOfWeek.Wednesday ||
                    DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek == DayOfWeek.Friday) {
                    var tbx = new TextBlock() {
                        Text = DatetimeParser.ParseExpressToDateTime((rect as Rectangle).Name, DateMode.DateWithSlash).DayOfWeek.ToString().Substring(0, 3),
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Windows.UI.Colors.Gray)
                    };
                    Canvas.SetLeft(tbx, Canvas.GetLeft(rect) - 30);
                    Canvas.SetTop(tbx, Canvas.GetTop(rect));
                    canvas.Children.Add(tbx);
                }
            }

            /*
             * 给每个月份开头的方块打上标记，从 Jan 到 Dec
             */
            Rectangle previousRect = null;
            // 每个标记创建完成后会同对应的方块一起写入该列表
            List<(TextBlock Tag, Rectangle TagedRect)> tagList = new List<(TextBlock Tag, Rectangle TagedRect)>();
            foreach (Rectangle rect in res2.Reverse()) {
                void setTopTag(string text) {
                    var tag = new TextBlock() {
                        Text = text,
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Windows.UI.Colors.Gray)
                    };
                    Canvas.SetLeft(tag, Canvas.GetLeft(rect));
                    Canvas.SetTop(tag, Canvas.GetTop(rect) - 15);
                    tagList.Add((tag, rect));
                    /*
                     * 采用下面的设计是因为 TextBlock.ActualWidth 只有在控件加载完成后（Loaded）才会产生有效值，
                     * TextBlock.ActualWidth 是检测两个 tag 是否重叠的关键参数，检测公式为：
                     * Canvas.GetLeft(tagList[i].Tag) - Canvas.GetLeft(tagList[i - 1].Tag) - tagList[i - 1].Tag.ActualWidth <= 0
                     * 当该表达式为 true，表明 tagList 中某两个相邻的 tag 发生重叠，需要将右边的 tag 往右移动一个方块的距离避免重叠。
                     * 当 today 为一年中的某些天时，RectanglesCanvas 的第一列和第二列会是两个相邻的月份，如此紧凑的距离会导致他们顶部
                     * 的标记重叠，一个例子是 today 为 10/12/2019，当输入的纪录中含有 2017 年的记录时，那么渲染 2017 年对应的 RectanglesCanvas
                     * 会导致该 Canvas 的第一列和第二列的顶部标记重叠。
                     * 
                     * tag.Loaded 事件函数的作用是对 tagList 中的 tag 进行距离检测，当所有的 tag 在 setTopTag 函数中
                     * 创建完毕后，UI 开始加载 Canvas 中的 UIElement，这个过程会触发每个 tag 的 Loaded 事件，通过 Loaded
                     * 事件对每个 tag 的距离进行轮询。
                     */
                    tag.Loaded += (object sender, RoutedEventArgs e) => {
                        for (int i = 1; i < tagList.Count - 1; i++) {
                            if (Canvas.GetLeft(tagList[i].Tag) - Canvas.GetLeft(tagList[i - 1].Tag) - tagList[i - 1].Tag.ActualWidth <= 0) {
                                Debug.WriteLine($"Canvas.GetLeft(list[{i}].Tag) - Canvas.GetLeft(list[{i - 1}].Tag) - list[{i - 1}].Tag.ActualWidth = {Canvas.GetLeft(tagList[i].Tag)} - {Canvas.GetLeft(tagList[i - 1].Tag)} - {tagList[i - 1].Tag.ActualWidth} = {Canvas.GetLeft(tagList[i].Tag) - Canvas.GetLeft(tagList[i - 1].Tag) - tagList[i - 1].Tag.ActualWidth}");
                                Canvas.SetLeft(tagList[i].Tag, Canvas.GetLeft(tagList[i].Tag) + tagList[i].TagedRect.Width);
                            }
                        }
                    };
                    canvas.Children.Add(tag);
                }
                if (previousRect == null) {
                    setTopTag(DatetimeParser.NumberToMonth(DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash).Month));
                }
                else {
                    int monthOfPreviousRectangle = DatetimeParser.ParseExpressToDateTime(previousRect.Name, DateMode.DateWithSlash).Month;
                    int monthOfCurrentRectangle = DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash).Month;
                    if (monthOfCurrentRectangle != monthOfPreviousRectangle) {
                        setTopTag(DatetimeParser.NumberToMonth(DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash).Month));
                    }
                }
                previousRect = rect;
            }
        }

        /// <summary>
        /// 初始化方块矩阵面板的布局，并返回方块矩阵中日期最古老的方块
        /// </summary>
        /// <param name="rectanglesCanvas">方块矩阵面板</param>
        /// <param name="today">
        /// 方块矩阵的布局方式以传入的日期作为起点，然后时间线不断地往过去回溯，
        /// 以此产生新的行和列
        /// </param>
        /// <returns>返回方块矩阵中日期最古老的方块</returns>
        private Rectangle RectanglesLayout(Canvas rectanglesCanvas, DateTime today) {
            DateTime todayOfLastyear = new DateTime(today.Year - 1, today.Month, today.Day);
            TimeSpan pastDay = today - todayOfLastyear;
            const int RECT_WIDHT = 10;
            const int RECT_HEIGHT = 10;
            const int COLUMN_DISTANCE = 3;
            const int ROW_DISTANCE = 3;
            const int MONTH_TITLE_SPACE = 40;
            const int BOTTOM_SPACE = 20;
            const int LEFT_SPACE = 80;
            const int TOP_SPACE = 37;
            const int RIGHT_SPACE = LEFT_SPACE;
            int totalWeek = pastDay.Days / 7;
            rectanglesCanvas.Width = totalWeek * COLUMN_DISTANCE + LEFT_SPACE + RIGHT_SPACE + totalWeek * RECT_WIDHT + RECT_WIDHT;
            rectanglesCanvas.Height = ROW_DISTANCE * 6 + BOTTOM_SPACE + MONTH_TITLE_SPACE + 7 * RECT_HEIGHT;
            DateTime dateOfEachRectangle = today;
            Rectangle earliestRectangleDate = null; // 当前方块矩阵中日期最古老的方块
            for (int column = totalWeek; column >= 0; column--) {
                if (column == totalWeek) {
                    for (int row = Convert.ToInt32(today.DayOfWeek); row >= 0; row--, dateOfEachRectangle = dateOfEachRectangle.AddDays(-1)) {
                        CreateRectangle(
                            rectanglesCanvas: rectanglesCanvas,
                            rectWidth: RECT_WIDHT,
                            rectHeight: RECT_HEIGHT,
                            canvasLeft: column * RECT_WIDHT + COLUMN_DISTANCE * (column - 1) + LEFT_SPACE,
                            canvasTop: row * RECT_HEIGHT + row * ROW_DISTANCE + TOP_SPACE,
                            dateTime: dateOfEachRectangle
                        );
                    }
                }
                else {
                    for (int row = 6; row >= 0; row--, dateOfEachRectangle = dateOfEachRectangle.AddDays(-1)) {
                        if (row != 0) {
                            CreateRectangle(
                                rectanglesCanvas: rectanglesCanvas,
                                rectWidth: RECT_WIDHT,
                                rectHeight: RECT_HEIGHT,
                                canvasLeft: column * RECT_WIDHT + COLUMN_DISTANCE * (column - 1) + LEFT_SPACE,
                                canvasTop: row * RECT_HEIGHT + row * ROW_DISTANCE + TOP_SPACE,
                                dateTime: dateOfEachRectangle
                            );
                        }
                        else {
                            earliestRectangleDate = CreateRectangle(
                                rectanglesCanvas: rectanglesCanvas,
                                rectWidth: RECT_WIDHT,
                                rectHeight: RECT_HEIGHT,
                                canvasLeft: column * RECT_WIDHT + COLUMN_DISTANCE * (column - 1) + LEFT_SPACE,
                                canvasTop: row * RECT_HEIGHT + row * ROW_DISTANCE + TOP_SPACE,
                                dateTime: dateOfEachRectangle
                            );
                        }
                    }
                }
            }
            DateTag(rectanglesCanvas);
            return earliestRectangleDate;
        }

        /// <summary>
        /// 为 RectangleCanvas 创建方块
        /// </summary>
        /// <param name="rectanglesCanvas">放置方块的容器</param>
        /// <param name="rectWidth">方块的宽度</param>
        /// <param name="rectHeight">方块的高度</param>
        /// <param name="canvasLeft">方块的横轴坐标值</param>
        /// <param name="canvasTop">方块的纵轴坐标值</param>
        /// <param name="dateTime">方块代表的日期</param>
        /// <returns>返回创建的方块</returns>
        private Rectangle CreateRectangle(Canvas rectanglesCanvas, int rectWidth, int rectHeight, int canvasLeft, int canvasTop, DateTime dateTime) {
            Rectangle rect = new Rectangle {
                Name = dateTime.ToShortDateString(),
                Width = rectWidth,
                Height = rectHeight,
                Fill = new SolidColorBrush(LightGray),
            };
            rect.PointerReleased += Rect_PointerReleased;
#if DEBUG
            ToolTip toolTip = new ToolTip {
                Content = rect.Name + $"  Level:0  Total:0  Color:{(rect.Fill as SolidColorBrush).Color}"
            };
#else
            ToolTip toolTip = new ToolTip {
                Content = dateTime.ToShortDateString()
            };
#endif
            ToolTipService.SetToolTip(rect, toolTip);
            rectanglesCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, canvasLeft);
            Canvas.SetTop(rect, canvasTop);
            return rect;
        }

        /// <summary>
        /// 初始化页面和改变窗体大小都需要调用该方法对 UI 布局进行变更，
        /// 该方法为设计自适应界面而准备。
        /// </summary>
        private void UpdateMainPageLayout() {
            Menu.Width = this.Window.Bounds.Width;
            RootGrid.Width = this.Window.Bounds.Width;
            RootGrid.Height = this.Window.Bounds.Height - ((double)RootCanvas.Resources["CanvasTopForRootGrid"]);
            Canvas.SetTop(AvatarStack, 80);
            Canvas.SetLeft(AvatarStack, this.Window.Bounds.Width - AvatarStack.ActualWidth - 50);
            Canvas.SetTop(SettingPanle, 0);
            Canvas.SetLeft(SettingPanle, this.Window.Bounds.Width);
            SettingPanle.Height = this.Window.Bounds.Height;
        }

        /// <summary>
        /// 根据记录给每个方块面板中的方块绘制颜色。
        /// </summary>
        /// <param name="entries">分级后条目列表</param>
        /// <param name="haveProgressBoard">是否开启进度条面板，true 为开启，反之不开启</param>
        private void DrawRectangleColor(TioSalamanca[] entries, bool haveProgressBoard) {
            IDictionary<int, SolidColorBrush> colorDic = ClassifyColorByLevelScore(entries.Length);

            Windows.Foundation.IAsyncAction slideOnProgressBoardForCanvas = Windows.System.Threading.ThreadPool.RunAsync(
                async (asyncAction) => {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        priority: Windows.UI.Core.CoreDispatcherPriority.Normal,
                        agileCallback: () => {
                            foreach (Canvas canvas in this.StackCanvas.Children) {
                                if (haveProgressBoard) {
                                    ProgressBoard.SlideOn(canvas, new ProgressBoard());
                                }
                            }
                        }
                    );
                }
            );
            Windows.Foundation.IAsyncAction fillRectanglesColor = Windows.System.Threading.ThreadPool.RunAsync(
                async (asyncAction) => {
                    GusFring gusFring = new GusFring(entries, colorDic);
                    var rectangles = from c in this.StackCanvas.Children
                                     from r in ((Canvas)c).Children
                                     where r is Rectangle
                                     // 过滤掉非 Rectangle 的元素（比如 ProgressBoard 和 DateTag）
                                     select r as Rectangle;

                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        priority: Windows.UI.Core.CoreDispatcherPriority.Normal,
                        agileCallback: () => {
                            foreach (Canvas canvas in this.StackCanvas.Children) {
                                foreach (var item in canvas.Children) { // 过滤掉非 Rectangle 的元素（比如 ProgressBoard 和 DateTag）
                                    if (item is Rectangle rect) {
#if DEBUG
                                        (int Level, BigInteger Total, SolidColorBrush Color) = gusFring[rect.Name];
                                        rect.Fill = Color;
                                        ToolTip toolTip = new ToolTip {
                                            Content = rect.Name + $"  Level:{Level}  Total:{Total}  Color:{Color}"
                                        };
                                        ToolTipService.SetToolTip(rect, toolTip);
#endif
#if DEBUG == false
                                        rect.Fill = gusFring[rect.Name];
                                        ToolTip toolTip = new ToolTip {
                                            Content = rect.Name
                                        };
                                        ToolTipService.SetToolTip(rect, toolTip);
#endif
                                        _rectangleRegisteTable.Add(rect);
                                    }

                                }
                            }
                        });
                }
            );
        }

        /// <summary>
        /// 重置方块的颜色和闪烁状态，以及所有的 RectanglesCanvas 面板布局。
        /// </summary>
        private void ResetRectangleAndCanvasLayout() {
            if (_rectangleRegisteTable.Count > 0) {
                foreach (var rect in _rectangleRegisteTable) {
                    rect.Fill = new SolidColorBrush(LightGray);
                }
                // 重置方块颜色之后要紧接着清空该表
                _rectangleRegisteTable.Clear();
                // 停止所有闪烁状态的方块
                Blink.StopAllBlink();
                // 对 StackCanvas 重新洗牌
                this.StackCanvas.Children.Clear();
                this.CurrentRectanglesCanvas = new Canvas() {
                    Name = "CurrentRectanglesCanvas"
                };
                this.CurrentRectanglesCanvas.Loaded += (object sender, RoutedEventArgs e) =>
                 ProgressBoard.SlideOn(CurrentRectanglesCanvas, new ProgressBoard());
                this.StackCanvas.Children.Add(this.CurrentRectanglesCanvas);
                this._earliestRectangle = this.RectanglesLayout(this.CurrentRectanglesCanvas, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            }
            else {
                ProgressBoard.SlideOn(CurrentRectanglesCanvas, new ProgressBoard());
            }
        }

        /// <summary>
        /// 渲染最终效果
        /// </summary>
        /// <param name="model"></param>
        /// <param name="earliestRectangle"></param>
        private void Render(StatistTotalByDateTimeModel model, Rectangle earliestRectangle) {
            ExtendStackCanvasByFilterOldRecorders(EarlierThanEarliestRectangle(model.ToStatistTotalByDateTimeArray().ToList(), earliestRectangle), earliestRectangle);
            DrawRectangleColor(model.GroupDateTimesByTotal(), false);
        }

        /// <summary>
        /// 将变更作为新文件存储。
        /// </summary>
        private async Task SaveNewFileAsync() {
            FileSavePicker savePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "New Record"
            };
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt", ".mast" });
            _file = await savePicker.PickSaveFileAsync();
            if (_file != null) {
                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(_file);
                switch (status) {
                    case Windows.Storage.Provider.FileUpdateStatus.CompleteAndRenamed:
                    case Windows.Storage.Provider.FileUpdateStatus.Complete:
                        await FileIO.WriteLinesAsync(_file, _model.ToStringArray());
                        break;
                    case Windows.Storage.Provider.FileUpdateStatus.Incomplete:
                    case Windows.Storage.Provider.FileUpdateStatus.UserInputNeeded:
                    case Windows.Storage.Provider.FileUpdateStatus.CurrentlyUnavailable:
                    case Windows.Storage.Provider.FileUpdateStatus.Failed:
                    default:
                        throw new FilePickFaildException($"Pick a file faild! Windows.Storage.Provider.FileUpdateStatus = {status}");
                }
            }
            DrawRectangleColor(_model?.GroupDateTimesByTotal(), true);
        }

        /// <summary>
        /// 将变更覆盖原有文件。
        /// </summary>
        private async Task SaveOrginalFileAsync() {
            CachedFileManager.DeferUpdates(_file);
            await FileIO.WriteLinesAsync(_file, _model.ToStringArray());
            Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(_file);
            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete || status == Windows.Storage.Provider.FileUpdateStatus.CompleteAndRenamed) {
                Debug.WriteLine("File " + _file.Name + " was saved.");
            }
            else {
                throw new FileNotSaveException($"File {_file.Name} couldn't be saved.");
            }
            DrawRectangleColor(_model?.GroupDateTimesByTotal(), true);
        }

        /// <summary>
        /// 返回所有的事件记录中含有比 rect 的日期还要早的记录。
        /// </summary>
        /// <param name="entries">存储事件链的模型类</param>
        /// <param name="rect">用于日期比较的目标方块</param>
        /// <returns></returns>
        private static List<StatistTotalByDateTime> EarlierThanEarliestRectangle(List<StatistTotalByDateTime> entries, Rectangle rect) {
            List<StatistTotalByDateTime> earlierThanEarliestRectangleLik = new List<StatistTotalByDateTime>();
            foreach (var item in entries) {
                if (item.DateTime < DatetimeParser.ParseExpressToDateTime(rect.Name, DateMode.DateWithSlash)) {
                    earlierThanEarliestRectangleLik.Add(item);
                }
            }
            return earlierThanEarliestRectangleLik;
        }

        /// <summary>
        /// 筛选出没有在当前 RectanglesCanvas 面板中显示的、日期更久远的记录，
        /// 然后根据这些记录在 StackCanvas 面板中生成新的方块面板，接着在这些
        /// 新生成的面板中着色日期更久远的记录。
        /// </summary>
        private void ExtendStackCanvasByFilterOldRecorders(List<StatistTotalByDateTime> oldRecorders, Rectangle earliestRectangle, int canvasOrdinal = 1) {
            if (oldRecorders != null && oldRecorders.Count > 0) {
                Canvas oldRectanglesCanvas = new Canvas() {
                    Name = $"OldRectanglesCanvas_{canvasOrdinal}"
                };
                Rectangle oldRect = this.RectanglesLayout(oldRectanglesCanvas, DatetimeParser.ParseExpressToDateTime(earliestRectangle.Name, DateMode.DateWithSlash).AddDays(-1));
                oldRectanglesCanvas.Loaded += (object sender, RoutedEventArgs e) => {
                    Canvas canvas = sender as Canvas;
                    foreach (var item in canvas.Children) {
                        if (item is TextBlock tag) {
                            Debug.WriteLine($"tag.ActualWidth: {tag.ActualWidth}");
                        }
                    }
                };
                this.StackCanvas.Children.Insert(0, oldRectanglesCanvas);
                List<StatistTotalByDateTime> newOldRecorders = EarlierThanEarliestRectangle(oldRecorders, oldRect);
                ExtendStackCanvasByFilterOldRecorders(newOldRecorders, oldRect, canvasOrdinal + 1);
            }
            // 扩展结束后，给每个 RectanglesCanvas 附加进度条
            else {
                foreach (Canvas canvas in this.StackCanvas.Children) {
                    ProgressBoard.SlideOn(canvas, new ProgressBoard());
                }
            }
        }

        private static async Task<StatistTotalByDateTimeModel> EncodingToStatistTotalByDateTimeModelAsync(StorageFile file) {
            string text = await FileIO.ReadTextAsync(file);
            if (file.FileType == ".txt") {
                IEnumerable<string> lines = DatetimeParser.SplitByLine(text);
                return new StatistTotalByDateTimeModel(
                    lines: lines,
                    dateMode: Tool.LocalSetting.LocalSettingInstance.DateMode
                );
            }
            else if (file.FileType == ".mast") {
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
                return new StatistTotalByDateTimeModel(statistTotalByDateTimes);
            }
            else {
                throw new FilePickFaildException($"错误的文件类型：{_file.FileType}");
            }
        }

        /// <summary>
        /// 设置面板滑入主页面
        /// </summary>
        private void SettingPanleSlideIn() {
            Storyboard storyboard = new Storyboard();
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 100));
            DoubleAnimationUsingKeyFrames slideAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();

            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetLeft(SettingPanle),
                KeyTime = startTime
            });
            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = this.Window.Bounds.Width - SettingPanle.Width,
                KeyTime = endTime
            });
            storyboard.Children.Add(slideAnimationUsingKeyFrames);
            Storyboard.SetTarget(slideAnimationUsingKeyFrames, SettingPanle);
            Storyboard.SetTargetName(slideAnimationUsingKeyFrames, SettingPanle.Name);
            Storyboard.SetTargetProperty(slideAnimationUsingKeyFrames, "(Canvas.Left)");
            storyboard.Begin();
        }

        /// <summary>
        /// 设置面板滑出主页面
        /// </summary>
        private void SettingPanleSlideOut() {
            Storyboard storyboard = new Storyboard();
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 100));
            DoubleAnimationUsingKeyFrames slideAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();

            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetLeft(SettingPanle),
                KeyTime = startTime
            });
            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = this.Window.Bounds.Width,
                KeyTime = endTime
            });
            storyboard.Children.Add(slideAnimationUsingKeyFrames);
            Storyboard.SetTarget(slideAnimationUsingKeyFrames, SettingPanle);
            Storyboard.SetTargetName(slideAnimationUsingKeyFrames, SettingPanle.Name);
            Storyboard.SetTargetProperty(slideAnimationUsingKeyFrames, "(Canvas.Left)");
            storyboard.Begin();
        }
    }
}
