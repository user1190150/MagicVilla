﻿using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	//Could be implemented like that [Route("api/[controller]")]
	[Route("api/villaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{

		//Endpoint.
		//Swagger will identify this Endpoint as a GET Endpoint.
		[HttpGet]
		public IEnumerable<VillaDTO> GetVillas()
		{
			return new List<VillaDTO> { 
			new VillaDTO {Id=1, Name="Pool View"},
			new VillaDTO {Id=2, Name="Beach View"}
			};
		}
	}
}
