using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
		{			
			return Ok(await _db.Villas.ToListAsync()); //retrive all villas :D
		}

		//Endpoint. For getting one specific Villa.
		//or [HttpGet("{id:int}")] then id must be a integer.
		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<VillaDTO>> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			
			var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);

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
		public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaDTO)
		{
			if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa already Exists!");
				return BadRequest(ModelState);
			}

			if (villaDTO == null)
			{
				return BadRequest(villaDTO);
			}

			
			// EF Core will automaticly populate the ID. See Villa.cs.
			Villa model = new()
			{				
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Sqft = villaDTO.Sqft,
				Rate = villaDTO.Rate,
				ImageUrl = villaDTO.ImageUrl,
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details
			};

			await _db.Villas.AddAsync(model);
			await _db.SaveChangesAsync();

			//Produces a 201 response.
			return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
		}

		//Endpoint. For deleting a Villa.
		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}


			var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			_db.Villas.Remove(villa);
			await _db.SaveChangesAsync();

			return NoContent();
		}

		//Endpoint. Will Update whole record(OBJEKT).
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaDTO)
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
			await _db.SaveChangesAsync();

			return NoContent();
		}

		//Endpoint. Will Update just one.
		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

			VillaUpdateDTO villaDTO = new()
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
			await _db.SaveChangesAsync();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return NoContent();
		}


	}
}
