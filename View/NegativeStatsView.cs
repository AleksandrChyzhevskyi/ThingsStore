using UnityEngine;

public class NegativeStatsView : MonoBehaviour
{
    public Transform DifferenceParametersOfThings;
    public Transform ItemParametersContent;

    public void SetBehaviourDifferenceParametersOfThings(bool state) => 
        DifferenceParametersOfThings.parent.gameObject.SetActive(state);
}
