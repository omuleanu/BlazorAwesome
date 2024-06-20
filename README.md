## Blazor Awesome Components

[![buildtest](https://github.com/omuleanu/BlazorAwesome/actions/workflows/buildtest.yml/badge.svg)](https://github.com/omuleanu/BlazorAwesome/actions/workflows/buildtest.yml) [![NuGet](http://img.shields.io/nuget/v/Omu.BlazorAwesome.svg?label=NuGet)](https://www.nuget.org/packages/Omu.BlazorAwesome/)

Live demo: [blazor.aspnetawesome.com](https://blazor.aspnetawesome.com)

Installation: [aspnetawesome.com/learn/blazor/Installation](https://www.aspnetawesome.com/learn/blazor/Installation)

Demos downloads: [Blazor Server](https://www.aspnetawesome.com/Download/BlazorAwesomeDemo) [Blazor Wasm](https://www.aspnetawesome.com/Download/BlazorWasmAweDemo)


### Getting started
_(Installation and DropdownList tutorial)_
1) Install the library into your blazor project as shown [here](https://www.aspnetawesome.com/learn/blazor/Installation)
2) In your blazor page (Home.razor for example) add this code:
``` html
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
``` csharp
private OPopup popup1;
```
2) In the markup add this:
``` html
<br />
<OPopup @ref="popup1" Opt="@(new(){ Modal = true, Title = "My Modal Popup", OutClickClose = true })">
    <div style="min-width: 30vw; padding: 1em;">
        Modal popup content
    </div>
</OPopup>
<OButton OnClick="() => popup1.Open()">Open popup</OButton>
```
3) Hit `Ctrl+F5` and try clicking the `Open popup` button.

### Grid with column filter
1) Add this to your markup:
``` html
<br />
<br />
<OGrid Opt="gopt" />
```
2) Add this to you `@code` section
``` csharp
private GridOpt<Lunch> gopt = new();

protected override void OnInitialized()
{
    initGrid();
}

private void initGrid()
{
    gopt.ContentHeight = 250;

    gopt.KeyProp = m => m.Id;
    
    gopt.GetQuery = () => new Lunch[]
    {
        new Lunch { Id = 1, Name = "apple"},
        new Lunch { Id = 2, Name = "banana"},
        new Lunch { Id = 3, Name = "cherry"},
    }.AsQueryable();

    gopt.FilterRow = true;

    gopt.Column(new()
        {
            For = m => m.Id,
            Width = 100
        });

    gopt.Column(new()
        {
            For = m => m.Name
        })
    .StringFilter(gopt, this);
}

class Lunch
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```
3) Hit `Ctrl + F5`, and try the grid, you can sort, group, filter by the Name Column, and pick your columns from the bottom corner menu.

### Multiselect, Multicheck and Combobox
We can reuse the `opt` options object from the 1st tutorial "Getting started", to demonstrate other editors.
After the DropdownList paste this code:
``` html
<br />
<br />
<OMultiselect TKey="int" Opt="opt" />
<OMulticheck TKey="int" Opt="opt" />
<OCombobox Opt="opt" />
```
You can use `@bind-Value="Prop1"` instead of specifying the `TKey`, as shown for the DropdownList, in that case `Prop1` could be of type `IEnumerable<int>`. The combobox `bind-Value` property type should be `object` because its value can be either string or the key type.
