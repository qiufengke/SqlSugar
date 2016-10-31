using System;
using System.Collections.Generic;
using NewTest.Dao;
using NewTest.Interface;
using NewTest.Models;
using SqlSugar;
using SqlSugar.PubModel;
using SqlSugar.Queryable;

namespace NewTest.Demos
{
    //行过滤加列过滤
    //权限管理的最佳设计
    public class Filter2Test : IDemos
    {
        public void Init()
        {
            Console.WriteLine("启动Filter2.Init");
            using (var db = SugarDaoFilter.GetInstance()) //开启数据库连接
            {
                //设置走哪个过滤器
                db.CurrentFilterKey = "role1";
                //queryable
                var list = db.Queryable<StudentEntity>().ToJson(); //where id=1 , 可以查看id和name


                //设置走哪个过滤器
                db.CurrentFilterKey = "role2";
                //queryable
                var list2 = db.Queryable<StudentEntity>().ToJson(); //where id=2 , 可以查看name
            }
        }

        /// <summary>
        /// 扩展SqlSugarClient
        /// </summary>
        public class SugarDaoFilter
        {
            //禁止实例化
            private SugarDaoFilter()
            {
            }

            /// <summary>
            /// 页面所需要的过滤行
            /// </summary>
            private static readonly Dictionary<string, Func<KeyValueObj>> _filterRos = new Dictionary
                <string, Func<KeyValueObj>>
            {
                {
                    "role1", () => { return new KeyValueObj {Key = " id=@id", Value = new {id = 1}}; }
                },
                {
                    "role2", () => { return new KeyValueObj {Key = " id=@id", Value = new {id = 2}}; }
                }
            };

            /// <summary>
            /// 页面所需要的过滤列
            /// </summary>
            private static readonly Dictionary<string, List<string>> _filterColumns = new Dictionary
                <string, List<string>>
            {
                {
                    "role1", new List<string> {"id", "name"}
                },
                {
                    "role2", new List<string> {"name"}
                }
            };


            public static SqlSugarClient GetInstance()
            {
                var connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
                var db = new SqlSugarClient(connection);

                //支持sqlable和queryable
                db.SetFilterFilterParas(_filterRos);

                //列过滤只支持queryable
                db.SetFilterFilterParas(_filterColumns);


                db.IsEnableLogEvent = true; //启用日志事件
                db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
                return db;
            }
        }
    }
}