using System;
using NewTest.Demos;
using NewTest.Interface;
using SqlSugar.PubModel;

/*
 * 更多例子请移步  http://www.cnblogs.com/sunkaixuan/
 * T4生成 http://www.cnblogs.com/sunkaixuan/p/5751503.html
 */

namespace NewTest
{
    internal class Program
    {
        /// <summary>
        /// SqlSugar的功能介绍
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            // 设置执行的DEMO    
            var switchOn = "select";
            IDemos demo = null;
            switch (switchOn)
            {
                #region 基本功能

                // 查询
                case "select":
                    demo = new SelectTest();
                    break;
                // 删除
                case "delete":
                    demo = new DeleteTest();
                    break;
                // 插入
                case "insert":
                    demo = new InsertTest();
                    break;
                // 更新
                case "update":
                    demo = new UpdateTest();
                    break;
                // 基层函数的用法
                case "ado":
                    demo = new AdoTest();
                    break;
                // 事务
                case "tran":
                    demo = new TranTest();
                    break;
                // 创建实体函数
                case "createclass":
                    demo = new CreateClassTest();
                    break;
                // 日志记录
                case "log":
                    demo = new LogTest();
                    break;
                // 枚举支持
                case "enum":
                    demo = new EnumDemoTest();
                    break;

                #endregion

                #region 实体映射

                // 自动排除非数据库列
                case "ignoreerrorcolumns":
                    demo = new IgnoreErrorColumnsTest();
                    break;
                // 别名表
                case "mappingtable":
                    demo = new MappingTableTest();
                    break;
                // 别名列
                case "mappingcolumns":
                    demo = new MappingColumnsTest();
                    break;
                // 通过属性的方法设置别名表和别名字段
                case "attributesmapping":
                    demo = new AttributesMappingTest();
                    break;

                #endregion

                #region 业务应用

                // 过滤器
                case "filter":
                    demo = new FilterTest();
                    break;
                // 过滤器2
                case "filter2":
                    demo = new Filter2Test();
                    break;
                // 流水号功能
                case "serialnumber":
                    demo = new SerialNumberTest();
                    break;
                // 多语言支持 http://www.cnblogs.com/sunkaixuan/p/5709583.html
                // 多库并行计算 http://www.cnblogs.com/sunkaixuan/p/5046517.html
                // 配置与实例的用法
                case "initconfig":
                    demo = new InitConfigTest();
                    break;

                #endregion

                #region 支持

                // 公开函数数
                case "pubmethod":
                    demo = new PubMethodTest();
                    break;
                // Sql2012分页的支持
                case "sqlpagemodel":
                    demo = new SqlPageModelTest();
                    break;
                // 设置ToJson的日期格式
                case "serializerdateformat":
                    demo = new SerializerDateFormatTest();
                    break;

                #endregion

                #region 测试用例

                case "test":
                    demo = new UnitTest();
                    break;
                default:
                    Console.WriteLine("switchOn的值错误，请输入正确的 case");
                    break;

                    #endregion
            }

            if (demo == null) return;

            // 执行DEMO
            demo.Init();

            Console.WriteLine("执行成功请关闭窗口");
            Console.ReadKey();
        }
    }
}