using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ZXing.QrCode;
using ZXing;
using System.Drawing;
using PdfSharpCore;
using PdfSharpCore.Drawing.Layout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.Utility;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class StorageCodeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StorageCodeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string RowCode, string SectionCode, string ColumnCode, string BinCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var storageRows = await _unitOfWork.StorageRow.GetAllAsync();
            var model = await _unitOfWork.StorageCode.GetAllAsync(
                includeProperties:
                    m => m.Include(x => x.InvStorageCategory)
                    .Include(x => x.InvStorageSize.InvStorageBesaran)
                    .Include(x => x.InvStorageSize.InvStorageTebal)
                    .Include(x => x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode)
                    .Include(x => x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                storageRows = storageRows.Where(m => m.HouseCode == HouseCode).ToList();
                model = model.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode == HouseCode).ToList();
            }

            ViewData["RowCode"] = storageRows.Select(n => new SelectListItem
                                            {
                                                Value = n.RowCode.ToString(),
                                                Text = string.Concat(n.RowCode, " | ", n.RowName)
                                            }).ToList();
            if (RowCode != null)
            {
                model = model.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.RowCode == RowCode).ToList();
            }
            if (SectionCode != null)
            {
                model = model.Where(m => m.InvStorageBin.InvStorageLevel.SectionCode == SectionCode).ToList();
            }
            if (ColumnCode != null)
            {
                model = model.Where(m => m.InvStorageBin.InvStorageLevel.ColumnCode == ColumnCode).ToList();
            }
            if (BinCode != null)
            {
                model = model.Where(m => m.BinCode == BinCode).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PrintStorageCode(string RowCode, string SectionCode, string ColumnCode, string BinCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var storageCode  = await _unitOfWork.StorageCode.GetAllAsync(
                includeProperties:
                    m => m.Include(x => x.InvStorageCategory)
                    .Include(x => x.InvStorageSize.InvStorageBesaran)
                    .Include(x => x.InvStorageSize.InvStorageTebal)
                    .Include(x => x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode)
                    .Include(x => x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode == HouseCode).ToList();
            }

            if (RowCode != null)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.RowCode == RowCode).ToList();
            }
            if (SectionCode != null)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.SectionCode == SectionCode).ToList();
            }
            if (ColumnCode != null)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.ColumnCode == ColumnCode).ToList();
            }
            if (BinCode != null)
            {
                storageCode = storageCode.Where(m => m.BinCode == BinCode).ToList();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;

            //Generate QRCode
            foreach (var item in storageCode)
            {
                var writer = new QRCodeWriter();

                var matrix = writer.encode(item.StorageCode.ToString(), BarcodeFormat.QR_CODE, 150, 150);
                Bitmap result = new Bitmap(matrix.Width * 2, matrix.Height * 2);
                for (int x = 0; x < matrix.Width; x++)
                {
                    for (int y = 0; y < matrix.Height; y++)
                    {
                        Color pixel = matrix[x, y] ? Color.Black : Color.White;
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 2; j++)
                                result.SetPixel(x * 2 + i, y * 2 + j, pixel);
                        }
                    }
                    result.Save(webRootPath + "\\img\\Storage\\QRCode\\" + item.StorageCode.ToString() + ".png");
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                PdfDocument document = new PdfDocument();

                XFont Arial6 = new XFont("Arial", 6);
                XFont Arial6Bold = new XFont("Arial", 6, XFontStyle.Bold);
                XFont Arial5 = new XFont("Arial", 5);
                XFont Arial5Bold = new XFont("Arial", 5, XFontStyle.Bold);

                PdfPage page = document.AddPage();
                page.Size = PageSize.Letter;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                int xPos = 5;
                int yPos = 5;
                XTextFormatter tf;
                for (int j = 0; j < storageCode.Count; j++)
                {
                    if (j % 4 == 0)
                    {
                        if (j == 0)
                        {
                            xPos = 5;
                            yPos = 5;
                        }
                        else
                        {
                            xPos = 5;
                            yPos += 77;
                        }
                    }
                    else
                    {
                        xPos += 151;
                    }

                    if (j + 1 % 50 == 0)
                    {
                        page = document.AddPage();
                        page.Size = PageSize.Letter;
                        gfx = XGraphics.FromPdfPage(page);
                        xPos = 5;
                        yPos = 5;
                    }
                    tf = new XTextFormatter(gfx);

                    gfx.DrawRectangle(XPens.Black, xPos, yPos, 147, 72);

                    var houseCode = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode.ToString().ToUpper().Trim();
                    var zoneCode = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneCode.ToString().ToUpper().Trim();
                    var rowCode = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.RowCode.ToString().ToUpper().Trim();
                    var sectionCode = storageCode[j].InvStorageBin.InvStorageLevel.SectionCode.ToString().ToUpper().Trim();
                    var columnCode = storageCode[j].InvStorageBin.InvStorageLevel.ColumnCode.ToString().ToUpper().Trim();
                    var levelCode = storageCode[j].InvStorageBin.InvStorageLevel.LevelCode.ToString().ToUpper().Trim();
                    var binCode = storageCode[j].InvStorageBin.BinCode.ToString().ToUpper().Trim();

                    gfx.DrawImage(XImage.FromFile(webRootPath + "\\img\\Storage\\QRCode\\" + storageCode[j].StorageCode.ToString().Trim() + ".png"), xPos + 1, yPos + 1, 70, 70);
                    gfx.DrawImage(XImage.FromFile(webRootPath + "\\img\\Storage\\up.png"), xPos + 126, yPos , 15, 20);

                    string temp = "";
                    temp = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode.HouseName.ToString().ToUpper();
                    gfx.DrawString(s: temp, font: Arial5Bold, brush: XBrushes.Black, x: xPos + 70, y: yPos + 14);

                    temp = houseCode + " - " + storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneName.ToString().ToUpper();
                    gfx.DrawString(s: temp, font: Arial6Bold, brush: XBrushes.Black, x: xPos + 70, y: yPos + 21);

                    temp = string.Concat("ROW : ", rowCode.ToString().AsSpan((houseCode + zoneCode).Length));
                    gfx.DrawString(s: temp, Arial6Bold, XBrushes.Black, xPos + 70, yPos + 28);

                    temp = string.Concat("BIN : ", levelCode.ToString().AsSpan(rowCode.Length), "/", binCode.ToString().AsSpan(levelCode.Length));
                    gfx.DrawString(s: temp, Arial6Bold, XBrushes.Black, xPos + 70, yPos + 35);

                    temp = storageCode[j].InvStorageSize.SizeName.ToString().ToUpper().Trim();
                    gfx.DrawString(s: temp, Arial5Bold, XBrushes.Black, xPos + 70, yPos + 50);

                    tf.Alignment = XParagraphAlignment.Left;
                    temp = storageCode[j].InvStorageCategory.StorageCategoryName.ToString().ToUpper().Trim();
                    tf.DrawString(text: temp, Arial6Bold, XBrushes.Black, new XRect(xPos + 70, yPos + 52, 100, 30), XStringFormats.TopLeft);
                    
                    System.IO.File.Delete(webRootPath + "\\img\\Storage\\QRCode\\" + storageCode[j].StorageCode.ToString().Trim() + ".png");
                }
                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintStorageCodeTronix(string RowCode, string SectionCode, string ColumnCode, string BinCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var storageCode = await _unitOfWork.StorageCode.GetAllAsync(
                includeProperties:
                    m => m.Include(x => x.InvStorageCategory)
                    .Include(x => x.InvStorageSize.InvStorageBesaran)
                    .Include(x => x.InvStorageSize.InvStorageTebal)
                    .Include(x => x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode)
                    .Include(x => x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode == HouseCode).ToList();
            }

            if (RowCode != null)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.RowCode == RowCode).ToList();
            }
            if (SectionCode != null)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.SectionCode == SectionCode).ToList();
            }
            if (ColumnCode != null)
            {
                storageCode = storageCode.Where(m => m.InvStorageBin.InvStorageLevel.ColumnCode == ColumnCode).ToList();
            }
            if (BinCode != null)
            {
                storageCode = storageCode.Where(m => m.BinCode == BinCode).ToList();
            }

            if(storageCode.Count < 1)
            {
                TempData["error"] = "Data Notfound!";
                return RedirectToAction("Index");
            }

            string webRootPath = _webHostEnvironment.WebRootPath;

            foreach (var item in storageCode)
            {
                var writer = new QRCodeWriter();

                var resultBit = writer.encode(item.StorageCode.ToString(), BarcodeFormat.QR_CODE, 150, 150);
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
                                result.SetPixel(x * scale + i, y * scale + j, pixel);
                        }
                    }
                    result.Save(webRootPath + "\\img\\Storage\\QRCode\\" + item.StorageCode.ToString() + ".png");

                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Print Storage Code";

                XFont Title20 = new XFont("Arial", 20, XFontStyle.Bold);
                XFont Title12 = new XFont("Arial", 12, XFontStyle.Bold);
                XFont Title10 = new XFont("Arial", 10, XFontStyle.Bold);
                XFont Body = new XFont("Arial", 8, XFontStyle.Bold);
                XFont Description = new XFont("Arial", 6);
                XFont Arial6Bold = new XFont("Arial", 6, XFontStyle.Bold);

                for (int j = 0; j < storageCode.Count; j++)
                {
                    PdfPage page = document.AddPage();
                    page.Width = 377.95275591;
                    page.Height = 188.97637795;
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XTextFormatter tf = new XTextFormatter(gfx);

                    double marginX = 5;
                    double marginY = 5;
                    double lebar = page.Width - (marginX * 2);
                    double tinggi = page.Height - (marginY * 2);
                    string temp = "";

                    double lebarqr = 175;
                    double tinggiqr = 175;
                    
                    gfx.DrawImage(XImage.FromFile(webRootPath + "\\img\\Storage\\QRCode\\" + storageCode[j].StorageCode.ToString() + ".png"), 0, 0, lebarqr, tinggiqr);
                    gfx.DrawImage(XImage.FromFile(webRootPath + "\\img\\Storage\\up.png"), 335, 5, 30, 40);
                    temp = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode.HouseCode.ToString().ToUpper() +
                        " | " + storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode.HouseName.ToString().ToUpper();
                    gfx.DrawString(temp, Title10, XBrushes.Black, 20, 175);
                    
                    XRect layout = new XRect(marginX, marginY, lebar, tinggi);
                    gfx.DrawRectangle(XPens.Black, layout);

                    var houseCode = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode.ToString().ToUpper().Trim();
                    var zoneCode = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneCode.ToString().ToUpper().Trim();
                    var rowCode = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.RowCode.ToString().ToUpper().Trim();
                    var levelCode = storageCode[j].InvStorageBin.InvStorageLevel.LevelCode.ToString().ToUpper().Trim();
                    var binCode = storageCode[j].InvStorageBin.BinCode.ToString().ToUpper().Trim();

                    marginX = lebar / 2;
                    marginY = marginY + 10;
                    double lebarkiri = (lebar / 2 / 2);
                    double tinggikiri = 35;

                    temp = "ROW";
                    XRect infokiri = new XRect(marginX, marginY, lebarkiri - 25, tinggikiri);
                    //gfx.DrawRectangle(XPens.Black, infokiri);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title20, XBrushes.Black, infokiri, XStringFormats.TopLeft);

                    marginX = marginX + lebarkiri - 25;
                    temp = ": " + rowCode.Substring(houseCode.Length + zoneCode.Length);

                    XRect infokanan = new XRect(marginX, marginY, lebarkiri + 25, tinggikiri);
                    //gfx.DrawRectangle(XPens.Black, infokanan);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title20, XBrushes.Black, infokanan, XStringFormats.TopLeft);

                    marginY = marginY + tinggikiri;
                    marginX = lebar / 2;
                    temp = "LEVEL";
                    infokiri = new XRect(marginX, marginY, lebarkiri - 25, tinggikiri);
                    //gfx.DrawRectangle(XPens.Black, infokiri);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title20, XBrushes.Black, infokiri, XStringFormats.TopLeft);

                    marginX = marginX + lebarkiri - 25;
                    temp = ": " + levelCode.Substring(rowCode.Length);

                    infokanan = new XRect(marginX, marginY, lebarkiri + 25, tinggikiri);
                    //gfx.DrawRectangle(XPens.Black, infokanan);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title20, XBrushes.Black, infokanan, XStringFormats.TopLeft);

                    marginY = marginY + tinggikiri;
                    marginX = lebar / 2;
                    temp = "BIN";
                    infokiri = new XRect(marginX, marginY, lebarkiri - 25, tinggikiri);
                    //gfx.DrawRectangle(XPens.Black, infokiri);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title20, XBrushes.Black, infokiri, XStringFormats.TopLeft);

                    marginX = marginX + lebarkiri - 25;
                    temp = ": " + binCode.Substring(levelCode.Length);

                    infokanan = new XRect(marginX, marginY, lebarkiri + 25, tinggikiri);
                    //gfx.DrawRectangle(XPens.Black, infokanan);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title20, XBrushes.Black, infokanan, XStringFormats.TopLeft);

                    marginX = lebar / 2;
                    marginY = marginY + tinggikiri;
                    double lebarbawah = lebar / 2;
                    double tinggibawah = 20;
                    temp = storageCode[j].InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneName + " (" + zoneCode + ")";

                    XRect infobawah = new XRect(marginX, marginY, lebarbawah, tinggibawah); 
                    //gfx.DrawRectangle(XPens.Black, infobawah);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title12, XBrushes.Black, infobawah, XStringFormats.TopLeft);


                    marginY = marginY + tinggibawah;
                    temp = storageCode[j].InvStorageSize.SizeName;

                    infobawah = new XRect(marginX, marginY, lebarbawah, tinggibawah);
                    //gfx.DrawRectangle(XPens.Black, infobawah);
                    tf.Alignment = XParagraphAlignment.Left;
                    tf.DrawString(temp, Title12, XBrushes.Black, infobawah, XStringFormats.TopLeft);

                    System.IO.File.Delete(webRootPath + "\\img\\Storage\\QRCode\\" + storageCode[j].StorageCode.ToString() + ".png");
                }
                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetStorageCodeByStorageCode(Guid StorageCode)
        {
            var invStorageCode = await _unitOfWork.StorageCode.GetSingleOrDefaultAsync(
                filter:
                    m => m.StorageCode == StorageCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageBin));
                
            if(invStorageCode == null)
            {
                return Json(NotFound("Storage Code NotFound!"));
            }

            return Json(Ok(invStorageCode));
        }

        [HttpGet]
        public async Task<JsonResult> GetStorageCodeByRowCode(string RowCode)
        {
            var model = await _unitOfWork.StorageCode.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn)
                    .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageSection)
                    .Include(m => m.InvStorageSize),
                filter:
                    m => m.InvStorageBin.InvStorageLevel.InvStorageSection.RowCode == RowCode);

            return Json(model);
        }
    }
}
