using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazurefunctions.Functions.Entities
{
    public class TimeConsolidatedEntity : TableEntity
    {

        public int IdEmployee { get; set; }

        public DateTime DateHour { get; set; }

        public bool Consolidated { get; set; }

    }
}
