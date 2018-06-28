using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Stitching;
using Emgu.CV.Util;

namespace CV
{
    public class ImageStitching
    {
        /* Stitcher object used to stitch images: */
        private Stitcher stitcher = new Stitcher(false);

        /* Clean up: */
        public void Cleanup()
        {
            stitcher.Dispose();
        }

        /* Load captured images to be stitched: */
        public VectorOfMat Load_images(string path, int seg_R, int seg_C, string name)
        {
            var vm = new VectorOfMat();

            for (int i = 0; i < seg_R; i++)
            {
                for (int ii = 0; ii < seg_C; ii++)
                {
                    var img_to_add = CvInvoke.Imread(path + "\\" + name + i + ii + ".jpg", ImreadModes.Color);
                    vm.Push(img_to_add);
                }
            }
            return vm;
        }

        /* Attempt to stitch images together: */
        public Mat Stitch(VectorOfMat vm)
        {
            Mat result = new Mat();
            stitcher.Stitch(vm, result);

            return result;
        }

    }
}
