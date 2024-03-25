using Blazored.LocalStorage;

namespace Iconify;

public sealed class Registry(ILocalStorageService LocalStorage)
{
    private const string CachedIconsKey = "cached-icons";

    private List<IconMetadata> _icons = [];

    public async Task AddIcon(IconMetadata metadata)
    {
        if (IsRegistered(metadata.Name)) return;

        _icons.Add(metadata);
        await LocalStorage.SetItemAsync(CachedIconsKey, _icons);
    }

    public async Task<IconMetadata?> GetIcon(string icon)
    {
        var icons = await GetCachedIcons();
        return icons.FirstOrDefault(x => x.Name == icon);
    }

    public async Task Clear()
    {
        _icons.Clear();
        await LocalStorage.RemoveItemAsync(CachedIconsKey);
    }

    private async Task<List<IconMetadata>> GetCachedIcons()
    {
        if (_icons.Count > 0) return _icons;
        return _icons = await LocalStorage.GetItemAsync<List<IconMetadata>>(CachedIconsKey) ?? [];
    }

    private bool IsRegistered(string icon) =>
        _icons.Exists(x => x.Name == icon);
}