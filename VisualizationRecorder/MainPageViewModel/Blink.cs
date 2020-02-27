using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace VisualizationRecorder {
    /// <summary>
    /// 表示闪烁动画，提示用户该方块有未保存的变更
    /// </summary>
    static class Blink {
        /// <summary>
        /// 记录处于闪烁状态的方块的名称
        /// </summary>
        public static SortedDictionary<string, (Rectangle rectangle, Storyboard storyboard)> BlinkedRectangles { get; } = new SortedDictionary<string, (Rectangle rectangle, Storyboard storyboard)>();

        public static void PlayBlink(Rectangle target) {
            if (!BlinkedRectangles.ContainsKey(target.Name)) {
                BlinkedRectangles.Add(target.Name, (target, new Storyboard()));
                KeyTime redTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
                KeyTime orginalTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600));

                ColorAnimationUsingKeyFrames colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames() {
                    EnableDependentAnimation = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                DiscreteColorKeyFrame OrginalColorToRed = new DiscreteColorKeyFrame() {
                    Value = Windows.UI.Colors.Red,
                    KeyTime = redTime
                };
                DiscreteColorKeyFrame RedToOrginalColor = new DiscreteColorKeyFrame() {
                    Value = (target.Fill as SolidColorBrush).Color,
                    KeyTime = orginalTime
                };

                colorAnimationUsingKeyFrames.KeyFrames.Add(OrginalColorToRed);
                colorAnimationUsingKeyFrames.KeyFrames.Add(RedToOrginalColor);
                BlinkedRectangles[target.Name].storyboard.Children.Add(colorAnimationUsingKeyFrames);
                Storyboard.SetTarget(colorAnimationUsingKeyFrames, target);
                Storyboard.SetTargetName(colorAnimationUsingKeyFrames, target.Name);
                Storyboard.SetTargetProperty(colorAnimationUsingKeyFrames, "(Rectangle.Fill).(SolidColorBrush.Color)");

                BlinkedRectangles[target.Name].storyboard.Begin();
            }
        }

        public static void StopBlink(Rectangle target) {
            if (BlinkedRectangles[target.Name].storyboard.GetCurrentState() == ClockState.Active) {
                BlinkedRectangles[target.Name].storyboard.Stop();
            }
        }
    }
}
