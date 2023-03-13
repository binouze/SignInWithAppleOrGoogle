
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


        /// <summary><para>Returns a string that represents the current object.</para></summary>
        public override string ToString()
        {
            return $"{nameof( Email )}: {Email}, {nameof( FirstName )}: {FirstName}, {nameof( LastName )}: {LastName}, {nameof( UserId )}: {UserId}";
        }
    }
}