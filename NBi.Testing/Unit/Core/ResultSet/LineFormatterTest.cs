﻿#region Using directives
using System;
using System.Data;
using NBi.Core.ResultSet;
using NBi.Core.ResultSet.Comparer;
using NUnit.Framework;
#endregion

namespace NBi.Testing.Unit.Core.ResultSet
{
    [TestFixture]
    public class LineFormatterTest
    {
        [Test]
        public void GetText_ShortColumn_NoException()
        {
            // Design Dummy Column
            DataColumn colKey = new DataColumn("BusinessKey");
            colKey.ExtendedProperties.Add("NBi::Role", ColumnRole.Key);
            colKey.ExtendedProperties.Add("NBi::Type", ColumnType.Text);

            // Design Dummy Column
            DataColumn colValue = new DataColumn("NumValue");
            colValue.ExtendedProperties.Add("NBi::Role", ColumnRole.Value);
            colValue.ExtendedProperties.Add("NBi::Type", ColumnType.Numeric);
            colValue.ExtendedProperties.Add("NBi::Tolerance", new NumericAbsoluteTolerance(new decimal(0.001)));

            // Design dummy table
            DataTable table = new DataTable();
            table.Columns.Add(colKey);
            table.Columns.Add(colValue);

            var row = table.NewRow();
            row[0]  = "Alpha";
            row[1] = 77.005;
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Beta";
            row[1] = 103.5;
            table.Rows.Add(row);
            
            
            ICellFormatter cf = LineFormatter.BuildHeader(table, 0);

            // This must not throw an exception when the header is bigger that requested size
            cf.GetText(4);
            Assert.Pass();
        }

        
    }
}
