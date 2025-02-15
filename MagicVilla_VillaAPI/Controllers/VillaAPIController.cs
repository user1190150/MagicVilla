﻿using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		protected APIResponse _response;
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;

		public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
		{
			_dbVilla = dbVilla;
			_mapper = mapper;
			this._response = new();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillas()
		{
			IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
			_response.Result = _mapper.Map<List<VillaDTO>>(villaList);
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}

		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			var villa = await _dbVilla.GetAsync(u => u.Id == id);

			if (villa == null)
			{
				return NotFound();
			}
		
			_response.Result = _mapper.Map<VillaDTO>(villa);
			_response.StatusCode = HttpStatusCode.OK;

			return Ok(_response);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
		{
			if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa already Exists!");
				return BadRequest(ModelState);
			}

			if (createDTO == null)
			{
				return BadRequest(createDTO);
			}

			Villa villa = _mapper.Map<Villa>(createDTO);

			await _dbVilla.CreateAsync(villa);
			_response.Result = _mapper.Map<VillaDTO>(villa);
			_response.StatusCode = HttpStatusCode.Created;

			//Produces a 201 response.
			return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
		}

		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}


			var villa = await _dbVilla.GetAsync(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			await _dbVilla.RemoveAsync(villa);
			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;
			return Ok(_response);
			
		}

		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
		{
			if (updateDTO == null || id != updateDTO.Id)
			{
				return BadRequest();
			}

			Villa model = _mapper.Map<Villa>(updateDTO);

			await _dbVilla.UpdateAsync(model);

			return NoContent();
		}

		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

			VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

			if (villa == null)
			{
				return BadRequest();
			}

			//Will apply the "changes" to the found villa Object with de id.
			patchDTO.ApplyTo(villaDTO, ModelState);

			Villa model = _mapper.Map<Villa>(villaDTO);

			await _dbVilla.UpdateAsync(model);
			

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return NoContent();
		}


	}
}
