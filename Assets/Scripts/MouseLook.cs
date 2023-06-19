using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 3.5f;
    public float zoomSpeed = 3.5f;

    private float Z;
    private float Y;

    public float minHeight;
    public LayerMask heightCheckMask;



    void Update() {
        CalculatePositionAndRotation();

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 99f);
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 99f)) {
            if(hit.collider != null) {
                minHeight = hit.point.y + 2f;
            }
        }
    }

    void CalculatePositionAndRotation() {
        float x, y;

        x = -Input.GetAxis("Vertical");
        y = Input.GetAxis("Horizontal");

        Vector3 front = transform.forward;
        front.y = 0;
        Vector3 right = transform.right;
        right.y = 0;

        Vector3 changeF = new Vector3(0, 0, 0);
        Vector3 changeR = new Vector3(0, 0, 0);

        if (x != 0) {
            changeR = right * x * speed * Time.deltaTime;
        }
        if (y != 0) {
            changeF += front * y * speed * Time.deltaTime;
        }

        Vector3 zoom = Vector3.down * zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        float height = transform.position.y;
        if(height + zoom.y < minHeight) {
            zoom = Vector3.zero;
        }
        if(transform.position.y <= minHeight) {
            transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
        }

        Vector3 change = changeR + changeF + zoom;


        transform.position = Vector3.Lerp(transform.position, transform.position + change, .18f);





        if (Input.GetMouseButton(0)) {
            transform.Rotate(new Vector3(0, -Input.GetAxis("Mouse X") * -rotateSpeed, Input.GetAxis("Mouse Y") * -rotateSpeed));
            Z = transform.rotation.eulerAngles.z;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, Y, Z);
        }
    }
}
