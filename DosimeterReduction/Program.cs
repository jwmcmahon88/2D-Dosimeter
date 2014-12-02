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

            var primaryDark = 0.0;
            var secondaryDark = 0.0;

            try
            {
                // Older frames don't specify dark counts, so let these gracefully fail
                primaryDark = (double)frame.ReadDecimalKey("DARKCNTA");
                secondaryDark = (double)frame.ReadDecimalKey("DARKCNTB");
            }
            catch (Exception)
            {
                frame.ClearErrorStatus();
            }

            var region = frame.ReadStringKey("SCANAREA")
                .Split(' ')
                .Select(x => decimal.Parse(x))
                .ToArray();

            
            var overscanColumns = 0;
            try
            {
                // Older frames don't specify the overscan
                overscanColumns = frame.ReadIntegerKey("OVERSCAN");
            }
            catch (Exception)
            {
                frame.ClearErrorStatus();
            }

            Console.WriteLine("Overscan columns: {0}", overscanColumns);

            var inWidth = frame.Dimensions[0];
            var inHeight = frame.Dimensions[1];

            var binning = (int)Math.Round(rowStride / colStride);
            if (binning <= 0)
                throw new InvalidOperationException("Invalid row or column stride");

            var outWidth = (inWidth - 2 * overscanColumns) / binning;
            var outHeight = inHeight;

            var binnedFile = Path.GetDirectoryName(dataFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(dataFile) + ".binned.fits";
            
            using (var binned = new MiniFits(binnedFile, new[] { outWidth, outHeight, 3 }, MiniFitsType.F64, true))
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
                var frameStride = outWidth * outHeight;
                var binnedData = new double[frameStride * 3];
                for (var i = 0; i < outHeight; i++)
                {
                    for (var j = 0; j < outWidth; j++)
                    {
                        var primaryOffset = inWidth * i + binning * j + overscanColumns;
                        var secondaryOffset = primaryOffset + inWidth * inHeight;

                        var primaryBinned = 0.0;
                        var secondaryBinned = 0.0;
                        for (var k = 0; k < binning; k++)
                        {
                            primaryBinned += data[primaryOffset + k] - primaryDark;
                            secondaryBinned += data[secondaryOffset + k] - secondaryDark;
                        }

                        var outIndex = i * outWidth + j;
                        binnedData[outIndex] = primaryBinned / secondaryBinned;
                        binnedData[outIndex + frameStride] = primaryBinned;
                        binnedData[outIndex + 2 * frameStride] = secondaryBinned;
                    }
                }

                binned.WriteImageData(binnedData);
            }
        }
    }
}
