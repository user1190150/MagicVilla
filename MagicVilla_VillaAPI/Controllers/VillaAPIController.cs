using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	//Could be implemented like that [Route("api/[controller]")]
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{

		private readonly ApplicationDbContext _db;

		public VillaAPIController(ApplicationDbContext db)
		{
			_db = db;
		}


		//Endpoint. For getting the List of Villas.
		//Swagger will identify this Endpoint as a GET Endpoint.
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{			
			return Ok(_db.Villas); //retrive all villas :D
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

			
			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

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
			if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
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

			Villa model = new()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Sqft = villaDTO.Sqft,
				Rate = villaDTO.Rate,
				ImageUrl = villaDTO.ImageUrl,
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details
			};

			_db.Villas.Add(model);
			_db.SaveChanges();

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


			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			_db.Villas.Remove(villa);
			_db.SaveChanges();

			return NoContent();
		}

		//Endpoint. Will Update whole record(OBJEKT).
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
		{
			if (villaDTO == null || id != villaDTO.Id)
			{
				return BadRequest();
			}

			Villa model = new()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Sqft = villaDTO.Sqft,
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				ImageUrl = villaDTO.ImageUrl,
				Rate = villaDTO.Rate,
			};

			_db.Villas.Update(model);
			_db.SaveChanges();

			return NoContent();
		}

		//Endpoint. Will Update just one.
		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

			VillaDTO villaDTO = new()
			{
				Id = villa.Id,
				Name = villa.Name,
				Occupancy = villa.Occupancy,
				Sqft = villa.Sqft,
				Amenity = villa.Amenity,
				Details = villa.Details,
				ImageUrl = villa.ImageUrl,
				Rate = villa.Rate,
			};

			if (villa == null)
			{
				return BadRequest();
			}

			//Will apply the "changes" to the found villa Object with de id.
			patchDTO.ApplyTo(villaDTO, ModelState);

			Villa model = new Villa()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Sqft = villaDTO.Sqft,
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				ImageUrl = villaDTO.ImageUrl,
				Rate = villaDTO.Rate
			};

			_db.Villas.Update(model);
			_db.SaveChanges();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return NoContent();
		}


	}
}
