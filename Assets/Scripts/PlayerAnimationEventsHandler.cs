using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventsHandler : MonoBehaviour
{
    public PlayerController playerController;

    public void ActivateLightAttackHitbox1()
    {
        playerController.ActivateHitboxLightAttack1();
    }
    public void ActivateLightAttackHitbox2()
    {
        playerController.ActivateHitboxLightAttack2();
    }
    public void ActivateLightAttackHitbox3()
    {
        playerController.ActivateHitboxLightAttack3();
    }

    //deactivates
    public void DectivateLightAttackHitbox1()
    {
        playerController.DeactivateHitboxLightAttack1();
    }
    public void DectivateLightAttackHitbox2()
    {
        playerController.DeactivateHitboxLightAttack2();
    }
    public void DectivateLightAttackHitbox3()
    {
        playerController.DeactivateHitboxLightAttack3();
    }
}