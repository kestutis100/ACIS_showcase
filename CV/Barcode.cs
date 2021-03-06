﻿using DataMatrix.net;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace CV
{
    public class Barcode
    {
        /* object that is used to decode Dmtx */
        private DmtxImageDecoder decoder = new DmtxImageDecoder();

        /* struct to hold return information of MinMax function: */
        struct MinMax_info
        {
            public double max_val;
            public Point top_left;
            public Point bottom_right;
            public bool is_none;
        };

        /* Holds state of last scanned barcode: */
        private MinMax_info barcode_info;

        /* Used to calculate lineraly spaced steps: */
        static double Linspace_step(double x1, double x2, int num)
        {
            double step = (x2 - x1) / (num - 1);
            return step;
        }

        /* Crops Image based on ROI. Returns binary-threshold cropped Image: */
        static Mat Crop_image(Mat input, Rectangle crop_region)
        {
            Image<Gray, Byte> buffer_im = input.ToImage<Gray, byte>();
            buffer_im.ROI = crop_region;
            Image<Gray, byte> cropped_im = buffer_im.Copy();
            CvInvoke.Threshold(cropped_im, cropped_im, 0, 230, ThresholdType.BinaryInv | ThresholdType.Otsu);
            return cropped_im.Mat;
        }

        /* Finds barcode in a given Image:
        * Returns Mat containing barcode only. */
        private Mat Find_barcode(Mat img)
        {
            /*  Load in templates: */
            List<Mat> template = new List<Mat>();
            Image<Bgr, Byte> imgCV = new Image<Bgr, byte>(Properties.Resources.DMTX);
            Mat imgMAT = imgCV.Mat;
            template.Add(imgMAT);

            var resized = new Mat();
            var res = new Mat();
            /* Barcode location Information initilization: */
            MinMax_info found;
            found.is_none = true;
            found.max_val = 0;
            found.bottom_right = new Point(0, 0);
            found.top_left = new Point(0, 0);
            /* set up linspace loop for templates scaling: */
            double start_val = 1;
            double stop_val = 4;
            double step_val = Linspace_step(start_val, stop_val, 10);
            double min_val = 0; double max_val = 0; Point min_loc = new Point(); Point max_loc = new Point();
            /* Loop through scale increments to find barcode location: */
            for (int i = 0; i < template.Count; i++)
            {
                for (double scale = start_val; scale < stop_val; scale += step_val)
                {
                    CvInvoke.Resize(template[i], resized, new System.Drawing.Size(Convert.ToInt32(scale * template[i].Size.Width), Convert.ToInt32(scale * template[i].Size.Height)));
                    if (template[i].Size.Height < img.Size.Height && template[i].Size.Width < img.Size.Width)
                    {
                        CvInvoke.MatchTemplate(img, resized, res, TemplateMatchingType.CcoeffNormed);
                        CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc, null);
                        if (found.is_none == true || max_val > found.max_val)
                        {
                            found.is_none = false;
                            found.max_val = max_val;
                            found.top_left = max_loc;
                            found.bottom_right.X = max_loc.X + resized.Size.Width;
                            found.bottom_right.Y = max_loc.Y + resized.Size.Height;
                        }
                    }
                }
            }
            int offset = 0;       //offset to add padding to image, or reduce padding.
            Rectangle img_box = new Rectangle(found.top_left.X - offset, found.top_left.Y - offset, found.bottom_right.X - found.top_left.X + offset, found.bottom_right.Y - found.top_left.Y + offset);
            barcode_info = found;
            return Crop_image(img, img_box);
        }

        /* Decodes given barcode Image:
         * Returns decoded string if barcode can be read,
         * Otherwise returns empty string.*/
        private string Barcode_decoder(Mat img)
        {
            var list = decoder.DecodeImage(img.Bitmap, 1, new TimeSpan(0, 0, 1));
            if (list.Count > 0)
                return list[0];
            else
                return "";
        }

        /* Finds and decodes dmtx of given stitched image.
         * Returns decoded string on sucesses or empy string on failure: */
        public string Barcode_finding_run(Mat img)
        {
            /* Image of barcode: */
            var barcode = new Mat();
            string barcode_string = "";
            int offset = 0;

            barcode_string = Barcode_decoder(img);

            if (barcode_string == "")
            {
                barcode = Find_barcode(img);
            }

            while (barcode_string == "" && offset < 40)
            {
                barcode_string = Barcode_decoder(barcode);
                Rectangle img_box = new Rectangle(barcode_info.top_left.X - offset, barcode_info.top_left.Y - offset, barcode_info.bottom_right.X - barcode_info.top_left.X + offset, barcode_info.bottom_right.Y - barcode_info.top_left.Y + offset);
               // CvInvoke.Rectangle(img, img_box, new MCvScalar(0, 0, 255), 2, LineType.EightConnected, 0);        //Draws rectangle around barcode, good for debugging.

                offset += 10;
            }
            return barcode_string;
        }
    }
}
