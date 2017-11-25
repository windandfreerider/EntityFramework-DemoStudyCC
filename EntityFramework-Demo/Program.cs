using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Mock 数据代码
            // 先循环插入吧
            //using (var context = new Entities())
            //{
            //    for (int i = 0; i < 500; i++)
            //    {
            //        context.Test.Add(new Test
            //        {
            //            Id=i,
            //            Name = "Name" + i,
            //            Age = i,
            //            Descr = "我是EF插进来的数据" + i
            //        });
            //        //context.SaveChanges(); 这里犯了错误。。保存不能写到循环里面
            //        // 主键忘记自增了
            //    }
            //    context.SaveChanges();
            //} 
            #endregion
            //数据创建好了

            var query = new QueryTest();
            //先查询 年龄 20-50的我们只需要
            Console.WriteLine("年龄 20-50:");
            var result = query.Query(new QueryParams
            {
                MinAge = 20,
                MaxAge = 50
            });
            //打印结果

            foreach (var item in result)
            {
                Console.WriteLine($"Id:{item.Id},Name:{item.Name}");
            }
            //验证结果是正确的
            Console.WriteLine("查询名字包含有1 所有数据:");
            //再来根据名字查,查询名字包含有1 所有数据 
            result = query.Query(new QueryParams
            {
                Name = "1"
            });
            foreach (var item in result)
            {
                Console.WriteLine($"Id:{item.Id},Name:{item.Name}");
            }
            Console.ReadKey();

        }
    }
    //查询 测试类，我在这里创建了一个Class
    public class QueryTest
    {
        public List<Test> Query(QueryParams queryParams)
        {
            using (var context = new Entities())
            {
                //var query = context.Test.AsQueryable();
                //if (queryParams.MinAge.HasValue)
                //{
                //    query=query.Where(w => w.Age >= queryParams.MinAge);
                //}
                //if (queryParams.MaxAge.HasValue)
                //{
                //    query = query.Where(w => w.Age <= queryParams.MaxAge);
                //}
                //这里顺便提下。由于这样写很难看。我一般会封装一个扩展方法
                //有了扩展方法。我们这里可以这样写了
                return context.Test
                            .WhereIf(queryParams.MinAge.HasValue, w => w.Age >= queryParams.MinAge.Value)
                            .WhereIf(queryParams.MaxAge.HasValue, w => w.Age <= queryParams.MaxAge.Value)
                            .WhereIf(!string.IsNullOrEmpty(queryParams.Name), w => w.Name.Contains(queryParams.Name))
                            .ToList();


            }
        }
    }
    /// <summary>
    /// 查询参数模型
    /// 假设我们这里，可以根据年龄段，和名字(模糊查找)
    /// </summary>
    public class QueryParams
    {
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string Name { get; set; }
    }
    public static class LinqExt
    {
        /// <summary>
        /// 泛型扩展方法。具体细节 ，以后有时间再解释吧
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="func"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool b, Expression<Func<T, bool>> predicate)
        {
            return b ? query.Where(predicate) : query;
        }
    }
}
