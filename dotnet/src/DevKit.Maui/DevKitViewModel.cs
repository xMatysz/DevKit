using CommunityToolkit.Mvvm.ComponentModel;

namespace DevKit.Maui;

public abstract class DevKitViewModel : ObservableValidator
{
    public abstract string Title { get; }
}
