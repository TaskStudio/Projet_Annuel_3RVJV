using UnityEngine;
using UnityEngine.UIElements;

public class StatisticsUpdater : MonoBehaviour
{
    public static StatisticsUpdater Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void UpdateStatisticsContainer(BaseObject profile)
    {
        UIManager.Instance.statisticsScrollView.Clear();
        if (profile == null) return;

        if (!string.IsNullOrEmpty(profile.Data.objectName))
        {
            var nameLabel = new Label { text = profile.Data.objectName };
            nameLabel.AddToClassList("labels");
            UIManager.Instance.statisticsScrollView.Add(nameLabel);
        }

        if (!string.IsNullOrEmpty(profile.Data.description))
        {
            var descriptionLabel = new Label { text = profile.Data.description };
            descriptionLabel.AddToClassList("labels");
            UIManager.Instance.statisticsScrollView.Add(descriptionLabel);
        }

        if (profile is Entity entity)
        {
            if (entity.currentHealth != 0)
            {
                var hpLabel = new Label { text = "HP : " + entity.currentHealth };
                hpLabel.AddToClassList("labels");
                UIManager.Instance.statisticsScrollView.Add(hpLabel);
            }
        }

        if (profile is Unit unit)
        {
            var unitData = unit.Data;

            if (unit.currentMana != 0)
            {
                var manaLabel = new Label { text = "Mana : " + unit.currentMana };
                manaLabel.AddToClassList("labels");
                UIManager.Instance.statisticsScrollView.Add(manaLabel);
            }

            if (unit.attackSpeed != 0)
            {
                var attackSpeedLabel = new Label { text = "Attack Speed : " + unit.attackSpeed };
                attackSpeedLabel.AddToClassList("labels");
                UIManager.Instance.statisticsScrollView.Add(attackSpeedLabel);
            }

            if (unit.movementSpeed != 0)
            {
                var movementSpeedLabel = new Label { text = "Movement Speed : " + unit.movementSpeed };
                movementSpeedLabel.AddToClassList("labels");
                UIManager.Instance.statisticsScrollView.Add(movementSpeedLabel);
            }

            var raceLabel = new Label { text = "Faction : " + unitData.faction };
            raceLabel.AddToClassList("labels");
            UIManager.Instance.statisticsScrollView.Add(raceLabel);
        }
    }
}