using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.AppData
{
    internal class ConDB
    {
        private static AnturazhBDEntities c;
        public static AnturazhBDEntities context
        {
            get

            {
                if (c == null)

                    c = new AnturazhBDEntities();
                return c;
            }
        }
    }
}
