﻿using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using SIT.Manager.Avalonia.Interfaces;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels.Installation;

public partial class InstallViewModel : InstallationViewModelBase
{
    private readonly IInstallerService _installerService;

    private readonly Progress<double> _downloadProgress = new();
    private readonly Progress<double> _extractionProgress = new();

    [ObservableProperty]
    private double _downloadProgressPercentage = 0;

    [ObservableProperty]
    private double _extractionProgressPercentage = 0;

    [ObservableProperty]
    private double _installProgressPercentage = 0;

    public InstallViewModel(IInstallerService installerService) : base()
    {
        _installerService = installerService;

        _downloadProgress.ProgressChanged += DownloadProgress_ProgressChanged;
        _extractionProgress.ProgressChanged += ExtractionProgress_ProgressChanged;

        this.WhenActivated(async (CompositeDisposable disposables) => await RunInstaller());
    }

    private void DownloadProgress_ProgressChanged(object? sender, double e)
    {
        // For some reason this is 100 times smaller than expected so just x100 I guess?
        DownloadProgressPercentage = e * 100;
        UpdateInstallProgress();
    }

    private void ExtractionProgress_ProgressChanged(object? sender, double e)
    {
        ExtractionProgressPercentage = e;
        UpdateInstallProgress();
    }

    private void UpdateInstallProgress()
    {
        InstallProgressPercentage = (DownloadProgressPercentage + ExtractionProgressPercentage) / 2;
    }

    private async Task RunInstaller()
    {
        DownloadProgressPercentage = 0;
        ExtractionProgressPercentage = 0;

        try
        {
            if (IsServerInstall)
            {
                await _installerService.InstallServer(CurrentInstallProcessState.RequestedVersion, CurrentInstallProcessState.SptAkiInstallPath, _downloadProgress, _extractionProgress);
            }
            else if (IsSitInstall)
            {
                await _installerService.InstallSit(CurrentInstallProcessState.RequestedVersion, CurrentInstallProcessState.EftInstallPath, _downloadProgress, _extractionProgress);
            }
        }
        catch
        {
            // TODO show the information that we need out of this
            return;
        }

        ProgressInstall();
    }
}
