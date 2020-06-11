﻿#region LICENSE

// Copyright 2014 LeagueSharp.Loader
// InstallerWindow.xaml.cs is part of LeagueSharp.Loader.
// 
// LeagueSharp.Loader is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Loader is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Loader. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace LeagueSharp.Loader.Views
{
    #region

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Forms;
    using System.Windows.Input;
    using LeagueSharp.Loader.Class;
    using LeagueSharp.Loader.Data;
    using MahApps.Metro.Controls.Dialogs;
    using Application = System.Windows.Application;
    using TextBox = System.Windows.Controls.TextBox;

    #endregion

    public partial class InstallerWindow : INotifyPropertyChanged
    {
        private bool _ableToList = true;
        private List<LeagueSharpAssembly> _foundAssemblies = new List<LeagueSharpAssembly>();
        private ProgressDialogController controller;

        public InstallerWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public List<LeagueSharpAssembly> FoundAssemblies
        {
            get { return _foundAssemblies; }
            set
            {
                _foundAssemblies = value;
                OnPropertyChanged("FoundAssemblies");
            }
        }

        public bool AbleToList
        {
            get { return _ableToList; }
            set
            {
                _ableToList = value;
                OnPropertyChanged("AbleToList");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void ShowProgress(string location, bool isSvn, string autoInstallName = null)
        {
            controller =
                await
                    this.ShowProgressAsync(
                        Utility.GetMultiLanguageText("Updating"), Utility.GetMultiLanguageText("DownloadingData"));
            controller.SetIndeterminate();
            controller.SetCancelable(true);
            ListAssemblies(location, isSvn, autoInstallName);
        }

        public void ListAssemblies(string location, bool isSvn, string autoInstallName = null)
        {
            AbleToList = false;
            var bgWorker = new BackgroundWorker();

            if (!isSvn)
            {
                bgWorker.DoWork += delegate { FoundAssemblies = LeagueSharpAssemblies.GetAssemblies(location); };
            }
            else
            {
                bgWorker.DoWork += delegate
                {
                    var updatedDir = GitUpdater.Update(location, Logs.MainLog, Directories.RepositoryDir);
                    FoundAssemblies = LeagueSharpAssemblies.GetAssemblies(updatedDir, location);
                    foreach (var assembly in FoundAssemblies)
                    {
                        if (autoInstallName != null && assembly.Name.ToLower() == autoInstallName.ToLower())
                        {
                            assembly.InstallChecked = true;
                        }
                    }
                };
            }

            bgWorker.RunWorkerCompleted += delegate
            {
                if (controller != null)
                {
                    controller.CloseAsync();
                    controller = null;
                }

                AbleToList = true;
                Application.Current.Dispatcher.Invoke(() => installTabControl.SelectedIndex++);
                if (autoInstallName != null)
                {
                    InstallSelected();
                }
            };

            bgWorker.RunWorkerAsync();
        }

        public void InstallSelected()
        {
            var amount = FoundAssemblies.Count(a => a.InstallChecked);

            foreach (var assembly in FoundAssemblies)
            {
                if (assembly.InstallChecked)
                {
                    if (assembly.Compile())
                    {
                        if (
                            Config.Instance.SelectedProfile.InstalledAssemblies.All(
                                a => a.Name != assembly.Name || a.SvnUrl != assembly.SvnUrl))
                        {
                            Config.Instance.SelectedProfile.InstalledAssemblies.Add(assembly);
                        }
                        amount--;
                    }
                }
            }

            if (amount == 0)
            {
                AfterInstallMessage(Utility.GetMultiLanguageText("SuccessfullyInstalled"), true);
            }
            else
            {
                AfterInstallMessage(Utility.GetMultiLanguageText("ErrorInstalling"));
            }
        }

        private void Step1_Click(object sender, RoutedEventArgs e)
        {
            if (InstalledRadioButton.IsChecked == true)
            {
                FoundAssemblies.Clear();
                foreach (var profile in Config.Instance.Profiles)
                {
                    foreach (var assembly in profile.InstalledAssemblies)
                    {
                        FoundAssemblies.Add(assembly.Copy());
                    }
                }
                FoundAssemblies = FoundAssemblies.Distinct().ToList();

                installTabControl.SelectedIndex++;
            }
            else
            {
                ShowProgress(
                    (SvnRadioButton.IsChecked == true) ? SvnComboBox.Text : PathTextBox.Text,
                    (SvnRadioButton.IsChecked == true));
            }
        }

        private void Step2_Click(object sender, RoutedEventArgs e)
        {
            InstallSelected();
        }

        private async void AfterInstallMessage(string msg, bool close = false)
        {
            await this.ShowMessageAsync(Utility.GetMultiLanguageText("Installer"), msg);
            if (close)
            {
                Close();
            }
        }

        private void Step2P_Click(object sender, RoutedEventArgs e)
        {
            installTabControl.SelectedIndex--;
        }

        private void PathTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = (TextBox) sender;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.SelectedText))
            {
                var folderDialog = new FolderSelectDialog();

                folderDialog.Title = "Select project folder";

                if (folderDialog.ShowDialog())
                {
                    textBox.Text = folderDialog.FileName;
                    LocalRadioButton.IsChecked = true;
                }
            }
        }

        private void InstallerWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SvnComboBox.ItemsSource = Config.Instance.KnownRepositories;
        }

        private void SvnComboBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            SvnRadioButton.IsChecked = true;
            LocalRadioButton.IsChecked = !SvnRadioButton.IsChecked;
        }

        private void PathTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            SvnRadioButton.IsChecked = false;
            LocalRadioButton.IsChecked = !SvnRadioButton.IsChecked;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var assembly in FoundAssemblies)
            {
                assembly.InstallChecked = true;
            }
            OnPropertyChanged("FoundAssemblies");
        }

        private void UnselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var assembly in FoundAssemblies)
            {
                assembly.InstallChecked = false;
            }
            OnPropertyChanged("FoundAssemblies");
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = ((TextBox) sender).Text;
            var view = CollectionViewSource.GetDefaultView(FoundAssemblies);
            searchText = searchText.Replace("*", "(.*)");
            view.Filter = obj =>
            {
                try
                {
                    var assembly = obj as LeagueSharpAssembly;
                    var nameMatch = Regex.Match(assembly.Name, searchText, RegexOptions.IgnoreCase);

                    return nameMatch.Success;
                }
                catch (Exception)
                {
                    return true;
                }
            };
        }
    }
}