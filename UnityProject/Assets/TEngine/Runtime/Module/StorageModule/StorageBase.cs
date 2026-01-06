using System;

namespace TEngine
{
    /// <summary>
    /// 存档数据基类。
    /// 所有需要保存的游戏数据都应该继承此类。
    /// </summary>
    [Serializable]
    public abstract class StorageBase
    {
        /// <summary>
        /// 获取存档管理器引用（内部使用）。
        /// </summary>
        internal IStorageModule StorageModule { get; set; }

        /// <summary>
        /// 标记数据已更改，需要保存。
        /// </summary>
        protected void MarkDirty()
        {
            IStorageModule storageModule = StorageModule ?? ModuleSystem.GetModule<IStorageModule>();
            if (storageModule != null)
            {
                storageModule.ForceSaveToDisk();
            }
        }
    }
}

