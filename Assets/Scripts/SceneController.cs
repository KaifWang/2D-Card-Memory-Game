using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SceneController : MonoBehaviour {
    public TMP_Dropdown dropdown;
    private List<string> names = new List<string> { "2 x 3", "2 x 4", "2 x 5", "3 x 4", "4 x 4", "4 x 5" };
    private MemoryCard[] cardArray = new MemoryCard[20];
    private int numCard = 0;
    [SerializeField] private MemoryCard originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMesh scoreLabel;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject end;
    [SerializeField] private GameObject smoke;


    private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;
	private int _score = 0;
    private bool shake = false;
    private Vector3 firstP;
    private Vector3 secondP;

	public bool canReveal {
		get {return _secondRevealed == null;}
	}

	// Use this for initialization
	void Start() {
        dropdown.AddOptions(names);
        ChangeGridSize(0);
	}

    void Update()
    {
        if (shake)
        {
            _firstRevealed.transform.localPosition = firstP + Random.insideUnitSphere * 0.2f;
            _secondRevealed.transform.localPosition = secondP + Random.insideUnitSphere * 0.2f;
        }
    }

	// Knuth shuffle algorithm
	private int[] ShuffleArray(int[] numbers) {
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++ ) {
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(MemoryCard card) {
		if (_firstRevealed == null) {
			_firstRevealed = card;
		} else {
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}

    public void ChangeGridSize(int index)
    {
        _score = 0;
        scoreLabel.text = "Score: " + _score;
        start.SetActive(false);
        end.SetActive(false);
        originalCard.gameObject.SetActive(true);
        for (int i = 0; i < 20; i++)
        {
            if (cardArray[i] != null)
                if (i == 0)
                {
                    cardArray[0].Unreveal();
                }
                else
                {
                    Destroy(cardArray[i].gameObject);
                }
;        }
        int gridRows = 0;
        int gridCols = 0;
        if(index == 0)
        {
            gridRows = 2;
            gridCols = 3;
        }
        if (index == 1)
        {
            gridRows = 2;
            gridCols = 4;
        }
        if (index == 2)
        {
            gridRows = 2;
            gridCols = 5;
        }
        if (index == 3)
        {
            gridRows = 3;
            gridCols = 4;
        }
        if (index == 4)
        {
            gridRows = 4;
            gridCols = 4;
        }
        if (index == 5)
        {
            gridRows = 4;
            gridCols = 5;
        }
        float sizeFactorX = 4f / gridCols;
        float sizeFactorY = 2f / gridRows;
        float offsetX = 2f * sizeFactorX;
        float offsetY = 2.5f * sizeFactorY;

        originalCard.transform.localScale = new Vector3(sizeFactorX, sizeFactorY, 1f);

        int[] numbers = new int[gridRows * gridCols];
        numCard = gridRows * gridCols;
        int k = 0;
        while(k < numbers.Length - 1)
        {
            int rand = Random.Range(0, 51);
            bool isExist = false;
            for(int j = 0; j < k; j++)
            {
                if(numbers[j] == rand)
                {
                    isExist = true;
                }
            }
            if (!isExist)
            {
                numbers[k] = rand;
                numbers[k + 1] = rand;
                k = k + 2;
            }
        }
        numbers = ShuffleArray(numbers);

        Vector3 startPos = originalCard.transform.position;

        // create shuffled list of cards


        // place cards in a grid
        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                MemoryCard card;

                // use the original for the first grid space
                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard) as MemoryCard;
                }

                // next card in the list for each grid space
                int index2 = j * gridCols + i;
                cardArray[index2] = card;
                int id = numbers[index2];
                card.SetCard(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY = -(offsetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private IEnumerator CheckMatch() {

		// increment score if the cards match
		if (_firstRevealed.id == _secondRevealed.id) {
            shake = true;
            firstP = _firstRevealed.transform.localPosition;
            secondP = _secondRevealed.transform.localPosition;
            yield return new WaitForSeconds(0.7f);
            shake = false;
            GameObject smoke1 = Instantiate(smoke);
            GameObject smoke2 = Instantiate(smoke);
            _firstRevealed.gameObject.SetActive(false);
            _secondRevealed.gameObject.SetActive(false);
            smoke1.transform.position = firstP;
            smoke2.transform.position = secondP;
            _score++;
			scoreLabel.text = "Score: " + _score;
            yield return new WaitForSeconds(1f);
            Destroy(smoke1);
            Destroy(smoke2);
            numCard -= 2;
            if(numCard == 0)
            {
                GameObject _win = Instantiate(win);
                yield return new WaitForSeconds(1.5f);
                Destroy(_win.gameObject);
                start.SetActive(true);
                end.SetActive(true);
            }
		}

		// otherwise turn them back over after .5s pause
		else {
			yield return new WaitForSeconds(.5f);

			_firstRevealed.Unreveal();
			_secondRevealed.Unreveal();
		}
		
		_firstRevealed = null;
		_secondRevealed = null;
	}

	public void Restart() {
		Application.LoadLevel("Scene");
	}
    public void Quit()
    {
        Application.Quit();
    }
}
