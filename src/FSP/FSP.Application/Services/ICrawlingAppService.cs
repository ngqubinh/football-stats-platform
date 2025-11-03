using FSP.Application.DTOs.Core;
using FSP.Application.Mappings;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;

namespace FSP.Application.Services;

public interface ICrawlingAppService
{
    Task<Result<List<URLInformationDto>>> CrawlPremierLeagueAsync();
    Task<Result<List<URLInformationDto>>> CrawlEnglandChampionshipAsync();
    Task<Result<List<URLInformationDto>>> CrawlRomaniaLiga1Async();
    Task<Result<List<URLInformationDto>>> CrawlTurkeySuperLigAsync();
    Task<Result<List<URLInformationDto>>> CrawlChineseSuperLeagueAsync();
    Task<Result<List<URLInformationDto>>> CrawlGermanyBundeslia1Async();
    Task<Result<List<URLInformationDto>>> CrawlItalySerieAAsync();
    Task<Result<List<URLInformationDto>>> CrawlFranceLigue1Async();
    Task<Result<List<URLInformationDto>>> CrawlFranceLigue2Async();
    Task<Result<List<URLInformationDto>>> CrawlSpainLaligaAsync();
    Task<Result<List<URLInformationDto>>> CrawlNetherlandsEredivisieAsync();
    Task<Result<List<URLInformationDto>>> CrawlAustraliaALeagueAsync();
    Task<Result<List<URLInformationDto>>> CrawlSwedenAllsvenskanAsync();
    Task<Result<List<URLInformationDto>>> CrawlArgentinaTorneoBetanoAsync();
    Task<Result<List<URLInformationDto>>> CrawlUSAMLSAsync();
    Task<Result<List<URLInformationDto>>> CrawlBrazilSerieABetanoAsync();
    Task<Result<List<URLInformationDto>>> CrawlEcuadorLigaProAsync();
    Task<Result<List<URLInformationDto>>> CrawlColombiaPrimeraAsync();
    Task<Result<List<URLInformationDto>>> CrawlChileLigaDePrimeraAsync();
    Task<Result<List<URLInformationDto>>> CrawlSaudiArabiaProfessionalLeagueAsync();
    Task<Result<List<URLInformationDto>>> CrawlMexicoLigaMXAsync();
    Task<Result<List<URLInformationDto>>> CrawlKoreaKLeague1Async();
}

public class CrawlingAppService : ICrawlingAppService
{
    private readonly ICrawlingService _crawling;
    private readonly ICoreMappingService _core;

    public CrawlingAppService(ICrawlingService crawling, ICoreMappingService core)
    {
        this._crawling = crawling;
        this._core = core;
    }

    public async Task<Result<List<URLInformationDto>>> CrawlArgentinaTorneoBetanoAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlArgentinaTorneoBetanoAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlAustraliaALeagueAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlAustraliaALeagueAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlBrazilSerieABetanoAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlBrazilSerieABetanoAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlChileLigaDePrimeraAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlChileLigaDePrimeraAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlChineseSuperLeagueAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlChineseSuperLeagueAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlColombiaPrimeraAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlColombiaPrimeraAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlEcuadorLigaProAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlEcuadorLigaProAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlEnglandChampionshipAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlEnglandChampionshipAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlFranceLigue1Async()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlFranceLigue1Async();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlFranceLigue2Async()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlFranceLigue2Async();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlGermanyBundeslia1Async()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlGermanyBundeslia1Async();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlItalySerieAAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlItalySerieAAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlKoreaKLeague1Async()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlKoreaKLeague1Async();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlMexicoLigaMXAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlMexicoLigaMXAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlNetherlandsEredivisieAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlNetherlandsEredivisieAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlPremierLeagueAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlPremierLeagueAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlRomaniaLiga1Async()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlRomaniaLiga1Async();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }

    }

    public async Task<Result<List<URLInformationDto>>> CrawlSaudiArabiaProfessionalLeagueAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlSaudiArabiaProfessionalLeagueAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlSpainLaligaAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlSpainLaligaAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlSwedenAllsvenskanAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlSwedenAllsvenskanAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlTurkeySuperLigAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlTurkeySuperLigAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }

    public async Task<Result<List<URLInformationDto>>> CrawlUSAMLSAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlUSAMLSAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }
}
