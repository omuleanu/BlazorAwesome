namespace Omu.BlazorAwesome.Models.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class AweKey
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string ArrowUp = nameof(ArrowUp);

        public const string ArrowLeft = nameof(ArrowLeft);
        public const string ArrowRight = nameof(ArrowRight);
        public const string ArrowDown = nameof(ArrowDown);
        public const string Shift = nameof(Shift);
        public const string Ctrl = nameof(Ctrl);
        public const string Escape = nameof(Escape);
        public const string Backspace = nameof(Backspace);
        public const string Enter = nameof(Enter);
        public const string Tab = nameof(Tab);

        public static readonly string[] NonOpenKeys = new[] { Enter, Escape, Shift, ArrowLeft, ArrowRight, Tab, Ctrl, Backspace };

        //var nonOpenKeys = [keyEnter, keyEsc, keyShift, keyLeft, keyRight, keyTab, keyCtrl, 33, 34, 35, 36]; // keys that won't open the menu
    }
}