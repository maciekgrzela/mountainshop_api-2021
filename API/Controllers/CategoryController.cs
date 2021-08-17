﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Category;
using Application.Product;
using Application.Product.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/categories")]
    public class CategoryController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagingParams queryParams)
        {
            var categories = await Mediator.Send(new GetAllCategories.Query{ QueryParams = queryParams});
            return HandlePaginationHeader(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetCategory.Query{Id = id});
        }
        
        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductResource>>> GetProductsForCategoryAsync(Guid id)
        {
            return await Mediator.Send(new GetProductsForCategory.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateCategory.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateCategory.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteCategory.Command { Id = id });
            return NoContent();
        }
    }
}