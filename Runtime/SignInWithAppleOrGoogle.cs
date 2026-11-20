using System;
using UnityEngine;

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
        public static void SetLoggingEnabled( bool val )
        {
            PluginLogger.SetEnabled( val );
            GoogleSignIn.SetLoggingEnabled( val );
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
        /// Supported status of the plugin, on old iOS devices the sign in with Apple does not work
        /// </summary>
        public static bool Apple_Supported => SignInWithApple.IsSupported;
        
        /// <summary>
        /// Launch Apple sign in process
        /// </summary>
        /// <param name="OnComplete"></param>
        /// <param name="AppleID">if set, only checks the existing credential when possible (no Apple sheet, no authorization code).
        /// Leave null to always show the Apple sheet (re-authentication) and get <see cref="AppleSignInUser.AuthorizationCode"/>.</param>
        public static void Apple_SignIn( Action<bool, AppleSignInUser> OnComplete, string AppleID = null )
        {
            SignInWithApple.Connect( () =>
            {
                var user = new AppleSignInUser
                {
                    Email             = SignInWithApple.AppleIDEmail,
                    FirstName         = SignInWithApple.AppleIDFName,
                    LastName          = SignInWithApple.AppleIDLName,
                    UserId            = SignInWithApple.AppleID,
                    AuthorizationCode = SignInWithApple.AppleIDAuthorizationCode,
                    IdentityToken     = SignInWithApple.AppleIDIdentityToken
                };
                OnComplete?.Invoke( SignInWithApple.IsConnected, user );
            }, AppleID );
        }

        /// <summary>
        /// The last Apple sign in was canceled by the user himself (Apple sheet or sign in tab closed): a choice, not an error.
        /// </summary>
        public static bool Apple_LastCanceled => SignInWithApple.LastCanceled;

        /// <summary>
        /// Error code of the last Apple sign in: on iOS the Apple AuthorizationErrorCode (1001 = canceled, 1000 = unknown,
        /// 1004 = failed...), -1 for an error without code (Android, unsupported platform...), 0 when there was no error.
        /// </summary>
        public static int Apple_LastErrorCode => SignInWithApple.LastErrorCode;

        /// <summary>
        /// Description of the last Apple sign in error (null when there was no error).
        /// </summary>
        public static string Apple_LastErrorDescription => SignInWithApple.LastErrorDescription;

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