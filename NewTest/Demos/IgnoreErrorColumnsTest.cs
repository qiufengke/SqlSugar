using System;
using NewTest.Dao;
using NewTest.Interface;
using SqlSugar.Tool;

namespace NewTest.Demos
{
    //排除错误列
    public class IgnoreErrorColumnsTest : IDemos
    {
        public void Init()
        {
            Console.WriteLine("启动IgnoreErrorColumns.Init");
            using (var db = SugarDao.GetInstance())
            {
                db.IsIgnoreErrorColumns = true;

                //Student表并没有 AreaName
                var id = db.Insert(new STUDENT { name = "张三", AreaName = "北大" });


                db.Update(new STUDENT { id = id.ObjToInt(), name = "张三2", AreaName = "北大" });
            }
        }

        public class STUDENT
        {
            /// <summary>
            /// Desc:- 
            /// Default:- 
            /// Nullable:False 
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// Desc:- 
            /// Default:- 
            /// Nullable:True 
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// Desc:学校ID 
            /// Default:- 
            /// Nullable:True 
            /// </summary>
            public int? sch_id { get; set; }

            /// <summary>
            /// Desc:- 
            /// Default:- 
            /// Nullable:True 
            /// </summary>
            public string sex { get; set; }

            /// <summary>
            /// Desc:- 
            /// Default:- 
            /// Nullable:True 
            /// </summary>
            public bool? isOk { get; set; }

            public string SchoolName { get; set; }
            public string AreaName { get; set; }
            public string SubjectName { get; set; }
        }
    }
}