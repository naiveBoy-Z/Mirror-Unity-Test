using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ModelController : MonoBehaviour
{
    [Header("IK Object")]
    public GameObject body;
    public GameObject headTarget;
    public GameObject leftArmTarget;
    public GameObject leftArmHint;
    public GameObject rightArmTarget;
    public GameObject rightArmHint;
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
            body,
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


    public void CalibrateModelPose(int order)
    {
        List<float> bodyPartsOffsets = SyncList.instance.bodyPartsOffsetsList[order - 1];

        float armRatio = 8 / 13, forearmRatio = 5 / 13;

        //transform.localScale = new Vector3(bodyPartsOffsets[0], bodyPartsOffsets[0], bodyPartsOffsets[0]);
        //// body center:
        //bodyPartsTransform[1].position = bodyPartsTransform[1].position + new Vector3(0, bodyPartsOffsets[1], 0);
        //// foot:
        //bodyPartsTransform[2].position = bodyPartsTransform[2].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[3].position = bodyPartsTransform[3].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[4].position = bodyPartsTransform[4].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[5].position = bodyPartsTransform[5].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[16].position = bodyPartsTransform[16].position + new Vector3(0, bodyPartsOffsets[2] * 2, 0);
        //bodyPartsTransform[17].position = bodyPartsTransform[17].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[18].position = bodyPartsTransform[18].position + new Vector3(0, bodyPartsOffsets[2] * 2, 0);
        //bodyPartsTransform[19].position = bodyPartsTransform[19].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //// upper body:
        //bodyPartsTransform[6].position = bodyPartsTransform[6].position + new Vector3(0, bodyPartsOffsets[3] / 3, 0);
        //bodyPartsTransform[7].position = bodyPartsTransform[7].position + new Vector3(0, bodyPartsOffsets[3] / 3, 0);
        //bodyPartsTransform[8].position = bodyPartsTransform[8].position + new Vector3(0, bodyPartsOffsets[3] / 3, 0);
        //bodyPartsTransform[9].position = bodyPartsTransform[9].position + new Vector3(0, bodyPartsOffsets[4] / 2, 0);
        //bodyPartsTransform[10].position = bodyPartsTransform[10].position + new Vector3(0, bodyPartsOffsets[4] / 2, 0);
        //// arm:
        //bodyPartsTransform[12].position = bodyPartsTransform[12].position + new Vector3(-bodyPartsOffsets[5] * armRatio, 0, 0);
        //bodyPartsTransform[13].position = bodyPartsTransform[13].position + new Vector3(-bodyPartsOffsets[5] * forearmRatio, 0, 0);
        //bodyPartsTransform[14].position = bodyPartsTransform[14].position + new Vector3(bodyPartsOffsets[5] * armRatio, 0, 0);
        //bodyPartsTransform[15].position = bodyPartsTransform[15].position + new Vector3(bodyPartsOffsets[5] * forearmRatio, 0, 0);
        //bodyPartsTransform[20].position = bodyPartsTransform[20].position + new Vector3(-bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);
        //bodyPartsTransform[21].position = bodyPartsTransform[21].position + new Vector3(-bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);
        //bodyPartsTransform[22].position = bodyPartsTransform[22].position + new Vector3(bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);
        //bodyPartsTransform[23].position = bodyPartsTransform[23].position + new Vector3(bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);

        GetComponent<Animator>().enabled = true;
    }


    public List<Transform> GetBodyPartsTransform()
    {
        return bodyPartsTransform;
    }


    public void DisplayPlayerStatsUI(string playerName)
    {
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
