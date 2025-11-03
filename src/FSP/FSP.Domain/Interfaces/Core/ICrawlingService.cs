using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.Core;

public interface ICrawlingService
{
    Task<Result<bool>> IsServerAliveAsync();
    Task<Result<List<URLInformation>>> CrawlPremierLeagueAsync();
    Task<Result<List<URLInformation>>> CrawlEnglandChampionshipAsync();
    Task<Result<List<URLInformation>>> CrawlRomaniaLiga1Async();
    Task<Result<List<URLInformation>>> CrawlTurkeySuperLigAsync();
    Task<Result<List<URLInformation>>> CrawlChineseSuperLeagueAsync();
    Task<Result<List<URLInformation>>> CrawlGermanyBundeslia1Async();
    Task<Result<List<URLInformation>>> CrawlItalySerieAAsync();
    Task<Result<List<URLInformation>>> CrawlFranceLigue1Async();
    Task<Result<List<URLInformation>>> CrawlFranceLigue2Async();
    Task<Result<List<URLInformation>>> CrawlSpainLaligaAsync();
    Task<Result<List<URLInformation>>> CrawlNetherlandsEredivisieAsync();
    Task<Result<List<URLInformation>>> CrawlAustraliaALeagueAsync();
    Task<Result<List<URLInformation>>> CrawlSwedenAllsvenskanAsync();
    Task<Result<List<URLInformation>>> CrawlArgentinaTorneoBetanoAsync();
    Task<Result<List<URLInformation>>> CrawlUSAMLSAsync();
    Task<Result<List<URLInformation>>> CrawlBrazilSerieABetanoAsync();
    Task<Result<List<URLInformation>>> CrawlEcuadorLigaProAsync();
    Task<Result<List<URLInformation>>> CrawlColombiaPrimeraAsync();
    Task<Result<List<URLInformation>>> CrawlChileLigaDePrimeraAsync();
    Task<Result<List<URLInformation>>> CrawlSaudiArabiaProfessionalLeagueAsync();
    Task<Result<List<URLInformation>>> CrawlMexicoLigaMXAsync();
    Task<Result<List<URLInformation>>> CrawlKoreaKLeague1Async();
}
