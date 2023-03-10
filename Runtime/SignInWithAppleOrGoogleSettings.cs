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
                !string.IsNullOrEmpty( _URL_APPLECONNECT_REDIRECT )           &&
                !string.IsNullOrEmpty( _SignInWithGoogleWebClientID_Android ) &&
                !string.IsNullOrEmpty( _SignInWithGoogleClientID_Android )    &&
                !string.IsNullOrEmpty( _SignInWithGoogleWebClientID_ios )    &&
                !string.IsNullOrEmpty( _SignInWithGoogleClientID_ios );
        }
        
        // -- APPLE
        
        [SerializeField]
        private string _URL_APPLECONNECT_REDIRECT = string.Empty;

        
        // -- GOOGLE IOS
        
        [SerializeField]
        private string _SignInWithGoogleWebClientID_ios = string.Empty;
        [SerializeField]
        private string _SignInWithGoogleClientID_ios;

        
        // -- GOOGLE ANDROID
        
        [SerializeField]
        private string _SignInWithGoogleWebClientID_Android = string.Empty;
        [SerializeField]
        private string _SignInWithGoogleClientID_Android;
        
        public string URL_APPLECONNECT_REDIRECT
        {
            get => _URL_APPLECONNECT_REDIRECT;
            set => _URL_APPLECONNECT_REDIRECT = value;
        }

        public string SignInWithGoogleWebClientID_ios
        {
            get => _SignInWithGoogleWebClientID_ios;
            set => _SignInWithGoogleWebClientID_ios = value;
        }
        
        public string SignInWithGoogleClientID_ios
        {
            get => _SignInWithGoogleClientID_ios;
            set => _SignInWithGoogleClientID_ios = value;
        }
        
        public string SignInWithGoogleWebClientID_Android
        {
            get => _SignInWithGoogleWebClientID_Android;
            set => _SignInWithGoogleWebClientID_Android = value;
        }
        
        public string SignInWithGoogleClientID_Android
        {
            get => _SignInWithGoogleClientID_Android;
            set => _SignInWithGoogleClientID_Android = value;
        }
    }
}