using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthArgs : EventArgs
{
    public int MaxLives { get; private set; }
    public int CurrentLives { get; private set; }

    public HealthArgs(int currentLives,int maxLives)
    {
        CurrentLives = currentLives;
        MaxLives = maxLives;
    }
    public HealthArgs GetUpdatedArgs(int currentLives, int maxLives) //to avoid GC overhead
    {
        CurrentLives = currentLives;
        MaxLives = maxLives;
        return this;
    }
}
public class Health : MonoBehaviour, IDamageable,IHealable, IResettable
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    [SerializeField] private HeartsBarView healthBarView;

    private HealthArgs healthArgs;
    public event EventHandler<HealthArgs> OnHealthChanged = delegate { };
    public event Action OnOutOfHealth;

    private int defaultMaxLives = 1;//PLAYER DATA ScriptableObject
    void Awake()
    {
        MaxHealth = 1;
        CurrentHealth = 1;
        healthArgs = new HealthArgs(CurrentHealth, MaxHealth);
        ResetToDefault();
    } 
    public void TakeDamage(int amount)
    {
        if (CurrentHealth > 0){
            CurrentHealth -= amount;
            if (CurrentHealth <= 0){
                CurrentHealth = 0;
                OnOutOfHealth?.Invoke();
            }
            OnHealthChanged(this, healthArgs.GetUpdatedArgs(CurrentHealth, MaxHealth));
        }
    }
    public void Heal(int amount)
    {
        if (CurrentHealth < MaxHealth)
            CurrentHealth += amount;
        OnHealthChanged(this, healthArgs.GetUpdatedArgs(CurrentHealth, MaxHealth));
        healthBarView.UpdateHealthBar(this, healthArgs.GetUpdatedArgs(CurrentHealth, MaxHealth));
    }
    public void ResetToDefault()
    {
        MaxHealth = defaultMaxLives; // PlayerData.MaxHealth
        CurrentHealth = MaxHealth;
        OnHealthChanged(this, healthArgs.GetUpdatedArgs(CurrentHealth, MaxHealth));
    }
}
