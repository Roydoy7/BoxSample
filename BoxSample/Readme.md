# box.com interop sample application
This is a blazor server side application used to demo how to grant access to box.com via OAuth.

## Steps of using this sample application
## 1 Create a box application at the box developer's webside
In box developer's webside, navigate to My Apps page, click "Create New App", select "Custom App",
then select "User Authentication (OAuth 2.0)", set the App Name and click "Create App" button.

## 2 Set the OAuth 2.0 Redirect URL
In the box app's detail settings page, click "Configuration" tab, scroll down to
"OAuth 2.0 Redirect URI", for development purpose, add "https://localhost/oauth/callback",
for production add webside's real URL. Click save changes to save.

## 3 Copy Client ID, Client Secret to appsettings.json
In the box app's detail settings page, click "Configuration" tab, scroll down to 
"OAuth 2.0 Credentials", copy Client ID and Client secret to the appsettings.json file,
which locates in the root folder of this sample application.


The appsettings.json may like this:

    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      },
      "box": {
        "AppId": "==Client ID HERE==",
        "AppSecret": "==Client Secret HERE=="
      },
      "AllowedHosts": "*"
    }

## 4 Use of the access token
After successful login from the box, access token can be obtained from the "AccessTokenService".

Inject service in the web page:

    @inject AccessTokenService TokenService

See the FetchData.razor page for detail.

Or inject in the code block:

    [Inject]
    private AccessTokenService TokenService { get; set; }

See the Counter.razor page for detail.

## 5 Upload file to box.com

After successfully get the access token. Please use the BoxHelper.cs locates in the Components folder to upload file to box.
For more box apis, goto https://developer.box.com/reference/ for further reference.

***This is a blazor server side application. Please open with Visual Studio 2019. Click "IIS Express" to run this application.***
