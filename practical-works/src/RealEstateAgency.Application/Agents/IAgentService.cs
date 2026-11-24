using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Application.Agents;

public interface IAgentService
{
    Task<IReadOnlyList<AgentDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<AgentDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<AgentDto> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken);

    Task<AgentDto?> UpdateAsync(int id, UpdateAgentRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
