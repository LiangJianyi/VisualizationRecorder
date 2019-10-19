# 可视化事件记录器项目简介
该项目是一个可对事件的发生频率和发生时间进行可视化的UWP应用程序，其使用 GitHub 小绿点的形式表示事件的发生频率和发生时间。可视化事件记录器读取一个纯文本中的事件日期和事件频率，然后将其渲染成不同颜色的小方块。每个小方块对应一个日期，方块面板中的每一列对应一年中的一周，每一列中的每个方块对应一周中的某一天，最上面的代表周日，最下面的代表周六。小方块的颜色深浅代表事件的频率。颜色越深事件的频率越高，灰色代表当天没有事件发生。

可视化事件记录器已登录 ![Microsoft Store][https://www.microsoft.com/zh-cn/p/%e5%8f%af%e8%a7%86%e5%8c%96%e4%ba%8b%e4%bb%b6%e8%ae%b0%e5%bd%95%e5%99%a8/9mxnff1j4q0k?activetab=pivot:overviewtab]。

# 开发环境  [GitHub](https://img.shields.io/badge/csharp-7.3-blue.svg)
+ IDE： Visual Studio 2019
+ 语言：C# 7.3
+ 额外的依赖库：
  + [System.Numerics][1]
  + [BigIntegerExtension][2]
  + [SuckerInterpreter][3]
  
  
# 项目结构
项目的解决方案文件名称为 VisualizationRecorder.sln，该解决方案下包含三个项目：
+ BigIntegerExtension
+ SuckerInterpreter
+ VisualizationRecorder(Universal Windows)

VisualizationRecorder(Universal Windows)是主体项目。BigIntegerExtension 和 SuckerInterpreter 需要手动添加该项目到解决方案下（VisualizationRecorder.sln），VisualizationRecorderTest(Universal Windows) 是主体项目的单元测试项目。

VisualizationRecorder的大致运行流程如下：[![Watch the video](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/vedio/VisualizationRecorderSlashVedioFrame.png)](https://youtu.be/VsvTEOE04bs)

(点击上面的图片播放YouTube教学视频)

# 工作原理

可视化事件纪录器由三个工作单元组成：
+ 语法分析
+ 方块矩阵生成
+ 数据呈现

## 语法分析
当用户点击文件选择器并选择指定的文本文件后（只能打开 .txt .mast 后缀的文件），提取文本内容，通过 EncodingToStatistTotalByDateTimeModel 方法将其解释为 StatistTotalByDateTimeModel 对象，最后通过 Render 方法将 StatistTotalByDateTimeModel 对象呈现为带有深浅不一的绿色小方块的方块矩阵。 
.txt 和 .mast 文本分别有各自的语法和解释器，参见：[txt 语法][4]和[SuckerML 标记语言][5]。

## 方块矩阵生成
![Rectangle Canvas](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/image/RectanglesCanvas.png)
上图的灰色边框范围表示一个方块面板，它位于 MainPage.xaml 页面，名为 CurrentRectanglesCanvas，包含在一个 Panel 容器中，它的页面初始代码为：
```
<StackPanel x:Name="StackCanvas">
<Canvas x:Name="CurrentRectanglesCanvas" Width="300" Height="300"></Canvas>
</StackPanel>
```
里面的小方块由 MainPage.RectangleLayout() 方法生成并填充到方块面板中。填充小方块之前，方块面板的大小会重新调整，它的长和宽由下面两行代码决定：
```
rectanglesCanvas.Width = totalWeek * COLUMN_DISTANCE + LEFT_SPACE + RIGHT_SPACE + totalWeek * RECT_WIDHT + RECT_WIDHT;
rectanglesCanvas.Height = ROW_DISTANCE * 6 + BOTTOM_SPACE + MONTH_TITLE_SPACE + 7 * RECT_HEIGHT;
```


## 数据呈现

方块的着色工作交由 DrawRectangleColor 方法执行，用户输入的文本交由解释器生成 List<IGrouping<BigInteger, StatistTotalByDateTime>>[]，该数组的长度给ClassifyColorByLevelScore方法获得色阶字典，类型为IDictionary<int, SolidColorBrush>，然后根据每个条目所在的索引分配指定颜色。

方块有4种状态：
+ 方块对应的日期没有事件发生且用户没有点击它进行修改，用灰色表示：
+ 方块对应的日期有事件发生且用户没有点击它进行修改，根据事件发生的频率高低用五种颜色表示：
![level color](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/image/level%20color.png)
+ 方块对应的日期没有事件发生且用户点击它进行修改，闪烁为红色：
![story1](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/image/story1.gif)
+ 方块对应的日期有事件发生且用户点击它进行修改，闪烁为红色： 
![story2](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/image/story2.gif) 
![story3](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/image/story3.gif)

  
注意：整个项目使用整数的地方几乎都采用 BigInteger，这是为了对付几亿数据量设计的，用来做极限测试。

# 开源协议  ![GitHub](https://img.shields.io/github/license/Liangjianyi/MasturbationRecorder.svg?style=popout)
MasturbationRecorder 使用 MIT 协议。本项目的代码可由任何个人和组织随意使用。

## 协议使用条款

Copyright (C) 2019 Janyee Liang

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 


[1]: https://docs.microsoft.com/en-us/dotnet/api/system.numerics?view=netframework-4.8
[2]: https://github.com/LiangJianyi/SundryUtilty/tree/master/.NET%20Standard/BigIntegerExtension
[3]: https://github.com/LiangJianyi/SundryUtilty/tree/master/.NET%20Standard/Sucker
[4]: https://github.com/LiangJianyi/VisualizationRecorder/blob/master/txt%20syntax.md
[5]: https://github.com/LiangJianyi/VisualizationRecorder/blob/master/SuckerML%20syntax.md