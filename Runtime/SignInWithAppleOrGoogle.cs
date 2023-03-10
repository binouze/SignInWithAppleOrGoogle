using System;

namespace com.binouze
{
    public class SignInWithAppleOrGoogle
    {
        // -- RESET --

        public static void ResetAll()
        {
            // reset SignInWithApple variables and callbacks
            SignInWithApple.ResetStatics();
            // reset SignInWithGoogle variables and callbacks
            SignInWithGoogle.ResetStatics();
        }
        
        // -- LOGGING --
        
        private static bool LOGGINGENABLED;
        
        /// <summary>
        /// Enable or Disable Logs
        /// </summary>
        /// <param name="val"></param>
        public void SetLoggingEnabled( bool val )
        {
            PluginLogger.SetEnabled( val );
            
            #if !UNITY_EDITOR
                #if UNITY_ANDROID
                    using var cls = new AndroidJavaClass("com.lagoonsoft.GoogleSignInHelper");
                    cls.CallStatic("enableDebugLogging", val);
                #elif UNITY_IOS
                    GoogleSignIn_EnableDebugLogging(val);
                #endif
            #endif
        }
        
        // -- GOOGLE --

        /// <summary>
        /// Launch Google sign in process
        /// </summary>
        /// <param name="OnComplete"></param>
        /// <param name="silent"></param>
        /// <param name="silentOnly"></param>
        public static void Google_SignIn( Action<bool, GoogleSignInUser> OnComplete, bool silent = false, bool silentOnly = false )
        {
            SignInWithGoogle.SignIn( () =>
            {
                OnComplete?.Invoke( SignInWithGoogle.IsConnected, SignInWithGoogle.User );
            }, silent, silentOnly );
        }

        /// <summary>
        /// Sign out from Google
        /// </summary>
        /// <param name="OnComplete"></param>
        public static void Google_SignOut( Action OnComplete )
        {
            SignInWithGoogle.SignOut( OnComplete );
        }

        /*
        /// <summary>
        /// Disconnect from Google
        /// </summary>
        /// <param name="OnComplete"></param>
        public static void Google_Disconnect( Action OnComplete )
        {
            SignInWithGoogle.Disconnect( OnComplete );
        }
        */
        
        // -- APPLE

        /// <summary>
        /// Launch Apple sign in process
        /// </summary>
        /// <param name="OnComplete"></param>
        /// <param name="AppleID"></param>
        public static void Apple_SignIn( Action<bool, AppleSignInUser> OnComplete, string AppleID = null )
        {
            SignInWithApple.Connect( () =>
            {
                var user = new AppleSignInUser
                {
                    Email     = SignInWithApple.AppleIDEmail,
                    FirstName = SignInWithApple.AppleIDFName,
                    LastName  = SignInWithApple.AppleIDLName,
                    UserId    = SignInWithApple.AppleID
                };
                OnComplete?.Invoke( SignInWithApple.IsConnected, user );
            }, AppleID );
        }

        /// <summary>
        /// Disconnect from Apple
        /// </summary>
        /// <param name="OnComplete"></param>
        public static void Apple_SignOut( Action OnComplete )
        {
            SignInWithApple.Disconnect( OnComplete );
        }

        internal static Action OnTokenRevoked;

        public static void SetOnApplicationRevokedCallback( Action action )
        {
            OnTokenRevoked = action;
        }
    }
}