using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DosimeterController;

namespace DosimeterReduction
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataFile = args[0];
            var frame = new MiniFits(dataFile, false);
            var rowStride = frame.ReadDecimalKey("ROWSTRID");
            var colStride = frame.ReadDecimalKey("COLSTRID");

            var region = frame.ReadStringKey("SCANAREA")
                .Split(' ')
                .Select(x => decimal.Parse(x))
                .ToArray();

            var inWidth = frame.Dimensions[0];
            var inHeight = frame.Dimensions[1];

            var binning = 8;
            var outWidth = inWidth / binning;
            var outHeight = inHeight;

            var binnedFile = Path.GetDirectoryName(dataFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(dataFile) + ".binned.fits";
            
            using (var binned = new MiniFits(binnedFile, new[] { outWidth, outHeight }, MiniFitsType.F64, true))
            {
                // Define the image coordinates for the display program
                // Physical coordinates
                binned.WriteKey("LTM1_1", 1 / (colStride * binning), 6);
                binned.WriteKey("LTM1_2", 0);
                binned.WriteKey("LTM2_1", 0);
                binned.WriteKey("LTM2_2", 1 / rowStride, 6);

                binned.WriteKey("LTV1", -region[0] / (colStride * binning) - 0.5m, 6);
                binned.WriteKey("LTV2", -region[1] / rowStride - 0.5m, 6);

                // WCS coordinates
                binned.WriteKey("CTYPE1", "LINEAR");
                binned.WriteKey("CUNIT1", "mm");
                binned.WriteKey("CRPIX1", 0);
                binned.WriteKey("CRVAL1", region[0] + colStride * binning / 2, 6);
                binned.WriteKey("CDELT1", colStride * binning, 6);

                binned.WriteKey("CTYPE2", "LINEAR");
                binned.WriteKey("CUNIT2", "mm");
                binned.WriteKey("CRPIX2", 0);
                binned.WriteKey("CRVAL2", region[1] + rowStride / 2, 6);
                binned.WriteKey("CDELT2", rowStride, 6);

                binned.WriteKey("WCSNAME", "Scan Pos.");

                var data = frame.ReadU16ImageData();
                var binnedData = new double[outWidth * outHeight];
                for (var i = 0; i < outHeight; i++)
                {
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
}
