using System.Text;
using System.Xml;
using Blazored.LocalStorage;
using Iconify.Extensions;
using Microsoft.AspNetCore.Components;

namespace Iconify;

public partial class Iconify : ComponentBase
{
    private const string API = "https://api.iconify.design/";
    private const string ErrorIcon = "ic:baseline-do-not-disturb";

    private string _previousIcon = string.Empty;
    private string _svg = string.Empty;
    
    [Inject] public HttpClient HttpClient { get; set; } = null!;
    [Inject] public ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] public Registry Registry { get; set; } = null!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = null!;

    [Parameter] public string Icon { get; set; } = string.Empty;

    private string IconUrl => $"{API}{Icon.Replace(':', '/')}.svg?color=white";

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Icon))
            Icon = ErrorIcon;
        
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

            var svg = TryConvertToXml(_svg);
            if (svg is null) return;
            
            UpdateSvg(svg);
            
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

    private static XmlDocument? TryConvertToXml(string content)
    {
        try
        {
            var document = new XmlDocument();
            document.LoadXml(content);

            return document;
        }
        catch (XmlException ex)
        {
            Console.WriteLine("Failed to parse xml from svg file.");
        }

        return null;
    }
    
    private void UpdateSvg(XmlDocument document)
    {
        var svg = document.DocumentElement;
        
        if (svg is null)
        {
            Console.WriteLine("Failed to find svg element.");
            return;
        }
        
        svg.SetAttribute("class", Attributes.Get("i-class"));
        svg.RemoveAttribute("width");
        svg.RemoveAttribute("height");

        foreach (XmlElement child in svg.ChildNodes)
        {
            if (child.Name.ToLower() is not "path") continue;
            child.RemoveAttribute("fill");
        }
        
        _svg = svg.InnerXml;
    }
}