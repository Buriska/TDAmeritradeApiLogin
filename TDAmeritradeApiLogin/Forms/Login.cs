using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDAmeritradeApiLogin
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            lblError.Visible = false;

            HttpResponseMessage resp = TDApiHandler.Login(txtUsername.Text, txtPassword.Text);

            if (resp.IsSuccessStatusCode)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                btnLogin.Enabled = true;
                lblError.Visible = true;
                lblError.Text = resp.ReasonPhrase;
            }
        }
    }
}
