using UiServer.Data;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using UiServer.Models.Input;
using Omu.BlazorAwesome.Models.Utils;

namespace Test
{
    public class ExpressionTests
    {
        [Test]
        public void ExprTest()
        {
            var input = new DinnerInput { Name = "hi" };
            var nameProp = typeof(DinnerInput).GetProperty(nameof(DinnerInput.Name));

            var propExpr = Expression.Property(Expression.Constant(input), nameProp);

            var lambda = Expression.Lambda<Func<object>>(propExpr);

            var res = lambda.Compile()();

            Console.WriteLine("lambda res = {0}", res);

            Expression<Func<DinnerInput, object>> mainExp = din => din.Name;
        }

        [Test]
        public void GetMemberExpr()
        {
            var dinners = new List<Lunch>()
            {
                new Lunch() { Person = "a", Food = new(){ Name = "abc"} },
                new Lunch() { Person = "b", Food = new(){ Name = "bbb"} },
            }.AsQueryable();

            var res = dinners.Where(AweExprUtil.ContainsExpr<Lunch>("a", "Person")).ToList();
            Assert.AreEqual("a", res[0].Person);

            var res2 = dinners.Where(AweExprUtil.ContainsExpr<Lunch>("bb", "Food.Name")).ToList();
            Assert.AreEqual("b", res2[0].Person);
        }
    }
}