using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public class Manager
    {
        public void FireProjectile(GameObject projectilePrefab, Transform spawnPoint, Unit caster, Unit target, float damage, List<Buffs.Buff> debuffs, Sprite projectileSprite)
        {
            if (projectilePrefab == null || spawnPoint == null) return;

            GameObject projectileObject = Object.Instantiate(projectilePrefab, spawnPoint.position, caster.transform.rotation);
            Projectiles.Projectile projectile = projectileObject.GetComponent<Projectiles.Projectile>();

            if (projectile != null)
            {
                projectile.target = target.transform;
                projectile.damage = new Damages.Damage(damage, "远程攻击", true);
                projectile.debuffs = new List<Buffs.Buff>(debuffs);
                projectile.caster = caster;

                SpriteRenderer projectileSpriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
                if (projectileSpriteRenderer != null)
                {
                    projectileSpriteRenderer.sprite = projectileSprite;
                }

                projectileObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
        }
    }
}