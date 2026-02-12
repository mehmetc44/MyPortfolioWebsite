using System;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.Services;
public interface IProjectService
{
    Task<List<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task CreateProjectAsync(Project project);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Guid id);
}
