using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6
{
    public abstract class ApiOption
    {
        /// <summary>
        /// 起始语言 (翻译前文本的语言)
        /// </summary>
        public abstract Language Lan_Start { get; protected set; }
        /// <summary>
        /// 用于进行翻译的中间语言列表 (可以不含起始和结尾)
        /// </summary>
        public abstract List<Language> Lan_List { get; protected set; }
        /// <summary>
        /// 结束语言 (翻译全部完成后应为本语言)
        /// </summary>
        public abstract Language Lan_End { get; protected set; }
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public abstract int ExecuteTimes { get; protected set; }
        /// <summary>
        /// 调用 API 的等待间隔 ( 防止调用速度过快导致IP冷却 )
        /// </summary>
        public abstract int Interval { get; protected set; }
        /// <summary>
        /// 是否使用随机语言
        /// </summary>
        public abstract bool UseRandom { get; protected set; }
        /// <summary>
        /// 所属的API (用于获取语言列表等)
        /// </summary>
        public abstract API Api { get; init; }
        /// <summary>
        /// 设置文件的保存路径
        /// </summary>
        public abstract string FilePath { get; init; }
        /// <summary>
        /// 修改设置
        /// </summary>
        public abstract void Modify();
        /// <summary>
        /// 输出 API 的设置信息到控制台
        /// </summary>
        public abstract void Print();
        /// <summary>
        /// 从文件中加载设置
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// 保存设置到文件
        /// </summary>
        public abstract void Save();
    }
}
