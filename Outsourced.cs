using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Task
{
    internal class Outsourced : Parts
    {
        // Declare companyName attribute
        public string CompanyName
        {
            get { return Misc; }
            set { Misc = value; }
        }

        // Create Constructor for Outsourced class
        public Outsourced(int partId, string partName, int partInventory, decimal partPrice, int partMin, int partMax, string companyName)
        {
            this.PartId = partId;
            this.PartName = partName;
            this.PartInventory = partInventory;
            this.PartPrice = partPrice;
            this.PartMin = partMin;
            this.PartMax = partMax;
            this.CompanyName = companyName;
        }
    }
}
