using EXILED;
using EXILED.Extensions;
using ItemManager;
using Mirror;
using PlayableScps;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knife
{
    public class KnifeItem : CustomItem
    {
        private const int PlayerLayerMask = 1208246273;
        /*public override int MagazineCapacity => 9999;

        public override float FireRate => 1f;*/

        /*public KnifeItem(PluginMain pl)
        {
            plugin = pl;
        }*/

        public override void OnMedkitUse()
        {
            // Nullify damage from straight shot
            float damage = 0; // todo: this causes everything to be a oneshot

            Log.Info("A KNIFE!!!");
            PlayerObject.GetPlayer().SetCurrentItem(ItemType.None);
            PlayerObject.GetPlayer().SetCurrentItem(ItemType.Adrenaline);

            Transform cam = PlayerObject.GetComponent<ReferenceHub>().PlayerCameraReference;
            Ray[] rays = new Ray[1];
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = new Ray(cam.position + cam.forward, cam.forward);
            }

            RaycastHit[] hits = new RaycastHit[1];
            bool[] didHit = new bool[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                didHit[i] = Physics.Raycast(rays[i], out hits[i], 500f, PlayerLayerMask);
            }

            bool confirm = false;
            WeaponManager weps = PlayerObject.GetComponent<WeaponManager>();
            for (int i = 0; i < hits.Length; i++)
            {
                if (!didHit[i])
                {
                    continue;
                }

                HitboxIdentity hitbox = hits[i].collider.GetComponent<HitboxIdentity>();
                if (hitbox != null)
                {
                    GameObject parent = hits[i].collider.GetComponentInParent<NetworkIdentity>().gameObject;
                    CharacterClassManager hitCcm = parent.GetComponent<CharacterClassManager>();
                    PlayerStats stats = parent.GetComponent<PlayerStats>();

                    if (weps.GetShootPermission(hitCcm))
                    {
                        stats.HurtPlayer(
                            new PlayerStats.HitInfo(
                                PluginMain.damage,
                                PlayerObject.GetComponent<NicknameSync>().MyNick + " (" + PlayerObject.GetComponent<CharacterClassManager>().UserId + ")",
                                DamageTypes.Com15,
                                PlayerObject.GetComponent<QueryProcessor>().PlayerId
                            ),
                            parent);

                        weps.RpcPlaceDecal(true, hitCcm.Classes.SafeGet(hitCcm.CurClass).bloodType, hits[i].point + hits[i].normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hits[i].normal));
                        confirm = true;
                    }

                    continue;
                }

                BreakableWindow window = hits[i].collider.GetComponent<BreakableWindow>();
                if (window != null)
                {
                    window.ServerDamageWindow(PluginMain.damage);
                    confirm = true;
                    continue;
                }

                weps.RpcPlaceDecal(false, weps.curWeapon, hits[i].point + hits[i].normal * 0.01f, Quaternion.FromToRotation(Vector3.up, hits[i].normal));
            }

            int shots = 1;
            for (int i = 0; i < shots; i++)
            {
                weps.RpcConfirmShot(confirm, weps.curWeapon);
            }

        }
    }
}