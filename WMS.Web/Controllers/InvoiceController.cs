using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using WMS.Utility;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp.Drawing;
using System.Globalization;
using Microsoft.CodeAnalysis;
using System;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InvoiceController(IUnitOfWork unitOfWork, INotyfService notyf, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _notyf = notyf;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var invoice = await _unitOfWork.Invoice.GetAllAsync(includeProperties: m => m.Include(x => x.MasDataTenant));
            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.Flag == FlagEnum.Active), "TenantId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MasInvoicing model, int InputMaterial)
        {
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.Flag == FlagEnum.Active), "TenantId", "Name");

            if (model == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return View(model);
            }

            string periode = model.InvoicePeriod.ToString();

            string[] subs = periode.Split('-');


            if (subs.Length != 2)
            {
                TempData["error"] = "Invalid input date!";
                return View(model);
            }

            if (CheckInt(subs[0]) == false || CheckInt(subs[1]) == false)
            {
                TempData["error"] = "Invalid input date!";
                return View(model);
            }

            int tanggal = 1;

            var model_date = new DateTime(Int16.Parse(subs[1]), Int16.Parse(subs[0]), tanggal);
            var this_date = DateTime.Now;

            if (model_date.Year > this_date.Year)
            {
                TempData["error"] = "Invalid input date!";
                return View(model);
            }

            if (model_date.Year == this_date.Year)
            {
                if (model_date.Month >= this_date.Month)
                {
                    TempData["error"] = "Invalid input date!";
                    return View(model);
                }
            }

            var checkerperiode = await _unitOfWork.Invoice.GetSingleOrDefaultAsync(
                filter:
                    m => m.TenantId == model.TenantId &&
                    m.InvoicePeriod == model.InvoicePeriod);

            if (checkerperiode != null)
            {
                TempData["error"] = "Invoice already exist!";
                return View(model);
            }

            var pricing = await _unitOfWork.Pricing.GetSingleOrDefaultAsync(
                filter:
                    m => m.TenantId == model.TenantId &&
                    m.Flag == FlagEnum.Active,
                includeProperties:
                    m => m.Include(m => m.MasPricingAdditionals));

            if (pricing == null)
            {
                TempData["error"] = "Price list unavailable!";
                return View(model);
            }

            var Code = "INV-";
            var Tanggal = this_date.ToString("yyMM-");
            var Last = 1.ToString("D2");

            string InvoiceNumber = Code + Tanggal + Last;
            if (await _unitOfWork.Invoice.AnyAsync(m => m.InvoiceNumber == Code + Tanggal + Last))
            {
                var temp = await _unitOfWork.Invoice.GetAllAsync(m => m.InvoiceNumber.Contains(Code + Tanggal));
                int LastCount = int.Parse(temp.Max(m => m.InvoiceNumber.Substring(Code.Length + Tanggal.Length)));
                InvoiceNumber = Code + Tanggal + (LastCount + 1).ToString("0#");
            }

            model.InvoiceNumber = InvoiceNumber;
            model.CreatedDate = this_date;
            model.CreatedBy = User.FindFirst("UserName")?.Value;

            await _unitOfWork.Invoice.AddAsync(model);

            decimal jumlah = 0;

            var storageFee = new MasInvoicingDetail
            {
                InvoiceNumber = InvoiceNumber,
                CostName = "Storage Fee",
                CostItem = "M2",
                CostTotal = pricing.StorageMin,
                CostAmount = pricing.StorageRates,
                CostGrandAmount = ((decimal)pricing.StorageMin) * pricing.StorageRates
            };
            await _unitOfWork.InvoiceDetail.AddAsync(storageFee);

            jumlah = jumlah + storageFee.CostGrandAmount;

            var delivery = await _unitOfWork.DeliveryOrder.GetAllAsync(
                filter:
                    m => m.TenantId == model.TenantId &&
                    m.Status != SD.FlagDO_OPN &&
                    m.IncDeliveryOrderProducts
                        .Any(m => m.IncDeliveryOrderArrivals.InvProductPutaways
                            .Any(m => m.DatePutaway.Month == model_date.Month && m.DatePutaway.Year == model_date.Year)) ||
                    m.IncDeliveryOrderProducts
                        .Any(m => m.IncItemProducts
                            .Any(m => m.DatePutedAway.Month == model_date.Month && m.DatePutedAway.Year == model_date.Year)),
                includeProperties:
                    m => m.Include(m=> m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncItemProducts)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncDeliveryOrderArrivals.InvProductPutaways));

            float total_handling = 0;
            for (int i = 0; i < delivery.Count; i++)
            {
                for(int j = 0; j < delivery[i].IncDeliveryOrderProducts.Count; j++)
                {
                    if (delivery[i].IncDeliveryOrderProducts[j].IncDeliveryOrderArrivals != null)
                    {
                        if(delivery[i].IncDeliveryOrderProducts[j].IncDeliveryOrderArrivals.InvProductPutaways != null)
                        {
                            total_handling = total_handling + (delivery[i].IncDeliveryOrderProducts[j].MasProductData.VolWight/100 * delivery[i].IncDeliveryOrderProducts[j].IncDeliveryOrderArrivals.InvProductPutaways.Sum(m => m.Quantity));
                        }

                        if(delivery[i].IncDeliveryOrderProducts[j].IncItemProducts != null)
                        {
                            total_handling = total_handling + (delivery[i].IncDeliveryOrderProducts[j].MasProductData.VolWight / 100 * delivery[i].IncDeliveryOrderProducts[j].IncItemProducts.Count);
                        }
                    }
                }
            }

            var totalHandling = new MasInvoicingDetail
            {
                InvoiceNumber = InvoiceNumber,
                CostName = "Total Handling",
                CostItem = "M3",
                CostTotal = total_handling,
                CostAmount = 0,
                CostGrandAmount = 0
            };
            await _unitOfWork.InvoiceDetail.AddAsync(totalHandling);

            var minHandling = new MasInvoicingDetail
            {
                InvoiceNumber = InvoiceNumber,
                CostName = "Handling Fee (Min. Charge)",
                CostItem = "M3",
                CostTotal = pricing.ReceivingFeeMin,
                CostAmount = pricing.ReceivingFeeRates,
                CostGrandAmount = (decimal)pricing.ReceivingFeeMin * pricing.ReceivingFeeRates
            };
            await _unitOfWork.InvoiceDetail.AddAsync(minHandling);
            jumlah = jumlah + minHandling.CostGrandAmount;

            var addHandling = new MasInvoicingDetail();
            addHandling.InvoiceNumber = InvoiceNumber;
            addHandling.CostName = "Handling Fee Additional";
            addHandling.CostItem = "M3";
            if (totalHandling.CostTotal > pricing.ReceivingFeeMin)
            {
                addHandling.CostTotal = totalHandling.CostTotal - pricing.ReceivingFeeMin;
                addHandling.CostAmount = pricing.ReceivingFeeRates;
                addHandling.CostGrandAmount = (decimal)addHandling.CostTotal * pricing.ReceivingFeeRates;
            }
            else
            {
                addHandling.CostTotal = 0;
                addHandling.CostAmount = 0;
                addHandling.CostGrandAmount = 0;
            }
            await _unitOfWork.InvoiceDetail.AddAsync(addHandling);
            jumlah = jumlah + addHandling.CostGrandAmount;

            var repack = await _unitOfWork.Repack.GetAllAsync(
                filter:
                    m => m.MasProductData.TenantId == model.TenantId &&
                    m.DateRepacked.Month == model_date.Month &&
                    m.DateRepacked.Year == model_date.Year,
                includeProperties:
                    m => m.Include(m => m.MasProductData));

            for (int i = 0; i < pricing.MasPricingAdditionals.Count; i++)
            {
                var addPricing = new MasInvoicingDetail();
                addPricing.InvoiceNumber = InvoiceNumber;
                addPricing.CostName = pricing.MasPricingAdditionals[i].AddName;
                addPricing.CostItem = pricing.MasPricingAdditionals[i].AddFeeType;
                if (pricing.MasPricingAdditionals[i].AddName == "Repack")
                {
                    if(repack.Sum(m => m.Quantity) < pricing.MasPricingAdditionals[i].AddMin)
                    {
                        addPricing.CostTotal = repack.Sum(m => m.Quantity);
                    }
                }
                else
                {
                    addPricing.CostTotal = pricing.MasPricingAdditionals[i].AddMin;
                }
                addPricing.CostAmount = pricing.MasPricingAdditionals[i].AddFee;
                addPricing.CostGrandAmount = (decimal)addPricing.CostTotal * addPricing.CostAmount;

                await _unitOfWork.InvoiceDetail.AddAsync(addPricing);
                jumlah = jumlah + addPricing.CostGrandAmount;
            }

            var packMaterial = new MasInvoicingDetail
            {
                InvoiceNumber = InvoiceNumber,
                CostName = "Packing Material",
                CostItem = "-",
                CostGrandAmount = InputMaterial,
            };
            await _unitOfWork.InvoiceDetail.AddAsync(packMaterial);
            jumlah = jumlah + packMaterial.CostGrandAmount;

            var feeManagement = new MasInvoicingDetail
            {
                InvoiceNumber = InvoiceNumber,
                CostName = "Management Fee",
                CostItem = "%",
                CostTotal = pricing.ManagementFee,
                CostAmount = jumlah,
                CostGrandAmount = jumlah * ((decimal)pricing.ManagementFee / 100)
            };
            await _unitOfWork.InvoiceDetail.AddAsync(feeManagement);

            decimal subtotal = jumlah + feeManagement.CostGrandAmount;

            var pph = new MasInvoicingDetail
            {
                InvoiceNumber = InvoiceNumber,
                CostName = "PPh",
                CostItem = "%",
                CostTotal = pricing.PPh,
                CostAmount = subtotal,
                CostGrandAmount = subtotal * ((decimal)pricing.PPh / 100)
            };
            await _unitOfWork.InvoiceDetail.AddAsync(pph);

            model.TotalAmount = subtotal + pph.CostGrandAmount;

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Invoice generated successfully!";
            return RedirectToAction("Index");
        }

        private static bool CheckInt(string input)
        {
            return int.TryParse(input, out _);
        }

        public async Task<IActionResult> PrintInvoiceAsync(string InvoiceNumber)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var model = await _unitOfWork.Invoice.GetSingleOrDefaultAsync(
                filter:
                    m => m.InvoiceNumber == InvoiceNumber,
                includeProperties:
                    m => m.Include(m => m.MasInvoicingDetails)
                    .Include(m => m.MasDataTenant.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));

            if(model == null)
            {
                TempData["error"] = "Invoice Notfound!";
                return RedirectToAction("Index");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                PdfDocument document = new PdfDocument();

                XFont Arial25 = new XFont("Arial", 25);
                XFont Arial25Bold = new XFont("Arial", 25, XFontStyle.Bold);
                XFont Arial20 = new XFont("Arial", 20);
                XFont Arial20Bold = new XFont("Arial", 20, XFontStyle.Bold);
                XFont Arial15 = new XFont("Arial", 15);
                XFont Arial15Bold = new XFont("Arial", 15, XFontStyle.Bold);
                XFont Arial12 = new XFont("Arial", 12);
                XFont Arial12Bold = new XFont("Arial", 12, XFontStyle.Bold);
                XFont Arial10 = new XFont("Arial", 10);
                XFont Arial10Bold = new XFont("Arial", 10, XFontStyle.Bold);
                XFont Arial8 = new XFont("Arial", 8);
                XFont Arial8Bold = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Arial7 = new XFont("Arial", 7);
                XFont Arial7Bold = new XFont("Arial", 7, XFontStyle.Bold);
                XFont Arial6 = new XFont("Arial", 6);
                XFont Arial6Bold = new XFont("Arial", 6, XFontStyle.Bold);
                XFont Arial5 = new XFont("Arial", 5);
                XFont Arial5Bold = new XFont("Arial", 5, XFontStyle.Bold);
                XFont Arial4 = new XFont("Arial", 4);
                XFont Arial4Bold = new XFont("Arial", 4, XFontStyle.Bold);
                XFont Calibri5Bold = new XFont("Calibri", 5, XFontStyle.Bold);
                XFont Calibri6 = new XFont("Calibri", 6);
                XFont Calibri7 = new XFont("Calibri", 7);
                PdfPage page;
                XGraphics gfx;
                page = document.AddPage();
                page.Size = PageSize.Letter;
                gfx = XGraphics.FromPdfPage(page);
                int xwidth = 590;
                int xPos = 5;
                int yPos = 5;
                XTextFormatter tf;

                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\Head.png", 0, -50, 270, 125);
                PDF.setImage(gfx, webRootPath + "\\img\\Logo\\confidential.png", 60, 150, 516, 434);

                gfx.DrawString("WMS DEAL", Arial15Bold, XBrushes.WhiteSmoke, xPos + 70, yPos + 40);
                gfx.DrawString("INVOICE", Arial25Bold, XBrushes.CadetBlue, xPos + 370, yPos + 60);
                gfx.DrawString("Invoice Number : ", Arial8Bold, XBrushes.Black, xPos + 370, yPos + 75);
                PDF.writeRightText(gfx, model.InvoiceNumber, Arial10, xPos + 540, yPos + 65, 0);
                gfx.DrawString("Invoice Period : ", Arial8Bold, XBrushes.Black, xPos + 370, yPos + 90);
                PDF.writeRightText(gfx, model.InvoicePeriod, Arial10, xPos + 540, yPos + 80, 0);

                PDF.writeText(gfx, "Invoice Date : ", Arial8Bold, xPos + 20, yPos + 80, 0);
                PDF.writeText(gfx, DateTime.Now.ToString("dd / MM / yyyy"), Arial10, xPos + 80, yPos + 78, 0);

                gfx.DrawString("INVOICE TO  ", Arial8Bold, XBrushes.Black, xPos + 20, yPos + 120);
                gfx.DrawString(model.MasDataTenant.Name, Arial20Bold, XBrushes.Black, xPos + 20, yPos + 140);

                var alamat_tenant = model.MasDataTenant.MasKelurahan.MasKecamatan.KecName + ", " + model.MasDataTenant.MasKelurahan.MasKecamatan.MasKabupaten.KabName;

                gfx.DrawString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(alamat_tenant), Arial10, XBrushes.Black, xPos + 20, yPos + 155);
                gfx.DrawString("Phone : ", Arial10, XBrushes.Black, xPos + 20, yPos + 170);
                gfx.DrawString(model.MasDataTenant.PhoneNumber, Arial10, XBrushes.Black, xPos + 70, yPos + 170);
                gfx.DrawString("Mail : ", Arial10, XBrushes.Black, xPos + 20, yPos + 185);
                gfx.DrawString(model.MasDataTenant.EmailAddress, Arial10, XBrushes.Black, xPos + 70, yPos + 185);

                gfx.DrawString("INVOICE FROM  ", Arial8Bold, XBrushes.Black, xPos + 370, yPos + 120);
                gfx.DrawString("NCS ", Arial20Bold, XBrushes.Black, xPos + 370, yPos + 140);
                gfx.DrawString("Jln Brigjend Katamso No 7 Slipi ", Arial10, XBrushes.Black, xPos + 370, yPos + 155);
                gfx.DrawString("Phone : ", Arial10, XBrushes.Black, xPos + 370, yPos + 170);
                gfx.DrawString("021 123456", Arial10, XBrushes.Black, xPos + 420, yPos + 170);
                gfx.DrawString("Mail : ", Arial10, XBrushes.Black, xPos + 370, yPos + 185);
                gfx.DrawString("invoice@wmsdeal.com ", Arial10, XBrushes.Black, xPos + 420, yPos + 185);

                gfx.DrawString("ITEM DESCRIPTION", Arial8Bold, XBrushes.Black, xPos + 20, yPos + 230);
                gfx.DrawString("PRICE (IDR)", Arial8Bold, XBrushes.Black, xPos + 325, yPos + 230);
                gfx.DrawString("QTY.", Arial8Bold, XBrushes.Black, xPos + 420, yPos + 230);
                gfx.DrawString("TOTAL (IDR)", Arial8Bold, XBrushes.Black, xPos + 520, yPos + 230);
                gfx.DrawLine(new XPen(XBrushes.Black, 0.7), xPos + 20, yPos + 240, xwidth, yPos + 240);

                CultureInfo culture = new CultureInfo("es-ES");

                var StorageFee = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Storage Fee");
                gfx.DrawString(StorageFee.CostName, Arial10, XBrushes.Black, xPos + 20, yPos + 270);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", StorageFee.CostAmount), Arial10, xPos + 370, yPos + 260, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", StorageFee.CostTotal), Arial10, xPos + 435, yPos + 260, 0);
                char[] subs1 = StorageFee.CostItem.ToCharArray();
                PDF.writeText(gfx, subs1[0].ToString(), Arial10, xPos + 440, yPos + 260, 0);
                PDF.writeText(gfx, subs1[1].ToString(), Arial8, xPos + 450, yPos + 258, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", StorageFee.CostGrandAmount), Arial10, xPos + 570, yPos + 260, 0);

                var TotalHandling = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Total Handling");

                var HandlingFeeMC = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Handling Fee (Min. Charge)");
                var CostTotal = " - " + String.Format(culture, "{0:n0}", TotalHandling.CostTotal);
                if (TotalHandling.CostTotal > HandlingFeeMC.CostTotal)
                {
                    CostTotal = "";
                }
                gfx.DrawString(HandlingFeeMC.CostName + CostTotal, Arial10, XBrushes.Black, xPos + 20, yPos + 300);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", HandlingFeeMC.CostAmount), Arial10, xPos + 370, yPos + 290, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", HandlingFeeMC.CostTotal), Arial10, xPos + 435, yPos + 290, 0);
                char[] subs2 = HandlingFeeMC.CostItem.ToCharArray();
                PDF.writeText(gfx, subs2[0].ToString(), Arial10, xPos + 440, yPos + 290, 0);
                PDF.writeText(gfx, subs2[1].ToString(), Arial8, xPos + 450, yPos + 288, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", HandlingFeeMC.CostGrandAmount), Arial10, xPos + 570, yPos + 290, 0);

                var HandlingFeeAdditional = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Handling Fee Additional");
                gfx.DrawString(HandlingFeeAdditional.CostName, Arial10, XBrushes.Black, xPos + 50, yPos + 330);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", HandlingFeeAdditional.CostAmount), Arial10, xPos + 370, yPos + 320, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", HandlingFeeAdditional.CostTotal), Arial10, xPos + 435, yPos + 320, 0);
                char[] subs3 = HandlingFeeAdditional.CostItem.ToCharArray();
                PDF.writeText(gfx, subs3[0].ToString(), Arial10, xPos + 440, yPos + 320, 0);
                PDF.writeText(gfx, subs3[1].ToString(), Arial8, xPos + 450, yPos + 318, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", HandlingFeeAdditional.CostGrandAmount), Arial10, xPos + 570, yPos + 320, 0);

                gfx.DrawString("Additional Service Charge : ", Arial10, XBrushes.Black, xPos + 20, yPos + 360);

                var Repack = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Repack");
                gfx.DrawString(Repack.CostName, Arial10, XBrushes.Black, xPos + 50, yPos + 380);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", Repack.CostAmount), Arial10, xPos + 370, yPos + 370, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", Repack.CostTotal), Arial10, xPos + 435, yPos + 370, 0);
                PDF.writeText(gfx, Repack.CostItem, Arial10, xPos + 440, yPos + 370, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", Repack.CostGrandAmount), Arial10, xPos + 570, yPos + 370, 0);

                var Relabelling = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Relabelling");
                gfx.DrawString(Relabelling.CostName, Arial10, XBrushes.Black, xPos + 50, yPos + 410);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", Relabelling.CostAmount), Arial10, xPos + 370, yPos + 400, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", Relabelling.CostTotal), Arial10, xPos + 435, yPos + 400, 0);
                PDF.writeText(gfx, Relabelling.CostItem, Arial10, xPos + 440, yPos + 400, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", Relabelling.CostGrandAmount), Arial10, xPos + 570, yPos + 400, 0);

                var PackingMaterial = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Packing Material");
                gfx.DrawString(PackingMaterial.CostName, Arial10, XBrushes.Black, xPos + 20, yPos + 440);
                PDF.writeRightText(gfx, "", Arial10, xPos + 370, yPos + 430, 0);
                PDF.writeRightText(gfx, "", Arial10, xPos + 450, yPos + 430, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", PackingMaterial.CostGrandAmount), Arial10, xPos + 570, yPos + 430, 0);

                var ManagementFee = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "Management Fee");
                gfx.DrawLine(new XPen(XBrushes.Black, 0.2), xPos + 20, yPos + 455, xwidth, yPos + 455);
                PDF.writeText(gfx, "Jumlah", Arial10Bold, xPos + 330, yPos + 465, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", ManagementFee.CostAmount), Arial10Bold, xPos + 570, yPos + 465, 0);

                PDF.writeText(gfx, ManagementFee.CostName + " (" + ManagementFee.CostTotal + ManagementFee.CostItem + ")", Arial10, xPos + 20, yPos + 485, 0);
                PDF.writeRightText(gfx, "", Arial10, xPos + 370, yPos + 485, 0);
                PDF.writeRightText(gfx, "", Arial10, xPos + 450, yPos + 485, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", ManagementFee.CostGrandAmount), Arial10, xPos + 570, yPos + 485, 0);

                var PPh = model.MasInvoicingDetails.SingleOrDefault(m => m.CostName == "PPh");
                gfx.DrawLine(new XPen(XBrushes.Black, 0.2), xPos + 310, yPos + 505, xwidth, yPos + 505);
                PDF.writeText(gfx, "SubTotal", Arial10Bold, xPos + 330, yPos + 515, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", PPh.CostAmount), Arial10Bold, xPos + 570, yPos + 515, 0);

                PDF.writeText(gfx, PPh.CostName + " (" + PPh.CostTotal + PPh.CostItem + ")", Arial10, xPos + 20, yPos + 535, 0);
                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", PPh.CostGrandAmount), Arial10, xPos + 570, yPos + 535, 0);
                //gfx.DrawLine(new XPen(XBrushes.Black, 0.2), xPos + 310, yPos + 560, xwidth, yPos + 560);

                PDF.writeText(gfx, "TOTAL", Arial12Bold, xPos + 330, yPos + 572, 0);

                PDF.writeRightText(gfx, String.Format(culture, "{0:n0}", model.TotalAmount), Arial12Bold, xPos + 570, yPos + 572, 0);
                PDF.drawBox(gfx, xPos + 310, yPos + 560, 275, 40, 1, 5);

                PDF.writeText(gfx, "Term & Condition", Arial8Bold, xPos + 20, yPos + 670, 0);

                PDF.setLine(gfx, xPos + 400, yPos + 730, 110);
                PDF.writeText(gfx, "Manager", Arial10, xPos + 435, yPos + 735, 0);
                
                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }
    }
}
