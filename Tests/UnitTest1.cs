using System;
using System.Collections.Generic;
using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{

    static class Helper
    {
        public static void Create(this Journal result, List<int> counts)
        {
            int groupId = 1;
            foreach (var groupCount in counts)
            {
                var IthGroup = new List<Student>();
                for (var i = 0; i < groupCount; i++)
                {
                    IthGroup.Add(new Student()
                    {
                        Name = $"test name {i}"
                    });
                }

                result.AddGroup(groupId, IthGroup);
                groupId++;
            }
        }

        public static void SetMarks(this Journal result, int groupId, int number, List<int> marks)
        {
            var student = result.GetStudent(groupId, number);
            student.Marks = marks;
        }
    }

    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void MiddlePerformanceTest_FirstGroup()
        {
            var j = new Journal();
            j.Create(new List<int>() {2});
            j.SetMarks(1, 0, new List<int>() { 2, 2, 2, 2});
            j.SetMarks(1, 1, new List<int>() { 4 });

            Assert.AreEqual(2.4, j.GetMiddleGroupPerformance(1));
        }


        [TestMethod]
        public void MiddlePerformanceTest_SecondGroup()
        {
            var j = new Journal();
            j.Create(new List<int>() { 2, 1 });
            j.SetMarks(1, 0, new List<int>() { 2, 2, 2, 2 });
            j.SetMarks(1, 1, new List<int>() { 4 });
            j.SetMarks(2, 0, new List<int>() { 5, 5, 5 });

            Assert.AreEqual(5, j.GetMiddleGroupPerformance(2));
        }

        [TestMethod]
        public void MiddlePerformanceTest_UnknownGroup()
        {
            var j = new Journal();
            j.Create(new List<int>() { 2, 1 });
            j.SetMarks(1, 0, new List<int>() { 2, 2, 2, 2 });
            j.SetMarks(1, 1, new List<int>() { 4 });
            j.SetMarks(2, 0, new List<int>() { 5, 5, 5 });

            bool throwed = false;
            try
            {
                j.GetMiddleGroupPerformance(3);
            }
            catch (Exception)
            {
                throwed = true;
            }

            Assert.IsTrue(throwed);
        }

        [TestMethod]
        public void GroupPerformanceTest()
        {
            var j = new Journal();
            j.Create(new List<int>() { 2 });
            j.SetMarks(1, 0, new List<int>() { 2, 5, 2, 5 });
            j.SetMarks(1, 1, new List<int>() { 4, 5 });

            Assert.AreEqual(4, j.GetGroupPerformance(1));
        }

        [TestMethod]
        public void MiddleMarkTest()
        {
            var j = new Journal();
            j.Create(new List<int>() { 2 });
            j.SetMarks(1, 0, new List<int>() { 2, 5, 2, 5 });
            j.SetMarks(1, 1, new List<int>() { 4, 5 });

            var student1 = j.GetStudent(1, 0);
            var student2 = j.GetStudent(1, 1);
            Assert.AreEqual(3.5, student1.GetMiddleMark());
            Assert.AreEqual(4.5, student2.GetMiddleMark());
        }

        [TestMethod]
        public void MiddleGroupAgeTest()
        {
            var j = new Journal();
            j.Create(new List<int>() { 2 });

            var student1 = j.GetStudent(1, 0);
            var student2 = j.GetStudent(1, 1);
            student1.Age = 15;
            student2.Age = 16;

            Assert.AreEqual(15.5, j.GetMiddleGroupAge(1));
        }

        [TestMethod]
        public void CountOfGroupsTest()
        {
            var j = new Journal();
            j.Create(new List<int>() { 2, 1 });
            j.SetMarks(1, 0, new List<int>() { 2, 2, 2, 2 });
            j.SetMarks(1, 1, new List<int>() { 4 });
            j.SetMarks(2, 0, new List<int>() { 5, 5, 5 });

            Assert.AreEqual(2, j.GetCountOfGroups());
        }
    }
}
