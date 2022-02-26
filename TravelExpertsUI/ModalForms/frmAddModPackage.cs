

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
    public partial class frmAddModPackage : Form
    {
        //declaring public varriables
        public bool isAdd;
        public Package package;
        public List<SelectedProdSup> selectedProdSup = new List<SelectedProdSup>(); //list of selected product & supplier data

        //declaring private variables
        private string selectedProduct;
        private string selectedSupplier;

        public frmAddModPackage()
        {
            InitializeComponent();
        }

        //Form load actions
        private void frmAddModPackage_Load(object sender, EventArgs e)
        {
            GetProducts(); // call method to populate product selection.

            if (isAdd)
            {
                this.Text = "Add Package"; // update the form text.
                //this.txtPackageID.Enabled = false; // disable packageID field as it will be auto generated in the DB.

            }
            else
            {
                this.Text = "View and Modify Package";  // update the form text.

                // load txtboxes data with the package data
                txtPackageID.Text = package.PackageId.ToString();
                txtName.Text = package.PkgName;
                txtDesc.Text = package.PkgDesc;
                dtpStartDate.Value = (DateTime)package.PkgStartDate;
                dtpEndDate.Value = (DateTime)package.PkgEndDate;
                txtBasePrice.Text = package.PkgBasePrice.ToString("N2");
                txtCommission.Text = package.PkgAgencyCommission.ToString("N2");

                GetPackagesProductSupplier(); 


            }
        }

        // get all packages product supplier data using the package id
        private void GetPackagesProductSupplier()
        {
            try
            {
                using (TravelExpertContext db = new TravelExpertContext())
                {
                    //query by joing 4 tables
                    var includedProdSupData = db.PackagesProductsSuppliers
                                .Join(
                                 db.ProductsSuppliers,
                                 pps => pps.ProductSupplierId,
                                 ps => ps.ProductSupplierId,
                                 (pps, ps) => new
                                 {
                                     ProductSupplierId = ps.ProductSupplierId,
                                     ProductId = ps.ProductId,
                                     PackageId = pps.PackageId,
                                     SupplierId = ps.SupplierId

                                 }

                                 )
                                .Where(pps => pps.PackageId == package.PackageId)
                                .Join(
                                 db.Products,
                                 ps => ps.ProductId,
                                 pr => pr.ProductId,
                                 (ps, pr) => new
                                 {
                                     ProductSupplierId = ps.ProductSupplierId,
                                     ProductId = ps.ProductId,
                                     ProductName = pr.ProdName,
                                     SupplierId = ps.SupplierId

                                 })
                                .Join(
                                 db.Suppliers,
                                 ps => ps.SupplierId,
                                 s => s.SupplierId,
                                 (ps, s) => new
                                 {
                                     ProductSupplierId = ps.ProductSupplierId,
                                     ProductId = ps.ProductId,
                                     ProductName = ps.ProductName,
                                     SupplierId = ps.SupplierId,
                                     SupplierName = s.SupName

                                 }).ToList();

                    foreach (var data in includedProdSupData)
                    {


                        //create an object with supplier data and product data using the helper class.
                        SelectedProdSup selected = new SelectedProdSup
                                                        (
                                                            data.ProductSupplierId,
                                                            Convert.ToInt32(data.ProductId),
                                                            Convert.ToInt32(data.SupplierId),
                                                            data.ProductName,
                                                            data.SupplierName

                                                         );

                        selectedProdSup.Add(selected); //add the data to the list.
                        ShowProdSup(); // show the product supplier of the package.

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while retrieving added selected product:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Show product supplier of the package
        /// </summary>

        private void ShowProdSup()
        {
            lstAddedProducts.DataSource = null; // clear listbox list.
            lstAddedProducts.DataSource = selectedProdSup.OrderBy(ps => ps.prodname).ToList(); // set listbox datasource to the list of product-supplier object to the 
            lstAddedProducts.SelectedIndex = -1; // set listbox selected to nothing.
        }


        /// <summary>
        /// Method that loads available product from the database.
        /// </summary>
        private void GetProducts()
        {
            // initiate connection to access data in DB
            try
            {
                using (TravelExpertContext db = new TravelExpertContext()) // instantiate context object
                {
                    //EF 
                    var products = db.Products.OrderBy(prod => prod.ProdName).ToList();
                    
                    foreach( var prod in products)
                    {
                        cboProduct.Items.Add(prod.ProdName); // add all product name in the combobox.
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the products:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!(cboProduct.SelectedIndex == -1))
                {
                    selectedProduct = null ; // select the current selected product to null.The variable will be reuse for each product selected.
                    selectedProduct = cboProduct.SelectedItem.ToString(); //assignt he selected product string to a variable

                    GetProductSupplier(); // get suppliers of the product

                }


        }

        /// <summary>
        /// Method that queries all configured supplier of the selected product
        /// </summary>
        /// <param name="selectedProduct"></param>
        private void GetProductSupplier()
        {

            try
            {
                using (TravelExpertContext db = new TravelExpertContext())
                {
                    var supplierList = db.Products
                                        .Join(
                                              db.ProductsSuppliers,
                                              prd => prd.ProductId,
                                              ps => ps.ProductId,
                                              (prd, ps) => new
                                              {
                                                
                                                  ProductId = ps.ProductId,
                                                  SupplierId = ps.SupplierId,
                                                  ProdName = prd.ProdName,
                                                  ProductsSupplierID = ps.ProductSupplierId
                                              }
                                           )
                                        .Where(prd => prd.ProdName == selectedProduct)
                                        .Join(
                                              db.Suppliers,
                                              ps => ps.SupplierId,
                                              s => s.SupplierId,
                                              (ps, s) => new
                                              {
                                                  SupplierId = s.SupplierId,
                                                  SupplierName = s.SupName,
                                                  ProductSupplierId = ps.ProductsSupplierID

                                              })
                                        .OrderBy(s => s.SupplierName)
                                        .ToList();


                    lstSuppliers.Items.Clear(); // clear suppliers list. This will remove the previous suppliers to accomodate new product supplier list.

                    foreach (var supplier in supplierList)
                    {
                        lstSuppliers.Items.Add(supplier.SupplierName); // add all suppliers to listbox and display
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the supplier:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //method to prevent error when user click on empty space in listbox
        private void lstSuppliers_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            if(lstSuppliers.SelectedItem != null)
            {
                selectedSupplier = null;
                selectedSupplier = lstSuppliers.SelectedItem.ToString();
            }
          
        }

        //clear all data to the input and selection controls
        private void btnClear_Click(object sender, EventArgs e)
        {

            if (!isAdd)
            {            
                // prompt user to confirm deletion of the data.
                DialogResult confirmClear = MessageBox.Show($"Are you sure on clearing all package data?",
                                       "Clear Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(confirmClear == DialogResult.Yes)
                {
                    formClear();
                }
            }
            else
            {

                formClear();
            }

           

        }

        
        //clear all txtboxes and list data.
        private void formClear()
        {
            txtBasePrice.Clear();
            txtCommission.Clear();
            txtDesc.Clear();
            txtName.Clear();

            dtpEndDate.CustomFormat = " ";
            dtpEndDate.Format = DateTimePickerFormat.Custom;

            dtpStartDate.CustomFormat = " ";
            dtpStartDate.Format = DateTimePickerFormat.Custom;

            clearProductSupplier();
            lstAddedProducts.DataSource = null;
        }

        //Method that set the Product and supplier selection control to default.
        private void clearProductSupplier()
        {
            cboProduct.SelectedIndex = -1;
            selectedProduct = null;

            //lstSuppliers.SelectedIndex = -1;
            lstSuppliers.Items.Clear();
            selectedSupplier = null;
        }

  
        /// <summary>
        /// Method that include selected products to the package.
        /// </summary>

        private void btnAdd_Click(object sender, EventArgs e)
        {

      
            if (selectedProduct != null && selectedSupplier != null) // validate that the user selected a product and a supplier.
            {
                if (Validator.isDuplicate(selectedProduct, selectedSupplier, lstAddedProducts.Items)) // validate to product selected to avoid duplicate.
                {
                
                    GetProductSupplierData();
                    ShowProdSup();


                }
                else
                {
                    // set focus depending on the missing data.
                    if(selectedProduct == null)
                    {
                        cboProduct.Focus();
                    }
                    else
                    {
                        lstSuppliers.Focus();
                    }
                   

                }
            }
            else
            {
                MessageBox.Show("Please select product and supplier to add.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


            clearProductSupplier();

            

        }


        /// <summary>
        /// Method that creates an object of the selected product and supplier
        /// </summary>

        private void GetProductSupplierData()
        {
         
            try
            {
                using (TravelExpertContext db = new TravelExpertContext())
                {
                    //retrieve supplier data based on the selected product
                    var selectedProdSupData = db.Products
                                        .Join(
                                              db.ProductsSuppliers,
                                              prd => prd.ProductId,
                                              ps => ps.ProductId,
                                              (prd, ps) => new
                                              {

                                                  ProductId = ps.ProductId,
                                                  SupplierId = ps.SupplierId,
                                                  ProdName = prd.ProdName,
                                                  ProductsSupplierID = ps.ProductSupplierId
                                              }
                                           )
                                        .Where(prd => prd.ProdName == selectedProduct)
                                        .Join(
                                              db.Suppliers,
                                              ps => ps.SupplierId,
                                              s => s.SupplierId,
                                              (ps, s) => new
                                              {
                                                  SupplierId = s.SupplierId,
                                                  SupplierName = s.SupName,
                                                  ProductSupplierId = ps.ProductsSupplierID,
                                                  ProductName = ps.ProdName,
                                                  ProductId = Convert.ToInt32(ps.ProductId)


                                              })
                                         .Where(s => s.SupplierName == selectedSupplier)
                                         .First();

                    //create an object with supplier data and product data
                    SelectedProdSup selected = new SelectedProdSup
                                                    (
                                                        selectedProdSupData.ProductSupplierId,
                                                        selectedProdSupData.ProductId,
                                                        selectedProdSupData.SupplierId,
                                                        selectedProdSupData.ProductName,
                                                        selectedProdSupData.SupplierName

                                                     );

                    selectedProdSup.Add(selected); //add the data to the list.

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error while retrieving the supplier:" + ex.Message, ex.GetType().ToString(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        

        }

        //Remove selected product supplier to the added product of the package.
        private void btnRemove_Click(object sender, EventArgs e)
        {
        
            if(lstAddedProducts.SelectedItem != null) // validate if a product is selected
            {
                // prompt user to confirm the action.
                DialogResult remove = MessageBox.Show("Are you sure to remove " + lstAddedProducts.SelectedItem + " ?",
                                       "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (remove == DialogResult.Yes)
                {

                    selectedProdSup.Remove((SelectedProdSup)lstAddedProducts.SelectedItem); // remove the selected product obj to the list.

                    // display updated list of products.
                    ShowProdSup();


                }
            }
            else
            {
                MessageBox.Show("Please select product to remove!", "Invalid Product", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            //cancel button setup
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (isValidInput()) // validate input
            {
                if (isAdd)
                {
                    this.package = new Package(); // instantiate package object
                }
                this.LoadPackageData(); // assign values to the package obj fields.
       
                this.DialogResult = DialogResult.OK; // set dialog result.
            }

        }


        //Method that validates the input on the textboxes.
        private bool isValidInput()
        {
            bool inputValid = true;

            if (!(Validator.IsPresent(txtName) && Validator.IsPresent(txtDesc) &&
                  Validator.isDateValid(dtpStartDate, dtpEndDate) && Validator.IsPresent(txtBasePrice) &&
                  Validator.IsNonNegativeDecimal(txtBasePrice) && Validator.IsPresent(txtCommission) &&
                  Validator.IsNonNegativeDecimal(txtCommission) && Validator.isPriceValid(txtBasePrice, txtCommission)

                ))
            {
                inputValid = false;
            }

            return inputValid;
        }

        /// <summary>
        /// Method that assign textbox data to the package object fields.
        /// </summary>
        private void LoadPackageData()
        {
            package.PkgName = txtName.Text;
            package.PkgDesc = txtDesc.Text;
            package.PkgStartDate = dtpStartDate.Value.Date;
            package.PkgEndDate = dtpEndDate.Value.Date;
            package.PkgBasePrice = Convert.ToDecimal(txtBasePrice.Text);
            package.PkgAgencyCommission = Convert.ToDecimal(txtCommission.Text);

        }

    }
}
