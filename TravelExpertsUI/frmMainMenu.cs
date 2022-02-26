



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelExpertsUI
{

    /// <summary>
    /// Blueprint for the main menu / tab selection form
    /// </summary>
    public partial class frmMainMenu : Form
    {

        //declare variables
        private Button activeButton;
        private Form activeForm;
        public string agentLogin;

        public frmMainMenu()
        {
            InitializeComponent();      
        }


        //selected button from menu
        private void SelectButton(object btnSender)
        {
            if(btnSender != null) 
            {
                //check if the button pressed is not the current active button
                if(activeButton != (Button)btnSender)
                {

                    DisableButton(); 
                    activeButton = (Button)btnSender; // set the clicked button as active button
                    //change button colors & font to show as active
                    activeButton.BackColor = Color.FromArgb(19, 154, 144);
                    activeButton.ForeColor = Color.Gainsboro;
                    activeButton.Font =  new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                    pnlHeader.BackColor = Color.FromArgb(19, 154, 144);


                }
            }
        }

        // method that changes the button's appearance
        // show button as disabled
        private void DisableButton()
        {
            foreach (Control passiveBtn in pnlMenu.Controls)
            {
              if (passiveBtn.GetType() == typeof(Button))
                {
                    //change color and font to show as the passive button
                    passiveBtn.BackColor = Color.FromArgb(3, 28, 30);
                    passiveBtn.ForeColor = Color.White;
                    passiveBtn.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

                }
            }

            
        }


        private void DisplayMenuForm(object btnSender, Form menuForm )
        {
            if(activeForm != null)
            {
                activeForm.Close();
            }

            SelectButton(btnSender);
            activeForm = menuForm;
            menuForm.TopLevel = false;
            menuForm.FormBorderStyle = FormBorderStyle.None;
            menuForm.Dock = DockStyle.Fill;

            this.pnlDisplayForm.Controls.Add(menuForm);
            this.pnlDisplayForm.Tag = menuForm;

            menuForm.BringToFront();
            menuForm.Show();

            lblHeaderTitle.Text = menuForm.Text;


        }

        //open & display package form menu
        private void btnPackages_Click(object sender, EventArgs e)
        {
            DisplayMenuForm(sender, new MenuForms.frmMenuPackage());
        }

        //open & display product form menu
        private void btnProducts_Click(object sender, EventArgs e)
        {
            DisplayMenuForm(sender, new MenuForms.frmMenuProduct());
        }


        //open   & display supplier form menu
        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            DisplayMenuForm(sender, new MenuForms.frmMenuSupplier());
        }

        //close opened form and back to home form
        private void btnHome_Click(object sender, EventArgs e)
        {
            if(activeForm != null)
            {
                activeForm.Close();
            }

            showHome();
        }

        //back to landing form. Update text title and back color.
        private void showHome()
        {
            DisableButton(); //no selected button by default
            lblHeaderTitle.Text = "Home";
            pnlHeader.BackColor = Color.FromArgb(8, 80, 90);
            activeButton = null;

        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            //confirm user for logout
            DialogResult confirmLogout = MessageBox.Show("All done! Time for a walk  and logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(confirmLogout == DialogResult.Yes)
            {
                this.Close(); //close the current form
                Application.OpenForms["frmLoginForm"].Close(); //close the login form
            }


        }

        //display the logged in user 
        private void pnlDisplayForm_Paint(object sender, PaintEventArgs e)
        {
            lblAgent.Text = agentLogin + " !";
        }
    }
}
