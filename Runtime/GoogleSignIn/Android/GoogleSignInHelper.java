/*
 * Copyright 2017 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.lagoonsoft;
import com.unity3d.player.UnityPlayer;

//import android.app.Activity;
import android.util.Log;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
//import com.google.android.gms.common.api.CommonStatusCodes;

import org.json.JSONObject;
import java.util.HashMap;
import android.net.Uri;

/**
 * Helper class used by the native C++ code to interact with Google Sign-in API. The general flow is
 * Call configure, then one of signIn or signInSilently.
 */
public class GoogleSignInHelper 
{
    // Set to true to get more debug logging.
    public static boolean loggingEnabled = false;
    private static final String TAG = "GoogleSignInFragment";

    /**
    * Enables verbose logging
    */
    public static void enableDebugLogging(boolean flag) 
    {
        loggingEnabled = flag;
    }

    /**
    * Sets the configuration of the sign-in api that should be used.
    *
    * @param webClientId - the web client id of the backend server associated with this application.
    * @param requestAuthCode - true if a server auth code is needed. This also requires the web client id to be set.
    * @param forceRefreshToken - true to force a refresh token when using the server auth code.
    * @param requestEmail - true if email address of the user is requested.
    * @param requestIdToken - true if an id token for the user is requested.
    * @param requestProfile - true if an id token for the user is requested.
    * @param defaultAccountName - the account name to attempt to default to when signing in.
    */
    public static void configure(
      String ClientId, // not used on android platform
      String webClientId,
      boolean requestAuthCode,
      boolean forceRefreshToken,
      boolean requestEmail,
      boolean requestIdToken,
      boolean requestProfile,
      String defaultAccountName ) 
    {
        logDebug("TokenFragment.configure called");
        GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.configure(webClientId,requestAuthCode,forceRefreshToken,requestEmail,requestIdToken,requestProfile,defaultAccountName);
    }

    public static void signIn() 
    {
        logDebug("AuthHelperFragment.authenticate called!");
        GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.signIn(false);
    }

    public static void signInSilently() 
    {
        logDebug("AuthHelperFragment.signinSilently called!");
        GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.signIn(true);
    }

    public static void signOut() 
    {
        logDebug("AuthHelperFragment.signOut called!");
        GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.signOut();
    }

    public static void disconnect() 
    {
        logDebug("AuthHelperFragment.disconnect called!");
        GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.disconnect();
    }


    public static void closeDialog()
    {
        GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.closeDialog();
    }


    public static void logInfo(String msg) 
    {
        if( loggingEnabled ) 
        {
            Log.i(TAG, TAG+"::"+msg);
        }
    }

    public static void logError(String msg) 
    {
        Log.e(TAG, TAG+"::"+msg);
    }

    public static void logDebug(String msg) 
    {
        if( loggingEnabled ) 
        {
            Log.d(TAG, TAG+"::"+msg);
        }
    }

    public static void nativeOnResult( int result, GoogleSignInAccount acct )
    {
        logDebug( "nativeOnResult: " + result );
    
        JSONObject jsonObject = new JSONObject();
        JSONObject strStrMap  = new JSONObject();
        
        // map status codes
        if( result == 12501 )      // CANCELLED
            result = 2;
        else if( result == 12502 ) // AUTRE ACTION EN COURS
            result = 10;
        else if( result == 12500 ) // FAIL
            result = 9;
        else if( result == 7 )     // NETWORK ERROR
            result = 8;
        else if( result == 5 )     // INVALID ACCOUNTS
            result = 4;
        else if( result == 8 )     // INTERNAL ERROR
            result = 7;
        else if( result == 4 )     // SIGN IN REQUIRED
            result = 9;
        
        String strResult;
        try
        {
            strStrMap.put("Status", result);
            
            if( acct != null )
            {
                strStrMap.put("GivenName",   acct.getGivenName());
                strStrMap.put("FamilyName",  acct.getFamilyName());
                strStrMap.put("DisplayName", acct.getDisplayName());
                strStrMap.put("Email",       acct.getEmail());
                strStrMap.put("UserId",      acct.getId());
                strStrMap.put("IdToken",     acct.getIdToken());
                
                Uri photoUrl = acct.getPhotoUrl();
                if( photoUrl != null )
                    strStrMap.put("PhotoUrl", photoUrl.toString());
            }
        
            jsonObject.put("result", strStrMap);
            strResult = jsonObject.toString();
        }
        catch( Exception e )
        {
            strResult = "{\"result\":{\"Status\":9, \"ErrorMessage\":\"JSON OBJECT EXCEPTION\"}}";
        }
        
        logDebug( " > json: " + strResult );
    
        UnityPlayer.UnitySendMessage("GoogleSignInHelperObject", "OnSignInResult", strResult);
    }
}
