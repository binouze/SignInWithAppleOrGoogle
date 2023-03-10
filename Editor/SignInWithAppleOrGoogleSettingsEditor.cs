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
        
        private SerializedProperty _SignInWithGoogleWebClientID_Android;
        private SerializedProperty _SignInWithGoogleClientID_Android;
        
        private SerializedProperty _SignInWithGoogleWebClientID_ios;
        private SerializedProperty _SignInWithGoogleClientID_ios;

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
            
            _SignInWithGoogleWebClientID_Android = serializedObject.FindProperty("_SignInWithGoogleWebClientID_Android");
            _SignInWithGoogleClientID_Android    = serializedObject.FindProperty("_SignInWithGoogleClientID_Android");
            
            _SignInWithGoogleWebClientID_ios = serializedObject.FindProperty("_SignInWithGoogleWebClientID_ios");
            _SignInWithGoogleClientID_ios    = serializedObject.FindProperty("_SignInWithGoogleClientID_ios");
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
            EditorGUILayout.LabelField("Android", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_SignInWithGoogleWebClientID_Android, new GUIContent("Web Client ID"));
            EditorGUILayout.PropertyField(_SignInWithGoogleClientID_Android,    new GUIContent("Client ID"));
            EditorGUI.indentLevel--;
            
            EditorGUILayout.LabelField("iOS", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_SignInWithGoogleWebClientID_ios, new GUIContent("Web Client ID"));
            EditorGUILayout.PropertyField(_SignInWithGoogleClientID_ios,    new GUIContent("Client ID"));
            EditorGUI.indentLevel--;
            
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