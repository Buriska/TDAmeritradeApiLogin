Winforms application to streamline login & token management for the TD Ameritrade API

This application automates the TD Ameritrade API Login process using Selenium to handle browser actions in the background. The application handles the login process, two-factor authentication, token refreshment, and TD Ameritrade account information. The application is intended to provide you with a template application that can be used to quickly get your automated trading up and running.

You will still need to create an account and generate an App ID on the API site: https://developer.tdameritrade.com/

Once you have your App ID, update the 'AppID' variable in the 'Constants' class.

A version of 'chromedriver' has been included in the Debug bin for version 83 of Chrome, you may need to update this driver to match your current version of Google Chrome.

The most recent version of the 'chromedriver.exe' can be found below: https://chromedriver.chromium.org/downloads
