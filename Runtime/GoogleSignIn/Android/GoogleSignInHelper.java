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
package com.binouze;
import com.google.android.libraries.identity.googleid.GetGoogleIdOption;
import com.google.android.libraries.identity.googleid.GetSignInWithGoogleOption;
import com.google.android.libraries.identity.googleid.GoogleIdTokenCredential;
import com.unity3d.player.UnityPlayer;

import android.os.CancellationSignal;
import android.util.Base64;
import android.util.Log;

import org.json.JSONObject;

import java.nio.charset.StandardCharsets;
import java.util.concurrent.Executors;

import android.net.Uri;

import androidx.annotation.NonNull;
import androidx.credentials.ClearCredentialStateRequest;
import androidx.credentials.Credential;
import androidx.credentials.CredentialManager;
import androidx.credentials.CredentialManagerCallback;
import androidx.credentials.CustomCredential;
import androidx.credentials.GetCredentialRequest;
import androidx.credentials.GetCredentialResponse;
import androidx.credentials.exceptions.ClearCredentialException;
import androidx.credentials.exceptions.GetCredentialCancellationException;
import androidx.credentials.exceptions.GetCredentialException;

/**
 * Helper class used by the native C++ code to interact with Google Sign-in API. The general flow is
 * Call configure, then one of signIn or signInSilently.
 */
public class GoogleSignInHelper 
{
    // Set to true to get more debug logging.
    public static boolean loggingEnabled = false;
    private static final String TAG = "GoogleSignInHelper";

    /**
    * Enables verbose logging
    */
    public static void enableDebugLogging( boolean flag )
    {
        loggingEnabled = flag;
    }

    /**
    * Sets the configuration of the sign-in api that should be used.
    *
    * @param webClientId - the web client id of the backend server associated with this application.
    * @param useSignInWithGoogleForm - if true, the sign in will show the sign in with google form instead of the GoogleId credential sign in card.
    * @param forceRefreshToken - not used anymore.
    * @param requestEmail - if true, the response will try to return the email.
    * @param requestIdToken - if true, the response will try to return the idToken.
    * @param requestProfile - if true, the response will try to return the familyName, givenName and username.
    * @param defaultAccountName - not used anymore.
    * @param urlScheme - not used anymore.
    */
    public static void configure(
      //String ClientId, // not used on android platform
      String webClientId,
      boolean useSignInWithGoogleForm,
      //boolean forceRefreshToken, not used anymore
      boolean requestEmail,
      boolean requestIdToken,
      boolean requestProfile/*,
      String defaultAccountName,
      String urlScheme*/ ) 
    {
        _webClientId             = webClientId;
        _requestEmail            = requestEmail;
        _requestIdToken          = requestIdToken;
        _requestProfile          = requestProfile;
        _useSignInWithGoogleForm = useSignInWithGoogleForm;

        logDebug("GoogleSignInHelper.configure called");
    }

    public static String _webClientId;
    public static boolean _requestEmail;
    public static boolean _requestIdToken;
    public static boolean _requestProfile;
    public static boolean _useSignInWithGoogleForm;

    public static void signIn()
    {
        logDebug("GoogleSignInHelper.signIn called!");
        _signIn(false);
    }
    public static void signInSilently()
    {
        logDebug("AuthHelperFragment.signInSilently called!");
        _signIn(true);
    }

    public static void _signIn( boolean silent )
    {
        logDebug("AuthHelperFragment.authenticate called! " + _webClientId );

        // get the credential manager
        CredentialManager credentialManager = CredentialManager.create(UnityPlayer.currentActivity);



        GetCredentialRequest credentialRequest;
        if( _useSignInWithGoogleForm )
        {
            GetSignInWithGoogleOption siwg =  new GetSignInWithGoogleOption.Builder(_webClientId).build();
            // create the credential request
            credentialRequest = new GetCredentialRequest.Builder()
                    .addCredentialOption(siwg)
                    .build();
        }
        else
        {
            // option sign in with google
            GetGoogleIdOption gid = new GetGoogleIdOption.Builder()
                    .setServerClientId(_webClientId)
                    .setFilterByAuthorizedAccounts(false)
                    .setAutoSelectEnabled(silent)
                    .build();

            // create the credential request
            credentialRequest = new GetCredentialRequest.Builder()
                    .addCredentialOption(gid)
                    .build();
        }

        // launch the sign in flow
        credentialManager.getCredentialAsync(
                UnityPlayer.currentActivity,
                credentialRequest,
                new CancellationSignal(),
                Executors.newSingleThreadExecutor(),
                new CredentialManagerCallback<>() {
                    @Override
                    public void onResult(GetCredentialResponse result)
                    {
                        Credential credential = result.getCredential();

                        if( credential instanceof CustomCredential )
                        {
                            if( GoogleIdTokenCredential.TYPE_GOOGLE_ID_TOKEN_CREDENTIAL.equals(credential.getType()) )
                            {
                                try
                                {
                                    GoogleIdTokenCredential customCred = GoogleIdTokenCredential.createFrom(credential.getData());
                                    // call the response handler
                                    nativeOnCredentialManagerResult(0, customCred);
                                }
                                catch( Exception e )
                                {
                                    // call the response handler with status failed
                                    logErrorWithError("Failed to parse an GoogleIdTokenCredential", e);
                                    nativeOnCredentialManagerResult(8,null);
                                }
                            }
                            else
                            {
                                // call the response handler with status failed
                                logError("Unexpected type of credential");
                                nativeOnCredentialManagerResult(8,null);
                            }
                        }
                    }

                    @Override
                    public void onError(@NonNull GetCredentialException e)
                    {
                        logErrorWithError("GetCredentialException ", e);

                        // canceled ou internal error
                        if( e instanceof GetCredentialCancellationException )
                            nativeOnCredentialManagerResult(2,null);
                        else
                            nativeOnCredentialManagerResult(8,null);
                    }
                });
    }

    public static void nativeOnCredentialManagerResult( int result, GoogleIdTokenCredential acct )
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
                // get the user ID from the ID token
                String idToken            = acct.getIdToken();
                String[] segments         = idToken.split("\\.");
                byte[] payloadAsByteArray = Base64.decode(segments[1], Base64.NO_PADDING);
                JSONObject payloadInJson  = new JSONObject(new String(payloadAsByteArray, StandardCharsets.UTF_8));
                String uniqueIdentifier   = (String)payloadInJson.get("sub");

                // add the user data
                strStrMap.put("GivenName",   _requestProfile ? acct.getGivenName()   : "");
                strStrMap.put("FamilyName",  _requestProfile ? acct.getFamilyName()  : "");
                strStrMap.put("DisplayName", _requestProfile ? acct.getDisplayName() : "");
                strStrMap.put("Email",       _requestEmail   ? acct.getId()          : "");
                strStrMap.put("UserId",      uniqueIdentifier);
                strStrMap.put("IdToken",     _requestIdToken ? acct.getIdToken()     : "");

                // add user picture if available
                Uri photoUrl = acct.getProfilePictureUri();
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

    public static void signOut() 
    {
        logDebug("AuthHelperFragment.signOut called!");

        ClearCredentialStateRequest req = new ClearCredentialStateRequest();
        // get the credential manager
        CredentialManager credentialManager = CredentialManager.create(UnityPlayer.currentActivity);
        credentialManager.clearCredentialStateAsync(
                req,
                new CancellationSignal(),
                Executors.newSingleThreadExecutor(),
                new CredentialManagerCallback<>() {
                    @Override
                    public void onResult(Void unused) {
                        nativeOnCredentialManagerResult(-2,null);
                    }
                    @Override
                    public void onError(@NonNull ClearCredentialException e)
                    {
                        nativeOnCredentialManagerResult(-2,null);
                    }
                });
    }

    public static void disconnect() 
    {
        signOut();
    }


    public static void closeDialog()
    {
        // not available anymore
        /*GoogleSignInFragment fragment = GoogleSignInFragment.getInstance(UnityPlayer.currentActivity);
        fragment.closeDialog();*/
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

    public static void logErrorWithError(String msg, Exception e)
    {
        Log.e(TAG, TAG+"::"+msg, e);
    }

    public static void logDebug(String msg) 
    {
        if( loggingEnabled )
        {
            Log.d(TAG, TAG+"::"+msg);
        }
    }

    /*
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
    }*/
}
