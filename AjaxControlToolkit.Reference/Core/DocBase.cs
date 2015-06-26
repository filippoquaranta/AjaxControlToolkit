﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace AjaxControlToolkit.Reference.Core {

    public abstract class DocBase {

        public DocBase(string fullName) {
            var nameMatch = Regex.Match(fullName, @"(?<namespace>.*)\.(?<name>[^.]+)$");
            if(!nameMatch.Success)
                throw new ArgumentException("Unable to split fullName", "fullName");

            Name = nameMatch.Groups["name"].Value;
            Namespace = nameMatch.Groups["namespace"].Value;
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }

        public abstract DocBase Fill(IEnumerable<XElement> values);
    }

}