using System;
using System.Linq;
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
        static extern int ReadKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, ref IntPtr value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "fffree", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int FreeFitsBuffer(IntPtr ptr, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffukls", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, [MarshalAs(UnmanagedType.LPStr)] string value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        // Integer
        [DllImport(FITS_DLL, EntryPoint = "ffgkyj", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int ReadKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, ref int value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffukyj", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, long value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        // Floating point
        [DllImport(FITS_DLL, EntryPoint = "ffgkyd", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int ReadKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, ref double value, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffukyg", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern int UpdateKey(IntPtr fptr, [MarshalAs(UnmanagedType.LPStr)] string keyname, double value, int places, [MarshalAs(UnmanagedType.LPStr)] string comm, ref int status);

        // Image data
        [DllImport(FITS_DLL, EntryPoint = "ffgpxv", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern unsafe int ReadImage(IntPtr fptr, int datatype, int[] firstpix, long nelem, IntPtr nulval, void* array, ref int anynul, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffppr", CallingConvention = FITS_CALLING_CONVENTION)]
        static extern unsafe int WriteImage(IntPtr fptr, int datatype, long firstelem, long nelem, void* array, ref int status);

        [DllImport(FITS_DLL, EntryPoint = "ffclos", CallingConvention = FITS_CALLING_CONVENTION)]
        public static extern int CloseFile(IntPtr fptr, ref int status);

        readonly IntPtr fptr;
        readonly int[] dimensions;
        readonly MiniFitsType type;

        int status;

        public int[] Dimensions { get { return dimensions; } }

        /// <summary>Create a new frame</summary>
        public MiniFits(string filename, int[] dimensions, MiniFitsType type, bool overwrite)
        {
            if (overwrite)
                filename = "!" + filename;

            if (MiniFits.CreateFile(ref fptr, filename, ref status) != 0)
                throw new MiniFitsException("Failed to create file");

            if (MiniFits.CreateImage(fptr, type == MiniFitsType.F64 ? -64 : 20, dimensions.Length, dimensions, ref status) != 0)
                throw new MiniFitsException("Failed to create image");

            this.dimensions = dimensions;
            this.type = type;
        }

        /// <summary>Load an existing frame</summary>
        public MiniFits(string filename, bool readwrite)
        {
            if (MiniFits.OpenImage(ref fptr, filename, readwrite ? 1 : 0, ref status) != 0)
                throw new MiniFitsException("Failed to open image");

            dimensions = new[]
            {
                ReadIntegerKey("NAXIS1"),
                ReadIntegerKey("NAXIS2"),
                ReadIntegerKey("NAXIS3")
            };
        }

        /// <summary>Add or update a header keyword.</summary>
        public void WriteKey(string key, string value, string comment = null)
        {
            UpdateKey(fptr, key, value, comment, ref status);
            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        /// <summary>Add or update a header keyword.</summary>
        public void WriteKey(string key, int value, string comment = null)
        {
            UpdateKey(fptr, key, value, comment, ref status);

            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        /// <summary>Add or update a header keyword.</summary>
        public void WriteKey(string key, decimal value, int significantFigures, string comment = null)
        {
           var val = (double)value;

           UpdateKey(fptr, key, val, significantFigures - 1, comment, ref status);

            if (status != 0)
                throw new MiniFitsException("Failed to update key");
        }

        public string ReadStringKey(string key)
        {
            var ptr = IntPtr.Zero;
            if (MiniFits.ReadKey(fptr, key, ref ptr, null, ref status) != 0)
                throw new MiniFitsException("Failed to read key");

            var value = Marshal.PtrToStringAnsi(ptr);
            FreeFitsBuffer(ptr, ref status);
            return value;
        }

        public int ReadIntegerKey(string key)
        {
            int value = 0;
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

            var elements = dimensions.Aggregate(1, (a, b) => a * b);
            if (imageData.Length != elements)
                throw new MiniFitsException("Data length doesn't match frame geometry");

            unsafe
            {
                fixed (double* data = imageData)
                if (WriteImage(fptr, 82, 1, elements, (void*)data, ref status) != 0)
                    throw new MiniFitsException(string.Format("Failed to update data with error {0}", status));
            }
        }

        public void WriteImageData(ushort[] imageData)
        {
            if (type != MiniFitsType.U16)
                throw new MiniFitsException("Attempting to write ushort data to a double image");

            var elements = dimensions.Aggregate(1, (a, b) => a * b);
            if (imageData.Length != elements)
                throw new MiniFitsException("Data length doesn't match frame geometry");

            unsafe
            {
                fixed (ushort* data = imageData)
                if (WriteImage(fptr, 20, 1, elements, (void*)data, ref status) != 0)
                    throw new MiniFitsException(string.Format("Failed to update data with error {0}", status));
            }
        }

        public ushort[] ReadU16ImageData()
        {
            if (type != MiniFitsType.U16)
                throw new MiniFitsException("Attempting to read ushort data from a double image");

            var elements = dimensions.Aggregate(1, (a, b) => a * b);
            var imageData = new ushort[elements];
            int anynul = 0;

            unsafe
            {
                fixed (ushort* data = imageData)
                if (MiniFits.ReadImage(fptr, 20, new int[] { 1, 1, 1 }, elements, IntPtr.Zero, data, ref anynul, ref status) != 0)
                  throw new MiniFitsException("Failed to read pixel data");
            }

            return imageData;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CloseFile(fptr, ref status);
        }
    }
}
