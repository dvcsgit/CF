using CF;
using System;
using System.Data.Entity;

namespace CFConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new Initializer());
            using (var context=new CFContext())
            {
                context.Database.Initialize(true);
                context.Database.Log = Console.Write;
            }
        }
    }
}
