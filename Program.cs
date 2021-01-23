﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Console.WriteLine("data[1]=" + l[1]);
            Console.WriteLine("data[2]=" + l[2]);
            Console.WriteLine("data[3]=" + l[3]);

            l.Clear();
            l.Add("end");
            Console.WriteLine("data[1]=" + l[1]);

            // Compare

            PersistentList<string> pl = new PersistentList<string>(true);

            pl.Add("start");
            pl.Add("next");
            pl.Add("end");
            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[1]=" + pl[1]);
            Console.WriteLine("data[2]=" + pl[2]);
            Console.WriteLine("data[3]=" + pl[3]);

            pl.Clear();
            pl.Add("end");
            Console.WriteLine("data[1]=" + pl[1]);

        }
    }
}