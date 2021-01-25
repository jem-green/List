using System;
using System.Collections.Generic;

namespace List
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> l = new List<string>();

            l.Add("start");
            l.Add("next");
            l.Add("end");
            Console.WriteLine("count" + l.Count);

            Console.WriteLine("data[0]=" + l[0]);
            Console.WriteLine("data[1]=" + l[1]);
            Console.WriteLine("data[2]=" + l[2]);

            l.Insert(0, "insert");
            Console.WriteLine("count" + l.Count);
            Console.WriteLine("data[0]=" + l[0]);
            Console.WriteLine("data[1]=" + l[1]);
            Console.WriteLine("data[2]=" + l[2]);
            Console.WriteLine("data[3]=" + l[3]);

            Console.WriteLine("Clear List");
            l.Clear();
            l.Add("end");
            Console.WriteLine("data[0]=" + l[0]);
            l.Add("next");
            l.Add("start");

            Console.WriteLine("Update list");
            //l[1] = "to";
            l[1] = "between";

            Console.WriteLine("count" + l.Count);
            Console.WriteLine("data[0]=" + l[0]);
            Console.WriteLine("data[1]=" + l[1]);
            Console.WriteLine("data[2]=" + l[2]);

            l.Remove("end");

            Console.WriteLine("count" + l.Count);
            Console.WriteLine("data[0]=" + l[0]);
            Console.WriteLine("data[1]=" + l[1]);
            //Console.WriteLine("data[2]=" + l[2]);

            foreach (string s in l)
            {
                Console.WriteLine("Enumerate " + s);
            }

            l.Insert(1, "insert");

            Console.WriteLine("count" + l.Count);
            Console.WriteLine("data[0]=" + l[0]);
            Console.WriteLine("data[1]=" + l[1]);
            Console.WriteLine("data[2]=" + l[2]);


            Console.WriteLine("--------");

            PersistentList<string> pl = new PersistentList<string>(true);

            pl.Add("start");
            pl.Add("next");
            pl.Add("end");
            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[0]=" + pl[0]);
            Console.WriteLine("data[1]=" + pl[1]);
            Console.WriteLine("data[2]=" + pl[2]);

            pl.Insert(3, "insert");
            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[0]=" + pl[0]);
            Console.WriteLine("data[1]=" + pl[1]);
            Console.WriteLine("data[2]=" + pl[2]);
            Console.WriteLine("data[3]=" + pl[3]);

            Console.WriteLine("Clear List");
            pl.Clear();
            pl.Add("end");
            Console.WriteLine("data[0]=" + pl[0]);
            pl.Add("next");
            pl.Add("start");


            Console.WriteLine("Update list");
            //pl[1] = "to";
            pl[1] = "between";

            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[0]=" + pl[0]);
            Console.WriteLine("data[1]=" + pl[1]);
            Console.WriteLine("data[2]=" + pl[2]);

            pl.Remove("end");

            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[0]=" + pl[0]);
            Console.WriteLine("data[1]=" + pl[1]);
            //Console.WriteLine("data[2]=" + pl[2]);

            foreach (string s in pl)
            {
                Console.WriteLine("Enumerate " + s);
            }

            pl.Insert(1, "insert");

            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[0]=" + pl[0]);
            Console.WriteLine("data[1]=" + pl[1]);
            Console.WriteLine("data[2]=" + pl[2]);

        }
    }
}
