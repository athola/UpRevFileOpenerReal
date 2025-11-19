using Microsoft.Extensions.Logging;
using UpRevFileOpener.Services;

namespace UpRevFileOpener;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register platform-specific services
#if WINDOWS
        builder.Services.AddSingleton<IFileSaveService, WindowsFileSaveService>();
#else
        builder.Services.AddSingleton<IFileSaveService, DefaultFileSaveService>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
