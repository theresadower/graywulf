﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class Bootstrap : ScriptLibrary
    {
        public override Script[] Scripts
        {
            get
            {
                return new[] {
                    new Script()
                    {
                        Name = "bootstrap",
                        Mapping = new ScriptResourceDefinition()
                        {
                            Path = "~/Scripts/Bootstrap/js/bootstrap.min.js",
                            DebugPath = "~/Scripts/Bootstrap/js/bootstrap.js",
                            CdnPath = "http://ajax.aspnetcdn.com/ajax/bootstrap/3.3.6/bootstrap.min.js",
                            CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/bootstrap/3.3.6/bootstrap.js",
                        },
                    }
                };
            }
        }
    }
}
