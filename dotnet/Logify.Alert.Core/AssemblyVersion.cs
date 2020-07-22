using System;

namespace DevExpress.Logify.Core.Internal {
    public static class AssemblyInfo {
        public const string AssemblyCopyright = "Copyright (c) 2000-2018 Developer Express Inc.";
        public const string AssemblyCompany = "Developer Express Inc.";
        public const string AssemblyProduct = "DevExpress Logify Alert";
        public const string AssemblyTrademark = "DevExpress Logify Alert";
        public const string AssemblyDescriptionCommon = "Logify Alert is a cloud service that catches unhandled exceptions in your applications and delivers crash reports.\r\n\r\n" +
"The service provides two approaches to catching exceptions.\r\n" +
"1) Automatic listening to unhandled exceptions, and sending reports for each exception that has been raised.\r\n" +
"2) Catching exceptions manually, and sending reports based on the required exceptions only.\r\n\r\n";

        public const string VersionShort = "1.0";

#if SILVERLIGHT
    public const string Version = VersionShort + ".0.5"; //SL
#elif NETFX_CORE
    public const string Version = VersionShort + ".0.3";
#else
        public const string Version = VersionShort + ".54";
#endif


        public const string FileVersion = Version;
        public const string SatelliteContractVersion = VersionShort + ".0";
    }
}
