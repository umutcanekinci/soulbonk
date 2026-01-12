using UnityEngine;

public class SpriteShatter : MonoBehaviour
{
    [Header("Parçalanma Ayarları")]
    [SerializeField] private int piecesX = 5; 
    [SerializeField] private int piecesY = 5;
    [SerializeField] private float explosionForce = 150f;
    [SerializeField] private float destroyAfter = 4f; 
    
    [Header("Top-Down Fizik")]
    [SerializeField] private float groundFriction = 3f;

    public event System.Action OnDestroy;

    public void ShatterAndDie()
    {
        SpriteRenderer originalSR = GetComponent<SpriteRenderer>();
        
        if (originalSR == null || originalSR.sprite == null) 
        {
            Destroy(gameObject);
            return;
        }

        // ÖNEMLİ DEĞİŞİKLİK:
        // Texture'ın tamamını değil, sadece Sprite'ın olduğu dikdörtgen alanı alıyoruz.
        Texture2D tex = originalSR.sprite.texture;
        Rect spriteRect = originalSR.sprite.rect; 

        // Parça boyutlarını sprite'ın kendi genişliğine göre hesapla
        float pieceWidth = spriteRect.width / piecesX;
        float pieceHeight = spriteRect.height / piecesY;
        
        Color spriteColor = originalSR.color;

        for (int x = 0; x < piecesX; x++)
        {
            for (int y = 0; y < piecesY; y++)
            {
                // Koordinatları hesaplarken Sprite'ın başlangıç noktasını (spriteRect.x/y) ekliyoruz
                // Böylece büyük resmin doğru yerinden parça alıyoruz.
                float globalX = spriteRect.x + (x * pieceWidth);
                float globalY = spriteRect.y + (y * pieceHeight);

                // Şeffaflık Kontrolü (Düzeltilmiş Koordinat ile)
                Color pixelColor = tex.GetPixel((int)(globalX + pieceWidth/2), (int)(globalY + pieceHeight/2));
                
                // Eğer parça çok şeffafsa oluşturma
                if (pixelColor.a <= 0.1f) continue; 

                // --- Parça Oluşturma ---
                Rect newRect = new Rect(globalX, globalY, pieceWidth, pieceHeight);
                
                GameObject piece = new GameObject("Bone_Shard");
                
                // Parçayı doğru konuma yerleştirme hesabı
                // (Merkezden sapmayı hesaplarken yerel parça indeksini kullanıyoruz)
                float localX = (x * pieceWidth) - (spriteRect.width / 2) + (pieceWidth / 2);
                float localY = (y * pieceHeight) - (spriteRect.height / 2) + (pieceHeight / 2);
                
                Sprite newSprite = Sprite.Create(tex, newRect, new Vector2(0.5f, 0.5f), originalSR.sprite.pixelsPerUnit);
                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite = newSprite;
                sr.color = spriteColor;
                sr.sortingOrder = originalSR.sortingOrder; // Üstte görünsün

                // Transform
                piece.transform.position = transform.position + (transform.rotation * new Vector3(localX / originalSR.sprite.pixelsPerUnit, localY / originalSR.sprite.pixelsPerUnit, 0));
                
                // Rastgelelik (Mozaik görüntüyü kırmak için)
                piece.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); 
                float randomScale = Random.Range(0.5f, 1.0f); // Parçalar biraz küçülsün ki aralar açılsın
                piece.transform.localScale = transform.localScale * randomScale;

                // Fizik
                Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; 
                rb.linearDamping = groundFriction; 
                rb.angularDamping = 2f; 

                // Patlama
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDir * explosionForce * Random.Range(0.8f, 1.5f));
                rb.AddTorque(Random.Range(-200f, 200f));

                Destroy(piece, destroyAfter);
            }
        }

        OnDestroy?.Invoke();
    }
}