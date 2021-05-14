using System;
using System.Collections.Generic;
using System.Reflection;

namespace List
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test the standard List

            IList<string> l = new List<string>();
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

            // Test the PersistentList

            PersistentList<string> pl = new PersistentList<string>(true);

            pl.Add("start");
            pl.Add("next");
            pl.Add("end");
            Console.WriteLine("count" + pl.Count);
            Console.WriteLine("data[0]=" + pl[0]);
            Console.WriteLine("data[1]=" + pl[1]);
            Console.WriteLine("data[2]=" + pl[2]);

            pl.Insert(0, "insert");
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

            pl.Dispose();   // Delete the file

            Console.WriteLine("--------");

            // Test the PersistentList private methods

            PersistentList<string> ppl = new PersistentList<string>(true);
            object obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Create", ppl, new object[2] { 0, "start"});
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Create", ppl, new object[2] { 1, "next" });
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Create", ppl, new object[2] { 2, "end" });
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Delete", ppl, new object[1] { 1 });
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Create", ppl, new object[2] { 1, "next" });
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Update", ppl, new object[2] { 1, "to" });
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Update", ppl, new object[2] { 1, "longer" });

            for (int i = 0; i < 3; i++)
            {
                obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Read", ppl, new object[1] { i });
                string s = obj.ToString();
                Console.WriteLine(s);
            }
            obj = RunInstanceMethod(typeof(List.PersistentList<string>), "Close", ppl, null);

        }

        public static object RunStaticMethod(System.Type t, string strMethod, object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(t, strMethod, null, aobjParams, eFlags);
        }

        public static object RunInstanceMethod(System.Type t, string strMethod, object objInstance, object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(t, strMethod, objInstance, aobjParams, eFlags);
        }

        private static object RunMethod(System.Type t, string strMethod, object objInstance, object[] aobjParams, BindingFlags eFlags)
        {
            MethodInfo m;
            try
            {
                m = t.GetMethod(strMethod, eFlags);
                if (m == null)
                {
                    throw new ArgumentException("There is no method '" + strMethod + "' for type '" + t.ToString() + "'.");
                }

                object objRet = m.Invoke(objInstance, aobjParams);
                return objRet;
            }
            catch
            {
                throw;
            }
        }
    }
}
