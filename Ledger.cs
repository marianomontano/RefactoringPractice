using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public enum Currency
{
    USD = '$',
    EURO = '€'
}

public static class LocaleFactory
{
    public static ILocaleManager GetUsLocale(string currency)
    {
        if (currency.ToUpper() == "USD")
            return new UsLocaleManager(Currency.USD, "MM/dd/yyyy");
        else if (currency.ToUpper() == "EUR")
            return new UsLocaleManager(Currency.EURO, "dd/MM/yyyy");
        else
            throw new ArgumentException("Invalid currency");
    }

    public static ILocaleManager GetDutchLocale(string currency)
    {
        if (currency.ToUpper() == "USD")
            return new DutchLocaleManager(Currency.USD, "dd/MM/yyyy");
        else if (currency.ToUpper() == "EUR")
            return new DutchLocaleManager(Currency.EURO, "dd/MM/yyyy");
        else
            throw new ArgumentException("Invalid currency");
    }
}

public interface ILocaleManager
{
    CultureInfo CultureInfo { get; }
    string Header { get; }
}

public class UsLocaleManager : ILocaleManager
{
    public UsLocaleManager(Currency currency, string datePattern)
    {
        CultureInfo = new CultureInfo("en-US");
        CultureInfo.NumberFormat.CurrencySymbol = ((char)currency).ToString();
        CultureInfo.NumberFormat.CurrencyNegativePattern = 0;
        CultureInfo.DateTimeFormat.ShortDatePattern = datePattern;
        Header = "Date       | Description               | Change       ";
    }
    public CultureInfo CultureInfo { get; }
    public string Header { get; }
}

public class DutchLocaleManager : ILocaleManager
{
    public DutchLocaleManager(Currency currency, string datePattern)
    {
        CultureInfo = new CultureInfo("nl-NL");
        CultureInfo.NumberFormat.CurrencySymbol = ((char)currency).ToString();
        CultureInfo.NumberFormat.CurrencyNegativePattern = 12;
        CultureInfo.DateTimeFormat.ShortDatePattern = datePattern;
        Header = "Datum      | Omschrijving              | Verandering  ";
    }
    public CultureInfo CultureInfo { get; }
    public string Header { get; }
}

public class LedgerEntry
{
    public LedgerEntry(DateTime date, string description, decimal change)
    {
        Date = date;
        Description = description;
        Change = change;
    }

    public DateTime Date { get; }
    public string Description { get; }
    public decimal Change { get; }
}

public static class Ledger
{
    public static LedgerEntry CreateEntry(string date, string description, int change)
    {
        return new LedgerEntry(DateTime.Parse(date, CultureInfo.InvariantCulture), description, change / 100.0m);
    }

    private static ILocaleManager CreateLocale(string currency, string locale)
    {
        if (locale == "en-US")
            return LocaleFactory.GetUsLocale(currency);
        else if (locale == "nl-NL")
            return LocaleFactory.GetDutchLocale(currency);
        else
            throw new ArgumentException("Invalid locale");
    }

    //private static CultureInfo CreateCulture(string cur, string loc)
    //{
    //     #region OLDCODE
    //     string curSymb = null;
    //     int curNeg = 0;
    //     string datPat = null;

    //     if (cur != "USD" && cur != "EUR")
    //     {
    //         throw new ArgumentException("Invalid currency");
    //     }
    //     else
    //     {
    //         if (loc != "nl-NL" && loc != "en-US")
    //         {
    //             throw new ArgumentException("Invalid currency");
    //         }

    //         if (cur == "USD")
    //         {
    //             if (loc == "en-US")
    //             {
    //                 curSymb = "$";
    //                 datPat = "MM/dd/yyyy";
    //             }
    //             else if (loc == "nl-NL")
    //             {
    //                 curSymb = "$";
    //                 curNeg = 12;
    //                 datPat = "dd/MM/yyyy";
    //             }
    //         }

    //         if (cur == "EUR")
    //         {
    //             if (loc == "en-US")
    //             {
    //                 curSymb = "€";
    //                 datPat = "MM/dd/yyyy";
    //             }
    //             else if (loc == "nl-NL")
    //             {
    //                 curSymb = "€";
    //                 curNeg = 12;
    //                 datPat = "dd/MM/yyyy";
    //             }
    //         }
    //     }

    //     var culture = new CultureInfo(loc);
    //     culture.NumberFormat.CurrencySymbol = curSymb;
    //     culture.NumberFormat.CurrencyNegativePattern = curNeg;
    //     culture.DateTimeFormat.ShortDatePattern = datPat;
    //     return culture;
    //     #endregion
    // }

    //private static string PrintHead(ILocaleManager localeManager)
    //{
    //    #region OLDCODE
    //    // if (loc == "en-US")
    //    //{
    //    //    return "Date       | Description               | Change       ";
    //    //}

    //    //else
    //    //{
    //    //    if (loc == "nl-NL")
    //    //    {
    //    //        return "Datum      | Omschrijving              | Verandering  ";
    //    //    }
    //    //    else
    //    //    {
    //    //        throw new ArgumentException("Invalid locale");
    //    //    }
    //    //}
    //    #endregion

    //    return localeManager.Header;
    //}

    private static string Date(IFormatProvider culture, DateTime date) => date.ToString("d", culture);

    private static string Description(string description)
    {
        if (description.Length > 25)
        {
            #region OLDCODE
            //var trunc = description.Substring(0, 22);
            //trunc += "...";
            //return trunc;
            #endregion

            return description.Substring(0, 22) + "...";
        }

        return description;
    }

    private static string Change(IFormatProvider culture, decimal change)
    {
        #region OLDCODE
        //return change < 0.0m ? change.ToString("C", culture) : change.ToString("C", culture) + " ";
        #endregion

        if (change < 0.0m)
            return change.ToString("C", culture);
        else
            return change.ToString("C", culture) + " ";

    }

    private static string PrintEntry(IFormatProvider culture, LedgerEntry entry)
    {
        var formatted = "";
        var date = Date(culture, entry.Date);
        var description = string.Format("{0,-25}", Description(entry.Description));
        var change = string.Format("{0,13}", Change(culture, entry.Change));

        #region OLDCODE
        //formatted += date;
        //formatted += " | ";
        //formatted += string.Format("{0,-25}", description);
        //formatted += " | ";
        //formatted += string.Format("{0,13}", change);
        #endregion

        formatted += $"{date} | {description} | {change}";

        return formatted;
    }


    private static IEnumerable<LedgerEntry> SortEntries(LedgerEntry[] entries)
    {
        var negatives = entries.Where(e => e.Change < 0).OrderBy(x => x.Date + "@" + x.Description + "@" + x.Change);
        var positives = entries.Where(e => e.Change >= 0).OrderBy(x => x.Date + "@" + x.Description + "@" + x.Change);

        var result = new List<LedgerEntry>();
        result.AddRange(negatives);
        result.AddRange(positives);

        return result;
    }

    public static string Format(string currency, string locale, LedgerEntry[] entries)
    {
        var formatted = "";
        var localeManager = CreateLocale(currency, locale);
        var culture = localeManager.CultureInfo;

        formatted += localeManager.Header;

        if (entries.Length > 0)
        {
            var entriesForOutput = SortEntries(entries);

            for (var i = 0; i < entriesForOutput.Count(); i++)
            {
                formatted += "\n" + PrintEntry(culture, entriesForOutput.Skip(i).First());
            }
        }

        return formatted;
    }
}
