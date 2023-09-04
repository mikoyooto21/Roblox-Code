using System.Collections;
using UnityEngine;

public class StairDisappearing : MonoBehaviour
{
    private bool isColliding = false;
    private GameObject collidedStair = null;

    private void Update()
    {
        if (isColliding && collidedStair != null)
        {
            StartCoroutine(DisappearAndAppear(collidedStair));
            isColliding = false;
            collidedStair = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            isColliding = true;
            collidedStair = collision.gameObject;
        }
    }

    private IEnumerator DisappearAndAppear(GameObject stair)
    {
        MeshRenderer meshRenderer = stair.GetComponent<MeshRenderer>();
        Collider collider = stair.GetComponent<Collider>();

        if (meshRenderer && collider)
        {
            yield return new WaitForSeconds(0.8f); // �������� �� 2 �������

            meshRenderer.enabled = false;
            collider.enabled = false;

            yield return new WaitForSeconds(5f); // �'������� ����� 5 ������

            meshRenderer.enabled = true;
            collider.enabled = true;
        }
    }
}
