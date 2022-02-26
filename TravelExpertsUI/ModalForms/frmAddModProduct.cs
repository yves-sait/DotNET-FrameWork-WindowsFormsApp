

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelExpertData;

namespace TravelExpertsUI.ModalForms
{
    /// <summary>
    /// Blueprint on adding/modifying Products
    /// </summary>
    public partial class frmAddModProduct : Form
    {

        //declare  variables
        public Product product; //product object
        public bool isAdd;
        public List<Supplier> Suppliers = new List<Supplier>(); // Existing suppliers of the product.Users cannot delete on this list as it is already saved in the DB.
        private List<Supplier> allSuppliers = new List<Supplier>(); // new and existing suppliers

        public List<Supplier> newSuppliers = new List<Supplier>(); // New suppliers on this current context.
                                                                   // Users can delete suppliers on this list.Not yet saved in DB.

        public frmAddModProduct()
        {
            InitializeComponent();
        }

        private void frmAddModProduct_Load(object sender, EventArgs e)
        {
            //update form text based on action (add or modify)
            if (isAdd)
            {
                this.Text = "Add Product";

            }
            else
            {
                this.Text = "Modify Product";
                btnRemoveSup.Enabled = false; //disabled as no selected product
                //fill txtboxes data from main supplier form object
                txtName.Text = product.ProdName;
                txtPackageID.Text = product.ProductId.ToString();
            }

            GetSuppliers();
            ShowSuppliers();
            SetDatagridviewDisplay();
        }


        // get all suppliers
        private void GetSuppliers()
        {
            // initiate connection to access data in DB
            try
            {
                using (TravelExpertContext db = new TravelExpertContext()) // instantiate context object
                {
                    //EF 
                    var allSuppliers = db.Suppliers.Select(s => new
                    {
                        s.SupplierId,
                        s.SupName
                    }).OrderBy(s => s.SupName).ToList();

                    dgvAllSuppliers.DataSource = allSuppliers; //set the retrieve data as dgv datasource

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the suppliers:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //set the DGV interface settings
        private void SetDatagridviewDisplay()
        {

            // header settings
            dgvAllSuppliers.EnableHeadersVisualStyles = false;
            dgvAllSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvAllSuppliers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAllSuppliers.ColumnHeadersHeight = 40;
            dgvAllSuppliers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvAllSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //set selected row colow
            dgvAllSuppliers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);
            //set selected columns header colow
            dgvAllSuppliers.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);
            // hide the row header
            dgvAllSuppliers.RowHeadersVisible = false;
            // dgvSuppliers clicked on a cell, the full row will be selected.
            dgvAllSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAllSuppliers.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            dgvAllSuppliers.DefaultCellStyle.ForeColor = Color.Black;
            dgvAllSuppliers.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvAllSuppliers.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //set header column text and formatting
            DataGridViewColumn column0 = dgvAllSuppliers.Columns[0];
            column0.Width = 80;
            dgvAllSuppliers.Columns[0].HeaderText = "ID";
            dgvAllSuppliers.Columns[1].HeaderText = "Supplier Name";

            dgvAllSuppliers.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAllSuppliers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //hide id column
            dgvAllSuppliers.Columns[0].Visible = false;




            // header settings
            dgvProductSupplier.EnableHeadersVisualStyles = false;
            dgvProductSupplier.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvProductSupplier.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProductSupplier.ColumnHeadersHeight = 40;
            dgvProductSupplier.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvProductSupplier.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //set selected row colow
            dgvProductSupplier.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);
            //set selected columns header colow
            dgvProductSupplier.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);
            // hide the row header
            dgvProductSupplier.RowHeadersVisible = false;
            // dgvSuppliers clicked on a cell, the full row will be selected.
            dgvProductSupplier.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductSupplier.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            dgvProductSupplier.DefaultCellStyle.ForeColor = Color.Black;
            dgvProductSupplier.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;



        }

        //display the added supplier
        private void ShowSuppliers()
        {
 
           dgvProductSupplier.DataSource = null; // set datasource to null to always get updated data.
           allSuppliers = Suppliers.Concat(newSuppliers).ToList();//combine new and existing suppliers
           dgvProductSupplier.DataSource = allSuppliers.OrderBy(s => s.SupName).ToList(); //assign the combined list as dgvdatasource
          
           dgvProductSupplier.ClearSelection();

        }


        //confirm the changes made
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtName))
            {
                //create new product object if add action
                if (isAdd)
                {
                    this.product = new Product();

                }
                product.ProdName = txtName.Text; // set the product name using the txtbox input

                this.DialogResult = DialogResult.OK; // set dialog result.

            }
        }

        //add button enabled when a supplier is selected
        private void dgvAllSuppliers_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvAllSuppliers.SelectedRows.Count > 0)
            {
                btnAddSup.Enabled = true;
            }

        }


        //add supplier to product
        private void btnAddSup_Click(object sender, EventArgs e)
        {
            //get the selected data
            var selectedSupName = dgvAllSuppliers.CurrentRow.Cells[1].Value.ToString();
            var selectedSupID = Convert.ToInt32(dgvAllSuppliers.CurrentRow.Cells[0].Value);

            //validate if the supplier already exist as product supplier
            if (!allSuppliers.Any(s => s.SupName == selectedSupName && s.SupplierId == selectedSupID))
            {
                //add the supplier to list of new suppliers
                newSuppliers.Add(new Supplier
                {
                    SupplierId = selectedSupID,
                    SupName = selectedSupName
                });
              

                ShowSuppliers();
                
            }
            else
            {
                MessageBox.Show("Supplier is already added", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        //remove supplier in the list. Can only remove suppliers that are not yet saved in the DB.
        private void btnRemoveSup_Click(object sender, EventArgs e)
        {
                //get the selected supplier
                var psSelected = Convert.ToInt32(dgvProductSupplier.CurrentRow.Cells[0].Value);
                var psdelete = newSuppliers.SingleOrDefault(ps => ps.SupplierId == psSelected); //query from newsupplier list

                //remove the selected data from list of new suppliers
            if (psdelete != null)
            {
                 newSuppliers.Remove(psdelete);
            }
            else // if not new supplier, return below.
            {
                MessageBox.Show("UNATHORIZED ACTION! Removing supplier to products that has been saved can cause data integrity issue. \n" +
                "Please contact administrator for more info.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
   
            ShowSuppliers();

        }


        private void dgvProductSupplier_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //hide id column
            dgvProductSupplier.Columns[0].Visible = false;
            dgvProductSupplier.Columns[2].Visible = false;
            dgvProductSupplier.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //set header column text and formatting
            dgvProductSupplier.Columns[1].HeaderText = "Product supplier(s)";

        }

        //enable/disable remove button when a selection is made
        private void dgvProductSupplier_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvProductSupplier.SelectedRows.Count > 0)
            {
                btnRemoveSup.Enabled = true;
            }
            else
            {
                btnRemoveSup.Enabled = false;
            }

        }

        //clear txtfields
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();

        }


    }
}
