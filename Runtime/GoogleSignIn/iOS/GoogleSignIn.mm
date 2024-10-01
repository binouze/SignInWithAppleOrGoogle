/**
 * Copyright 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#import "GoogleSignIn.h"
//#import <GoogleSignIn/GIDAuthentication.h>
#import <GoogleSignIn/GIDGoogleUser.h>
#import <GoogleSignIn/GIDProfileData.h>
#import <GoogleSignIn/GIDSignIn.h>
#import <UnityAppController.h>

#import "UnityInterface.h"

#import <memory>

// These values are in the Unity plugin code.  The iOS specific
// codes are mapped to these.
static const int kStatusCodeSuccessCached = -1;
static const int kStatusCodeSuccess = 0;
static const int kStatusCodeApiNotConnected = 1;
static const int kStatusCodeCanceled = 2;
static const int kStatusCodeInterrupted = 3;
static const int kStatusCodeInvalidAccount = 4;
static const int kStatusCodeTimeout = 5;
static const int kStatusCodeDeveloperError = 6;
static const int kStatusCodeInternalError = 7;
static const int kStatusCodeNetworkError = 8;
static const int kStatusCodeError = 9;

/**
 * Helper method to pause the Unity player.  This is done when showing any UI.
 */
void UnpauseUnityPlayer() {
  dispatch_async(dispatch_get_main_queue(), ^{
    if (UnityIsPaused() > 0) {
      UnityPause(0);
    }
  });
}
// 
// // result for pending operation.  Access to this should be protected using the
// // resultLock.
// struct SignInResult {
//   int result_code;
//   bool finished;
// };
// 
// std::unique_ptr<SignInResult> currentResult_;
// 
// NSRecursiveLock *resultLock = [NSRecursiveLock alloc];

@implementation GoogleSignInHandler

GIDConfiguration* signInConfiguration = nil;
NSString* loginHint = nil;
NSMutableArray* additionalScopes = nil;
BOOL modalOpen;

+ (GoogleSignInHandler *)sharedInstance 
{
    static dispatch_once_t once;
    static GoogleSignInHandler *sharedInstance;
    dispatch_once(&once, ^{
        sharedInstance = [self alloc];
    });
    return sharedInstance;
}

/**
 * Overload the presenting of the UI so we can pause the Unity player.
 */
- (void)signIn:(GIDSignIn *)signIn
        presentViewController:(UIViewController *)viewController 
{
    UnityPause(true);
    [UnityGetGLViewController() presentViewController:viewController
                                animated:YES
                                completion:nil];
}

/**
 * Overload the dismissing so we can resume the Unity player.
 */
- (void)signIn:(GIDSignIn *)signIn
        dismissViewController:(UIViewController *)viewController 
{
    UnityPause(false);
    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];
}

/**
 * The sign-in flow has finished and was successful if |error| is |nil|.
 * Map the errors from the iOS SDK back to the Android values for consistency's
 * sake in the Unity layer.
 */
- (void)signIn:(GIDSignIn *)signIn
        didSignInForUser:(GIDGoogleUser *)user
        withError:(NSError *)_error 
{
    [GoogleSignInHandler sharedInstance]->modalOpen = NO;

    int result = kStatusCodeSuccess;
    if( _error == nil ) 
    {
        NSLog(@"didSignInForUser: SUCCESS");
    } 
    else 
    {
        NSLog(@"didSignInForUser: %@", _error.localizedDescription);
        switch( _error.code ) 
        {
            case kGIDSignInErrorCodeUnknown:
                result = kStatusCodeError;
                break;
                
            case kGIDSignInErrorCodeKeychain:
                result = kStatusCodeInternalError;
                break;
                
            case kGIDSignInErrorCodeHasNoAuthInKeychain:
                result = kStatusCodeError;
                break;
                
            case kGIDSignInErrorCodeCanceled:
                result = kStatusCodeCanceled;
                break;
                
            default:
                NSLog(@"Unmapped error code: %ld, returning Error",static_cast<long>(_error.code));
                result = kStatusCodeError;
        }
        
        // s'assurer que le user soit null en cas d'erreur
        user = nil;
    }
    
    UnpauseUnityPlayer();
    
    // UNITY SEND MESSAGE LOGIN STATUS
    DoSendUnityMessage(result,user);
}

// Finished disconnecting |user| from the app successfully if |error| is |nil|.
- (void)signIn:(GIDSignIn *)signIn
        didDisconnectWithUser:(GIDGoogleUser *)user
        withError:(NSError *)_error 
{
    if( _error == nil ) 
    {
        NSLog(@"didDisconnectWithUser: SUCCESS");
    } 
    else 
    {
        NSLog(@"didDisconnectWithUser: %@", _error);
    }
    
    // UNITY SEND MESSAGE LOGOUT STATUS
    DoSendUnityMessage(-2,nil);
}

void DoSendUnityMessage(int status, GIDGoogleUser* user)
{
    NSDictionary *dic;
    if( user != nil )
    {
        NSString* img = user.profile != nil && user.profile.hasImage ? [[user.profile imageURLWithDimension:1000] absoluteString] : @"";
        dic = @{
            @"Status":      [NSNumber numberWithInt:status],
            @"DisplayName": user.profile != nil ? user.profile.name : @"",
            @"GivenName":   user.profile != nil && user.profile.givenName  != nil ? user.profile.givenName : @"",
            @"FamilyName":  user.profile != nil && user.profile.familyName != nil ? user.profile.familyName : @"",
            @"Email":       user.profile != nil ? user.profile.email : @"",
            @"UserId":      user.userID,
            @"PhotoUrl":    img != nil ? img : @""
        };
    }
    else
    {
        dic = @{
            @"Status": [NSNumber numberWithInt:status]
        };
    }

    NSDictionary *result = @{
        @"result": dic
    };

    NSData *data        = [NSJSONSerialization dataWithJSONObject:result options:kNilOptions error:nil];
    NSString *json      = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    const char *payload = (const char*) [json UTF8String];
    
    NSLog(@"OnSignInResult: %@", json);
    UnitySendMessage( "GoogleSignInHelperObject", "OnSignInResult", payload );
}

@end

/**
 * These are the external "C" methods that are imported by the Unity C# code.
 * The parameters are intended to be primative, easy to marshall.
 */
extern "C" {

    static NSString* gameObjectName        = @"GoogleSignInHelperObject";
    static NSString* deeplinkMethodName    = @"OnSignInResult";
    
    // There is no public unity header, need to declare this manually:
    // http://answers.unity3d.com/questions/58322/calling-unitysendmessage-requires-which-library-on.html
    extern void UnitySendMessage(const char *, const char *, const char *);
    
    
    /**
     * forcer la fermeture du dialogue si affiche
     */
    void GoogleSignIn_CloseDialog()
    {
        if( [GoogleSignInHandler sharedInstance]->modalOpen == YES )
        {
            [GoogleSignInHandler sharedInstance]->modalOpen = NO;
            [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];
        }
    }
    
    
    void GoogleSignIn_EnableDebugLogging(bool flag) 
    {
        if( flag ) 
        {
            NSLog(@"GoogleSignIn: No optional logging available on iOS");
        }
    }
    
    /*
          String webClientId,
          boolean requestAuthCode,
          boolean forceRefreshToken,
          boolean requestEmail,
          boolean requestIdToken,
          boolean requestProfile,
          String defaultAccountName
                */
    
    /**
     * Configures the GIDSignIn instance.  The first parameter is unused in iOS.
     * It is here to make the API between Android and iOS uniform.
     */
    void GoogleSignIn_Configure(const char *clientId,
                                const char *webClientId,
                                bool requestAuthCode,
                                bool forceTokenRefresh, 
                                bool requestEmail,
                                bool requestIdToken, 
                                bool requestProfile,
                                const char *accountName) 
    {
        NSString* nsClientID = [NSString stringWithUTF8String:clientId];
        GIDConfiguration* config;
        if( webClientId ) 
        {
            config = [[GIDConfiguration alloc] initWithClientID:nsClientID serverClientID:[NSString stringWithUTF8String:webClientId]];
        }
        else
        {
            config = [[GIDConfiguration alloc] initWithClientID:nsClientID];
        }
        [GoogleSignInHandler sharedInstance]->signInConfiguration = config;
        
        // save the configuration in the GIDSignIn sharedInstance
        GIDSignIn.sharedInstance.configuration = config;
       
        if( accountName ) 
        {
            [GoogleSignInHandler sharedInstance]->loginHint = [NSString stringWithUTF8String:accountName];
        }
    }
    
    /**
     Starts the sign-in process.  Returns and error result if error, null otherwise.
     */
    // static SignInResult *startSignIn() {
    //   bool busy = false;
    //   [resultLock lock];
    //   if (!currentResult_ || currentResult_->finished) {
    //     currentResult_.reset(new SignInResult());
    //     currentResult_->result_code = 0;
    //     currentResult_->finished = false;
    //   } else {
    //     busy = true;
    //   }
    //   [resultLock unlock];
    // 
    //   if (busy) {
    //     NSLog(@"ERROR: There is already a pending sign-in operation.");
    //     // Returned to the caller, should be deleted by calling
    //     // GoogleSignIn_DisposeFuture().
    //     return new SignInResult{.result_code = kStatusCodeDeveloperError,
    //                             .finished = true};
    //   }
    //   return nullptr;
    // }
    
    /**
     * Sign-In.  The return value is a pointer to the currentResult object.
     */
    void GoogleSignIn_SignIn() 
    {
        [GoogleSignInHandler sharedInstance]->modalOpen = YES;
        [GIDSignIn.sharedInstance signInWithPresentingViewController:UnityGetGLViewController()
                                                                hint:[GoogleSignInHandler sharedInstance]->loginHint
                                                    additionalScopes:[GoogleSignInHandler sharedInstance]->additionalScopes
                                                          completion:^(GIDSignInResult * _Nullable signInResult, NSError * _Nullable error)
         {
            [[GoogleSignInHandler sharedInstance] signIn:[GIDSignIn sharedInstance]
                                        didSignInForUser:GIDSignIn.sharedInstance.currentUser
                                               withError:error];
        }];
        
        
        /*[[GIDSignIn sharedInstance] signInWithConfiguration:[GoogleSignInHandler sharedInstance]->signInConfiguration
                                   presentingViewController:UnityGetGLViewController()
                                                       hint:[GoogleSignInHandler sharedInstance]->loginHint
                                                   callback:^(GIDGoogleUser *user, NSError *error) {
            [[GoogleSignInHandler sharedInstance] signIn:[GIDSignIn sharedInstance] didSignInForUser:user withError:error];
        }];*/
    }
    
    /**
     * Attempt a silent sign-in. Return value is the pointer to the currentResult
     * object.
     */
    void GoogleSignIn_SignInSilently() 
    {
        [GIDSignIn.sharedInstance restorePreviousSignInWithCompletion:^(GIDGoogleUser * _Nullable user, NSError * _Nullable error)
         {
            [[GoogleSignInHandler sharedInstance] signIn:[GIDSignIn sharedInstance]
                                        didSignInForUser:GIDSignIn.sharedInstance.currentUser
                                               withError:error];
        }];
        
        /*[[GIDSignIn sharedInstance] restorePreviousSignInWithCallback:^(GIDGoogleUser *user, NSError *error) {
            [[GoogleSignInHandler sharedInstance] signIn:[GIDSignIn sharedInstance] didSignInForUser:user withError:error];
        }];*/
    }
    
    void GoogleSignIn_Signout() 
    {
        //GIDSignIn *signIn = [GIDSignIn sharedInstance];
        //[signIn signOut];
        
        [GIDSignIn.sharedInstance signOut];
        // SEND MESSAGE SIGNOUT COMPLETE
        DoSendUnityMessage(-2,nil);
    }
    
    void GoogleSignIn_Disconnect() 
    {
        [GIDSignIn.sharedInstance disconnectWithCompletion:^(NSError * _Nullable error) {
            [[GoogleSignInHandler sharedInstance] signIn:[GIDSignIn sharedInstance] didDisconnectWithUser:nil withError:error];
        }];
        
        /*GIDSignIn *signIn = [GIDSignIn sharedInstance];
        [signIn disconnectWithCallback:^(NSError *error) {
            [[GoogleSignInHandler sharedInstance] signIn:[GIDSignIn sharedInstance] didDisconnectWithUser:nil withError:error];
        }];*/
    }

} // extern "C"
