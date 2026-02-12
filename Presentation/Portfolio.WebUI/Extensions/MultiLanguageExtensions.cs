using System;
using System.Globalization;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.WebUI.Extensions;

public static class MultiLanguageExtensions
{
    public static string GetCurrent(this MultiLanguageString multiString)
    {
        if (multiString == null) return string.Empty;

        // Getting current culture
        var currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        return currentCulture switch
        {
            "tr" => multiString.Tr ?? multiString.En ?? multiString.De ?? "",
            "en" => multiString.En ?? multiString.Tr ?? multiString.De ?? "",
            "de" => multiString.De ?? multiString.En ?? multiString.Tr ?? "",
            _ => multiString.En ?? multiString.Tr ?? "" // Default English
        };
    }
}
