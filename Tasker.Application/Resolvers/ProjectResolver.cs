﻿using Microsoft.EntityFrameworkCore;
using Tasker.Application.DTOs;
using Tasker.Application.Resolvers.Interfaces;
using Tasker.Domain.Entities.Application;
using Tasker.Domain.Exceptions;
using Tasker.Infrastructure.Data.Application;

namespace Tasker.Application.Resolvers;

public class ProjectResolver : IResolver<Project, ProjectDto>
{
    private readonly ApplicationContext _context;

    public ProjectResolver(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Project> ResolveAsync(ProjectDto dto)
        => await _context.Projects.FirstOrDefaultAsync(p => p.Id == dto.Id)
           ?? throw new InvalidEntityException($"Project with id {dto.Id} doesnt exists");
}