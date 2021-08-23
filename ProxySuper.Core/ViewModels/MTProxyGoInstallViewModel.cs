﻿using MvvmCross.Commands;
using MvvmCross.ViewModels;
using ProxySuper.Core.Models;
using ProxySuper.Core.Models.Hosts;
using ProxySuper.Core.Models.Projects;
using ProxySuper.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySuper.Core.ViewModels
{
    public class MTProxyGoInstallViewModel : MvxViewModel<Record>
    {
        Host _host;

        MTProxyGoSettings _settings;

        MTProxyGoService _mtproxyService;

        public override void Prepare(Record parameter)
        {
            _host = parameter.Host;
            _settings = parameter.MTProxyGoSettings;
        }

        public override Task Initialize()
        {
            _mtproxyService = new MTProxyGoService(_host, _settings);
            _mtproxyService.Progress.StepUpdate = () => RaisePropertyChanged("Progress");
            _mtproxyService.Progress.LogsUpdate = () => RaisePropertyChanged("Logs");
            _mtproxyService.Connect();
            return base.Initialize();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            _mtproxyService.Disconnect();
            this.SaveInstallLog();
            base.ViewDestroy(viewFinishing);
        }

        public ProjectProgress Progress
        {
            get => _mtproxyService.Progress;
        }

        public string Logs
        {
            get => _mtproxyService.Progress.Logs;
        }


        #region Command

        public IMvxCommand InstallCommand => new MvxCommand(_mtproxyService.Install);

        public IMvxCommand UpdateSettingsCommand => new MvxCommand(_mtproxyService.UpdateSettings);

        public IMvxCommand UninstallCommand => new MvxCommand(_mtproxyService.Uninstall);

        #endregion


        private void SaveInstallLog()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            var fileName = System.IO.Path.Combine("Logs", DateTime.Now.ToString("yyyy-MM-dd hh-mm") + ".mtproxy-go.txt");
            File.WriteAllText(fileName, Logs);
        }
    }
}