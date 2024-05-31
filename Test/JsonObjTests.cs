using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Omu.BlazorAwesome.Models.Utils;

namespace Test
{
    public class JsonObjTests
    {
        [Test]
        public void ToJsonObj()
        {
            var list = new List<object>();
            list.Add(1);
            list.Add("abc");
            list.Add(new DateTime(2012, 12, 31));

            var jsonObjs = list.Select(o => AweJsonUtil.ToAweJsonObj(o));
            var res = jsonObjs.Select(o => AweJsonUtil.FromAweJsonObj(o)).ToList();

            Assert.AreEqual(3, res.Count());

            for (int i = 0; i < res.Count(); i++)
            {
                Assert.AreEqual(list[i], res[i]);
            }
        }

        [Test]
        public void ParseArrayToJson()
        {
            object arr = new int[]{1,2,3};
            var jo = AweJsonUtil.ToAweJsonObj(arr);
            var arr1 = AweJsonUtil.FromAweJsonObj(jo);

            Assert.AreEqual(arr, arr1);
        }

        [Test]
        public void TryConvertToType()
        {
            var x = "1";
            var y = Convert.ChangeType(x, Type.GetType(typeof(int).AssemblyQualifiedName));
            Assert.AreEqual(1, y);
        }
    }
}
