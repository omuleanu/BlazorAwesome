using UiServer.Data;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using Omu.BlazorAwesome.Models.Utils;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class AweUtilTests
    {
        [Test]
        public void Test1()
        {
            var str = "123.12345";
            var res = CompUtil.RemExtraDecimals(str, ".", 3);
            Assert.AreEqual("123.123", res);
        }

        [Test]
        public void GetExpressionPropStr()
        {
            Expression<Func<Dinner, object>> exp1 = o => o.Id;
            Expression<Func<Dinner, object>> exp2 = o => o.Name;
            Expression<Func<Dinner, object>> exp3 = o => o.BonusMeal.Name;

            var str1 = AweExprUtil.GetExpressionStr(exp1);
            var str2 = AweExprUtil.GetExpressionStr(exp2);
            var str3 = AweExprUtil.GetExpressionStr(exp3);

            Assert.AreEqual("Id", str1);
            Assert.AreEqual("Name", str2);
            Assert.AreEqual("BonusMeal.Name", str3);
        }
    }
}