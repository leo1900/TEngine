using System.Collections.Generic;

namespace TEngine
{
    /// <summary>
    /// 存档系统使用示例。
    /// 此文件仅作为参考，实际使用时可以删除。
    /// </summary>
    public static class StorageModuleExample
    {
        /// <summary>
        /// 初始化存档系统的示例。
        /// </summary>
        public static void InitializeStorageExample()
        {
            // 1. 创建存档模块实例
            StorageModule storageModule = new StorageModule();
            
            // 2. 注册到模块系统
            ModuleSystem.RegisterModule<IStorageModule>(storageModule);
            
            // 3. 创建要管理的存档数据列表
            List<StorageBase> storages = new List<StorageBase>
            {
                new StorageCommon()  // 添加你的存档数据类
            };
            
            // 4. 初始化存档系统
            storageModule.InitializeStorage(storages, isFullInitialized: true);
        }

        /// <summary>
        /// 使用存档数据的示例。
        /// </summary>
        public static void UseStorageExample()
        {
            // 获取存档模块
            IStorageModule storageModule = ModuleSystem.GetModule<IStorageModule>();
            
            // 获取存档数据
            StorageCommon common = storageModule.Get<StorageCommon>();
            
            // 修改数据（会自动标记为需要保存）
            common.PlayerName = "Player1";
            common.Level = 10;
            common.Coins = 1000;
            
            // 强制保存到磁盘
            storageModule.ForceSaveToDisk();
        }

        /// <summary>
        /// 创建自定义存档数据类的示例。
        /// </summary>
        public class CustomStorage : StorageBase
        {
            private int _customValue;

            public int CustomValue
            {
                get => _customValue;
                set
                {
                    if (_customValue != value)
                    {
                        _customValue = value;
                        MarkDirty();  // 标记数据已更改
                    }
                }
            }
        }
    }
}

