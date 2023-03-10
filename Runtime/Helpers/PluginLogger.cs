using UnityEngine;

namespace com.binouze
{
    public static class PluginLogger
    {
        private static bool LOGGINGENABLED;

        public static void SetEnabled( bool enabled )
        {
            LOGGINGENABLED = enabled;
        }
        
        public static void Log( string value )
        {
            if( LOGGINGENABLED )
                Debug.Log( $"[SignInWithAppleOrGoogle] {value}" );
        }

        public static void LogWarning( string value )
        {
            if( LOGGINGENABLED )
                Debug.LogWarning( $"[SignInWithAppleOrGoogle] {value}" );
        }
        
        public static void LogError( string value )
        {
            if( LOGGINGENABLED )
                Debug.LogError( $"[SignInWithAppleOrGoogle] {value}" );
        }
    }
}