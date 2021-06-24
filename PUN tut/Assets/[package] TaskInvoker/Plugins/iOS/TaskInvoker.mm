
#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

// Define type of the C# callback delegate
typedef void (*UnityIntEvent) (int key);

@interface TaskInvoker : NSObject
{
    @public
    UnityIntEvent InvokedHandler;
    @public
    UnityIntEvent ExpiredHandler;
}
@end

@implementation TaskInvoker

static TaskInvoker *Instance = NULL;
static NSMutableDictionary *Tasks;
static int TaskKeys = 1;

// Starts a new background task and returns its task key
-(int)start:(int) delay
{
    // Keep track of taskIDs using dictionary keys
    int key = TaskKeys;
    if (TaskKeys < INT_MAX) TaskKeys++;
    else TaskKeys = 1;
    
    // Ask for excecution time from iOS
    NSUInteger taskId = [[UIApplication sharedApplication] beginBackgroundTaskWithExpirationHandler:^{
        // Handle task expiration
        [Instance expire:key];
    }];
    
    // Call invoke after a delay on the main thread
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, delay * NSEC_PER_MSEC), dispatch_get_main_queue(), ^{
        [Instance invoke:key withDelay:delay];
    });
    
    // Keep track of running tasks
    [Tasks setObject:[NSNumber numberWithUnsignedLong:taskId] forKey:[NSNumber numberWithInt:key]];
     
    return key;
}

// Invokes the C# expiration event and stops the task
-(void)expire:(int)key
{
    NSNumber *taskNumber = [Tasks objectForKey:[NSNumber numberWithInt:key]];
    if (taskNumber)
    {
        Instance->ExpiredHandler(key);
        [Instance stop:key];
    }
}

// Stops a task with the given key
-(void)stop:(int)key
{
    NSNumber *taskNumber = [Tasks objectForKey:[NSNumber numberWithInt:key]];
    if (taskNumber)
    {
        NSUInteger taskId = [taskNumber unsignedLongValue];
        
        // End the background task
        if (taskId != UIBackgroundTaskInvalid){
            [[UIApplication sharedApplication] endBackgroundTask:taskId];
        }
        
        // Keep track of running tasks
        [Tasks removeObjectForKey:[NSNumber numberWithInt:key]];
    }
}

// Stops all running tasks
-(void)stopAllTasks
{
    // Stop all tasks in the Tasks dictionary
    for (NSNumber *key in Tasks.allKeys)
    {
        NSNumber *taskNumber = [Tasks objectForKey:key];
        if (taskNumber)
        {
            // End the background task
            NSUInteger taskId = [taskNumber unsignedLongValue];
            if (taskId != UIBackgroundTaskInvalid){
                [[UIApplication sharedApplication] endBackgroundTask:taskId];
            }
        }
    }
    
    [Tasks removeAllObjects];
}

// Invokes the C# callback and calls itself after delay
-(void)invoke:(int)key withDelay:(int) delay
{
    NSNumber *taskNumber = [Tasks objectForKey:[NSNumber numberWithInt:key]];
    if (taskNumber)
    {
        // Invoke C# callback
        Instance->InvokedHandler(key);
        
        // Reinvoke after delay
        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, delay * NSEC_PER_MSEC), dispatch_get_main_queue(), ^{
            [Instance invoke:key withDelay:delay];
        });
    }
}

extern "C"
{
    int startTask(int delay, UnityIntEvent invokedHandler, UnityIntEvent expiredHandler)
    {
        if (Instance == NULL)
        {
            Instance = [[TaskInvoker alloc] init];
            Tasks = [NSMutableDictionary dictionary];
        }

        // Set event handlers
        Instance->InvokedHandler = invokedHandler;
        Instance->ExpiredHandler = expiredHandler;

        // Handle invalid arguments
        if (invokedHandler == NULL){
            NSLog(@"StartTask called with null invokedHandler");
            return -1;
        } else if (expiredHandler == NULL){
            NSLog(@"StartTask called with null expiredHandler");
            return -1;
        }
        
        // Enforce positive delay
        if (delay <= 0) delay = 1;

        // Start task and return task key
        return [Instance start:delay];
    }
    
    void stopTask(int key)
    {
        if (Instance != NULL){
            [Instance stop:key];
        }
    }

    void stopAllTasks()
    {
        if (Instance != NULL){
            [Instance stopAllTasks];
        }
    }
}

@end
