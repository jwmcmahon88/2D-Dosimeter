
using System;
using System.Runtime.InteropServices;

namespace DosimeterController
{
    public class MiniFitsException : Exception
    {
        public MiniFitsException() { }
        public MiniFitsException(string message) : base(message) { }
        public MiniFitsException(string message, Exception inner) : base(message, inner) { }
    }

    public class MiniFits : IDisposable
    {
        internal const string FITS_DLL = "cfitsio.dll";
        internal const CallingConvention FITS_CALLING_CONVENTION = CallingConvention.Cdecl;

        [DllImport(FITS_DLL, EntryPoint = "ffinit", CallingConvention = FITS_CALLING_CONVENTION)]
        public static extern int CreateFile(ref IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string filename, ref int status);
    
        [DllImport(FITS_DLL, EntryPoint = "ffcrim", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int CreateImage(IntPtr fptr, int bitpix, int naxis, int[] naxes, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffuky", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, int datatype, [MarshalAs(UnmanagedType.LPStr)] string keyname, [MarshalAs(UnmanagedType.LPStr)] string value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffppr", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int WriteImage(IntPtr fptr, int datatype, long firstelem, long nelem, ushort[] array, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffclos", CallingConvention = FITS_CALLING_CONVENTION)]
        public static extern int CloseFile(IntPtr fptr, ref int status);

        readonly IntPtr fptr;
        readonly int width, height;
        readonly ushort[] data;
        int status;

        public MiniFits(string filename, int width, int height, bool overwrite)
        {
            if (overwrite)
                filename = "!" + filename;

            if (MiniFits.CreateFile(ref fptr, filename, ref status) != 0)
                throw new MiniFitsException("Failed to create file");

            if (MiniFits.CreateImage(fptr, 20, 2, new int[] { width, height }, ref status) != 0)
                throw new MiniFitsException("Failed to create image");

            data = new ushort[width * height];
            this.width = width;
            this.height = height;
        }

        public void UpdateKey(string key, string value, string comment)
        {
            UpdateKey(fptr, 16, key, value, comment, ref status);
            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        public void SetImageRow(int row, ushort[] rowData)
        {
            if (rowData.Length != width)
                throw new MiniFitsException(string.Format("rowData length ({0}) doesn't match image width ({1})", rowData.Length, width));

            if (row < 0 || row >= height)
                throw new MiniFitsException("row is outside the image");

            Array.Copy(rowData, 0, data, row * width, width);

            if (WriteImage(fptr, 20, 1, data.Length, data, ref status) != 0)
                throw new MiniFitsException("Failed to update data");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CloseFile(fptr, ref status);
        }
    }
}
