using RealEstateAgency.Application.Contracts;
using RealEstateAgency.Domain;

namespace RealEstateAgency.Application.Agents;

public sealed class AgentService : IAgentService
{
    private readonly IAgentRepository _agents;

    public AgentService(IAgentRepository agents)
    {
        _agents = agents;
    }

    public async Task<IReadOnlyList<AgentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var agents = await _agents.GetAllAsync(cancellationToken);
        return agents.Select(Map).ToList();
    }

    public async Task<AgentDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var agent = await _agents.GetByIdAsync(id, cancellationToken);
        return agent is null ? null : Map(agent);
    }

    public async Task<AgentDto> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken)
    {
        var agent = Agent.Create(
            request.FullName,
            request.Phone,
            request.Email,
            request.ExperienceYears);

        await _agents.AddAsync(agent, cancellationToken);
        await _agents.SaveChangesAsync(cancellationToken);

        return Map(agent);
    }

    public async Task<AgentDto?> UpdateAsync(int id, UpdateAgentRequest request, CancellationToken cancellationToken)
    {
        var agent = await _agents.GetByIdAsync(id, cancellationToken);
        if (agent is null)
        {
            return null;
        }

        agent.Update(
            request.FullName,
            request.Phone,
            request.Email,
            request.ExperienceYears);

        _agents.Update(agent);
        await _agents.SaveChangesAsync(cancellationToken);

        return Map(agent);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var agent = await _agents.GetByIdAsync(id, cancellationToken);
        if (agent is null)
        {
            return false;
        }

        _agents.Remove(agent);
        await _agents.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AgentDto Map(Agent agent) => new(
        agent.Id,
        agent.FullName,
        agent.Phone,
        agent.Email,
        agent.ExperienceYears,
        agent.Properties.Count);
}
