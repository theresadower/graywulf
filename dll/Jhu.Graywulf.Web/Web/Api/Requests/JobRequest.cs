﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract]
    public class JobRequest
    {
        [DataMember(Name = "queryJob", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public QueryJob QueryJob { get; set; }

        [DataMember(Name = "exportJob", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public ExportJob ExportJob { get; set; }

        [DataMember(Name = "importJob", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public ImportJob ImportJob { get; set; }

        public JobRequest()
        {
        }

        public Job GetValue()
        {
            if (QueryJob != null)
            {
                return QueryJob;
            }
            else if (ExportJob != null)
            {
                return ExportJob;
            }
            else if (ImportJob != null)
            {
                return ImportJob;
            }
            else
            {
                throw new ArgumentNullException();  // TODO
            }
        }
    }
}
