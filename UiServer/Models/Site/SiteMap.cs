using System.Collections.Generic;

namespace UiServer.Models.Site
{
    public static class SiteMap
    {
        public static List<SiteMapItem> LinksList = new() {
        new(string.Empty, "Home"){ Title = "Overview"},
        new("Editors", "Editors")
        { Keywords = "input odropdownlist omultiselect oradiolist, ocheckboxlist otoggle onumeric otextbox omultichk datepicker"},

        new("Popup", "Popup") { Keywords = "form open"},
        new("GridInlineEdit", "Grid Inline Editing") { Keywords = "crud"},
        new("GridNestCrud", "Grid Nest Crud"),
        new("GridMasterDetailWithInlineEdit", "Grid Master Detail With Inline Editing"),
        new("GridCrud", "Grid Popup Crud") { Keywords = "create edit delete"},
        new("GridFilterRow", "Grid Filter Row") { Keywords = "search"},
        new("GridGrouping", "Grid Grouping") { Keywords = "aggregates"},
        new("GridSelect", "Grid Selection"),
        new("GridCheckboxes", "Grid Checkboxes") {Keywords = "selection"},
        new("GridHierarchy", "Grid Hierarchy") { Keywords = "details nesting"},
        new("TreeGrid", "Tree Grid") { Keywords = "nodes structure"},
        new("TreeGridInlineEdit", "Tree Grid Inline Editing"),
        new("GridSaveState", "Grid Save State") { Keywords = "persistence"},
        new("RemoteSearch", "Remote Search") { Keywords = "DropdownList Multiselect Multicheck Combobox"},
        new("GridSearch", "Grid Search") { Keywords = "filtering"},
        new("GridReorderRows", "Grid Reorder Rows"),
        new("Cascade", "Cascade") { Keywords = "parent children"},
        new("DragAndDrop", "Drag And Drop"),
        new("GridArrayDataSource", "Grid Array Data"),
        new("GridLocalData", "Grid Local Data"),
        new("About", "About") { Keywords = "help"},

        //::privatecons
        // tests
        //new("AfterChange", "AfterChange"),
        //new("UnboundEditors", "UnboundEditors"),
        //new("CheckValInData", "CheckValInData"),
        //new("gridchangecolumn", "Grid Change Column"),
        //new("InlineEdit2", "InlineEdit2")
        //::privatecone
        };
    }
}