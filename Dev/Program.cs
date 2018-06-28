using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV;
using ACIS_Showcase;
using System.IO;
using System.Security.Policy;

/* Used for debugging and developing: */
namespace Dev
{
    class Program
    {

        public static object SolutionDir { get; private set; }

        /* Determines the Solution Directory and goes to Images,
         * Assumes application is run through debugging mode: */
        private static AppDomain Startup()
        {
            var domaininfo = new AppDomainSetup();
            domaininfo.ConfigurationFile = System.Environment.CurrentDirectory +
                                       Path.DirectorySeparatorChar +
                                       "ADSetup.exe.config";
            domaininfo.ApplicationBase = System.Environment.CurrentDirectory;

            //Create evidence for the new appdomain from evidence of the current application domain
            Evidence adEvidence = AppDomain.CurrentDomain.Evidence;

            // Create appdomain
            AppDomain domain = AppDomain.CreateDomain("Domain2", adEvidence, domaininfo);

            SolutionDir = domain.BaseDirectory + "\\..\\..\\..\\CV";

            return domain;
        }

        static void Main(string[] args)
        {
            AppDomain domain = Startup();
            ObjectDetection detector = new ObjectDetection();
            detector.Load(SolutionDir.ToString());

        }
    }
}
