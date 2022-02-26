
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
    /// Blueprint for Supplier Entities
    /// </summary>
    public partial class frmMenuSupplier : Form
    {

        private Supplier currentSupplier; // variable to track current supplier

        private int selectedID;

        public frmMenuSupplier()
        {
            InitializeComponent();
        }

        //Load Suppliers to DGV
        private void frmMenuSupplier_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            // clear current data if any.
            dgvSuppliers.DataSource = null;
            dgvSuppliers.Rows.Clear();
            dgvSuppliers.Columns.Clear();

            // initiate connection to access data in DB
            try
            {
                using (TravelExpertContext db = new TravelExpertContext()) // instantiate context object
                {
                    //EF  query all suppliers
                    var suppliers = db.Suppliers.Select(s => new
                                            { s.SupplierId, s.SupName })
                                              .OrderBy(s => s.SupName)
                                              .ToList();

                    dgvSuppliers.DataSource = suppliers; // assign result as DGV datasource


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the suppliers:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetDatagridviewDisplay(); // Call method to configure DGV
            //buttonDisable();
        }

        // Set the DGV interface settings
        private void SetDatagridviewDisplay()
        {
            // header settings
            dgvSuppliers.EnableHeadersVisualStyles = false;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Regular);
            dgvSuppliers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSuppliers.ColumnHeadersHeight = 50;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //set selected row colow
            dgvSuppliers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);
            //set selected columns header colow
            dgvSuppliers.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);
            // hide the row header
            dgvSuppliers.RowHeadersVisible = false;
            // dgvSuppliers clicked on a cell, the full row will be selected.
            dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSuppliers.DefaultCellStyle.Font = new Font("Century Gothic", 9, FontStyle.Regular);
            dgvSuppliers.DefaultCellStyle.ForeColor = Color.Black;
            dgvSuppliers.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSuppliers.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //set header column text and formatting
            DataGridViewColumn column0 = dgvSuppliers.Columns[0];
            column0.Width = 80;
            dgvSuppliers.Columns[0].HeaderText = "ID";
            dgvSuppliers.Columns[1].HeaderText = "Supplier Name";

            dgvSuppliers.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // show the listbox of  supplier product
        private void dgvSuppliers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //set row height
            foreach (DataGridViewRow row in dgvSuppliers.Rows)
            {
                row.Height = 40;
            }
   

            lstSupplierProduct.Visible = true;
        }


        // enable modify and delete buttons only if a row/cell is selected.
        private void dgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {

            btnModify.Enabled = true;
            btnRemove.Enabled = true;

            selectedID = Convert.ToInt32(dgvSuppliers.CurrentRow.Cells[0].Value); // store the selected supplier id to variable

            GetProduct(); 
        }

        //retrieve all th products attached to the selected supplier.
        private void GetProduct()
        {
            try
            {

                using(TravelExpertContext db = new TravelExpertContext())  // instantiate context object
                {
                    // EF query all products that  attached to the selected supplier.
                    var products = db.Suppliers.Join(
                                                db.ProductsSuppliers,
                                                s => s.SupplierId,
                                                ps => ps.SupplierId,
                                                (s, ps) => new
                                                {
                                                    SupplierId = s.SupplierId,
                                                    ProductId = ps.ProductId,
                                                    ProductSupplierId = ps.ProductSupplierId
                                                })
                                                .Where(s => s.SupplierId == selectedID)
                                                .Join(
                                                  db.Products,
                                                   ps => ps.ProductId,
                                                   pr => pr.ProductId,
                                                   (ps, pr) => new
                                                   {
                                                       ProductId = pr.ProductId,
                                                       ProdName = pr.ProdName

                                                   })
                                              
                                                .ToList();

                    lstSupplierProduct.Items.Clear();
                    foreach(var prod in products)
                    {
                        lstSupplierProduct.Items.Add(prod.ProdName);// add all products to listbox and display
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while retrieving the Products:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // return the searched supplier to the dgv
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtSearchBox)) // validate if user input is present on the txtbox
            {
                string searchString = txtSearchBox.Text;

                // clear current data if any.
                dgvSuppliers.DataSource = null;
                dgvSuppliers.Rows.Clear();
                dgvSuppliers.Columns.Clear();


                // initiate DB connection via EF
                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())
                    {
                        var searchedSupplier = db.Suppliers.Where(s => s.SupName.Contains(searchString))
                                                  .Select(s => new
                                                  {
                                                      s.SupplierId,
                                                      s.SupName
                                                  })
                                                  .ToList();

                        dgvSuppliers.DataSource = searchedSupplier; // assign retrieved objects as the datasource

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while displaying the suppliers:" + ex.Message, ex.GetType().ToString(),
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SetDatagridviewDisplay(); // call method to set the DGV interface
                //buttonDisable();

            }
        }


        //search all products
        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            txtSearchBox.Clear();
            LoadSuppliers(); //call the method that loads all the packages. 
            //buttonDisable();

        }

        // add new supplier
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //show add/modify form
            frmAddModSupplier addModForm = new frmAddModSupplier();
            addModForm.isAdd = true;

            DialogResult result = addModForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                currentSupplier = addModForm.supplier; //assign the second form supplier object to this object

                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())
                    {
                        if (addModForm.newProducts.Count != 0) // supplier has attached product
                        {
                            // add product-supplier data to ProductsSuppliers table
                            foreach (var supProduct in addModForm.newProducts)  
                            {
                                currentSupplier.ProductsSuppliers.Add(new ProductsSupplier
                                {
                                    ProductId = supProduct.ProductId

                                });
                            }
                        }

                        db.Suppliers.Add(currentSupplier);// add the current of suppliers
                        db.SaveChanges();

                        MessageBox.Show($"Supplier has been added successfully",
                                    "Supplier Add", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
                catch (DbUpdateException ex)
                {

                    this.HandleDbUpdateException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while adding a supplier: " + ex.Message, ex.GetType().ToString(),
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadSuppliers();//load the suppliers to the DGV

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


        //supplier modify
        private void btnModify_Click(object sender, EventArgs e)
        {
            frmAddModSupplier addModForm = new frmAddModSupplier();
            addModForm.isAdd = false;

            var supId = Convert.ToInt32(dgvSuppliers.CurrentRow.Cells[0].Value); // get selected supplier id

            // instantiate new supplier obj using the selected row data.
            addModForm.supplier = new()
            {
                SupplierId = supId,
                SupName = dgvSuppliers.CurrentRow.Cells[1].Value.ToString()
            };
            try
            {
                using (TravelExpertContext db = new TravelExpertContext())
                {
                    //retrieve all products attached to the supplier
                    var supplierProducts = db.ProductsSuppliers.Join(
                                                      db.Suppliers,
                                                      ps => ps.SupplierId,
                                                      s => s.SupplierId,
                                                      (ps, s) => new
                                                      {
                                                        
                                                          ProductId = ps.ProductId,
                                                          SupplierId =  s.SupplierId

                                                      }
                                                      )
                                                  .Where(s => s.SupplierId == supId) //use the supplier ID to filter
                                                  .Join(
                                                     db.Products,
                                                     ps => ps.ProductId,
                                                     pr => pr.ProductId,
                                                     (ps, pr) => new
                                                     {
                                                         ProductId = ps.ProductId,
                                                         ProdName = pr.ProdName

                                                     }).ToList();

                    // create product object from the  retrieve data
                    foreach (var sp in supplierProducts)
                    {
                        //add the products to the existing product list.
                        addModForm.Products.Add(new Product
                        {
                            ProductId = Convert.ToInt32(sp.ProductId),
                            ProdName = sp.ProdName
                        });
                    }



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while retrieving products: " + ex.Message, ex.GetType().ToString(),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DialogResult result = addModForm.ShowDialog();

            if(result == DialogResult.OK)
            {
                currentSupplier = addModForm.supplier; //assign the second form object to  current supplier object

                try
                {
                    using(TravelExpertContext db = new TravelExpertContext())
                    {
                        db.Entry(currentSupplier).State = EntityState.Modified; //track the entity as modified.

                        //since users cannot remove products thats has been tagged to the product and save to the DB
                        // we only check for new products
                        var productsAdded = addModForm.newProducts;  

                        if(productsAdded.Count != 0)
                        {
                            // add all products to the ps entity

                            foreach (var supProd in productsAdded)
                            {
                                currentSupplier.ProductsSuppliers.Add(new ProductsSupplier
                                {
                                    ProductId = supProd.ProductId

                                });
                            }
                        }

                        //save changes
                        db.SaveChanges();
                        MessageBox.Show($"Supplier updated successfully",
                                           "Success Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSuppliers();

                    }
                }
                catch (DbUpdateException ex)
                {

                    this.HandleDbUpdateException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while modifying a supplier: " + ex.Message, ex.GetType().ToString(),
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //To Maintain data integrity, removing for  products is not allowed.
        //Recommend to perform a data clean up exercise. Frequency and data to be removed should be decided by business/Travel Experts.
        //And should be included in the requirements.
        private void btnRemove_Click(object sender, EventArgs e)
        {
            MessageBox.Show("UNATHORIZED ACTION! Removing products can cause data integrity issue. \n" +
            "Please contact administrator for more info.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            //below code can be use to delete supplier . But due to data integrity issue. It is not implemented.


            //get the selected supplier id
            //int removeSupplier = Convert.ToInt32(dgvSuppliers.CurrentRow.Cells[0].Value);

            //try
            //{
            //    using (TravelExpertContext db = new TravelExpertContext())
            //    {


            //        //get the supplier object selected id
            //        currentSupplier = db.Suppliers.Where(s => s.SupplierId == removeSupplier).FirstOrDefault();

            //        //get the product supplier object to be deleted
            //        var psDelete = db.ProductsSuppliers.Where(ps => ps.SupplierId == removeSupplier);

            //        //get the package product supplier object to be deleted
            //        var ppsToDelete = psDelete.SelectMany(p => p.PackagesProductsSuppliers);



            //        if (currentSupplier != null)
            //        {
            //            // prompt user to confirm deletion of the data.
            //            DialogResult confirmDelete = MessageBox.Show($"Do you want to delete {currentSupplier.SupName}? \n \n" +
            //                $" NOTE: It will be deleted as package included supplier...",
            //                                   "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //            if (confirmDelete == DialogResult.Yes)
            //            {

            //                //delete data from the 3 tables
            //                db.PackagesProductsSuppliers.RemoveRange(ppsToDelete);
            //                db.ProductsSuppliers.RemoveRange(psDelete);
            //                db.Suppliers.Remove(currentSupplier);
            //                db.SaveChanges(); // save changes 

            //                MessageBox.Show($"The operation completed successfully",
            //                                   "Success Delete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //                LoadSuppliers(); //load updated suppliers data
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
            //    MessageBox.Show("Error while deleting product: " + ex.Message, ex.GetType().ToString(),
            //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }
    }
    
}
