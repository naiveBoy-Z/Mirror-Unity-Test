using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ModelController : MonoBehaviour
{
    [Header("Hips")]
    public GameObject hipsBone;
    [Header("Head IK")]
    public GameObject headTarget;
    [Header("Arm IK")]
    public GameObject leftArmTarget;
    public GameObject leftArmHint;
    public GameObject rightArmTarget;
    public GameObject rightArmHint;
    [Header("Leg IK")]
    public GameObject leftLegTarget;
    public GameObject leftLegHint;
    public GameObject rightLegTarget;
    public GameObject rightLegHint;
    [Header("Body Parts Transform")]
    public List<Transform> bodyPartsTransform;
    [Header("Player UI")]
    public GameObject uiCanvas;
    public Image imgHpBar;
    public Image imgHpForegroundBar;
    public TextMeshProUGUI txtPlayerName;
    [Header("Collider Manager")]
    public List<BodyPartCollider> bodyPartsColliders;
    [Header("Player Object")]
    public Transform localPlayerCamera;
    public List<Transform> otherPlayersCanvas;
    [Header("Equipments")]
    public Transform gunMuzzle;
    [Header("Sound Effects")]
    public AudioSource audioSource;

    InputAction pressRightTrigger;


    private void Start()
    {
        pressRightTrigger = new InputAction(null, InputActionType.Button, "<ViveWaveController>{RightHand}/triggerPressed");
        pressRightTrigger.Enable();
    }


    private void Update()
    {
        foreach (var canvas in otherPlayersCanvas)
        {
            canvas.LookAt(localPlayerCamera);
        }

        if (pressRightTrigger.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(audioSource.clip);

            if (Physics.Raycast(new Ray(gunMuzzle.position, gunMuzzle.right), out RaycastHit hit, 30))
            {
                hit.collider.GetComponent<BodyPartCollider>().TakeDamage();
            }
        }
    }


    public List<GameObject> GetIkObjects()
    {
        List<GameObject> list = new()
        {
            hipsBone,
            headTarget,
            leftArmTarget,
            leftArmHint,
            rightArmTarget,
            rightArmHint,
            leftLegTarget,
            leftLegHint,
            rightLegTarget,
            rightLegHint
        };
        return list;
    }


    public List<Transform> GetBodyPartsTransform()
    {
        return bodyPartsTransform;
    }


    public void DisplayPlayerStatsUI(string playerName)
    {
        Debug.Log("________________________");
        txtPlayerName.text = playerName;
        uiCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        uiCanvas.SetActive(true);
    } 


    public void UpdateHp(int hp, int maxHp, bool isLocalPlayer)
    {
        if (!isLocalPlayer)
        {
            imgHpForegroundBar.fillAmount = (float)hp / maxHp;
        }
        if (hp <= 0)
        {
            gameObject.SetActive(false);
            if (isLocalPlayer)
            {
                UIUpdater.Instance.DisplayFinalResult(false);
            }
            else
            {
                UIUpdater.Instance.DisplayFinalResult(true);
            }
        }
    }


    public void SetPlayerInforForColliders(PlayerInfor infor)
    {
        foreach (var bodyPartCollider in bodyPartsColliders)
        {
            bodyPartCollider.player = infor;
        }
    }
}
