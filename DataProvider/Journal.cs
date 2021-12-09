using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProvider
{
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<int> Marks { get; set; }

        public double GetMiddleMark() => GetMarksSum() / Marks.Count;

        public double GetMarksSum() => Marks.Aggregate(0.0, (sum, mark) => sum + mark);
    }

    public class Journal
    {
        public Dictionary<int, List<Student>> Groups { get; private set; }

        public Journal()
        {
            Groups = new Dictionary<int, List<Student>>();
        }

        public double GetMiddleGroupPerformance(int groupId)
        {
            if (!Groups.ContainsKey(groupId))
                throw new Exception($"Группа {groupId} не существует");

            var group = Groups[groupId];
            var allMarksSum = group.Aggregate(0.0, (sum, student) => sum + student.GetMarksSum());
            var allMarksCount = group.Aggregate(0.0, (sum, student) => sum + student.Marks.Count);
            return allMarksSum / allMarksCount;
        }

        public double GetGroupPerformance(int groupId)
        {
            if (!Groups.ContainsKey(groupId))
                throw new Exception($"Группа {groupId} не существует");

            var group = Groups[groupId];
            var middleMarksSum = group.Aggregate(0.0, (sum, value) => sum + value.GetMiddleMark());
            return middleMarksSum / group.Count;
        }

        public double GetMiddleGroupAge(int groupId)
        {
            if (!Groups.ContainsKey(groupId))
                throw new Exception($"Группа {groupId} не существует");

            var group = Groups[groupId];
            var sumOfAges = group.Aggregate(0.0, (sum, student) => sum + student.Age);
            return sumOfAges / group.Count;
        }

        public int GetCountOfGroups() => Groups.Count;

        public void AddGroup(int groupId, List<Student> group) => Groups.Add(groupId, group);

        public Student GetStudent(int group, int number) => Groups[group][number];
    }
}
