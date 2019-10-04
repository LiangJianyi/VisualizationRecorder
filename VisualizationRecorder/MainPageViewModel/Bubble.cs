using System;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;

namespace VisualizationRecorder {
    static class Bubble {
        /// <summary>
        /// 表示气泡的大小，用一个元组表示，该元组包含4个double值指示气泡的最小状态和最大状态
        /// </summary>
        private static (double minWidth, double minHeight, double maxWidth, double maxHeight) _zoom;
        /// <summary>
        /// 表示当前气泡对象
        /// </summary>
        internal static Rectangle _bubble;

        /// <summary>
        /// 为相应的方块创建气泡，当鼠标点击该方块时会在它的上层弹出气泡
        /// </summary>
        /// <param name="canvas">方块所在的面板</param>
        /// <param name="hostRect">弹气泡的宿主方块</param>
        /// <param name="bubbleName">气泡对象名称</param>
        /// <param name="zoom">气泡大小，用一个元组表示，该元组包含4个double值指示气泡的最小状态和最大状态</param>
        internal static void CreateBubbleRectangle(Canvas canvas,
                                                   Rectangle hostRect,
                                                   string bubbleName,
                                                   (double minWidth, double minHeight, double maxWidth, double maxHeight) zoom) {
            _zoom = zoom;
            _bubble = new Rectangle() {
                Name = bubbleName,
                Width = hostRect.Width,
                Height = hostRect.Height,
                Fill = hostRect.Fill
            };
            Canvas.SetLeft(_bubble, Canvas.GetLeft(hostRect));
            Canvas.SetTop(_bubble, Canvas.GetTop(hostRect));
            Canvas.SetZIndex(_bubble, Canvas.GetZIndex(hostRect) + 1);
            canvas.Children.Add(_bubble);
        }

        // 下面的代码会导致气泡方块有奇怪的闪烁
        //internal static void CreateStoryboard(Page page, Rectangle target, EventHandler<object> storyboard_Completed) {
        //    (double left, double top) bubblePosition = (Canvas.GetLeft(target), Canvas.GetTop(target));

        //    Storyboard storyboard = new Storyboard {
        //        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300))
        //    };
        //    storyboard.Completed += storyboard_Completed;
        //    /*
        //     * 将下面两个动画的 SpeedRatio 设置为相对于其父级容器的7倍，
        //     * 否则动画无法在300毫秒内达到80x80的面积
        //     */
        //    DoubleAnimation width_doubleAnimation = new DoubleAnimation {
        //        From = 8,
        //        To = 80,
        //        SpeedRatio = 7,
        //        EnableDependentAnimation = true
        //    };
        //    DoubleAnimation height_doubleAnimation = new DoubleAnimation {
        //        From = 8,
        //        To = 80,
        //        SpeedRatio = 7,
        //        EnableDependentAnimation = true
        //    };
        //    DoubleAnimation opacity_doubleAnimation = new DoubleAnimation {
        //        From = 1,
        //        To = 0,
        //        SpeedRatio = 2,
        //        EnableDependentAnimation = true
        //    };
        //    DoubleAnimation canvasLeft_doubleAnimation = new DoubleAnimation {
        //        From = bubblePosition.left,
        //        To = bubblePosition.left - 25,
        //        SpeedRatio = 7,
        //        EnableDependentAnimation = true
        //    };
        //    DoubleAnimation canvasTop_doubleAnimation = new DoubleAnimation {
        //        From = bubblePosition.top,
        //        To = bubblePosition.top - 25,
        //        SpeedRatio = 7,
        //        EnableDependentAnimation = true
        //    };

        //    storyboard.Children.Add(width_doubleAnimation);
        //    storyboard.Children.Add(height_doubleAnimation);
        //    storyboard.Children.Add(opacity_doubleAnimation);
        //    storyboard.Children.Add(canvasLeft_doubleAnimation);
        //    storyboard.Children.Add(canvasTop_doubleAnimation);

        //    Storyboard.SetTarget(width_doubleAnimation, target);
        //    Storyboard.SetTarget(height_doubleAnimation, target);
        //    Storyboard.SetTarget(opacity_doubleAnimation, target);
        //    Storyboard.SetTarget(canvasLeft_doubleAnimation, target);
        //    Storyboard.SetTarget(canvasTop_doubleAnimation, target);

        //    Storyboard.SetTargetName(width_doubleAnimation, target.Name);
        //    Storyboard.SetTargetProperty(width_doubleAnimation, "Width");

        //    Storyboard.SetTargetName(height_doubleAnimation, target.Name);
        //    Storyboard.SetTargetProperty(height_doubleAnimation, "Height");

        //    Storyboard.SetTargetName(opacity_doubleAnimation, target.Name);
        //    Storyboard.SetTargetProperty(opacity_doubleAnimation, "Opacity");

        //    Storyboard.SetTargetName(canvasLeft_doubleAnimation, target.Name);
        //    Storyboard.SetTargetProperty(canvasLeft_doubleAnimation, "(Canvas.Left)");

        //    Storyboard.SetTargetName(canvasTop_doubleAnimation, target.Name);
        //    Storyboard.SetTargetProperty(canvasTop_doubleAnimation, "(Canvas.Top)");

        //    storyboard.Begin();
        //}

        /// <summary>
        /// 创建气泡弹出动画
        /// </summary>
        /// <param name="hostPosition">携带气泡的宿主方块的坐标</param>
        /// <param name="storyboard_Completed">动画结束事件</param>
        internal static void CreateBubbleStoryboard((double left, double top) hostPosition, EventHandler<object> storyboard_Completed) {

            #region 与下面代码等价的 XAML
            // 气泡方块在 Canvas 的 Position 初始化为其宿主方块的 Position，这个初始化行为在 CreateBubbleRectangle 里已经完成了
            //
            // <Storyboard x:Name="SecondStoryboard">
            //            <DoubleAnimationUsingKeyFrames Storyboard.TargetName="TrumpPop" Storyboard.TargetProperty="Width" EnableDependentAnimation="True">
            //                  <LinearDoubleKeyFrame Value="8" KeyTime="0:0:0"/>
            //                  <LinearDoubleKeyFrame Value="80" KeyTime="0:0:0.3"/>
            //            </DoubleAnimationUsingKeyFrames>
            //            <DoubleAnimationUsingKeyFrames Storyboard.TargetName="TrumpPop" Storyboard.TargetProperty="Height" EnableDependentAnimation="True">
            //                  <LinearDoubleKeyFrame Value="8" KeyTime="0:0:0"/>
            //                  <LinearDoubleKeyFrame Value="80" KeyTime="0:0:0.3"/>
            //            </DoubleAnimationUsingKeyFrames>
            //            <DoubleAnimationUsingKeyFrames Storyboard.TargetName="TrumpPop" Storyboard.TargetProperty="Opacity" EnableDependentAnimation="True">
            //                  <LinearDoubleKeyFrame Value="1" KeyTime="0:0:0"/>
            //                  <LinearDoubleKeyFrame Value="0" KeyTime="0:0:0.3"/>
            //            </DoubleAnimationUsingKeyFrames>
            //            <DoubleAnimationUsingKeyFrames Storyboard.TargetName="TrumpPop" Storyboard.TargetProperty="(Canvas.Left)" EnableDependentAnimation="True">
            //                  <LinearDoubleKeyFrame Value="Canvas.GetLeft(target)" KeyTime="0:0:0"/>
            //                  <LinearDoubleKeyFrame Value="Canvas.GetLeft(target) - 25" KeyTime="0:0:0.3"/>
            //            </DoubleAnimationUsingKeyFrames>
            //            <DoubleAnimationUsingKeyFrames Storyboard.TargetName="TrumpPop" Storyboard.TargetProperty="(Canvas.Top)" EnableDependentAnimation="True">
            //                  <LinearDoubleKeyFrame Value="Canvas.GetTop(target)" KeyTime="0:0:0"/>
            //                  <LinearDoubleKeyFrame Value="Canvas.GetTop(target) - 25" KeyTime="0:0:0.3"/>
            //            </DoubleAnimationUsingKeyFrames>
            // </Storyboard>
            #endregion

            Storyboard storyboard = new Storyboard();
            if (storyboard_Completed != null) {
                storyboard.Completed += storyboard_Completed;
            }
            KeyTime startTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0));
            KeyTime endTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));

            DoubleAnimationUsingKeyFrames width_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            width_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = _zoom.minWidth,
                KeyTime = startTime
            });
            width_DoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = _zoom.maxWidth,
                KeyTime = endTime
            });

            DoubleAnimationUsingKeyFrames height_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            height_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = _zoom.minHeight,
                    KeyTime = startTime
                });
            height_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = _zoom.maxHeight,
                    KeyTime = endTime
                });

            DoubleAnimationUsingKeyFrames opacity_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            opacity_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = 1,
                    KeyTime = startTime
                });
            opacity_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = 0,
                    KeyTime = endTime
                });

            DoubleAnimationUsingKeyFrames canvasLeft_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            canvasLeft_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = hostPosition.left,
                    KeyTime = startTime
                });
            canvasLeft_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = hostPosition.left - 36,
                    KeyTime = endTime
                });

            DoubleAnimationUsingKeyFrames canvasTop_DoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = true };
            canvasTop_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = hostPosition.top,
                    KeyTime = startTime
                });
            canvasTop_DoubleAnimationUsingKeyFrames.KeyFrames.Add(
                new LinearDoubleKeyFrame() {
                    Value = hostPosition.top - 36,
                    KeyTime = endTime
                });

            storyboard.Children.Add(width_DoubleAnimationUsingKeyFrames);
            storyboard.Children.Add(height_DoubleAnimationUsingKeyFrames);
            storyboard.Children.Add(opacity_DoubleAnimationUsingKeyFrames);
            storyboard.Children.Add(canvasLeft_DoubleAnimationUsingKeyFrames);
            storyboard.Children.Add(canvasTop_DoubleAnimationUsingKeyFrames);

            Storyboard.SetTarget(width_DoubleAnimationUsingKeyFrames, Bubble._bubble);
            Storyboard.SetTarget(height_DoubleAnimationUsingKeyFrames, Bubble._bubble);
            Storyboard.SetTarget(opacity_DoubleAnimationUsingKeyFrames, Bubble._bubble);
            Storyboard.SetTarget(canvasLeft_DoubleAnimationUsingKeyFrames, Bubble._bubble);
            Storyboard.SetTarget(canvasTop_DoubleAnimationUsingKeyFrames, Bubble._bubble);

            Storyboard.SetTargetName(width_DoubleAnimationUsingKeyFrames, Bubble._bubble.Name);
            Storyboard.SetTargetName(height_DoubleAnimationUsingKeyFrames, Bubble._bubble.Name);
            Storyboard.SetTargetName(opacity_DoubleAnimationUsingKeyFrames, Bubble._bubble.Name);
            Storyboard.SetTargetName(canvasLeft_DoubleAnimationUsingKeyFrames, Bubble._bubble.Name);
            Storyboard.SetTargetName(canvasTop_DoubleAnimationUsingKeyFrames, Bubble._bubble.Name);

            Storyboard.SetTargetProperty(width_DoubleAnimationUsingKeyFrames, "Width");
            Storyboard.SetTargetProperty(height_DoubleAnimationUsingKeyFrames, "Height");
            Storyboard.SetTargetProperty(opacity_DoubleAnimationUsingKeyFrames, "Opacity");
            Storyboard.SetTargetProperty(canvasLeft_DoubleAnimationUsingKeyFrames, "(Canvas.Left)");
            Storyboard.SetTargetProperty(canvasTop_DoubleAnimationUsingKeyFrames, "(Canvas.Top)");

            storyboard.Begin();
        }
    }
}
