using System;
using System.Reflection;

namespace TEngine
{
    /// <summary>
    /// 存档辅助工具类。
    /// </summary>
    public static class StorageHelper
    {
        /// <summary>
        /// 清空存档数据，将所有字段重置为默认值。
        /// </summary>
        public static StorageBase Clear(StorageBase storage)
        {
            if (storage == null)
            {
                return null;
            }

            PropertyInfo[] properties = storage.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (PropertyInfo property in properties)
            {
                Type propertyType = property.PropertyType;

                // 跳过只读属性
                if (!property.CanWrite)
                {
                    continue;
                }

                // 字符串类型
                if (propertyType == typeof(string))
                {
                    property.SetValue(storage, string.Empty, null);
                    continue;
                }

                // 数值类型
                if (propertyType == typeof(int))
                {
                    property.SetValue(storage, 0, null);
                    continue;
                }

                if (propertyType == typeof(long))
                {
                    property.SetValue(storage, 0L, null);
                    continue;
                }

                if (propertyType == typeof(uint))
                {
                    property.SetValue(storage, 0u, null);
                    continue;
                }

                if (propertyType == typeof(ulong))
                {
                    property.SetValue(storage, 0uL, null);
                    continue;
                }

                if (propertyType == typeof(float))
                {
                    property.SetValue(storage, 0f, null);
                    continue;
                }

                if (propertyType == typeof(double))
                {
                    property.SetValue(storage, 0.0, null);
                    continue;
                }

                if (propertyType == typeof(bool))
                {
                    property.SetValue(storage, false, null);
                    continue;
                }

                // StorageBase子类
                if (propertyType.IsSubclassOf(typeof(StorageBase)))
                {
                    object value = property.GetValue(storage, null);
                    if (value != null)
                    {
                        Clear(value as StorageBase);
                    }
                    continue;
                }
                
                // 其他有Clear方法的类型
                object value2 = property.GetValue(storage, null);
                if (value2 != null)
                {
                    MethodInfo clearMethod = value2.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(value2, null);
                    }
                }
            }

            return storage;
        }
    }
}

