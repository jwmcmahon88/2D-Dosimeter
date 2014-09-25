using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DosimeterController;
using System.IO;

namespace DosimeterReduction
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataFile = args[0];
            var frame = new MiniFits(dataFile, false);

            //var scanArea = frame.GetStringKey("SCANAREA");
            //var rowStart = frame.GetIntKey("ROWSTART");
            //var rowStride = frame.GetDecimalKey("ROWSTRID");
            //var colStride = frame.GetDecimalKey("COLSTRID");

            var outWidth = frame.Width / 32;
            var outHeight = frame.Height;

            var binnedFile = Path.GetDirectoryName(dataFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(dataFile) + ".binned.fits";
            var binned = new MiniFits(binnedFile, outWidth, outHeight, 1, MiniFitsType.F64, true);

            var data = frame.ReadU16ImageData();
            var binnedData = new double[outWidth * outHeight];

            for (var i = 0; i < frame.Height; i++)
            {
                for (var j = 0; j < outWidth; j++)
                {
                    var primaryOffset = frame.Width * i + 32 * j;
                    var secondaryOffset = primaryOffset + frame.Width * frame.Height;

                    var primaryBinned = 0.0;
                    var secondaryBinned = 0.0;
                    for (var k = 0; k < 32; k++)
                    {
                        primaryBinned += data[primaryOffset + k];
                        secondaryBinned += data[secondaryOffset + k];
                    }

                    binnedData[i * outWidth + j] = primaryBinned / secondaryBinned;
                }
            }

            binned.WriteImageData(binnedData);
        }
    }
}
