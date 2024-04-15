using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public GameObject card;

    private void Start()
    {
        int[] arr = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };        

        arr = arr.OrderBy(x => Random.Range(0f, 7f)).ToArray();
     
        GameManager.instance.cardCount = arr.Length;

        for (var i = 0; i < 16; i++)
        {   
            var go = Instantiate(card, transform);
            var x = i % 4 * 1.4f - 2.1f;
            var y = i / 4 * 1.4f - 3.0f;

            go.transform.position = new Vector2(x, y);            
            go.GetComponent<Card>().Setting(arr[i]);            
        }
    }
}
