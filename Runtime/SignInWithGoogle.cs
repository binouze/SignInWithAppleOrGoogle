using System;
using UnityEngine;

namespace com.binouze
{
    internal static class SignInWithGoogle
    {
        private static Action OnSignInResponse;

        internal static bool             IsConnected;
        internal static GoogleSignInUser User;

        static SignInWithGoogle()
        {
            var settings = SignInWithAppleOrGoogleSettings.LoadInstance();
            if( settings == null )
            {
                Debug.LogException( new Exception("[SignInWithAppleOrGoogle] GOOGLE Fail to load settings file") );
                return;
            }
            
            var configuration = new GoogleSignInConfiguration
            {
                WebClientId    = settings.SignInWithGoogleWebClientID,
                ClientId       = settings.SignInWithGoogleClientID,
                RequestProfile = true,
                RequestEmail   = true
            };
            GoogleSignIn.GetInstance().SetConfiguration( configuration );
            GoogleSignIn.OnAuthenticationFinished = OnAuthenticationFinished;
            ResetStatics();
        }

        internal static void ResetStatics()
        {
            Log( "ResetStatics" );
            
            User               = null;
            IsConnected        = false;
            IsSilentSignIn     = false;
            IsSilentSignInOnly = true;
            OnSignInResponse   = null;
            
            GoogleSignIn.GetInstance().CloseDialog();
        }

        private static void Log( string str )
        {
            PluginLogger.Log( $"[Google] {str}" );
        }

        private static bool IsSilentSignIn;
        private static bool IsSilentSignInOnly;
        
        /// <summary>
        /// Lancer la connexion Google
        /// </summary>
        internal static void SignIn( Action OnComplete, bool silent = false, bool silentOnly = false )
        {
            if( IsConnected && User != null )
            {
                // deja connecte
                OnComplete?.Invoke();
            }
            else
            {
                Log( "Calling SignIn" );
                
                if( Application.isEditor )
                {
                    EditorHelper.ShowInputDialog( "connecte google ?", "oui", "non",
                        uid =>
                        {
                            OnSignInResponse      = OnComplete;
                            IsSilentSignIn        = false;
                            GoogleSignIn.FAKE_UID = uid;
                            GoogleSignIn.GetInstance().SignIn();
                        },
                        () =>
                        {
                            OnComplete?.Invoke();
                        } );
                    return;
                }

                OnSignInResponse = OnComplete;
                
                if( silent )
                {
                    IsSilentSignIn     = true;
                    IsSilentSignInOnly = silentOnly;
                    GoogleSignIn.GetInstance().SignInSilently();
                }
                else
                {
                    IsSilentSignIn = false;
                    GoogleSignIn.GetInstance().SignIn();
                }
            }
        }

        /// <summary>
        /// Se deconnecter de google
        /// </summary>
        internal static void SignOut( Action OnComplete )
        {
            Log( "SignOut" );
            
            IsConnected      = false;
            User             = null;
            OnSignInResponse = OnComplete;
            GoogleSignIn.GetInstance().SignOut();
        }

        /// <summary>
        /// Revoquer le compte
        /// </summary>
        internal static void Disconnect( Action OnComplete )
        {
            Log( $"Disconnect {IsConnected}" );
            
            IsConnected      = false;
            User             = null;
            OnSignInResponse = OnComplete;
            GoogleSignIn.GetInstance().Disconnect();
        }
        

        private static void OnAuthenticationFinished( GoogleSignInUser user )
        {
            Log( $"OnAuthenticationFinished Status: {user?.Status}" );
            
            if( user is { Status: GoogleSignInStatusCode.Success or GoogleSignInStatusCode.SuccessCached } )
            {
                User = user;

                Log( $"IDToken:     {User.IdToken}" );
                Log( $"UserID:      {User.UserId}" );
                Log( $"DisplayName: {User.DisplayName}" );
                Log( $"GivenName:   {User.GivenName}" );
                Log( $"FamilyName:  {User.FamilyName}" );
                Log( $"PhotoUrl:    {User.PhotoUrl}" );
                Log( $"Email:       {User.Email}" );
                Log( $"AuthCode:    {User.AuthCode}" );
                Log( $"STATUS:      {User.Status}" );

                IsConnected = true;
            }

            // s'assurer que le user soit null si on est pas connecte
            if( !IsConnected )
                User = null;

            if( User == null )
            {
                Log( "[ConnectWithGoogle] RESULT WITHOUT USER:: NOT CONNECTED" );
                IsConnected = false;
            }

            // si c'etait un login silencieux echoue, on tente un login normal
            if( !IsConnected && IsSilentSignIn && !IsSilentSignInOnly )
            {
                SignIn( OnSignInResponse, false );
                return;
            }
            
            
            // invoke the callback
            OnSignInResponse?.Invoke();
        }
    }
}