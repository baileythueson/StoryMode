using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using StoryMode.Models;
using StoryMode.Services;

namespace StoryMode.ViewModels;

/// <summary>
/// Represents the ViewModel responsible for managing the settings of the application,
/// including language preferences.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    public ObservableCollection<LanguageOption> Languages => LanguageManager.Instance.AvailableLanguages;

    public LanguageOption? CurrentLanguage => _selectedLanguage;
    
    private LanguageOption? _selectedLanguage;
    public LanguageOption? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (SetProperty(ref _selectedLanguage, value) && value != null)
            {
                // Change live language
                LanguageManager.Instance.LoadLanguage(value.IsoCode);
                
                // update persisted settings
                SettingsService.Instance.CurrentSettings.Language = value.IsoCode;
                SettingsService.Instance.Save();
            }
        }
    }

    public SettingsViewModel()
    {
        var currentCode = SettingsService.Instance.CurrentSettings.Language;
        _selectedLanguage = Languages.FirstOrDefault(x => x.IsoCode == currentCode);
    }
}