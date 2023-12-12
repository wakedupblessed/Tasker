﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tasker.Application.DTOs;
using Tasker.Application.DTOs.Application;
using Tasker.Application.DTOs.Application.Task;
using Tasker.Application.EntitiesExtension;
using Tasker.Application.Interfaces.Repositories;
using Tasker.Application.Resolvers.DTOs;
using Tasker.Application.Resolvers.Interfaces;
using Tasker.Domain.Entities.Application;
using Tasker.Infrastructure.Data.Application;
using Task = Tasker.Domain.Entities.Application.Task;

namespace Tasker.Application.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IResolver<TaskResolvedPropertiesDto, TaskUpdateDto> _taskResolver;
    private readonly IResolver<Project, ProjectDto> _projectResolver;
    private readonly IResolver<User, UserDto> _userResolver;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    public TaskRepository(ApplicationContext context, IMapper mapper, 
        IResolver<TaskResolvedPropertiesDto, TaskUpdateDto> taskResolver,
        IResolver<User, UserDto> userResolver,
        IResolver<Project, ProjectDto> projectResolver)
    {
        _projectResolver = projectResolver;
        _userResolver = userResolver;
        _taskResolver = taskResolver;
        _context = context;
        _mapper = mapper;
    }

    public async Task<TaskDto?> CreateAsync(TaskDto dto)
    {
        var task = _mapper.Map<Task>(dto);

        task.Creator = await _userResolver.ResolveAsync(dto.Creator);
        task.Project = await _projectResolver.ResolveAsync(dto.Project);

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto?> UpdateAsync(TaskUpdateDto dto)
    {
        var task =  await GetTaskEntity(dto.Id);

        if (task is null)
        {
            return null;
        }

        var resolvedProperties = await _taskResolver.ResolveAsync(dto);

        task.Update(dto, resolvedProperties);

        _context.Entry(task).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task is null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TaskDto?> GetAsync(string id)
    {
        var task = await GetTaskEntity(id);

        return task is not null ? _mapper.Map<TaskDto>(task) : null;
    }

    private async Task<Task?> GetTaskEntity(string id)
        => await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Release)
            .Include(t => t.Creator)
            .Include(t => t.Status)
            .AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
}