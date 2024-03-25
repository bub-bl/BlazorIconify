using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Iconify;

public partial class Iconify : ComponentBase
{
    public const string API = "https://api.iconify.design/";

    private string _svg = string.Empty;
    
    [Inject] public HttpClient HttpClient { get; set; }
    [Inject] public ILocalStorageService LocalStorage { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    [Parameter] public string Icon { get; set; } = "fluent-emoji-flat:alarm-clock";

    public string IconUrl => $"{API}{Icon.Replace(':', '/')}.svg?color=white";

    protected override async Task OnParametersSetAsync()
    {
        var prefix = Icon.Split(':')[0];
        var icon = Icon.Split(':')[1];

        if (await IsCached(prefix, icon))
        {
            _svg = await LocalStorage.GetItemAsStringAsync(Icon);
        }
        else
        {
            _svg = await FetchIconAsync(IconUrl);
            await LocalStorage.SetItemAsStringAsync(Icon, _svg);
        }
        
        // Remove the fill attribute, we can't define custom color without it
        _svg = _svg.Replace("fill=\"white\"", "")
            // Adding class to the svg element
            .Replace("xmlns=\"http://www.w3.org/2000/svg\"",
                $"xmlns=\"http://www.w3.org/2000/svg\" class=\"{Attributes.GetValueOrDefault("i-class")}\"");

        if (string.IsNullOrEmpty(_svg))
            throw new Exception($"Failed to fetch icon {this}");
        
        StateHasChanged();
    }

    private async Task<bool> IsCached(string prefix, string icon)
    {
        var path = $"{prefix}:{icon}.svg";
        return await LocalStorage.ContainKeyAsync(path);
    }

    private async Task<string> FetchIconAsync(string url)
    {
        var response = await HttpClient.GetByteArrayAsync(url);
        var iconContents = Encoding.UTF8.GetString(response);

        // this API doesn't actually return a 404 status code :( check the document for '404' itself...
        if (response is { Length: 0 } or null)
            throw new Exception($"Failed to fetch icon {this}");

        return iconContents;
    }
}