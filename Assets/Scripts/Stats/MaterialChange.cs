using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// [Michael Szolowicz]
/// Updates material color based on current player status.
/// </summary>

public class MaterialChange : MonoBehaviour
{
    public PlayerStats_Szolo stats;
    public MeshRenderer meshRenderer;

    protected Color defaultColor;
    [SerializeField] protected Color burnColor = Color.red;

    protected void Start()
    {
        if(meshRenderer != null)
        {
            defaultColor = meshRenderer.material.color;
        }

        if(stats != null)
        {
            stats.AddInflictBurnCallback(Burned);
            stats.AddClearBurnCallback(ClearBurn);
        }
    }

    private void Burned()
    {
        meshRenderer.material.color = burnColor;
    }

    private void ClearBurn()
    {
        meshRenderer.material.color = defaultColor;
    }

}
