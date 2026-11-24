namespace RealEstateAgency.Application.Contracts;

public sealed record PropertyTypeDto(int Id, string Name);

public sealed record DistrictDto(int Id, string Name, string City);

public sealed record DictionariesDto(
    IReadOnlyList<PropertyTypeDto> PropertyTypes,
    IReadOnlyList<DistrictDto> Districts,
    IReadOnlyList<AgentDto> Agents);
