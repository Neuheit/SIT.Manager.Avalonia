using FluentAvalonia.UI.Controls;
using SIT.Manager.Interfaces;
using SIT.Manager.ViewModels.Play;
using System;
using System.Threading.Tasks;

namespace SIT.Manager.Views.Play;

public partial class CreateServerDialogView : ContentDialog
{
    private const string DEFAULT_SERVER_ADDRESS = "http://127.0.0.1:6969";

    private readonly CreateServerDialogViewModel dc;

    protected override Type StyleKeyOverride => typeof(ContentDialog);

    public CreateServerDialogView(ILocalizationService localizationService, bool isEdit, string currentServerAddress = DEFAULT_SERVER_ADDRESS)
    {
        dc = new CreateServerDialogViewModel(currentServerAddress, isEdit, localizationService);
        DataContext = dc;
        InitializeComponent();
    }

    public new Task<(ContentDialogResult, Uri)> ShowAsync()
    {
        return ShowAsync(null).ContinueWith(t => (t.Result, dc.ServerUri));
    }
}
