using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeOutSpeed = 1f;

    private void Update()
    {
        MoveUp();
        FadeOut();
    }

    private void MoveUp() 
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    private void FadeOut()
    {
        Color color = textMesh.color;
        color.a -= fadeOutSpeed * Time.deltaTime;
        textMesh.color = color;

        if (color.a <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetColor(Color color)
    {
        textMesh.color = color;
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetFadeOutSpeed(float speed)
    {
        fadeOutSpeed = speed;
    }
}