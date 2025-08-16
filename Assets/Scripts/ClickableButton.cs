using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ButtonRole
{
    Calibrate,
    StartHost,
    StartClient,
    ReadyToPlay,
    StopConnecting
}

public class ClickableButton : MonoBehaviour
{
    public ButtonRole buttonRole;
    float hoverTime = 0f;
    bool isHovered = false;
    Image imageComponent;
    Color startColor;


    private void Start()
    {
        imageComponent = GetComponent<Image>();
        startColor = imageComponent.color;
    }


    private void Update()
    {
        if (isHovered)
        {
            hoverTime += Time.deltaTime;
            if (hoverTime > 2.0f)
            {
                hoverTime = 0f;
                isHovered = false;
                imageComponent.color = startColor;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isHovered)
        {
            isHovered = true;
            Color color = startColor;
            color.g -= 0.2f;
            imageComponent.color = color;

            if (other.CompareTag("LeftIndexFinger"))
            {
                MenuManager.Instance.VibrateController(0);
            }
            else if (other.CompareTag("RightIndexFinger"))
            {
                MenuManager.Instance.VibrateController(1);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (hoverTime <= 2.0f)
        {
            imageComponent.color = startColor;
            isHovered = false;

            switch (buttonRole)
            {
                case ButtonRole.Calibrate:
                    gameObject.SetActive(false);
                    MenuManager.Instance.CalibrateTracker(gameObject);
                    break;
                case ButtonRole.StartHost:
                    MenuManager.Instance.StartHost();
                    break;
                case ButtonRole.StartClient:
                    MenuManager.Instance.StartClient();
                    break;
                case ButtonRole.ReadyToPlay:
                    MenuManager.Instance.ReadyToPlay();
                    break;
                case ButtonRole.StopConnecting:
                    MenuManager.Instance.StopClient();
                    break;
            }

            if (other.CompareTag("LeftIndexFinger"))
            {
                MenuManager.Instance.VibrateController(0);
            }
            else if (other.CompareTag("RightIndexFinger"))
            {
                MenuManager.Instance.VibrateController(1);
            }
        }
    }

    public void DisplayButtonAgain() {
        StartCoroutine(DisplayButtonAgainCoroutine());
    }

    private IEnumerator DisplayButtonAgainCoroutine()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(true);
    }
}
