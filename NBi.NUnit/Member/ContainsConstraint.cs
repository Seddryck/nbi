﻿using System;
using System.Collections;
using System.Collections.Generic;
using NBi.Core;
using NBi.Core.Analysis.Discovery;
using NBi.Core.Analysis.Member;
using NBi.Core.Analysis.Metadata;
using NUnitCtr = NUnit.Framework.Constraints;

namespace NBi.NUnit.Member
{
    public class ContainsConstraint : NUnitCtr.CollectionContainsConstraint
    {
        protected IEnumerable<string> expectedCaptions;
        protected IComparer comparer;
        protected MembersDiscoveryCommand command;
        protected MembersAdomdEngine memberEngine;

        /// <summary>
        /// Engine dedicated to MetadataExtractor acquisition
        /// </summary>
        protected internal MembersAdomdEngine MemberEngine
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                memberEngine = value;
            }
        }

        protected MembersAdomdEngine GetEngine()
        {
            if (memberEngine == null)
                memberEngine = new MembersAdomdEngine();
            return memberEngine;
        }

        /// <summary>
        /// Construct a CollectionContainsConstraint specific for Members
        /// </summary>
        /// <param name="expected"></param>
        public ContainsConstraint(string expected)
            : base(StringComparerHelper.Build(expected))
        {
            var list = new List<string>();
            list.Add(expected);
            expectedCaptions = list;
            comparer = new NBi.Core.Analysis.Member.Member.ComparerByCaption(true);
        }

        /// <summary>
        /// Construct a CollectionContainsConstraint specific for Members
        /// </summary>
        /// <param name="expected"></param>
        public ContainsConstraint(IEnumerable<string> expected)
            : base(expected)
        {
            expectedCaptions = expected;
            comparer = new NBi.Core.Analysis.Member.Member.ComparerByCaption(true);
        }
        
        #region Modifiers
        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public new ContainsConstraint IgnoreCase
        {
            get
            {
                comparer = new NBi.Core.Analysis.Member.Member.ComparerByCaption(false);
                return this;
            }
        }
        #endregion

        public override bool Matches(object actual)
        {
            if (actual is MembersDiscoveryCommand)
                return Process((MembersDiscoveryCommand)actual);
            else
            {
                base.Using(comparer);
                var res = base.Matches(actual);
                return res;
            }
        }

        protected bool doMatch(IEnumerable<IField> actual)
        {
            return base.Using(comparer).Matches(actual);
        }

        protected bool Process(MembersDiscoveryCommand actual)
        {
            command = actual;
            var extr = GetEngine();
            MemberResult result = extr.GetMembers(command);
            return this.Matches(result);
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public override void WriteDescriptionTo(NUnitCtr.MessageWriter writer)
        {
            if (command != null)
            {
                writer.WritePredicate(string.Format("On perspective \"{0}\", a {1} of \"{2}\" containing a member with caption"
                                                            , command.Perspective
                                                            , GetFunctionLabel(command.Function)
                                                            , command.Path));
                writer.WriteExpectedValue(expectedCaptions);
            }
            else
                base.WriteDescriptionTo(writer);
        }

        protected string GetFunctionLabel(string function)
        {
            switch (function.ToLower())
            {
                case "children":
                    return "child";
                case "members":
                    return "member";
                default:
                    return "?";
            }
        }

       
    }
}
