using WMS.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Components
{
    [Authorize]
    public class MenuItem:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuItem(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IViewComponentResult Invoke()
        {
            var ProfileId = Int32.Parse(UserClaimsPrincipal.FindFirst("ProfileId").Value);
            var model = _mapper.Map<List<MenuViewModel>>(_unitOfWork.Menu.Get(ProfileId));

            return View(model);
        }
    }
}
