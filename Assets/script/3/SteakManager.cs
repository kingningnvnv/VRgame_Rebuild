using UnityEngine;

public enum SteakCookingStepType
{
    Salt,
    Pepper,
    Oil,
    Garlic,
    Rosemary,
    Butter
}

public class SteakManager : MonoBehaviour
{
    [Header("Step State")]
    public bool HasSalt = false;
    public bool HasGarlic = false;
    public bool HasRosemary = false;
    public bool HasPepper = false;
    public bool HasButter = false;
    public bool HasOil = false;

    [Header("Pan Reference")]
    public PanHeatReceiver currentPanHeatReceiver;

    [Header("Cooking Order Debug")]
    [SerializeField] private int cookingOrderCounter = 0;
    [SerializeField] private int saltOrder = -1;
    [SerializeField] private int pepperOrder = -1;
    [SerializeField] private int oilOrder = -1;
    [SerializeField] private int garlicOrder = -1;
    [SerializeField] private int rosemaryOrder = -1;
    [SerializeField] private int butterOrder = -1;

    private void Update()
    {
        // 如果牛排当前在一个锅里，并且这个锅已经有油
        // 那么牛排自动记录自己已经完成 Oil 步骤
        if (currentPanHeatReceiver != null)
        {
            if (currentPanHeatReceiver.HasOil && !HasOil)
            {
                HasOil = true;
                RegisterCookingStep(SteakCookingStepType.Oil);
            }

            if (currentPanHeatReceiver.HasButter && !HasButter)
            {
                HasButter = true;
                RegisterCookingStep(SteakCookingStepType.Butter);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pan"))
        {
            currentPanHeatReceiver = collision.gameObject.GetComponentInParent<PanHeatReceiver>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pan"))
        {
            currentPanHeatReceiver = null;
        }
    }

    public void RegisterCookingStep(SteakCookingStepType stepType)
    {
        if (HasRecordedStep(stepType)) return;

        cookingOrderCounter++;

        switch (stepType)
        {
            case SteakCookingStepType.Salt:
                HasSalt = true;
                saltOrder = cookingOrderCounter;
                break;

            case SteakCookingStepType.Pepper:
                HasPepper = true;
                pepperOrder = cookingOrderCounter;
                break;

            case SteakCookingStepType.Oil:
                HasOil = true;
                oilOrder = cookingOrderCounter;
                break;

            case SteakCookingStepType.Garlic:
                HasGarlic = true;
                garlicOrder = cookingOrderCounter;
                break;

            case SteakCookingStepType.Rosemary:
                HasRosemary = true;
                rosemaryOrder = cookingOrderCounter;
                break;

            case SteakCookingStepType.Butter:
                HasButter = true;
                butterOrder = cookingOrderCounter;
                break;
        }

        Debug.Log($"{gameObject.name} recorded cooking step: {stepType}, order = {cookingOrderCounter}");
    }

    public bool HasRecordedStep(SteakCookingStepType stepType)
    {
        switch (stepType)
        {
            case SteakCookingStepType.Salt:
                return saltOrder > 0;
            case SteakCookingStepType.Pepper:
                return pepperOrder > 0;
            case SteakCookingStepType.Oil:
                return oilOrder > 0;
            case SteakCookingStepType.Garlic:
                return garlicOrder > 0;
            case SteakCookingStepType.Rosemary:
                return rosemaryOrder > 0;
            case SteakCookingStepType.Butter:
                return butterOrder > 0;
        }

        return false;
    }

    public int GetCookingStepOrder(SteakCookingStepType stepType)
    {
        switch (stepType)
        {
            case SteakCookingStepType.Salt:
                return saltOrder;
            case SteakCookingStepType.Pepper:
                return pepperOrder;
            case SteakCookingStepType.Oil:
                return oilOrder;
            case SteakCookingStepType.Garlic:
                return garlicOrder;
            case SteakCookingStepType.Rosemary:
                return rosemaryOrder;
            case SteakCookingStepType.Butter:
                return butterOrder;
        }

        return -1;
    }

    public bool IsCookingStepOrderCorrect(SteakCookingStepType stepType)
    {
        switch (stepType)
        {
            case SteakCookingStepType.Salt:
                return IsSaltCorrect();

            case SteakCookingStepType.Pepper:
                return IsPepperCorrect();

            case SteakCookingStepType.Oil:
                return IsOilCorrect();

            case SteakCookingStepType.Garlic:
                return IsLateIngredientCorrect(SteakCookingStepType.Garlic);

            case SteakCookingStepType.Rosemary:
                return IsLateIngredientCorrect(SteakCookingStepType.Rosemary);

            case SteakCookingStepType.Butter:
                return IsLateIngredientCorrect(SteakCookingStepType.Butter);
        }

        return false;
    }

    private bool IsSaltCorrect()
    {
        if (!HasRecordedStep(SteakCookingStepType.Salt)) return false;

        int order = GetCookingStepOrder(SteakCookingStepType.Salt);

        if (HasRecordedStep(SteakCookingStepType.Oil) && order >= oilOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Garlic) && order >= garlicOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Rosemary) && order >= rosemaryOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Butter) && order >= butterOrder) return false;

        return true;
    }

    private bool IsPepperCorrect()
    {
        if (!HasRecordedStep(SteakCookingStepType.Pepper)) return false;

        int order = GetCookingStepOrder(SteakCookingStepType.Pepper);

        if (HasRecordedStep(SteakCookingStepType.Oil) && order >= oilOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Garlic) && order >= garlicOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Rosemary) && order >= rosemaryOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Butter) && order >= butterOrder) return false;

        return true;
    }

    private bool IsOilCorrect()
    {
        if (!HasRecordedStep(SteakCookingStepType.Oil)) return false;
        if (!HasRecordedStep(SteakCookingStepType.Salt)) return false;
        if (!HasRecordedStep(SteakCookingStepType.Pepper)) return false;

        int order = GetCookingStepOrder(SteakCookingStepType.Oil);

        if (order <= saltOrder) return false;
        if (order <= pepperOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Garlic) && order >= garlicOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Rosemary) && order >= rosemaryOrder) return false;
        if (HasRecordedStep(SteakCookingStepType.Butter) && order >= butterOrder) return false;

        return true;
    }

    private bool IsLateIngredientCorrect(SteakCookingStepType stepType)
    {
        if (!HasRecordedStep(stepType)) return false;
        if (!HasRecordedStep(SteakCookingStepType.Salt)) return false;
        if (!HasRecordedStep(SteakCookingStepType.Pepper)) return false;
        if (!HasRecordedStep(SteakCookingStepType.Oil)) return false;

        int order = GetCookingStepOrder(stepType);

        if (order <= saltOrder) return false;
        if (order <= pepperOrder) return false;
        if (order <= oilOrder) return false;

        return true;
    }
}