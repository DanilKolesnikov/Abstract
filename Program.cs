using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsolCourse
{
    
    class Program
    {

        static void Main(string[] args)
        {
            AbstractModel model = new AbstractModel();
            model.startAbstract();
            Console.ReadKey();
            
        }
    }
}
