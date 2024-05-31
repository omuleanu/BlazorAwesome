namespace Omu.BlazorAwesome.Core
{
    /// <summary>
    /// Default awesome settings
    /// </summary>
    public static class AweSettings
    {
        /// <summary>
        /// use StringComparison.InvariantCultureIgnoreCase for the grid string filter,
        /// when using Entity Framework this should be set false, to avoid LINQ to SQL translation errors;
        /// default = false;
        /// </summary>
        public static bool StringFilterIgnoreCase { get; set; }
    }
}