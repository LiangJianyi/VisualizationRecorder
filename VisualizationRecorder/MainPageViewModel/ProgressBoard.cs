using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;

namespace VisualizationRecorder {
    class ProgressBoard {
        private Canvas _progressBoard;
        private Storyboard _animation;

        private Canvas CreateProgressBoard(string name) {
            Canvas canvas = new Canvas() {
                Name = name,
                Width = 50,
                Height = 50,
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.LightGray),
                Opacity = 0.7
            };
            Canvas.SetZIndex(canvas, 2);
            ProgressRing processingRing = new ProgressRing() {
                Width = 40,
                Height = 40,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black),
                IsActive = true
            };
            canvas.Children.Add(processingRing);
            HorizontalCenterOnCanvas(processingRing, canvas);
            VerticalCenterOnCanvas(processingRing, canvas);
            return canvas;
        }

        /// <summary>
        /// 将进度条面板以父级面板为基准进行水平居中
        /// </summary>
        /// <param name="progressBoardCanvas">进度条面板</param>
        /// <param name="parentCanvas">父级面板</param>
        private static void HorizontalCenterOnCanvas(Canvas progressBoardCanvas, Canvas parentCanvas) {
            Canvas.SetLeft(progressBoardCanvas, (parentCanvas.Width - progressBoardCanvas.Width) / 2);
        }

        /// <summary>
        /// 将进度条控件以进度条面板为基准进行水平居中
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="progressBoardCanvas">进度条面板</param>
        private static void HorizontalCenterOnCanvas(Control control, Canvas progressBoardCanvas) {
            Canvas.SetLeft(control, (progressBoardCanvas.ActualWidth - control.Width) / 2);
        }

        /// <summary>
        /// 将进度条面板以父级面板为基准进行垂直居中
        /// </summary>
        /// <param name="progressBoardCanvas">进度条面板</param>
        /// <param name="parentCanvas">父级面板</param>
        private static void VerticalCenterOnCanvas(Canvas progressBoardCanvas, Canvas parentCanvas) {
            Canvas.SetTop(progressBoardCanvas, (parentCanvas.Height - progressBoardCanvas.Height) / 2);
        }

        /// <summary>
        /// 将进度条控件以进度条面板为基准进行垂直居中
        /// </summary>
        /// <param name="control"></param>
        /// <param name="progressBoardCanvas"></param>
        private static void VerticalCenterOnCanvas(Control control, Canvas progressBoardCanvas) {
            Canvas.SetTop(control, (progressBoardCanvas.ActualHeight - control.Width) / 2);
        }

        /// <summary>
        /// 播放进度条模块动画
        /// </summary>
        /// <param name="parentCanvas">承载进度条模块的容器</param>
        /// <param name="progressBoard">进度条模块</param>
        public static void SlideOn(Canvas parentCanvas, ProgressBoard progressBoard) {
            progressBoard._progressBoard = progressBoard.CreateProgressBoard("ProgressBoard");
            if (parentCanvas.Children.Contains(progressBoard._progressBoard) == false) {
                parentCanvas.Children.Add(progressBoard._progressBoard);
            }
            HorizontalCenterOnCanvas(progressBoard._progressBoard, parentCanvas);

            progressBoard._animation = new Storyboard();
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
            DoubleAnimationUsingKeyFrames slideAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();

            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Canvas.GetTop(progressBoard._progressBoard),
                KeyTime = startTime
            });
            slideAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = (parentCanvas.Height - progressBoard._progressBoard.Height) / 2,
                KeyTime = endTime
            });

            /*
             * 把 startStoryboard.Completed 事件的 Handler 作为局部函数能让它的作用域捕获 ProgressBoard._progressBoard,
             * 从而使得 Completed 事件能够在动画播放结束时为 parentCanvas 移除  ProgressBoard._progressBoard
             */
            void StartStoryboard_Completed(object sender, object e) {
                parentCanvas.Children.Remove(progressBoard._progressBoard);
            }
            progressBoard._animation.Completed += StartStoryboard_Completed;
            progressBoard._animation.Children.Add(slideAnimationUsingKeyFrames);
            Storyboard.SetTarget(slideAnimationUsingKeyFrames, progressBoard._progressBoard);
            Storyboard.SetTargetName(slideAnimationUsingKeyFrames, progressBoard._progressBoard.Name);
            Storyboard.SetTargetProperty(slideAnimationUsingKeyFrames, "(Canvas.Top)");
            progressBoard._animation.Begin();
        }

        /// <summary>
        /// 取消进度条模块
        /// </summary>
        /// <param name="parentCanvas">承载进度条模块的容器</param>
        /// <param name="progressBoard">进度条模块</param>
        public static void CancelOn(Canvas parentCanvas, ProgressBoard progressBoard) {
            parentCanvas.Children.Remove(progressBoard._progressBoard);
        }
    }
}
