using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TDAmeritradeApiLogin
{
    public class Constants
    {
        public static string AppID = "YOUR_ID_HERE@AMER.OAUTHAP"; //App ID generated on the TD Ameritrade API website goes here.
        public static string RedirectURL = "https://127.0.0.1"; //URL should be the same URL used when creating your App ID

        public static Account.TradingAccount Account;
        public static HttpClient TDAmeritradeHttpClient = new HttpClient();
        public static string TDAmeritradeAPIToken = "";
        public static string RefreshToken = "";
    }
}
