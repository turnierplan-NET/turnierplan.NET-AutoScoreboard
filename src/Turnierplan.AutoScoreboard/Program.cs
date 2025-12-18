using Turnierplan.AutoScoreboard;

Console.WriteLine();
Console.WriteLine( "  __                                                     ___                                        __");
Console.WriteLine(@" /\ \__                        __                       /\_ \                                      /\ \__");
Console.WriteLine(@" \ \ ,_\  __  __  _ __    ___ /\_\     __   _ __   _____\//\ \      __      ___         ___      __\ \ ,_\");
Console.WriteLine(@"  \ \ \/ /\ \/\ \/\`'__\/' _ `\/\ \  /'__`\/\`'__\/\ '__`\\ \ \   /'__`\  /' _ `\     /' _ `\  /'__`\ \ \/");
Console.WriteLine(@"   \ \ \_\ \ \_\ \ \ \/ /\ \/\ \ \ \/\  __/\ \ \/ \ \ \L\ \\_\ \_/\ \L\.\_/\ \/\ \  __/\ \/\ \/\  __/\ \ \_");
Console.WriteLine(@"    \ \__\\ \____/\ \_\ \ \_\ \_\ \_\ \____\\ \_\  \ \ ,__//\____\ \__/.\_\ \_\ \_\/\_\ \_\ \_\ \____\\ \__\");
Console.WriteLine(@"     \/__/ \/___/  \/_/  \/_/\/_/\/_/\/____/ \/_/   \ \ \/ \/____/\/__/\/_/\/_/\/_/\/_/\/_/\/_/\/____/ \/__/");
Console.WriteLine(@"                                                     \ \_\");
Console.WriteLine(@"                                                      \/_/   turnierplan.NET AutoScoreboard");
Console.WriteLine();

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AutoScoreboardOptions>(builder.Configuration.GetSection("AutoScoreboard"));
builder.Services.AddSingleton<TournamentLoader>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<TournamentLoader>());
builder.Services.AddTransient<ITournamentLoader>(sp => sp.GetRequiredService<TournamentLoader>());
builder.Services.AddRazorPages();

var app = builder.Build();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
