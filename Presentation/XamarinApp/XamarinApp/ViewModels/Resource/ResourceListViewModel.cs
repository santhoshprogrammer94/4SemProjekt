﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Syncfusion.ListView.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinApp.Application.Common.Interfaces;
using XamarinApp.Domain.Common;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace XamarinApp.ViewModels.Resource
{
    public class ResourceListViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Navigation
        private readonly INavigationService _navigator;
        public string NavigationPath { get; }
        public ListViewLoadedEventHandler GenerateResourcesEvent { get; }
        #endregion
        
        public ObservableCollection<Domain.Entities.Resource> Resources { get; set; }
        public string ErrorMessage { get; private set; }

        private SearchBar _searchBar;

        public ResourceListViewModel(INavigationService navigator, string navigationPath)
        {
            #region Navigation
            _navigator = navigator;
            NavigationPath = navigationPath;
            GenerateResourcesEvent = async (sender, args) =>
            {
                await _navigator.NavigateToModal(new BusyViewModel(_navigator, $"{NavigationPath}/Busy"));
                await GenerateResources();
                await _navigator.PopModal();
            };
            #endregion
            
            Resources = new ObservableCollection<Domain.Entities.Resource>();
        }
        
        private async Task GenerateResources()
        {
            var mobileBffUrl = Xamarin.Forms.Application.Current.Properties["MobileBffUrl"] as string;
            var token = await SecureStorage.GetAsync("jwt_token");
            try
            {
                var result = await mobileBffUrl.AppendPathSegment("Resource").WithOAuthBearerToken(token).GetJsonAsync<List<Domain.Entities.Resource>>();
                if (result.Any())
                {
                    foreach (var resource in result.ToList())
                    {
                        Resources.Add(resource);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        public void OnFilterTextChanged(object sender, TextChangedEventArgs e, SfListView listView)
        {
            _searchBar = (sender as SearchBar);
            if (listView.DataSource == null) return;
            listView.DataSource.Filter = FilterContacts;
            listView.DataSource.RefreshFilter();
        }

        private bool FilterContacts(object obj)
        {
            if (_searchBar?.Text == null)
                return true;

            return obj is Domain.Entities.Resource resource && (resource.Name.ToLower().Contains(_searchBar.Text.ToLower())
                                                                || resource.Description.ToLower().Contains(_searchBar.Text.ToLower()));
        }

        public void ItemTappedEvent(object sender, ItemTappedEventArgs e)
        {
            _navigator.NavigateTo(new ResourceViewModel(_navigator, $"{NavigationPath}/Resource/{((XamarinApp.Domain.Entities.Resource) e.ItemData).Id}", (XamarinApp.Domain.Entities.Resource) e.ItemData));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}