using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    GameObject ship;
    Vector3 currentVelocity;

    [SerializeField] float MAX_SPEED = 5.0f;
    [SerializeField] float SMOOTH_TIME = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ship = GameObject.Find("Ship");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            new Vector3(ship.transform.position.x, ship.transform.position.y, -1), 
            ref currentVelocity,
            SMOOTH_TIME,
            MAX_SPEED);
    }
}
