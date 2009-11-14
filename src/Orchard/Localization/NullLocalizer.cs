namespace Orchard.Localization {
    public static class NullLocalizer {
        
        static NullLocalizer () {
            _instance = (format, args) => (args == null || args.Length == 0) ? format : string.Format(format, args);
        }
        
        public static Localizer _instance;

        public static Localizer Instance { get { return _instance; } }
    }
}