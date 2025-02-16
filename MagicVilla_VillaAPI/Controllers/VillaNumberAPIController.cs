using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/VillaNumberAPI")]
	[ApiController]
	public class VillaNumberAPIController : ControllerBase
	{
		protected APIResponse _response;
		private readonly IVillaNumberRepository _dbVillaNumber;
		private readonly IMapper _mapper;

		public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper)
		{
			_dbVillaNumber = dbVillaNumber;
			_mapper = mapper;
			this._response = new();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillaNumbers()
		{
			try
			{
				IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Errors = new List<string>() { ex.ToString() };
			}
			return _response;
		}

		[HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo == 0)
				{
					return BadRequest();
				}

				var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == villaNo);
				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Errors = new List<string>() { ex.ToString() };
			}
			return _response;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
		{
			try
			{
				if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
				{
					ModelState.AddModelError("Custom Error", "Villa Number already exists!");
					return BadRequest(ModelState);
				}

				if (createDTO == null)
				{
					return BadRequest(createDTO);
				}

				VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

				await _dbVillaNumber.CreateAsync(villaNumber);
				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.Created;
				return CreatedAtRoute("GetVillaNumber", new { villaNo = villaNumber.VillaNo }, _response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Errors = new List<string>() { ex.ToString() };
			}
			return _response;
		}

		[HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo == 0)
				{
					return BadRequest();
				}

				var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == villaNo);

				if (villaNumber == null)
				{
					return NotFound();
				}

				await _dbVillaNumber.RemoveAsync(villaNumber);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Errors = new List<string>() { ex.ToString() };
			}
			return _response;
		}

		[HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO updateDTO)
		{
			try
			{ 
			if (updateDTO == null || villaNo != updateDTO.VillaNo)
			{
				return BadRequest();
			}

			VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);
			await _dbVillaNumber.UpdateAsync(villaNumber);

			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;
			return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Errors = new List<string>() { ex.ToString() };
			}
			return _response;
		}
	}
}
