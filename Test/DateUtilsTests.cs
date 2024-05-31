using System;
using System.Diagnostics;
using NUnit.Framework;
using Omu.BlazorAwesome.Models.Utils;

namespace Test
{
    public class DateUtilsTests
    {
        [Test]
        public void StartOfWeek()
        {
            Assert.AreEqual(8, AweDateUtil.StartOfWeek(DayOfWeek.Monday, new DateTime(2021, 11, 13)).Day);

            Assert.AreEqual(7, AweDateUtil.StartOfWeek(DayOfWeek.Sunday, new DateTime(2021, 11, 13)).Day);

            Assert.AreEqual(1, AweDateUtil.StartOfWeek(DayOfWeek.Monday, new DateTime(2021, 11, 7)).Day);
        }

        [Test]
        public void FirstDayOfCalendar()
        {
            var res = AweDateUtil.FirstDayOfCalendar(new DateTime(2021, 11, 13), DayOfWeek.Sunday);
            Assert.AreEqual(31, res.Day);
            Assert.AreEqual(10, res.Month);
            Assert.AreEqual(2021, res.Year);
        }

        [Test]
        public void Asda()
        {
            var x = 123;
            object y = 321;            

            var sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < 1000000; i++)
            {
                x = (int) y;
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}