

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
using TravelExpertsUI.ModalForms;

namespace TravelExpertsUI.MenuForms

{

    /// <summary>
    /// BLueprint for Product Entities
    /// </summary>
    public partial class frmMenuProduct : Form
    {
        private Product currentProduct;  // variable to track current product

        private int selectedID;

        public frmMenuProduct() { InitializeComponent(); }

        private void frmMenuProduct_Load(object sender, EventArgs e) { LoadProducts(); }

        // Load the products to DGV
        private void LoadProducts()
        {
            // clear current data if any.
            dgvProducts.DataSource = null;
            dgvProducts.Rows.Clear();
            dgvProducts.Columns.Clear();
            // initiate connection to access data in DB
            try
            {
                using (TravelExpertContext db = new TravelExpertContext())  // instantiate context object
                {
                    // query all Products
                    var products = db.Products.Select(p => new { p.ProductId, p.ProdName }).OrderBy(p => p.ProdName).ToList();

                    dgvProducts.DataSource = products;  // assign result as DGV datasource
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the products:" + ex.Message, ex.GetType().ToString(),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetDatagridviewDisplay();  // Call method to configure DGV
        }

        // Set the DGV interface settings
        private void SetDatagridviewDisplay()
        {
            // header settings
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Regular);
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProducts.ColumnHeadersHeight = 50;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            // set selected row colow
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);
            // set selected columns header colow
            dgvProducts.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);
            // hide the row header
            dgvProducts.RowHeadersVisible = false;
            // dgvProducts clicked on a cell, the full row will be selected.
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            dgvProducts.DefaultCellStyle.ForeColor = Color.Black;
            dgvProducts.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvProducts.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // set header column text and formatting
            DataGridViewColumn column0 = dgvProducts.Columns[0];
            column0.Width = 80;
            dgvProducts.Columns[0].HeaderText = "ID";
            dgvProducts.Columns[1].HeaderText = "Product Name";

            dgvProducts.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // show the listbox of product supplier
        private void dgvProducts_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // set row height
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                row.Height = 40;
            }

            lstProductSupplier.Visible = true;
        }

        // enable modify and delete buttons only if a row/cell is selected.
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            btnModify.Enabled = true;
            btnRemove.Enabled = true;

            selectedID = Convert.ToInt32(dgvProducts.CurrentRow.Cells[0].Value);  // store the selected product id to variable

            GetSupplier();  // load the supplier
        }

        //retrieve all th suppliers attached to the selected product.
        private void GetSupplier()
        {
            try
            {
                using (TravelExpertContext db = new TravelExpertContext())  // instantiate context object
                {
                    // EF query all suppliers that  attached to the selected product.
                    var productSuppliers = db.Products
                                               .Join(db.ProductsSuppliers, prd => prd.ProductId, ps => ps.ProductId,
                                                     (prd, ps) => new {
                                                         SupplierId = ps.SupplierId,
                                                         ProductId = prd.ProductId,
                                                         ProductSupplierId = ps.ProductSupplierId,

                                                     })
                                               .Where(ps => ps.ProductId == selectedID)
                                               .Join(db.Suppliers, ps => ps.SupplierId, s => s.SupplierId,
                                                     (ps, s) => new { SupplierId = s.SupplierId, SupplierName = s.SupName })
                                               .ToList();

                    lstProductSupplier.Items.Clear();

                    foreach (var supplier in productSuppliers)
                    {
                        lstProductSupplier.Items.Add(supplier.SupplierName);  // add all suppliers to listbox and display
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while retrieving the Suppliers:" + ex.Message, ex.GetType().ToString(),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //search all products
        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            txtSearchBox.Clear();
            LoadProducts();  // call the method that loads all the packages.
        }

        // return the searched product to the dgv
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtSearchBox))  // validate if user input is present on the txtbox
            {
                string searchString = txtSearchBox.Text; // capture the search string

                // clear current data if any.
                dgvProducts.DataSource = null;
                dgvProducts.Rows.Clear();
                dgvProducts.Columns.Clear();

                // initiate DB connection via EF
                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())
                    {
                        var searchedProduct = db.Products.Where(prd => prd.ProdName.Contains(searchString))
                                                  .Select(prd => new { prd.ProductId, prd.ProdName })
                                                  .ToList();

                        dgvProducts.DataSource = searchedProduct;  // assign retrieved objects as the datasource
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while displaying the products:" + ex.Message, ex.GetType().ToString(),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SetDatagridviewDisplay();  // call method to set the DGV interface
            }
        }


        // add new product
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //show add/modify form
            frmAddModProduct addModForm = new frmAddModProduct();
            addModForm.isAdd = true;

            DialogResult result = addModForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                currentProduct = addModForm.product; //assign the second form object to this object

                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())
                    {
                        if (addModForm.newSuppliers.Count != 0) // product has attached suppliers
                        {
                            // add product-supplier data to ProductsSuppliers table
                            foreach (var prodSupplier in addModForm.newSuppliers)
                            {
                                currentProduct.ProductsSuppliers.Add(new ProductsSupplier
                                {
                                    SupplierId = prodSupplier.SupplierId

                                });
                            }
                        }

                        db.Products.Add(currentProduct); // add the current of product
                        db.SaveChanges();

                        MessageBox.Show($"Product has been added successfully", "Product Add", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                }
                catch (DbUpdateException ex)
                {
                    this.HandleDbUpdateException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while adding a product: " + ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }

                LoadProducts(); //load the products to the DGV
            }
        }

        //handle DB exception
        private void HandleDbUpdateException(DbUpdateException ex)
        {
            // get the inner exception with potentially multiple errors
            SqlException innerException = (SqlException)ex.InnerException;
            string message = "";
            foreach (SqlError err in innerException.Errors)
            {
                message += $"Error {err.Number}: {err.Message}\n";
            }
            MessageBox.Show(message, "Database error(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        //product modify
        private void btnModify_Click(object sender, EventArgs e)
        {
            frmAddModProduct addModForm = new frmAddModProduct();
            addModForm.isAdd = false;

            var pid = Convert.ToInt32(dgvProducts.CurrentRow.Cells[0].Value); // get selected product id

            // instantiate new product obj using the selected row data.
            addModForm.product = new() { ProductId = pid, ProdName = dgvProducts.CurrentRow.Cells[1].Value.ToString() };

            try
            {
                using (TravelExpertContext db = new TravelExpertContext())
                {
                    //retrieve all suppliers attached to the product
                    var productSuppliers = db.ProductsSuppliers
                                               .Join(db.Products, ps => ps.ProductId, pr => pr.ProductId,
                                                     (ps, pr) => new {
                                                         SupplierId = ps.SupplierId,
                                                         ProductId = ps.ProductId,

                                                     })
                                               .Where(pr => pr.ProductId == pid) //use the product id to filter
                                               .Join(db.Suppliers, ps => ps.SupplierId, s => s.SupplierId,
                                                     (ps, s) => new {
                                                         SupplierId = ps.SupplierId,
                                                         SupName = s.SupName,

                                                     })
                                               .ToList();

                    // create supplier object from retrieve data
                    foreach (var ps in productSuppliers)
                    {
                        //add the suppliers to the existing supplier list.
                        addModForm.Suppliers.Add(new Supplier { SupplierId = Convert.ToInt32(ps.SupplierId), SupName = ps.SupName });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while retrieving supplier: " + ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            DialogResult result = addModForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                currentProduct = addModForm.product; //assign the second form object to  current product object

                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())
                    {
                        db.Entry(currentProduct).State = EntityState.Modified;//track the entity as modified

                        //since users cannot remove suppliers thats has been tagged to the product and save to the DB
                        // we only check  for new suppliers
                        if(addModForm.newSuppliers.Count != 0)
                        {
                            var suppliersAdded = addModForm.newSuppliers; //assign the modified/new list of suppliers

                            // add all supplier to the current product supplier entity
                            foreach (var sup in suppliersAdded)
                            {
                                var newPS = new ProductsSupplier { SupplierId = sup.SupplierId };
                                currentProduct.ProductsSuppliers.Add(newPS);
                            }
                        }

                        db.SaveChanges();

                        MessageBox.Show($"Product updated successfully", "Success Update", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        LoadProducts();
                    }
                }
                catch (DbUpdateException ex)
                {
                    this.HandleDbUpdateException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while modifying a product: " + ex.Message, ex.GetType().ToString(),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //To Maintain data integrity, removing for  products is not allowed.
        //Recommended to perform a data clean up exercise. Frequency and data to be removed should be decided by business/Travel Experts.
        //And should be included in the requirements.
        private void btnRemove_Click(object sender, EventArgs e)
        {


            MessageBox.Show("UNATHORIZED ACTION! Removing products can cause data integrity issue. \n" +
                 "Please contact administrator for more info.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            //below code can be use to delete product. But due to data integrity issue. It is not implemented.

            ////get the selected product id
            //int removeProductId = Convert.ToInt32(dgvProducts.CurrentRow.Cells[0].Value);

            //try
            //{
            //    using (TravelExpertContext db = new TravelExpertContext())
            //    {

            //        //get the product  object selected ID
            //        currentProduct = db.Products.Where(pr => pr.ProductId == removeProductId).FirstOrDefault();

            //        //get the product supplier object to be deleted
            //        var psDelete = db.ProductsSuppliers.Where(ps => ps.ProductId == removeProductId);

            //        //get the package product supplier object to be deleted
            //        var ppsToDelete = psDelete.SelectMany(p => p.PackagesProductsSuppliers);

            //        if (currentProduct != null)
            //        {
            //            // prompt user to confirm deletion of the data.
            //            DialogResult confirmDelete =
            //                MessageBox.Show($"Do you want to delete {currentProduct.ProdName}? \n" +
            //                                    $" NOTE: It will be deleted as package included product...",
            //                                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //            if (confirmDelete == DialogResult.Yes)
            //            {
            //                //delete data from the 3 tables
            //                db.PackagesProductsSuppliers.RemoveRange(ppsToDelete);
            //                db.ProductsSuppliers.RemoveRange(psDelete);
            //                db.Products.Remove(currentProduct);
            //                db.SaveChanges();  // save changes

            //                MessageBox.Show($"The operation completed successfully", "Success Delete", MessageBoxButtons.OK,
            //                                MessageBoxIcon.Exclamation);

            //                LoadProducts(); //load updated products data
            //            }
            //        }
            //    }
            //}
            //catch (DbUpdateException ex)
            //{
            //    this.HandleDbUpdateException(ex);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error while deleting product: " + ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK,
            //                    MessageBoxIcon.Error);
            //}
        }
    }
}
