using System.Collections.Generic;
using EditorCools.Editor;
using UnityEngine;

public class AirStrikeHandler : MonoBehaviour
{
    public GameObject AirStrikeButton;

    public void Start() => AirStrikeButton.SetActive(true);
    public void StartAirStrike()
    {
        DamageAllEnemies();
        AirStrikeButton.SetActive(false);
    }

    private void DamageAllEnemies()
    {
        foreach (var enemy in FindObjectsByType<UnitController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            enemy.TakeDamage(enemy.unitData.maxHealth);
        }
    }
}
