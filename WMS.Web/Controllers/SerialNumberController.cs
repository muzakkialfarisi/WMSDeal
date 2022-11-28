using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Utility;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using IronBarCode;
using PdfSharpCore;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class SerialNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SerialNumberController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string DONumber, Guid DOProductCode)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.DOProductCode == DOProductCode &&
                    m.MasProductData.ProductLevel == "SKU",
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers));

            if (model == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string DONumber, Guid DOProductCode, IncSerialNumber model)
        {
            var doproduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.DOProductCode == DOProductCode &&
                    m.MasProductData.ProductLevel == "SKU",
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers));

            if (doproduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            var product = await _unitOfWork.SerialNumber.GetAllAsync(
                filter:
                    m => m.ProductId == doproduct.ProductId);

            var SerialNumber = model.SerialNumber.Trim().ToUpper();

            if (product.Any(m => m.SerialNumber == SerialNumber) || SerialNumber == string.Empty)
            {
                TempData["error"] = "Serial Number already exists!";
                return View(doproduct);
            }

            if (doproduct.IncSerialNumbers.Count >= doproduct.Quantity)
            {
                TempData["error"] = "Serial Number Melebihi Quantity!";
                return View(doproduct);
            }

            model.SerialId = Guid.NewGuid().ToString();
            model.SerialNumber = SerialNumber;
            model.ProductId = doproduct.ProductId;
            model.DOProductId = doproduct.DOProductId;
            model.Status = SD.FlagSerialNumber_Open;
            model.CreatedBy = User.FindFirst("UserName")?.Value;
            model.CreatedDate = DateTime.Now;

            await _unitOfWork.SerialNumber.AddAsync(model);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "SerialNumber Added Successfully!";
            return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadUpsertDO(string DONumber, Guid DOProductCode, IFormFile SerialNumberList)
        {
            var doproduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.DOProductCode == DOProductCode &&
                    m.MasProductData.ProductLevel == "SKU",
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers));

            if (doproduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            var listSN = UploadFile(DONumber, DOProductCode, SerialNumberList);

            if (listSN == string.Empty)
            {
                TempData["error"] = "Product notfound!!";
                return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
            }

            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/SerialNumber");
            string filePath = Path.Combine(uploadFolder, listSN);
            string[] texts = System.IO.File.ReadAllLines(filePath);

            texts = texts.Select(s => s.Trim().ToUpper()).ToArray();
            if (texts.Length != texts.Distinct().Count())
            {
                TempData["error"] = "There is a duplicate serial number!";
                return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
            }


            if (texts.Length > doproduct.Quantity - doproduct.IncSerialNumbers.Count)
            {
                TempData["error"] = "SerialNumber melebihi quantity!";
                return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
            }

            var product = await _unitOfWork.SerialNumber.GetAllAsync(
                filter:
                    m => m.ProductId == doproduct.ProductId);

            foreach (var item in texts)
            {
                if (item != string.Empty)
                {
                    var model = new IncSerialNumber
                    {
                        SerialId = Guid.NewGuid().ToString(),
                        SerialNumber = item,
                        ProductId = doproduct.ProductId,
                        DOProductId = doproduct.DOProductId,
                        Status = SD.FlagSerialNumber_Open,
                        CreatedBy = User.FindFirst("UserName")?.Value,
                        CreatedDate = DateTime.Now
                    };

                    if (product.Any(m => m.SerialNumber == model.SerialNumber))
                    {
                        TempData["error"] = "Serial Number already exists!";
                        return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
                    }

                    await _unitOfWork.SerialNumber.AddAsync(model);
                }
            }

            await _unitOfWork.SaveAsync();
            System.IO.File.Delete(filePath);

            TempData["success"] = "SerialNumber Added Successfully!";
            return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeneratorUpsertDO(string DONumber, Guid DOProductCode)
        {
            var doproduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                    filter:
                        m => m.DONumber == DONumber &&
                        m.DOProductCode == DOProductCode &&
                        m.MasProductData.ProductLevel == "SKU",
                    includeProperties:
                        m => m.Include(m => m.IncSerialNumbers));

            if (doproduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            var count_serial = doproduct.Quantity - doproduct.IncSerialNumbers.Count;

            if (count_serial < 0)
            {
                TempData["error"] = "Serial Number melebihi quantity!";
                return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
            }

            var Tanggal = "W" + DateTime.Now.ToString("yy") + "M" + DateTime.Now.ToString("MM") + "S";
            var Last = 1.ToString("D6");
            var SerialNumber = Tanggal + Last;

            for (int i = 0; i < count_serial; i++)
            {
                var check = await _unitOfWork.SerialNumber.GetAllAsync(
                    filter:
                        m => m.SerialNumber.Contains(Tanggal) && m.SerialNumber.Length == 13);

                if (check.Count > 0)
                {
                    int LastCount = int.Parse(check.Max(m => m.SerialNumber.Substring(Tanggal.Length)));
                    SerialNumber = Tanggal + (LastCount + 1).ToString("00000#");
                }

                var model = new IncSerialNumber
                {
                    SerialId = Guid.NewGuid().ToString(),
                    SerialNumber = SerialNumber,
                    ProductId = doproduct.ProductId,
                    DOProductId = doproduct.DOProductId,
                    Status = SD.FlagSerialNumber_Open,
                    CreatedBy = User.FindFirst("UserName")?.Value,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.SerialNumber.AddAsync(model);
                await _unitOfWork.SaveAsync();
            }

            TempData["success"] = "SerialNumber Added Successfully!";
            return RedirectToAction("Upsert", new { DONumber = DONumber, DOProductCode = DOProductCode });
        }

        private string UploadFile(string DONumber, Guid DOProductCode, IFormFile SerialNumberList)
        {
            string uniqueFileName = string.Empty;

            if (SerialNumberList != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/SerialNumber");
                uniqueFileName = DONumber + DOProductCode + SerialNumberList.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    SerialNumberList.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> DetailDO(string DONumber, Guid DOProductCode)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.DOProductCode == DOProductCode &&
                    m.MasProductData.ProductLevel == "SKU",
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.IncSerialNumbers)
                    .Include(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderArrivals));

            if (model == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> EditSerialNumber(string SerialId, IncSerialNumber model)
        {
            var models = await _unitOfWork.SerialNumber.GetAllAsync(
                disableTracking:
                    false,
                filter:
                    m => m.ProductId == model.ProductId);

            var result = models.SingleOrDefault(m => m.SerialId == SerialId);

            var DOProduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == model.DOProductId);

            if (result == null || DOProduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            if (models.Any(m => m.SerialNumber == model.SerialNumber &&
            m.ProductId == model.ProductId &&
            m.SerialId != SerialId))
            {
                TempData["error"] = "SN already exsist!";
                return RedirectToAction("DetailDO", new { DONumber = DOProduct.DONumber, DOProductCode = DOProduct.DOProductCode });
            }

            if (result.Status == SD.FlagSerialNumber_OUT)
            {
                TempData["error"] = "SN tidak dapat diupdate!";
                return RedirectToAction("DetailDO", new { DONumber = DOProduct.DONumber, DOProductCode = DOProduct.DOProductCode });
            }

            result.SerialNumber = model.SerialNumber;

            _unitOfWork.SerialNumber.Update(result);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "SN updated successfully!";
            return RedirectToAction("DetailDO", new { DONumber = DOProduct.DONumber, DOProductCode = DOProduct.DOProductCode });
        }

        [HttpGet]
        public async Task<IActionResult> UpsertSO(string OrderId, int OrdProductId)
        {
            var product = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.OrdProductId == OrdProductId &&
                    m.MasProductData.ProductLevel == "SKU",
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers));

            if (product == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Dashboards");
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertSO(string OrderId, int OrdProductId, IncSerialNumber model)
        {
            var product = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.OrdProductId == OrdProductId &&
                    m.MasProductData.ProductLevel == "SKU",
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers));

            if (product == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            if (product.IncSerialNumbers.Count >= product.Quantity)
            {
                TempData["error"] = "SerialNumber Melebihi Quantity!";
                return RedirectToAction("UpsertSO", new { OrderId = OrderId, OrdProductId = OrdProductId });
            }

            var serials = await _unitOfWork.SerialNumber.GetAllAsync(
                disableTracking:
                    false,
                filter:
                    m => m.ProductId == product.ProductId &&
                    m.Status == SD.FlagSerialNumber_IN);

            var serial = serials.SingleOrDefault(m => m.SerialNumber == model.SerialNumber.Trim().ToUpper());

            if (serial == null)
            {
                TempData["error"] = "Serial Number Notfound!";
                return View(product);
            }

            serial.OrdProductId = model.OrdProductId;
            serial.Status = SD.FlagSerialNumber_OUT;

            _unitOfWork.SerialNumber.Update(serial);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "SerialNumber Added Successfully!";
            return RedirectToAction("UpsertSO", new { OrderId = OrderId, OrdProductId = OrdProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int DOProductId, int OrdProductId, string SerialId)
        {
            var model = await _unitOfWork.SerialNumber.GetSingleOrDefaultAsync(
                filter:
                    m => m.SerialId == SerialId);

            if (model == null)
            {
                TempData["error"] = "Serial Number Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            if (DOProductId != 0)
            {
                if (model.DOProductId != DOProductId)
                {
                    TempData["error"] = "Serial Number Notfound!";
                    return RedirectToAction("Index", "Dashboards");
                }

                _unitOfWork.SerialNumber.Remove(model);
                await _unitOfWork.SaveAsync();

                var doproduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == DOProductId);

                TempData["success"] = "Serial Number Deleted Successfully!";
                return RedirectToAction("Upsert", new { DONumber = doproduct.DONumber, DOProductCode = doproduct.DOProductCode });
            }

            if (OrdProductId != 0)
            {
                if (model.OrdProductId != OrdProductId)
                {
                    TempData["error"] = "Serial Number Notfound!";
                    return RedirectToAction("Index", "Dashboards");
                }

                model.Status = SD.FlagSerialNumber_IN;
                model.OrdProductId = null;
                _unitOfWork.SerialNumber.Update(model);

                await _unitOfWork.SaveAsync();

                var product = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                    filter:
                        m => m.OrdProductId == OrdProductId);

                TempData["success"] = "Serial Number Deleted Successfully!";
                return RedirectToAction("UpsertSO", new { OrderId = product.OrderId, OrdProductId = OrdProductId });
            }

            return RedirectToAction("Index", "Dashboards");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Print(string? DONumber, Guid? DOProductCode)
        {
            var models = await _unitOfWork.SerialNumber.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProduct));

            if (DONumber != null)
            {
                models = models.Where(m => m.IncDeliveryOrderProduct.DONumber == DONumber).ToList();
            }

            if (DOProductCode != null)
            {
                models = models.Where(m => m.IncDeliveryOrderProduct.DOProductCode == DOProductCode).ToList();
            }

            if (models.Count < 1)
            {
                TempData["error"] = "SerialNumber notfound!";
            }

            string webRootPath = _webHostEnvironment.WebRootPath;

            foreach (var item in models)
            {
                GeneratedBarcode barcode = BarcodeWriter.CreateBarcode(item.SerialNumber.ToString(), BarcodeWriterEncoding.Code128);
                barcode.ResizeTo(145, 70);
                barcode.AddBarcodeValueTextBelowBarcode();

                barcode.ChangeBarCodeColor(Color.Black);
                barcode.SetMargins(5);
                string filePath = Path.Combine(webRootPath + "\\img\\Storage\\QRCode\\" + item.SerialNumber.ToString() + ".png");
                barcode.SaveAsPng(filePath);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Serial Numbers";

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

                XTextFormatter tf = new XTextFormatter(gfx);

                double marginX = 10;
                double marginY = 10;
                double lebar = (page.Width - ((marginX * 2) + 20)) / 4;
                double tinggi = 50;

                for (int i = 1; i <= models.Count; i++)
                {
                    XRect TableHeaderProductQuantity = new XRect(marginX + 5, marginY + 5, lebar, tinggi);
                    gfx.DrawRectangle(XPens.Black, TableHeaderProductQuantity);
                    gfx.DrawImage(XImage.FromFile(webRootPath + "\\img\\Storage\\QRCode\\" + models[i-1].SerialNumber.ToString() + ".png"), marginX + 5, marginY + 5, lebar - 1, tinggi - 2);

                    marginX += 5;

                    if (i % 4 == 0)
                    {
                        marginX = 10;
                        marginY += tinggi + 5;
                    }
                    else
                    {
                        marginX += lebar;
                    }

                    if (i % 60 == 0)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        marginX = 10;
                        marginY = 10;
                    }


                    System.IO.File.Delete(webRootPath + "\\img\\Storage\\QRCode\\" + models[i-1].SerialNumber.ToString() + ".png");
                }
                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSerialBySerialId(string SerialId)
        {
            var model = await _unitOfWork.SerialNumber.GetSingleOrDefaultAsync(
                filter:
                    m => m.SerialId == SerialId);

            return Json(model);
        }
    }
}
