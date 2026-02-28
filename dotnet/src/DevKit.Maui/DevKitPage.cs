namespace DevKit.Maui;

public abstract class DevKitPage<TViewModel> : ContentPage
{
    public TViewModel ViewModel { get; set; }

    protected DevKitPage(TViewModel viewModel)
    {
        BindingContext = viewModel;
        ViewModel = viewModel;
    }
}
