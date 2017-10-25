﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace NBi.Core.ResultSet.Comparer
{
    class TextComparer : BaseComparer
    {

        protected override ComparerResult CompareObjects(object x, object y)
        {
            var rxText = x.ToString();
            var ryText = y.ToString();

            //Compare decimals (with tolerance)
            if (IsEqual(rxText, ryText))
                return ComparerResult.Equality;

            return new ComparerResult(string.IsNullOrEmpty(rxText) ? "(empty)" : rxText);
        }

        protected override ComparerResult CompareObjects(object x, object y, Tolerance tolerance)
        {
            if (!(tolerance is TextTolerance))
                throw new ArgumentException("Tolerance must be of type 'TextTolerance'");

            return CompareObjects(x, y, (TextTolerance)tolerance);
        }

        protected ComparerResult CompareObjects(object x, object y, TextTolerance tolerance)
        {
            return CompareStrings(x as string, y as string, tolerance);
        }

        protected ComparerResult CompareStrings(string x, string y, TextTolerance tolerance)
        {
            var distance = tolerance.Implementation.Invoke(x, y);
            
            if (tolerance.Predicate.Invoke(distance,tolerance.Value))
                return ComparerResult.Equality;
            else
                return new ComparerResult(distance.ToString());
        }

        protected override ComparerResult CompareObjects(object x, object y, Rounding rounding)
        {
            throw new NotImplementedException("You cannot compare with a text comparer and a rounding.");
        }

        protected bool IsEqual(string x, string y)
        {
            //quick check
            if (x == y)
                return true;

            if (x == "(empty)" && string.IsNullOrEmpty(y))
                return true;

            if (y == "(empty)" && string.IsNullOrEmpty(x))
                return true;

            if (x == "(blank)" && string.IsNullOrWhiteSpace(y))
                return true;

            if (y == "(blank)" && string.IsNullOrWhiteSpace(x))
                return true;

            return false;
        }


        protected override bool IsValidObject(object x)
        {
            return true;
        }
    }
}
