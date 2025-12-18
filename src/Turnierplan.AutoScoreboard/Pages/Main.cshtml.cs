using System.Collections.Immutable;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Turnierplan.AutoScoreboard.Pages;

public sealed class Main : PageModel
{
    private readonly ITournamentLoader _tournamentLoader;
    private readonly AutoScoreboardOptions _options;

    public Main(ITournamentLoader tournamentLoader, IOptions<AutoScoreboardOptions> options)
    {
        _tournamentLoader = tournamentLoader;
        _options = options.Value;
    }

    public ImmutableArray<(long Delay, string TournamentUrl)> Switches { get; private set; } = [];

    public void OnGet()
    {
        var timeWithOffset = DateTime.UtcNow.AddSeconds(_options.ClockSkewForSwitchInSeconds);
        var switches = new List<(long Delay, string TournamentUrl)>();

        foreach (var entry in _tournamentLoader.Tournaments.OrderByDescending(x => x.StartsAt))
        {
            var startsInMilliseconds = (entry.StartsAt - timeWithOffset).Ticks / TimeSpan.TicksPerMillisecond;
            var tournamentUrl = GenerateTournamentUrl(entry.TournamentId);
            switches.Add((startsInMilliseconds, tournamentUrl));

            var hasBegun = entry.StartsAt < timeWithOffset;
            if (hasBegun)
            {
                // If this tournament has already started, we don't have to process any more tournaments since they are ordered by descending start timestamp
                break;
            }
        }

        if (switches.Count > 0)
        {
            // Replace the "last" switch with a delay of 1ms so this tournament is shown immediately
            var last = switches[^1];
            switches.RemoveAt(switches.Count - 1);
            switches.Add((1, last.TournamentUrl));
        }

        Switches = [..switches];
    }

    private string GenerateTournamentUrl(string tournamentId)
    {
        var sb = new StringBuilder();

        sb.Append(_options.BaseUrl.TrimEnd("/"));
        sb.Append("/TournamentFullscreen?id=");
        sb.Append(tournamentId);
        sb.Append("&showQrCode=");
        sb.Append(_options.FullscreenViewIncludeQrCode ? "true" : "false");
        sb.Append("&autoReload=");
        sb.Append(_options.FullscreenViewRefreshInterval);

        return sb.ToString();
    }
}
