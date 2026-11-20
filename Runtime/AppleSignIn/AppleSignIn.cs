using System;
using System.Collections.Generic;
using System.Net;
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

        /// <summary> code d'autorisation de la dernière réponse (null si absent) </summary>
        internal static string LastAuthorizationCode;
        /// <summary> identity token de la dernière réponse (null si absent) </summary>
        internal static string LastIdentityToken;
        /// <summary>
        /// pourquoi la dernière connexion a échoué (null si elle a réussi) : le message d'échec envoyé par le Java
        /// (« TAB HIDDEN » quand le joueur ferme l'onglet, « id token not found »…), ou ce qui manquait à la réponse
        /// </summary>
        internal static string LastFailMessage;
        /// <summary> message d'échec du Java quand le joueur a fermé l'onglet de connexion lui-même </summary>
        internal const string FAIL_TAB_HIDDEN = "TAB HIDDEN";

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
            OnComplete      = null;
            LastFailMessage = null;
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
                LastFailMessage = "AppleSignIn non initialisé";
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

            LastAuthorizationCode = null;
            LastIdentityToken     = null;
            LastFailMessage       = null;

            if( obj is Dictionary<string,object> dic )
            {
                // échec envoyé par le Java : {"result":"fail","message":"..."}
                if( dic.GetStringS( null, "result" ) == "fail" )
                    LastFailMessage = dic.GetStringS( null, "message" ) ?? "fail sans message";

                uid       = dic.GetStringS( null, "payload", "sub" );
                firstName = dic.GetStringS( null, "user",    "name", "firstName" );
                lastName  = dic.GetStringS( null, "user",    "name", "lastName" );
                email     = dic.GetStringS( null, "user",    "email" );

                // code d'autorisation et identity token (Services ID APPLECONNECT_CLIENT_ID comme client_id)
                LastAuthorizationCode = dic.GetStringS( null, "code" );
                LastIdentityToken     = dic.GetStringS( null, "id_token" );
            }
            else
            {
                LogError( "[AppleSignIn] OnAppleSignInResponse INVALID OBJECT" );
                LastFailMessage = "réponse illisible";
            }

            if( uid == null && LastFailMessage == null )
                LastFailMessage = "réponse sans identifiant Apple";

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
                    URL_SCHEME = settings == null ? "" : settings.APP_URL_SCHEME;
                    CLIENT_ID  = settings == null ? "" : settings.APPLECONNECT_CLIENT_ID;
                    SCOPE      = settings == null ? "" : WebUtility.UrlEncode(string.Join(' ',settings.APPLECONNECT_SCOPE));
                }
            }
            return _instance;
        }
    }
}