using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolCourse
{
    
    class AbstractModel
    {
        private List<string> files = new List<string>();
        public List<string> words = new List<string>();

        public static AbstractModel Model;

        public AbstractModel()
        {
            Model = this;
        }

        public void startAbstract()
        {
            MorphAnalysis.startAnalysis("C:\\Users\\DanilKolesnikov\\Desktop\\КУРСОВАЯ\\ConsolCourse\\ConsolCourse\\input.txt", "input.txt", "output.txt");

            Console.WriteLine("DoneMorf!");

            TextCleaner.startClear("output.txt", "fragmentoutput.txt", "vectors.bin", true);

            Console.WriteLine("DoneClear!");

            Abstract.startAbstract("vectors.bin", "probability.bin", 20);

            Console.WriteLine("DoneAbstract!");

            FragmentAnalysis.startSegment("fragmentoutput.txt", "segments.bin");

            Console.WriteLine("DoneSegment!");

            FragmentAnalysis.startFragment("fragmentoutput.txt", "segments.bin", "probability.bin", "test.txt");

            Console.WriteLine("Done!");
            
        }
    }
}
