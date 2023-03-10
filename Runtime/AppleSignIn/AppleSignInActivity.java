package com.lagoonsoft;

//import androidx.appcompat.app.AppCompatActivity;
import android.app.Activity;
import android.content.Intent;
import android.util.Log;
import android.os.Bundle;
import android.os.Handler;
import java.lang.Runnable;
import com.unity3d.player.UnityPlayer;

public class AppleSignInActivity extends Activity
{
    private static final String TAG = "AppleSignInActivity";

    @Override
    protected void onCreate( Bundle savedInstanceState ) 
    {
        super.onCreate( savedInstanceState );
        
        Intent intent = getIntent();
        Log.d( TAG, "onCreate " + intent.getAction() + " " + intent.getDataString() );
        
        HandleLink( intent );
    }

    @Override
    protected void onNewIntent( Intent intent ) 
    {
        super.onNewIntent( intent );
        Log.d( TAG, "onNewIntent " + intent.getAction() + " " + intent.getDataString() );
        
        HandleLink( intent );
    }
    
    private void HandleLink(Intent intent)
    {
        AppleSignIn.OnSignInResponse( intent.getDataString() );
        
        // return to main application
        Intent main = new Intent( this, UnityPlayer.currentActivity.getClass() );
        startActivity( main );
    }
}