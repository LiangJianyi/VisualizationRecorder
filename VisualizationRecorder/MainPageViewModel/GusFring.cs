using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml.Media;

namespace VisualizationRecorder {
    using TioSalamanca = List<IGrouping<BigInteger, StatistTotalByDateTime>>;

    /// <summary>
    /// GusFring 类用于解析 TioSalamanca 类型的统计数据，为方块生成着色字典，
    /// 用户可以直接将方块名称传递给 GusFring 的索引器然后返回相应的颜色。
    /// </summary>
    class GusFring {
        /// <summary>
        /// 方块着色字典
        /// </summary>
        private readonly Dictionary<string, SolidColorBrush> _gus = new Dictionary<string, SolidColorBrush>();
        /// <summary>
        /// 颜色字典
        /// </summary>
        private readonly IDictionary<int, SolidColorBrush> _colorDic;
#if DEBUG
        /// <summary>
        /// 保存方块名称与相关统计数据元组的键值对，元组存储方块对应的事件统计数据，分别含有级别、事件频率、方块颜色
        /// </summary>
        private readonly Dictionary<string, (int Level, BigInteger Total, SolidColorBrush Color)> _debug_gus = new Dictionary<string, (int Level, BigInteger Total, SolidColorBrush Color)>();
#endif

        public GusFring(TioSalamanca[] tio, IDictionary<int, SolidColorBrush> colorDic) {
            _colorDic = colorDic;
            for (int level = 0; level < tio.Length; level++) {
                foreach (IGrouping<BigInteger, StatistTotalByDateTime> group in tio[level]) {
                    StatistTotalByDateTime[] res = group.ToArray();
                    foreach (var item in res) {
#if DEBUG==false
                        _gus.Add(item.DateTime.ToShortDateString(), colorDic[level + 1]); 
#endif
#if DEBUG
                        _debug_gus.Add(
                            item.DateTime.ToShortDateString(),
                            (Level: level + 1, Total: item.Total, Color: colorDic[level + 1])
                        );
#endif
                    }
                }
            }
        }

        /*
         * DEBUG 模式和 RELEASE 模式分别编译两个不同的索引器
         */
#if DEBUG==false
        public SolidColorBrush this[string rectName] {
            get {
                try {
                    return this._gus[rectName];
                }
                catch (KeyNotFoundException) {
                    return _colorDic[0];
                }
            }
        }
#endif

#if DEBUG
        public (int Level, BigInteger Total, SolidColorBrush Color) this[string rectName] {
            get {
                try {
                    return this._debug_gus[rectName];
                }
                catch (KeyNotFoundException) {
                    return (Level: 0, Total: 0, Color: _colorDic[0]);
                }
            }
        } 
#endif
    }
}
