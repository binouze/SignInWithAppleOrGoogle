package com.binouze;

import android.annotation.SuppressLint;
import android.app.Dialog;
import android.graphics.Rect;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.view.ViewGroup;
import android.view.Window;
import android.webkit.WebResourceRequest;
import android.webkit.WebResourceResponse;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;

import java.io.UnsupportedEncodingException;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

import androidx.annotation.Nullable;
import android.app.Activity;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.content.ActivityNotFoundException;
import android.net.Uri;

import androidx.browser.customtabs.CustomTabsIntent;
import androidx.browser.customtabs.CustomTabsCallback;
import androidx.browser.customtabs.CustomTabsClient;
import androidx.browser.customtabs.CustomTabsServiceConnection;
import androidx.browser.customtabs.CustomTabsSession;
import android.content.ComponentName;

import com.unity3d.player.UnityPlayer;

import java.util.Base64;

public class AppleSignIn
{
    private static final String TAG = "AppleSignIn";

    private static String appleAuthURLFull;
    private static Dialog appleLoginDialog;

    private static String ClientID;
    private static String RedirectURI;
    private static String Scope;
    private static String UrlScheme;

    private static boolean isActive;

    public static void init( String clientID, String redirectURI, String scope, String urlScheme ) 
    {
        Log.i( TAG, "init" );
    
        String state = UUID.randomUUID().toString();
        
        ClientID     = clientID;
        RedirectURI  = redirectURI;
        Scope        = scope;

        appleAuthURLFull = "https://appleid.apple.com/auth/authorize?response_type=code+id_token&response_mode=form_post" 
        + "&client_id="    + clientID 
        + "&scope="        + scope 
        + "&state="        + state 
        + "&redirect_uri=" + redirectURI;
    }

    public static void signIn()
    {
        Log.i(TAG, "signIn");
        
        isActive = true;
        bindCustomTabsService();
    }

    public static void OnSignInResponse( String url )
    {
        try 
        {
            Map<String, String> values = getUrlValues(url);
            
            if( values.containsKey("id_token") )
            {
                String id_token = values.get("id_token");
                Log.i(TAG, "FOUND id_token: " + id_token);
                
                // Get encoded user id by splitting idToken and taking the 2nd piece
                String[] split = id_token.split("\\.");
                if( split.length > 1 )
                {
                    String encodedUserID   = split[1];
                    Base64.Decoder decoder = Base64.getUrlDecoder();
                    String payload         = new String( decoder.decode(encodedUserID) );
                    
                    String user = "{\"empty\":1}";
                    if( values.containsKey("user") )
                    {
                        user = values.get("user");
                        Log.i(TAG, "FOUND user: " + user);
                    }
                    
                    /*
                    //Decode encodedUserID to JSON
                    String decodedUserData    = String(Base64.decode(encodedUserID, Base64.DEFAULT));
                    String userDataJsonObject = JSONObject(decodedUserData);
                    // Get User's ID
                    String userId = userDataJsonObject.getString("sub");
                    Log.i(TAG, "Apple User ID: " + userId);
                    String appleId = userId;
                    */
                    
                    SendResponseToUnity("{\"payload\":"+payload+",\"user\":"+user+"}");
                }
                else
                {
                    SendResponseFailToUnity("id token invalid");
                }
            } 
            else
            {
                SendResponseFailToUnity("id token not found");
            }
        } 
        catch( UnsupportedEncodingException e )
        {
            Log.e(TAG, e.getMessage());
            SendResponseFailToUnity( e.getMessage() );
        }
    }

    private static Map<String, String> getUrlValues(String url) throws UnsupportedEncodingException 
    {
        Log.i(TAG, "getUrlValues - url: " + url);
    
        int i = url.indexOf("?");
        Map<String, String> paramsMap = new HashMap<>();
        
        if( i > -1 ) 
        {
            String searchURL = url.substring(url.indexOf("?") + 1);
            String params[]  = searchURL.split("&");

            for( String param : params ) 
            {
                String temp[] = param.split("=");
                paramsMap.put(temp[0], java.net.URLDecoder.decode(temp[1], "UTF-8"));
            }
        }

        return paramsMap;
    }
    
    private static void SendResponseFailToUnity( String message )
    {
        SendResponseToUnity("{\"result\":\"fail\",\"message\":\""+message+"\"}");
    }
    
    private static void SendResponseToUnity( String message )
    {
        if( !isActive )
            return;
        isActive = false;
        
        UnityPlayer.UnitySendMessage( "[Lagoon-Utils]", "OnAppleSignInResponse", message );
        unbindCustomTabsService();
    }
    
    public static void closeDialog()
    {
        if( !isActive )
            return;
        isActive = false;
        
        // return to main application
        if( UrlScheme != null )
        {
            String url  = UrlScheme+"xxx";
            Intent main = new Intent( Intent.ACTION_VIEW, Uri.parse(url) );
        }
        UnityPlayer.currentActivity.startActivity( main );
        
        // Unbind Service
        unbindCustomTabsService();
    }
    
    public static final String CUSTOM_TAB_PACKAGE_NAME = "com.android.chrome";
    
    private static CustomTabsSession           customTabsSession;
    private static CustomTabsClient            customTabsClient;
    private static CustomTabsServiceConnection customTabsServiceConnection;
    
    private static void bindCustomTabsService() 
    {
        if( customTabsClient != null ) 
        {
            show();
            return;
        }
        
        customTabsServiceConnection = new CustomTabsServiceConnection() 
        {
            @Override
            public void onCustomTabsServiceConnected(ComponentName name, CustomTabsClient client) 
            {
                customTabsClient = client;
                show();
            }

            @Override
            public void onServiceDisconnected(ComponentName name) 
            {
                customTabsClient = null;
                SendResponseFailToUnity("SERVICE DISCONNECTED");
            }
        };
        
        boolean ok = CustomTabsClient.bindCustomTabsService( UnityPlayer.currentActivity, CUSTOM_TAB_PACKAGE_NAME, customTabsServiceConnection );
        if( !ok ) 
        {
            show();
            customTabsServiceConnection = null;
        }
    }
    
    private static CustomTabsSession getSession() 
    {
        if( customTabsClient == null ) 
        {
            customTabsSession = null;
        } 
        else if( customTabsSession == null ) 
        {
            customTabsSession = customTabsClient.newSession( new CustomTabsCallback() 
            {
                @Override
                public void onNavigationEvent( int navigationEvent, Bundle extras ) 
                {
                    String eventName;
                    switch( navigationEvent )
                    {
                        case 1:  eventName = "NAVIGATION_STARTED";  break;
                        case 2:  eventName = "NAVIGATION_FINISHED"; break;
                        case 3:  eventName = "NAVIGATION_FAILED";   break;
                        case 4:  eventName = "NAVIGATION_ABORTED";  break;
                        case 5:  eventName = "TAB_SHOWN";           break;
                        case 6:  eventName = "TAB_HIDDEN";          break;
                        default: eventName = "UNKNOWN";             break;
                    }
                    Log.w(TAG, "customTabsSession onNavigationEvent: Code = " + navigationEvent + " " + eventName);
                    
                    if( navigationEvent == 6 )
                    {
                        SendResponseFailToUnity("TAB HIDDEN");
                    }
                }
            });
        }
        return customTabsSession;
    }
    
    private static void show()
    {
        Log.i(TAG, "show customTab with url: " + appleAuthURLFull);
    
        CustomTabsIntent customTabsIntent = new CustomTabsIntent.Builder( getSession() )
                        .setShareState(CustomTabsIntent.SHARE_STATE_OFF)
                        .build();
        customTabsIntent.intent.addFlags(Intent.FLAG_ACTIVITY_NO_HISTORY);
        
        try
        {
            customTabsIntent.launchUrl( UnityPlayer.currentActivity, Uri.parse(appleAuthURLFull) );
        }
        catch( Exception e )
        {
            Log.e(TAG, "signIn FAIL NO CUSTOM TAB SUPPORT");
            SendResponseFailToUnity("CANNOT LAUNCH CUSTOMTAB "+e.getMessage());
        }
    }
    
    public static void unbindCustomTabsService() 
    {
        if( customTabsServiceConnection == null ) 
        {
            return;
        }
        
        UnityPlayer.currentActivity.unbindService( customTabsServiceConnection );
        customTabsClient  = null;
        customTabsSession = null;
    }
}