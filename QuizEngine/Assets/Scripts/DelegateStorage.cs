using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelegateStorage : MonoBehaviour
{
    [SerializeField] SpriteRenderer fileStatus;
	[SerializeField] Sprite goodStatus;
	[SerializeField] Sprite badStatus;
	public void CheckInputFile(string path)
    {
        if (File.Exists(path) && path.EndsWith(".json")) fileStatus.sprite = goodStatus;
        else if (path != "") fileStatus.sprite = badStatus;
        else fileStatus.sprite = null;
    }

    public void ReadFile(string path)
    {
		if (File.Exists(path) && path.EndsWith(".json"))
        {
            GameManager.Instance.GameData = new(path);
            SceneManager.LoadScene("UserSetup");
        }
    }
}
