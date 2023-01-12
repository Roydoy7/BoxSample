# This is a sample to demostrate AutoCAD C# plugin interop with box.com

## How to use
Build this program with Visual Studio, start AutoCAD and add build output folder to trusted path,
type NETLOAD command in editor to load the built dll.

After successfully load the dll, 3 commands are added:
* BoxLogin
* BoxUpload
* BoxDownload

## Commands
This sample provider 3 commands:

### 1.BoxLogin

Provide an user interface to login in to the box.com. Before login a Custom App must be added use box.com's developer's console.

### 2.BoxUpload

Provide an user interface to upload local file to the box.com.
If the file is larger than 50MB, upload will process in chunks.
### 3.BoxDownload

Provide an user interface to download file from box.com to local storage.

## AccessToken

Once logged in AccessToken is saved in a json file with DES encryption.

## RefreshToken

A timer is set, when the AccessToken is expired, will try to obtain a new 
token with the RefreshToken. If failed the current token's json file will be deleted.

## Program Entry
The program entry is BoxSample.cs, which has a Initialize function.
BoxModule's Init function is called here.

## Config box.com's Custom App
Log in to box.com's developer's console, create a custom app with OAuth2.0 authentication method, 
click Configuration tab, scroll down to 'OAuth 2.0 Credentials' secion, copy Client ID, Client secret 
and paste into BoxAuthCredential.json.

To download or upload file to box.com, check 'Write all files and folders stored in Box' at the Application Scopes section.

### Settings of Redirect URIs
A redirect URL must be set when logining to box.com.
For desktop application, the redirect URL can be this:
```
https://localhost/oauth/callback
```

## BoxAuthCredential.json
Make sure this file is located in the same folder with the dll.

## WebView2
WebView2 is needed to display Box.com's login interface. 
Please note that if the WebView2's user data folder doesn't have write permition,
WebView2 can't create necessary files at runtime, this will cause WebView2 fail to initialize and Box's login UI will not display as expected.

The default user data location has been changed as follows in BoxAuthForm.xaml.cs, 
modify the following code to change to another location: 

```
private async Task InitWebView2()
{
    //Change WebView2's user data folder to a custom location,
    //if the user doesn't have admin privilege, the standard location will cause
    //webview2 fail to initialize.
    var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(AppDataPath.GetAppDataPath(), "WebView2"));
    await webView2.EnsureCoreWebView2Async(env);
}
 ```