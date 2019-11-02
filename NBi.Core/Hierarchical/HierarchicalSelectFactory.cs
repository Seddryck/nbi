﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Hierarchical
{
    public class HierarchicalSelectFactory
    {
        public AbstractSelect Instantiate(string path, string attribute, bool isEvaluate)
        {
            if (isEvaluate)
                return new EvaluateSelect(path);
            if (string.IsNullOrEmpty(attribute))
                return new ElementSelect(path);
            else
                return new AttributeSelect(path, attribute);
        }
    }
}
