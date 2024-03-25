using System.Text;
using Blazored.LocalStorage;
using Iconify.Extensions;
using Microsoft.AspNetCore.Components;

namespace Iconify;

public partial class Iconify : ComponentBase
{
    private const string API = "https://api.iconify.design/";

    private string _previousIcon = string.Empty;
    private string _svg = string.Empty;

    [Inject] public HttpClient HttpClient { get; set; } = null!;
    [Inject] public ILocalStorageService LocalStorage { get; set; }
    [Inject] public Registry Registry { get; set; } = null!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = null!;

    [Parameter] public string Icon { get; set; } = "fluent-emoji-flat:alarm-clock";

    private string IconUrl => $"{API}{Icon.Replace(':', '/')}.svg?color=white";

    protected override async Task OnParametersSetAsync()
    {
        // Only fetch the icon if it has changed
        if (_previousIcon != Icon)
        {
            if (await Registry.IsCached(Icon))
            {
                var metadata = await Registry.GetIcon(Icon);
                if (metadata is null) return;

                _svg = metadata.Content;
                _previousIcon = Icon;
            }
            else
            {
                _svg = await FetchIconAsync(IconUrl);

                if (string.IsNullOrEmpty(_svg))
                {
                    Console.WriteLine($"Failed to fetch icon {(!string.IsNullOrEmpty(Icon) ? Icon : "\"null\"")}");
                    return;
                }

                await Registry.AddIcon(new IconMetadata
                {
                    Name = Icon,
                    Content = _svg,
                    TimeFetched = DateTime.Now
                });
            }

            // TODO - Find a better way to do this
            // Remove the fill attribute, we can't define custom color without it
            _svg = _svg.Replace("fill=\"white\"", "")
                // Adding class to the svg element
                .Replace("xmlns=\"http://www.w3.org/2000/svg\"",
                    $"xmlns=\"http://www.w3.org/2000/svg\" class=\"{Attributes.Get("i-class")}\"");

            if (string.IsNullOrEmpty(_svg))
                Console.WriteLine($"Failed to fetch icon {this}");

            _previousIcon = Icon;
            StateHasChanged();
        }
    }
    
    private async Task<string> FetchIconAsync(string url)
    {
        if (string.IsNullOrEmpty(Icon)) return string.Empty;
        
        var response = await HttpClient.GetByteArrayAsync(url);
        var iconContents = Encoding.UTF8.GetString(response);

        if (iconContents is not "404" && response is not ({ Length: 0 } or null))
            return iconContents;

        iconContents = string.Empty;
        return iconContents;
    }
}