using System;
using Newtonsoft.Json;

namespace TEngine
{
    /// <summary>
    /// 通用存档数据示例类。
    /// 可以根据实际需求修改或扩展此类。
    /// </summary>
    [Serializable]
    public class StorageCommon : StorageBase
    {
        [JsonProperty]
        private string _playerName = "";

        [JsonProperty]
        private int _level = 1;

        [JsonProperty]
        private ulong _experience = 0;

        [JsonProperty]
        private ulong _coins = 0;

        [JsonProperty]
        private ulong _lastLoginTime = 0;

    
        /// <summary>
        /// 玩家名称。
        /// </summary>
        [JsonIgnore]
        public string PlayerName
        {
            get => _playerName;
            set
            {
                if (_playerName != value)
                {
                    _playerName = value;
                    MarkDirty();
                }
            }
        }

        /// <summary>
        /// 玩家等级。
        /// </summary>
        [JsonIgnore]
        public int Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    MarkDirty();
                }
            }
        }

        /// <summary>
        /// 玩家经验值。
        /// </summary>
        [JsonIgnore]
        public ulong Experience
        {
            get => _experience;
            set
            {
                if (_experience != value)
                {
                    _experience = value;
                    MarkDirty();
                }
            }
        }

        /// <summary>
        /// 玩家金币。
        /// </summary>
        [JsonIgnore]
        public ulong Coins
        {
            get => _coins;
            set
            {
                if (_coins != value)
                {
                    _coins = value;
                    MarkDirty();
                }
            }
        }

        /// <summary>
        /// 最后登录时间（时间戳）。
        /// </summary>
        [JsonIgnore]
        public ulong LastLoginTime
        {
            get => _lastLoginTime;
            set
            {
                if (_lastLoginTime != value)
                {
                    _lastLoginTime = value;
                    MarkDirty();
                }
            }
        }

 
    }
}

