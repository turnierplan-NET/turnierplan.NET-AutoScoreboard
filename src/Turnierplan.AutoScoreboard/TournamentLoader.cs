using Microsoft.Extensions.Options;
using Turnierplan.Adapter;
using Turnierplan.Adapter.Enums;

namespace Turnierplan.AutoScoreboard;

public interface ITournamentLoader
{
    IReadOnlyList<TournamentStartInfo> Tournaments { get; }
}

internal sealed class TournamentLoader : ITournamentLoader, IHostedService
{
    private readonly AutoScoreboardOptions _options;
    private readonly ILogger<TournamentLoader> _logger;
    private readonly List<TournamentStartInfo> _tournaments = [];

    public TournamentLoader(IOptions<AutoScoreboardOptions> options, ILogger<TournamentLoader> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public IReadOnlyList<TournamentStartInfo> Tournaments => _tournaments.AsReadOnly();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading tournaments from API");

        var config = new TurnierplanClientOptions(_options.BaseUrl, _options.ApiKeyId, _options.ApiKeySecret);
        using var client = new TurnierplanClient(config);

        var tournamentHeaders = await client.GetTournaments(_options.FolderId);
        _logger.LogInformation("Loaded {tournamentCount} tournament headers", tournamentHeaders.Count);

        for (var i = 0; i < tournamentHeaders.Count; i++)
        {
            var header = tournamentHeaders[i];

            if (header.Visibility is not Visibility.Public)
            {
                _logger.LogInformation("Skipping tournament {tournamentIndex} of {totalTournaments} with id '{tournamentId}' because it is not public", i  + 1, tournamentHeaders.Count, header.Id);
                continue;
            }

            _logger.LogInformation("Loading tournament {tournamentIndex} of {totalTournaments} with id '{tournamentId}'", i  + 1, tournamentHeaders.Count, header.Id);

            var tournament = await client.GetTournament(header.Id);

            DateTime? firstKickoff = null;
            foreach (var match in tournament.Matches)
            {
                if (match.Kickoff.HasValue && (!firstKickoff.HasValue || firstKickoff.Value > match.Kickoff))
                {
                    firstKickoff = match.Kickoff;
                }
            }

            if (firstKickoff.HasValue)
            {
                _tournaments.Add(new TournamentStartInfo(header.Id, firstKickoff.Value));
            }
        }

        _logger.LogInformation("Tournament start information for {tournamentCount} tournaments is now available", _tournaments.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
