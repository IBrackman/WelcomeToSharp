using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {


            using (var file = File.Open("test.txt", FileMode.Open))
            using (var writer = new StreamWriter(file))
            {
                writer.WriteLine("hello!");
            }

            using (var file = File.Open("test1.txt", FileMode.OpenOrCreate))
            {
                var x = new XmlSerializer(typeof (List<Test>));
                x.Serialize(file, new List<Test>()
                {
                    new Test() {Value = 123},
                    new Test() {Value = 456},
                    new Test() {Value = 789},
                });
            }
        }
    }

    [XmlRoot]
    public class Test
    {
        [XmlElement("BLABLA")]
        public int Value { get; set; }
    }
}
