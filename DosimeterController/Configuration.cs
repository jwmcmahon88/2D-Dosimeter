using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DosimeterController
{
    public class Configuration
    {
        // Step sizes, fixed by the hardware configuration
        const decimal XStepsPerMM = 32;
        const decimal YStepsPerMM = 32;

        ScanOrigin origin;
        [CategoryAttribute("Geometry")]
        [DescriptionAttribute("The top-left corner of the scan area (in mm)")]
        public ScanOrigin Origin
        {
            get { return origin; }
            set
            {
                // Round to the nearest step
                var x = Math.Round(value.X * XStepsPerMM) / XStepsPerMM;
                var y = Math.Round(value.Y * YStepsPerMM) / YStepsPerMM;
                origin = new ScanOrigin { X = x, Y = y };
            }
        }

        ScanSize size;
        [Category("Geometry")]
        [Description("The size of the scan area (in mm)")]
        public ScanSize Size
        {
            get { return size; }
            set
            {
                // Round to the nearest step
                var width = Math.Round(value.Width * XStepsPerMM) / XStepsPerMM;
                var height = Math.Round(value.Height * YStepsPerMM) / YStepsPerMM;
                size = new ScanSize { Width = width, Height = height };
            }
        }

        [Category("Geometry")]
        [Description("The vertical offset of the scan head (in mm)")]
        public decimal FocusHeight { get; set; }

        decimal rowStride;
        [Category("Geometry")]
        [Description("The spacing between adjacent rows (in mm)")]
        public decimal RowStride
        {
            get { return rowStride; }
            set
            {
                // Round to the nearest step
                rowStride = Math.Round(value * YStepsPerMM) / YStepsPerMM;
            }
        }

        [Category("Geometry")]
        [Description("The horizontal speed (in mm/min)")]
        public decimal RowSpeed { get; set; }

        [Category("Geometry")]
        [Description("The spacing between adjacent columns (in mm)")]
        public decimal ColumnStride { get; private set; }

        [Category("Geometry")]
        [Description("The vertical row-change speed (in mm/min)")]
        public decimal ColumnSpeed { get; set; }

        [Category("Metadata")]
        [Description("The ID of the film being scanned")]
        public string Film { get; set; }

        [Category("Metadata")]
        [Description("The person who made the scan")]
        public string Operator { get; set; }

        [Category("Metadata")]
        [Description("A brief description of the image")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description { get; set; }

        [Description("Data file that is saved to disk")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(UITypeEditor))]
        public string DataFile { get; set; }

        public Configuration()
        {
            // Set the default values
            Origin = new ScanOrigin { X = 50, Y = 50 };
            Size = new ScanSize { Width = 100, Height = 100 };
            FocusHeight = 0;
            RowStride = 1;
            RowSpeed = 1000;
            ColumnStride = 1 / 32m;
            ColumnSpeed = 200;
            Film = "None";
            Operator = "Paul Chote";
            Description = "Test scan";
            DataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "scan.fits");
        }
    }

    [TypeConverter(typeof(ScanOriginTypeConverter))]
    public struct ScanOrigin
    {
        [Description("The left margin of the scan area (in mm)")]
        public decimal X { get; set; }

        [Description("The top margin of the scan area (in mm)")]
        public decimal Y { get; set; }
    }

    public class ScanOriginTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ScanOrigin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is ScanOrigin)
            {
                var so = (ScanOrigin)value;
                return string.Format("{0:F3}, {1:F3}", so.X, so.Y);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    var components = ((string)value).Split(',');
                    var x = decimal.Parse(components[0]);
                    var y = decimal.Parse(components[1]);

                    return new ScanOrigin { X = x, Y = y };
                }
                catch
                {
                    throw new ArgumentException("Invalid scan area'" + value + "'.");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(ScanOrigin), attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    [TypeConverter(typeof(ScanSizeTypeConverter))]
    public struct ScanSize
    {
        [Description("The width of the scan area (in mm)")]
        public decimal Width { get; set; }

        [Description("The height of the scan area (in mm)")]
        public decimal Height { get; set; }
    }

    public class ScanSizeTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ScanSize))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is ScanSize)
            {
                var so = (ScanSize)value;
                return string.Format("{0:F3}, {1:F3}", so.Width, so.Height);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    var components = ((string)value).Split(',');
                    var width = decimal.Parse(components[0]);
                    var height = decimal.Parse(components[1]);

                    return new ScanSize { Width = width, Height = height };
                }
                catch
                {
                    throw new ArgumentException("Invalid scan size'" + value + "'.");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(ScanSize), attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class SaveFileNameEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || provider == null)
                return base.EditValue(context, provider, value);

            using (var dialog = new SaveFileDialog())
            {
                if (value != null)
                {
                    var path = (string)value;
                    dialog.InitialDirectory = Path.GetDirectoryName(path);
                    dialog.FileName = Path.GetFileName(path);
                }

                dialog.Title = context.PropertyDescriptor.DisplayName;
                dialog.Filter = "Fits image (*.fits)|*.fits|Compressed fits image (*.fits.gz)|*.fits.gz";
                if (dialog.ShowDialog() == DialogResult.OK)
                    value = dialog.FileName;
            }

            return value;
        }
    }
}
