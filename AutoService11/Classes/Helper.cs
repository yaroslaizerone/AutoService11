using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoService11.Entity;

namespace AutoService11.Classes
{
    internal class Helper
    {
        public static bool flag = false;
        public static AutoServiseEntities context;

        public static AutoServiseEntities GetContext()
        {
            if (context == null)
                context = new AutoServiseEntities();
            return context;
        }
    }
}
