﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.Application.DTOs.Application.Project;
using Tasker.Application.Interfaces.Services;


namespace Tasker.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var dto = await _service.GetByIdAsync(id);

            return dto is null
                ? NotFound()
                : Ok(dto);
        }
        
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectCreateDto dto)
        {
            var createdDto = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = createdDto.Id }, createdDto);
        }
        
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProjectUpdateDto dto)
        {
            var updatedDto = await _service.UpdateAsync(dto);

            return Ok(updatedDto);
        }
    
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var deleted = await _service.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound(new { error = $"Project with id {id} does not exist" });
        }
    }
}