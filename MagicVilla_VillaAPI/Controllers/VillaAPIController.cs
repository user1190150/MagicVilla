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
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			return Ok(VillaStore.villaList);
		}

		//Endpoint. For getting one specific Villa.
		//or [HttpGet("{id:int}")] then id must be a integer.
		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			VillaDTO villa = new VillaDTO();
			villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			if (villa == null)
			{
				return NotFound();
			}

			return Ok(villa);
		}

		//Endpoint. For creating a new Villa.
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
		{
			if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa already Exists!");
				return BadRequest(ModelState);
			}

			if (villaDTO == null)
			{
				return BadRequest(villaDTO);
			}

			if (villaDTO.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
			VillaStore.villaList.Add(villaDTO);

			return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
		}

		//Endpoint. For deleting a Villa.
		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult DeleteVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}


			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			VillaStore.villaList.Remove(villa);

			return NoContent();
		}

		//Endpoint. Will Update whole record(OBJEKT).
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
		{
			if (villaDTO == null || id != villaDTO.Id)
			{
				return BadRequest();
			}

			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			villa.Name = villaDTO.Name;
			villa.Sqft = villaDTO.Sqft;
			villa.Occupancy = villaDTO.Occupancy;

			return NoContent();
		}

		//Endpoint. Will Update just one.
		//[HttpPatch]

	}
}
