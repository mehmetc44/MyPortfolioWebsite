using System.Globalization;

namespace Portfolio.Domain.ValueObjects
{
    public class MultiLanguageString
    {        
        public string Tr { get; set; } = string.Empty;
        public string? En { get; set; }
        public string? De { get; set; }

        public string Current 
        {
            get
            {
                var currentCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                return currentCode switch
                {
                    "tr" => Tr,
                    "en" => En ?? Tr,
                    "de" => De ?? En ?? Tr,
                    _ => Tr
                };
            }
        }
        public static implicit operator string(MultiLanguageString mls) => mls.Current;
    }
}
