﻿using System.Collections.Generic;
using NBi.Extensibility.FlatFile;

namespace NBi.Core.FlatFile
{
    public class CsvProfile : PocketCsvReader.CsvProfile, IFlatFileProfile
    {
        public static new CsvProfile SemiColumnDoubleQuote { get; private set; } = new CsvProfile(';');

        public CsvProfile(IDictionary<string, object> attributes)
            : base (
            (char)  (attributes.TryGetValue("field-separator", out var value) ? value : ';'),
            (char)  (attributes.TryGetValue("text-qualifier", out var value) ? value : '\"'),
            (string)(attributes.TryGetValue("record-separator", out var value) ? value : "\r\n"),
            (bool)  (attributes.TryGetValue("first-row-header", out var value) ? value : false),
            (bool)  (attributes.TryGetValue("performance-optimized", out var value) ? value : true),
            (int)   (attributes.TryGetValue("buffer-size", out var value) ? value : 4096),
            (string)(attributes.TryGetValue("empty-cell", out var value) ? value : "(empty)"),
            (string)(attributes.TryGetValue("missing-cell", out var value) ? value : "(null)")
        ) {}

        public IDictionary<string, object> Attributes => new Dictionary<string, object>()
                {
                    { "field-separator", base.FieldSeparator },
                    { "text-qualifier", base.TextQualifier },
                    { "record-separator", base.RecordSeparator },
                    { "first-row-header", base.FirstRowHeader },
                    { "performance-optimized", base.PerformanceOptmized },
                    { "missing-cell", base.MissingCell },
                    { "empty-cell", base.EmptyCell },
                };

        private CsvProfile(char fieldSeparator)
            : base(fieldSeparator, '\"', '\\', "\r\n", false, true, 4096, "(empty)", "(null)") { }

        public CsvProfile(bool firstRowHeader)
            : base(firstRowHeader) { }

        public CsvProfile(char fieldSeparator, char textQualifier, string recordSeparator, bool firstRowHeader, bool performanceOptimized, string emptyCell, string missingCell)
            : base(fieldSeparator, textQualifier, recordSeparator, firstRowHeader, performanceOptimized, 4096, emptyCell, missingCell) { }
    }
}
