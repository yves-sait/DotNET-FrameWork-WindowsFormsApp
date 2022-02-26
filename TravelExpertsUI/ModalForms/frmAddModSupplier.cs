

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
    /// Blueprint on adding/modifying Suppliers
    /// </summary>
    public partial class frmAddModSupplier : Form
    {

        //declare public variables
        public Supplier supplier;
        public bool isAdd;
        public List<Product> Products = new List<Product>(); // Existing products of the supplier.Users cannot delete on this list as it is already saved in the DB.
        private List<Product> allProducts = new List<Product>(); // new and existing products
        public List<Product> newProducts = new List<Product>(); // New products on this current context.
                                                                // Users can delete products on this list. Not yet saved in DB.
        public frmAddModSupplier()
        {
            InitializeComponent();
        }

        private void frmAddModSupplier_Load(object sender, EventArgs e)
        {
            //update form text based on action (add or modify)
            if (isAdd)
            {
                this.Text = "Add Supplier";

            }
            else
            {
                this.Text = "Modify Supplier";
                btnRemoveSup.Enabled = false; //disabled as no selected supplier
                //fill txtboxes data from main supplier form object
                txtName.Text = supplier.SupName;
                txtPackageID.Text = supplier.SupplierId.ToString();
            }

            GetProducts();
            ShowProducts();
            SetDatagridviewDisplay();

        }


        //get all products
        private void GetProducts()
        {
            // initiate connection to access data in DB dgvSupplierProduct
            try
            {
                using (TravelExpertContext db = new TravelExpertContext()) // instantiate context object
                {
                    //EF 
                    var allProducts = db.Products.Select(pr => new
                    {
                        pr.ProductId,
                        pr.ProdName
                    }).OrderBy(pr => pr.ProdName).ToList();

                    dgvAllProducts.DataSource = allProducts;  //set the retrieve data as dgv datasource

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the products:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //display the added product
        private void ShowProducts()
        {

            dgvSupplierProduct.DataSource = null; // set datasource to null to always get updated data.
            allProducts = Products.Concat(newProducts).ToList();//combine new and existing Products
            dgvSupplierProduct.DataSource = allProducts.OrderBy(p => p.ProdName).ToList(); //assign the combined list as dgvdatasource

            dgvSupplierProduct.ClearSelection();


        }


        //set the DGV interface settings
        private void SetDatagridviewDisplay()
        {
            // header settings
            dgvAllProducts.EnableHeadersVisualStyles = false;
            dgvAllProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvAllProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAllProducts.ColumnHeadersHeight = 40;
            dgvAllProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvAllProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //set selected row colow
            dgvAllProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);
            //set selected columns header colow
            dgvAllProducts.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);
            // hide the row header
            dgvAllProducts.RowHeadersVisible = false;
            // dgvSuppliers clicked on a cell, the full row will be selected.
            dgvAllProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAllProducts.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            dgvAllProducts.DefaultCellStyle.ForeColor = Color.Black;
            dgvAllProducts.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvAllProducts.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //set header column text and formatting
            DataGridViewColumn column0 = dgvAllProducts.Columns[0];
            column0.Width = 80;
            dgvAllProducts.Columns[0].HeaderText = "ID";
            dgvAllProducts.Columns[1].HeaderText = "Product Name";

            dgvAllProducts.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAllProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //hide id column
            dgvAllProducts.Columns[0].Visible = false;




            // header settings
            dgvSupplierProduct.EnableHeadersVisualStyles = false;
            dgvSupplierProduct.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvSupplierProduct.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSupplierProduct.ColumnHeadersHeight = 40;
            dgvSupplierProduct.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvSupplierProduct.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //set selected row colow
            dgvSupplierProduct.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);
            //set selected columns header colow
            dgvSupplierProduct.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);
            // hide the row header
            dgvSupplierProduct.RowHeadersVisible = false;
            // dgvSuppliers clicked on a cell, the full row will be selected.
            dgvSupplierProduct.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSupplierProduct.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            dgvSupplierProduct.DefaultCellStyle.ForeColor = Color.Black;
            dgvSupplierProduct.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtName))
            {
                //create new supplier object if add action
                if (isAdd)
                {
                    this.supplier = new Supplier();

                }

                supplier.SupName = txtName.Text.ToUpper(); // set the supplier name using the txtbox input

                this.DialogResult = DialogResult.OK; // set dialog result.

            }
        }

        //add button enabled when a product is selected
        private void dgvAllProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAllProducts.SelectedRows.Count > 0)
            {
                btnAddSup.Enabled = true;
            }


        }


        //add product to the supplier
        private void btnAddSup_Click(object sender, EventArgs e)
        {
            //get the selected data
            var selectedProdName = dgvAllProducts.CurrentRow.Cells[1].Value.ToString();
            var selectedProdID = Convert.ToInt32(dgvAllProducts.CurrentRow.Cells[0].Value);

            //validate if the selected product already exist as  supplier product
            if (!allProducts.Any(pr => pr.ProdName == selectedProdName && pr.ProductId == selectedProdID))
            {
                //add the product to list of new products
                newProducts.Add(new Product
                {
                    ProductId = selectedProdID,
                    ProdName = selectedProdName
                });
                ShowProducts();
            }
            else
            {
                MessageBox.Show("Product is already added", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //remove product in the list.Can only remove products that are not yet saved in the DB.
        private void btnRemoveSup_Click(object sender, EventArgs e)
        {
            //get the selected/clicked data
            var supSelected = Convert.ToInt32(dgvSupplierProduct.CurrentRow.Cells[0].Value);
            var supDelete = newProducts.SingleOrDefault(pr => pr.ProductId == supSelected);//query from newproducts list

            //remove the selected data from list of products
            if (supDelete != null)
            {
                newProducts.Remove(supDelete);
            }
            else  // if not new supplier, return below.
            {
                MessageBox.Show("UNATHORIZED ACTION! Removing products to supplier that has been saved  can cause data integrity issue. \n" +
                "Please contact administrator for more info.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            ShowProducts();
        }

        private void dgvSupplierProduct_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //hide id column
            dgvSupplierProduct.Columns[0].Visible = false;
            dgvSupplierProduct.Columns[2].Visible = false;
            dgvSupplierProduct.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //set header column text and formatting
            dgvSupplierProduct.Columns[1].HeaderText = "Supplier product(s)";
        }

        //enable/disable remove button when a selection is made
        private void dgvSupplierProduct_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvSupplierProduct.SelectedRows.Count > 0)
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
