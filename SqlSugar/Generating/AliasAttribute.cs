using System;
using System.Collections.Generic;
using SqlSugar.PubModel;
using SqlSugar.Tool;

namespace SqlSugar.Generating
{
    /// <summary>
    /// 表名属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// 据库对应的表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据库对应的列名
        /// </summary>
        public string ColumnName { get; set; }
    }

    internal class ReflectionSugarMapping
    {
        /// <summary>
        /// 通过反射取自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static SugarMappingModel GetMappingInfo<T>()
        {
            var objType = typeof(T);
            var cacheKey = "ReflectionSugarMapping.DisplaySelfAttribute" + objType.FullName;
            var cm = CacheManager<SugarMappingModel>.GetInstance();
            if (cm.ContainsKey(cacheKey))
            {
                return cm[cacheKey];
            }
            var reval = new SugarMappingModel();
            var tableName = string.Empty;
            var columnInfoList = new List<KeyValue>();
            var oldName = objType.Name;
            //取属性上的自定义特性
            foreach (var propInfo in objType.GetProperties())
            {
                var objAttrs = propInfo.GetCustomAttributes(typeof(AliasAttribute), true);
                if (objAttrs.Length > 0)
                {
                    if (objAttrs[0] is AliasAttribute)
                    {
                        var attr = objAttrs[0] as AliasAttribute;
                        if (attr != null)
                        {
                            columnInfoList.Add(new KeyValue { Key = propInfo.Name, Value = attr.ColumnName }); //列名
                        }
                    }
                }
            }

            //取类上的自定义特性
            var objs = objType.GetCustomAttributes(typeof(AliasAttribute), true);
            foreach (var obj in objs)
            {
                if (obj is AliasAttribute)
                {
                    var attr = obj as AliasAttribute;
                    if (attr != null)
                    {
                        tableName = attr.TableName; //表名只有获取一次
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = objType.Name;
            }
            reval.TableMaping = new KeyValue { Key = oldName, Value = tableName };
            reval.ColumnsMapping = columnInfoList;
            cm.Add(cacheKey, reval, cm.Day);
            return reval;
        }
    }

    internal class SugarMappingModel
    {
        public KeyValue TableMaping { get; set; }
        public List<KeyValue> ColumnsMapping { get; set; }
    }
}