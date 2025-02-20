using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Transform lobbyCameraPosition;
    public XROrigin xrOrigin;
    public Canvas screenFade;
    public float fadeTime = 2f;
    private Image _image;

    public void Start()
    {
        screenFade.gameObject.SetActive(true);
        _image = screenFade.GetComponentInChildren<Image>();
        StartCoroutine(Fade(fadeTime, false));
    }

    public void MovePlayerToLobby(bool fade)
    {
        StartCoroutine(MovePlayerToLobbyCoroutine(fade));
    }

    private IEnumerator MovePlayerToLobbyCoroutine(bool fade)
    {
        if (fade)
        {
            StartCoroutine(Fade(fadeTime, true));
            yield return new WaitForSeconds(fadeTime);
        }

        var rotationDifference = lobbyCameraPosition.rotation * Quaternion.Inverse(xrOrigin.Camera.transform.rotation);
        var eulerDifference = rotationDifference.eulerAngles;

        xrOrigin.MoveCameraToWorldLocation(lobbyCameraPosition.position);
        xrOrigin.RotateAroundCameraUsingOriginUp(eulerDifference.y);

        if (fade)
        {
            StartCoroutine(Fade(fadeTime, false));
        }
    }

    private IEnumerator Fade(float seconds, bool fadeOut)
    {
        if(fadeOut) screenFade.gameObject.SetActive(true);
        var startTime = Time.time;
        var currentTime = startTime;
        var elapsedTime = currentTime - startTime;
        while (elapsedTime < seconds)
        {
            currentTime = Time.time;
            var transparency = fadeOut ? elapsedTime : seconds - elapsedTime;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.InverseLerp(0, seconds, transparency));
            elapsedTime = currentTime - startTime;

            yield return new WaitForEndOfFrame();
        }
        
        if(!fadeOut) screenFade.gameObject.SetActive(false);
    }

    public void StopApplication()
    {
        StartCoroutine(StopApplicationCoroutine());
    }

    private IEnumerator StopApplicationCoroutine()
    {
        yield return new WaitForSeconds(10.0f);
        
        StartCoroutine(Fade(fadeTime, true));
        yield return new WaitForSeconds(fadeTime);

        // Check current platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Call the method to stop the application
            var activity =
                new AndroidJavaClass("com.unity3d.xrOrigin.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }
        else
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}