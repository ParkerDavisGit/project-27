using UnityEngine;

public class CurrentLevelHandler : MonoBehaviour
{
    public FloatingPlatform platformOne;
    public Transform platformOneDestination;
    public Titan titan;

    public void ActivateTitan()
    {
        platformOne.GoTo(new Vector2(platformOneDestination.position.x, platformOneDestination.position.y));
        titan.Activate();
    }

    public void DeactivateTitan()
    {

    }
}
