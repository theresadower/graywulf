﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="tableList")]
    public class TableList
    {
        [DataMember(Name = "tables")]
        public Table[] Tables { get; set; }

        public TableList()
        {
        }

        public TableList(IEnumerable<Jhu.Graywulf.Schema.Table> tables)
        {
            this.Tables = tables.Select(t => new Table(t)).ToArray();
        }
    }
}
