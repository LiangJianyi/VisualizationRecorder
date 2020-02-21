using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Janyee.Utilty;

namespace VisualizationRecorder {
    using TioSalamanca = List<IGrouping<BigInteger, StatistTotalByDateTime>>;

    /// <summary>
    /// 记录事件频率的对象，它是事件纪录器的核心对象类型，每一个StatistTotalByDateTime对象代表一条事件记录，
    /// 事件记录包含两个属性，日期（DateTime）和发生次数（Total）
    /// </summary>
    class StatistTotalByDateTime : IComparable<StatistTotalByDateTime> {
        public DateTime DateTime;
        public BigInteger Total;
        public int CompareTo(StatistTotalByDateTime other) {
            if (this.Total < other.Total) {
                return -1;
            }
            else if (this.Total == other.Total) {
                return 0;
            }
            else {
                return 1;
            }
        }
        public override string ToString() {
            string month = null;
            switch (DateTime.Month) {
                case 1:
                    month = "Jan";
                    break;
                case 2:
                    month = "Feb";
                    break;
                case 3:
                    month = "Mar";
                    break;
                case 4:
                    month = "Apr";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "Jun";
                    break;
                case 7:
                    month = "Jul";
                    break;
                case 8:
                    month = "Aug";
                    break;
                case 9:
                    month = "Sep";
                    break;
                case 10:
                    month = "Oct";
                    break;
                case 11:
                    month = "Nov";
                    break;
                case 12:
                    month = "Dec";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Month out of range.", $"Month = {DateTime.Month}");
            }
            return $"{month} {DateTime.Day} {DateTime.Year} x{Total}";
        }
        /// <summary>
        /// 比较两个 StatistTotalByDateTime 对象的相等性
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equivalent(StatistTotalByDateTime other) => this.DateTime.Equals(other.DateTime) && this.Total.Equals(other.Total);
        /// <summary>
        /// 如果当前 StatistTotalByDateTime 的发生时间比 other 要早，返回 true，否则返回 false
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EarlierThan(StatistTotalByDateTime other) => this.DateTime < other.DateTime;
        /// <summary>
        /// 如果当前 StatistTotalByDateTime 的发生时间比 other 要晚，返回 true，否则返回 false
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool LaterThan(StatistTotalByDateTime other) => this.DateTime > other.DateTime;
        public static bool operator >(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) == 1;
        public static bool operator <(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) == -1;
        public static bool operator >=(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) >= 0;
        public static bool operator <=(StatistTotalByDateTime left, StatistTotalByDateTime right) => left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// StatistTotalByDateTime 模型类，用于存储 StatistTotalByDateTime 对象，并对这些对象进行增删改查
    /// </summary>
    class StatistTotalByDateTimeModel {
        private readonly SortedList<DateTime, StatistTotalByDateTime> _entries = new SortedList<DateTime, StatistTotalByDateTime>();
        public SortedList<DateTime, StatistTotalByDateTime> Entries => _entries;

        /// <summary>
        /// 该构造器接收一个字符串序列，把它转换成StatistTotalByDateTime链表，同时接收一个 DateMode 指示日期字符串的分割方式
        /// </summary>
        /// <param name="lines">文本序列</param>
        /// <param name="dateMode">指示日期字符串的分割方式</param>
        public StatistTotalByDateTimeModel(IEnumerable<string> lines, DateMode dateMode) {
            foreach (var line in lines) {
                if (line != "" && line != "\r") {   // 忽略空行
                    StatistTotalByDateTime statist = DatetimeParser.ParseExpressToStatistTotalByDateTime(line, dateMode);
                    this.Add(statist);
                }
            }
        }
        /// <summary>
        /// 该构造器接收一个字符串序列，把它转换成StatistTotalByDateTime链表并显式指定每个StatistTotalByDateTime的Total，同时接收一个 DateMode 指示日期字符串的分割方式
        /// </summary>
        /// <param name="lines">文本序列</param>
        /// <param name="total">指定每个StatistTotalByDateTime的Tota</param>
        /// <param name="dateMode">指示日期字符串的分割方式</param>
        public StatistTotalByDateTimeModel(IEnumerable<string> lines, BigInteger total, DateMode dateMode) {
            foreach (var line in lines) {
                if (line != "" && line != "\r") {   // 忽略空行
                    StatistTotalByDateTime statist = DatetimeParser.ParseExpressToStatistTotalByDateTime(line, dateMode);
                    statist.Total = total;
                    this.Add(statist);
                }
            }
        }

        /// <summary>
        /// 该构造器接收一个 StatistTotalByDateTime 序列
        /// </summary>
        /// <param name="statistTotalByDateTimes"></param>
        public StatistTotalByDateTimeModel(IEnumerable<StatistTotalByDateTime> statistTotalByDateTimes) {
            foreach (var statist in statistTotalByDateTimes) {
                this.Add(statist);
            }
        }

        /// <summary>
        /// 添加 StatistTotalByDateTime，如果存在相同日期的事件，则累加该事件的 Total
        /// </summary>
        /// <param name="statist"></param>
        private void Add(StatistTotalByDateTime statist) {
            try {
                this._entries.Add(statist.DateTime, statist);
            }
            catch (ArgumentException) {
                this._entries[statist.DateTime].Total += statist.Total;
            }
        }
        /// <summary>
        /// 根据事件发生的频率进行分组，并以事件频率作为偏序关系进行升序排序，
        /// 产生一个与 IGrouping<BigInteger, StatistTotalByDateTime> 为单元的有序列表
        /// </summary>
        /// <returns></returns>
        public TioSalamanca[] GroupDateTimesByTotal() {
            IOrderedEnumerable<IGrouping<BigInteger, StatistTotalByDateTime>> groupingForTotal = from e in _entries
                                                                                                 group e.Value by e.Value.Total into newgroup
                                                                                                 orderby newgroup.Key
                                                                                                 select newgroup;

#if DEBUG
            Debug.WriteLine("Executing GroupDateTimesByDiff...");
            foreach (var item in groupingForTotal) {
                Debug.WriteLine(item.Key);
                foreach (var subitem in item) {
                    Debug.WriteLine($"  {subitem}");
                }
            }
#endif
            // 一个级别有若干个Key；一个Key有若干条记录
            // levelByTotal 指示每个级别有多少个 Key（groupingForTotal根据Total分组出来的Key）
            TioSalamanca levels = null;

            // groups 代表 dateTimes 根据每个元素的 Total 分组之后 groups（item.Key） 的总数
            BigInteger groups = groupingForTotal.LongCount();

            if (groups > 5) {
                // keysForEachLevel 表示每个级别应包含多少个 item.Key
                BigInteger keysForEachLevel = groups / 5;
                BigInteger remain = groups % 5;

#if DEBUG
                Debug.WriteLine($"groups: {groups}");
                Debug.WriteLine($"keysForEachLevel: {keysForEachLevel}");
                Debug.WriteLine($"remain: {remain}");
#endif

                TioSalamanca[] entries = new TioSalamanca[5];
                BigInteger keyIncre = 0;
                // entriesIncre 没有必要使用 BigInteger，因为不管有多少个 Key，分成多少个 group，
                // entries 的长度永远为 5，因为纪录器最多只能分五级
                int entriesIncre = 0;
                foreach (var item in groupingForTotal) {
                    keyIncre += 1;
                    if (keyIncre == keysForEachLevel) {
                        if (entriesIncre < entries.Length - 1) {
                            keyIncre = 0;
                            levels.Add(item);
                            entries[entriesIncre] = levels;
                            entriesIncre += 1;
                            levels = null;
                        }
                        else if (entriesIncre == entries.Length - 1) {
                            if (remain != 0) {
                                levels.Add(item);
                            }
                            // 这么做迫使下次循环 keyIncre 仍然为 5，
                            // 这样能再次进入 keyIncre == keysForEachLevel 语句块；
                            keyIncre -= 1;
                        }
                    }
                    else if (levels == null) {
                        levels = new TioSalamanca { item };
                    }
                    else {
                        levels.Add(item);
                    }
                }
                entries[entriesIncre] = levels;
                return entries;
            }
            else if (groups <= 5 && groups > 0) {
                TioSalamanca[] entries = new TioSalamanca[groups.BigIntegerToInt32()];
                TioSalamanca temp = groupingForTotal.ToList();
                for (int i = 0; i < entries.Length; i++) {
                    entries[i] = new TioSalamanca(1) {
                        temp[i]
                    };
                }
                return entries;
            }
            else {
                throw new InvalidOperationException($"Unkown error. Groups is {groups}");
            }
        }
        /// <summary>
        /// 添加一个条目
        /// </summary>
        /// <param name="rectName">接收一个字符串表达式，格式为“mm dd yyyy x{Total}”或“mm/dd/yyyy”</param>
        public void AddEntry(string rectName) {
            this.AddEntry(DatetimeParser.ParseExpressToStatistTotalByDateTime(rectName, DateMode.DateWithSlash));
        }
        /// <summary>
        /// 添加一个条目
        /// </summary>
        /// <param name="statistTotalByDateTime"></param>
        public void AddEntry(StatistTotalByDateTime statistTotalByDateTime) {
            this.Add(statistTotalByDateTime);
        }
        /// <summary>
        /// 转换为 StatistTotalByDateTime.ToString() 数组
        /// </summary>
        /// <returns></returns>
        public string[] ToStringArray() => (from statistTotalByDateTime in this._entries select statistTotalByDateTime.Value.ToString()).ToArray();
        /// <summary>
        /// 转换为 StatistTotalByDateTime 数组
        /// </summary>
        /// <returns></returns>
        public StatistTotalByDateTime[] ToStatistTotalByDateTimeArray()=> (from statistTotalByDateTime in this._entries select statistTotalByDateTime.Value).ToArray();
    }
}
