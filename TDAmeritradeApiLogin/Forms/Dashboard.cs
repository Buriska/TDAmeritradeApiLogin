using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDAmeritradeApiLogin
{
    public partial class Dashboard : Form
    {
        //Cancellation token
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        static CancellationToken token;

        #region Constructor
        public Dashboard()
        {
            InitializeComponent();
            AccountLogin();
        }
        #endregion

        #region Invoke Methods
        public void UpdateConstantsGrid()
        {
            if (InvokeRequired)
                Invoke(new Action(UpdateConstantsGrid));
            else
                try
                {
                    lblAccountNumber.Text = Constants.Account.securitiesAccount.accountId;
                }
                catch
                {

                }
        }
        public void UpdateConnectedLabel(string text)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(UpdateConnectedLabel), text);
            else
                lblConnectionStatus.Text = text;
        }
        #endregion

        private void AccountLogin()
        {
            HttpResponseMessage accountResponse = TDApiHandler.GetAccountInformation();
            if (accountResponse.IsSuccessStatusCode)
            {
                lblAccountNumber.Text = Constants.Account.securitiesAccount.accountId; //Grab Account ID to show we have successfully connected.

                //Start the authorization token refresh thread.
                Task.Factory.StartNew(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var capturedToken = token;

                        HttpResponseMessage authResponse = TDApiHandler.RefreshAuthorizationToken();
                        if (authResponse.IsSuccessStatusCode)
                        {
                            UpdateConnectedLabel("Connected");
                        }
                        else
                        {
                            UpdateConnectedLabel(String.Format("Authentication Refresh Error - {0}", authResponse.ReasonPhrase));
                        }

                        Thread.Sleep(1500000); //25 minutes, token expires every 30 minutes.
                    }

                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else
            {
                UpdateConnectedLabel(String.Format("Login Error - {0}", accountResponse.ReasonPhrase));
            }
        }
    }
}
