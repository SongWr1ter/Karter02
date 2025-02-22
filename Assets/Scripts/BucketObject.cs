using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BucketObject : MonoBehaviour
{
    private SpriteRenderer sprite;
    [Header("调酒UI")]
    [SerializeField]private TMP_Text baseText;
    [SerializeField]private TMP_Text balanceText;
    [SerializeField]private TMP_Text styleText;
    [SerializeField]private TMP_Text charcText;
    [Header("参数UI")]
    [SerializeField]private TMP_Text v5Text;
    [SerializeField]private TMP_Text v4Text;
    private Bartending.Vector5 inputBase = new Bartending.Vector5();
    private Bartending.Vector5 inputBalance = new Bartending.Vector5();
    private Bartending.Vector5 input = new Bartending.Vector5();
    private Vector4 output;
    [Header("按钮")]
    [SerializeField]private Button resetButton;
    [SerializeField]private Button submitButton;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        resetButton.onClick.AddListener(ResetIngredents);
        submitButton.onClick.AddListener(Submit);
        MessageCenter.AddListener(OnDrinkAll,MESSAGE_TYPE.DrinkAll);
        Refresh();
    }

    private void OnDisable()
    {
        MessageCenter.RemoveListener(OnDrinkAll, MESSAGE_TYPE.DrinkAll);
        resetButton.onClick.RemoveListener(ResetIngredents);
        submitButton.onClick.RemoveListener(Submit);
    }

    private void OnTrigger2DEnter(Collider2D other)
    {
        if (other.CompareTag("Ingrendient"))
        {
            sprite.DOColor(Color.cyan, 0.5f);
        }
    }
    private void OnTrigger2DExit(Collider2D other)
    {
        if (other.CompareTag("Ingrendient"))
        {
            sprite.DOColor(Color.white, 0.5f);
        }
    }

    public void Recv(Transform d)
    {
        if (d.TryGetComponent<Ingredents>(out Ingredents ing))
        {
            if (ing.type == Ingredents.IngredentType.Balance)
            {
                inputBalance = inputBalance + ing.Input;
                balanceText.text = "Balance:"+ing.iName+$"|||V5=[{inputBalance.x}],[{inputBalance.y}],[{inputBalance.z}],[{inputBalance.w}],[{inputBalance.t}]";
            }else if (ing.type == Ingredents.IngredentType.Base)
            {
                inputBase = inputBase + ing.Input;
                baseText.text = "Base:"+ing.iName+$"|||V5=[{inputBase.x}],[{inputBase.y}],[{inputBase.z}],[{inputBase.w}],[{inputBase.t}]";
            }
        }
        Refresh();
    }

    public void ReactionSum()
    {
        input = Bartending.Reaction(inputBalance, inputBase,((f1, f2) => f1+f2));
        Refresh();
    }
    public void ReactionMax()
    {
        input = Bartending.Reaction(inputBalance, inputBase,((f1, f2) => f1>f2?f1:f2));
        Refresh();
    }
    public void ReactionAvg()
    {
        input = Bartending.Reaction(inputBalance, inputBase,((f1, f2) => (f1+f2)/2.0f));
        Refresh();
    }

    public void Submit()
    {
        MessageCenter.SendMessage(new CommonMessage
        {
            Mid = (int)MESSAGE_TYPE.ServeItUp,
            content = (object)input
        },MESSAGE_TYPE.ServeItUp);
    }

    public void ResetIngredents()
    {
        Bartending.Vector5.Clear(inputBalance);
        balanceText.text = "Balance:xxx";
        Bartending.Vector5.Clear(inputBase);
        baseText.text = "Base:xxx";
        Bartending.Vector5.Clear(input);
        Refresh();
        output = Vector4.zero;
    }

    void Refresh()
    {
        v4Text.text = $"emotion=[{output.x}],[{output.y}],[{output.z}],[{output.w}]";
        v5Text.text = $"R=[{input.x}],[{input.y}],[{input.z}],[{input.w}],[{input.t}]";
    }

    public void OnDrinkAll(CommonMessage message)
    {
        output = (Vector4)message.content;
        Refresh();
    }
}