
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class HuTaoAbilities : NetworkBehaviour
{
    [SerializeField] private Transform shootTransform;

    [Header("Ability 1")]
    public Image abilityImage1;
    public TMP_Text abilityText1;
    public KeyCode ability1Key = KeyCode.Q;
    public float ability1Cooldown;

    public float delayBeforeDurationEnds = 9f;
    public const float ABILITY1ACTIVATIONCOST = 0.3f;
    public const float ABILITY1ATTACKINCREASE = .063f;

    [Header("Ability 2")]
    public Image abilityImage2;
    public TMP_Text abilityText2;
    public KeyCode ability2Key = KeyCode.W;
    public float ability2Cooldown;

    public Canvas ability2Canvas;
    public Image ability2Indicator;

    [Header("Ability 3")]
    public Image abilityImage3;
    public TMP_Text abilityText3;
    public KeyCode ability3Key = KeyCode.E;
    public float ability3Cooldown;

    public Canvas ability3Canvas;
    public Image ability3Indicator;

    [Header("Ability 4")]
    public Image abilityImage4;
    public TMP_Text abilityText4;
    public KeyCode ability4Key = KeyCode.R;
    public float ability4Cooldown;

    public Canvas ability4Canvas;
    public Image ability4Indicator;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;
    private bool isAbility4Cooldown = false;

    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;
    private float currentAbility4Cooldown;

    private Vector3 position;
    private RaycastHit hit;
    private Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        // Shows UI
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(0).gameObject.SetActive(true);
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(1).gameObject.SetActive(true);
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(2).gameObject.SetActive(true);
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(3).gameObject.SetActive(true);
        //NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(4).gameObject.SetActive(true);


        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;
        abilityImage4.fillAmount = 0;

        abilityText1.text = "";
        abilityText2.text = "";
        abilityText3.text = "";
        abilityText4.text = "";

        ability2Indicator.enabled = false;
        ability3Indicator.enabled = false;
        ability4Indicator.enabled = false;

        ability2Canvas.enabled = false;
        ability3Canvas.enabled = false;
        ability4Canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // TODO: Cast ability functionality
        Ability1Input();
        Ability2Input();
        Ability3Input();
        Ability4Input();

        AbilityCooldown(ref currentAbility1Cooldown, ability1Cooldown, ref isAbility1Cooldown, abilityImage1, abilityText1);
        AbilityCooldown(ref currentAbility2Cooldown, ability2Cooldown, ref isAbility2Cooldown, abilityImage2, abilityText2);
        AbilityCooldown(ref currentAbility3Cooldown, ability3Cooldown, ref isAbility3Cooldown, abilityImage3, abilityText3);
        AbilityCooldown(ref currentAbility4Cooldown, ability4Cooldown, ref isAbility4Cooldown, abilityImage4, abilityText4);

        Ability2Canvas();
        Ability3Canvas();
        Ability4Canvas();
    }

    private void Ability2Canvas()
    {
        if (ability2Indicator.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            Quaternion ab2Canvas = Quaternion.LookRotation(position - transform.position);
            ab2Canvas.eulerAngles = new Vector3(0, ab2Canvas.eulerAngles.y, ab2Canvas.eulerAngles.z);

            ability2Canvas.transform.rotation = Quaternion.Lerp(ab2Canvas, ability2Canvas.transform.rotation, 0);
        }
    }

    private void Ability3Canvas()
    {
        if (ability3Indicator.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            Quaternion ab3Canvas = Quaternion.LookRotation(position - transform.position);
            ab3Canvas.eulerAngles = new Vector3(0, ab3Canvas.eulerAngles.y, ab3Canvas.eulerAngles.z);

            ability3Canvas.transform.rotation = Quaternion.Lerp(ab3Canvas, ability3Canvas.transform.rotation, 0);
        }
    }

    private void Ability4Canvas()
    {
        if (ability4Indicator.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            Quaternion ab4Canvas = Quaternion.LookRotation(position - transform.position);
            ab4Canvas.eulerAngles = new Vector3(0, ab4Canvas.eulerAngles.y, ab4Canvas.eulerAngles.z);

            ability4Canvas.transform.rotation = Quaternion.Lerp(ab4Canvas, ability4Canvas.transform.rotation, 0);
        }
    }

    private void Ability1Input()
    {
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            ability3Canvas.enabled = false;
            ability3Indicator.enabled = false;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;

            isAbility1Cooldown = true;
            currentAbility1Cooldown = ability1Cooldown;

            CastAbility1ServerRpc();
        }
    }

    // summon projectile here
    [ServerRpc]
    private void CastAbility1ServerRpc()
    {
        // ability cost: 30% of CURRENT HP
        GameManager.Instance.TakeDamage(gameObject, GetComponent<PlayerPrefab>().Health * ABILITY1ACTIVATIONCOST);
        // atk increase (% max hp)
        GameManager.Instance.IncreaseDamage(gameObject, GetComponent<PlayerPrefab>().MaxHealth * ABILITY1ATTACKINCREASE);
        StartCoroutine(DestroyGuideToAfterlife());
    }
    IEnumerator DestroyGuideToAfterlife()
    {
        yield return new WaitForSeconds(delayBeforeDurationEnds);
        DestroyGuideToAfterlifeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyGuideToAfterlifeServerRpc()
    {
        GameManager.Instance.DecreaseDamage(gameObject, GetComponent<PlayerPrefab>().MaxHealth * ABILITY1ATTACKINCREASE);

    }
    private void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown)
        {
            ability2Canvas.enabled = true;
            ability2Indicator.enabled = true;

            ability3Canvas.enabled = false;
            ability3Indicator.enabled = false;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;

        }
            if (ability2Canvas.enabled && Input.GetMouseButtonDown(0))
            {
                isAbility2Cooldown = true;
                currentAbility2Cooldown = ability2Cooldown;

                ability2Canvas.enabled = false;
                ability2Indicator.enabled = false;
            }
    }

    private void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown)
        {
            ability3Canvas.enabled = true;
            ability3Indicator.enabled = true;

            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;

        }
        if (ability3Canvas.enabled && Input.GetMouseButtonDown(0))
        {
            isAbility3Cooldown = true;
            currentAbility3Cooldown = ability3Cooldown;

            ability3Canvas.enabled = false;
            ability3Indicator.enabled = false;
        }
    }

    private void Ability4Input()
    {
        if (Input.GetKeyDown(ability4Key) && !isAbility4Cooldown)
        {
            ability4Canvas.enabled = true;
            ability4Indicator.enabled = true;

            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            ability3Canvas.enabled = false;
            ability3Indicator.enabled = false;

        }
        if (ability4Canvas.enabled && Input.GetMouseButtonDown(0))
        {
            isAbility4Cooldown = true;
            currentAbility4Cooldown = ability4Cooldown;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;
        }
    }

    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, TMP_Text skillText)
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0f;

                if (skillImage != null)
                {
                    skillImage.fillAmount = 0f;
                }
                if (skillText != null)
                {
                    skillText.text = "";
                }
            }
            else
            {
                if (skillImage != null)
                {
                    skillImage.fillAmount = currentCooldown / maxCooldown;
                }
                if (skillText != null)
                {
                    skillText.text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
        }
    }
}
