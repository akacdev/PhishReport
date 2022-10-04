using System;

namespace PhishReport
{
    /// <summary>
    /// A custom Phish.Report exception for advanced catching.
    /// </summary>
    public class PhishReportException : Exception
    {
        public PhishReportException(string message) : base(message) { }
    }
}