
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelExpertsUI
{
    /// <summary>
    /// a repository of user input validation methods for Windows Forms projects
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// validates if text box is not empty
        /// </summary>
        /// <param name="tb"> text box to validate</param>
        /// <returns>true if not empty and false if empty</returns>
        public static bool IsPresent(TextBox tb)
        {
            bool isValid = true;
            if(tb.Text == "") // empty
            {
                isValid = false;
                MessageBox.Show(tb.Tag + " is required", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tb.Focus();
            }
            return isValid;
        }






        /// <summary>
        /// validates if text box contains non-negative decimal value
        /// </summary>
        /// <param name="tb">text box to validate</param>
        /// <returns>true if valid and false if not</returns>
        public static bool IsNonNegativeDecimal(TextBox tb)
        {
            bool isValid = true;
            decimal result; // for TryParse
            if (!Decimal.TryParse(tb.Text, out result)) // TryParse returned false
            {
                isValid = false;
                MessageBox.Show(tb.Tag + " must be a number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tb.SelectAll(); // select all content for replacement
                tb.Focus();
            }
            else // it's decimal value, but could be negative
            {
                if (result < 0)
                {
                    isValid = false;
                    MessageBox.Show(tb.Tag + " must be 0 or positive number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tb.SelectAll(); // select all content for replacement
                    tb.Focus();
                }
            }
            return isValid;
        }

        /// <summary>
        /// validates if the start date and end date are valid.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>true if enddate is later than start date otherwise  false </returns>
        public static bool isDateValid(DateTimePicker startDate, DateTimePicker endDate)
        {
            bool isValid = true;

            if (startDate.Value.CompareTo(endDate.Value) > 0)
            {
                 isValid = false;
                MessageBox.Show("Package " + endDate.Tag + " must be later than package " + startDate.Tag, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
           
                startDate.Focus();
            }


            return isValid;
        }

        /// <summary>
        /// Validate that the base price and commission price are valid
        /// </summary>
        /// <param name="basePrice"></param>
        /// <param name="commissionPrice"></param>
        /// <returns>return true if valid else error prompt and false</returns>

        public static bool isPriceValid(TextBox basePrice, TextBox commissionPrice)
        {
            bool isValid = true;
            decimal basePriceValue, commissionPriceValue;
            Decimal.TryParse(basePrice.Text, out basePriceValue);
            Decimal.TryParse(commissionPrice.Text, out commissionPriceValue);
            if (!(basePriceValue > commissionPriceValue))
            {
                isValid = false;
                MessageBox.Show(commissionPrice.Tag + " cannot be greater than or equal to the " + basePrice.Tag, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                commissionPrice.SelectAll();
                commissionPrice.Focus();
            }

            return isValid;
        }

        
        /// <summary>
        /// Validate that no duplicate product-supplier is added to the package.
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="selectedSupplier"></param>
        /// <param name="addedProducts"></param>
        /// <returns>return true if valid else retrn an error</returns>

        public static bool isDuplicate(string selectedProduct, string selectedSupplier, ListBox.ObjectCollection addedProducts)
        {
            bool isValid = true;

            foreach( var productsupplier in addedProducts)
            {
                if (productsupplier.ToString() == selectedProduct+ " by " + selectedSupplier)
                {
                    isValid = false;
                    MessageBox.Show(selectedProduct +"-"+ selectedSupplier + " is already added in the package. Please select new product or supplier. \n",
                                                      "Invalid Product", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }

            }


            return isValid;

        }

        /// <summary>
        /// validates if text box contains decimal with 1 decimal place
        /// </summary>
        /// <param name="tb">text box to validate</param>
        /// <returns>true if valid and false if not</returns>
        public static bool IsDecimalFormatted(TextBox tb)
        {
            bool isValid = true;
            Regex regex = new Regex(@"^\d+\.\d{0,1}$"); // regex pattern
            if (!regex.IsMatch(tb.Text)) // if pattern NOT match the input
            {
                isValid = false;
                MessageBox.Show(tb.Tag + " must have 1 decimal place.\n Ex.(1.1)");
                tb.SelectAll(); // select all content for replacement
                tb.Focus();

            }

            return isValid;
        }



    }
}
