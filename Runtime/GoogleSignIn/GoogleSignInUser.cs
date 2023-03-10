using System.Collections.Generic;

namespace com.binouze
{
    /// <summary> Information for the authenticated user.</summary>
    public class GoogleSignInUser
    {
        /// <summary> Server AuthCode to be exchanged for an auth token.</summary>
        ///<remarks> null if not requested, or if there was an error.</remarks>
        public string AuthCode { get; internal set; }

        /// <summary> Email address.</summary>
        ///<remarks> null if not requested, or if there was an error.</remarks>
        public string Email { get; internal set; }

        /// <summary> Id token.</summary>
        ///<remarks> null if not requested, or if there was an error.</remarks>
        public string IdToken { get; internal set; }

        /// <summary> Display Name.</summary>
        public string DisplayName { get; internal set; }

        /// <summary> Given Name.</summary>
        public string GivenName { get; internal set; }

        /// <summary> Family Name.</summary>
        public string FamilyName { get; internal set; }

        /// <summary> Profile photo</summary>
        /// <remarks> Can be null if the profile is not requested,
        /// or none set.</remarks>
        public string PhotoUrl { get; internal set; }

        /// <summary> User ID</summary>
        public string UserId { get; internal set; }


        public GoogleSignInStatusCode Status;

        public static GoogleSignInUser FromObject( Dictionary<string,object> obj )
        {
            if( obj == null )
                return new GoogleSignInUser { Status = GoogleSignInStatusCode.Error };

            return new GoogleSignInUser
            {
                AuthCode    = obj.GetString( "AuthCode" ),
                Email       = obj.GetString( "Email" ),
                IdToken     = obj.GetString( "IdToken" ),
                DisplayName = obj.GetString( "DisplayName" ),
                FamilyName  = obj.GetString( "FamilyName" ),
                GivenName   = obj.GetString( "GivenName" ),
                PhotoUrl    = obj.GetString( "PhotoUrl" ),
                UserId      = obj.GetString( "UserId" ),
                Status      = (GoogleSignInStatusCode)obj.GetInt( "Status" ),
            };
        }

        public override string ToString()
        {
            return $"{nameof( Status )}: {Status}, {nameof( AuthCode )}: {AuthCode}, {nameof( Email )}: {Email}, {nameof( IdToken )}: {IdToken}, {nameof( DisplayName )}: {DisplayName}, {nameof( GivenName )}: {GivenName}, {nameof( FamilyName )}: {FamilyName}, {nameof( PhotoUrl )}: {PhotoUrl}, {nameof( UserId )}: {UserId}";
        }
    }
}