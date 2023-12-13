﻿using Microsoft.AspNetCore.Mvc;
using Tasker.Application.DTOs.Application.Release;
using Tasker.Application.Interfaces.Repositories;
using Tasker.Application.Repositories;

namespace Tasker.Controllers
{
    [ApiController]
    [Route("api/releases")]
    public class ReleaseController : ControllerBase
    {
        private readonly IReleaseRepository _releaseRepository;

        public ReleaseController(IReleaseRepository releaseRepository)
        {
            _releaseRepository = releaseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        => Ok(await _releaseRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var dto = await _releaseRepository.GetAsync(id);

            return dto is null
                ? NotFound()
                : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReleaseCreateDto dto)
        {
            var createdDto = await _releaseRepository.CreateAsync(dto);

            return createdDto is null
                ? Conflict(new { error = $"Project with name {dto.Title} already exists" })
                : CreatedAtAction(nameof(Get), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReleaseUpdateDto dto)
        {
            var updatedDto = await _releaseRepository.UpdateAsync(dto);

            return updatedDto is null
                ? NotFound(new { error = $"Project with id {dto.Id} does not exist" })
                : Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var deleted = await _releaseRepository.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound(new { error = $"Project with id {id} does not exist" });
        }
    }
}