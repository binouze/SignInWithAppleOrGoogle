# SignInWithAppleOrGoogle

SignIn with Google or Apple Account for iOS and Android.

Sign in with Apple on iOS uses: https://github.com/lupidan/apple-signin-unity

## Installation

Choose your favourite method:

- **Plain install**
    - Clone or [download](https://github.com/binouze/SignInWithAppleOrGoogle/archive/refs/heads/master.zip) 
this repository and put it in the `Assets/Plugins` folder of your project.
- **Unity Package Manager (Manual)**:
    - Add the following line to *Packages/manifest.json*:
    - `"com.binouze.signinwithappleorgoogle": "https://github.com/binouze/SignInWithAppleOrGoogle.git"`
- **Unity Package Manager (Auto)**
    - in the package manager, click on the + 
    - select `add package from GIT url`
    - paste the following url: `"https://github.com/binouze/SignInWithAppleOrGoogle.git"`


## Android Configuration:

### Apple SignIn:

#### 1. Redirect Page

Apple send connection data to an URL ("Apple Connect Redirect URL" in the settings) as POST variables,
This page must redirect to the app and send all the params as GET variables.

Here is an example of this page in php:

```php
// get the POST datas and sent it back to the app in GET variables
$post    = http_build_query($_POST);
$linkurl = "pbauth://applesignin/?$post";
header('Location: '.$linkurl);
```


#### 2. AndroidManifest

Add this in the AndroidManifest.xml

Change the {REDIRECT_SCHEME} and {REDIRECT_HOST} to what is set in your redirect url. (pbauth and applesignin in the previous exemple.)

```xml
<!-- POUR LES REDIRECT APPLE SIGNIN -->
<activity android:name="com.binouze.AppleSignInActivity" android:launchMode="singleTop" android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen" android:exported="true">
  <!-- POUR LES REDIRECT APPLE SIGNIN -->
  <intent-filter>
    <action android:name="android.intent.action.VIEW" />
    <category android:name="android.intent.category.DEFAULT" />
    <category android:name="android.intent.category.BROWSABLE" />
    <data android:scheme="{REDIRECT_SCHEME}" android:host="{REDIRECT_HOST}" />
  </intent-filter>
</activity>
```

## How to use

Go to **LagoonPlugins > SignInWithAppleOrGoogle Settings** and fill the settings.


```csharp
    
    // GOOGLE
    
    // Google SignIn
    SignInWithAppleOrGoogle.Google_SignIn( (isConnected, user) =>
    {
        if( isConnected )
        {
            // user connected, get user info in user object
        }
        else
        {
            user not connected
        }
    }, 
    silent,       // if true this will not prompt the user to connect
    silentOnly ); // if false, after a silent connect fail, this wil try a non silent connection
    
    // Google SignOut
    SignInWithAppleOrGoogle.Google_SignOut( OnComplete );
    
    
    // APPLE
    
    // Apple SignIn
    SignInWithAppleOrGoogle.Apple_SignIn( (isConnected, user) =>
    {
        ConnectComplete( isConnected, user, OnComplete );
    }, 
    appleID ); // IOS only: optionnaly an AppleID to connect to, if defined, this will check for connection status
    
    // Apple SignOut
    SignInWithAppleOrGoogle.Apple_SignOut( OnComplete );
    
    
```


### Proguard rules to add

```
# google Credential manager
-if class androidx.credentials.CredentialManager
-keep class androidx.credentials.playservices.** {
*;
}
```
