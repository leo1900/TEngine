using System;
using System.Collections.Generic;

namespace TEngine
{
    /// <summary>
    /// 存档管理模块。
    /// </summary>
    public class StorageModule : Module, IStorageModule, IUpdateModule
    {
        private StorageManager _storageManager;
        private bool _isInitialized;

        /// <summary>
        /// 获取模块优先级。
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 存档系统是否已初始化。
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 初始化模块。
        /// </summary>
        public override void OnInit()
        {
            _storageManager = new StorageManager();
        }

        /// <summary>
        /// 关闭模块。
        /// </summary>
        public override void Shutdown()
        {
            if (_storageManager != null)
            {
                _storageManager.SaveToLocal();
            }
            _storageManager = null;
            _isInitialized = false;
        }

        /// <summary>
        /// 更新模块。
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_storageManager != null && _isInitialized)
            {
                _storageManager.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 初始化存档系统。
        /// </summary>
        public void InitializeStorage(List<StorageBase> storages, bool isFullInitialized = true)
        {
            if (_isInitialized)
            {
                Log.Error("Storage system has already been initialized!");
                return;
            }

            if (storages == null || storages.Count == 0)
            {
                Log.Error("Storage list is null or empty!");
                return;
            }

            // 设置每个StorageBase的StorageModule引用
            foreach (StorageBase storage in storages)
            {
                storage.StorageModule = this;
            }

            _storageManager.Initialize(storages, isFullInitialized);
            _isInitialized = isFullInitialized;

            if (_isInitialized)
            {
                Log.Info("Storage system initialized successfully.");
            }
        }

        /// <summary>
        /// 获取指定类型的存档数据。
        /// </summary>
        public T Get<T>() where T : StorageBase
        {
            if (!_isInitialized)
            {
                throw new GameFrameworkException("Storage system has not been initialized!");
            }

            return _storageManager.Get<T>();
        }

        /// <summary>
        /// 强制保存到本地。
        /// </summary>
        public void ForceSaveToDisk()
        {
            if (!_isInitialized)
            {
                return;
            }

            _storageManager.ForceSaveToDisk();
        }

        /// <summary>
        /// 添加存档更新任务。
        /// </summary>
        public void AddStorageUpdateTask(Action updateTask)
        {
            if (!_isInitialized)
            {
                return;
            }

            _storageManager.AddStorageUpdateTask(updateTask);
        }

        /// <summary>
        /// 执行一次操作。
        /// </summary>
        public bool RunOnce(Action callback)
        {
            if (!_isInitialized)
            {
                return false;
            }

            return _storageManager.RunOnce(callback);
        }

        /// <summary>
        /// 清空所有存档数据。
        /// </summary>
        public void Clear()
        {
            if (!_isInitialized)
            {
                return;
            }

            _storageManager.Clear();
        }
    }
}

