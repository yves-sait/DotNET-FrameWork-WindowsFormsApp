

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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
    /// Blueprint for Package entities.
    /// </summary>
    public partial class frmMenuPackage : Form
    {
        //private variable to keeptrack of current package
        private Package currentPackage;

        public frmMenuPackage() { InitializeComponent(); }

        /// <summary>
        /// Event that will load all Packages on the initial form load
        /// </summary>
        private void MenuPackageForm_Load(object sender, EventArgs e)
        {
            LoadPackages();  // call method to load the packages.
        }

        private void LoadPackages()
        {
            // clear current data if any.
            dgvPackages.DataSource = null;
            dgvPackages.Rows.Clear();
            dgvPackages.Columns.Clear();

            // initiate connection to access data in DB
            try
            {
                using (TravelExpertContext db = new TravelExpertContext())  // instantiate context object
                {
                    // EF
                    var packages = db.Packages
                                       .Select(p => new {
                                           p.PackageId,
                                           p.PkgName,
                                           p.PkgDesc,
                                           p.PkgStartDate,
                                           p.PkgEndDate,
                                           p.PkgBasePrice,
                                           p.PkgAgencyCommission
                                       })
                                       .ToList();
                    dgvPackages.DataSource = packages;  // assign the object retrieved as the Datagridview Data Source
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the packages:" + ex.Message, ex.GetType().ToString(),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetDatagridviewDisplay();  // Call method to configure DGV

            SetProductColumn();  // call method to add product column
        }

        /// <summary>
        ///  Method to set the DGV interface settings
        /// </summary>
        private void SetDatagridviewDisplay()
        {
            // header settings
            dgvPackages.EnableHeadersVisualStyles = false;
            dgvPackages.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Regular);
            dgvPackages.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPackages.ColumnHeadersHeight = 50;
            dgvPackages.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 23, 136, 137);
            dgvPackages.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // set selected row colow
            dgvPackages.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 181, 173);

            // set selected columns header colow
            dgvPackages.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 23, 136, 137);

            // hide the row header
            dgvPackages.RowHeadersVisible = false;
            // when clicked on a cell, the full row will be selected.
            dgvPackages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvPackages.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);

            dgvPackages.DefaultCellStyle.ForeColor = Color.Black;

            // format the columns width.
            // set static width for Name column to handle longer Name and Desc for better display instead of autofilling.
            // Other columns will autofill gridsize.
            DataGridViewColumn column0 = dgvPackages.Columns[0];
            column0.Width = 50;
            DataGridViewColumn column1 = dgvPackages.Columns[1];
            column1.Width = 250;
            DataGridViewColumn column2 = dgvPackages.Columns[2];
            column2.Width = 400;
            dgvPackages.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvPackages.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvPackages.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvPackages.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvPackages.Columns[5].DefaultCellStyle.Format = "c";
            dgvPackages.Columns[6].DefaultCellStyle.Format = "c";

            dgvPackages.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // set header column text and formatting
            dgvPackages.Columns[0].HeaderText = "ID";
            dgvPackages.Columns[1].HeaderText = "Name";
            dgvPackages.Columns[2].HeaderText = "Description";
            dgvPackages.Columns[3].HeaderText = "Start Date";
            dgvPackages.Columns[4].HeaderText = "End Date";
            dgvPackages.Columns[5].HeaderText = "Base Price";
            dgvPackages.Columns[6].HeaderText = "Agency Commission";
        }

        /// <summary>
        /// Method that add the product column to the Datagridview
        /// </summary>
        private void SetProductColumn()
        {
            //set product column DGV settings
            var columnProduct = new DataGridViewButtonColumn() 
            {   UseColumnTextForButtonValue = true, 
                HeaderText = "Product", 
                Text = "View"
            };

            dgvPackages.Columns.Insert(0, columnProduct);
            DataGridViewColumn columnP = dgvPackages.Columns[0];
            columnP.Width = 100;
        }

        // clear the default selected row and disable modify & delete button
        private void clearRowSelected()
        {
            dgvPackages.ClearSelection();

            // disable modify and delete button if no row selected
            btnModify.Enabled = false;
            btnRemove.Enabled = false;
        }


        /// <summary>
        /// Method that search packages based on the input by the user.
        /// </summary>

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtSearchBox))  // validate if user input is present on the txtbox
            {
                string searchString = txtSearchBox.Text;

                // clear current data if any.
                dgvPackages.DataSource = null;
                dgvPackages.Rows.Clear();
                dgvPackages.Columns.Clear();

                // initiate DB connection via EF
                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())
                    {
                        var packages = db.Packages.Where(p => p.PkgName.Contains(searchString))
                                           .Select(p => new {
                                               p.PackageId,
                                               p.PkgName,
                                               p.PkgDesc,
                                               p.PkgStartDate,
                                               p.PkgEndDate,
                                               p.PkgBasePrice,
                                               p.PkgAgencyCommission
                                           })
                                           .ToList();

                        dgvPackages.DataSource = packages;  // assign retrieved objects as the datasource
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while displaying the packages:" + ex.Message, ex.GetType().ToString(),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SetDatagridviewDisplay();  // call method to set the DGV interface
                SetProductColumn();
                clearRowSelected();
            }
        }

        // enable modify and delete buttons only if a row/cell is selected.
        private void dgvPackages_SelectionChanged(object sender, EventArgs e)
        {
            btnModify.Enabled = true;
            btnRemove.Enabled = true;
        }

        // After data has bound to the DGV. clear selected and disable modify and delete button.
        private void dgvPackages_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // set row height
            foreach (DataGridViewRow row in dgvPackages.Rows)
            {
                row.Height = 40;
            }
            clearRowSelected();
        }


        //Reload the DGV with all the data
        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            txtSearchBox.Clear();
            LoadPackages();  // call the method that loads all the packages.
            clearRowSelected();
        }

        /// <summary>
        /// Button method to add packages and package products
        /// </summary>

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddModPackage addModForm = new frmAddModPackage();  // instantiate a secondary form object
            addModForm.isAdd = true;                               // set field to true for adding new package
            DialogResult result = addModForm.ShowDialog();         // display form for use to enter package data

            if (result == DialogResult.OK)  // if user confirm on adding new package data
            {
                currentPackage = addModForm.package;  // assign package object from 2nd form to this package object.

                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())  // instantiate DB connection and create db context.
                    {
                        if (addModForm.selectedProdSup.Count() != 0)  // check if user added product(s) to package created.
                        {
                            foreach (var prodSupId in addModForm.selectedProdSup)  // iterate on the added product obj
                            {
                                // create PackagesProductsSupplier obj using ProductSupplierId of the added product obj of the 2nd form/
                                PackagesProductsSupplier pps = new() { ProductSupplierId = prodSupId.prodsupid };

                                //  selectedProductSupplier.Add(pps); // add the created PackagesProductsSupplier to the collection of
                                //  PackagesProductsSupplier on the current instance.
                                currentPackage.PackagesProductsSuppliers.Add(pps);
                            }

                            // assign the collection of created PackagesProductsSupplier to this
                            // Package object's PackagesProductsSupplier collection.
                        }

                        db.Packages.Add(currentPackage);  // add the package to Packages entity
                        db.SaveChanges();                 // save changes. Add package data to the DB.

                        MessageBox.Show($"Package has been added successfully", "Package Add", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        LoadPackages();  // Display updated packages data
                        clearRowSelected();

                        // set second form objects to null.
                        addModForm.selectedProdSup = null;
                        addModForm.package = null;
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
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            frmAddModPackage addModForm = new frmAddModPackage();
            addModForm.isAdd = false;
            
            // create package object using the dgv selected data
            addModForm.package = new()
            {

                PackageId = Convert.ToInt32(dgvPackages.CurrentRow.Cells[1].Value),
                PkgName = dgvPackages.CurrentRow.Cells[2].Value.ToString(),
                PkgDesc = dgvPackages.CurrentRow.Cells[3].Value.ToString(),
                PkgStartDate = DateTime.Parse(dgvPackages.CurrentRow.Cells[4].Value.ToString()),
                PkgEndDate = DateTime.Parse(dgvPackages.CurrentRow.Cells[5].Value.ToString()),
                PkgBasePrice = Convert.ToDecimal(dgvPackages.CurrentRow.Cells[6].Value),
                PkgAgencyCommission = Convert.ToDecimal(dgvPackages.CurrentRow.Cells[7].Value)

            };

            DialogResult result = addModForm.ShowDialog();

            if (result == DialogResult.OK)  // if user confirm on adding new package data
            {
                currentPackage = addModForm.package;  // assign package object from 2nd form to this package object.

                try
                {
                    using (TravelExpertContext db = new TravelExpertContext())  // instantiate DB connection and create db context.
                    {
                        db.Entry(currentPackage).State = EntityState.Modified;  // track changes to the package object
                                                                                // get current packages product supplier object
                        var currentPPS =
                            db.PackagesProductsSuppliers.Where(pps => pps.PackageId == currentPackage.PackageId).ToList();

                        db.PackagesProductsSuppliers.RemoveRange(currentPPS);

                        if (addModForm.selectedProdSup.Count() != 0)
                        {
                            foreach (var prodSupId in addModForm.selectedProdSup)  // iterate on the added product obj
                            {
                                // create PackagesProductsSupplier obj using ProductSupplierId of the added product obj of the 2nd form/
                                PackagesProductsSupplier pps = new() { ProductSupplierId = prodSupId.prodsupid };

                                // selectedProdSup.Add(pps); // add the created PackagesProductsSupplier to the collection of
                                // PackagesProductsSupplier on the current instance. MessageBox.Show(pps.ProductSupplierId.ToString());

                                currentPackage.PackagesProductsSuppliers.Add(pps);
                            }
                        }

                        db.SaveChanges();  // save changes. Add package data to the DB.

                        MessageBox.Show($"Package updated successfully", "Success Update", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        LoadPackages();  // Display updated packages data
                        clearRowSelected();

                        // set second form objects to null.
                        addModForm.selectedProdSup = null;
                        addModForm.package = null;
                    }
                }
                catch (DbUpdateException ex)
                {
                    this.HandleDbUpdateException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while adding a package: " + ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

            // get the package id of the selected row
            int removePackageId = Convert.ToInt32(dgvPackages.CurrentRow.Cells[1].Value);

            try
            {
                using (TravelExpertContext db = new TravelExpertContext())  // instantiate the db context
                {
                    currentPackage = db.Packages.Where(p => p.PackageId == removePackageId)
                                         .FirstOrDefault();  // query the product obj to be deleted via the selectedproduct code.

                    var pps = db.PackagesProductsSuppliers.Where(pps => pps.PackageId == removePackageId);

                    if (currentPackage != null)  // additional validation. proceed if the data selected exist in the db.
                    {
                        // prompt user to confirm deletion of the data.
                        DialogResult confirmDelete =
                            MessageBox.Show($"Do you want to delete {currentPackage.PkgName}?", "Confirm Delete",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (confirmDelete == DialogResult.Yes)  // if user confirm proceed to delete the data
                        {
                            db.PackagesProductsSuppliers.RemoveRange(pps);
                            db.Packages.Remove(currentPackage);  // delete the selected obj
                            db.SaveChanges();                    // save changes

                            MessageBox.Show($"The operation completed successfully", "Success Delete", MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);

                            LoadPackages();  // Display updated packages data
                            clearRowSelected();
                        }
                    }
                }

            }  // catch exceptions and display message accordingly to user
            catch (DbUpdateException ex)
            {
                this.HandleDbUpdateException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting package: " + ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

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

        // Event to handle cellclick on the product column
        private void dgvPackages_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // get the package id of the selected row
            int selectedPackageId = Convert.ToInt32(dgvPackages.CurrentRow.Cells[1].Value);

            //if cell click is the product column
            if (e.ColumnIndex == 0)
            {
                //open second form
                frmPackageProduct formProduct = new frmPackageProduct();
                formProduct.SelectedPackageId = selectedPackageId;
                formProduct.ShowDialog();
            }

        }
    }
}
