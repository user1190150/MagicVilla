using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	//Could be implemented like that [Route("api/[controller]")]
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{

		//Endpoint. For getting the List of Villas.
		//Swagger will identify this Endpoint as a GET Endpoint.
		[HttpGet]
		public IEnumerable<VillaDTO> GetVillas()
		{
			return VillaStore.villaList;
		}

		//Endpoint. For getting one specific Villa.
		//or [HttpGet("{id:int}")] then id must be a integer.
		[HttpGet("id")]
		public VillaDTO GetVilla(int id)
		{
			VillaDTO villa = new VillaDTO();
			villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			return villa;
		}
	}
}
