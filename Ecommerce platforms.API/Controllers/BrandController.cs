using AutoMapper;
using Ecommerce_platforms.API.ModelsDTO;
using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_platforms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public BrandController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetALLBrands()
        {


            var brands = await _unitOfWork.Brand.GetAllAsync();

            var brandDTOs = _mapper.Map<List<BrandDTO>>(brands);

            return Ok(brandDTOs);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDTO>> GetSpecificBrand(int id)
        {
            var brand = await _unitOfWork.Brand.GetByIdAsync(id);
            var brandDTO = _mapper.Map<BrandDTO>(brand);
            return Ok(brandDTO);
        }
        [HttpPost]
        public async Task<IActionResult> AddBrandAsync(BrandDTO brandDTO)
        {
            var brand = _mapper.Map<Brand>(brandDTO);
            var result = await _unitOfWork.Brand.AddAsync(brand);
            if (result == null) { return BadRequest("unknown error while creating brand"); }
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBrand(BrandDTO brandDTO)
        {
            var brand = _mapper.Map<Brand>(brandDTO);
            var result = await _unitOfWork.Brand.UpdateAsync(brand);
            if (result == null) { return BadRequest("unknown error while updating brand"); }
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBrnad(int id)
        {
            var result = _unitOfWork.Brand.DeleteAsync(id);
            if (result == null) { return BadRequest("unknown error while deleting brand"); }
            return Ok(result);
        }
    }
}
