using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace TilesDavis.AddShortcut
{
    [RunInstaller(true)]
    public partial class EventLogInstaller : System.Diagnostics.EventLogInstaller
    {
        public EventLogInstaller()
        {
            this.Log = "Application";
            this.Source = "DeiCon.TilesDavis";
            InitializeComponent();
        }
    }
}
