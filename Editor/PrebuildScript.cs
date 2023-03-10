using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace com.binouze
{
    public class PrebuildScript : IPreprocessBuildWithReport
    {
        public const string BASE_FOLDER = "Packages/com.binouze.adimplementation/Plugins/AdImplementation/";
        
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("AdsPrebuildScript.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
            // creer les settings
            var settings = SignInWithAppleOrGoogleSettingsEditor.LoadSettingsInstance();
            if( settings == null || !settings.IsValide() )
            {
                Debug.LogError( "settings not valid please check LagoonPlugins/SignInWithAppleOrGoogle Settings" );
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