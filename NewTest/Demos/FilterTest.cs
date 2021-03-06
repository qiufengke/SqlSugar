﻿using System;
using System.Collections.Generic;
using NewTest.Dao;
using NewTest.Interface;
using NewTest.Models;
using SqlSugar;
using SqlSugar.PubModel;
using SqlSugar.Queryable;

namespace NewTest.Demos
{
    //过滤器用法
    //使用场合(例如：假删除查询，这个时候就可以设置一个过滤器,不需要每次都 .Where(it=>it.IsDelete==true))  
    public class FilterTest : IDemos
    {
        public void Init()
        {
            Console.WriteLine("启动Filter.Init");
            using (var db = SugarDaoFilter.GetInstance()) //开启数据库连接
            {
                //设置走哪个过滤器
                db.CurrentFilterKey = "role,role2"; //支持多个过滤器以逗号隔开

                //queryable
                var list = db.Queryable<StudentEntity>().ToList(); //通过全局过滤器对需要权限验证的数据进行过滤
                //相当于db.Queryable<Student>().Where("id=@id",new{id=1})


                //sqlable
                var list2 = db.Sqlable().From<StudentEntity>("s").SelectToList<StudentEntity>("*");
                //同上

                //sqlQuery
                var list3 = db.SqlQuery<StudentEntity>("select * from Student WHERE 1=1");
                //同上
            }
        }

        /// <summary>
        /// 扩展SqlSugarClient
        /// </summary>
        public class SugarDaoFilter
        {
            /// <summary>
            /// 页面所需要的过滤函数
            /// </summary>
            private static readonly Dictionary<string, Func<KeyValueObj>> _filterParas = new Dictionary
                <string, Func<KeyValueObj>>
            {
                {
                    "role", () => new KeyValueObj {Key = " id=@id", Value = new {id = 1}}
                },
                {
                    "role2", () => new KeyValueObj {Key = " id>0"}
                }
            };

            //禁止实例化
            private SugarDaoFilter()
            {
            }

            public static SqlSugarClient GetInstance()
            {
                var connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
                var db = new SqlSugarClient(connection);
                db.SetFilterFilterParas(_filterParas);

                db.IsEnableLogEvent = true; //启用日志事件
                db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
                return db;
            }
        }
    }
}