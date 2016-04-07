﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [DbTable]
    class EntityWithGuidKey : Entity
    {
        [DbColumn(Binding = DbColumnBinding.Key)]
        public Guid Guid { get; set; }

        [DbColumn]
        public string Name { get; set; }

        public EntityWithGuidKey()
        {
        }

        public EntityWithGuidKey(Context context)
            : base(context)
        {
        }
    }
}
