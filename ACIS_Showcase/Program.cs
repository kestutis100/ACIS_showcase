using System;
using System.IO;
using CV;
using System.Security.Policy;
using System.Xml.Linq;


namespace ACIS_Showcase
{
    class Program
    {
        /* Solution Directory for file IO: */
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

        /* Clean up objects: */
        static void Cleanup(AppDomain domain, ImageStitching stitcher)
        {
            AppDomain.Unload(domain);
            stitcher.Cleanup();
        }

        static void Main(string[] args)
        {
            AppDomain domain = Startup();
            ImageStitching stitcher = new ImageStitching();
            Barcode decoder = new Barcode();
            ObjectDetection detector = new ObjectDetection();
            string cpu_folder_name;

            /* Stitch Image and decode barcode: */
            var img_list = stitcher.Load_images(SolutionDir.ToString(), Data.Constants.Seg_R, Data.Constants.Seg_R, Data.Constants.filename_top);
            var img = stitcher.Stitch(img_list);
            var barcode = decoder.Barcode_finding_run(img);

            /* Create file and save Image: */
            cpu_folder_name = "ACIS_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            string save_path = SolutionDir.ToString()+"\\"+cpu_folder_name;
            Directory.CreateDirectory(Path.Combine(SolutionDir.ToString(), cpu_folder_name));
            String fileName = "BarcodeInfo_" + Data.Constants.filename_top  + ".xml";
            new XDocument(new XElement("ACIS", new XElement("barcode", barcode))).Save(Path.Combine(save_path, fileName));
            img.Save(Path.Combine(save_path, Data.Constants.filename_top + ".jpg"));
       
            /* Defect Detection: */

            



            Cleanup(domain, stitcher);
            Console.WriteLine("\n\n");
            Console.WriteLine("The stitched image and xml file holding the barcode has been saved");
            Console.WriteLine("The save location is in the ACIS_Showcase solution directory under Images");
            Console.WriteLine("Defect Detection Will be added shortly.");
            Console.WriteLine("Thank you. Hit Enter to continue:");
            Console.ReadLine();
        }

    }
}
