﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SqlSugar.Generating;
using SqlSugar.Tool;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Sqlable扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static class SqlableExtensions
    {
        /// <summary>
        /// Form
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="tableName">表名</param>
        /// <param name="shortName">表名简写</param>
        /// <returns></returns>
        public static Sqlable.Sqlable From(this Sqlable.Sqlable sqlable, string tableName, string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", tableName.GetTranslationSqlName(), shortName,
                sqlable.DB.IsNoLock.GetLockString());
            return sqlable;
        }

        /// <summary>
        /// Form
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="shortName">表名简写</param>
        /// <returns></returns>
        public static Sqlable.Sqlable From<T>(this Sqlable.Sqlable sqlable, string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", typeof(T).Name.GetTranslationSqlName(), shortName,
                sqlable.DB.IsNoLock.GetLockString());
            return sqlable;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="tableName">表名字符串</param>
        /// <param name="shortName">表名简写</param>
        /// <param name="leftFiled">join左边连接字段</param>
        /// <param name="RightFiled">join右边连接字段</param>
        /// <param name="type">join类型</param>
        /// <returns></returns>
        public static Sqlable.Sqlable Join(this Sqlable.Sqlable sqlable, string tableName, string shortName, string leftFiled,
            string RightFiled, JoinType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} JOIN {1} {2}  {3} ON  {4} = {5} ", type, tableName.GetTranslationSqlName(),
                shortName, sqlable.DB.IsNoLock.GetLockString(), leftFiled, RightFiled);
            return sqlable;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="shortName">表名简写</param>
        /// <param name="leftFiled">join左边连接字段</param>
        /// <param name="RightFiled">join右边连接字段</param>
        /// <param name="type">join类型</param>
        /// <returns></returns>
        public static Sqlable.Sqlable Join<T>(this Sqlable.Sqlable sqlable, string shortName, string leftFiled, string RightFiled,
            JoinType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} JOIN {1} {2}  {3} ON  {4} = {5} ", type,
                typeof(T).Name.GetTranslationSqlName(), shortName, sqlable.DB.IsNoLock.GetLockString(), leftFiled,
                RightFiled);
            return sqlable;
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="where">查询条件、开头无需写 AND或者WHERE</param>
        /// <returns></returns>
        public static Sqlable.Sqlable Where(this Sqlable.Sqlable sqlable, string where)
        {
            if (where.IsValuable())
            {
                sqlable.Where.Add(string.Format(" AND {0} ", where));
            }
            return sqlable;
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="orderBy">排序字段，可以多个</param>
        /// <returns></returns>
        public static Sqlable.Sqlable OrderBy(this Sqlable.Sqlable sqlable, string orderBy)
        {
            sqlable.OrderBy = "ORDER BY " + orderBy + " ";
            return sqlable;
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="applySql">apply主体内容</param>
        /// <param name="shotName">apply简写</param>
        /// <param name="type">Apply类型</param>
        /// <returns></returns>
        public static Sqlable.Sqlable Apply(this Sqlable.Sqlable sqlable, string applySql, string shotName, ApplyType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} APPLY ({1}) {2}} ", type, applySql, shotName);
            return sqlable;
        }

        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="groupBy">GroupBy字段，可以多个</param>
        /// <returns></returns>
        public static Sqlable.Sqlable GroupBy(this Sqlable.Sqlable sqlable, string groupBy)
        {
            sqlable.GroupBy = "GROUP BY " + groupBy + " ";
            return sqlable;
        }

        /// <summary>
        /// 设置查询列执行查询，并且将结果集转成List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <param name="preSql">在这语句之前可插入自定义SQL</param>
        /// <param name="nextSql">在这语句之后可以插自定义SQL</param>
        /// <returns></returns>
        public static List<T> SelectToList<T>(this Sqlable.Sqlable sqlable, string fileds, object whereObj = null,
            string preSql = null, string nextSql = null) where T : class
        {
            var sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sbSql.Insert(0, string.Format("SELECT {0} ", fileds));
                sbSql.Append(" WHERE 1=1").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                var sqlParams = GetAllParas(sqlable, whereObj);
                if (preSql != null)
                {
                    sbSql.Insert(0, preSql);
                }
                if (nextSql != null)
                {
                    sbSql.Append(nextSql);
                }
                var reval = SqlSugarTool.DataReaderToList<T>(typeof(T),
                    sqlable.DB.GetReader(sbSql.ToString(), sqlParams), fileds);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
                sbSql = null;
            }
        }

        /// <summary>
        /// 获取页面参数
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        private static SqlParameter[] GetAllParas(Sqlable.Sqlable sqlable, object whereObj)
        {
            var allParams = new List<SqlParameter>();
            var selectParas = SqlSugarTool.GetParameters(whereObj).ToList();
            if (selectParas.IsValuable())
            {
                allParams.AddRange(selectParas);
            }
            if (sqlable.Params.IsValuable())
            {
                allParams.AddRange(sqlable.Params);
            }
            return allParams.ToArray();
        }

        /// <summary>
        /// 设置查询列执行查询，并且将结果集转成DataTable
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static DataTable SelectToDataTable(this Sqlable.Sqlable sqlable, string fileds, object whereObj = null)
        {
            var sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sbSql.Insert(0, string.Format("SELECT {0} ", fileds));
                sbSql.Append(" WHERE 1=1").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                var sqlParams = GetAllParas(sqlable, whereObj);
                var reval = sqlable.DB.GetDataTable(sbSql.ToString(), sqlParams);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
                sbSql = null;
            }
        }

        /// <summary>
        /// 设置查询列执行查询，并且将结果集转成json
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static string SelectToJson(this Sqlable.Sqlable sqlable, string fileds, object whereObj = null)
        {
            return JsonConverter.DataTableToJson(SelectToDataTable(sqlable, fileds, whereObj),
                sqlable.DB.SerializerDateFormat);
        }

        /// <summary>
        /// 设置查询列执行查询，并且将结果集转成dynamic
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static dynamic SelectToDynamic(this Sqlable.Sqlable sqlable, string fileds, object whereObj = null)
        {
            return JsonConverter.ConvertJson(SelectToJson(sqlable, fileds, whereObj));
        }

        /// <summary>
        /// 生成查询结果对应的实体类字符串
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static string ToClass(this Sqlable.Sqlable sqlable, string fileds, object whereObj = null)
        {
            var cg = new ClassGenerating();
            var dt = SelectToDataTable(sqlable, fileds, whereObj);
            return cg.DataTableToClass(dt, "TableName");
        }

        /// <summary>
        /// 反回记录数
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="whereObj">匿名参数 (例如：new{id=1,name="张三"})</param>
        /// <param name="preSql">在这语句之前可插入自定义SQL</param>
        /// <param name="nextSql">在这语句之后可以插自定义SQL</param>
        /// <returns></returns>
        public static int Count(this Sqlable.Sqlable sqlable, object whereObj = null, string preSql = null,
            string nextSql = null)
        {
            var sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，Count必需要在.Form后面使用");
                sbSql.Insert(0, "SELECT COUNT(1) ");
                sbSql.Append(" WHERE 1=1").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                var sqlParams = GetAllParas(sqlable, whereObj);
                if (preSql != null)
                {
                    sbSql.Insert(0, preSql);
                }
                if (nextSql != null)
                {
                    sbSql.Append(nextSql);
                }
                return sqlable.DB.GetInt(sbSql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
                sbSql = null;
            }
        }

        /// <summary>
        /// 设置查询列和分页参数执行查询，并且将结果集转成List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="orderByFiled">Order By字段，可以多个</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static List<T> SelectToPageList<T>(this Sqlable.Sqlable sqlable, string fileds, string orderByFiled,
            int pageIndex, int pageSize, object whereObj = null) where T : class
        {
            var sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                if (pageIndex == 0) pageIndex = 1;
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                SqlSugarTool.GetSqlableSql(sqlable, fileds, orderByFiled, pageIndex, pageSize, sbSql);
                var sqlParams = GetAllParas(sqlable, whereObj);
                var reval = SqlSugarTool.DataReaderToList<T>(typeof(T),
                    sqlable.DB.GetReader(sbSql.ToString(), sqlParams), fileds);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sbSql = null;
                sqlable = null;
            }
        }

        /// <summary>
        /// 设置查询列和分页参数执行查询，并且将结果集转成DataTable
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="orderByFiled">Order By字段，可以多个</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static DataTable SelectToPageTable(this Sqlable.Sqlable sqlable, string fileds, string orderByFiled,
            int pageIndex, int pageSize, object whereObj = null)
        {
            var sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                if (pageIndex == 0) pageIndex = 1;
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                SqlSugarTool.GetSqlableSql(sqlable, fileds, orderByFiled, pageIndex, pageSize, sbSql);
                var sqlParams = GetAllParas(sqlable, whereObj);
                var reval = sqlable.DB.GetDataTable(sbSql.ToString(), sqlParams);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sbSql = null;
                sqlable = null;
            }
        }

        /// <summary>
        /// 设置查询列和分页参数执行查询，并且将结果集转成json
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="orderByFiled">Order By字段，可以多个</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static string SelectToPageJson(this Sqlable.Sqlable sqlable, string fileds, string orderByFiled, int pageIndex,
            int pageSize, object whereObj = null)
        {
            return
                JsonConverter.DataTableToJson(
                    SelectToPageTable(sqlable, fileds, orderByFiled, pageIndex, pageSize, whereObj),
                    sqlable.DB.SerializerDateFormat);
        }

        /// <summary>
        /// 设置查询列和分页参数执行查询，并且将结果集转成dynamic
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="orderByFiled">Order By字段，可以多个</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static dynamic SelectToPageDynamic(this Sqlable.Sqlable sqlable, string fileds, string orderByFiled,
            int pageIndex, int pageSize, object whereObj = null)
        {
            return
                JsonConverter.ConvertJson(SelectToPageJson(sqlable, fileds, orderByFiled, pageIndex, pageSize, whereObj));
        }
    }
}