namespace Turnierplan.AutoScoreboard;

public sealed record AutoScoreboardOptions
{
    public required string BaseUrl { get; init; }

    public required string ApiKeyId { get; init; }

    public required string ApiKeySecret { get; init; }

    public required string FolderId { get; init; }

    public required int ClockSkewForSwitchInSeconds { get; init; }

    public required int FullscreenViewRefreshInterval { get; init; }

    public required bool FullscreenViewIncludeQrCode { get; init; }
}
