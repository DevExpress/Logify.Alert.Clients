using System;
using System.Globalization;

namespace DevExpress.Logify.Core.Internal {
    //TODO: move to platform specific assembly
    public class CultureBaseCollector : IInfoCollector {
        readonly CultureInfo culture;
        readonly string name;

        protected CultureBaseCollector(string name) {
            this.name = name;
        }

        public CultureBaseCollector(CultureInfo culture, string name) {
            this.culture = culture;
            this.name = name;
        }

        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject(Name);
            try {
                logger.WriteValue("name", Culture.Name);
                logger.WriteValue("englishName", Culture.EnglishName);
                logger.WriteValue("displayName", Culture.DisplayName);
#if !NETSTANDARD
                logger.WriteValue("isCultureCustomize", Culture.UseUserOverride);
#endif
                try {
#if !NETSTANDARD
                    if (Culture.UseUserOverride)
#endif
                        SerializeCultureDetails(logger);
                }
                catch {
                }
            }
            finally {
                logger.EndWriteObject(Name);
            }
        }

        void SerializeCultureDetails(ILogger logger) {
            logger.BeginWriteObject("details");
            try {

                this.SerializeNumberFormatInfo(logger);
                this.SerializeDateTimeInfo(logger);
                this.SerializeTextInfo(logger);

                logger.WriteValue("isNeutralCulture", Culture.IsNeutralCulture);
                logger.WriteValue("ISO2", Culture.TwoLetterISOLanguageName);
#if !NETSTANDARD
                logger.WriteValue("ISO1", Culture.ThreeLetterISOLanguageName);
                logger.WriteValue("WinAPI", Culture.ThreeLetterWindowsLanguageName);
#endif
            }
            finally {
                logger.EndWriteObject("details");
            }
        }

        void SerializeTextInfo(ILogger logger) {
            TextInfo textInfo = Culture.TextInfo;
            logger.WriteValue("listSeparator", textInfo.ListSeparator);
            logger.WriteValue("isRightToLeft", textInfo.IsRightToLeft);
#if !NETSTANDARD
            logger.WriteValue("ANSICodePage", textInfo.ANSICodePage);
            logger.WriteValue("EBCDICCodePage", textInfo.EBCDICCodePage);
            logger.WriteValue("LCID", textInfo.LCID);
            logger.WriteValue("MacCodePage", textInfo.MacCodePage);
            logger.WriteValue("OEMCodePage", textInfo.OEMCodePage);
#endif
        }

        void SerializeDateTimeInfo(ILogger logger) {

            DateTimeFormatInfo formatInfo = Culture.DateTimeFormat;

            logger.BeginWriteObject("dateTime");

#if NETSTANDARD
            try {
                DateTime today = DateTime.Today;
                string dateSeparator = today.ToString("%/", culture);
                string timeSeparator = today.ToString("%:", culture);
                logger.WriteValue("dateSeparator", dateSeparator);
                logger.WriteValue("timeSeparator", timeSeparator);
            }
            catch {
            }
#else
            logger.WriteValue("dateSeparator", formatInfo.DateSeparator);
            logger.WriteValue("timeSeparator", formatInfo.TimeSeparator);
#endif
            logger.WriteValue("firstDayOfWeek", formatInfo.FirstDayOfWeek.ToString());
            logger.WriteValue("am", formatInfo.AMDesignator);
            logger.WriteValue("pm", formatInfo.PMDesignator);
            logger.WriteValue("abbreviatedDayNames", formatInfo.AbbreviatedDayNames);
            logger.WriteValue("abbreviatedMonthNames", formatInfo.AbbreviatedMonthNames);
            logger.WriteValue("dayNames", formatInfo.DayNames);
            logger.WriteValue("monthNames", formatInfo.MonthNames);
            logger.WriteValue("shortestDayNames", formatInfo.ShortestDayNames);

            logger.BeginWriteObject("pattern");
            logger.WriteValue("shortTime", formatInfo.ShortTimePattern);
            logger.WriteValue("longTime", formatInfo.LongTimePattern);
            logger.WriteValue("shortDate", formatInfo.ShortDatePattern);
            logger.WriteValue("longDate", formatInfo.LongDatePattern);
            logger.WriteValue("monthDay", formatInfo.MonthDayPattern);
            logger.WriteValue("yearMonth", formatInfo.YearMonthPattern);
            logger.WriteValue("fullDateTime", formatInfo.FullDateTimePattern);
            logger.WriteValue("sortableDateTime", formatInfo.SortableDateTimePattern);
            logger.WriteValue("universalSortableDateTime", formatInfo.UniversalSortableDateTimePattern);
            logger.WriteValue("RFC1123", formatInfo.RFC1123Pattern);
            logger.EndWriteObject("pattern");

            logger.EndWriteObject("dateTime");
        }

        void SerializeNumberFormatInfo(ILogger logger) {

            NumberFormatInfo formatInfo = Culture.NumberFormat;

            logger.BeginWriteObject("numberFormat");

            logger.BeginWriteObject("currency");
            logger.WriteValue("decimalDigits", formatInfo.CurrencyDecimalDigits);
            logger.WriteValue("decimalSeparator", formatInfo.CurrencyDecimalSeparator);
            logger.WriteValue("groupSeparator", formatInfo.CurrencyGroupSeparator);
            logger.WriteValue("groupSize", formatInfo.CurrencyGroupSizes);
            logger.WriteValue("symbol", formatInfo.CurrencySymbol);
            logger.WriteValue("negativePatternID", formatInfo.CurrencyNegativePattern);
            logger.WriteValue("positivePatternID", formatInfo.CurrencyPositivePattern);
            logger.EndWriteObject("currency");
                
            logger.BeginWriteObject("number");
            logger.WriteValue("decimalDigits", formatInfo.NumberDecimalDigits);
            logger.WriteValue("decimalSeparator", formatInfo.NumberDecimalSeparator);
            logger.WriteValue("groupSeparator", formatInfo.NumberGroupSeparator);
            logger.WriteValue("groupSize", formatInfo.NumberGroupSizes);
            logger.WriteValue("negativePatternID", formatInfo.NumberNegativePattern);
            logger.EndWriteObject("number");

            logger.BeginWriteObject("percent");
            logger.WriteValue("decimalDigits", formatInfo.PercentDecimalDigits);
            logger.WriteValue("decimalSeparator", formatInfo.PercentDecimalSeparator);
            logger.WriteValue("groupSeparator", formatInfo.PercentGroupSeparator);
            logger.WriteValue("groupSize", formatInfo.PercentGroupSizes);
            logger.WriteValue("symbol", formatInfo.PercentSymbol);
            logger.WriteValue("negativePatternID", formatInfo.PercentNegativePattern);
            logger.WriteValue("positivePatternID", formatInfo.PercentPositivePattern);
            logger.EndWriteObject("percent");

            logger.BeginWriteObject("sign");
            logger.WriteValue("positive", formatInfo.PositiveSign);
            logger.WriteValue("negative", formatInfo.NegativeSign);
            logger.EndWriteObject("sign");

            logger.BeginWriteObject("infinity");
            logger.WriteValue("positive", formatInfo.PositiveInfinitySymbol);
            logger.WriteValue("negative", formatInfo.NegativeInfinitySymbol);
            logger.EndWriteObject("infinity");

            logger.WriteValue("perMilleSymbol", formatInfo.PerMilleSymbol);
            logger.WriteValue("NaNSymbol", formatInfo.NaNSymbol);
#if !NETSTANDARD
            logger.WriteValue("digitShapes", formatInfo.DigitSubstitution.ToString());
            logger.WriteValue("nativeDigits", formatInfo.NativeDigits);
#endif

            logger.EndWriteObject("numberFormat");
        }

        public virtual CultureInfo Culture { get { return culture ?? CultureInfo.CurrentCulture; } }
        public virtual string Name { get { return name; } }
    }
    public class CurrentCultureCollector : CultureBaseCollector {
        public CurrentCultureCollector() : base("currentCulture") {
        }
        public override CultureInfo Culture { get { return CultureInfo.CurrentCulture; } }
    }
    public class CurrentUICultureCollector : CultureBaseCollector {
        public CurrentUICultureCollector() : base("currentUICulture") {
        }
        public override CultureInfo Culture { get { return CultureInfo.CurrentUICulture; } }
    }
}