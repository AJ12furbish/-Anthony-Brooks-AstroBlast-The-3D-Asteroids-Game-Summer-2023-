using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    //Self generating code.
    public static DamagePopup Create(Vector3 position, int damageAmount, Color inColor, bool isCrit)
    {
        GameAssets i = GameObject.Find("GameAssets").GetComponent<GameAssets>();
        Transform damagePopupTransform = Instantiate(i.pfDamagePopup, position, Quaternion.Euler(90, 0, 0));
        DamagePopup damagePopupScript = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopupScript.setup(damageAmount, inColor, isCrit);

        return damagePopupScript;
    }



    // Instance variables
    private static int sortingOrder = 0;

    private TextMeshPro textMesh;
    //Store the desired color of the text.
    private Color textColor;
    //How long to disappear
    [SerializeField] private float disappearTimerMax = 1f;
    private float disappearTimer = 0;
    [SerializeField] private float moveXSpeed = 0f;
    [SerializeField] private float moveXRandom = 1.0f;
    [SerializeField] private float moveYSpeed = 0f;
    [SerializeField] private float moveZSpeed = 2f;
    private Vector3 moveVector;

    private string hexColorCrit = "#FF2B00"; 

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    //Setup for no input color.  It will use the color of the prefab.
    public void setup(int damageAmount)
    {
        textMesh.SetText("" + damageAmount);
        textColor = textMesh.color;
    }

    //Set for receiving an input color, and will override the prefab color
    public void setup(int damageAmount, Color inTextColor, bool isCrit)
    {
        textMesh.SetText("" + damageAmount);
        textColor = inTextColor;
        textMesh.color = textColor;
        disappearTimer = disappearTimerMax; //Set the timer to start disappearing
        //moveXSpeed = Random.Range(0.8f, 1.6f) * UnityEngine.Random.value < 0.5f ? 1 : -1; //Generate 0.8 to 1.6 either pos or neg
        moveXSpeed = Random.Range(-moveXRandom, moveXRandom);
        moveVector = new Vector3(moveXSpeed, moveYSpeed, moveZSpeed);

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        if (isCrit == true)
        {
            textMesh.fontSize = Mathf.RoundToInt(textMesh.fontSize * 1.5f);
            if (ColorUtility.TryParseHtmlString(hexColorCrit, out textColor))
                textMesh.color = textColor;
        }
    }


    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 2f * Time.deltaTime;

        disappearTimer -= Time.deltaTime;

        //Optional: Make the text get bigger and smaller
        if (disappearTimer > disappearTimerMax * 0.5f)
        {
            //first half of timer
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            //second half of timer
            float increaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * increaseScaleAmount * Time.deltaTime;
        }

        if (disappearTimer <= 0)
        {
            float disappearSpeed = 3f; //Decays 300% alpha per sec, basically 100% in .33s
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
                Destroy(gameObject);
        }

    }
}
