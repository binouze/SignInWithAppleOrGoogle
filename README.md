# SignInWithAppleOrGoogle

SignIn with Google or Apple Account for iOS and Android

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


## How to use

Go to **LagoonPlugins > SignInWithAppleOrGoogle Settings** and enter the required settings.

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