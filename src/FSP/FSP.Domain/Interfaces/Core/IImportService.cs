using FSP.Domain.Entities;

namespace FSP.Domain.Interfaces.Core;

public interface IImportService
{
    //Task<Result<bool>> ImportFromJsonAsync(string jsonContent, string dataType, string clubName, string leagueName, string nation);
    Task<Result<bool>> ImportFromJsonAsync(string jsonContent, string dataType, string clubName, string leagueName, string nation, string season);
}
