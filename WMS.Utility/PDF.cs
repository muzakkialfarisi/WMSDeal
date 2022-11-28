using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.BarCodes;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;

namespace WMS.Utility
{
    public class PDF
    {
        XTextFormatter tf;
        XRect rect;
        public const string _DEFAULT_FONT = "Arial";

        public static void setImage(XGraphics Gfx, string path, double xPos, double yPos, double width, double height)
        {
            string imagex = path;
            XImage image = XImage.FromFile(imagex);
            Gfx.DrawImage(image, xPos, yPos, width, height);
        }

        public static void setLine(XGraphics _gfx, double xPos, double yPos, double width)
        {
            var pen = new XPen(XColors.Black, 0.5d);
            _gfx.DrawLine(pen, xPos, yPos, xPos + width, yPos);
        }

        public static void setDashLine(XGraphics _gfx, double xPos, double yPos, double width)
        {
            var pen = new XPen(XColors.Black, 1);
            pen.DashStyle = XDashStyle.Dash;
            _gfx.DrawLine(pen, xPos, yPos, xPos + width, yPos);
        }

        public static void setvLine(XGraphics _gfx, double xPos, double yPos, double height)
        {
            var pen = new XPen(XColors.Black, 0.5d);
            _gfx.DrawLine(pen, xPos, yPos, xPos, yPos + height);
        }
        public static Code3of9Standard xBarcode39s(string _Text, double _Width, double _Height)
        {
            var bc = new Code3of9Standard();
            bc.Text = _Text;
            bc.Size = new XSize(_Width, _Height);
            return bc;
        }

        public static Code2of5Interleaved xBarcode25i(string _Text, double _Width, double _Height)
        {
            var bc = new Code2of5Interleaved();
            bc.Text = _Text;
            bc.Size = new XSize(_Width, _Height);
            return bc;
        }

        public static double Barcode39(XGraphics _gfx, string _content, double _x, double _y, double _width, double _height, int _FontSize)
        {
            _gfx.DrawBarCode(PDF.xBarcode39s(_content, _width, _height), new XPoint(_x, _y));
            writeRightText(_gfx, _content, DefaultFont(_FontSize), _x, _y + _height, _width);
            return _height + 15d;
        }

        public static double Barcode39Loan(XGraphics _gfx, string _content, double _x, double _y, double _width, double _height, int _FontSize)
        {
            _gfx.DrawBarCode(PDF.xBarcode39s(_content, _width, _height), new XPoint(_x, _y));
            writeRightText(_gfx, _content.Insert(_content.Length - 1, "-"), DefaultFont(_FontSize), _x, _y + _height, _width);
            return _height + 15d;
        }

        public static double Barcode25(XGraphics _gfx, string _content, double _x, double _y, double _width, double _height, int _FontSize)
        {
            _gfx.DrawBarCode(PDF.xBarcode25i(_content, _width, _height), new XPoint(_x, _y));
            writeRightText(_gfx, _content, DefaultFont(_FontSize), _x, _y + _height, _width);
            return _height + 15d;
        }

        public static XFont myFont(string _FontName, float _Size)
        {
            var sFont = new XFont(_FontName, _Size, XFontStyle.Regular);
            return sFont;
        }
        public static XFont ArialFont(double _Size)
        {
            return new XFont("Arial", _Size, XFontStyle.Regular);
        }
        public static XFont ArialFontBold(double _Size)
        {
            return new XFont("Arial", _Size, XFontStyle.Bold);
        }
        public static XFont ArialFontItalic(double _Size)
        {
            return new XFont("Arial", _Size, XFontStyle.Italic);
        }
        public static XFont ArialFontBoldItalic(double _Size)
        {
            return new XFont("Arial", _Size, XFontStyle.BoldItalic);
        }
        public static XFont DefaultFont(double _Size)
        {
            return new XFont(_DEFAULT_FONT, _Size, XFontStyle.Regular);
        }
        public static XFont DefaultFontBold(double _Size)
        {
            return new XFont(_DEFAULT_FONT, _Size, XFontStyle.Bold);
        }
        public static XFont DefaultFontItalic(double _Size)
        {
            return new XFont(_DEFAULT_FONT, _Size, XFontStyle.Italic);
        }
        public static XFont DefaultFontBoldItalic(double _Size)
        {
            return new XFont(_DEFAULT_FONT, _Size, XFontStyle.BoldItalic);
        }
        public static XFont CustomFont(string _FontName, double _Size, XFontStyle _Style = XFontStyle.Regular)
        {
            return new XFont(_FontName, _Size, _Style);
        }

        public static XStringFormat xCenter()
        {
            var xFormat = new XStringFormat();
            xFormat.Alignment = XStringAlignment.Center;
            xFormat.LineAlignment = XLineAlignment.Center;
            return xFormat;
        }
        public static XStringFormat xLeft()
        {
            var xFormat = new XStringFormat();
            xFormat.Alignment = XStringAlignment.Near;
            xFormat.LineAlignment = XLineAlignment.Center;
            return xFormat;
        }
        public static XStringFormat xRight()
        {
            var xFormat = new XStringFormat();
            xFormat.Alignment = XStringAlignment.Far;
            xFormat.LineAlignment = XLineAlignment.Center;
            return xFormat;
        }

        public static double miletoPoint(double mm)
        {
            return mm * 2.83464567d;
        }
        public static double inchtoPoint(double inc)
        {
            return inc * 72d;
        }
        public static double point(double percentation)
        {
            return XUnit.FromPresentation(percentation).Point;
        }

        public static double writeText(XGraphics _gfx, string _Teks, XFont _Font, double _x, double _y, double _width, double _height = 0d)
        {
            double _h;
            if (_height == 0d)
            {
                _h = _Font.Height;
            }
            else
            {
                _h = _height;
            }
            _gfx.DrawString(_Teks, _Font, XBrushes.Black, new XRect((float)_x, (float)_y, (float)_width, (float)_h), XStringFormats.TopLeft);
            return _h + 5;
        }
        public static double writeCenterText(XGraphics _gfx, string _Teks, XFont _Font, double _x, double _y, double _width, double _height = 0d)
        {
            double _h;
            if (_height == 0d)
            {
                _h = _Font.Height;
            }
            else
            {
                _h = _height;
            }
            _gfx.DrawString(_Teks, _Font, XBrushes.Black, new XRect((float)_x, (float)_y, (float)_width, (float)_h), XStringFormats.Center);
            return _h + 5;
        }
        public static double writeRightText(XGraphics _gfx, string _Teks, XFont _Font, double _x, double _y, double _width, double _height = 0d)
        {
            double _h;
            if (_height == 0d)
            {
                _h = _Font.Height;
            }
            else
            {
                _h = _height;
            }
            var rect = new XRect(_x, _y, _width, _h);
            _gfx.DrawString(_Teks, _Font, XBrushes.Black, rect, XStringFormats.CenterRight);
            return _h + point(5);
        }

        public static double writeWrapTextCentre(XGraphics _gfx, string _Teks, XFont _Font, double _x, double _y, double _width, double _height = 0d)
        {
            double _h;

            if (_height == 0d)
            {
                _h = _Font.Height;
            }
            else
            {
                _h = _height;
            }

            var tf = new XTextFormatter(_gfx);
            var rect = new XRect(_x, _y, _width, _h);
            tf.Alignment = XParagraphAlignment.Center;
            _Teks = _Teks.Replace("/", " /").Replace("-", " -").Replace(@"\", @" \");
            tf.DrawString(_Teks, _Font, XBrushes.Black, rect, XStringFormats.Center);
            return _h;
        }

        public static double writeWrapText(XGraphics _gfx, string _Teks, XFont _Font, double _x, double _y, double _width, double _height = 0d)
        {
            double _h;

            if (_height == 0d)
            {
                _h = _Font.Height;
            }
            else
            {
                _h = _height;
            }

            var tf = new XTextFormatter(_gfx);
            var rect = new XRect(_x, _y, _width, _h);
            tf.Alignment = XParagraphAlignment.Left;
            _Teks = _Teks.Replace("/", " /").Replace("-", " -").Replace(@"\", @" \");
            tf.DrawString(_Teks, _Font, XBrushes.Black, rect, XStringFormats.TopLeft);
            return _h;
        }

        public static double writeWrapTextRight(XGraphics _gfx, string _Teks, XFont _Font, double _x, double _y, double _width, double _height = 0d)
        {
            double _h;

            if (_height == 0d)
            {
                _h = _Font.Height;
            }
            else
            {
                _h = _height;
            }
            var tf = new XTextFormatter(_gfx);
            var rect = new XRect(_x, _y, _width, _h);
            tf.Alignment = XParagraphAlignment.Right;
            _Teks = _Teks.Replace("/", " /").Replace("-", " -").Replace(@"\", @" \");
            tf.DrawString(_Teks, _Font, XBrushes.Black, rect, XStringFormats.TopLeft);
            // _gfx.DrawString(_Teks, _Font, PdfSharp.Drawing.XBrushes.Black, New System.Drawing.RectangleF(_x, _y, _width, _h), xLeft)
            return _h;
        }

        public static void drawBox(XGraphics _gfx, double _x, double _y, double _width, double _height, double _lineWidth = 0d, double _Round = 1d)
        {
            if (_lineWidth == 0d)
                _lineWidth = 0.5d;
            _gfx.DrawRoundedRectangle(new  XPen( XColor.FromName("Black"), _lineWidth), new  XRect(_x, _y, _width, _height), new  XSize(_Round, _Round));
        }

    }
}
