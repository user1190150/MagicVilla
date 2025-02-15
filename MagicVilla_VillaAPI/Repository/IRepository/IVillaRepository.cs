﻿using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
	public interface IVillaRepository
	{
		Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true);
		Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null);	
		Task CreateAsync(Villa entity);
		Task UpdateAsync(Villa entity);
		Task RemoveAsync(Villa entity);
		Task SaveAsync();
		
	}
}
