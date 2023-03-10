using System;
using System.Collections.Generic;
using System.Globalization;

namespace com.binouze
{
    internal static class ExtensionsMethods
    {
        // VALUES FROM DICTIONNARY
        
        public static object GetValue( this Dictionary<string, object> dico, params string[] args )
        {
            if( dico == null ) return null;

            var len = args.Length;
            for( var i = 0; i<args.Length; i++ )
            {
                var key = args[i];
                var obj = dico.GetValue( key );
                
                if( i < len - 1 )
                {
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return null;
                }
                else
                {
                    return obj;
                }
            }
            
            return null;
        }

        public static object GetValue( this Dictionary<string, object> dico, string key )
        {
            if( dico == null ) return null;
            return dico.TryGetValue( key, out var val ) ? val : null;
        }
        
        public static int GetInt( this Dictionary<string, object> dico, int defaut = 0, params string[] args )
        {
            if( dico == null ) return defaut;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];
                
                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return defaut;
                }
                else
                {
                    return dico.GetInt( key, defaut );
                }
            }

            return defaut;
        }

        public static bool GetIntBool( this Dictionary<string, object> dico, string key, bool defaut = false )
        {
            return dico.GetInt( key, defaut ? 1 : 0 ) == 1;
        }
        
        public static int GetInt( this Dictionary<string, object> dico, string key, int defaut = 0 )
        {
            if( dico == null ) return defaut;
            return dico.TryGetValue( key, out var val ) ? val.GetInt( defaut ) : defaut;
        }

        public static float GetFloat( this Dictionary<string, object> dico, float defaut = 0f, params string[] args )
        {
            if( dico == null ) return defaut;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return defaut;
                }
                else
                {
                    return dico.GetFloat( key, defaut );
                }
            }

            return defaut;
        }

        public static float GetFloat( this Dictionary<string, object> dico, string key, float defaut = 0f )
        {
            if( dico == null ) return defaut;
            return dico.TryGetValue( key, out var val ) ? val.GetFloat( defaut ) : defaut;
        }

        public static double GetDouble( this Dictionary<string, object> dico, string key, float defaut = 0f )
        {
            if( dico == null ) return defaut;
            return dico.TryGetValue( key, out var val ) ? val.GetDouble( defaut ) : defaut;
        }
        
        
        public static string GetStringS( this Dictionary<string, object> dico, string defaut = null, params string[] args )
        {
            if( dico == null ) return defaut;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return defaut;
                }
                else
                {
                    return dico.GetString( key, defaut );
                }
            }

            return defaut;
        }

        public static string GetString( this Dictionary<string, object> dico, string key, string defaut = null )
        {
            if( dico == null ) return defaut;
            return dico.TryGetValue( key, out var val ) ? Convert.ToString(val) : defaut;
        }

        public static Dictionary<string,object> GetDictionary( this Dictionary<string, object> dico, string key )
        {
            if( dico == null ) return null;
            return dico.TryGetValue( key, out var val ) ? val as Dictionary<string, object> : null;
        }

        public static Dictionary<string, object> GetDictionary( this Dictionary<string, object> dico, params string[] args )
        {
            if( dico == null ) return null;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return null;
                }
                else
                {
                    return dico.GetDictionary( key );
                }
            }

            return null;
        }

        public static Dictionary<string,object> GetDictionary( this object obj )
        {
            return obj as Dictionary<string, object>;
        }
        
        public static List<object> GetList( this Dictionary<string, object> dico, string key )
        {
            if( dico == null ) return null;
            return dico.TryGetValue( key, out var val ) ? val as List<object> : null;
        }

        public static List<object> GetList( this Dictionary<string, object> dico, params string[] args )
        {
            if( dico == null ) return null;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return null;
                }
                else
                {
                    return dico.GetList( key );
                }
            }

            return null;
        }



        public static bool GetBool( this Dictionary<string, object> dico, string key, bool defaut = false )
        {
            if( dico == null ) return defaut;
            return dico.TryGetValue( key, out var val ) ? Convert.ToBoolean( val ) : defaut;
        }
        
        public static bool GetBool( this Dictionary<string, object> dico, bool defaut, params string[] args )
        {
            if( dico == null ) return false;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return false;
                }
                else
                {
                    return dico.GetBool( key );
                }
            }

            return false;
        }

        public static List<string> GetListString( this List<object> liste )
        {
            if( liste != null )
            {
                var retour = new List<string>();
                foreach( var value in liste )
                {
                    retour.Add( value.GetString() );
                }

                return retour;
            }
            return null;
        }
        
        public static List<string> GetListString( this Dictionary<string, object> dico, string key )
        {
            var liste = dico.GetList( key );
            if( liste != null )
            {
                var retour = new List<string>();
                foreach( var value in liste )
                {
                    retour.Add( value.GetString() );
                }

                return retour;
            }
            return null;
        }
        
        public static List<int> GetListInt( this List<object> liste )
        {
            if( liste != null )
            {
                var retour = new List<int>();
                foreach( var value in liste )
                {
                    retour.Add( value.GetInt() );
                }

                return retour;
            }
            return null;
        }
        
        public static List<int> GetListInt( this Dictionary<string, object> dico, string key )
        {
            var liste = dico.GetList( key );
            if( liste != null )
            {
                var retour = new List<int>();
                foreach( var value in liste )
                {
                    retour.Add( value.GetInt() );
                }

                return retour;
            }
            return null;
        }
        
        public static List<int> GetListInt( this object obj )
        {
            if( obj is List<object> liste )
            {
                var retour = new List<int>();
                foreach( var value in liste )
                {
                    retour.Add( value.GetInt() );
                }

                return retour;
            }
            return null;
        }
        
        public static List<object> GetList( this object obj )
        {
            if( obj is List<object> liste )
            {
                return liste;
            }
            return null;
        }

        public static List<int> GetListInt( this Dictionary<string, object> dico, params string[] args )
        {
            if( dico == null ) return null;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return null;
                }
                else
                {
                    return dico.GetListInt( key );
                }
            }

            return null;
        }

        public static List<bool> GetListBool( this Dictionary<string, object> dico, string key )
        {
            var liste = dico?.GetList( key );
            if( liste != null )
            {
                var retour = new List<bool>();
                foreach( var value in liste )
                {
                    retour.Add( Convert.ToBoolean(value) );
                }

                return retour;
            }
            return null;
        }

        public static List<bool> GetListBool( this Dictionary<string, object> dico, params string[] args )
        {
            if( dico == null ) return null;

            var len = args.Length;
            for( var i = 0; i < args.Length; i++ )
            {
                var key = args[i];

                if( i < len - 1 )
                {
                    var obj = dico.GetValue( key );
                    dico = obj as Dictionary<string, object>;
                    if( dico == null )
                        return null;
                }
                else
                {
                    return dico.GetListBool( key );
                }
            }

            return null;
        }
        
        public static string EchoDico( this IDictionary<object, object> dico )
        {
            var s = new List<string>();
            foreach( var keyvalue in dico )
            {
                s.Add( "{ " + keyvalue.Key + " -> " + keyvalue.Value + " }" );
            }
            return string.Join( ",", s );
        }
        public static string EchoDico( this IDictionary<string, object> dico )
        {
            var s = new List<string>();
            foreach( var keyvalue in dico )
            {
                s.Add( "{ " + keyvalue.Key + " -> " + keyvalue.Value + " }" );
            }
            return string.Join( ",", s );
        }
        public static string EchoDico( this IDictionary<string, string> dico )
        {
            var s = new List<string>();
            foreach( var keyvalue in dico )
            {
                s.Add( "{ " + keyvalue.Key + " -> " + keyvalue.Value + " }" );
            }
            return string.Join( ",", s );
        }

        // VALUES FROM OBJECT
        
        public static string GetString( this object obj, string defaut = null )
        {
            var s = obj as string;
            return s ?? defaut;
        }
        
        /// <summary>
        /// Récuperer un int quand on est sur que la variable est un int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaut"></param>
        /// <returns></returns>
        public static int GetInt( this object obj, int defaut = 0 )
        {
            if( obj is char c )
                obj = c.ToString();
            
            try
            {
                return Convert.ToInt32( Convert.ToDecimal( obj, CultureInfo.InvariantCulture ), CultureInfo.InvariantCulture );
            }
            catch( Exception )
            {
                return defaut;
            }
        }

        /// <summary>
        /// Récuperer un float quand on est sur que la variable est un float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaut"></param>
        /// <returns></returns>
        public static float GetFloat( this object obj, float defaut = 0 )
        {
            return (float)Convert.ToDouble( obj, CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Récuperer un double quand on est sur que la variable est un double
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaut"></param>
        /// <returns></returns>
        public static double GetDouble( this object obj, double defaut = 0 )
        {
            return Convert.ToDouble( obj, CultureInfo.InvariantCulture );
        }
        
        // FOREACH
        
        
        /// <summary>
        /// foreach helper
        /// </summary>
        /// <param name="ie"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>( this IEnumerable<T> ie, Action<T> action )
        {
            foreach( var i in ie )
            {
                action( i );
            }
        }
    }
}
