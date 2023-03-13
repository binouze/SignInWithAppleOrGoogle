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
        
        private SerializedProperty _Google_WebClientID;
        private SerializedProperty _Google_IosClientID;
        private SerializedProperty _Google_IosClientScheme;

        public static SignInWithAppleOrGoogleSettings LoadSettingsInstance()
        {
            PrebuildScript.PrepareProjectFolders();
            
            var instance = SignInWithAppleOrGoogleSettings.LoadInstance();
            // Create instance if null.
            if( instance == null )
            {
                Directory.CreateDirectory(SignInWithAppleOrGoogleSettings.SignInWithAppleOrGoogleSettingsResDir);
                instance = CreateInstance<SignInWithAppleOrGoogleSettings>();
                var assetPath = Path.Combine( SignInWithAppleOrGoogleSettings.SignInWithAppleOrGoogleSettingsResDir, SignInWithAppleOrGoogleSettings.SignInWithAppleOrGoogleSettingsFile + SignInWithAppleOrGoogleSettings.SignInWithAppleOrGoogleSettingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }
            return instance;
        }
        
        [MenuItem("LagoonPlugins/SignInWithAppleOrGoogle Settings")]
        public static void OpenInspector()
        {
            Selection.activeObject = LoadSettingsInstance();
        }

        public void OnEnable()
        {
            _URL_APPLECONNECT_REDIRECT   = serializedObject.FindProperty("_URL_APPLECONNECT_REDIRECT");
            
            _Google_WebClientID     = serializedObject.FindProperty("_Google_WebClientID");
            _Google_IosClientID     = serializedObject.FindProperty("_Google_IosClientID");
            _Google_IosClientScheme = serializedObject.FindProperty("_Google_IosClientScheme");
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
            
            EditorGUILayout.LabelField("SignIn with Apple configuration:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            //EditorGUILayout.HelpBox( "enter your AdMost applications ids here", MessageType.Info);
            
            //EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_URL_APPLECONNECT_REDIRECT, new GUIContent("Apple Connect Redirect URL:"));
            //EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            
            // -- Google
            
            EditorGUILayout.LabelField("SignIn with Google configuration:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            
            //EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_Google_WebClientID,     new GUIContent("Web Client ID:"));
            EditorGUILayout.PropertyField(_Google_IosClientID,     new GUIContent("iOS Client ID:"));
            
            EditorGUI.BeginDisabledGroup( true );
            EditorGUILayout.PropertyField(_Google_IosClientScheme, new GUIContent("iOS Scheme:"));
            EditorGUI.EndDisabledGroup();
            //EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
    }
}