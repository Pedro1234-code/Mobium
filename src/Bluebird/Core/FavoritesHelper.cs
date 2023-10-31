﻿using Bluebird.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bluebird.Core;

public class FavoritesHelper
{
    public static async void LoadFavoritesOnStartup()
    {
        SettingsViewModel.SettingsVM.FavoritesList = await GetFavoritesListAsync();
    }

    public static async void CreateFirstFavorite(string title, string url)
    {
        // Generate json
        string json = "[{\"title\":\"" + title + "\"," + "\"url\":\"" + url + "\"}]";
        // create json file
        var file = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
        // write json to json file
        await FileIO.WriteTextAsync(file, json);
    }

    public static async void AddFavorite(string title, string url)
    {
        var fileData = await localFolder.TryGetItemAsync("Favorites.json");
        if (fileData == null) CreateFirstFavorite(title, url);
        else
        {
            // new historyitem
            FavoriteItems newFavoriteItem = new()
            {
                Title = title,
                Url = url
            };
            // add item to favorites list
            SettingsViewModel.SettingsVM.FavoritesList.Insert(0, newFavoriteItem);
            SaveListChangesToDisk();
        }
    }

    public static void RemoveFavorite(FavoriteItems item)
    {
        SettingsViewModel.SettingsVM.FavoritesList.Remove(item);
        SaveListChangesToDisk();
    }

    public static async Task<ObservableCollection<FavoriteItems>> GetFavoritesListAsync()
    {
        var fileData = await localFolder.TryGetItemAsync("Favorites.json");
        if (fileData == null) return null;
        else
        {
            string filecontent = await FileIO.ReadTextAsync((IStorageFile)fileData);
            return JsonConvert.DeserializeObject<ObservableCollection<FavoriteItems>>(filecontent);
        }
    }

    private static async void SaveListChangesToDisk()
    {
        var fileData = await localFolder.TryGetItemAsync("Favorites.json");
        // Convert list to json
        string newJson = JsonConvert.SerializeObject(SettingsViewModel.SettingsVM.FavoritesList);
        // Write json to json file
        await FileIO.WriteTextAsync((IStorageFile)fileData, newJson);
    }
}
