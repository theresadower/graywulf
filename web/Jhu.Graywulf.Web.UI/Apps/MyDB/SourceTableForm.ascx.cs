﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class SourceTableForm : FederationUserControlBase
    {
        public TableOrView Table
        {
            get
            {
                return FederationContext.MyDBDataset.Tables[tableList.SelectedValue];
            }
            set
            {
                tableList.SelectedValue = value.UniqueKey;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshTableList();
            }
        }

        private void RefreshTableList()
        {
            FederationContext.MyDBDataset.Tables.LoadAll(true);

            foreach (var table in FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                tableList.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }
        }

    }
}