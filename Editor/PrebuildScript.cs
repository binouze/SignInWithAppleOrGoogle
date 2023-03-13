using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace com.binouze
{
    public class PrebuildScript : IPreprocessBuildWithReport
    {
        public int callbackOrder => int.MaxValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("AdsPrebuildScript.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
            // creer les settings
            var settings = SignInWithAppleOrGoogleSettingsEditor.LoadSettingsInstance();
            if( settings == null || !settings.IsValide() )
            {
                Debug.LogError( "settings not valid please check LagoonPlugins/SignInWithAppleOrGoogle Settings" );
            }
            
            // -- Add the SignInWithGoogle iOS URL Scheme in the settings if needed

            if( !string.IsNullOrWhiteSpace( settings.APP_URL_SCHEME ) )
            {
                var schemesOK     = false;
                var actualSchemes = PlayerSettings.iOS.iOSUrlSchemes;
                foreach( var scheme in actualSchemes )
                {
                    if( scheme == settings.Google_IosClientScheme )
                    {
                        schemesOK = true;
                        break;
                    }
                }

                if( !schemesOK )
                {
                    var schemes = new string[PlayerSettings.iOS.iOSUrlSchemes.Length + 1];
                    for( var i = 0; i < PlayerSettings.iOS.iOSUrlSchemes.Length; i++ )
                    {
                        schemes[i] = PlayerSettings.iOS.iOSUrlSchemes[i];
                    }

                    schemes[PlayerSettings.iOS.iOSUrlSchemes.Length] = settings.Google_IosClientScheme;
                    PlayerSettings.iOS.iOSUrlSchemes                 = schemes;
                    
                    Debug.Log( "SignInWithGoogle iOS URL Scheme added to PlayerSettings.iOS.iOSUrlSchemes" );
                }
            }
            
            // copier les fichiers necessaires avant la compilation
            PrepareProjectFolders();
        }

        public static void PrepareProjectFolders()
        {
            if( !AssetDatabase.IsValidFolder("Assets/LagoonPlugins") )
            {
                AssetDatabase.CreateFolder("Assets", "LagoonPlugins");
            }

            if( !AssetDatabase.IsValidFolder("Assets/LagoonPlugins/SignInWithAppleOrGoogle") )
            {
                AssetDatabase.CreateFolder("Assets/LagoonPlugins", "SignInWithAppleOrGoogle");
            }
            
            if( !AssetDatabase.IsValidFolder("Assets/LagoonPlugins/SignInWithAppleOrGoogle/Resources") )
            {
                AssetDatabase.CreateFolder("Assets/LagoonPlugins/SignInWithAppleOrGoogle", "Resources");
            }
        }
    }
}