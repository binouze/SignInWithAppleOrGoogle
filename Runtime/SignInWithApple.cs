using System;
using AppleAuth;
using UnityEngine;
#if UNITY_IOS
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif

namespace com.binouze
{
    public class SignInWithApple : MonoBehaviour
    {
        #if UNITY_ANDROID
        internal static bool IsSupported = true;
        #elif UNITY_IOS
        internal static bool IsSupported = AppleAuthManager.IsCurrentPlatformSupported;
        #else
        internal static bool IsSupported => false;
        #endif
        
        internal static  string           AppleID;
        internal static  string           AppleIDEmail;
        internal static  string           AppleIDFName;
        internal static  string           AppleIDLName;
        internal static bool              IsConnected { get; private set; }
        private         bool              MustUpdate;
        private         IAppleAuthManager appleAuthManager;

        private bool IsInit;
        
        private static void Log( string str )
        {
            PluginLogger.Log( $"[Apple] {str}" );
        }
        
        private void Init()
        {
            if( IsInit )
                return;
            IsInit = true;
            
            
            #if UNITY_IOS
            // If the current platform is supported
            if( IsSupported && !Application.isEditor )
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                appleAuthManager = new AppleAuthManager(deserializer);
                MustUpdate       = true;
                
                appleAuthManager.SetCredentialsRevokedCallback(result =>
                {
                    // deco -> reset game
                    OnTokenRevoked?.Invoke();
                });
            }
            #else
            var settings = SignInWithAppleOrGoogleSettings.LoadInstance();
            if( settings == null )
            {
                Debug.LogException( new Exception("[SignInWithAppleOrGoogle] APPLE Fail to load settings file") );
                return;
            }
            
            AppleSignIn.GetInstance().Init( settings.URL_APPLECONNECT_REDIRECT );
            #endif
        }

        internal static void ResetStatics()
        {
            #if UNITY_ANDROID
            AppleSignIn.ResetStatics();
            AppleSignIn.GetInstance().CloseDialog();
            #endif

            AppleID      = null;
            AppleIDEmail = null;
            AppleIDFName = null;
            AppleIDLName = null;
            IsConnected  = false;
        }
        
        #if UNITY_IOS
        private void Update()
        {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if( MustUpdate )
            {
                appleAuthManager.Update();
            }
        }
        #endif

        internal static void Connect( Action OnComplete, string appleID = null )
        {
            GetInstance()._Connect( OnComplete, appleID );
        }
        internal static void Disconnect( Action OnComplete )
        {
            GetInstance()._Disconnect( OnComplete );
        }

        private void _Disconnect( Action OnComplete )
        {
            OnComplete?.Invoke();
        }
        
        /// <summary>
        /// Lancer la connection a Apple ID
        /// </summary>
        /// <param name="OnComplete"></param>
        /// <param name="appleID"></param>
        private void _Connect( Action OnComplete, string appleID = null )
        {
            Log( $"Connect {appleID}" );

            if( Application.isEditor )
            {
                EditorHelper.ShowInputDialog( "connecte apple ?", "oui", "non",
                    fuid =>
                    {
                        AppleSignIn.FAKE_UID = fuid;
                        AppleSignIn.GetInstance().SignIn( (uid, email, firstname, lastname) =>
                        {
                            Log( $"ConnectComplete {uid}, {email}, {firstname}, {lastname}" );
                    
                            IsConnected  = uid != null;
                            AppleID      = uid;
                            AppleIDEmail = email;
                            AppleIDFName = firstname;
                            AppleIDLName = lastname;
                    
                            ConnectComplete( OnComplete );
                        });
                    },
                    () =>
                    {
                        OnComplete?.Invoke();
                    } );
                return;
            }

            #if UNITY_ANDROID
            if( !string.IsNullOrWhiteSpace(appleID) )
            {
                IsConnected   = true;
                AppleID       = appleID;
                AppleIDEmail  = null;
                AppleIDFName  = null;
                AppleIDLName  = null;
                ConnectComplete( OnComplete );
            }
            else
            {
                AppleSignIn.GetInstance().SignIn( (uid, email, firstname, lastname) =>
                {
                    Log( $"ConnectComplete {uid}, {email}, {firstname}, {lastname}" );
                    
                    IsConnected  = uid != null;
                    AppleID      = uid;
                    AppleIDEmail = email;
                    AppleIDFName = firstname;
                    AppleIDLName = lastname;
                    
                    ConnectComplete( OnComplete );
                });
            }
            #elif UNITY_IOS
            if( !IsSupported )
            {
                Log( "Connect IOS => NOT SUPPORTED" );
                OnComplete?.Invoke();
                return;
            }
            
            var instance = GetInstance();
            if( !appleID.IsNullOrEmpty() )
            {
                Log( $"Connect => CheckCredentials {appleID}" );
                instance.CheckCredentials( appleID, OnComplete );
            }
            else
            {
                Log( "Connect => QuickLogin" );
                instance.QuickLogin( OnComplete );
            }
            #else
            Log( "Connect PLATEFORM NOT SUPPORTED" );
            OnComplete?.Invoke();
            #endif
        }
        
        #if UNITY_IOS
        private void CheckCredentials( string userId, Action OnComplete )
        {
            appleAuthManager.GetCredentialState(
                userId,
                state =>
                {
                    if( state == CredentialState.Authorized )
                    {
                        AppleID     = userId;
                        IsConnected = true;
                        
                        Log( $"CheckCredentials OK {AppleID}" );
                        OnComplete?.Invoke();
                    }
                    else
                    {
                        Log( $"CheckCredentials FAIL {state}" );
                        QuickLogin( OnComplete );
                    }
                },
                error =>
                {
                    Log( $"CheckCredentials ERROR {error.LocalizedDescription}" );
                    QuickLogin( OnComplete );
                });
        }

        private void QuickLogin( Action OnComplete )
        {
            DoConnect( OnComplete );
        }
        
        private void DoConnect( Action OnComplete )
        {
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // Obtained credential, cast it to IAppleIDCredential
                    if( credential is IAppleIDCredential appleIdCredential )
                    {
                        AppleID       = appleIdCredential.User;
                        AppleIDEmail  = appleIdCredential.Email;
                        AppleIDFName  = appleIdCredential.FullName?.GivenName ?? "";
                        AppleIDLName  = appleIdCredential.FullName?.FamilyName ?? "";

                        IsConnected  = true;
                        
                        Log( $"DoConnect OK {AppleID}" );
                        ConnectComplete( OnComplete );
                    }
                    else
                    {
                        Log( $"[ConnectWithApple] DoConnect FAIL {credential}" );
                    }
                },
                error =>
                {
                    Log( $"[ConnectWithApple] DoConnect ERROR {error?.LocalizedDescription ?? "NO ERROR DESC"}");
                    
                    // Something went wrong
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Log( $"ConnectWithApple ERROR: {authorizationErrorCode}" );
                    IsConnected = false;
                    AppleID     = null;
                    ConnectComplete( OnComplete );
                });
        }
        #endif

        /// <summary>
        /// La connection à apple ID est terminée
        /// </summary>
        /// <param name="OnComplete"></param>
        private static void ConnectComplete( Action OnComplete )
        {
            Log( $"ConnectComplete {IsConnected} {AppleID}" );
            OnComplete.Invoke();
        }
        
        private void OnDestroy()
        {
            _instance = null;
        }

        private static SignInWithApple _instance;
        private static SignInWithApple GetInstance() 
        {
            if( _instance == null ) 
            {
                _instance.Init();
                _instance = (SignInWithApple)FindObjectOfType( typeof(SignInWithApple) );
                if( _instance == null ) 
                {
                    const string goName = "[Lagoon-Utils]";          

                    var go = GameObject.Find( goName );
                    if( go == null ) 
                    {
                        go = new GameObject {name = goName};
                        DontDestroyOnLoad( go );
                    }
                    _instance = go.AddComponent<SignInWithApple>();     
                    _instance.Init();
                }
            }
            return _instance;
        }
    }
}