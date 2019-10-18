# SuckerML 语法
SuckerML 是一种采用 S 表达式的标记语言。S 表达式采用一对圆括号作为其记法形式，括号内的每一项可以是某种类型的对象映射，也可以是另一个 S 表达式。SuckerML 的语义范畴只采取了 S 表达式的一个子集，去除了部分抽象能力，它只是 [StatistTotalByDateTime][1] 这种类型的对象的一种形式化表示。

以下是 SuckerML 的 BNF 规范：
```
<YearList> ::= {<YearNode>}
<YearNode> ::= (Year <Year> <MonthList>)
<MonthList> ::= {<MonthNode>}
<MonthNode> ::= (Month <Month> <DayList>)
<DayList> ::= {<DayNode>}
<DayNode> ::= (day-total <Day> <Total>)
<Month> ::= Jan | Feb | Mar | Apr | May | Jun | Jul | Aug | Sep | Oct | Nov | Dec 
<Day> ::= <DigitsStartingWithTheNumberOne> | <DigitsStartingWithTheNumberTwo> | <DigitsStartingWithTheNumberThree> | <NonZeroDigit>
<Year> ::= <NonZeroDigit> {<Digit>}
<Total> ::= x <NonZeroDigit> {<Digit>}
<DigitsStartingWithTheNumberOne> ::= 1 [<Digit>]
<DigitsStartingWithTheNumberTwo> ::= 2 [<Digit>]
<DigitsStartingWithTheNumberThree> ::= 3 [1|0]
<NonZeroDigit> ::= 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
<Digit> ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
```

[1]: https://github.com/LiangJianyi/VisualizationRecorder/blob/master/VisualizationRecorder/Model/StatistTotalByDateTime.cs