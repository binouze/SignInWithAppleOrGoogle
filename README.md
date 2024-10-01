# SignInWithAppleOrGoogle

Unity plugin to SignIn with Google or Apple Account for iOS and Android.

Sign in with Apple on iOS uses: https://github.com/lupidan/apple-signin-unity

## PACKAGE INSTALLATION:

- in the package manager, click on the + 
- select `add package from GIT url`
- paste the following url: `"https://github.com/binouze/SignInWithAppleOrGoogle.git"`


## SET UP GOOGLE SIGN IN:

1. Set up your Google APIs console project for Android: https://developer.android.com/identity/sign-in/credential-manager-siwg#set-google
2. Set up your Google APIs console project for iOS: https://developers.google.com/identity/sign-in/ios/start-integrating#get_an_oauth_client_id
3. Go to **LagoonPlugins > SignInWithAppleOrGoogle Settings** and fill the settings.
    - **Web Client ID:** the client Id you created on the 1st step (Web App).
    - **iOS Client ID:** the client Id you created on step 2 (iOS).
    - **iOS Scheme:** is auto generated based on the iOS Client ID.


## (TODO) SET UP APPLE SIGN IN:

1. Set up your Google APIs console project for Android: https://developer.android.com/identity/sign-in/credential-manager-siwg#set-google
2. Set up your Google APIs console project for iOS: https://developers.google.com/identity/sign-in/ios/start-integrating#get_an_oauth_client_id
3. Go to **LagoonPlugins > SignInWithAppleOrGoogle Settings** and fill the settings.
    - **Android App URL Scheme:** the app url scheme, used to force close the login form when calling AppleSignIn.CloseDialog().
    - **Apple Connect Redirect URL:** see below.
    - **Apple Connect ClientID:** xxxxxxxxx.
    - **Apple Connect Scope:** xxxxxxxxx.
   

## ANDROID CONFIGURATION:

### Apple SignIn:

#### 1. Redirect Page

On Android, Apple sends connection data to an URL ("Apple Connect Redirect URL" in the settings) as POST variables,
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


### Proguard rules to add:

If you use proguard to minify your project you should add theses rules to be sure everything works.

```
# plugins com.binouze
-keep class com.binouze.** { *; } 

# google Credential manager
-if class androidx.credentials.CredentialManager
-keep class androidx.credentials.playservices.** {
*;
}
```


## HOW TO USE

Once configured, it's pretty simple to use:

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
    SignInWithAppleOrGoogle.Google_SignOut( () => 
    {
        // user is signed out
    } );
    
    
    // APPLE
    
    // Apple SignIn
    SignInWithAppleOrGoogle.Apple_SignIn( (isConnected, user) =>
    {
        ConnectComplete( isConnected, user, OnComplete );
    }, 
    appleID ); // IOS only: optionnaly an AppleID to connect to, if defined, this will check for connection status
    
    // Apple SignOut
    SignInWithAppleOrGoogle.Apple_SignOut( () => 
    {
        // user is signed out
    } );
    
    
```


