using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using System.Drawing;
using System.Text;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Utility;
using ZXing;
using ZXing.QrCode;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PrintController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PrintController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //FM-WH-001
        [HttpGet]
        public async Task<IActionResult> PurchaseOrderManifest(string PONumber)
        {
            var model = await _unitOfWork.PurchaseOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.PONumber == PONumber,
                includeProperties:
                    m => m.Include(m => m.IncPurchaseOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.MasSupplierData.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.MasDataTenantWarehouse.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.MasDataTenant)
                    .Include(m => m.masDataTenantDivision));


            if (model == null)
            {
                TempData["error"] = "Purchase Order NotFound!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string dirBarcode = webRootPath + "\\img\\QRCode\\ItemProducts\\" + model.PONumber.ToString() + ".png";

            var matrix = new QRCodeWriter().encode(model.PONumber.ToString(), BarcodeFormat.QR_CODE, 150, 150);
            Bitmap result = new Bitmap(matrix.Width, matrix.Height);
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                            result.SetPixel((x * 1) + i, (y * 1) + j, pixel);
                    }
                }
                result.Save(dirBarcode);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Manifest Purchase Order - " + model.PONumber;

                XFont Tittle = new XFont("Arial", 14, XFontStyle.Bold);
                XFont Subtitle = new XFont("Arial", 11, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 9);
                XFont BodyBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont Alert = new XFont("Arial", 8);
                XFont AlertBold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                string temp = "";

                XTextFormatter tf = new XTextFormatter(gfx);

                //PDF.setImage(gfx, webRootPath + "\\img\\Logo\\confidential.png", 60, 150, 516, 434);

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, 75, 45);

                XRect rect = new XRect(25, 85, 200, 30);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "No. Form   : FM-WH-001 \n" +
                       "Date           : " + DateTime.Now.ToString("dd / MM / yyyy");
                tf.DrawString(temp, AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);

                PDF.setImage(gfx, dirBarcode, 470, 0, 100, 100);

                //HEADER BODY
                double lebar = 80;
                double tinggi = 80;
                double marginX = 20;
                double marginY = 100;
                lebar = page.Width - marginX * 2;
                tinggi = 15;

                rect = new XRect(marginX, marginY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                temp = "MANIFEST PURCHASE ORDER";
                tf.DrawString(temp, Tittle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + 20;
                tinggi = 25;
                rect = new XRect(marginX, marginY, lebar, tinggi);
                temp = model.PONumber;
                tf.DrawString(temp, Subtitle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                double tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                double tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = 50;

                //IDENTITAS KIRI BODY
                tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = tinggi + 40;

                XRect IdentKiri = new XRect(marginX, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Supplier \n" +
                    "Phone Number \n" +
                    "Office Number \n" +
                    "Email \n" +
                    "Address ";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, IdentKiri);

                XRect IdentKiriValue = new XRect(marginX + tempat1, marginY, tempat2 - 10, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + model.MasSupplierData.Name + "\n" +
                        ": " + model.MasSupplierData.HandPhone + "\n" +
                        ": " + model.MasSupplierData.OfficePhone + "\n" +
                        ": " + model.MasSupplierData.Email + "\n" +
                        ": " + model.MasSupplierData.Address +
                        ", " + model.MasSupplierData.MasKelurahan.KelName +
                        ", " + model.MasSupplierData.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.MasSupplierData.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.MasSupplierData.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.MasSupplierData.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, IdentKiriValue);

                //IDENTITAS KANAN BODY
                IdentKiri = new XRect(marginX + tempat1 + tempat2, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Tenant \n" +
                        "Phone Number \n" +
                        "Office Phone\n" +
                        "Warehouse \n" +
                        "Office Phone\n" +
                        "Address ";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);
                IdentKiriValue = new XRect(marginX + tempat1 * 2 + tempat2, marginY, tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + model.MasDataTenant.Name + "\n" +
                        ": " + model.MasDataTenant.PhoneNumber + "\n" +
                        ": " + model.MasDataTenant.OfficePhone + "\n" +
                        ": " + model.MasDataTenantWarehouse.MasHouseCode.HouseName + "\n" +
                        ": " + model.MasDataTenantWarehouse.MasHouseCode.OfficePhone + "\n" +
                        ": " + model.MasDataTenantWarehouse.MasHouseCode.Address +
                        ", " + model.MasDataTenantWarehouse.MasHouseCode.MasKelurahan.KelName +
                        ", " + model.MasDataTenantWarehouse.MasHouseCode.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.MasDataTenantWarehouse.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.MasDataTenantWarehouse.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.MasDataTenantWarehouse.MasHouseCode.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT TITLE
                marginX = 20 * 2;
                marginY = marginY + tinggi;
                lebar = page.Width - marginX * 2;
                tinggi = 20;

                XRect SubTitle = new XRect(marginX, marginY, lebar, tinggi);
                temp = "PRODUCT LIST";
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(temp, Subtitle, XBrushes.Black, SubTitle, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, SubTitle);

                //PRODUCT HEADER
                marginY = marginY + tinggi;
                lebar = lebar / 2 / 8;
                var lebarno = lebar;
                tinggi = 15;
                XRect TableHeaderNo = new XRect(marginX, marginY, lebar, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                marginX = marginX + lebar;
                lebar = (lebar * 2 * 7 / 2) + lebarno;
                XRect TableHeaderProductName = new XRect(marginX, marginY, lebar, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                marginX = marginX + lebar;
                lebar = (lebar + lebarno) / 3;
                var lebarprice = lebar;
                XRect TableHeaderProductPrice = new XRect(marginX, marginY, lebar, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductPrice);
                tf.DrawString("Price", BodyBold, XBrushes.Black, TableHeaderProductPrice, XStringFormats.TopLeft);

                marginX = marginX + lebar;
                XRect TableHeaderProductQuantity = new XRect(marginX, marginY, lebar - lebarno - lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebar - lebarno - lebarno;
                XRect TableHeaderProductSubtotal = new XRect(marginX, marginY, lebar, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductSubtotal);
                tf.DrawString("Subtotal", BodyBold, XBrushes.Black, TableHeaderProductSubtotal, XStringFormats.TopLeft);

                int totalhalaman = 1;
                if (model.IncPurchaseOrderProducts.Count >= 21)
                {
                    decimal totallist = 28;
                    totalhalaman = (int)Math.Ceiling((model.IncPurchaseOrderProducts.Count - 20) / totallist) + 1;
                }

                int halaman = 1;
                XRect Footer = new XRect(0, 40 + (page.Height - 40 * 2) + 5, page.Width, page.Height - (40 + (page.Height - 40 * 2)));
                //gfx.DrawRectangle(XPens.Black, Footer);
                temp = "Page " + (halaman).ToString() + " of " + totalhalaman;
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                for (int i = 0; i < model.IncPurchaseOrderProducts.Count; i++)
                {
                    marginX = 20 * 2;
                    marginY = marginY + tinggi;
                    lebar = page.Width - marginX * 2;
                    lebar = lebar / 2 / 8;
                    tinggi = 25;

                    TableHeaderNo = new XRect(marginX, marginY, lebar, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                    TableHeaderNo = new XRect(marginX, marginY + 7, lebar, tinggi);
                    temp = (i + 1).ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                    marginX = marginX + lebar;
                    lebar = (lebar * 2 * 7 / 2) + lebarno;
                    TableHeaderProductName = new XRect(marginX, marginY, lebar, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                    TableHeaderProductName = new XRect(marginX + 2, marginY + 2, lebar - 2, tinggi);
                    temp = "SKU : " + model.IncPurchaseOrderProducts[i].MasProductData.SKU + "\n" +
                        model.IncPurchaseOrderProducts[i].MasProductData.ProductName;
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                    marginX = marginX + lebar;
                    lebar = (lebar + lebarno) / 3;
                    TableHeaderProductPrice = new XRect(marginX, marginY, lebar, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductPrice);
                    TableHeaderProductPrice = new XRect(marginX, marginY + 7, lebar - 2, tinggi);
                    temp = "Rp. " + String.Format("{0:n0}", model.IncPurchaseOrderProducts[i].UnitPrice) + " ";
                    tf.Alignment = XParagraphAlignment.Right;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductPrice, XStringFormats.TopLeft);

                    marginX = marginX + lebar;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebar - lebarno - lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 7, lebar - lebarno - lebarno, tinggi);
                    temp = model.IncPurchaseOrderProducts[i].Quantity.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebar - lebarno - lebarno;
                    TableHeaderProductSubtotal = new XRect(marginX, marginY, lebar, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductSubtotal);
                    TableHeaderProductSubtotal = new XRect(marginX, marginY + 7, lebar - 2, tinggi);
                    temp = "Rp. " + String.Format("{0:n0}", model.IncPurchaseOrderProducts[i].SubTotal) + " ";
                    tf.Alignment = XParagraphAlignment.Right;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductSubtotal, XStringFormats.TopLeft);

                    if (marginY > page.Height - 60 * 2)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        marginX = 40;
                        marginY = 40 / 2;
                        temp = "";

                        tf = new XTextFormatter(gfx);

                        //HEADER
                        lebar = page.Width - marginX * 2;
                        tinggi = marginY;
                        rect = new XRect(marginX, marginY, lebar, tinggi);
                        tf.Alignment = XParagraphAlignment.Right;
                        tf.DrawString("Date : " + DateTime.Now.ToString("dd / MM / yyyy"), AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString("No. Form : FM-WH-001", AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        //gfx.DrawRectangle(XPens.Black, rect);

                        marginY = marginY + tinggi;
                        lebar = lebar / 2 / 8;
                        lebarno = lebar;
                        tinggi = 15;
                        TableHeaderNo = new XRect(marginX, marginY, lebar, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                        marginX = marginX + lebar;
                        lebar = (lebar * 2 * 7 / 2) + lebarno;
                        TableHeaderProductName = new XRect(marginX, marginY, lebar, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                        marginX = marginX + lebar;
                        lebar = (lebar + lebarno) / 3;
                        TableHeaderProductPrice = new XRect(marginX, marginY, lebar, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductPrice);
                        tf.DrawString("Price", BodyBold, XBrushes.Black, TableHeaderProductPrice, XStringFormats.TopLeft);

                        marginX = marginX + lebar;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebar - lebarno - lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebar - lebarno - lebarno;
                        TableHeaderProductSubtotal = new XRect(marginX, marginY, lebar, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductSubtotal);
                        tf.DrawString("Subtotal", BodyBold, XBrushes.Black, TableHeaderProductSubtotal, XStringFormats.TopLeft);

                        //gfx.DrawRectangle(XPens.Black, Footer);
                        temp = "Page " + (halaman + 1).ToString() + " of " + totalhalaman;
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                        halaman++;
                    }
                }

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                lebar = page.Width - marginX * 2;
                var lebar_col_footer1 = lebar - lebarprice;
                tinggi = 15;
                XRect RowSubtotal = new XRect(marginX, marginY, lebar_col_footer1, tinggi);
                gfx.DrawRectangle(XPens.Black, RowSubtotal);
                RowSubtotal = new XRect(marginX, marginY + 3, lebar_col_footer1, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Subtotal", Body, XBrushes.Black, RowSubtotal, XStringFormats.TopLeft);

                marginX = marginX + lebar_col_footer1;
                var lebar_col_footer2 = lebarprice;
                XRect RowSubtotalValue = new XRect(marginX, marginY, lebar_col_footer2, tinggi);
                gfx.DrawRectangle(XPens.Black, RowSubtotalValue);
                RowSubtotalValue = new XRect(marginX, marginY + 3, lebar_col_footer2 - 2, tinggi);
                temp = "Rp. " + String.Format("{0:n0}", model.IncPurchaseOrderProducts.Sum(m => m.SubTotal)) + " ";
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString(temp, Body, XBrushes.Black, RowSubtotalValue, XStringFormats.TopLeft);

                marginX = 20 * 2;
                marginY = marginY + tinggi;
                XRect RowTax = new XRect(marginX, marginY, lebar_col_footer1, tinggi);
                gfx.DrawRectangle(XPens.Black, RowTax);
                RowTax = new XRect(marginX, marginY + 3, lebar_col_footer1, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Tax", Body, XBrushes.Black, RowTax, XStringFormats.TopLeft);

                marginX = marginX + lebar_col_footer1;
                XRect RowTaxValue = new XRect(marginX, marginY, lebar_col_footer2, tinggi);
                gfx.DrawRectangle(XPens.Black, RowTaxValue);
                RowTaxValue = new XRect(marginX, marginY + 3, lebar_col_footer2 - 2, tinggi);
                temp = "Rp. " + String.Format("{0:n0}", model.OrderTax) + " ";
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString(temp, Body, XBrushes.Black, RowTaxValue, XStringFormats.TopLeft);

                marginX = 20 * 2;
                marginY = marginY + tinggi;
                XRect RowDiscount = new XRect(marginX, marginY, lebar_col_footer1, tinggi);
                gfx.DrawRectangle(XPens.Black, RowDiscount);
                RowDiscount = new XRect(marginX, marginY + 3, lebar_col_footer1, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Discount", Body, XBrushes.Black, RowDiscount, XStringFormats.TopLeft);

                marginX = marginX + lebar_col_footer1;
                XRect RowDiscountValue = new XRect(marginX, marginY, lebar_col_footer2, tinggi);
                gfx.DrawRectangle(XPens.Black, RowDiscountValue);
                RowDiscountValue = new XRect(marginX, marginY + 3, lebar_col_footer2 - 2, tinggi);
                temp = "Rp. " + String.Format("{0:n0}", model.Discount) + " ";
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString(temp, Body, XBrushes.Black, RowDiscountValue, XStringFormats.TopLeft);

                marginX = 20 * 2;
                marginY = marginY + tinggi;
                XRect RowTotal = new XRect(marginX, marginY, lebar_col_footer1, tinggi);
                gfx.DrawRectangle(XPens.Black, RowTotal);
                RowTotal = new XRect(marginX, marginY + 3, lebar_col_footer1, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Total", BodyBold, XBrushes.Black, RowTotal, XStringFormats.TopLeft);

                marginX = marginX + lebar_col_footer1;
                XRect RowTotalValue = new XRect(marginX, marginY, lebar_col_footer2, tinggi);
                gfx.DrawRectangle(XPens.Black, RowTotalValue);
                RowTotalValue = new XRect(marginX, marginY + 3, lebar_col_footer2 - 2, tinggi);
                temp = "Rp. " + String.Format("{0:n0}", (model.IncPurchaseOrderProducts.Sum(m => m.SubTotal) - model.Discount + model.OrderTax)) + " ";
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString(temp, BodyBold, XBrushes.Black, RowTotalValue, XStringFormats.TopLeft);

                System.IO.File.Delete(webRootPath + "\\img\\QRCode\\ItemProducts\\" + model.PONumber.ToString() + ".png");

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        //FM-WH-002
        [HttpGet]
        public async Task<IActionResult> DeliveryOrderManifest(string DONumber)
        {
            var deliveries = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.MasDeliveryOrderCourier)
                    .Include(m => m.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));

            if (deliveries == null)
            {
                TempData["error"] = "Delivery Order Notfound!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string dirBarcode = webRootPath + "\\img\\QRCode\\ItemProducts\\" + deliveries.DONumber.ToString() + ".png";

            var matrix = new QRCodeWriter().encode(deliveries.DONumber.ToString(), BarcodeFormat.QR_CODE, 150, 150);
            Bitmap result = new Bitmap(matrix.Width, matrix.Height);
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                            result.SetPixel((x * 1) + i, (y * 1) + j, pixel);
                    }
                }
                result.Save(dirBarcode);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Incoming Manifest - " + deliveries.DONumber;

                XFont Tittle = new XFont("Arial", 14, XFontStyle.Bold);
                XFont Subtitle = new XFont("Arial", 11, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 9);
                XFont BodyBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont Alert = new XFont("Arial", 8);
                XFont AlertBold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                string temp = "";

                XTextFormatter tf = new XTextFormatter(gfx);

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, 75, 45);

                XRect rect = new XRect(25, 85, 200, 30);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "No. Form   : FM-WH-002 \n" + 
                       "Date           : " + DateTime.Now.ToString("dd / MM / yyyy");
                tf.DrawString(temp, AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);

                PDF.setImage(gfx, dirBarcode, 470, 0, 100, 100);

                //HEADER BODY
                double marginX = 20;
                double marginY = 100;
                double lebar = page.Width - marginX * 2;
                double tinggi = 15;

                rect = new XRect(marginX, marginY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                temp = "INCOMING MANIFEST";
                tf.DrawString(temp, Tittle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + 20;
                tinggi = 25;
                rect = new XRect(marginX, marginY, lebar, tinggi);
                temp = deliveries.DOSupplier;
                tf.DrawString(temp, Subtitle, XBrushes.Black, rect, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                double tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                double tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = 90;

                //IDENTITAS KIRI BODY

                XRect IdentKiri = new XRect(marginX, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Tenant \n" +
                    "Status \n\n" +
                    "Warehouse \n" +
                    "Office Phone \n" +
                    "Address";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                XRect IdentKiriValue = new XRect(marginX + tempat1, marginY, tempat2 - 10, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + deliveries.MasDataTenant.Name + "\n" +
                        ": " + deliveries.Status + "\n\n" +
                        ": " + deliveries.MasHouseCode.HouseName + "\n" +
                        ": " + deliveries.MasHouseCode.OfficePhone + "\n" +
                        ": " + deliveries.MasHouseCode.Address +
                        ", " + deliveries.MasHouseCode.MasKelurahan.KelName +
                        ", " + deliveries.MasHouseCode.MasKelurahan.MasKecamatan.KecName +
                        ", " + deliveries.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + deliveries.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + deliveries.MasHouseCode.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //IDENTITAS KANAN BODY
                IdentKiri = new XRect(marginX + tempat1 + tempat2, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "DO Date \n" +
                        "Total Product \n" +
                        "Total Quantity \n\n" +
                        "Delivered By \n" +
                        "ID Card \n" +
                        "Name \n";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                IdentKiriValue = new XRect(marginX + tempat1 * 2 + tempat2, marginY, tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + deliveries.DateDelivered.Value.ToString("dd MMMM yyyy") + "\n" +
                        ": " + deliveries.IncDeliveryOrderProducts.Count() + " \n" +
                        ": " + deliveries.IncDeliveryOrderProducts.Sum(m => m.Quantity) + " \n\n" +
                        ": " + deliveries.MasDeliveryOrderCourier.Name + "\n" +
                        ": " + deliveries.KTP + "\n" +
                        ": " + deliveries.Name;
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT TITLE
                marginX = 20 * 2;
                marginY = marginY + tinggi;
                lebar = page.Width - marginX * 2;
                tinggi = 20;

                XRect SubTitle = new XRect(marginX, marginY, lebar, tinggi);
                temp = "PRODUCT LIST";
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(temp, Subtitle, XBrushes.Black, SubTitle, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT HEADER
                marginY = marginY + tinggi;
                var lebarno = lebar / 2 / 8;
                var leabrqty = lebar / 2 / 3;
                var lebarproduk = lebar - (lebarno * 3) - leabrqty;

                tinggi = 15;
                XRect TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                XRect TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                marginX = marginX + lebarproduk;
                XRect TableHeaderProductCondition = new XRect(marginX, marginY, leabrqty, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductCondition);
                tf.DrawString("Status", BodyBold, XBrushes.Black, TableHeaderProductCondition, XStringFormats.TopLeft);

                marginX = marginX + leabrqty;
                XRect TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("QTY", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("UOM", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                int totalhalaman = 1;
                if (deliveries.IncDeliveryOrderProducts.Count >= 13)
                {
                    decimal totallist = 20;
                    totalhalaman = (int)Math.Ceiling((deliveries.IncDeliveryOrderProducts.Count - 12) / totallist) + 1;
                }

                int halaman = 1;
                XRect Footer = new XRect(0, 40 + (page.Height - 40 * 2) + 5, page.Width, page.Height - (40 + (page.Height - 40 * 2)));
                //gfx.DrawRectangle(XPens.Black, Footer);
                temp = "Page " + (halaman).ToString() + " of " + totalhalaman;
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                for (int i = 0; i < deliveries.IncDeliveryOrderProducts.Count; i++)
                {
                    marginX = 20 * 2;
                    marginY = marginY + tinggi;
                    tinggi = 33;

                    TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                    TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                    temp = (i + 1).ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                    TableHeaderProductName = new XRect(marginX + 2, marginY + 8, lebarproduk - 2, tinggi);
                    temp = "SKU : " + deliveries.IncDeliveryOrderProducts[i].MasProductData.SKU + "\n" +
                        deliveries.IncDeliveryOrderProducts[i].MasProductData.ProductName;
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                    marginX = marginX + lebarproduk;
                    TableHeaderProductCondition = new XRect(marginX, marginY, leabrqty, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductCondition);
                    TableHeaderProductCondition = new XRect(marginX, marginY + 10, leabrqty, tinggi);
                    temp = deliveries.IncDeliveryOrderProducts[i].MasProductData.ProductCondition;
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductCondition, XStringFormats.TopLeft);

                    marginX = marginX + leabrqty;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarno, tinggi);
                    temp = deliveries.IncDeliveryOrderProducts[i].Quantity.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarno, tinggi);
                    temp = deliveries.IncDeliveryOrderProducts[i].MasProductData.Unit;
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    if (marginY > (page.Height - 60 * 2) - 60)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        marginX = 40;
                        marginY = 40 / 2;
                        temp = "";

                        tf = new XTextFormatter(gfx);

                        //HEADER
                        lebar = page.Width - marginX * 2;
                        tinggi = marginY;
                        rect = new XRect(marginX, marginY, lebar, tinggi);
                        tf.Alignment = XParagraphAlignment.Right;
                        tf.DrawString("Date : " + DateTime.Now.ToString("dd / MM / yyyy"), AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString("No. Form : FM-WH-002", AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        //gfx.DrawRectangle(XPens.Black, rect);

                        marginY = marginY + tinggi;
                        tinggi = 15;
                        TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);


                        marginX = marginX + lebarproduk;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, leabrqty, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("Status", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + leabrqty;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("QTY", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("UOM", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        temp = "Page " + (halaman + 1).ToString() + " of " + totalhalaman;
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                        halaman++;
                    }
                }

                //TABLE FOOTER KIRI
                marginY = marginY + tinggi;
                marginX = 20 * 2;
                var lebartotal = lebar - lebarno - lebarno;
                tinggi = 15;
                XRect TableFooterKiri = new XRect(marginX, marginY, lebartotal, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKiri);
                TableFooterKiri = new XRect(marginX, marginY + 3, lebartotal, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Total", BodyBold, XBrushes.Black, TableFooterKiri, XStringFormats.TopLeft);

                //TABLE FOOTER KANAN
                marginX = marginX + lebartotal;
                XRect TableFooterKanan = new XRect(marginX, marginY, lebarno + lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 3, lebarno + lebarno, tinggi);
                temp = String.Format("{0:n0}", deliveries.IncDeliveryOrderProducts.Sum(m => m.Quantity)) + "  ";
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 30;

                XRect TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(".............................., " + DateTime.Now.ToString("... / MM / yyyy") + "\n Diserahkan Oleh,", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);


                marginX = marginX + tempat1 + tempat2;
                XRect TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("\n Diterima Oleh,", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 60;

                TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);

                marginX = marginX + tempat1 + tempat2;
                TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                System.IO.File.Delete(webRootPath + "\\img\\QRCode\\ItemProducts\\" + deliveries.DONumber.ToString() + ".png");

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        //FM-WH-003
        [HttpGet]
        public async Task<IActionResult> StockOpnameManifest(string OpnameId)
        {
            var model = await _unitOfWork.StockOpname.GetSingleOrDefaultAsync(
                filter:
                    m => m.OpnameId == OpnameId &&
                    m.Status == SD.FlagOpname_Done,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.InvStockOpnameProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.InvStockOpnameProducts),
                orderBy:
                    m => m.OrderByDescending(m => m.DateOpname));

            if (model == null)
            {
                TempData["error"] = "Opname not found!";
                return RedirectToAction("Index");
            }

            if (model.Status != SD.FlagOpname_Done)
            {
                TempData["error"] = "Opname not finished!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Manifest Stock Opname - " + OpnameId;

                XFont Tittle = new XFont("Arial", 14, XFontStyle.Bold);
                XFont Subtitle = new XFont("Arial", 11, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 9);
                XFont BodyBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont Alert = new XFont("Arial", 8);
                XFont AlertBold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                string temp = "";

                XTextFormatter tf = new XTextFormatter(gfx);

                //PDF.setImage(gfx, webRootPath + "\\img\\Logo\\confidential.png", 60, 150, 516, 434);

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, 75, 45);
                //PDF.writeText(gfx, "Manifest Date : " + DateTime.Now.ToString("dd / MM / yyyy"), Body, 25, 85, 0);

                XRect rect = new XRect(25, 85, 200, 30);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "No. Form   : FM-WH-003 \n" +
                       "Date           : " + DateTime.Now.ToString("dd / MM / yyyy");
                tf.DrawString(temp, AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);

                //HEADER BODY
                double lebar = 80;
                double tinggi = 80;
                double marginX = 20;
                double marginY = 100;
                lebar = page.Width - marginX * 2;
                tinggi = 15;

                rect = new XRect(marginX, marginY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                temp = "MANIFEST STOCK OPNAME";
                tf.DrawString(temp, Tittle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + 20;
                tinggi = 25;
                rect = new XRect(marginX, marginY, lebar, tinggi);
                temp = model.OpnameId;
                tf.DrawString(temp, Subtitle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                double tempat1 = (page.Width - marginX * 2) / 2 / 3 * 3 / 4;
                double pembagi = 5;
                double tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = 50 + 15;

                XRect IdentKiri = new XRect(marginX, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Tenant \n" +
                    "Warehouse \n" +
                    "Address";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, IdentKiri);

                XRect TitikBagi = new XRect(marginX + tempat1, marginY, pembagi, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": \n" +
                    ": \n" +
                    ":";
                tf.DrawString(temp, Body, XBrushes.Black, TitikBagi, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, TitikBagi);

                XRect IdentKiriValue = new XRect(marginX + tempat1 + pembagi, marginY, tempat2 - 10, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = model.MasDataTenant.Name + "\n" +
                        model.MasHouseCode.HouseName + "\n" +
                        model.MasHouseCode.Address +
                        ", " + model.MasHouseCode.MasKelurahan.KelName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.MasHouseCode.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, IdentKiriValue);

                //IDENTITAS KANAN BODY
                IdentKiri = new XRect(marginX + tempat1 + tempat2, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Opname By \n" +
                        "Date Opname";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                TitikBagi = new XRect(marginX + tempat1 * 2 + tempat2, marginY, pembagi, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": \n" +
                    ":";
                tf.DrawString(temp, Body, XBrushes.Black, TitikBagi, XStringFormats.TopLeft);

                IdentKiriValue = new XRect(marginX + tempat1 * 2 + tempat2 + pembagi, marginY, tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = model.CreatedBy + "\n" +
                        model.DateOpname +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT TITLE
                marginX = 20 * 2;
                marginY = marginY + tinggi;
                lebar = page.Width - marginX * 2;
                tinggi = 20;

                XRect SubTitle = new XRect(marginX, marginY, lebar, tinggi);
                temp = "PRODUCT LIST";
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(temp, Subtitle, XBrushes.Black, SubTitle, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, SubTitle);

                //PRODUCT HEADER
                marginY = marginY + tinggi;
                //lebar = lebar / 2 / 8;

                var lebarno = lebar / 2 / 8;
                tinggi = 30;
                XRect TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                var lebarproduct = lebar - ((lebarno * 8) + (lebarno * 1 / 2));
                marginX = marginX + lebarno;
                XRect TableHeaderProductName = new XRect(marginX, marginY, lebarproduct, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                TableHeaderProductName = new XRect(marginX, marginY + 10, lebarproduct, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                var lebarquantity = lebarno * 2 - (lebarno * 1 / 2);
                marginX = marginX + lebarproduct;
                XRect TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                tf.DrawString("WMSDeal \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                tf.DrawString("Rusak \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                tf.DrawString("Expired \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                tf.DrawString("Perbedaan \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                tf.DrawString("Aktual \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                int totalhalaman = 1;
                if (model.InvStockOpnameProducts.Count >= 13)
                {
                    decimal totallist = 18;
                    totalhalaman = (int)Math.Ceiling((model.InvStockOpnameProducts.Count - 12) / totallist) + 1;
                }

                int halaman = 1;
                XRect Footer = new XRect(0, 40 + (page.Height - 40 * 2) + 5, page.Width, page.Height - (40 + (page.Height - 40 * 2)));
                //gfx.DrawRectangle(XPens.Black, Footer);
                temp = "Page " + (halaman).ToString() + " of " + totalhalaman;
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                for (int i = 0; i < model.InvStockOpnameProducts.Count; i++)
                {
                    marginX = 20 * 2;
                    marginY = marginY + tinggi;
                    lebar = page.Width - marginX * 2;
                    lebar = lebar / 2 / 8;
                    tinggi = 35;

                    TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                    TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                    temp = (i + 1).ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderProductName = new XRect(marginX, marginY, lebarproduct, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                    TableHeaderProductName = new XRect(marginX + 2, marginY + 2, lebarproduct - 2, tinggi);
                    temp = "SKU : " + model.InvStockOpnameProducts[i].MasProductData.SKU + "\n" +
                        model.InvStockOpnameProducts[i].MasProductData.ProductName + "\n" +
                        model.InvStockOpnameProducts[i].MasProductData.ProductCondition;
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                    marginX = marginX + lebarproduct;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarquantity, tinggi);
                    temp = model.InvStockOpnameProducts[i].SystemQty.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebarquantity;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarquantity, tinggi);
                    temp = model.InvStockOpnameProducts[i].BrokenQty.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebarquantity;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarquantity, tinggi);
                    temp = model.InvStockOpnameProducts[i].ExpiredQty.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebarquantity;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarquantity, tinggi);
                    temp = model.InvStockOpnameProducts[i].DiscrepancyQty.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebarquantity;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 10, lebarquantity, tinggi);
                    temp = model.InvStockOpnameProducts[i].StockQty.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    if (marginY > (page.Height - 60 * 2) - 60)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        marginX = 40;
                        marginY = 40 / 2;
                        temp = "";

                        tf = new XTextFormatter(gfx);

                        //HEADER
                        lebar = page.Width - marginX * 2;
                        tinggi = marginY;
                        rect = new XRect(marginX, marginY, lebar, tinggi);
                        tf.Alignment = XParagraphAlignment.Right;
                        tf.DrawString("Date : " + DateTime.Now.ToString("dd / MM / yyyy"), AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString("No. Form : FM-WH-003", AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        //gfx.DrawRectangle(XPens.Black, rect);

                        marginY = marginY + tinggi;
                        tinggi = 30;
                        TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                        TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductName = new XRect(marginX, marginY, lebarproduct, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                        TableHeaderProductName = new XRect(marginX, marginY + 10, lebarproduct, tinggi);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                        marginX = marginX + lebarproduct;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                        tf.DrawString("WMSDeal \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarquantity;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                        tf.DrawString("Rusak \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarquantity;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                        tf.DrawString("Expired \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarquantity;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                        tf.DrawString("Perbedaan \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarquantity;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarquantity, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarquantity, tinggi);
                        tf.DrawString("Aktual \n Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        //gfx.DrawRectangle(XPens.Black, Footer);
                        temp = "Page " + (halaman + 1).ToString() + " of " + totalhalaman;
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                        halaman++;
                    }
                }

                //TABLE FOOTER KIRI
                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 15;
                XRect TableFooterKiri = new XRect(marginX, marginY, lebarno + lebarproduct, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKiri);
                TableFooterKiri = new XRect(marginX, marginY + 2, lebarno + lebarproduct, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Total", BodyBold, XBrushes.Black, TableFooterKiri, XStringFormats.TopLeft);

                //TABLE FOOTER KANAN
                marginX = marginX + lebarno + lebarproduct;
                XRect TableFooterKanan = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 2, lebarquantity, tinggi);
                temp = model.InvStockOpnameProducts.Sum(m => m.SystemQty).ToString();
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableFooterKanan = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 2, lebarquantity, tinggi);
                temp = model.InvStockOpnameProducts.Sum(m => m.BrokenQty).ToString();
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableFooterKanan = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 2, lebarquantity, tinggi);
                temp = model.InvStockOpnameProducts.Sum(m => m.ExpiredQty).ToString();
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableFooterKanan = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 2, lebarquantity, tinggi);
                temp = model.InvStockOpnameProducts.Sum(m => m.DiscrepancyQty).ToString();
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginX = marginX + lebarquantity;
                TableFooterKanan = new XRect(marginX, marginY, lebarquantity, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 2, lebarquantity, tinggi);
                temp = model.InvStockOpnameProducts.Sum(m => m.StockQty).ToString();
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 30;

                XRect TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(".............................., " + DateTime.Now.ToString("... / MM / yyyy") + "\n Dibuat Oleh,", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);


                marginX = marginX + tempat1 + tempat2;
                XRect TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("\n Diketahui Oleh,", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 60;

                TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);

                marginX = marginX + tempat1 + tempat2;
                TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);


                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        //FM-WH-004
        [HttpGet]
        public async Task<IActionResult> SalesOrderManifest(string OrderId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts)
                    .ThenInclude(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutsalesOrderDelivery.MasSalesCourier),
                filter:
                    m => m.OrderId == OrderId);

            if (model == null)
            {
                TempData["error"] = "Sales Order Notfound!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string dirBarcode = webRootPath + "\\img\\QRCode\\ItemProducts\\" + model.OrderId.ToString() + ".png";

            var matrix = new QRCodeWriter().encode(model.OrderId.ToString(), BarcodeFormat.QR_CODE, 150, 150);
            Bitmap result = new Bitmap(matrix.Width, matrix.Height);
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                            result.SetPixel((x * 1) + i, (y * 1) + j, pixel);
                    }
                }
                result.Save(dirBarcode);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Outgoing Manifest - " + OrderId;

                XFont Tittle = new XFont("Arial", 14, XFontStyle.Bold);
                XFont Subtitle = new XFont("Arial", 11, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 9);
                XFont BodyBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont Alert = new XFont("Arial", 8);
                XFont AlertBold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                string temp = "";

                XTextFormatter tf = new XTextFormatter(gfx);

                //PDF.setImage(gfx, webRootPath + "\\img\\Logo\\confidential.png", 60, 150, 516, 434);

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, 75, 45);
                //PDF.writeText(gfx, "Manifest Date : " + DateTime.Now.ToString("dd / MM / yyyy"), Body, 25, 85, 0);

                XRect rect = new XRect(25, 85, 200, 30);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "No. Form   : FM-WH-004 \n" +
                       "Date           : " + DateTime.Now.ToString("dd / MM / yyyy");
                tf.DrawString(temp, AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);

                PDF.setImage(gfx, dirBarcode, 470, 0, 100, 100);

                //HEADER BODY
                double marginX = 20;
                double marginY = 100;
                double lebar = page.Width - marginX * 2;
                double tinggi = 15;

                rect = new XRect(marginX, marginY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                temp = "OUTGOING MANIFEST";
                tf.DrawString(temp, Tittle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + 20;
                tinggi = 25;
                rect = new XRect(marginX, marginY, lebar, tinggi);
                temp = model.OrderId;
                tf.DrawString(temp, Subtitle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                double tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                double tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = 100;

                //IDENTITAS KIRI BODY

                XRect IdentKiri = new XRect(marginX, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Tenant \n" +
                    "SO Type \n\n" +
                    "Warehouse \n" +
                    "Office Phone \n" +
                    "Address";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                XRect IdentKiriValue = new XRect(marginX + tempat1, marginY, tempat2 - 10, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + model.MasDataTenant.Name + "\n" +
                        ": " + model.MasSalesType.StyName + "\n \n" +
                        ": " + model.MasHouseCode.HouseName + "\n" +
                        ": " + model.MasHouseCode.OfficePhone + "\n" +
                        ": " + model.MasHouseCode.Address +
                        ", " + model.MasHouseCode.MasKelurahan.KelName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.MasHouseCode.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //IDENTITAS KANAN BODY
                IdentKiri = new XRect(marginX + tempat1 + tempat2, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;

                temp = "Picked Up By \n" +
                     "ID Card \n" +
                        "Name \n \n";

                if (model.OrdSalesType == SD.SOType_Sales)
                {
                    temp = temp +
                            "Consignee \n" +
                            "Phone Number \n" +
                            "Address \n";
                }

                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                IdentKiriValue = new XRect(marginX + tempat1 * 2 + tempat2, marginY, tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Left;

                temp = ": " + model.OutsalesOrderDelivery.MasSalesCourier.Name + "\n" +
                        ": " + model.OutSalesOrderCustomer.KTP + "\n" +
                        ": " + model.OutSalesOrderCustomer.CustName + "\n \n";
                if (model.OrdSalesType == SD.SOType_Sales)
                {
                    temp = temp +
                        ": " + model.OutSalesOrderConsignee.CneeName + "\n" +
                        ": " + model.OutSalesOrderConsignee.CneePhone + "\n" +
                        ": " + model.OutSalesOrderConsignee.CneeAddress +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.KelName +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.OutSalesOrderConsignee.OrdZipCode;
                }

                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT TITLE
                marginX = 20 * 2;
                marginY = marginY + tinggi;
                lebar = page.Width - marginX * 2;
                tinggi = 20;

                XRect SubTitle = new XRect(marginX, marginY, lebar, tinggi);
                temp = "PRODUCT LIST";
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(temp, Subtitle, XBrushes.Black, SubTitle, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, SubTitle);

                //PRODUCT HEADER
                marginY = marginY + tinggi;
                var lebarno = lebar / 2 / 8;
                var lebarqty = lebar / 2 / 3;
                var lebarproduk = lebar - (lebarno * 3) - lebarqty;

                tinggi = 15;
                XRect TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                XRect TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                marginX = marginX + lebarproduk;
                XRect TableHeaderProductCondition = new XRect(marginX, marginY, lebarqty, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductCondition);
                tf.DrawString("Status", BodyBold, XBrushes.Black, TableHeaderProductCondition, XStringFormats.TopLeft);

                marginX = marginX + lebarqty;
                XRect TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("QTY", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("UOM", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                int totalhalaman = 1;
                if (model.OutSalesOrderProducts.Count >= 17)
                {
                    decimal totallist = 26;
                    totalhalaman = (int)Math.Ceiling((model.OutSalesOrderProducts.Count - 16) / totallist) + 1;
                }

                int halaman = 1;
                XRect Footer = new XRect(0, 40 + (page.Height - 40 * 2) + 5, page.Width, page.Height - (40 + (page.Height - 40 * 2)));
                //gfx.DrawRectangle(XPens.Black, Footer);
                temp = "Page " + (halaman).ToString() + " of " + totalhalaman;
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                for (int i = 0; i < model.OutSalesOrderProducts.Count; i++)
                {
                    marginX = 20 * 2;
                    marginY = marginY + tinggi;
                    tinggi = 25;

                    TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                    TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                    temp = (i + 1).ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                    TableHeaderProductName = new XRect(marginX + 2, marginY + 2, lebarproduk - 2, tinggi);
                    temp = "SKU : " + model.OutSalesOrderProducts[i].MasProductData.SKU + "\n" +
                        model.OutSalesOrderProducts[i].MasProductData.ProductName;
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                    marginX = marginX + lebarproduk;
                    TableHeaderProductCondition = new XRect(marginX, marginY, lebarqty, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductCondition);
                    TableHeaderProductCondition = new XRect(marginX, marginY + 5, lebarqty, tinggi);
                    temp = model.OutSalesOrderProducts[i].MasProductData.ProductCondition;
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductCondition, XStringFormats.TopLeft);

                    marginX = marginX + lebarqty;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarno, tinggi);
                    temp = model.OutSalesOrderProducts[i].Quantity.ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarno, tinggi);
                    temp = model.OutSalesOrderProducts[i].MasProductData.Unit;
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);


                    if (marginY > (page.Height - 60 * 2) - 50)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        marginX = 40;
                        marginY = 40 / 2;
                        temp = "";

                        tf = new XTextFormatter(gfx);

                        //HEADER
                        lebar = page.Width - marginX * 2;
                        tinggi = marginY;
                        rect = new XRect(marginX, marginY, lebar, tinggi);
                        tf.Alignment = XParagraphAlignment.Right;
                        tf.DrawString("Date : " + DateTime.Now.ToString("dd / MM / yyyy"), AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString("No. Form : FM-WH-004", AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        //gfx.DrawRectangle(XPens.Black, rect);

                        marginY = marginY + tinggi;
                        tinggi = 15;
                        TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);


                        marginX = marginX + lebarproduk;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarqty, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("Status", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarqty;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("QTY", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("UOM", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        //gfx.DrawRectangle(XPens.Black, Footer);
                        temp = "Page " + (halaman + 1).ToString() + " of " + totalhalaman;
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                        halaman++;
                    }
                }

                //TABLE FOOTER KIRI
                marginY = marginY + tinggi;
                marginX = 20 * 2;
                var lebartotal = lebar - lebarno - lebarno;
                tinggi = 15;
                XRect TableFooterKiri = new XRect(marginX, marginY, lebartotal, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKiri);
                TableFooterKiri = new XRect(marginX, marginY + 3, lebartotal, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Total", BodyBold, XBrushes.Black, TableFooterKiri, XStringFormats.TopLeft);

                //TABLE FOOTER KANAN
                marginX = marginX + lebartotal;
                XRect TableFooterKanan = new XRect(marginX, marginY, lebarno + lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableFooterKanan);
                TableFooterKanan = new XRect(marginX, marginY + 3, lebarno + lebarno, tinggi);
                temp = String.Format("{0:n0}", model.OutSalesOrderProducts.Sum(m => m.Quantity)) + "  ";
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, TableFooterKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 30;

                XRect TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(".............................., " + DateTime.Now.ToString("... / MM / yyyy") + "\n Diserahkan Oleh,", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);


                marginX = marginX + tempat1 + tempat2;
                XRect TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("\n Diterima Oleh,", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 60;

                TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);

                marginX = marginX + tempat1 + tempat2;
                TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);


                System.IO.File.Delete(webRootPath + "\\img\\QRCode\\ItemProducts\\" + model.OrderId.ToString() + ".png");

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        //FM-WH-005
        [HttpGet]
        public async Task<IActionResult> PickUpRouteManifest(string OrderId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts)
                    .ThenInclude(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderProducts)
                    .ThenInclude(m => m.OutSalesOrderStorages).ThenInclude(m => m.InvStorageCode.InvStorageBin)
                    .Include(m => m.OutSalesOrderCustomer)
                    .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutsalesOrderDelivery.MasSalesCourier),
                filter:
                    m => m.OrderId == OrderId &&
                    m.FlagPick == 1);

            if (model == null)
            {
                TempData["error"] = "Sales Order Notfound!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string dirBarcode = webRootPath + "\\img\\QRCode\\ItemProducts\\" + model.OrderId.ToString() + ".png";

            var matrix = new QRCodeWriter().encode(model.OrderId.ToString(), BarcodeFormat.QR_CODE, 150, 150);
            Bitmap result = new Bitmap(matrix.Width, matrix.Height);
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                            result.SetPixel((x * 1) + i, (y * 1) + j, pixel);
                    }
                }
                result.Save(dirBarcode);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Manifest Sales Order - " + OrderId;

                XFont Tittle = new XFont("Arial", 14, XFontStyle.Bold);
                XFont Subtitle = new XFont("Arial", 11, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 9);
                XFont BodyBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont Alert = new XFont("Arial", 8);
                XFont AlertBold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                string temp = "";

                XTextFormatter tf = new XTextFormatter(gfx);

                //PDF.setImage(gfx, webRootPath + "\\img\\Logo\\confidential.png", 60, 150, 516, 434);

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, 75, 45);
                //PDF.writeText(gfx, "Manifest Date : " + DateTime.Now.ToString("dd / MM / yyyy"), Body, 25, 85, 0);

                XRect rect = new XRect(25, 85, 200, 30);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "No. Form   : FM-WH-005 \n" +
                       "Date           : " + DateTime.Now.ToString("dd / MM / yyyy");
                tf.DrawString(temp, AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);

                PDF.setImage(gfx, dirBarcode, 470, 0, 100, 100);

                //HEADER BODY
                double marginX = 20;
                double marginY = 100;
                double lebar = page.Width - marginX * 2;
                double tinggi = 15;

                rect = new XRect(marginX, marginY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                temp = "MANIFEST PICK-UP ROUTE";
                tf.DrawString(temp, Tittle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + 20;
                tinggi = 25;
                rect = new XRect(marginX, marginY, lebar, tinggi);
                temp = model.OrderId;
                tf.DrawString(temp, Subtitle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                double tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                double tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = 100;

                //IDENTITAS KIRI BODY

                XRect IdentKiri = new XRect(marginX, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Tenant \n" +
                    "SO Type \n\n" +
                    "Warehouse \n" +
                    "Office Phone \n" +
                    "Address";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                XRect IdentKiriValue = new XRect(marginX + tempat1, marginY, tempat2 - 10, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + model.MasDataTenant.Name + "\n" +
                        ": " + model.MasSalesType.StyName + "\n \n" +
                        ": " + model.MasHouseCode.HouseName + "\n" +
                        ": " + model.MasHouseCode.OfficePhone + "\n" +
                        ": " + model.MasHouseCode.Address +
                        ", " + model.MasHouseCode.MasKelurahan.KelName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.MasHouseCode.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //IDENTITAS KANAN BODY
                IdentKiri = new XRect(marginX + tempat1 + tempat2, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;

                temp = "Picked Up By \n" +
                     "ID Card \n" +
                        "Name \n \n";

                if (model.OrdSalesType == SD.SOType_Sales)
                {
                    temp = temp +
                            "Consignee \n" +
                            "Phone Number \n" +
                            "Address \n";
                }

                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                IdentKiriValue = new XRect(marginX + tempat1 * 2 + tempat2, marginY, tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Left;

                temp = ": " + model.OutsalesOrderDelivery.MasSalesCourier.Name + "\n" +
                        ": " + model.OutSalesOrderCustomer.KTP + "\n" +
                        ": " + model.OutSalesOrderCustomer.CustName + "\n \n";
                if (model.OrdSalesType == SD.SOType_Sales)
                {
                    temp = temp +
                        ": " + model.OutSalesOrderConsignee.CneeName + "\n" +
                        ": " + model.OutSalesOrderConsignee.CneePhone + "\n" +
                        ": " + model.OutSalesOrderConsignee.CneeAddress +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.KelName +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.KecName +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + model.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + model.OutSalesOrderConsignee.OrdZipCode;
                }

                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT TITLE
                marginX = 20 * 2;
                marginY = marginY + tinggi;
                lebar = page.Width - marginX * 2;
                tinggi = 20;

                XRect SubTitle = new XRect(marginX, marginY, lebar, tinggi);
                temp = "PRODUCT LIST";
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(temp, Subtitle, XBrushes.Black, SubTitle, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, SubTitle);

                //PRODUCT HEADER
                marginY = marginY + tinggi;
                var lebarno = lebar / 2 / 8;
                var lebarstatus = lebar / 2 / 3;
                var lebarstorage = lebarstatus + lebarno;
                var lebarproduk = lebar - (lebarno * 2) - lebarstatus - lebarstorage;

                tinggi = 15;
                XRect TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                XRect TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                marginX = marginX + lebarproduk;
                XRect TableHeaderProductCondition = new XRect(marginX, marginY, lebarstatus, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductCondition);
                tf.DrawString("Status", BodyBold, XBrushes.Black, TableHeaderProductCondition, XStringFormats.TopLeft);

                marginX = marginX + lebarstatus;
                XRect TableHeaderProductQuantity = new XRect(marginX, marginY, lebarstorage, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("Storage", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                marginX = marginX + lebarstorage;
                TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                int totalhalaman = 1;
                if (model.OutSalesOrderProducts.Count >= 17)
                {
                    decimal totallist = 26;
                    totalhalaman = (int)Math.Ceiling((model.OutSalesOrderProducts.Count - 16) / totallist) + 1;
                }

                int halaman = 1;
                XRect Footer = new XRect(0, 40 + (page.Height - 40 * 2) + 5, page.Width, page.Height - (40 + (page.Height - 40 * 2)));
                //gfx.DrawRectangle(XPens.Black, Footer);
                temp = "Page " + (halaman).ToString() + " of " + totalhalaman;
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                for (int i = 0; i < model.OutSalesOrderProducts.Count; i++)
                {
                    for (int j = 0; j < model.OutSalesOrderProducts[i].OutSalesOrderStorages.Count; j++)
                    {
                        marginX = 20 * 2;
                        marginY = marginY + tinggi;
                        tinggi = 25;

                        TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                        TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                        temp = (i + 1).ToString();
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, Body, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                        TableHeaderProductName = new XRect(marginX + 2, marginY + 2, lebarproduk - 2, tinggi);
                        temp = "SKU : " + model.OutSalesOrderProducts[i].MasProductData.SKU + "\n" +
                            model.OutSalesOrderProducts[i].MasProductData.ProductName;
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);

                        marginX = marginX + lebarproduk;
                        TableHeaderProductCondition = new XRect(marginX, marginY, lebarstatus, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductCondition);
                        TableHeaderProductCondition = new XRect(marginX, marginY + 5, lebarstatus, tinggi);
                        temp = model.OutSalesOrderProducts[i].MasProductData.ProductCondition;
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductCondition, XStringFormats.TopLeft);

                        marginX = marginX + lebarstatus;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarstorage, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarstorage, tinggi);
                        temp = model.OutSalesOrderProducts[i].OutSalesOrderStorages[j].InvStorageCode.InvStorageBin.BinName.ToString();
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        marginX = marginX + lebarstorage;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        TableHeaderProductQuantity = new XRect(marginX, marginY + 5, lebarno, tinggi);
                        temp = model.OutSalesOrderProducts[i].OutSalesOrderStorages[j].QtyPick.ToString();
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);


                        if (marginY > (page.Height - 60 * 2) - 50)
                        {
                            page = document.AddPage();
                            page.Size = PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            marginX = 40;
                            marginY = 40 / 2;
                            temp = "";

                            tf = new XTextFormatter(gfx);

                            //HEADER
                            lebar = page.Width - marginX * 2;
                            tinggi = marginY;
                            rect = new XRect(marginX, marginY, lebar, tinggi);
                            tf.Alignment = XParagraphAlignment.Right;
                            tf.DrawString("Date : " + DateTime.Now.ToString("dd / MM / yyyy"), AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                            tf.Alignment = XParagraphAlignment.Left;
                            tf.DrawString("No. Form : FM-WH-005", AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                            //gfx.DrawRectangle(XPens.Black, rect);

                            marginY = marginY + tinggi;
                            tinggi = 15;
                            TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                            gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                            tf.Alignment = XParagraphAlignment.Center;
                            tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                            marginX = marginX + lebarno;
                            TableHeaderProductName = new XRect(marginX, marginY, lebarproduk, tinggi);
                            gfx.DrawRectangle(XPens.Black, TableHeaderProductName);
                            tf.Alignment = XParagraphAlignment.Center;
                            tf.DrawString("Produk", BodyBold, XBrushes.Black, TableHeaderProductName, XStringFormats.TopLeft);


                            marginX = marginX + lebarproduk;
                            TableHeaderProductQuantity = new XRect(marginX, marginY, lebarstatus, tinggi);
                            gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                            tf.DrawString("Status", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                            marginX = marginX + lebarstatus;
                            TableHeaderProductQuantity = new XRect(marginX, marginY, lebarstorage, tinggi);
                            gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                            tf.DrawString("Storage", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                            marginX = marginX + lebarstorage;
                            TableHeaderProductQuantity = new XRect(marginX, marginY, lebarno, tinggi);
                            gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                            tf.DrawString("Qty", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                            //gfx.DrawRectangle(XPens.Black, Footer);
                            temp = "Page " + (halaman + 1).ToString() + " of " + totalhalaman;
                            tf.Alignment = XParagraphAlignment.Center;
                            tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                            halaman++;
                        }
                    }
                }

                //TABLE FOOTER KIRI
                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 15;
                
                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 30;

                XRect TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(".............................., " + DateTime.Now.ToString("... / MM / yyyy") + "\n Diserahkan Oleh,", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);


                marginX = marginX + tempat1 + tempat2;
                XRect TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("\n Diterima Oleh,", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 60;

                TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);

                marginX = marginX + tempat1 + tempat2;
                TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);


                System.IO.File.Delete(webRootPath + "\\img\\QRCode\\ItemProducts\\" + model.OrderId.ToString() + ".png");

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        //FM-WH-006
        [HttpPost]
        public async Task<IActionResult> HandoverManifest(string HouseCode, Guid TenantId, DateTime Date, int OrdCourier)
        {
            var tenant = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                filter:
                    m => m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));

            var warehouse = await _unitOfWork.HouseCode.GetSingleOrDefaultAsync(
                filter:
                    m => m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));

            var model = await _unitOfWork.SalesOrderDispatch.GetAllAsync(
                filter:
                    m => m.OrdCourier == OrdCourier &&
                    m.Flag == 1 &&
                    m.OutSalesOrder.TenantId == TenantId &&
                    m.OutSalesOrder.HouseCode == HouseCode &&
                    m.DatedHandOvered.Year == Date.Year &&
                    m.DatedHandOvered.Month == Date.Month &&
                    m.DatedHandOvered.Day == Date.Day,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutSalesOrder.OutSalesOrderProducts));

            if (model.Count < 1)
            {
                TempData["error"] = "Data notfound!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Handover Manifest";

                XFont Tittle = new XFont("Arial", 14, XFontStyle.Bold);
                XFont Subtitle = new XFont("Arial", 11, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 9);
                XFont BodyBold = new XFont("Arial", 9, XFontStyle.Bold);
                XFont Alert = new XFont("Arial", 8);
                XFont AlertBold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                string temp = "";

                XTextFormatter tf = new XTextFormatter(gfx);

                //PDF.setImage(gfx, webRootPath + "\\img\\Logo\\confidential.png", 60, 150, 516, 434);

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, 75, 45);
                //PDF.writeText(gfx, "Manifest Date : " + DateTime.Now.ToString("dd / MM / yyyy"), Body, 25, 85, 0);

                XRect rect = new XRect(25, 85, 200, 30);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "No. Form   : FM-WH-006 \n" +
                       "Date           : " + DateTime.Now.ToString("dd / MM / yyyy");
                tf.DrawString(temp, AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);

                //HEADER BODY
                double lebar = 80;
                double tinggi = 80;
                double marginX = 20;
                double marginY = 100;
                lebar = page.Width - marginX * 2;
                tinggi = 15;

                rect = new XRect(marginX, marginY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                temp = "MANIFEST HANDOVER";
                tf.DrawString(temp, Tittle, XBrushes.Black, rect, XStringFormats.TopLeft);

                marginY = marginY + 20;
                tinggi = 25;
                rect = new XRect(marginX, marginY, lebar, tinggi);
                temp = DateTime.Now.ToString(tenant.TenantCode + "-yyyyMMdd");
                tf.DrawString(temp, Subtitle, XBrushes.Black, rect, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                double tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                double tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = 50;

                //IDENTITAS KIRI BODY
                tempat1 = ((page.Width - marginX * 2) / 2) / 3;
                tempat2 = ((page.Width - marginX * 2) / 2) - (tempat1);
                tinggi = tinggi + 40;

                XRect IdentKiri = new XRect(marginX, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Tenant \n" +
                    "Phone Number \n" +
                    "Address";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                XRect IdentKiriValue = new XRect(marginX + tempat1, marginY, tempat2 - 10, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + tenant.Name + "\n" +
                        ": " + tenant.PhoneNumber + "\n" +
                        ": " + tenant.Address +
                        ", " + tenant.MasKelurahan.KelName +
                        ", " + tenant.MasKelurahan.MasKecamatan.KecName +
                        ", " + tenant.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + tenant.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + tenant.KodePos +
                        ".";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //IDENTITAS KANAN BODY
                IdentKiri = new XRect(marginX + tempat1 + tempat2, marginY, tempat1, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "Warehouse \n" +
                    "Office Phone \n" +
                    "Address";
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiri, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                IdentKiriValue = new XRect(marginX + tempat1 * 2 + tempat2, marginY, tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + warehouse.HouseName + "\n" +
                        ": " + warehouse.OfficePhone + "\n" +
                        ": " + warehouse.Address +
                        ", " + warehouse.MasKelurahan.KelName +
                        ", " + warehouse.MasKelurahan.MasKecamatan.KecName +
                        ", " + warehouse.MasKelurahan.MasKecamatan.MasKabupaten.KabName +
                        ", " + warehouse.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName +
                        ", " + warehouse.KodePos +
                        "."; ;
                tf.DrawString(temp, Body, XBrushes.Black, IdentKiriValue, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT TITLE
                marginX = 20 * 2;
                marginY = marginY + tinggi;
                lebar = page.Width - marginX * 2;
                tinggi = 20;

                XRect SubTitle = new XRect(marginX, marginY, lebar, tinggi);
                temp = "SALES ORDER LIST";
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(temp, Subtitle, XBrushes.Black, SubTitle, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //PRODUCT HEADER
                marginY = marginY + tinggi;
                var lebarno = lebar / 2 / 8;
                tinggi = 15;
                XRect TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                var lebarorderno = lebar / 2 / 2;
                XRect TableHeaderSONumber = new XRect(marginX, marginY, lebarorderno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderSONumber);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Order Number", BodyBold, XBrushes.Black, TableHeaderSONumber, XStringFormats.TopLeft);

                marginX = marginX + lebarorderno;
                XRect TableHeaderProductQty = new XRect(marginX, marginY, lebarno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQty);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("Qty", BodyBold, XBrushes.Black, TableHeaderProductQty, XStringFormats.TopLeft);

                marginX = marginX + lebarno;
                XRect TableHeaderProductPrice = new XRect(marginX, marginY, lebarorderno, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductPrice);
                tf.DrawString("Cnee. Name", BodyBold, XBrushes.Black, TableHeaderProductPrice, XStringFormats.TopLeft);

                marginX = marginX + lebarorderno;
                var lebaradress = lebarorderno + lebarno + lebarno;
                XRect TableHeaderProductQuantity = new XRect(marginX, marginY, lebaradress, tinggi);
                gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                tf.DrawString("Cnee. Address", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                int totalhalaman = 1;
                if (model.Count >= 14)
                {
                    decimal totallist = 21;
                    totalhalaman = (int)Math.Ceiling((model.Count - 13) / totallist) + 1;
                }

                int halaman = 1;
                XRect Footer = new XRect(0, 40 + (page.Height - 40 * 2) + 5, page.Width, page.Height - (40 + (page.Height - 40 * 2)));
                //gfx.DrawRectangle(XPens.Black, Footer);
                temp = "Page " + (halaman).ToString() + " of " + totalhalaman;
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);

                for (int i = 0; i < model.Count; i++)
                {
                    marginX = 20 * 2;
                    marginY = marginY + tinggi;
                    lebar = page.Width - marginX * 2;
                    tinggi = 33;

                    TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                    TableHeaderNo = new XRect(marginX, marginY + 10, lebarno, tinggi);
                    temp = (i + 1).ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderSONumber = new XRect(marginX, marginY, lebarorderno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderSONumber);
                    TableHeaderSONumber = new XRect(marginX + 2, marginY + 8, lebarorderno - 2, tinggi);
                    temp = model[i].OrderId;
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderSONumber, XStringFormats.TopLeft);

                    marginX = marginX + lebarorderno;
                    TableHeaderProductQty = new XRect(marginX, marginY, lebarno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQty);
                    TableHeaderProductQty = new XRect(marginX + 2, marginY + 8, lebarno - 2, tinggi);
                    temp = model[i].OutSalesOrder.OutSalesOrderProducts.Sum(m => m.Quantity).ToString();
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQty, XStringFormats.TopLeft);

                    marginX = marginX + lebarno;
                    TableHeaderProductPrice = new XRect(marginX, marginY, lebarorderno, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductPrice);
                    TableHeaderProductPrice = new XRect(marginX + 2, marginY + 8, lebarorderno - 2, tinggi);
                    temp = model[i].OutSalesOrder.OutSalesOrderConsignee.CneeName;
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductPrice, XStringFormats.TopLeft);

                    marginX = marginX + lebarorderno;
                    TableHeaderProductQuantity = new XRect(marginX, marginY, lebaradress, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    TableHeaderProductQuantity = new XRect(marginX + 2, marginY, lebaradress - 2, tinggi);
                    temp = model[i].OutSalesOrder.OutSalesOrderConsignee.CneeAddress + ", " +
                        model[i].OutSalesOrder.OutSalesOrderConsignee.MasKelurahan.KelName + ", " +
                        model[i].OutSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.KecName + ", " +
                        model[i].OutSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.KabName + ", " +
                        model[i].OutSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName + ", " +
                        model[i].OutSalesOrder.OutSalesOrderConsignee.OrdZipCode + ". ";
                    tf.DrawString(temp, Body, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                    if (marginY > (page.Height - 60 * 2) - 55)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        marginX = 40;
                        marginY = 40 / 2;
                        temp = "";

                        tf = new XTextFormatter(gfx);

                        //HEADER
                        lebar = page.Width - marginX * 2;
                        tinggi = marginY;
                        rect = new XRect(marginX, marginY, lebar, tinggi);
                        tf.Alignment = XParagraphAlignment.Right;
                        tf.DrawString("Date : " + DateTime.Now.ToString("dd / MM / yyyy"), AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString("No. Form : FM-WH-006", AlertBold, XBrushes.Black, rect, XStringFormats.TopLeft);
                        //gfx.DrawRectangle(XPens.Black, rect);

                        marginY = marginY + tinggi;
                        marginY = marginY + tinggi;
                        tinggi = 15;
                        TableHeaderNo = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderNo);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("No", BodyBold, XBrushes.Black, TableHeaderNo, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        lebarorderno = lebar / 2 / 2;
                        TableHeaderSONumber = new XRect(marginX, marginY, lebarorderno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderSONumber);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("Order Number", BodyBold, XBrushes.Black, TableHeaderSONumber, XStringFormats.TopLeft);

                        marginX = marginX + lebarorderno;
                        TableHeaderProductQty = new XRect(marginX, marginY, lebarno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQty);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString("Qty", BodyBold, XBrushes.Black, TableHeaderProductQty, XStringFormats.TopLeft);

                        marginX = marginX + lebarno;
                        TableHeaderProductPrice = new XRect(marginX, marginY, lebarorderno, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductPrice);
                        tf.DrawString("Cnee. Name", BodyBold, XBrushes.Black, TableHeaderProductPrice, XStringFormats.TopLeft);

                        marginX = marginX + lebarorderno;
                        TableHeaderProductQuantity = new XRect(marginX, marginY, lebaradress, tinggi);
                        gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                        tf.DrawString("Cnee. Address", BodyBold, XBrushes.Black, TableHeaderProductQuantity, XStringFormats.TopLeft);

                        temp = "Page " + (halaman + 1).ToString() + " of " + totalhalaman;
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(temp, BodyBold, XBrushes.Black, Footer, XStringFormats.TopLeft);
                        halaman++;
                    }
                }

                //TABLE FOOTER KIRI


                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 30;

                XRect TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(".............................., " + DateTime.Now.ToString("... / MM / yyyy") + "\n Diserahkan Oleh,", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);


                marginX = marginX + tempat1 + tempat2;
                XRect TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("\n Diterima Oleh,", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                marginY = marginY + tinggi;
                marginX = 20 * 2;
                tinggi = 60;

                TTDKiri = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKiri = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKiri, XStringFormats.TopLeft);

                marginX = marginX + tempat1 + tempat2;
                TTDKanan = new XRect(marginX, marginY, tempat1 + tempat2, tinggi);
                TTDKanan = new XRect(marginX, marginY + tinggi, tempat1 + tempat2, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("..................................................", Body, XBrushes.Black, TTDKanan, XStringFormats.TopLeft);

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        public async Task<IActionResult> ProductIKU(int DOProductId)
        {
            var model = await _unitOfWork.ItemProduct.GetAllAsync(
                filter:
                    m => m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProduct.MasProductData.InvStorageSize)
                    .Include(m => m.IncDeliveryOrderProduct.MasProductData.InvStorageCategory)
                    .Include(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone)
                    .Include(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode));

            List<IncItemProduct> ItemProducts = model.ToList();

            foreach (var item in ItemProducts)
            {
                var writer = new QRCodeWriter();

                var resultBit = writer.encode(item.IKU.ToString(), BarcodeFormat.QR_CODE, 150, 150);
                var matrix = resultBit;
                int scale = 2;
                Bitmap result = new Bitmap(matrix.Width * scale, matrix.Height * scale);
                for (int x = 0; x < matrix.Width; x++)
                {
                    for (int y = 0; y < matrix.Height; y++)
                    {
                        Color pixel = matrix[x, y] ? Color.Black : Color.White;
                        for (int i = 0; i < scale; i++)
                        {
                            for (int j = 0; j < scale; j++)
                                result.SetPixel((x * scale) + i, (y * scale) + j, pixel);
                        }
                    }
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    result.Save(webRootPath + "\\img\\QRCode\\ItemProducts\\" + item.IKU.ToString() + ".png");
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                string webRootPath = _webHostEnvironment.WebRootPath;

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Print IKU";

                XFont Arial12 = new XFont("Arial", 12);
                XFont Arial10 = new XFont("Arial", 10);
                XFont Arial8 = new XFont("Arial", 8);
                XFont Arial6 = new XFont("Arial", 6);
                XFont Arial6Bold = new XFont("Arial", 6, XFontStyle.Bold);
                XFont Arial5 = new XFont("Arial", 5);
                XFont Arial5Bold = new XFont("Arial", 5, XFontStyle.Bold);
                XFont Arial4 = new XFont("Arial", 4);
                XFont Arial4Bold = new XFont("Arial", 4, XFontStyle.Bold);
                PdfPage page;
                XGraphics gfx;
                page = document.AddPage();
                page.Size = PageSize.A4;
                gfx = XGraphics.FromPdfPage(page);

                double xPos = 0;
                double yPos = 0;
                XTextFormatter tf;
                for (int j = 0; j < ItemProducts.Count(); j++)
                {
                    if (j % 4 == 0)
                    {
                        if (j == 0)
                        {
                            xPos = 0;
                            yPos = 0;
                        }
                        else
                        {
                            xPos = 0;
                            yPos += 60.09449;
                        }
                    }
                    else
                    {
                        xPos += 148.8189;
                    }

                    if (j + 1 % 56 == 0)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.RA5;
                        gfx = XGraphics.FromPdfPage(page);
                        xPos = 0;
                        yPos = 0;
                    }
                    tf = new XTextFormatter(gfx);

                    var houseCode = ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode.ToString().ToUpper().Trim();
                    var zoneCode = ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneCode.ToString().ToUpper().Trim();
                    var rowCode = ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.RowCode.ToString().ToUpper().Trim();
                    var sectionCode = ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.SectionCode.ToString().ToUpper().Trim();
                    var columnCode = ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.ColumnCode.ToString().ToUpper().Trim();
                    var levelCode = ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.LevelCode.ToString().ToUpper().Trim();
                    var binCode = ItemProducts[j].InvStorageCode.InvStorageBin.BinCode.ToString().ToUpper().Trim();

                    var product = ItemProducts[j].IncDeliveryOrderProduct.MasProductData;

                    gfx.DrawImage(XImage.FromFile(webRootPath + "\\img\\QRCode\\ItemProducts\\" + ItemProducts[j].IKU.ToString().Trim() + ".png"), xPos - 2, yPos + 4, 58, 58);

                    gfx.DrawString("IKU", Arial6, XBrushes.Black, xPos + 25, yPos + 9);
                    //gfx.DrawString(ItemProducts[j].IKU.ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 11);
                    gfx.DrawString("SKU : " + product.SKU.ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 11 + 6);

                    var pjg_product = product.ProductName.Length;

                    if (pjg_product < 25)
                    {
                        gfx.DrawString(product.ProductName.ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 14 + 8 * 2);
                    }
                    else
                    {
                        gfx.DrawString(product.ProductName.Substring(0, 24).ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 11 + 8 * 2);
                        if (pjg_product < 50)
                        {
                            gfx.DrawString(product.ProductName.Substring(24).ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 9 + 8 * 3);
                        }
                        else
                        {
                            char[] charArr = product.ProductName.ToString().ToUpper().ToCharArray();
                            char[] string1 = new char[24];
                            for (int i = 0; i < 24; i++)
                            {
                                string1[i] = charArr[i + 24];
                            }
                            string charsStr = new string(string1);
                            gfx.DrawString(charsStr, Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 9 + 8 * 3);
                        }
                    }

                    gfx.DrawString(ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode.HouseName.ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 11 + 8 * 4);
                    gfx.DrawString(houseCode + " - " + ItemProducts[j].InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneName.ToString().ToUpper(), Arial6, XBrushes.Black, xPos + 58 - 4, yPos + 9 + 8 * 5);
                    gfx.DrawString("BIN : " + rowCode.ToString().Substring((houseCode + zoneCode).Length) + " / " + levelCode.ToString().Substring(rowCode.Length) + " / " + binCode.ToString().Substring(levelCode.Length), Arial6, XBrushes.Black, xPos + 58 - 4, yPos - 1 + 8 * 7);

                    //gfx.DrawRectangle(XPens.Black, xPos, yPos, 148.8189, 60.09449);
                    System.IO.File.Delete(webRootPath + "\\img\\QRCode\\ItemProducts\\" + ItemProducts[j].IKU.ToString() + ".png");
                }

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        public async Task<IActionResult> ProductSKU(int DOProductId)
        {
            var product = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductData.InvStorageZone)
                    .Include(m => m.MasProductData.InvStorageCategory)
                    .Include(m => m.MasProductData.InvStorageSize)
                    .Include(m => m.IncDeliveryOrder.MasDataTenant)
                    .Include(m => m.IncDeliveryOrder.MasHouseCode));

            if (product == null)
            {
                return RedirectPermanent("~/Error/Notfound");
            }

            string dirFile = _webHostEnvironment.WebRootPath + "\\img\\QRCode\\ItemProducts\\" + product.DOProductCode.ToString() + ".png";
            var matrix = new QRCodeWriter().encode(product.DOProductCode.ToString(), BarcodeFormat.QR_CODE, 150, 150);
            int scale = 2;
            Bitmap result = new Bitmap(matrix.Width * scale, matrix.Height * scale);
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < scale; i++)
                    {
                        for (int j = 0; j < scale; j++)
                            result.SetPixel((x * scale) + i, (y * scale) + j, pixel);
                    }
                }
                result.Save(dirFile);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Print SKU";

                XFont Title = new XFont("Arial", 10, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Description = new XFont("Arial", 6);
                XFont Arial6Bold = new XFont("Arial", 6, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Width = 377.95275591;
                page.Height = 188.97637795;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XTextFormatter tf = new XTextFormatter(gfx);

                double pointX = 5;
                double pointY = 5;
                double lebar = page.Width;
                double tinggi = page.Height;
                string temp = "";

                //LAYOUT
                double marginX = 5;
                double marginY = 5;
                lebar = page.Width - marginX * 2;
                tinggi = page.Height - marginY * 2;
                XRect rect = new XRect(marginX, marginY, lebar, tinggi);
                gfx.DrawRectangle(XPens.Black, rect);

                //KIRI BARCODE
                pointX = marginX;
                pointY = marginY;
                lebar = 140;
                tinggi = 140;
                gfx.DrawImage(XImage.FromFile(dirFile), pointX + 1, pointY + 1, lebar, tinggi);

                //KIRI DETIL PERUSAHAAN
                //pointX = pointX + (marginX * 3);
                pointY = marginY + tinggi;
                //lebar = lebar - (marginX * 2) - (marginX * 3);
                tinggi = page.Height - tinggi - (marginY * 2) - (marginY * 3);
                rect = new XRect(pointX, pointY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString("WMS Deal \n Warehouse Management System", Body, XBrushes.Black, rect, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //KANAN PRODUCT NAME
                pointX = pointX + lebar;
                pointY = marginY + 15;
                lebar = page.Width - lebar - (marginX * 3 * 2);
                tinggi = 40;
                rect = new XRect(pointX, pointY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Center;

                var panjang_productname = product.MasProductData.ProductName.Length;
                temp = product.MasProductData.ProductName.ToUpper();
                tf.DrawString(temp, Title, XBrushes.Black, rect, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //KANAN PRODUCT DETAILS KIRI
                pointY = pointY + tinggi;
                lebar = lebar / 3;
                tinggi = page.Height - tinggi - (marginY * 2) - (marginY * 3 * 2);
                rect = new XRect(pointX, pointY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = "SKU \n" +
                        "\n" +
                        "DO Number \n" +
                        "Tenant \n" +
                        "Warehouse \n" +
                        "Zona \n \n" +
                        "Size \n" +
                        "Kategori \n \n" +
                        "Tanggal Kirim";
                tf.DrawString(temp, Body, XBrushes.Black, rect, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                //KANAN PRODUCT DETAILS KANAN
                var panjang_sku = product.MasProductData.SKU.Length;
                string sku1 = "";
                string sku2 = "";
                if (panjang_sku < 30)
                {
                    sku1 = product.MasProductData.SKU.ToUpper();
                }
                else
                {
                    if (panjang_sku < 60)
                    {
                        sku1 = product.MasProductData.SKU.ToUpper().Substring(0, 29);
                        sku2 = product.MasProductData.SKU.ToUpper().Substring(29, panjang_sku - 29);
                    }
                    else
                    {
                        sku1 = product.MasProductData.SKU.ToUpper().Substring(0, 29);
                        sku2 = product.MasProductData.SKU.ToUpper().Substring(29, 58);
                    }
                }
                pointX = pointX + lebar;
                lebar = lebar * 2;
                rect = new XRect(pointX, pointY, lebar, tinggi);
                tf.Alignment = XParagraphAlignment.Left;
                temp = ": " + sku1 + "\n" +
                        "  " + sku2 + "\n" +
                        ": " + product.IncDeliveryOrder.DOSupplier + " \n" +
                        ": " + product.IncDeliveryOrder.MasDataTenant.Name + " \n" +
                        ": " + product.IncDeliveryOrder.MasHouseCode.HouseName + " \n" +
                        ": " + product.MasProductData.InvStorageZone.ZoneName + " \n \n" +
                        ": " + product.MasProductData.InvStorageSize.SizeName + " \n" +
                        ": " + product.MasProductData.InvStorageCategory.StorageCategoryName + "\n \n" +
                        ": " + product.IncDeliveryOrder.DateDelivered;
                tf.DrawString(temp, Body, XBrushes.Black, rect, XStringFormats.TopLeft);
                //gfx.DrawRectangle(XPens.Black, rect);

                document.Save(stream, false);

                System.IO.File.Delete(dirFile);

                return File(stream.ToArray(), "application/pdf");
            }
        }
    }
}