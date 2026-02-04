using System.ComponentModel;
using Avalonia.Controls;
using StoryMode.Services;
using Ursa.Controls;

namespace StoryMode.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closing += OnClosing;
    }

    private async void OnClosing(object sender, WindowClosingEventArgs e)
    {
        if (ProjectManager.Instance.IsDirty)
        {
            e.Cancel = true;
            
            var result = await MessageBox.ShowAsync()
        }
    }
}