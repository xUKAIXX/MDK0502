using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Agents;

public interface IAgentRepository
{
    Task<IReadOnlyList<Agent>> GetAllAsync(CancellationToken cancellationToken);

    Task<Agent?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Agent agent, CancellationToken cancellationToken);

    void Update(Agent agent);

    void Remove(Agent agent);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
