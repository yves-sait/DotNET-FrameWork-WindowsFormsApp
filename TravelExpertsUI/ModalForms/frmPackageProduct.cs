

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
    /// Blueprint for Products attached to Package
    /// </summary>
    public partial class frmPackageProduct : Form
    {

        // variable that holds the selected package id
        public int SelectedPackageId;
        public frmPackageProduct()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // load the products that are attached to the package
        private void frmPackageProduct_Load(object sender, EventArgs e)
        {

            try
            {
                using(TravelExpertContext db = new TravelExpertContext())
                {
                    // query the product by joining pps,ps, suppliers and products table
                    var productList = db.PackagesProductsSuppliers
                                        .Join(
                                              db.ProductsSuppliers,
                                              pps => pps.ProductSupplierId,
                                              ps => ps.ProductSupplierId,
                                              (pps, ps) => new
                                              {
                                                  PackageId = pps.PackageId,
                                                  ProductId = ps.ProductId,
                                                  ProdSupId = ps.ProductSupplierId,
                                                  SupplierId = ps.SupplierId
                                              }
                                           )
                                        .Where(i => i.PackageId == SelectedPackageId)
                                        .Join(db.Suppliers,
                                              ps => ps.SupplierId,
                                              s => s.SupplierId,
                                              (ps ,s) => new
                                              {
                                                  ProductId = ps.ProductId,
                                                  SupplierName = s.SupName
                                              })
                                        .Join(
                                              db.Products,
                                              ps => ps.ProductId,
                                              prd => prd.ProductId,
                                              (ps, prd) => new
                                              {
                                                  SupplierName = ps.SupplierName,
                                                  ProductName = prd.ProdName,

                                              })
                                        .OrderBy(prdName => prdName.ProductName)
                                        .ToList();
                   

                    lstProducts.Items.Clear();

                    // iterate to the query result and add to listbox
                    foreach (var p in productList)
                    {
                        lstProducts.Items.Add(p.ProductName +" BY "+p.SupplierName);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while displaying the products:" + ex.Message, ex.GetType().ToString(),
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
     

        }
    }
}
