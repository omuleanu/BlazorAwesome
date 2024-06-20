## Blazor Awesome Components

[![buildtest](https://github.com/omuleanu/BlazorAwesome/actions/workflows/buildtest.yml/badge.svg)](https://github.com/omuleanu/BlazorAwesome/actions/workflows/buildtest.yml) [![NuGet](http://img.shields.io/nuget/v/Omu.BlazorAwesome.svg?label=NuGet)](https://www.nuget.org/packages/Omu.BlazorAwesome/)

Live demo: [blazor.aspnetawesome.com](https://blazor.aspnetawesome.com)

Installation: [aspnetawesome.com/learn/blazor/Installation](https://www.aspnetawesome.com/learn/blazor/Installation)

Demos downloads: [Blazor Server](https://www.aspnetawesome.com/Download/BlazorAwesomeDemo) [Blazor Wasm](https://www.aspnetawesome.com/Download/BlazorWasmAweDemo)


### Getting started

1) Install the library into your blazor project as shown [here](https://www.aspnetawesome.com/learn/blazor/Installation)
2) In your blazor page (Home.razor for example) add this code:
```
<OButton>hi</OButton>

<ODropdownList @bind-Value="input.Dropdown1" Opt="opt" />

<ODropdownList TKey="int" Opt="opt" />

@code {
    private InputModel input = new();

    private DropdownOpt opt = new DropdownOpt
        {
            Data = new KeyContent[]
            {
                new KeyContent(1, "apple"),
                new KeyContent(2, "banana"),
                new KeyContent(3, "cherry"),
            }
        };

    class InputModel
    {
        public int? Dropdown1 { get; set; }
    }
}
```
3) Hit `Ctrl+F5` in Visual Studio and try the **Button** and the 2 **DropdownLists**.

### Open a popup
1) In your `@code` section add this:
```
private OPopup popup1;
```
2) In the markup add this:
```
<br />
<OPopup @ref="popup1" Opt="@(new(){ Modal = true, Title = "My Modal Popup", OutClickClose = true })">
    <div style="min-width: 30vw; padding: 1em;">
        Modal popup content
    </div>
</OPopup>
<OButton OnClick="() => popup1.Open()">Open popup</OButton>
```
3) Hit `Ctrl+F5` and try clicking the `Open popup` button.

