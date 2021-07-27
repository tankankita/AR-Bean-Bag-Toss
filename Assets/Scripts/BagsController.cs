using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagsController : Singleton<BagsController>
{

    [SerializeField] private GameObject beanBagPrefab;
    [SerializeField] private float forwardVelocity = 4f;
    private Transform _throwLocation;

    private List<GameObject> bags;

    // Start is called before the first frame update
    void Start()
    {
        _throwLocation = Camera.main.transform;
        bags = new List<GameObject>();
    }

   public void ThrowBag()
    {
        ScreenLog.Log("Throw Bag");
        var bagObject = Instantiate(beanBagPrefab, _throwLocation.position,
            _throwLocation.rotation);

        bags.Add(bagObject);
        bagObject.GetComponent<Rigidbody>().AddForce(bagObject.transform.forward
           * forwardVelocity, ForceMode.Impulse);

    }

    public void UpdateThrowForce (float value)
    {
        forwardVelocity = value;
    }

    public void ResetBags()
    {
        foreach (GameObject bag in bags)
        {
            Destroy(bag);
        }
        bags.Clear();
    }
}
