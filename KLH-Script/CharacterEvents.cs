using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents
{
    // Character damaged and damage value
    public static UnityEvent<GameObject, int> characterDamaged = new UnityEvent<GameObject, int>();

    // Character healed and amount healed
    public static UnityEvent<GameObject, int> characterHealed = new UnityEvent<GameObject, int>();
}
