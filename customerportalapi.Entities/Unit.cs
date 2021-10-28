using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Unit
    {
        public string UnitName { get; set; }

        public string SmUnitId { get; set; }

        public string Size { get; set; }

        public string UnitCategory { get; set; }

        public string Subtype { get; set; }

        public string Height { get; set; }

        public string Width { get; set; }

        public string Depth { get; set; }

        public string Colour { get; set; }
        public string Corridor { get; set; }
        public string Exceptions { get; set; }
        public string Floor { get; set; }
        public string Zone { get; set; }
    }

}
