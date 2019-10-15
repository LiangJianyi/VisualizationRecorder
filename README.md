# VisualizationRecorder项目简介
该项目是一个可对事件的发生频率和发生时间进行可视化的UWP应用程序，其使用 GitHub 小绿点的形式表示事件的发生频率和发生时间。MasturbationRecorder 读取一个纯文本中的事件日期和事件频率，然后将其渲染成不同颜色的小方块。每个小方块对应一个日期，方块面板中的每一列对应一年中的一周，每一列中的每个方块对应一周中的某一天，最上面的代表周日，最下面的代表周六。小方块的颜色深浅代表事件的频率。颜色越深事件的频率越高，灰色代表当天没有事件发生。

# 开发环境  ![GitHub](https://img.shields.io/badge/csharp-7.3-blue.svg)
+ IDE： Visual Studio 2019
+ 语言：C# 7.3
+ 额外的依赖库：
  + [System.Numerics][1]
  + [BigIntegerExtension][2]
  + [SuckerInterpreter][3]
  
  [1]: https://docs.microsoft.com/en-us/dotnet/api/system.numerics?view=netframework-4.8
  [2]: https://github.com/LiangJianyi/SundryUtilty/tree/master/.NET%20Standard/BigIntegerExtension
  [3]: https://github.com/LiangJianyi/SundryUtilty/tree/master/.NET%20Standard/Sucker
  
# 项目结构
项目的解决方案文件名称为 VisualizationRecorder.sln，该解决方案下包含三个项目：
+ BigIntegerExtension
+ SuckerInterpreter
+ VisualizationRecorder(Universal Windows)

VisualizationRecorder(Universal Windows)是主体项目。BigIntegerExtension 和 SuckerInterpreter 需要手动添加该项目到解决方案下（VisualizationRecorder.sln），VisualizationRecorderTest(Universal Windows) 是主体项目的单元测试项目。

VisualizationRecorder的大致运行流程如下：![ExampleVedio](https://github.com/LiangJianyi/liangjianyi.github.io/blob/master/vedio/VisualizationRecorderExampleVedio.mp4)

当用户点击文件选择器并选择指定的文本文件后（只能打开 .txt .mast 后缀的文件），提取文本内容，将文本的每一行切割成 string 对象装载为一个 IEnumerable<string>，然后将该文本序列传递给 MainPageViewModel.LinesConvertToStatistTotalByDateTimes 方法，MainPageViewModel.LinesConvertToStatistTotalByDateTimes 方法把每行 string 转换为一个 StatistTotalByDateTime 对象，最后装载成 LinkedList<StatistTotalByDateTime> 传递给 MainPageViewModel.GroupDateTimesByTotal 方法。MainPageViewModel.GroupDateTimesByTotal 方法对链表按 StatistTotalByDateTime.Total 分组，产生一个IGrouping序列，存放在局部变量groupingForTotal中，最后对分组结果进行分级，传递给 DrawRectangleColor 方法根据每个日期所在分组的级别对每个方块进行着色。
  
这个IGrouping序列的长度决定了接下来的分级行为，如果总数大于5，那么数组entries的长度总是5，因为纪录器最多只能分5级，颜色字典也只能产生6个色阶，五种绿色加一种灰色，下面的***分级示例一***描述了 groups>5 时分级表的结构，***分级示例二***描述了 groups<=5 时分级表的结构。
  
## 分级示例一：

```
May 27 2019 x16
May 27 2019
Apr 28 2019 x2610
May 29 2019 x1000
May 29 2019 x2
May 31 2019 x5
Jun 12 2019 x5
Jun 15 2019 x7000
Jun 17 2019 x8
Jun 20 2018 x9
Jun 24 2018 x15
Jun 26 2018 x20
Jul 04 2018 x25
Jul 05 2018 x30
Jul 10 2018 x30
Jul 11 2018 x30
Jul 13 2018 x30
Jul 16 2018 x1000
Jul 17 2018 x1001
Jul 18 2018 x1002
Jul 19 2018 x2000
Jul 22 2018 x2002
Jul 24 2018 x2010
Jul 29 2018 x2017
Jul 31 2018 x2517
Aug 21 2018 x2618
Aug 29 2018 x2719
Sep 14 2018 x2741
Sep 27 2018 x2517
Sep 30 2018 x2805
Oct 15 2018 x2719
Jan 01 2019
Jan 02 2019
Feb 02 2019
Mar 02 2019
Apr 02 2019 x16
```

上面的文本将转换为如下结构的分组表：

![List<IGrouping<BigInteger, StatistTotalByDateTime>>[]](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E5%88%86%E7%BA%A7%E8%A1%A8%E7%BB%93%E6%9E%84.jpg)


## 分级示例二：

```
May 27 2018 x20
May 29 2018
May 31 2018
Jul 05 2018
Jul 10 2018 x2
Jul 19 2018
Jul 22 2018 x2
Jul 24 2018
Aug 16 2018
Aug 21 2018 x2
Aug 22 2018
Aug 25 2018
Aug 29 2018 x2
Sep 14 2018 x2
Sep 27 2018 x2
Oct 5 2018
```

上面的文本将转换为如下结构的分组表：

![List<IGrouping<BigInteger, StatistTotalByDateTime>>[]](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E5%88%86%E7%BA%A7%E8%A1%A8%E7%BB%93%E6%9E%842.jpg)


最外层的灰色边框代表一个数组，类型为List<IGrouping<BigInteger, StatistTotalByDateTime>>[]，其第一列的数组是每个元素的索引，索引越大，记录级别越大，第二列为每个元素对应的List<IGrouping<BigInteger, StatistTotalByDateTime>>，用红框表示List的范围，索引越大的元素，其保存在List中的每个group的Key也越大。Key代表当前分组的事件频率，每个列表的元素按Key值升序排列。每个Key对应一张由StatistTotalByDateTime组成的表，同一个Key中，每个元素的StatistTotalByDateTime.Total相同。

![指示元素](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E6%8C%87%E7%A4%BA%E5%85%83%E7%B4%A0.jpg)

DrawRectangleColor 方法的着色原理很简单，传递List<IGrouping<BigInteger, StatistTotalByDateTime>>[]的长度给ClassifyColorByLevelScore方法获得色阶字典，类型为IDictionary<int, SolidColorBrush>，然后根据每个条目所在的索引分配指定颜色。

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
