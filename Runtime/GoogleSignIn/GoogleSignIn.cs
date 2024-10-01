using System.Collections.Generic;

#if UNITY_ANDROID
using UnityEngine;
#elif UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace com.binouze
{
    using System;
    using UnityEngine;

    public class GoogleSignIn : MonoBehaviour
    {
        #if UNITY_EDITOR
        private static string SUCCESS_RESPONSE => "{\"result\":{\"Status\":0,\"Email\":\"fakeuser@gmail.com\",\"FamilyName\":\"FAKE\",\"UserId\":\""+FAKE_UID+"\",\"DisplayName\":\"User FAKE\",\"GivenName\":\"User\",\"PhotoUrl\":\"\"}}";
        #endif
           
        /// <summary>
        /// set it to false to sho the GoogleId Credential SignIn Card instead of the SignInWith Google Form.
        /// </summary>
        public static bool UseGoogleSignInFormOnAndroid = true;
        
        public static string FAKE_UID;
        #if !UNITY_ANDROID && !UNITY_IOS
        static GoogleSignIn() 
        {
            PluginLogger.LogError("This platform is not supported");
        }
        #endif

        #if UNITY_IOS
        [DllImport( "__Internal")]
        private static extern void GoogleSignIn_EnableDebugLogging(bool val);
        
        
        [DllImport( "__Internal")]
        private static extern void GoogleSignIn_Configure(  string clientID,
                                                            string webClientId,  
                                                            bool   requestAuthCode,
                                                            bool   forceTokenRefresh, 
                                                            bool   requestEmail,
                                                            bool   requestIdToken, 
                                                            bool   requestProfile,
                                                            string accountName );
        
        [DllImport("__Internal")]
        private static extern void GoogleSignIn_SignIn();
        
        [DllImport("__Internal")]
        private static extern void GoogleSignIn_SignInSilently();
        
        [DllImport("__Internal")]
        private static extern void GoogleSignIn_Signout();
        
        [DllImport("__Internal")]
        private static extern void GoogleSignIn_Disconnect();

        [DllImport("__Internal")]
        private static extern void GoogleSignIn_CloseDialog();
        #endif
        
        public static  Action<GoogleSignInUser> OnAuthenticationFinished;

        
        private static void Log( string val )
        {
            PluginLogger.Log( $"[GoogleSignIn] {val}" );
        }
        
        public static void SetLoggingEnabled( bool enabled )
        {
            #if !UNITY_EDITOR
                #if UNITY_ANDROID
                    using var cls = new AndroidJavaClass("com.lagoonsoft.GoogleSignInHelper");
                    cls.CallStatic("enableDebugLogging", enabled);
                #elif UNITY_IOS
                    GoogleSignIn_EnableDebugLogging(enabled);
                #endif
            #endif
        }

        public void SetConfiguration( GoogleSignInConfiguration configuration )
        {
            Log( "Calling SetConfiguration" );
            
            /*
      String ClientId,
      String webClientId,
      boolean requestAuthCode,
      boolean forceRefreshToken,
      boolean requestEmail,
      boolean requestIdToken,
      boolean requestProfile,
      String defaultAccountName,
      String urlScheme
            */
            
#if !UNITY_EDITOR
            #if UNITY_ANDROID

            using var cls = new AndroidJavaClass("com.binouze.GoogleSignInHelper");
            cls.CallStatic("configure",
                //configuration.ClientId,          // not used on Android
                configuration.WebClientId,
                UseGoogleSignInFormOnAndroid,
                //configuration.ForceTokenRefresh, // not used anymore since the migration to CredentialManager
                configuration.RequestEmail,
                configuration.RequestIdToken,
                configuration.RequestProfile
                // configuration.AccountName,      // not used anymore since the migration to CredentialManager
                // URL_SCHEME                      // not used anymore since the migration to CredentialManager
                );
            #elif UNITY_IOS
            GoogleSignIn_Configure( configuration.ClientId,
                                    configuration.WebClientId,
                                    configuration.RequestAuthCode,
                                    configuration.ForceTokenRefresh,
                                    configuration.RequestEmail,
                                    configuration.RequestIdToken,
                                    configuration.RequestProfile,
                                    configuration.AccountName );
            #endif
#endif
        }
        
        public void CloseDialog()
        {
            Log( "Calling CloseDialog" );
            
#if !UNITY_EDITOR
            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.GoogleSignInHelper");
            cls.CallStatic("closeDialog");
            #elif UNITY_IOS
            GoogleSignIn_CloseDialog();
            #endif
#endif
        }
        
        public void SignIn()
        {
            Log( "Calling SignIn" );
#if !UNITY_EDITOR
            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.GoogleSignInHelper");
            cls.CallStatic("signIn");
            #elif UNITY_IOS
            GoogleSignIn_SignIn();
            #endif
#else
            OnSignInResult( SUCCESS_RESPONSE );
#endif
        }

        public void SignInSilently()
        {
            Log( "Calling SignInSilently" );
#if !UNITY_EDITOR
            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.GoogleSignInHelper");
            cls.CallStatic("signInSilently");
            #elif UNITY_IOS
            GoogleSignIn_SignInSilently();            
            #endif
#else
            OnSignInResult( SUCCESS_RESPONSE );
#endif
        }
        
        public void SignOut()
        {
            Log( "Calling SignOut" );
            
#if !UNITY_EDITOR
            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.GoogleSignInHelper");
            cls.CallStatic("signOut");
            #elif UNITY_IOS
            GoogleSignIn_Signout();
            #endif
#else
            OnSignInResult( "{\"deco\":\"ok\"}" );
#endif
        }
        
        public void Disconnect()
        {
            Log( "Calling Disconnect" );
#if !UNITY_EDITOR
            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.GoogleSignInHelper");
            cls.CallStatic("disconnect");
            #elif UNITY_IOS
            GoogleSignIn_Disconnect();
            #endif
#else
            OnSignInResult( "{\"deco\":\"ok\"}" );
#endif
        }
        
        
        /// <summary>
        /// La methode appelee par le plugin natif pour renvoyer les resultats de login
        /// </summary>
        /// <param name="result"></param>
        public void OnSignInResult( string result )
        {
            Log( $"OnSignInResult Result: {result}" );
            
            var datas = Json.Deserialize( result );
            if( datas is Dictionary<string, object> dic )
            {
                var signedInUser = GoogleSignInUser.FromObject( dic.GetDictionary( "result" ) );
                OnAuthenticationFinished( signedInUser );
            }
            else
            {
                OnAuthenticationFinished( new GoogleSignInUser{Status = GoogleSignInStatusCode.Error} );
            }
        }

        private static string URL_SCHEME;

        private static GoogleSignIn _instance;
        public static GoogleSignIn GetInstance()
        {
            if( _instance == null )
            {
                _instance = (GoogleSignIn)FindObjectOfType( typeof(GoogleSignIn) );
                if( _instance == null )
                {
                    const string goName = "GoogleSignInHelperObject";
                    var          go     = GameObject.Find( goName );
                    if( go == null )
                    {
                        go = new GameObject { name = goName };
                        DontDestroyOnLoad( go );
                    }
                    _instance = go.AddComponent<GoogleSignIn>();
                    
                    var settings  = SignInWithAppleOrGoogleSettings.LoadInstance();
                    URL_SCHEME = settings == null ? "" : settings.APP_URL_SCHEME;
                }
            }

            return _instance;
        }
    }
}