using System;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Resources;
using System.Collections;
using System.IO;


namespace CV
{
    public class ObjectDetection
    {
        /* Threshold value to determine if a match is found: */
        private double threshold; 

        /* Detection settings: */
        public void Init_detection(double thresh)
        {
            threshold = thresh;
        }

        /* struct to hold return information of MinMax function: */
        struct MinMax_info
        {
            public double max_val;
            public Size temp_size;
            public Point max_loc;
        };

        /* Load templates into template list: */
        public List<Mat> Load_tempaltes(string temp_path, string temp_name, int img_count, int img_count_start)
        {
            List<Mat> templates = new List<Mat>();

            for (int i = img_count_start; i < img_count; i++)
            {
                var filename = temp_name + i + ".jpg";
                var temp_to_add = CvInvoke.Imread(temp_path + "/" + filename, ImreadModes.Color);
                templates.Add(temp_to_add);
            }
            return templates;
        }

        public void Load(string SolutionDir)
        {
            List<Mat> templates = new List<Mat>();

            var file_path = Path.Combine(SolutionDir, Data.Constants.template_file_name);
            var img = CvInvoke.Imread(file_path, ImreadModes.AnyColor);
           
            

            CvInvoke.Imshow("img", img);
            CvInvoke.WaitKey();
            CvInvoke.DestroyAllWindows();

        }

        /* Match templates to given image, if defects found then return 1, else return 0: */
        public int Match_templates(Mat image, List<Mat> templates)
        {
            Image<Bgr, Byte> img = image.ToImage<Bgr, Byte>();
            Image<Gray, Byte> img_mask = new Image<Gray, Byte>(image.Size.Width, image.Size.Height);

            Mat res = new Mat();
            List<MinMax_info> found = new List<MinMax_info>();
            double min_val = 0;
            double max_val = 0;
            Point min_loc = new Point();
            Point max_loc = new Point();

            foreach (Mat temp in templates)
            {
                CvInvoke.MatchTemplate(image, temp, res, TemplateMatchingType.CcoeffNormed);
                CvInvoke.Threshold(res, res, threshold, 255, ThresholdType.ToZero);
                while (true)
                {
                    CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc, null);
                    if (max_val >= threshold)
                    {
                        var MinMax_to_add = new MinMax_info
                        {
                            max_val = max_val,
                            temp_size = temp.Size,
                            max_loc = max_loc
                        };
                        found.Add(MinMax_to_add);
                        var box_size = new Size
                        {
                            Width = Convert.ToInt32(Math.Round(temp.Size.Width / 1.5)),
                            Height = Convert.ToInt32(Math.Round(temp.Size.Height / 1.5))
                        };

                        Rectangle img_box = new Rectangle(max_loc, box_size);
                        CvInvoke.Rectangle(img, img_box, new MCvScalar(0, 0, 255), 2, LineType.EightConnected, 0);
                        CvInvoke.Rectangle(res, img_box, new MCvScalar(0, 0, 255), -1, LineType.EightConnected, 0);
                        CvInvoke.Rectangle(img_mask, img_box, new MCvScalar(255, 255, 255), -1, LineType.EightConnected, 0);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            CvInvoke.NamedWindow("Image", NamedWindowType.Normal);
            CvInvoke.Imshow("Image", img);
            CvInvoke.WaitKey();
            CvInvoke.DestroyAllWindows();
            CvInvoke.NamedWindow("Image", NamedWindowType.Normal);
            CvInvoke.Imshow("Image", img_mask);
            CvInvoke.WaitKey();
            CvInvoke.DestroyAllWindows();
            Console.WriteLine(found.Count);







            return 0;
        }

    }
}
