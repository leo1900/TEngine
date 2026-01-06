namespace TEngine
{
    /// <summary>
    /// 存档模块接口。
    /// </summary>
    public interface IStorageModule
    {
        /// <summary>
        /// 存档系统是否已初始化。
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 获取指定类型的存档数据。
        /// </summary>
        /// <typeparam name="T">存档数据类型。</typeparam>
        /// <returns>存档数据实例。</returns>
        T Get<T>() where T : StorageBase;

        /// <summary>
        /// 初始化存档系统。
        /// </summary>
        /// <param name="storages">要注册的存档数据列表。</param>
        /// <param name="isFullInitialized">是否完全初始化。</param>
        void InitializeStorage(System.Collections.Generic.List<StorageBase> storages, bool isFullInitialized = true);

        /// <summary>
        /// 强制保存到本地。
        /// </summary>
        void ForceSaveToDisk();

        /// <summary>
        /// 添加存档更新任务。
        /// </summary>
        /// <param name="updateTask">更新任务。</param>
        void AddStorageUpdateTask(System.Action updateTask);

        /// <summary>
        /// 执行一次操作（用于首次运行时的初始化）。
        /// </summary>
        /// <param name="callback">回调函数。</param>
        /// <returns>是否执行了回调（首次运行返回true）。</returns>
        bool RunOnce(System.Action callback);

        /// <summary>
        /// 清空所有存档数据。
        /// </summary>
        void Clear();
    }
}

