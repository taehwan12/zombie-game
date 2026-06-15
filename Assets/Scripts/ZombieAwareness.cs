using UnityEngine;
using System.Collections;

public class ZombieAwareness : MonoBehaviour
{
    public float searchRadius = 100f;
    private Transform playerTransform;

    private void Start()
    {
        GameObject player = GameObject.Find("Player Character");
        if (player != null) playerTransform = player.transform;
        StartCoroutine(AwarenessRoutine());
    }

    private IEnumerator AwarenessRoutine()
    {
        while (true)
        {
            if (playerTransform != null)
            {
                Zombie[] zombies = Object.FindObjectsByType<Zombie>(FindObjectsSortMode.None);
                foreach (var zombie in zombies)
                {
                    // If zombie doesn't have a target, manually search in a larger radius
                    // or just give it the player if it's within searchRadius
                    float dist = Vector3.Distance(zombie.transform.position, playerTransform.position);
                    if (dist < searchRadius)
                    {
                        // We use reflection or just assume we can set the targetEntity
                        // Since targetEntity is protected, we might need to be clever or just rely on UpdatePath
                        // Wait, Zombie.cs uses OverlapSphere with 20f. 
                        // Let's just move them closer if they are idle? No, that's cheating.
                        
                        // Actually, I'll just change the detection radius in the prefab instance in the scene if possible
                        // But prefabs are instantiated.
                    }
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
