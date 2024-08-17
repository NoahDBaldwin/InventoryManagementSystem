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
    public partial class ModifyProductForm : Form
    {
        MainForm mainForm;

        Products loadProduct;

        public ModifyProductForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void ModifyProductForm_Load(object sender, EventArgs e)
        {
            // Initialize the DGVs and clear associatedParts list
            modifyProductAllPartsDGV.DataSource = Inventory.partsList;
            Products.associatedParts.Clear();
            modifyProductAssociatedPartsDGV.DataSource = Products.associatedParts;

            // Make the misc section not visible in either DGV upon load
            modifyProductAllPartsDGV.Columns["Misc"].Visible = false;
            modifyProductAssociatedPartsDGV.Columns["Misc"].Visible = false;


            // Make sure that a row was selected from partsDataGridView on load
            if (mainForm.productsDataGridView.SelectedRows.Count < 1)
            {
                MessageBox.Show("Select a product to modify.");
                this.Close();
            }
            else
            {
                // Get the data from the selected product and use it to populate fields in modifyProduct form
                DataGridViewSelectedRowCollection selected = mainForm.productsDataGridView.SelectedRows;
                int index = mainForm.productsDataGridView.CurrentCell.RowIndex;

                loadProduct = Inventory.productsList[index];

                modifyProductIDTextBox.Text = loadProduct.ProductId.ToString();
                modifyProductNameTextBox.Text = loadProduct.ProductName;
                modifyProductInventoryTextBox.Text = loadProduct.ProductInventory.ToString();
                modifyProductPriceTextBox.Text = loadProduct.ProductPrice.ToString();
                modifyProductMinTextBox.Text = loadProduct.ProductMin.ToString();
                modifyProductMaxTextBox.Text = loadProduct.ProductMax.ToString();

                // need to copy all the parts into the part list for this newly created products object
                loadProduct.CopyListToParts();
            }
        }

        // When form is loaded, rows aren't automatically highlighted/selected
        private void modifyProductAllBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            modifyProductAllPartsDGV.ClearSelection();
        }

        private void modifyProductAssociatedBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            modifyProductAssociatedPartsDGV.ClearSelection();
        }


        // Add associated parts to product
        private void modifyProductAllPartsButton_Click(object sender, EventArgs e)
        {

            if (modifyProductAllPartsDGV.SelectedRows.Count > 0)
            {
                // Get the ID of the selected Part
                string associatedPartId = modifyProductAllPartsDGV.SelectedRows[0].Cells[0].Value.ToString();

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


        // Save button
        private void modifyProductSaveButton_Click(object sender, EventArgs e)
        {

            // Keep track if there are errors
            int errorCount1 = 0;

            // Make sure all fields are filled and doesn't have an error for fields that don't need to be filled
            foreach (Control txtBox in this.Controls)
            {
                if (txtBox.Name != "modifyProductSearchTextBox" && txtBox.Name != "modifyProductAllPartsDGV" && txtBox.Name != "modifyProductAssociatedPartsDGV")
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
                    Convert.ToInt32(modifyProductInventoryTextBox.Text);
                    Convert.ToDecimal(modifyProductPriceTextBox.Text);
                    Convert.ToInt32(modifyProductMinTextBox.Text);
                    Convert.ToInt32(modifyProductMaxTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    parseError = true;
                    return;
                }

                bool minMaxError = false;
                if (Convert.ToInt32(modifyProductMinTextBox.Text) > Convert.ToInt32(modifyProductMaxTextBox.Text))
                {
                    MessageBox.Show("Min must be lower than Max.");
                    minMaxError = true;
                }

                // Make sure that inventory is between min and max
                bool inventoryError = false;
                if (Convert.ToInt32(modifyProductInventoryTextBox.Text) < Convert.ToInt32(modifyProductMinTextBox.Text) ||
                    Convert.ToInt32(modifyProductInventoryTextBox.Text) > Convert.ToInt32(modifyProductMaxTextBox.Text))
                {
                    MessageBox.Show("Inventory must be between Min and Max values.");
                    inventoryError = true;
                }

                // If there are no errors, run the actual save code
                if (parseError == false && minMaxError == false && inventoryError == false)
                {
                    
                    // Clear the current associated part, and add the new one with the new properties, then close the window.
                    loadProduct.ProductId = Convert.ToInt32(modifyProductIDTextBox.Text);
                    loadProduct.ProductName = modifyProductNameTextBox.Text;
                    loadProduct.ProductInventory = Convert.ToInt32(modifyProductInventoryTextBox.Text);
                    loadProduct.ProductPrice = Convert.ToDecimal(modifyProductPriceTextBox.Text);
                    loadProduct.ProductMin = Convert.ToInt32(modifyProductMinTextBox.Text);
                    loadProduct.ProductMax = Convert.ToInt32(modifyProductMaxTextBox.Text);
                    
                    Inventory.UpdateProduct(loadProduct.ProductId, loadProduct);

                    // Close form and clear selection in main form
                    this.Close();
                    mainForm.productsDataGridView.ClearSelection();
                }
            }
        }


        // Delete an associated part when Delete button is clicked
        private void modifyProductDeleteButton_Click(object sender, EventArgs e)
        {
            // Confirm a part is selected
            if (modifyProductAssociatedPartsDGV.SelectedRows.Count > 0)
            {
                // Ask for confirmation
                DialogResult isSure = MessageBox.Show("Are you sure you want to delete this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (isSure == DialogResult.Yes)
                {
                    // Delete the part from the associatedParts list. (When saved it will be delted from AssociatedParts)
                    foreach (DataGridViewRow row in modifyProductAssociatedPartsDGV.SelectedRows)
                    {
                        int deletingPartId = Convert.ToInt32(modifyProductAssociatedPartsDGV.CurrentCell.RowIndex);
                        Products.RemoveAssociatedPart(deletingPartId);
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


        //Search button
        private void modifyProductSearchButton_Click(object sender, EventArgs e)
        {
            // Clear the selection in both DGVs
            modifyProductAssociatedPartsDGV.ClearSelection();
            modifyProductAllPartsDGV.ClearSelection();
            // Take in input of the ID number
            int partSearchId;
            if (modifyProductSearchTextBox.Text != "")
            {
                // Get the part ID and us it to look up the part by calling the LookupAssociatedPart function.
                partSearchId = Convert.ToInt32(modifyProductSearchTextBox.Text);
                Parts searched = Products.LookupAssociatedPart(partSearchId);
                if (searched == null)
                {
                    // if part is not found...
                    MessageBox.Show("Part not found");
                }
                else
                {
                    // highlight the selected part in both DGVs if found
                    foreach (DataGridViewRow row in modifyProductAllPartsDGV.Rows)
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

                    foreach (DataGridViewRow row in modifyProductAssociatedPartsDGV.Rows)
                    {
                        Parts rowPart = row.DataBoundItem as Parts;
                        if (rowPart != null && rowPart == searched)
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

        // Close form when cancel button is pressed.
        private void modifyProductCancelButton_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
    }
}
