using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C__Task
{
    public partial class AddProductForm : Form
    {
        MainForm mainForm;
        public AddProductForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }


        // Auto generate the next ID number
        int productIdCurrent;

        private void AddProductForm_Load(object sender, EventArgs e)
        {

            // Find the current largest ID number and increment it for current ID
            int maxId = 0;
            foreach (Products product in Inventory.productsList)
            {
                if (product.ProductId > maxId)
                {
                    maxId = product.ProductId;
                }
            }

            // Make ID number show in the text box
            productIdCurrent = maxId + 1;
            addProductIDTextBox.Text = Convert.ToString(productIdCurrent);




            // Initialize the DGVs
            addProductAllPartsDGV.DataSource = Inventory.partsList;
            addProductAssociatedPartsDGV.DataSource = Products.associatedParts;

            // Make the misc section not visible in either DGV upon load
            addProductAllPartsDGV.Columns["Misc"].Visible = false;
            addProductAssociatedPartsDGV.Columns["Misc"].Visible = false;
        }


        // When form is loaded, rows aren't automatically highlighted/selected
        private void addProductAllBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            addProductAllPartsDGV.ClearSelection();
        }

        private void addProductAssociatedBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            addProductAssociatedPartsDGV.ClearSelection();
        }


        // Add associated part to product
        private void addProductAllPartsButton_Click(object sender, EventArgs e)
        {
            
            if (addProductAllPartsDGV.SelectedRows.Count > 0)
            {
                // Get the ID of the selected Part
                string associatedPartId = addProductAllPartsDGV.SelectedRows[0].Cells[0].Value.ToString();

                // Identify the part from partsList with the selected part ID and use as parameter to add to associatedParts list
                Parts associatedPart = Inventory.partsList[Convert.ToInt32(associatedPartId)];

                // if associated part is already in the associatedParts list, don't add it.
                int count = 0;
                foreach (Parts part in Products.associatedParts)
                {
                    if (part.PartId == Convert.ToInt32(associatedPartId))
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    MessageBox.Show("This part is already an associated part.");
                }
                else
                {
                    Products.AddAssociatedPart(associatedPart);
                }
            }
            // If a part is not selected...
            else
            {
                MessageBox.Show("Please select a part in order to add it to the product");
            }
            
        }


        // Save changes and close window. Check for errors.
        private void addProductSaveButton_Click(object sender, EventArgs e)
        {

            // Keep track if there are errors
            int errorCount1 = 0;

            // Make sure all fields are filled and doesn't have an error for fields that don't need to be filled
            foreach (Control txtBox in this.Controls)
            {
                if (txtBox.Name != "addProductSearchTextBox" && txtBox.Name != "addProductAllPartsDGV" && txtBox.Name != "addProductAssociatedPartsDGV")
                {
                    if (txtBox.Text == "")
                    {
                        // Change color if missing a field
                        txtBox.BackColor = Color.FromArgb(255, 100, 100);
                        errorCount1++;
                    }
                }
            }

            // Show an error if any fields are missing
            if (errorCount1 > 0)
            {
                MessageBox.Show("Fill all of fields." + errorCount1);
            }
            else
            {

                // check if there is a parsing error (if users put in a string where a number belongs)
                bool parseError = false;

                // Make sure that it is numbers entered into the text boxes, catch parsing errors
                try
                {
                    // ensure they can be converted
                    Convert.ToInt32(addProductInventoryTextBox.Text);
                    Convert.ToDecimal(addProductPriceTextBox.Text);
                    Convert.ToInt32(addProductMinTextBox.Text);
                    Convert.ToInt32(addProductMaxTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    parseError = true;
                    return;
                }

                bool minMaxError = false;
                if (Convert.ToInt32(addProductMinTextBox.Text) > Convert.ToInt32(addProductMaxTextBox.Text))
                {
                    MessageBox.Show("Min must be lower than Max.");
                    minMaxError = true;
                }

                // Make sure that inventory is between min and max
                bool inventoryError = false;
                if (Convert.ToInt32(addProductInventoryTextBox.Text) < Convert.ToInt32(addProductMinTextBox.Text) ||
                    Convert.ToInt32(addProductInventoryTextBox.Text) > Convert.ToInt32(addProductMaxTextBox.Text))
                {
                    MessageBox.Show("Inventory must be between Min and Max values.");
                    inventoryError = true;
                }

                // If there are no errors, run the actual save code
                if (parseError == false && minMaxError == false && inventoryError == false)
                {
                    // create the new products object
                    Products newProduct = new Products
                    {
                        ProductId = productIdCurrent,
                        ProductName = addProductNameTextBox.Text,
                        ProductInventory = Convert.ToInt32(addProductInventoryTextBox.Text),
                        ProductPrice = Convert.ToDecimal(addProductPriceTextBox.Text),
                        ProductMin = Convert.ToInt32(addProductMinTextBox.Text),
                        ProductMax = Convert.ToInt32(addProductMaxTextBox.Text)
                    };

                    // need to copy all the parts into the part list for this newly created products object
                    newProduct.CopyPartsToList();

                    // Add the content from each of the fields in the form to the "product" object and add to the binding list.
                    Inventory.AddProduct(newProduct);

                    // Close the form and clear selection in main form.
                    this.Close();
                    mainForm.productsDataGridView.ClearSelection();
                }
            }
        }


        // Cancel button closes the window without saving
        private void addProductCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Search button
        private void addProductSearchButton_Click(object sender, EventArgs e)
        {
            // Clear the selected parts in both DGVs
            addProductAllPartsDGV.ClearSelection();
            addProductAssociatedPartsDGV.ClearSelection();
            // Take in input of the ID number
            int partSearchId;
            if (addProductSearchTextBox.Text != "")
            {
                // Get the part ID and us it to look up the part by calling the LookupAssociatedPart function.
                partSearchId = Convert.ToInt32(addProductSearchTextBox.Text);
                Parts searched = Products.LookupAssociatedPart(partSearchId);
                if (searched == null)
                {
                    // if no part matches the ID...
                    MessageBox.Show("Part not found");
                }
                else
                {
                    // Highlight the part that was searched in both DGVs
                    foreach (DataGridViewRow row in addProductAllPartsDGV.Rows)
                    {
                        Parts rowPart = row.DataBoundItem as Parts;
                        if (rowPart != null && rowPart == searched)
                        {
                            row.Selected = true;
                        }
                    }

                    // if the part not an associated part...
                    if (Products.associatedParts.Count == 0)
                    {
                        MessageBox.Show("There are no parts associated with this product");
                    }

                    foreach (DataGridViewRow row in addProductAssociatedPartsDGV.Rows)
                    {
                        Parts rowAssocPart = row.DataBoundItem as Parts;
                        if (rowAssocPart != null && rowAssocPart == searched)
                        {
                            row.Selected = true;
                        }
                        else
                        {
                            MessageBox.Show("The part is not associated with this product.");
                        }
                    }
                }
            }
        }

        // Delete an associated part when Delete button is clicked if a part is selected and user confirms
        private void addProductDeleteButton_Click(object sender, EventArgs e)
        {
            if (addProductAssociatedPartsDGV.SelectedRows.Count > 0)
            {
                DialogResult isSure = MessageBox.Show("Are you sure you want to delete this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (isSure == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in addProductAssociatedPartsDGV.SelectedRows)
                    {
                        addProductAssociatedPartsDGV.Rows.RemoveAt(row.Index);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Select an associated part to delete");
            }
        }


    }
}
