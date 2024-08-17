using C__Task;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace C__Task
{
    internal class Inventory
    {

        // Initialize the lists for parts and products by making new objects of type BindingList of parts/products
        public static BindingList<Parts> partsList = new BindingList<Parts>();
        public static BindingList<Products> productsList = new BindingList<Products>();


        // Add Product
        public static void AddProduct(Products product)
        {
            productsList.Add(product);
        }


        // Remove Product
        public static bool RemoveProduct(int productID)
        {
            // remove from products list, the product where the Id matches.
            productsList.RemoveAt(productID);
            return true;
        }


        // lookupProduct
        public static Products LookupProduct(int productSearchId)
        {

            foreach (Products product in productsList)
            {
                if (product.ProductId == productSearchId)
                {
                    return product;
                }

            }

            return null;

        }

        
        // Update Product. Replace old product with updated one.
        public static void UpdateProduct(int productId, Products product)
        {
            //productsList[productId] = product;
            product.AssociatedParts.Clear();
            product.CopyPartsToList();
        }
        



        // Add Part
        public static void AddPart(Parts part)
        {
            partsList.Add(part);
        }


        // Delete Part
        public static bool DeletePart(Parts part)
        {
            partsList.Remove(part);
            return true;
        }

        
        // lookupPart
        public static Parts LookupPart(int partSearchId)
        {
            
            foreach (Parts part in partsList)
            {
                if(part.PartId == partSearchId)
                {
                    return part;
                }
                
            }
            return null;
        }

        
        // Update part. Replace the old part in the list with the updated part.
        public static void UpdatePart(int partId, Parts part)
        {
            partsList[partId] = part; 
        }




        // Populate some data into the products list
        public static void ListInit()
        {
            productsList.Add(new Products
            {
                ProductId = 0,
                ProductName = "Specialized Sirrus",
                ProductInventory = 8,
                ProductPrice = 345.95M,
                ProductMin = 5,
                ProductMax = 25,
                AssociatedParts = new List<Parts>(),
            });

            productsList.Add(new Products
            {
                ProductId = 1,
                ProductName = "Specialized Reabux",
                ProductInventory = 12,
                ProductPrice = 890.99M,
                ProductMin = 5,
                ProductMax = 25,
                AssociatedParts = new List<Parts>(),
            });

            productsList.Add(new Products
            {
                ProductId = 2,
                ProductName = "Specialized S-Works",
                ProductInventory = 5,
                ProductPrice = 9559.00M,
                ProductMin = 5,
                ProductMax = 25,
                AssociatedParts = new List<Parts>(),
            });


        }
    }
}
