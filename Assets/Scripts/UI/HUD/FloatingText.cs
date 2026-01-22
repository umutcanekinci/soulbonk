using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh;
    // [SerializeField] private float moveSpeed = 0.3f;
    [SerializeField] private float fadeOutSpeed = 1f;
    [SerializeField] private float scaleDownSpeed = 0.5f;

    [Header("Physics Settings")]
    [SerializeField] private Vector2 initialUpwardSpeed = new Vector2(5f, 8f);
    [SerializeField] private float horizontalSpread = 2f;
    [SerializeField] private float gravity = 20f;

    private Vector3 currentVelocity;

    public void Initialize(string text, Color color, float scale)
    {
        textMesh.text = text;
        textMesh.color = color;
        transform.localScale = Vector3.one * scale;
        
        float xVelocity = Random.Range(-horizontalSpread, horizontalSpread);
        float yVelocity = Random.Range(initialUpwardSpeed.x, initialUpwardSpeed.y);
        
        currentVelocity = new Vector3(xVelocity, yVelocity, 0f);
        
        gameObject.SetActive(true);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void Update()
    {
        // MoveUp();
        // FadeOut();

        currentVelocity.y -= gravity * Time.deltaTime;
        transform.position += currentVelocity * Time.deltaTime;
        transform.localScale -= Vector3.one * scaleDownSpeed * Time.deltaTime;

        Color color = textMesh.color;
        color.a -= fadeOutSpeed * Time.deltaTime;
        textMesh.color = color;

        if (color.a <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    // private void MoveUp() 
    // {
    //     transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    // }

    // private void FadeOut()
    // {
    //     Color color = textMesh.color;
    //     color.a -= fadeOutSpeed * Time.deltaTime;
    //     textMesh.color = color;

    //     if (color.a <= 0)
    //     {
    //         gameObject.SetActive(false);
    //     }
    // }

}