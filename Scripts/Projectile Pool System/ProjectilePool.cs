using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
	
    #region Singleton

    private static ProjectilePool instance;


    public static ProjectilePool GetInstance () {
        return instance;
    }

    private void Start () {
		if (instance == null)
			instance = this;
		else if (instance != this) {
            Destroy(gameObject);
			return;
		}

		projectileControllerPrefab = projectilePrefab.GetComponent<ProjectileController>();

		while (transform.childCount < initialPoolSize)
			GenerateNewProjectile();
    }

	#endregion

	#region Projectile Pool

	[SerializeField] private GameObject projectilePrefab;
	private ProjectileController projectileControllerPrefab;

	[SerializeField] private int initialPoolSize = 10;
    private Queue<ProjectileController> projectilePool = new Queue<ProjectileController>();


    private void GenerateNewProjectile () {
        //GameObject projectileGameObject = Instantiate(projectilePrefab);
        //projectileGameObject.transform.SetParent(transform);
        //ProjectileController projectileController = projectileGameObject.GetComponent<ProjectileController>();
        
        ProjectileController projectileController = projectileControllerPrefab.Clone();
        projectilePool.Enqueue(projectileController);

		projectileController.gameObject.transform.SetParent(transform);
    }

    public ProjectileController GetProjectile () {
        if (projectilePool.Count == 0)
            GenerateNewProjectile();

        ProjectileController projectile = projectilePool.Dequeue();
		projectile.transform.SetParent(null);
        return projectile;
    }

    public void ReturnProjectile (ProjectileController projectileControler) {
        projectilePool.Enqueue(projectileControler);

		projectileControler.transform.SetParent(transform);
		projectileControler.transform.localPosition = Vector2.zero;
    }

	#endregion


}