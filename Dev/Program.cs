using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV;
using ACIS_Showcase;
using System.IO;
using System.Security.Policy;
using Emgu.CV;
using Emgu.CV.CvEnum;

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

            SolutionDir = domain.BaseDirectory + "\\..\\..\\..\\Images\\";

            return domain;
        }

        static void Main(string[] args)
        {
            var templates = new List<Mat>();
            AppDomain domain = Startup();
            ObjectDetection detector = new ObjectDetection();
            var img_path = Path.Combine(SolutionDir.ToString(), Data.Constants.filename_bot + ".jpg");

            var img = CvInvoke.Imread(img_path, ImreadModes.AnyColor);
            templates = detector.Load_tempaltes(SolutionDir.ToString());
            detector.Init_detection(0.96);
            detector.Match_templates(img, templates);
        }
    }
}
