using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TDAmeritradeApiLogin
{
    public class TDApiHandler
    {
        public static HttpResponseMessage Login(string username, string password)
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--log-level=3");

            IWebDriver driver = new ChromeDriver(driverService, options);


            //https://auth.tdameritrade.com/auth?response_type=code&redirect_uri=https%3A%2F%2F127.0.0.1&client_id=SOROSAPI6%40AMER.OAUTHAP
            string loginUrl = String.Format("https://auth.tdameritrade.com/auth?response_type=code&redirect_uri={0}&client_id={1}", HttpUtility.UrlEncode(Constants.RedirectURL), HttpUtility.UrlEncode(Constants.AppID));
            string[] code;
            string realCode = "";

            driver.Navigate().GoToUrl(loginUrl);
            var usernameBox = driver.FindElement(By.Id("username"));
            var passwordBox = driver.FindElement(By.Id("password"));

            usernameBox.SendKeys(username);
            passwordBox.SendKeys(password);

            //Need to press 'Login' button and 'Allow' button
            driver.FindElement(By.Id("accept")).Click();
            driver.FindElement(By.Id("accept")).Click();

            var codeBox = driver.FindElement(By.Id("smscode"));

            if (codeBox != null)
            {
                TwoFactorDialog dialog = new TwoFactorDialog();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    codeBox.SendKeys(dialog.txtSmsCode.Text);
                }
                else
                {

                }

            }

            driver.FindElement(By.Id("accept")).Click();
            driver.FindElement(By.Id("accept")).Click();

            while (true)
            {
                try
                {
                    if (driver.Url.Contains("code"))
                    {
                        code = driver.Url.Split(new[] { "code=" }, StringSplitOptions.None);

                        if (!String.IsNullOrWhiteSpace(code[1].ToString()))
                        {

                            realCode = HttpUtility.UrlDecode(code[1].ToString());
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                catch
                {
                    break;
                }
            }

            driver.Close();

            string tokenUrl = "https://api.tdameritrade.com/v1/oauth2/token";

            var content = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"refresh_token", ""},
                {"access_type", "offline"},
                {"code", realCode},
                {"client_id", Constants.AppID},
                {"redirect_uri", Constants.RedirectURL},
            };

            var httpContent = new FormUrlEncodedContent(content);

            HttpResponseMessage tokenResponse = Constants.TDAmeritradeHttpClient.PostAsync(tokenUrl, httpContent).Result;
            var test = tokenResponse.RequestMessage.RequestUri;

            if (tokenResponse.IsSuccessStatusCode)
            {
                var obj = JObject.Parse(tokenResponse.Content.ReadAsStringAsync().Result);

                Constants.TDAmeritradeAPIToken = String.Format("Bearer {0}", (string)obj["access_token"]);
                Constants.RefreshToken = (string)obj["refresh_token"];

            }

            return tokenResponse;
        }

        public static HttpResponseMessage RefreshAuthorizationToken()
        {
            string tokenUrl = "https://api.tdameritrade.com/v1/oauth2/token";

            //Delete previous Authorization token if it exists
            if (Constants.TDAmeritradeHttpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                Constants.TDAmeritradeHttpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            values.Add(new KeyValuePair<string, string>("refresh_token", Constants.RefreshToken));
            values.Add(new KeyValuePair<string, string>("client_id", Constants.AppID));
            var content = new FormUrlEncodedContent(values);

            HttpResponseMessage tokenResponse = Constants.TDAmeritradeHttpClient.PostAsync(tokenUrl, content).Result;

            if (tokenResponse.IsSuccessStatusCode)
            {
                var obj = JObject.Parse(tokenResponse.Content.ReadAsStringAsync().Result);

                Constants.TDAmeritradeAPIToken = String.Format("Bearer {0}", (string)obj["access_token"]);

                if (!Constants.TDAmeritradeHttpClient.DefaultRequestHeaders.Contains("Authorization"))
                    Constants.TDAmeritradeHttpClient.DefaultRequestHeaders.Add("Authorization", Constants.TDAmeritradeAPIToken);

                return tokenResponse;
            }
            else
            {
                return tokenResponse;
            }

        }

        public static HttpResponseMessage GetAccountInformation()
        {
            string accountUrl = "https://api.tdameritrade.com/v1/accounts";

            if (RefreshAuthorizationToken().IsSuccessStatusCode)
            {
                HttpResponseMessage accountResponse = Constants.TDAmeritradeHttpClient.GetAsync(accountUrl).Result;

                if (accountResponse.IsSuccessStatusCode)
                {
                    string responseBody = accountResponse.Content.ReadAsStringAsync().Result;
                    List<Account.TradingAccount> items = JsonConvert.DeserializeObject<List<Account.TradingAccount>>(responseBody);

                    if (items.Count == 1) //Grabs first account, logic should be expanded if using multiple accounts.
                    {
                        Constants.Account = items[0];
                    }

                    return accountResponse;
                }
                else
                {
                    return accountResponse;
                }
            }
            else
            {
                return RefreshAuthorizationToken();
            }
        }
    }
}
