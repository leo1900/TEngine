using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TEngine
{
    /// <summary>
    /// 存档管理器，负责管理所有存档数据的保存和加载。
    /// </summary>
    internal class StorageManager
    {
        private const string StorageDataKey = "TEngine_StorageData";
        private const string StorageVersionKey = "TEngine_StorageVersion";
        private const string RunOnceKey = "TEngine_RunOnce";

        private Dictionary<string, StorageBase> _storageMap;
        private Dictionary<Type, string> _typeToName;
        private Queue<Action> _storageUpdateTask;
        private ulong _localVersion;
        private ulong _lastSavedVersion;
        private float _elapseTimeSinceLastSave;
        private bool _needForceSave;

        /// <summary>
        /// 获取本地版本号。
        /// </summary>
        public ulong LocalVersion => _localVersion;

        /// <summary>
        /// 保存到磁盘的间隔时间（秒）。
        /// </summary>
        public float SaveToDiskInterval { get; set; } = 1f;

        /// <summary>
        /// 初始化存档管理器。
        /// </summary>
        public void Initialize(List<StorageBase> storages, bool isFullInitialized)
        {
            _storageMap = new Dictionary<string, StorageBase>();
            _typeToName = new Dictionary<Type, string>();
            _storageUpdateTask = new Queue<Action>();

            foreach (StorageBase storage in storages)
            {
                string name = storage.GetType().Name;
                _storageMap[name] = storage;
            }

            LoadLocalVersion();
            LoadLocalData();
        }

        /// <summary>
        /// 获取指定类型的存档数据。
        /// </summary>
        public T Get<T>() where T : StorageBase
        {
            Type type = typeof(T);
            if (!_typeToName.TryGetValue(type, out string name))
            {
                name = type.Name;
                _typeToName[type] = name;
            }

            if (_storageMap.TryGetValue(name, out StorageBase storage))
            {
                return storage as T;
            }

            throw new GameFrameworkException($"Storage type '{name}' not found.");
        }

        /// <summary>
        /// 从JSON字符串加载存档数据。
        /// </summary>
        public void FromJson(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            try
            {
                JObject jsonObject = JObject.Parse(jsonData);
                foreach (string key in _storageMap.Keys)
                {
                    JToken token = jsonObject[key];
                    if (token != null)
                    {
                        string storageJson = token.ToString();
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        JsonConvert.PopulateObject(storageJson, _storageMap[key], settings);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load storage from JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// 将存档数据序列化为JSON字符串。
        /// </summary>
        public string ToJson(bool updateCommon = true)
        {
            if (updateCommon)
            {
                ProcessUpdateTask();
            }

            return JsonConvert.SerializeObject(_storageMap, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        /// 更新本地版本号。
        /// </summary>
        public void UpdateLocalVersion()
        {
            _localVersion++;
        }

        /// <summary>
        /// 强制保存到磁盘。
        /// </summary>
        public void ForceSaveToDisk()
        {
            _needForceSave = true;
        }

        /// <summary>
        /// 保存到本地（如果版本有变化）。
        /// </summary>
        public bool SaveToLocal()
        {
            if (_lastSavedVersion >= _localVersion && !_needForceSave)
            {
                return false;
            }

            try
            {
                string json = ToJson();
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                string base64 = Convert.ToBase64String(bytes);
                
                Utility.PlayerPrefs.SetString(StorageDataKey, base64);
                Utility.PlayerPrefs.SetString(StorageVersionKey, _localVersion.ToString());
                Utility.PlayerPrefs.Save();

                _lastSavedVersion = _localVersion;
                _needForceSave = false;
                
                Log.Debug($"Storage saved. Version: {_localVersion}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save storage: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 从本地加载数据。
        /// </summary>
        private void LoadLocalData()
        {
            if (Utility.PlayerPrefs.HasKey(StorageDataKey))
            {
                try
                {
                    string base64 = Utility.PlayerPrefs.GetString(StorageDataKey);
                    byte[] bytes = Convert.FromBase64String(base64);
                    string json = Encoding.UTF8.GetString(bytes);
                    
                    Log.Debug($"Loading storage from local: {json}");
                    FromJson(json);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to load storage from local: {ex.Message}");
                }
            }
            else
            {
                Log.Debug("No local storage data found.");
            }
        }

        /// <summary>
        /// 加载本地版本号。
        /// </summary>
        private void LoadLocalVersion()
        {
            if (Utility.PlayerPrefs.HasKey(StorageVersionKey))
            {
                string versionStr = Utility.PlayerPrefs.GetString(StorageVersionKey);
                if (ulong.TryParse(versionStr, out ulong version))
                {
                    _localVersion = version;
                    _lastSavedVersion = version;
                    Log.Debug($"Loaded storage version: {_localVersion}");
                }
            }
            else
            {
                _localVersion = 0;
                _lastSavedVersion = 0;
                Log.Debug("No storage version found, starting from 0");
            }
        }

        /// <summary>
        /// 更新方法，定期保存数据。
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _elapseTimeSinceLastSave += elapseSeconds;
            if ((_needForceSave || _elapseTimeSinceLastSave > SaveToDiskInterval) && SaveToLocal())
            {
                _needForceSave = false;
                _elapseTimeSinceLastSave = 0f;
            }
        }

        /// <summary>
        /// 添加存档更新任务。
        /// </summary>
        public void AddStorageUpdateTask(Action updateTask)
        {
            lock (_storageUpdateTask)
            {
                _storageUpdateTask.Enqueue(updateTask);
            }
        }

        /// <summary>
        /// 处理更新任务。
        /// </summary>
        private void ProcessUpdateTask()
        {
            lock (_storageUpdateTask)
            {
                while (_storageUpdateTask.Count > 0)
                {
                    _storageUpdateTask.Dequeue()?.Invoke();
                }
            }
        }

        /// <summary>
        /// 执行一次操作。
        /// </summary>
        public bool RunOnce(Action callback)
        {
            if (Utility.PlayerPrefs.HasKey(RunOnceKey))
            {
                return false;
            }

            callback?.Invoke();
            Utility.PlayerPrefs.SetString(RunOnceKey, "true");
            ForceSaveToDisk();
            return true;
        }

        /// <summary>
        /// 清空所有存档数据。
        /// </summary>
        public void Clear()
        {
            foreach (string key in _storageMap.Keys)
            {
                _storageMap[key] = StorageHelper.Clear(_storageMap[key]);
            }
        }

        /// <summary>
        /// 删除本地存档。
        /// </summary>
        public void DeleteLocalStorage()
        {
            Utility.PlayerPrefs.DeleteKey(StorageDataKey);
            Utility.PlayerPrefs.DeleteKey(StorageVersionKey);
            Utility.PlayerPrefs.DeleteKey(RunOnceKey);
            Utility.PlayerPrefs.Save();
        }
    }
}

