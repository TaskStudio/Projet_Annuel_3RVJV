using UnityEngine;
using UnityEngine.UIElements;

public class StatisticsUpdater : MonoBehaviour
{
    public static StatisticsUpdater Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateStatisticsContainer(BaseObject profile)
    {
        UIManager.Instance.statisticsScrollView.Clear();
        if (profile == null) return;

        if (!string.IsNullOrEmpty(profile.Data.objectName))
        {
            var nameLabel = new Label { text = profile.Data.objectName };
            UIManager.Instance.statisticsScrollView.Add(nameLabel);
        }

        if (!string.IsNullOrEmpty(profile.Data.description))
        {
            var descriptionLabel = new Label { text = profile.Data.description };
            UIManager.Instance.statisticsScrollView.Add(descriptionLabel);
        }

        if (profile is Unit unit)
        {
            UnitData unitData = (UnitData)unit.Data;

            if (unit.currentHealth != 0)
            {
                var hpLabel = new Label { text = "HP : " + unit.currentHealth };
                UIManager.Instance.statisticsScrollView.Add(hpLabel);
            }

            if (unit.currentMana != 0)
            {
                var manaLabel = new Label { text = "Mana : " + unit.currentMana };
                UIManager.Instance.statisticsScrollView.Add(manaLabel);
            }

            if (unit.attackSpeed != 0)
            {
                var attackSpeedLabel = new Label { text = "Attack Speed : " + unit.attackSpeed };
                UIManager.Instance.statisticsScrollView.Add(attackSpeedLabel);
            }

            if (unit.movementSpeed != 0)
            {
                var movementSpeedLabel = new Label { text = "Movement Speed : " + unit.movementSpeed };
                UIManager.Instance.statisticsScrollView.Add(movementSpeedLabel);
            }

            var raceLabel = new Label { text = "Faction : " + unitData.faction };
            UIManager.Instance.statisticsScrollView.Add(raceLabel);
        }
    }
}