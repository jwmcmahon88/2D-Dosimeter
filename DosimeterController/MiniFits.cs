
using System;
using System.Runtime.InteropServices;

namespace DosimeterController
{
    public enum MiniFitsType { U16, F64 }
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

        [DllImport(FITS_DLL, EntryPoint = "ffiopn", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int OpenImage(ref IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string filename, int iomode, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffinit", CallingConvention = FITS_CALLING_CONVENTION)]
        public static extern int CreateFile(ref IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string filename, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffcrim", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int CreateImage(IntPtr fptr, int bitpix, int naxis, int[] naxes, ref int status);

        // String
        [DllImport(FITS_DLL, EntryPoint = "ffgkls", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int ReadKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] ref string value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffukls", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, [MarshalAs(UnmanagedType.LPStr)] string value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        // Integer
        [DllImport(FITS_DLL, EntryPoint = "ffgkyj", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int ReadKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, ref long value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffukyj", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, long value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        // Floating point
        [DllImport(FITS_DLL, EntryPoint = "ffgkyd", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int ReadKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, ref double value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffukyg", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, double value, int places, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        // Image data
        [DllImport(FITS_DLL, EntryPoint = "ffgpxv", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int ReadImage(IntPtr fptr, int datatype, int[] firstpix, long nelem, IntPtr nulval, ushort[] array, ref int anynul, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffppr", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int WriteImage(IntPtr fptr, int datatype, long firstelem, long nelem, ushort[] array, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffppr", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int WriteImage(IntPtr fptr, int datatype, long firstelem, long nelem, double[] array, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffclos", CallingConvention = FITS_CALLING_CONVENTION)]
        public static extern int CloseFile(IntPtr fptr, ref int status);

        readonly IntPtr fptr;
        readonly int width, height, planes;
        readonly MiniFitsType type;

        int status;

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int Planes { get { return planes; } }

        /// <summary>Create a new frame</summary>
        public MiniFits(string filename, int width, int height, int planes, MiniFitsType type, bool overwrite)
        {
            if (overwrite)
                filename = "!" + filename;

            if (MiniFits.CreateFile(ref fptr, filename, ref status) != 0)
                throw new MiniFitsException("Failed to create file");

            if (MiniFits.CreateImage(fptr, type == MiniFitsType.F64 ? -64 : 20, 3, new int[] { width, height, planes }, ref status) != 0)
                throw new MiniFitsException("Failed to create image");

            this.width = width;
            this.height = height;
            this.planes = planes;
            this.type = type;
        }

        /// <summary>Load an existing frame</summary>
        public MiniFits(string filename, bool readwrite)
        {
            if (MiniFits.OpenImage(ref fptr, filename, readwrite ? 1 : 0, ref status) != 0)
                throw new MiniFitsException("Failed to open image");

            this.width = ReadIntegerKey("NAXIS1");
            this.height = ReadIntegerKey("NAXIS2");
            this.planes = ReadIntegerKey("NAXIS3");
        }

        /// <summary>Add or update a header keyword.</summary>
        public void WriteKey(string key, string value, string comment)
        {
            UpdateKey(fptr, key, value, comment, ref status);
            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        /// <summary>Add or update a header keyword.</summary>
        public void WriteKey(string key, int value, string comment)
        {
            UpdateKey(fptr, key, value, comment, ref status);

            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        /// <summary>Add or update a header keyword.</summary>
        public void WriteKey(string key, decimal value, int significantFigures, string comment)
        {
           var val = (double)value;
           UpdateKey(fptr, key, val, significantFigures - 1, comment, ref status);

            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        public string ReadStringKey(string key)
        {
            string value = "";
            if (MiniFits.ReadKey(fptr, key, ref value, null, ref status) != 0)
                throw new MiniFitsException("Failed to read key");

            return value;
        }

        public int ReadIntegerKey(string key)
        {
            long value = 0;
            if (MiniFits.ReadKey(fptr, key, ref value, null, ref status) != 0)
                throw new MiniFitsException("Failed to read key");

            return (int)value;
        }

        public decimal ReadDecimalKey(string key)
        {
            double value = 0;
            if (MiniFits.ReadKey(fptr, key, ref value, null, ref status) != 0)
                throw new MiniFitsException("Failed to read key");

            return (decimal)value;
        }

        public void WriteImageData(double[] imageData)
        {
            if (type != MiniFitsType.F64)
                throw new MiniFitsException("Attempting to write double precision data to an integer image");

            if (imageData.Length != width * height * planes)
                throw new MiniFitsException("Data length doesn't match frame geometry");

            if (WriteImage(fptr, 82, 1, imageData.Length, imageData, ref status) != 0)
                throw new MiniFitsException(string.Format("Failed to update data with error {0}", status));
        }

        public void WriteImageData(ushort[] imageData)
        {
            if (type != MiniFitsType.U16)
                throw new MiniFitsException("Attempting to write ushort data to a double image");

            if (imageData.Length != width * height * planes)
                throw new MiniFitsException("Data length doesn't match frame geometry");

            if (WriteImage(fptr, 20, 1, imageData.Length, imageData, ref status) != 0)
                throw new MiniFitsException(string.Format("Failed to update data with error {0}", status));
        }

        public ushort[] ReadU16ImageData()
        {
            if (type != MiniFitsType.U16)
                throw new MiniFitsException("Attempting to read ushort data from a double image");

            var data = new ushort[width * height * planes];
            int anynul = 0;
            if (MiniFits.ReadImage(fptr, 20, new int[] { 1, 1, 1 }, width * height * planes, IntPtr.Zero, data, ref anynul, ref status) != 0)
                throw new MiniFitsException("Failed to read pixel data");

            return data;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CloseFile(fptr, ref status);
        }
    }
}
