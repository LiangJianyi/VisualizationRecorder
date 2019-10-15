# txt 语法
txt 文本记录由每一行条目（Entry）组成，每一行 Entry 由四部分组成：月份简写（Month）、月份天数（Day）、年份（Year）和事件频率（Total）组成，每部分由空格分离，次序不能颠倒。
txt 语法的 BNF 规范定义如下：

<Entry> ::= <Month> <Day> <Year> [<Total>]
<Month> ::= Jan | Feb | Mar | Apr | May | Jun | Jul | Aug | Sep | Oct | Nov | Dec 
<Day> ::= <DigitsStartingWithTheNumberOne> | <DigitsStartingWithTheNumberTwo> | <DigitsStartingWithTheNumberThree> | <NonZeroDigit>
<Year> ::= <NonZeroDigit> {<Digit>}
<Total> ::= x <NonZeroDigit> {<Digit>}
<DigitsStartingWithTheNumberOne> ::= 1 [<Digit>]
<DigitsStartingWithTheNumberTwo> ::= 2 [<Digit>]
<DigitsStartingWithTheNumberThree> ::= 3 [1|0]
<NonZeroDigit> ::= 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
<Digit> ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9

BNF 规范参考： http://www.cs.utsa.edu/~wagner/CS3723/grammar/examples2.html

## 解释原理

将文本的每一行切割成 string 对象装载为一个 IEnumerable<string>，然后将该文本序列传递给 MainPageViewModel.LinesConvertToStatistTotalByDateTimes 方法，MainPageViewModel.LinesConvertToStatistTotalByDateTimes 方法把每行 string 转换为一个 StatistTotalByDateTime 对象，最后装载成 LinkedList<StatistTotalByDateTime> 传递给 MainPageViewModel.GroupDateTimesByTotal 方法。MainPageViewModel.GroupDateTimesByTotal 方法对链表按 StatistTotalByDateTime.Total 分组，产生一个IGrouping序列，存放在局部变量groupingForTotal中，最后对分组结果进行分级，传递给 DrawRectangleColor 方法根据每个日期所在分组的级别对每个方块进行着色。
  
这个IGrouping序列的长度决定了接下来的分级行为，如果总数大于5，那么数组entries的长度总是5，因为纪录器最多只能分5级，颜色字典也只能产生6个色阶，五种绿色加一种灰色，下面的***分级示例一***描述了 groups>5 时分级表的结构，***分级示例二***描述了 groups<=5 时分级表的结构。
  
## txt 文本语法示例一：

``
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
``

上面的文本将转换为如下结构的分组表：

![List<IGrouping<BigInteger, StatistTotalByDateTime>>[]](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E5%88%86%E7%BA%A7%E8%A1%A8%E7%BB%93%E6%9E%84.jpg)


## txt 文本语法示例二：

``
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
``

StatistTotalByDateTimeModel 通过 GroupDateTimesByTotal 方法转换为如下结构的分组表：

![List<IGrouping<BigInteger, StatistTotalByDateTime>>[]](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E5%88%86%E7%BA%A7%E8%A1%A8%E7%BB%93%E6%9E%842.jpg)


最外层的灰色边框代表一个数组，类型为List<IGrouping<BigInteger, StatistTotalByDateTime>>[]，其第一列的数组是每个元素的索引，索引越大，记录级别越大，第二列为每个元素对应的List<IGrouping<BigInteger, StatistTotalByDateTime>>，用红框表示List的范围，索引越大的元素，其保存在List中的每个group的Key也越大。Key代表当前分组的事件频率，每个列表的元素按Key值升序排列。每个Key对应一张由StatistTotalByDateTime组成的表，同一个Key中，每个元素的StatistTotalByDateTime.Total相同。

![指示元素](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E6%8C%87%E7%A4%BA%E5%85%83%E7%B4%A0.jpg)