using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

[System.Serializable] public class GameObjectEvent : UnityEvent<GameObject> { }
[System.Serializable] public class BoolEvent : UnityEvent<bool> { }
[System.Serializable] public class ActorsEvent : UnityEvent<int, int> { }
[System.Serializable] public class IntEvent : UnityEvent<int> { }
[System.Serializable] public class PlayerEvent : UnityEvent<Player> { }