using UnityEngine;

namespace com.binouze
{
    public class SignInWithAppleOrGoogleSettings : ScriptableObject
    {
        public const string SignInWithAppleOrGoogleSettingsFile          = "SignInWithAppleOrGoogleSettings";
        public const string SignInWithAppleOrGoogleSettingsResDir        = "Assets/LagoonPlugins/SignInWithAppleOrGoogle/Resources";
        public const string SignInWithAppleOrGoogleSettingsFileExtension = ".asset";

        public static SignInWithAppleOrGoogleSettings LoadInstance()
        {
            // Read from resources.
            return Resources.Load<SignInWithAppleOrGoogleSettings>(SignInWithAppleOrGoogleSettingsFile);
        }

        public bool IsValide()
        {
            return
                !string.IsNullOrEmpty( _URL_APPLECONNECT_REDIRECT )   &&
                !string.IsNullOrEmpty( _SignInWithGoogleWebClientID ) &&
                !string.IsNullOrEmpty( _SignInWithGoogleClientID );
        }
        
        // -- APPLE
        
        [SerializeField]
        private string _URL_APPLECONNECT_REDIRECT = string.Empty;

        
        // -- GOOGLE
        
        [SerializeField]
        private string _SignInWithGoogleWebClientID = string.Empty;

        
        [SerializeField]
        private string _SignInWithGoogleClientID;

        
        public string URL_APPLECONNECT_REDIRECT
        {
            get => _URL_APPLECONNECT_REDIRECT;
            set => _URL_APPLECONNECT_REDIRECT = value;
        }

        public string SignInWithGoogleWebClientID
        {
            get => _SignInWithGoogleWebClientID;
            set => _SignInWithGoogleWebClientID = value;
        }
        
        public string SignInWithGoogleClientID
        {
            get => _SignInWithGoogleClientID;
            set => _SignInWithGoogleClientID = value;
        }
    }
}