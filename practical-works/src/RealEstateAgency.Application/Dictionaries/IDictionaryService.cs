using RealEstateAgency.Application.Contracts;

namespace RealEstateAgency.Application.Dictionaries;

public interface IDictionaryService
{
    Task<DictionariesDto> GetDictionariesAsync(CancellationToken cancellationToken);
}
