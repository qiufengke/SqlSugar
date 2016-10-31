using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using NewTest.Interface;
using NewTest.Models;
using SqlSugar;
using SqlSugar.Queryable;

namespace NewTest.Demos
{
    //插入
    public class InsertTest : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Inset.Init");
            using (var db = SugarDao.GetInstance())
            {

                db.Insert(GetInsertItem()); //插入一条记录 (有主键也好，没主键也好，有自增列也好都可以插进去)


                db.InsertRange(GetInsertList()); //批量插入 支持（别名表等功能）


                db.SqlBulkCopy(GetInsertList()); //批量插入 适合海量数据插入



                //设置不插入列
                db.DisableInsertColumns = new string[] { "sex" };//sex列将不会插入值
                StudentEntity s = new StudentEntity()
                {
                    name = "张" + new Random().Next(1, int.MaxValue),
                    sex = "gril"
                };

                var id = db.Insert(s); //插入

                //查询刚插入的sex是否有值
                var sex = db.Queryable<StudentEntity>().Single(it => it.id == id.ObjToInt()).sex;//无值
                var name = db.Queryable<StudentEntity>().Single(it => it.id == id.ObjToInt()).name;//有值


                //SqlBulkCopy同样支持不挺入列设置
                db.SqlBulkCopy(GetInsertList());

                //清空禁止插入列
                db.DisableInsertColumns = null;
                //添加禁止插入列
                db.AddDisableInsertColumns("name","id");
            }
        }

        private static List<StudentEntity> GetInsertList()
        {
            List<StudentEntity> list = new List<StudentEntity>()
                {
                     new StudentEntity()
                {
                     name="张"+new Random().Next(1,int.MaxValue),
                     isOk=true,
                     sch_id=1
                },
                 new StudentEntity()
                {
                     name="张"+new Random().Next(1,int.MaxValue),
                     isOk=false,
                     sch_id=2
                }
                };
            return list;
        }

        private static StudentEntity GetInsertItem()
        {
            StudentEntity s = new StudentEntity()
            {
                name = "张" + new Random().Next(1, int.MaxValue)
            };
            return s;
        }
    }
}
