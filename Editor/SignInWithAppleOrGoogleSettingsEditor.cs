using System.IO;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace com.binouze
{
    [CustomEditor( typeof(SignInWithAppleOrGoogleSettings))]
    public class SignInWithAppleOrGoogleSettingsEditor : Editor
    {
        private SerializedProperty _URL_APPLECONNECT_REDIRECT;
        private SerializedProperty _SignInWithGoogleWebClientID;
        private SerializedProperty _SignInWithGoogleClientID;

        public static SignInWithAppleOrGoogleSettings LoadSettingsInstance()
        {
            var instance = SignInWithAppleOrGoogleSettings.LoadInstance();
            // Create instance if null.
            if( instance == null )
            {
                Directory.CreateDirectory(SignInWithAppleOrGoogleSettings.AdImplementationSettingsResDir);
                instance = CreateInstance<SignInWithAppleOrGoogleSettings>();
                var assetPath = Path.Combine( SignInWithAppleOrGoogleSettings.AdImplementationSettingsResDir, SignInWithAppleOrGoogleSettings.SignInWithAppleOrGoogleSettingsFile + SignInWithAppleOrGoogleSettings.AdImplementationSettingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }
            return instance;
        }
        
        [MenuItem("AdImplementation/Settings...")]
        public static void OpenInspector()
        {
            Selection.activeObject = LoadSettingsInstance();
        }

        public void OnEnable()
        {
            _URL_APPLECONNECT_REDIRECT   = serializedObject.FindProperty("_URL_APPLECONNECT_REDIRECT");
            _SignInWithGoogleWebClientID = serializedObject.FindProperty("_SignInWithGoogleWebClientID");
            _SignInWithGoogleClientID    = serializedObject.FindProperty("_SignInWithGoogleClientID");
        }

        public override void OnInspectorGUI()
        {
            // Make sure the Settings object has all recent changes.
            serializedObject.Update();

            var settings = (SignInWithAppleOrGoogleSettings)target;

            if( settings == null )
            {
              Debug.LogError("SignInWithAppleOrGoogleSettings is null.");
              return;
            }

            // -- Apple
            
            EditorGUILayout.LabelField("SignIn with Apple configuration", EditorStyles.boldLabel);
            //EditorGUILayout.HelpBox( "enter your AdMost applications ids here", MessageType.Info);
            
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_URL_APPLECONNECT_REDIRECT, new GUIContent("URL_APPLECONNECT_REDIRECT"));
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            
            // -- Google
            
            EditorGUILayout.LabelField("SignIn with Google configuration", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_SignInWithGoogleWebClientID, new GUIContent("Web Client ID"));
            EditorGUILayout.PropertyField(_SignInWithGoogleClientID,    new GUIContent("Client ID"));
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            /*if( GUILayout.Button("Run Pre-Build Script") )
            {
                AdsPrebuildScript.CopyAssetsIntoProject();
            }*/
            
            EditorGUILayout.Separator();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}