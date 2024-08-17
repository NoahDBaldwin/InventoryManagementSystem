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
    public partial class ModifyPartForm : Form
    {
        MainForm mainForm;
        public ModifyPartForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }


        private void ModifyPartForm_Load(object sender, EventArgs e)
        {
            // Make sure that a row was selected from partsDataGridView on load
            if (mainForm.partsDataGridView.SelectedRows.Count < 1)
            {
                MessageBox.Show("Select a part to modify.");
                this.Close();
            }
            else
            {
                foreach (DataGridViewRow row in mainForm.partsDataGridView.SelectedRows)
                {
                    modifyPartIDTextBox.Text = row.Cells[0].Value.ToString();
                    modifyPartNameTextBox.Text = row.Cells[1].Value.ToString();
                    modifyPartInventoryTextBox.Text = row.Cells[2].Value.ToString();
                    modifyPartPriceTextBox.Text = row.Cells[3].Value.ToString();
                    modifyPartMinTextBox.Text = row.Cells[4].Value.ToString();
                    modifyPartMaxTextBox.Text = row.Cells[5].Value.ToString();
                    modifyPartMachineIDTextBox.Text = row.Cells[6].Value.ToString();
                }

            }

        }


        // Checks if the radio button is inhouse or outsourced, change label text
        bool isInHouse = true;
        private void modifyPartInHouseRadio_CheckedChanged(object sender, EventArgs e)
        {
            modifyPartMachineIDLabel.Text = "Machine ID";
        }
        private void modifyPartOutsourcedRadio_CheckedChanged(object sender, EventArgs e)
        {
            modifyPartMachineIDLabel.Text = "Company Name";
        }


        private void modifyPartSaveButton_Click(object sender, EventArgs e)
        {

            // Check if the part is inhouse or outsourced
            if (modifyPartInHouseRadio.Checked)
            {
                isInHouse = true;
            }
            else
            {
                isInHouse = false;
            }

            // Keep track if there are errors
            int errorCount1 = 0;

            foreach (Control txtBox in this.Controls)
            {
                if (txtBox.Text == "")
                {
                    // Change color if missing a field
                    txtBox.BackColor = Color.FromArgb(255, 100, 100);
                    errorCount1++;
                }
            }

            // Show an error if any fields are missing
            if (errorCount1 > 0)
            {
                MessageBox.Show("Fill all of fields.");
            } 
             
             
            else
            {
                // check if there is an error (if users put in a string where a number belongs)
                bool parseError = false;

                // Make sure that it is numbers entered into the text boxes, catch parsing errors
                try
                {
                    int inventory = Convert.ToInt32(modifyPartInventoryTextBox.Text);
                    decimal price = Convert.ToDecimal(modifyPartPriceTextBox.Text);
                    int min = Convert.ToInt32(modifyPartMinTextBox.Text);
                    int max = Convert.ToInt32(modifyPartMaxTextBox.Text);
                    if (isInHouse == true)
                    {
                        int machineId = Convert.ToInt32(modifyPartMachineIDTextBox.Text);
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    parseError = true;
                    return;
                }

                // Make sure that min is lower than max
                bool minMaxError = false;
                if (Convert.ToInt32(modifyPartMinTextBox.Text) > Convert.ToInt32(modifyPartMaxTextBox.Text))
                {
                    MessageBox.Show("Min must be lower than Max.");
                    minMaxError = true;
                }

                // Make sure that inventory is between min and max
                bool inventoryError = false;
                if (Convert.ToInt32(modifyPartInventoryTextBox.Text) < Convert.ToInt32(modifyPartMinTextBox.Text) ||
                    Convert.ToInt32(modifyPartInventoryTextBox.Text) > Convert.ToInt32(modifyPartMaxTextBox.Text))
                {
                    MessageBox.Show("Inventory must be between Min and Max values.");
                    inventoryError = true;
                }

                // If no errors, save the new part
                if (parseError == false && minMaxError == false && inventoryError == false)
                {
                    // Get the Id of the original part
                    int partId = Convert.ToInt32(modifyPartIDTextBox.Text);

                    // Add the content from each of the fields in the form to the "part" object (inhouse or outsourced).
                    if (isInHouse == true)
                    {

                        InHouse inHouse = new InHouse
                        (
                            Convert.ToInt32(modifyPartIDTextBox.Text),
                            modifyPartNameTextBox.Text,
                            Convert.ToInt32(modifyPartInventoryTextBox.Text),
                            Convert.ToDecimal(modifyPartPriceTextBox.Text),
                            Convert.ToInt32(modifyPartMinTextBox.Text),
                            Convert.ToInt32(modifyPartMaxTextBox.Text),
                            Convert.ToInt32(modifyPartMachineIDTextBox.Text)
                        );

                        // replace the old part with the updated part in the binding list
                        Inventory.UpdatePart(partId, inHouse);
                    }
                    else
                    {
                        Outsourced outsourced = new Outsourced
                        (
                            Convert.ToInt32(modifyPartIDTextBox.Text),
                            modifyPartNameTextBox.Text,
                            Convert.ToInt32(modifyPartInventoryTextBox.Text),
                            Convert.ToDecimal(modifyPartPriceTextBox.Text),
                            Convert.ToInt32(modifyPartMinTextBox.Text),
                            Convert.ToInt32(modifyPartMaxTextBox.Text),
                            modifyPartMachineIDTextBox.Text
                        );

                        // replace the old part with the updated part in the binding list
                        Inventory.UpdatePart(partId , outsourced);
                    }

                    // Close the form
                    this.Close();
                }
            }
        }




        // Cancel button closes the window
        private void modifyPartCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
