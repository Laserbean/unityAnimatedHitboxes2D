


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laserbean.Hitbox2D {

public interface IDamageable2
{
    void Damage(Damage damage);

}

public interface IDamageInfoable 
{
    public void DamageInfoed(DamageInfo damageInfo); 
}



}
