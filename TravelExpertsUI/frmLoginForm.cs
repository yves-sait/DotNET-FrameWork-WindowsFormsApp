

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using TravelExpertData;
using System.Security.Cryptography;

namespace TravelExpertsUI
{
    /// <summary>
    /// Blueprint for user login/authentication
    /// </summary>
    public partial class frmLoginForm : Form
    {

        public frmLoginForm()
        {
            InitializeComponent();
            pnlMain.BackColor = Color.FromArgb(125, Color.Black);

        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            //validate the txtboxes
            if(Validator.IsPresent(txtUsername) && Validator.IsPresent(txtPassword))
            {

                try
                {
                    using (TravelExpertContext db = new TravelExpertContext()) //create connection object
                    {

                        string inputPassword = txtPassword.Text;
                        string inputUsername = txtUsername.Text;

                        //get the agent object using the user username/email from  
                        var agent = db.Agents.FirstOrDefault(agent => agent.AgtEmail == inputUsername);

                        if (agent != null) // if agent exist
                        {
                            // create instance of the hashing algorithm
                            HashAlgorithm hashSHA512 = SHA512.Create();
                            byte[] dataInput = hashSHA512.ComputeHash(Encoding.UTF8.GetBytes(inputPassword)); //create hash of the input password

                            // check if password input match the save hashed password
                            if (PasswordsMatch(agent.AgtPassword, dataInput))
                            {
                                frmMainMenu mainMenu = new frmMainMenu(); // instantiate the main menu form
                                mainMenu.agentLogin = agent.AgtFirstName + " " + agent.AgtLastName; //passed the user data to main form
                      
                                mainMenu.Show();
                                this.Hide(); //hide login form
                            }
                            else
                            {
                                MessageBox.Show("Invalid Login Credentials! \n" +
                                    "Please enter correct  username and password.", "Login Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                    }
                }

                catch (Exception ex) //display messagebox when exception encountered.
                {
                    MessageBox.Show("Error during login credentials verification :" + ex.Message, ex.GetType().ToString(),
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        // check if the hash input match with the hash password
        // Credit to Stockoverflow on how to compare hashed password.
        private bool PasswordsMatch(byte[] psswd1, byte[] psswd2)
        {
            try
            {
                for (int i = 0; i < psswd1.Length; i++)
                {
                    if (psswd1[i] != psswd2[i])
                        return false;
                }
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

    }
}
