using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendingItemsMail
{
    class Program
    {
        static void Main(string[] args)
        {
            PendingItemsMailGeneration p = new PendingItemsMailGeneration();
            p.GeneratePendingManualdata();
        }
    }
}
