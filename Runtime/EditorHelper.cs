using System;
using UnityEngine;

namespace com.binouze
{
    internal class EditorHelper : MonoBehaviour
    {
        private static Rect _windowRect = new Rect(50, 50, 400, 400);
        
        private static bool   HasDialog;
        private static string Texte;
        private static string Bouton1;
        private static string Bouton2;
        private static Action Action1;
        private static Action<string> Action1Input;
        private static Action Action2;
        
        #region static Singleton
        private static EditorHelper _instance;
        private static EditorHelper GetInstance()
        {
            if( _instance == null ) 
            {
                _instance = new GameObject("SignInEditorHelper").AddComponent<EditorHelper> ();
                DontDestroyOnLoad (_instance.gameObject);
            }
            return _instance;
            
        }
        #endregion
        
        
        private void OnGUI ()
        {
            if( !HasDialog )
                return;
            
            var s1 = Screen.width  / 500f;
            var s2 = Screen.height / 500f;
            var s  = Math.Min( s1, s2 );
		
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, s)); 
            
            _windowRect = GUILayout.Window( 0, _windowRect, DoMyWindow, "TEST ADS" );
        }

        public static void ShowDialog( string texte, string bouton1, Action action1, string bouton2 = null, Action action2 = null )
        {
            GetInstance();
            
            Texte     = texte;
            Bouton1   = bouton1;
            Bouton2   = bouton2;
            Action1   = action1;
            Action2   = action2;
            HasDialog = true;
        }
        
        public static void ShowInputDialog( string texte, string bouton1, string bouton2 = null, Action<string> action1 = null, Action action2 = null )
        {
            GetInstance();
            
            Texte     = texte;
            Bouton1   = bouton1;
            Bouton2   = bouton2;
            Action1Input   = action1;
            Action2   = action2;
            HasDialog = true;
            HasInput  = true;
        }


        private static void Close()
        {
            HasDialog = false;
            Action1   = null;
            Action2   = null;
        }
        
        private static bool   HasInput;
        private static string InputVal;
        
        private static void DoMyWindow( int windowID )
        {
            GUILayout.Label(Texte);
            GUILayout.FlexibleSpace();
            
            GUILayout.BeginHorizontal();

            if( HasInput )
            {
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                InputVal = GUILayout.TextField( InputVal );
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
            }
            
            if( !string.IsNullOrEmpty(Bouton1))
            {
                if( GUILayout.Button( Bouton1 ) )
                {
                    if( HasInput )
                        Action1Input?.Invoke(InputVal);
                    else
                        Action1?.Invoke();
                    Close();
                }
            }

            if( !string.IsNullOrEmpty( Bouton2 ) )
            {
                if( GUILayout.Button( Bouton2 ) )
                {
                    Action2?.Invoke();
                    Close();
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}