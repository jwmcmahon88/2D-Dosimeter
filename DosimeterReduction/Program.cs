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

            var inWidth = frame.Dimensions[0];
            var inHeight = frame.Dimensions[1];
            var binning = 16;
            var outWidth = inWidth / binning;
            var outHeight = inHeight;

            var binnedFile = Path.GetDirectoryName(dataFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(dataFile) + ".binned.fits";
            var binned = new MiniFits(binnedFile, new[] { outWidth, outHeight }, MiniFitsType.F64, true);

            var data = frame.ReadU16ImageData();
            var binnedData = new double[outWidth * outHeight];
            for (var i = 0; i < outHeight; i++)
            {
                Console.WriteLine("\nRow {0}", i);
                for (var j = 0; j < outWidth; j++)
                {
                    var primaryOffset = inWidth * i + binning * j;
                    var secondaryOffset = primaryOffset + inWidth * inHeight;

                    var primaryBinned = 0.0;
                    var secondaryBinned = 0.0;
                    for (var k = 0; k < binning; k++)
                    {
                        primaryBinned += data[primaryOffset + k];
                        secondaryBinned += data[secondaryOffset + k];
                    }

                    var outIndex = i * outWidth + j;
                    binnedData[outIndex] = primaryBinned + secondaryBinned;
                }
            }

            binned.WriteImageData(binnedData);
        }
    }
}
