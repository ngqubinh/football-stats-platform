using FSP.Application.Services;
using FSP.Domain.Entities.Core;
using Microsoft.AspNetCore.Mvc;

namespace FSP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlJobsController : ControllerBase
    {
        private readonly ICrawlingAppService _crawlingService;
        private readonly ILogger<CrawlJobsController> _logger;

        public CrawlJobsController(ICrawlingAppService crawlingService, ILogger<CrawlJobsController> logger)
        {
            _crawlingService = crawlingService ?? throw new ArgumentNullException(nameof(crawlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("premier-league")]
        public async Task<ActionResult<List<URLInformation>>> GetPremierLeagueCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(GetPremierLeagueCrawl)
            });

            _logger.LogInformation("Starting crawl job for Premier League");

            try
            {
                var result = await _crawlingService.CrawlPremierLeagueAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Premier League crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Premier League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Premier League crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Premier League crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Premier League crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("england-championship")]
        public async Task<ActionResult<List<URLInformation>>> GetEnglandChampionshipCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetEnglandChampionshipCrawl)
            });

            _logger.LogInformation("Starting crawl job for England Championship");

            try
            {
                var result = await _crawlingService.CrawlEcuadorLigaProAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start England Championship crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start England Championship crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for England Championship crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed England Championship crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing England Championship crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("romania-liga1")]
        public async Task<ActionResult<List<URLInformation>>> GetRomaniaLiga1LeagueCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = this._logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetRomaniaLiga1LeagueCrawl)
            });

            this._logger.LogInformation("Starting crawl job for Romania Liga 1 League");

            try
            {
                var result = await this._crawlingService.CrawlRomaniaLiga1Async();
                if (!result.Success)
                {
                    this._logger.LogWarning("Failed to start Romania Liga 1 League crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Romania Liga 1 League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    this._logger.LogWarning("No data retrieved for Romania Liga 1 League crawl job");
                    return Ok(new List<URLInformation>());
                }

                this._logger.LogInformation("Successfully processed Romania Liga 1 League crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error processing Premier League crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("turkey-superlig")]
        public async Task<ActionResult<List<URLInformation>>> GetTurkeySuperLigCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(GetTurkeySuperLigCrawl)
            });

            _logger.LogInformation("Starting crawl job for Turkey Super Lig");

            try
            {
                var result = await _crawlingService.CrawlTurkeySuperLigAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Turkey Super Lig crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Turkey Super Lig crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Turkey Super Lig crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Turkey Super Lig crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Turkey Super Lig crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("chinese-superleague")]
        public async Task<ActionResult<List<URLInformation>>> GetChineseSuperLeagueCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(GetTurkeySuperLigCrawl)
            });

            _logger.LogInformation("Starting crawl job for Chinese Super League");

            try
            {
                var result = await _crawlingService.CrawlChineseSuperLeagueAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Chinese Super League crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Chinese Super League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Chinese Super League crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Chinese Super League crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Chinese Super League crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("germany-bundesliga1")]
        public async Task<ActionResult<List<URLInformation>>> GetGermanyBundesliga1Crawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(GetTurkeySuperLigCrawl)
            });

            _logger.LogInformation("Starting crawl job for Germany Bundeslia 1");

            try
            {
                var result = await _crawlingService.CrawlGermanyBundeslia1Async();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Germany Bundeslia 1 crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Germany Bundeslia 1 crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Germany Bundeslia 1 crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Germany Bundeslia 1crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Germany Bundeslia 1 crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("italy-seriea")]
        public async Task<ActionResult<List<URLInformation>>> GetItalySerieACrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(GetTurkeySuperLigCrawl)
            });

            _logger.LogInformation("Starting crawl job for Italy Serie A");

            try
            {
                var result = await _crawlingService.CrawlItalySerieAAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Italy Serie A crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Italy Serie A crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Italy Serie A crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Italy Serie A crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Italy Serie A crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("france-ligue1")]
        public async Task<ActionResult<List<URLInformation>>> GetFranceLigue1Crawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetFranceLigue1Crawl)
            });

            _logger.LogInformation("Starting crawl job for France Ligue 1");

            try
            {
                var result = await _crawlingService.CrawlFranceLigue1Async();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start France Ligue 1 crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start France Ligue 1 crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for France Ligue 1 crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed France Ligue 1 crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing France Ligue 1 crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("france-ligue2")]
        public async Task<ActionResult<List<URLInformation>>> GetFranceLigue2Crawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetFranceLigue2Crawl)
            });

            _logger.LogInformation("Starting crawl job for France Ligue 2");

            try
            {
                var result = await _crawlingService.CrawlFranceLigue2Async();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start France Ligue 1 crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start France Ligue 1 crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for France Ligue 2 crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed France Ligue 2 crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing France Ligue 2 crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("spain-laliga1")]
        public async Task<ActionResult<List<URLInformation>>> GetSpainLaliga1Crawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetSpainLaliga1Crawl)
            });

            _logger.LogInformation("Starting crawl job for Spain Laliga 1");

            try
            {
                var result = await _crawlingService.CrawlSpainLaligaAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Spain Laliga 1 crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Spain Laliga 1 crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Spain Laliga 1 crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Spain Laliga 1 crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Spain Laliga 1 crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("netherlands-eredivisie")]
        public async Task<ActionResult<List<URLInformation>>> GetNetherlandsEredivisieCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetNetherlandsEredivisieCrawl)
            });

            _logger.LogInformation("Starting crawl job for Netherlands Eredivisie");

            try
            {
                var result = await _crawlingService.CrawlNetherlandsEredivisieAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Netherlands Eredivisie crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Netherlands Eredivisie crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Netherlands Eredivisie crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Netherlands Eredivisie crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Netherlands Eredivisie crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("australia-aleague")]
        public async Task<ActionResult<List<URLInformation>>> GetAustraliaALeagueCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetNetherlandsEredivisieCrawl)
            });

            _logger.LogInformation("Starting crawl job for Australia A League");

            try
            {
                var result = await _crawlingService.CrawlAustraliaALeagueAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Australia A League crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Australia A League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Australia A League crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Australia A League crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Australia A League crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("sweden-allsvenskan")]
        public async Task<ActionResult<List<URLInformation>>> GetSwedenAllsvenskanCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetSwedenAllsvenskanCrawl)
            });

            _logger.LogInformation("Starting crawl job for Sweden Allsvenskan");

            try
            {
                var result = await _crawlingService.CrawlSwedenAllsvenskanAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Sweden Allsvenskan crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Sweden Allsvenskan crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Sweden Allsvenskan crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Sweden Allsvenskan crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Sweden Allsvenskan crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("argentina-torneobetano")]
        public async Task<ActionResult<List<URLInformation>>> GetArgentinaCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetArgentinaCrawl)
            });

            _logger.LogInformation("Starting crawl job for Argentina");

            try
            {
                var result = await _crawlingService.CrawlArgentinaTorneoBetanoAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Argentina crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Argentina crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Argentinacrawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processedArgentinarawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processingArgentina crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("usa-mls")]
        public async Task<ActionResult<List<URLInformation>>> GetUSAMLSCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetUSAMLSCrawl)
            });

            _logger.LogInformation("Starting crawl job for USA MLS");

            try
            {
                var result = await _crawlingService.CrawlUSAMLSAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start USA MLS crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start USA MLS crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for USA MLS crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed USA MLS crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing USA MLS crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("brazil-seriea")]
        public async Task<ActionResult<List<URLInformation>>> GetBrazilSerieACrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetBrazilSerieACrawl)
            });

            _logger.LogInformation("Starting crawl job for Brazil Serie A Betano");

            try
            {
                var result = await _crawlingService.CrawlBrazilSerieABetanoAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Brazil Serie A Betano crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Brazil Serie A Betano crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Brazil Serie A Betano crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Brazil Serie A Betano crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Brazil Serie A Betano crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("ecuador-ligapro")]
        public async Task<ActionResult<List<URLInformation>>> GetEcuadorLigaProCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetEcuadorLigaProCrawl)
            });

            _logger.LogInformation("Starting crawl job for Ecuador Liga Pro");

            try
            {
                var result = await _crawlingService.CrawlEcuadorLigaProAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Ecuador Liga Pro crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Ecuador Liga Pro crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Ecuador Liga Pro crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Ecuador Liga Pro crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Ecuador Liga Pro crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("colombia-primera")]
        public async Task<ActionResult<List<URLInformation>>> GetColombiaPrimeraCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetColombiaPrimeraCrawl)
            });

            _logger.LogInformation("Starting crawl job for Colombia Primera");

            try
            {
                var result = await _crawlingService.CrawlColombiaPrimeraAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Colombia Primera crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Colombia Primera crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Colombia Primera crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Colombia Primera crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Colombia Primera crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("chile-ligadeprimera")]
        public async Task<ActionResult<List<URLInformation>>> GetChileLigaDePrimeraCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetChileLigaDePrimeraCrawl)
            });

            _logger.LogInformation("Starting crawl job for Chile De Primera");

            try
            {
                var result = await _crawlingService.CrawlChileLigaDePrimeraAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Chile De Primera crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Chile De Primera crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Chile De Primera crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Chile De Primera crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Chile De Primera crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("saudiarabia-professionalleague")]
        public async Task<ActionResult<List<URLInformation>>> GetSaudiArabiaProfessionalLeagueCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetSaudiArabiaProfessionalLeagueCrawl)
            });

            _logger.LogInformation("Starting crawl job for Saudi Arabia Professional League");

            try
            {
                var result = await _crawlingService.CrawlSaudiArabiaProfessionalLeagueAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Saudi Arabia Professional League crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Saudi Arabia Professional League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Saudi Arabia Professional League crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Saudi Arabia Professional League crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Saudi Arabia Professional League crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }

        [HttpGet("mexico-ligamx")]
        public async Task<ActionResult<List<URLInformation>>> GetMexicoLigaMXCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetMexicoLigaMXCrawl)
            });

            _logger.LogInformation("Starting crawl job for Mexico");

            try
            {
                var result = await _crawlingService.CrawlMexicoLigaMXAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Mexico crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Saudi Arabia Professional League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Mexico crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Mexico crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Mexico crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }
        
        [HttpGet("korea-kleague1")]
        public async Task<ActionResult<List<URLInformation>>> GetKoreaKLeague1Crawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetKoreaKLeague1Crawl)
            });

            _logger.LogInformation("Starting crawl job for Korea K League 1");

            try
            {
                var result = await _crawlingService.CrawlKoreaKLeague1Async();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Korea K League 1 crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Saudi Arabia Professional League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Korea K League 1 crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Korea K League 1 crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Korea K League 1 crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }
    }
}
