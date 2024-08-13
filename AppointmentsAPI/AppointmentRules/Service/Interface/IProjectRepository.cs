using AppointmentRules.Models;

namespace AppointmentRules.Service.Interface
{
    public interface IProjectRepository
    {
        Task<Project> GetProjectByIdAsync(int projectId);
        Task AddProjectAsync(Project project);
    }
}
