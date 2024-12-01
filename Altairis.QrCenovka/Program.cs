var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();
app.UseRequestLocalization("cs-CZ");
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
QuestPDF.Settings.EnableDebugging = true;

app.Run();
