using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Agents;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Infrastructure.Persistence.Repositories;

public sealed class AgentRepository(ApplicationDbContext context) : IAgentRepository
{
    public async Task<IReadOnlyList<Agent>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Agents
            .AsNoTracking()
            .Include(agent => agent.Properties)
            .OrderBy(agent => agent.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Agent?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Agents
            .Include(agent => agent.Properties)
            .FirstOrDefaultAsync(agent => agent.Id == id, cancellationToken);
    }

    public async Task AddAsync(Agent agent, CancellationToken cancellationToken)
    {
        await context.Agents.AddAsync(agent, cancellationToken);
    }

    public void Update(Agent agent)
    {
        context.Agents.Update(agent);
    }

    public void Remove(Agent agent)
    {
        context.Agents.Remove(agent);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
