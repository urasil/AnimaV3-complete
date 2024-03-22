namespace dotnetAnima.Core {
    // the item in the language dropdown list, {country_icon + text}
    public class LanguageItem {
        public string icon {
            get;
            set;
        }
        public string text {
            get;
            set;
        }

        public LanguageItem(string icon, string text) {
            this.icon = icon;
            this.text = text;
        }
    }
}
