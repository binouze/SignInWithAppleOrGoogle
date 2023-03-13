using System;
using System.Collections.Generic;
using System.Web;
using UnityEngine;

namespace com.binouze
{
    public class AppleSignIn : MonoBehaviour
    {
        #if UNITY_EDITOR
        private static string SUCCESS_RESPONSE => "{\"payload\":{\"sub\":\""+FAKE_UID+"\"}},\"user\":{\"email\":\"fake_user@gmail.com\",\"name\":{\"lastName\":\"FAKE\",\"firstName\":\"User\"}}";
        #endif
        
        public static string FAKE_UID;
        public static string CLIENT_ID;
        public static string SCOPE;

        private static Action<string,string,string,string> OnComplete;

        private bool IsInit;

        private static void Log( string val )
        {
            PluginLogger.Log( $"[AppleSignIn] {val}" );
        }
        private static void LogError( string val )
        {
            PluginLogger.LogError( $"[AppleSignIn] {val}" );
        }
        
        public static void ResetStatics()
        {
            OnComplete = null;
        }
        
        public void Init( string redirectURI )
        {
            if( IsInit )
                return;

            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.AppleSignIn");
            cls.CallStatic("init", CLIENT_ID, redirectURI, SCOPE, URL_SCHEME);
            #endif

            IsInit = true;
        }
        
        public void SignIn( Action<string,string,string,string> OnCompleteSignin )
        {
            if( !IsInit )
            {
                OnCompleteSignin?.Invoke( null,null,null,null );
                return;
            }
            
            OnComplete = OnCompleteSignin;
            
            #if UNITY_EDITOR
            OnAppleSignInResponse( SUCCESS_RESPONSE );
            #elif UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.AppleSignIn");
            cls.CallStatic("signIn");
            #elif UNITY_IOS
            //GoogleSignIn_Disconnect();
            #endif
        }
        
        public void CloseDialog()
        {
            #if UNITY_ANDROID
            using var cls = new AndroidJavaClass("com.binouze.AppleSignIn");
            cls.CallStatic("closeDialog");
            #elif UNITY_IOS
            //GoogleSignIn_CloseDialog();
            #endif
        }

        public void OnAppleSignInResponse( string response )
        {
            Log( $"OnAppleSignInResponse {response}" );

            string uid       = null;
            string email     = null;
            string firstName = null;
            string lastName  = null;
            var    obj       = Json.Deserialize( response );
            if( obj is Dictionary<string,object> dic )
            {
                uid       = dic.GetStringS( null, "payload", "sub" );
                firstName = dic.GetStringS( null, "user",    "name", "firstName" );
                lastName  = dic.GetStringS( null, "user",    "name", "lastName" );
                email     = dic.GetStringS( null, "user",    "email" );
            }
            else
            {
                LogError( "[AppleSignIn] OnAppleSignInResponse INVALID OBJECT" );
            }
            
            OnComplete?.Invoke( uid, email, firstName, lastName );
        }
        
        private static string      URL_SCHEME;
        private static AppleSignIn _instance;
        public static AppleSignIn GetInstance() 
        {
            if (_instance == null)
            {
                _instance = (AppleSignIn)FindObjectOfType( typeof(AppleSignIn) );
                if( _instance == null ) 
                {
                    const string goName = "[Lagoon-Utils]";          

                    var go = GameObject.Find( goName );
                    if( go == null ) 
                    {
                        go = new GameObject {name = goName};
                        DontDestroyOnLoad( go );
                    }
                    _instance = go.AddComponent<AppleSignIn>();
                    
                    var settings = SignInWithAppleOrGoogleSettings.LoadInstance();
                    URL_SCHEME   = settings == null ? "" : settings.APP_URL_SCHEME;
                    CLIENT_ID    = settings == null ? "" : settings.APPLECONNECT_CLIENT_ID;
                    SCOPE        = settings == null ? "" : HttpUtility.UrlEncode(string.Join(' ',settings.APPLECONNECT_SCOPE));
                }
            }
            return _instance;
        }
    }
}