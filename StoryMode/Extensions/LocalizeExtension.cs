using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using StoryMode.Services;
// Base class lives here now

namespace StoryMode.Extensions;

/// <summary>
/// A markup extension for Avalonia XAML that provides localized strings by key.
/// This extension simplifies the binding of keys to their localized values
/// using the <see cref="LanguageService"/> and ensures fallback behavior during design-time or runtime errors.
/// </summary>
/// <remarks>
/// This extension resolves the <see cref="LanguageService"/> from the application's dependency injection container
/// and creates a one-way data binding to the localized string corresponding to the specified key.
/// During design-time, or if localization fails, fallback values are displayed.
/// </remarks>
public class LocalizeExtension : MarkupExtension
{
    private readonly string _key;

    public LocalizeExtension(string key)
    {
        _key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // 1. Safety Check: If we are in the XAML Designer, App.Current might be null/empty
        if (App.Current is not App app || app.Services == null)
        {
            return $"[{_key}]"; 
        }

        // 2. Resolve the LanguageService from DI
        var langService = app.Services.GetRequiredService<LanguageService>();

        // 3. Create a standard Binding
        // We bind to the indexer 'Strings[Key]'
        var binding = new Binding
        {
            Source = langService, 
            Path = $"Strings[{_key}]",
            Mode = BindingMode.OneWay,
            FallbackValue = $"#{_key}#" 
        };

        return binding;
    }
}