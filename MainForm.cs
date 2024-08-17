using System.ComponentModel;
using System.Security.AccessControl;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace C__Task
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // Add button under parts DGV on main directs to Add Parts Window
        private void addPartsButton_Click(object sender, EventArgs e)
        {
            // "this" allows the partsDataGridView to transfer to the add parts form
            AddPartForm addPartForm = new();
            addPartForm.Show();
        }

        // Modify button under parts DGV on main directs to Modify Parts Window
        private void modifyPartsButton_Click(object sender, EventArgs e)
        {
            ModifyPartForm modifyPartForm = new(this);
            modifyPartForm.Show();
        }

        // Add button under products DGV on main directs to Add Products Window
        private void addProductsButton_Click(object sender, EventArgs e)
        {
            AddProductForm addProductForm = new(this);
            addProductForm.Show();
        }

        // Modify button under products DGV on main directs to Modify Products Window
        private void modifyProductsButton_Click(object sender, EventArgs e)
        {
            ModifyProductForm modifyProductForm = new(this);
            modifyProductForm.Show();
        }

        /*
         _____                      _                    _ 
        |  ___|__  _ __ _ __ ___   | |    ___   __ _  __| |
        | |_ / _ \| '__| '_ ` _ \  | |   / _ \ / _` |/ _` |
        |  _| (_) | |  | | | | | | | |__| (_) | (_| | (_| |
        |_|  \___/|_|  |_| |_| |_| |_____\___/ \__,_|\__,_|
        */

        // Function to populate the tables of the main form on load
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create a few items for the parts list
            InHouse inHouse1 = new InHouse(0, "Tire", 11, 8.00M, 5, 25, 12);
            InHouse inHouse2 = new InHouse(1, "Bottom Bracket", 7, 24.87M, 5, 25, 3);
            Outsourced outsourced1 = new Outsourced(2, "Pannier", 16, 88.45M, 5, 25, "Ortlieb");
            Outsourced outsourced2 = new Outsourced(3, "Pannier", 16, 88.45M, 5, 25, "Ortlieb");


            // Initialize data in the data lists. Then populate DGV by using the lists as a datasource.
            Inventory.ListInit();
            Inventory.partsList.Add(inHouse1);
            Inventory.partsList.Add(inHouse2);
            Inventory.partsList.Add(outsourced1);
            partsDataGridView.DataSource = Inventory.partsList;
            productsDataGridView.DataSource = Inventory.productsList;
         
          
            // When form is loaded, make min, max not visible
            partsDataGridView.Columns["partMin"].Visible = false;
            partsDataGridView.Columns["partMax"].Visible = false;
            partsDataGridView.Columns["Misc"].Visible = false;
            productsDataGridView.Columns["productMin"].Visible = false;
            productsDataGridView.Columns["productMax"].Visible = false;
        }


        // When the form is loaded, there isn't a row automatically highlighted/selected
        private void partsMainBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            partsDataGridView.ClearSelection();
        }
        private void productsMainDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            productsDataGridView.ClearSelection();
        }


        /*
         ____       _      _         ____        _   _                  
        |  _ \  ___| | ___| |_ ___  | __ ) _   _| |_| |_ ___  _ __  ___ 
        | | | |/ _ \ |/ _ \ __/ _ \ |  _ \| | | | __| __/ _ \| '_ \/ __|
        | |_| |  __/ |  __/ ||  __/ | |_) | |_| | |_| || (_) | | | \__ \
        |____/ \___|_|\___|\__\___| |____/ \__,_|\__|\__\___/|_| |_|___/
        */

        // Parts delete button
        private void deletePartsButton_Click(object sender, EventArgs e)
        {
            if (partsDataGridView.SelectedRows.Count > 0) {
                DialogResult isSure = MessageBox.Show("Are you sure you want to delete this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (isSure == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in partsDataGridView.SelectedRows)
                    {
                        // Get the ID of the selected Part
                        string selectedPartId = partsDataGridView.SelectedRows[0].Cells[0].Value.ToString();

                        // Identify the part from partsList with the selected part ID and use as parameter to add to associatedParts list
                        Parts selectedPart = Inventory.partsList[Convert.ToInt32(selectedPartId)];

                        // if associated part is in the associatedParts list, don't remove it.
                        int count = 0;
                       
                        foreach (Products product in Inventory.productsList)
                        {
                            foreach (Parts assocPart in product.AssociatedParts)
                            {
                                if (assocPart.PartId == Convert.ToInt32(selectedPartId))
                                {
                                    count++;
                                }
                            }
                        }
                        if (count > 0)
                        {
                            MessageBox.Show("This part is associated with one or more products and can't be deleted unless it is removed from the product(s)");
                        }
                        else
                        {
                            // Delete the part.
                            Inventory.DeletePart(selectedPart);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            // If a part is not selected...
            else
            {
                MessageBox.Show("Select a part to delete");
            }

        }

        // Products Delete Button
        private void deleteProductsButton_Click(object sender, EventArgs e)
        {
            if (productsDataGridView.SelectedRows.Count > 0)
            {
                DialogResult isSure = MessageBox.Show("Are you sure you want to delete this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (isSure == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in productsDataGridView.SelectedRows)
                    {
                        Products deletingProduct = (Products)productsDataGridView.CurrentRow.DataBoundItem;
                        Inventory.RemoveProduct(deletingProduct.ProductId);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Select a product to delete");
            }
        }

        /*
          ____                      _       ____        _   _                  
         / ___|  ___  __ _ _ __ ___| |__   | __ ) _   _| |_| |_ ___  _ __  ___ 
         \___ \ / _ \/ _` | '__/ __| '_ \  |  _ \| | | | __| __/ _ \| '_ \/ __|
          ___) |  __/ (_| | | | (__| | | | | |_) | |_| | |_| || (_) | | | \__ \
         |____/ \___|\__,_|_|  \___|_| |_| |____/ \__,_|\__|\__\___/|_| |_|___/
        */

        // Show matching parts from search
        private void partsSearchButton_Click(object sender, EventArgs e)
        {
            partsDataGridView.ClearSelection();
            // Take in input of the ID number
            int partSearchId;
            if (partsSearchTextBox.Text != "")
            {
                
                partSearchId = Convert.ToInt32(partsSearchTextBox.Text);
                Parts searched = Inventory.LookupPart(partSearchId);
                if(searched == null)
                {
                    MessageBox.Show("Part not found");
                }
                else
                {
                    foreach (DataGridViewRow row in partsDataGridView.Rows)
                    {
                        Parts rowPart = row.DataBoundItem as Parts;
                        if(rowPart != null && rowPart == searched)
                        {
                            row.Selected = true;
                        } 
                    }
                }
            }
        }

        // Show matching Products from search
        private void productsSearchButton_Click(object sender, EventArgs e)
        {
            productsDataGridView.ClearSelection();
 
            // Take in input of the ID number
            int productSearchId;
            if (productsSearchTextBox.Text != "")
            {

                productSearchId = Convert.ToInt32(productsSearchTextBox.Text);
                Products searched = Inventory.LookupProduct(productSearchId);
                if (searched == null)
                {
                    MessageBox.Show("Product not found");
                }
                else
                {
                    foreach (DataGridViewRow row in productsDataGridView.Rows)
                    {
                        Products rowProduct = row.DataBoundItem as Products;
                        if (rowProduct != null && rowProduct == searched)
                        {
                            row.Selected = true;
                        }
                    }
                }
            }
        }
        /*
         _____      _ _     ____        _   _              
        | ____|_  _(_) |_  | __ ) _   _| |_| |_ ___  _ __  
        |  _| \ \/ / | __| |  _ \| | | | __| __/ _ \| '_ \ 
        | |___ >  <| | |_  | |_) | |_| | |_| || (_) | | | |
        |_____/_/\_\_|\__| |____/ \__,_|\__|\__\___/|_| |_|
        */

        // Exit Button closes Window
        private void mainExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
