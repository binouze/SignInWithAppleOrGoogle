using Unity.Collections;
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
                !string.IsNullOrEmpty( _URL_APPLECONNECT_REDIRECT ) &&
                !string.IsNullOrEmpty( _Google_WebClientID )        &&
                !string.IsNullOrEmpty( _Google_IosClientID )        &&
                !string.IsNullOrEmpty( _Google_IosClientScheme );
        }
        
        // -- APPLE
        
        [SerializeField]
        private string _URL_APPLECONNECT_REDIRECT = string.Empty;

        
        //TODO1 maj fields: WebClientID + iOSClientID
        //TODO1 add fields iosScheme auto generated (inverser le client ID) + Prebuild script pour ajouter le scheme aux PlayerSettings.iOS.iOSUrlSchemes
        
        // -- GOOGLE
        
        [SerializeField][TextAreaAttribute]
        private string _Google_WebClientID = string.Empty;
        [SerializeField][TextAreaAttribute]
        private string _Google_IosClientID;
        [SerializeField][TextAreaAttribute]
        private string _Google_IosClientScheme;

        
        public string URL_APPLECONNECT_REDIRECT
        {
            get => _URL_APPLECONNECT_REDIRECT;
            set => _URL_APPLECONNECT_REDIRECT = value;
        }

        public string Google_WebClientID
        {
            get => _Google_WebClientID;
            set => _Google_WebClientID = value;
        }
        
        public string Google_IosClientID
        {
            get => _Google_IosClientID;
            set => _Google_IosClientID = value;
        }
        
        public string Google_IosClientScheme
        {
            get => _Google_IosClientScheme;
            set => _Google_IosClientScheme = value;
        }

        private string GetIosScheme()
        {
            if( !string.IsNullOrWhiteSpace( Google_IosClientID ) )
            {
                var ex = Google_IosClientID.Split( "." );
                if( ex.Length >= 2 )
                {
                    // recuperer le dernier element qui ira au debut
                    var last = ex[^1];
                    // supprimer le dernier element de la liste
                    ex = ex[..^1];
                    // reformer le string avec le premier element suivi des autres
                    return $"{last}."+string.Join( '.', ex );
                }
            }

            return string.Empty;
        }


        private string saved_Google_IosClientScheme;
        void OnValidate()
        {
            _Google_IosClientScheme = GetIosScheme();
        }
    }
}