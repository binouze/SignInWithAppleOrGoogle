
namespace com.binouze
{
    /// <summary> Information for the authenticated user.</summary>
    public class AppleSignInUser
    {
        /// <summary> Email address.</summary>
        ///<remarks> null if not requested, or if there was an error.</remarks>
        public string Email { get; internal set; }


        /// <summary> First Name.</summary>
        public string FirstName { get; internal set; }

        /// <summary> Last Name.</summary>
        public string LastName { get; internal set; }

        /// <summary> User ID</summary>
        public string UserId { get; internal set; }

        /// <summary> Authorization code (valid 5 minutes, single use), to exchange server side for tokens
        /// (https://appleid.apple.com/auth/token), for example to revoke them (https://appleid.apple.com/auth/revoke).</summary>
        /// <remarks> null when no Apple sheet was shown (existing credential only checked) or on error.
        /// iOS: issued for the app bundle ID. Android: issued for APPLECONNECT_CLIENT_ID (Services ID)
        /// with URL_APPLECONNECT_REDIRECT as redirect_uri.</remarks>
        public string AuthorizationCode { get; internal set; }

        /// <summary> Identity token (JWT) returned with the authorization code.</summary>
        /// <remarks> null when <see cref="AuthorizationCode"/> is null.</remarks>
        public string IdentityToken { get; internal set; }


        /// <summary><para>Returns a string that represents the current object.</para></summary>
        public override string ToString()
        {
            return $"{nameof( Email )}: {Email}, {nameof( FirstName )}: {FirstName}, {nameof( LastName )}: {LastName}, {nameof( UserId )}: {UserId}, " +
                   $"{nameof( AuthorizationCode )}: {(string.IsNullOrEmpty( AuthorizationCode ) ? "none" : "yes")}";
        }
    }
}