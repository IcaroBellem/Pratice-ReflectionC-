using AppointmentRules.Data;
using AppointmentRules.Models;
using AppointmentRules.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace AppointmentRules.Service.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.Include(p => p.Tasks)
            .ThenInclude(t => t.Members)
            .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProjectAsync(Project project)
        {
            _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }
    }
}
