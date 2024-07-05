using UnityEngine;
using System.Collections;

public class InfiniteLoopWatcher : MonoBehaviour
{
    private float maxFrameTime = 1.0f; // Maximum allowed frame time in seconds
    private float frameStartTime;

    void Start()
    {
        StartCoroutine(WatchForInfiniteLoop());
    }

    IEnumerator WatchForInfiniteLoop()
    {
        while (true)
        {
            frameStartTime = Time.realtimeSinceStartup;
            yield return new WaitForEndOfFrame();

            float frameTime = Time.realtimeSinceStartup - frameStartTime;
            if (frameTime > maxFrameTime)
            {
                Debug.LogError("Infinite loop detected! Stopping the game.");
                StopGame();
                yield break; // Exit the coroutine
            }
        }
    }

    void StopGame()
    {
        // You can implement any game stopping logic here, like showing a warning screen
        // or pausing the game. For simplicity, let's just stop the game time.

        UnityEditor.EditorApplication.isPlaying = false;
    }
}