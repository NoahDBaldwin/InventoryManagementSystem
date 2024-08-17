using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Task
{
    internal class Products
    {

        public static BindingList<Parts> associatedParts = new BindingList<Parts>();

        public int ProductId { get; set; }
        public string ProductName {  get; set; }
        public int ProductInventory { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductMin {  get; set; }
        public int ProductMax {  get; set; }

        public List<Parts> AssociatedParts { get; set; }


        // add associated part
        public static void AddAssociatedPart(Parts associatedPart)
        {
            // get part by Id, then add it to associated parts
            associatedParts.Add(associatedPart);
            
        }

        public void CopyPartsToList()
        {
            // move all parts from associatedParts to AssociatedParts, and empty out associatedParts
            AssociatedParts = new List<Parts>();
            foreach (Parts p in associatedParts)
            {
                AssociatedParts.Add(p);
            }
            associatedParts.Clear();
        }

        // move all AssociatedParts into associatedParts
        public void CopyListToParts()
        {
            foreach (Parts p in AssociatedParts)
            {
                associatedParts.Add(p);
            }
        }

        // remove associated Part
        public static bool RemoveAssociatedPart(int deletingPartId)
        {
            Parts deletingPart = associatedParts[deletingPartId];
            associatedParts.Remove(deletingPart);
            return true;
        }




        //Lookup Associated Part
        public static Parts LookupAssociatedPart(int partSearchId)
        {
            foreach (Parts part in Inventory.partsList)
            {
                if (part.PartId == partSearchId)
                {
                    return part;
                }

            }
            return null;
        }
    }

}
