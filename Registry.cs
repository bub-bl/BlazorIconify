using Blazored.LocalStorage;

namespace Iconify;

internal sealed class Registry(ILocalStorageService LocalStorage)
{
    private const string CachedIconsKey = "cached-icons";

    private List<IconMetadata> _icons = [];

    public async Task AddIcon(IconMetadata metadata)
    {
        if (IsRegistered(metadata.Name)) return;

        _icons.Add(metadata);
        await LocalStorage.SetItemAsync(CachedIconsKey, _icons);
    }

    public async Task Clear()
    {
        _icons.Clear();
        await LocalStorage.RemoveItemAsync(CachedIconsKey);
    }

    private async Task<List<IconMetadata>> GetCachedIcons() =>
        _icons = await LocalStorage.GetItemAsync<List<IconMetadata>>(CachedIconsKey) ?? [];

    private bool IsRegistered(string icon) =>
        _icons.Exists(x => x.Name == icon);
}