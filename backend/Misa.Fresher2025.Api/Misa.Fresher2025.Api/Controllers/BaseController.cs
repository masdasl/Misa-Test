using Microsoft.AspNetCore.Mvc;
using Misa.Core.DTo;
using Misa.Core.Interfaces.Service;
using Misa.Core.Model;
using System;
using System.Collections.Generic;

namespace Misa.Fresher2025.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TKey, TKeys, TDto> : ControllerBase
        where TEntity : class
    {
        private readonly IBaseService<TEntity, TKey, TKeys, TDto> _service;

        public BaseController(IBaseService<TEntity, TKey, TKeys, TDto> service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetById(TKey id)
        {
            var response = _service.GetById(id);
            if (response.Data == null)
                return Ok(response);
            return Ok(response);
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] TDto[] entity)
        {
            var response = _service.Insert(entity);
            return StatusCode(201, response);
        }
        [HttpPost("data-table")]
        public virtual IActionResult LoadTaBleData([FromBody] DataTableRequest entity)
        {
            var response = _service.LoadCustomersTable(entity);
            return StatusCode(201, response);
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update(TKey id, [FromBody] TDto entity)
        {
            var response = _service.Update(id, entity);
            return Ok(response);
        }

        [HttpPut("delete")]
        public virtual IActionResult Delete([FromBody] TKeys id)
        {
            var response = _service.Delete(id);
            return Ok(response);
        }
        [HttpPost("update-image")]
        public IActionResult UploadImg(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Ảnh không hợp lệ");
            var imagePath = _service.UploadImg(image);
            return Ok(imagePath);
        }
    }
}
