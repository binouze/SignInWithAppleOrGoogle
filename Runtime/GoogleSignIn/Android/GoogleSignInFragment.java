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

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.net.Uri;
//import android.os.Bundle;
import androidx.annotation.NonNull;
//import androidx.annotation.Nullable;
//import android.view.View;
import com.google.android.gms.auth.api.Auth;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
//import com.google.android.gms.auth.api.signin.GoogleSignInOptionsExtension;
//import com.google.android.gms.auth.api.signin.GoogleSignInResult;
//import com.google.android.gms.common.ConnectionResult;
//import com.google.android.gms.common.api.Api;
//import com.google.android.gms.common.api.CommonStatusCodes;
//import com.google.android.gms.common.api.GoogleApiClient;
//import com.google.android.gms.common.api.ResultCallback;
//import com.google.android.gms.common.api.Scope;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.tasks.Task;
import com.google.android.gms.tasks.OnCompleteListener;
//import java.lang.reflect.Field;
//import java.lang.reflect.InvocationTargetException;
//import java.lang.reflect.Method;
//import java.util.Locale;
import com.unity3d.player.UnityPlayer;

/**
 * Activity fragment with no UI added to the parent activity in order to manage the accessing of the
 * player's email address and tokens.
 */
public class GoogleSignInFragment extends Fragment {
        /*implements
        GoogleApiClient.ConnectionCallbacks,
        GoogleApiClient.OnConnectionFailedListener */

    // Tag uniquely identifying this fragment.
    public static final String FRAGMENT_TAG = "GoogleSignin.SignInFragment";
    private static final int RC_SIGNIN = 9009;

    //private GoogleSignInOptions gso;
    private GoogleSignInClient mGoogleSignInClient;

    private boolean ActionEnCours = false;

    private String  WebClientId;
    private boolean RequestAuthCode;
    private boolean ForceTokenRefresh;
    private boolean RequestEmail;
    private boolean RequestIdToken;
    private boolean RequestProfile;
    private String  AccountName;
    
    private String  _WebClientId;
    private boolean _RequestAuthCode;
    private boolean _ForceTokenRefresh;
    private boolean _RequestEmail;
    private boolean _RequestIdToken;
    private boolean _RequestProfile;
    private String  _AccountName;

    private String _urlScheme;

    public void configure( String webClientId,
                           boolean requestAuthCode,
                           boolean forceTokenRefresh,
                           boolean requestEmail,
                           boolean requestIdToken,
                           boolean requestProfile,
                           String accountName,
                           String urlScheme)
    {
        // Maj config actuelle
        _WebClientId       = webClientId;
        _RequestAuthCode   = requestAuthCode;
        _ForceTokenRefresh = forceTokenRefresh;
        _RequestEmail      = requestEmail;
        _RequestIdToken    = requestIdToken;
        _RequestProfile    = requestProfile;
        _AccountName       = accountName;
        _urlScheme         = urlScheme;
    }


    private void majConfigIfNeeded()
    {
        if( mGoogleSignInClient == null             || 
            WebClientId       != _WebClientId       ||
            RequestAuthCode   != _RequestAuthCode   ||
            ForceTokenRefresh != _ForceTokenRefresh ||
            RequestEmail      != _RequestEmail      ||
            RequestIdToken    != _RequestIdToken    ||
            RequestProfile    != _RequestProfile    ||
            AccountName       != _AccountName )
        {
            // Configure sign-in to request the user's ID, email address, and basic
            // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
            
            GoogleSignInOptions.Builder builder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN);
            
            if( _RequestAuthCode )
                builder.requestServerAuthCode(_WebClientId,_ForceTokenRefresh);
            if( _RequestEmail )
                builder.requestEmail();
            if( _RequestIdToken )
                builder.requestIdToken(_WebClientId);
            if( _RequestProfile )
                builder.requestProfile();
            if( _AccountName != null && !_AccountName.isEmpty() && !_AccountName.trim().isEmpty() )
                builder.setAccountName(_AccountName);
            
            // Build a GoogleSignInClient with the options specified by GoogleSignInOptions.
            mGoogleSignInClient = GoogleSignIn.getClient(UnityPlayer.currentActivity, builder.build());
            
            WebClientId       = _WebClientId;
            RequestAuthCode   = _RequestAuthCode;
            ForceTokenRefresh = _ForceTokenRefresh;
            RequestEmail      = _RequestEmail;
            RequestIdToken    = _RequestIdToken;
            RequestProfile    = _RequestProfile;
            AccountName       = _AccountName;
            
            GoogleSignInHelper.logDebug( "FRAGMENT :: MAJ CONFIG OK");
        }
    }

    public void signIn( boolean silent ) 
    {
        if( ActionEnCours )
        {
            GoogleSignInHelper.nativeOnResult(10, null);
            return;
        }
    
        majConfigIfNeeded();
    
        if( !silent )
        {
            GoogleSignInHelper.logDebug( "FRAGMENT::start sign in");
        
            ActionEnCours = true;
            Intent signInIntent = mGoogleSignInClient.getSignInIntent();
            startActivityForResult( signInIntent, RC_SIGNIN );
        }
        else
        {
            // verifier si ono a pas deja une connexion dispo
            GoogleSignInAccount account = GoogleSignIn.getLastSignedInAccount(UnityPlayer.currentActivity);
            if( account != null )
            {
                GoogleSignInHelper.logDebug( "FRAGMENT::silent sign in account found");
                GoogleSignInHelper.nativeOnResult(0, account);
            }
            else
            {
                GoogleSignInHelper.logDebug( "FRAGMENT::silent sign in NO account found");
                GoogleSignInHelper.nativeOnResult(-2, null);
            }
        }
    }
    
    public void signOut()
    {
        GoogleSignInHelper.logDebug( "FRAGMENT::signout " + ActionEnCours);
    
        if( ActionEnCours )
        {
            GoogleSignInHelper.nativeOnResult(10, null);
            return;
        }
        
        if( mGoogleSignInClient == null )
        {
            GoogleSignInHelper.nativeOnResult(-2, null);
            return;
        }
        
        ActionEnCours = true;
        
        // Google sign out
        //Auth.GoogleSignInApi.signOut(mGoogleApiClient);
        
        mGoogleSignInClient.signOut().addOnCompleteListener(UnityPlayer.currentActivity, new OnCompleteListener<Void>() {
            @Override
            public void onComplete(@NonNull Task<Void> task) {
                GoogleSignInHelper.logDebug( "FRAGMENT::signout complete");
                ActionEnCours = false;
                mGoogleSignInClient = null;
                GoogleSignInHelper.nativeOnResult(-2, null);
            }
        });
    }
    
    public void disconnect()
    {
        GoogleSignInHelper.logDebug( "FRAGMENT::disconnect " + ActionEnCours);
    
        if( ActionEnCours )
        {
            GoogleSignInHelper.nativeOnResult(10, null);
            return;
        }
        
        if( mGoogleSignInClient == null )
        {
            GoogleSignInHelper.nativeOnResult(-2, null);
            return;
        }
        
        ActionEnCours = true;
    
        mGoogleSignInClient.revokeAccess().addOnCompleteListener(UnityPlayer.currentActivity, new OnCompleteListener<Void>() {
            @Override
            public void onComplete(@NonNull Task<Void> task) {
                GoogleSignInHelper.logDebug( "FRAGMENT::disconnect complete");
                ActionEnCours = false;
                mGoogleSignInClient = null;
                GoogleSignInHelper.nativeOnResult(-2, null);
            }
        });
    }
    
    
    public void closeDialog()
    {
        if( !ActionEnCours )
            return;
        ActionEnCours = false;
        
        // return to main application
        // seems not needed on Android.
        if( _urlScheme != null )
            Intent main = new Intent( Intent.ACTION_VIEW, Uri.parse(_urlScheme+"xxx") );
        UnityPlayer.currentActivity.startActivity( main );
    }
    
    
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) 
    {
        super.onActivityResult(requestCode, resultCode, data);
    
        GoogleSignInHelper.logDebug( "FRAGMENT::activityResult " + requestCode );
    
        // Result returned from launching the Intent from GoogleSignInClient.getSignInIntent(...);
        if( requestCode == RC_SIGNIN ) 
        {
            // The Task returned from this call is always completed, no need to attach
            // a listener.
            Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
            handleSignInResult(task);
        }
    }
    private void handleSignInResult(Task<GoogleSignInAccount> completedTask) 
    {
        GoogleSignInHelper.logDebug( "FRAGMENT::handleSignInResult" );
    
        try 
        {
            GoogleSignInAccount account = completedTask.getResult(ApiException.class);
            GoogleSignInHelper.nativeOnResult(0, account);
        } 
        catch( ApiException e ) 
        {
            // The ApiException status code indicates the detailed failure reason.
            // Please refer to the GoogleSignInStatusCodes class reference for more information.
            GoogleSignInHelper.logDebug( "FRAGMENT::signInResult:failed code=" + e.getStatusCode());
            GoogleSignInHelper.nativeOnResult(e.getStatusCode(), null);
        }
        catch( Exception ex ) 
        {
            // toute autre exception catch 
            GoogleSignInHelper.logDebug( "FRAGMENT::signInResult:failed code=" + ex.getMessage());
            GoogleSignInHelper.nativeOnResult(6, null);
        }
        
        ActionEnCours = false;
    }

    private static GoogleSignInFragment theFragment;


    /**
    * Gets the instance of the fragment.
    *
    * @param parentActivity - the activity to attach the fragment to.
    * @return the instance.
    */
    public static GoogleSignInFragment getInstance(Activity parentActivity) 
    {
        GoogleSignInFragment fragment = (GoogleSignInFragment) parentActivity.getFragmentManager().findFragmentByTag(FRAGMENT_TAG);
        
        fragment = (fragment != null) ? fragment : theFragment;
        if( fragment == null )
        {
            GoogleSignInHelper.logDebug("Creating fragment");
            fragment = new GoogleSignInFragment();
            FragmentTransaction trans = parentActivity.getFragmentManager().beginTransaction();
            trans.add(fragment, FRAGMENT_TAG);
            trans.commitAllowingStateLoss();
            theFragment = fragment;
        }
        return fragment;
    }
}
