using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazurefunctions.Functions.Entities
{
    public class TiempoEntity : TableEntity
    {

        public int IdEmployee { get; set; }

        public DateTime DateHour { get; set; }

        public string Type { get; set; }

        public bool Consolidated { get; set; }

    }
}
