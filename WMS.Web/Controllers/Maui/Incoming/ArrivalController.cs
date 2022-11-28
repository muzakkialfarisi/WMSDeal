using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using PdfSharpCore.Pdf.Filters;
using System.Drawing;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Models.ViewModels.ApiViewModel.Maui;
using WMS.Utility;
using ProductHistoryType = WMS.Models.ProductHistoryType;

namespace WMS.Web.Controllers.Maui.Incoming
{
    [Route("maui/incoming/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class ArrivalController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ErrorResponseViewModel _error = new ErrorResponseViewModel();
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArrivalController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string DONumber = null, Guid? TenantId = null, string Status = null, string DateDelivered = null,string PeriodeDelivered =null)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts)
                          .Include(m => m.MasDataTenant)
                                .ThenInclude(m => m.MasKelurahan)
                                    .ThenInclude(m => m.MasKecamatan)
                                        .ThenInclude(m => m.MasKabupaten)
                                            .ThenInclude(m => m.MasProvinsi),
                filter:
                    m => m.HouseCode == User.FindFirst("HouseCode").Value);

                List<APIDeliveryOrderViewModel> m = model.Select(x => new APIDeliveryOrderViewModel
                {
                    DoNumber = x.DONumber,
                    TenantId = x.TenantId.ToString(),
                    Status = x.Status,
                    DateDelivered=x.DateDelivered,
                    Name = x.MasDataTenant.Name,
                    Address = x.MasDataTenant.Address,
                    KodePos = x.MasDataTenant.KodePos,
                    ProName = x.MasDataTenant.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName,
                    ProfileImageUrl = x.MasDataTenant.ProfileImageUrl,
                    Count = x.IncDeliveryOrderProducts.Count
                }).ToList();


                if (DONumber != null)
                {
                    m = m.Where(m => m.DoNumber.Contains(DONumber)).ToList();
                }

                if (TenantId != null)
                {
                    m = m.Where(m => m.TenantId == TenantId.ToString()).ToList();
                }

                if (Status != null)
                {
                    char[] delimiterChars = { ',' };
                    string[] words = Status.Split(delimiterChars);

                    var tempStatus = new List<APIDeliveryOrderViewModel>();
                    for (int i = 0; i < words.Length; i++)
                    {
                        tempStatus.AddRange(m.Where(m => m.Status == words[i]));
                    }
                    m = tempStatus;
                }

                if (DateDelivered != null)
                {
                    m = m.Where(m => m.DateDelivered?.ToString("yyyy-MM-dd") == DateDelivered).ToList();
                }
                if (PeriodeDelivered != null)
                {
                    m = m.Where(m => m.DateDelivered?.ToString("MMyyyy") == PeriodeDelivered).ToList();
                }

                return Ok(m);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "IC0001";
                return BadRequest(_error);
            }
        }

        [HttpGet("Product")]
        public async  Task<IActionResult> GetArrivalProduct(string DONumber = null)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductData)
                          .Include(m => m.IncDeliveryOrderArrivals),
                filter:
                    m => m.Status != "Booked");

                var MOD = model.Select(x => new
                {
                    x.DONumber,
                    x.DOProductId,
                    x.DOProductCode,
                    x.Quantity,
                    x.Status,
                    x.DateOfExpired,
                    QtyArrival = x.IncDeliveryOrderArrivals == null ? 0 : x.IncDeliveryOrderArrivals.Quantity,
                    x.ClosedNote,
                    x.MasProductData.ProductName,
                    x.MasProductData.ProductLevel,
                    x.MasProductData.ProductCondition,
                    x.MasProductData.SKU,
                    x.MasProductData.Unit,
                    x.MasProductData.ActualWeight,
                    x.MasProductData.BeautyPicture,
                    x.MasProductData.SizeCode,
                    x.MasProductData.ZoneCode,
                    x.MasProductData.SerialNumber,

                    x.IncDeliveryOrderArrivals.DateArrived,
                    x.IncDeliveryOrderArrivals.ArrivedBy,
                    x.IncDeliveryOrderArrivals.Note,
                    x.IncDeliveryOrderArrivals.ProductImage,
                }).ToList();


                if (DONumber != null)
                {
                    MOD = MOD.Where(m => m.DONumber.Contains(DONumber)).ToList();
                }

                return Ok(MOD);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "IC0001";
                return BadRequest(_error);
            }
        }


        [HttpGet("{DONumber}")]
        public async Task<IActionResult> GetByDONumber(string DONumber)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                 includeProperties:
                   m => m.Include(m => m.IncDeliveryOrderProducts)
                         .Include(m => m.MasDataTenant)
                            .ThenInclude(m => m.MasKelurahan)
                                    .ThenInclude(m => m.MasKecamatan)
                                        .ThenInclude(m => m.MasKabupaten)
                                            .ThenInclude(m => m.MasProvinsi),
                  filter:
                   m => m.DONumber == DONumber);

                var MOD = model.Select(x => new
                {
                    x.DONumber,
                    x.TenantId,
                    x.Status,
                    x.DateDelivered,
                    x.MasDataTenant.Name,
                    x.MasDataTenant.Address,
                    x.MasDataTenant.KodePos,
                    x.MasDataTenant.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName,
                    x.MasDataTenant.ProfileImageUrl,
                    x.IncDeliveryOrderProducts.Count
                });

                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = msg.Message;
                _error.Code = "IC0002";
                return BadRequest(_error);
            }
        }

        [HttpGet("DOProduct/{DONumber}")]
        public async Task<IActionResult> GetProductDONumber(string DONumber)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                 includeProperties:
                   m => m.Include(m => m.MasDataTenant)
                            .ThenInclude(m => m.MasKelurahan)
                                    .ThenInclude(m => m.MasKecamatan)
                                        .ThenInclude(m => m.MasKabupaten)
                                            .ThenInclude(m => m.MasProvinsi)
                        .Include(m => m.IncDeliveryOrderProducts)
                            .ThenInclude(m => m.MasProductData)
                        .Include(m => m.IncDeliveryOrderProducts)
                            .ThenInclude(m => m.IncDeliveryOrderArrivals),
                  filter:
                   m => m.DONumber == DONumber);

                var MOD = model.Select(x => new
                {
                    x.DONumber,
                    x.TenantId,
                    x.Status,
                    x.DateDelivered,
                    x.MasDataTenant.Name,
                    x.MasDataTenant.Address,
                    x.MasDataTenant.KodePos,
                    x.MasDataTenant.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName,
                    x.MasDataTenant.ProfileImageUrl,
                    x.IncDeliveryOrderProducts.Count,
                    ProductData = x.IncDeliveryOrderProducts.Select(s => new
                    {
                        s.DONumber,
                        s.DOProductId,
                        s.DOProductCode,
                        s.Quantity,
                        s.Status,
                        s.DateOfExpired,
                        QtyArrival = s.IncDeliveryOrderArrivals == null ? 0 : s.IncDeliveryOrderArrivals.Quantity,
                        s.ClosedNote,
                        s.MasProductData.ProductName,
                        s.MasProductData.ProductLevel,
                        s.MasProductData.ProductCondition,
                        s.MasProductData.SKU,
                        s.MasProductData.Unit,
                        s.MasProductData.ActualWeight,
                        s.MasProductData.BeautyPicture,
                        s.MasProductData.SizeCode,
                        s.MasProductData.ZoneCode,
                        s.MasProductData.SerialNumber,
                    })
                });

                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = msg.Message;
                _error.Code = "IC0002";
                return BadRequest(_error);
            }
        }


        [HttpGet("ProductItems/{DOProductId}")]
        public async Task<IActionResult> GetByDONumberProductItems(int DOProductId)
        {
            try
            {
                var model = await _unitOfWork.ItemProduct.GetAllAsync(
                 includeProperties:
                   m => m.Include(m => m.InvStorageCode)
                            .ThenInclude(m => m.InvStorageBin)
                                .ThenInclude(m => m.InvStorageLevel)
                                    .ThenInclude(m => m.InvStorageSection)
                                        .ThenInclude(m => m.InvStorageRow)
                        .Include(m => m.InvStorageCode)
                            .ThenInclude(m => m.InvStorageCategory)
                        .Include(m => m.InvStorageCode)
                            .ThenInclude(m => m.InvStorageSize),
                  filter:
                   m => m.DOProductId == DOProductId);

                var MOD = model.Select(s => new
                {
                    s.IKU,
                    s.DOProductId,
                    s.StorageCode,
                    s.Status,
                    s.Note,
                    s.InvStorageCode.InvStorageCategory.StorageCategoryCode,
                    s.InvStorageCode.InvStorageCategory.StorageCategoryName,
                    s.InvStorageCode.InvStorageSize.SizeCode,
                    s.InvStorageCode.InvStorageSize.SizeName,
                    s.InvStorageCode.BinCode,
                    s.InvStorageCode.InvStorageBin.BinName,
                    s.InvStorageCode.InvStorageBin.InvStorageLevel.LevelCode,
                    s.InvStorageCode.InvStorageBin.InvStorageLevel.LevelName,
                    s.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageSection.SectionCode,
                    s.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageSection.SectionName,
                    s.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageSection.InvStorageRow.RowCode,
                    s.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageSection.InvStorageRow.RowName,
                });

                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = msg.Message;
                _error.Code = "IC0003";
                return BadRequest(_error);
            }
        }


        [HttpGet("Products/{DONumber}")]
        public async Task<IActionResult> GetProductsbyDONumber(string DONumber)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                   includeProperties:
                       m => m.Include(m => m.MasProductData)
                             .Include(m => m.IncDeliveryOrderArrivals),
                   filter:
                       m => m.DONumber == DONumber);

                var MOD = model.Select(x => new
                {
                    x.DONumber,
                    x.DOProductId,
                    x.DOProductCode,
                    x.Quantity,
                    x.Status,
                    x.DateOfExpired,
                    QtyArrival = x.IncDeliveryOrderArrivals == null ? 0 : x.IncDeliveryOrderArrivals.Quantity,
                    x.ClosedNote,
                    x.MasProductData.ProductName,
                    x.MasProductData.ProductLevel,
                    x.MasProductData.ProductCondition,
                    x.MasProductData.SKU,
                    x.MasProductData.Unit,
                    x.MasProductData.ActualWeight,
                    x.MasProductData.BeautyPicture,
                    x.MasProductData.SizeCode,
                    x.MasProductData.ZoneCode,
                    x.MasProductData.SerialNumber,
                });

                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = msg.Message;
                _error.Code = "IC0003";
                return BadRequest(_error);
            }
        }

        [HttpGet("TotalArrived/{DOProductId}")]
        public async Task<IActionResult> GetTotalArrived(int DOProductId)
        {
            try
            {
                //if (ProductLevel == "SKU")
                //{
                var model = await _unitOfWork.DeliveryOrderArrival.GetAllAsync(
                    filter: m => m.DOProductId == DOProductId);

                var MOD = model.Select(x => new
                {
                    x.DOProductId,
                    x.Quantity,
                });
                return Ok(MOD);
                //}
                //else
                //{
                //    var model = await _unitOfWork.ItemProduct.GetAllAsync(
                //       filter: m => m.DOProductId == DOProductId);

                //    var MOD = model.Select(x => new
                //    {
                //        x.DOProductId,
                //        Quantity = (x.Status > 2) ? 1 : 0,
                //    }); ;
                //    return Ok(MOD);
                //}

            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = msg.Message;
                _error.Code = "IC0003";
                return BadRequest(_error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeliveryOrderArrivalViewModel model)
        {
            try
            {
                if (model == null)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Invalid Modelstate!";
                    _error.Code = "AR0010";
                    return BadRequest(_error);
                }

                if (model.Quantity <= 0)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Quantity harus lebih dari dari 0";
                    _error.Code = "AR0011";
                    return BadRequest(_error);
                }

                var DOProduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.DOProductId == model.DOProductId,
                    includeProperties:
                        m => m.Include(m => m.IncDeliveryOrder.IncDeliveryOrderProducts)
                        .Include(m => m.MasProductData.InvProductStocks)
                        .Include(m => m.IncDeliveryOrderArrivals)
                        .Include(m => m.IncSerialNumbers)
                        .Include(m => m.IncItemProducts));

                if (DOProduct == null)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Delivery Order tidak ditemukan!";
                    _error.Code = "AR0003";
                    return BadRequest(_error);
                }

                if (DOProduct.IncDeliveryOrderArrivals == null)
                {
                    if (model.Quantity > DOProduct.Quantity)
                    {
                        _error.StatusCode = "400";
                        _error.Error = "Error";
                        _error.Message = "Over Quantity";
                        _error.Code = "AR0003";
                        return BadRequest(_error);
                    }

                    DOProduct.IncDeliveryOrderArrivals = new IncDeliveryOrderArrival();

                    DOProduct.IncDeliveryOrderArrivals.DOProductId = model.DOProductId;
                    DOProduct.IncDeliveryOrderArrivals.ProductImage = model.DOProductId.ToString() + ".jpg";
                    DOProduct.IncDeliveryOrderArrivals.Quantity = model.Quantity;
                    DOProduct.IncDeliveryOrderArrivals.ArrivedBy = User.FindFirst("UserName")?.Value.ToString();
                    DOProduct.IncDeliveryOrderArrivals.Note = model.Note;
                    DOProduct.IncDeliveryOrderArrivals.DateArrived = DateTime.Now;

                    await _unitOfWork.DeliveryOrderArrival.AddAsync(DOProduct.IncDeliveryOrderArrivals);
                }
                else
                {
                    DOProduct.IncDeliveryOrderArrivals.ProductImage = model.DOProductId.ToString() + ".jpg";
                    DOProduct.IncDeliveryOrderArrivals.Quantity = DOProduct.IncDeliveryOrderArrivals.Quantity + model.Quantity;
                    DOProduct.IncDeliveryOrderArrivals.ArrivedBy = User.FindFirst("UserName")?.Value.ToString();
                    DOProduct.IncDeliveryOrderArrivals.Note = model.Note;
                    DOProduct.IncDeliveryOrderArrivals.DateArrived = DateTime.Now;

                    if (DOProduct.IncDeliveryOrderArrivals.Quantity > DOProduct.Quantity)
                    {
                        _error.StatusCode = "400";
                        _error.Error = "Error";
                        _error.Message = "Over Quantity";
                        _error.Code = "AR0003";
                        return BadRequest(_error);
                    }
                    _unitOfWork.DeliveryOrderArrival.Update(DOProduct.IncDeliveryOrderArrivals);
                }

                var arrivalproduct = new IncDeliveryOrderArrivalProduct();

                if (model.ProductImage != null)
                {
                    byte[] bytes = Convert.FromBase64String(model.ProductImage);

                    arrivalproduct.ImageUrl = Path.Combine("img/DeliveryOrder/Arrival", arrivalproduct.Id.ToString() + ".jpg"); ;
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, arrivalproduct.ImageUrl);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Image pic = Image.FromStream(ms);
                        pic.Save(filePath);
                    }
                }

                arrivalproduct.DOProductId = model.DOProductId;
                arrivalproduct.ProductId = DOProduct.ProductId;
                arrivalproduct.Quantity = model.Quantity;
                arrivalproduct.CreatedBy = User.FindFirst("UserName")?.Value;
                arrivalproduct.Note = model.Note;

                await _unitOfWork.DeliveryOrderArrivalProduct.AddAsync(arrivalproduct);

                var productStock = DOProduct.MasProductData.InvProductStocks.SingleOrDefault(m => m.HouseCode == DOProduct.IncDeliveryOrder.HouseCode);

                if (productStock == null)
                {
                    productStock = new InvProductStock();
                    productStock.Id = DOProduct.ProductId + DOProduct.IncDeliveryOrder.HouseCode;
                    productStock.ProductId = (int)DOProduct.ProductId;
                    productStock.HouseCode = DOProduct.IncDeliveryOrder.HouseCode;
                    productStock.Stock = model.Quantity;
                    await _unitOfWork.ProductStock.AddAsync(productStock);
                }
                else
                {
                    productStock.Stock = productStock.Stock + model.Quantity;
                    _unitOfWork.ProductStock.Update(productStock);
                }


                InvProductHistory productHistory = new InvProductHistory
                {
                    ProductId = (int)DOProduct.ProductId,
                    HouseCode = DOProduct.IncDeliveryOrder.HouseCode,
                    HistoryType = ProductHistoryType.In,
                    TrxNo = DOProduct.DONumber,
                    Interest = "Delivery Order",
                    Quantity = model.Quantity,
                    Note = model.Note,
                    Stock = productStock.Stock,
                    DatedTime = DateTime.Now,
                    UserBy = User.FindFirst("UserName")?.Value,
                    Flag = 1,
                };
                await _unitOfWork.ProductHistory.AddAsync(productHistory);

                //if (DOProduct.Quantity > DOProduct.IncDeliveryOrderArrivals.Quantity)
                //{
                //    return BadRequest("Over Quantity!");
                //}

                if (DOProduct.Quantity == DOProduct.IncDeliveryOrderArrivals.Quantity)
                {
                    DOProduct.Status = "Arrived";
                    _unitOfWork.DeliveryOrderProduct.Update(DOProduct);

                    if (!DOProduct.IncDeliveryOrder.IncDeliveryOrderProducts.Any(m => m.Status == "Booked"))
                    {
                        DOProduct.IncDeliveryOrder.Status = "AR";
                        _unitOfWork.DeliveryOrder.Update(DOProduct.IncDeliveryOrder);
                    }
                }

                //serial number
                if (DOProduct.IncSerialNumbers.Count > 0)
                {
                    var serials = DOProduct.IncSerialNumbers.Where(m => m.Status == SD.FlagSerialNumber_Open).ToList();
                    if (serials.Count() < model.Quantity)
                    {
                        _error.StatusCode = "400";
                        _error.Error = "Error";
                        _error.Message = "Over Quantity";
                        _error.Code = "AR0003";
                        return BadRequest(_error);
                    }

                    for (int i = 0; i < model.Quantity; i++)
                    {
                        serials[i].Status = SD.FlagSerialNumber_IN;
                        _unitOfWork.SerialNumber.Update(serials[i]);
                    }
                }

                await _unitOfWork.SaveAsync();
                return Ok("Product sukses di arrival!");

            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error";
                _error.Message = msg.Message;
                _error.Code = "AR1000";
                return BadRequest(_error);
            }
        }


        [HttpPost("UploadManifest")]
        public async Task<IActionResult> UploadManifestDeliveryOrder([FromBody] DeliveryOrderUploadViewModel model)
        {
            var result = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts).ThenInclude(m => m.IncDeliveryOrderArrivals),
                filter:
                    m => m.DONumber == model.DONumber);

            if (result == null)
            {
                _error.StatusCode = "400";
                _error.Error = "Error";
                _error.Message = "Data Notfound";
                _error.Code = "AR1000";
                return BadRequest(_error);
            }

            if (model.NotaImage == null)
            {
                _error.StatusCode = "400";
                _error.Error = "Error";
                _error.Message = "Foto produk diwajibkan!";
                _error.Code = "AR1000";
                return BadRequest(_error);
            }

            byte[] bytes = Convert.FromBase64String(model.NotaImage);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/DeliveryOrder");
            var fileName = model.DONumber + ".jpg";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image pic = Image.FromStream(ms);
                pic.Save(filePath);
            }

            for (int i = 0; i < result.IncDeliveryOrderProducts.Count; i++)
            {
                result.IncDeliveryOrderProducts[i].IncDeliveryOrderArrivals.NotaImage = filePath;
                _unitOfWork.DeliveryOrderArrival.Update(result.IncDeliveryOrderProducts[i].IncDeliveryOrderArrivals);
            }

            result.DateArrived = DateTime.Now;
            _unitOfWork.DeliveryOrder.Update(result);

            await _unitOfWork.SaveAsync();

            return Ok("Sukses upload manifes!");
        }
    }
}
