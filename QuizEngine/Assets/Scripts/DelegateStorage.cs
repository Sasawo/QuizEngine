using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelegateStorage : MonoBehaviour
{
    [SerializeField] SpriteRenderer fileStatus;
	[SerializeField] SpriteRenderer divisorStatus;
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
        }
		if (GameManager.Instance.Divisor != 0)
		{
			SceneManager.LoadScene("UserSetup");
		}

	}

	public void CheckDivisor(string num)
	{
		if (num == "")
		{
			divisorStatus.sprite = null;
			return;
		}

		try
		{
			int.Parse(num);
			int test = 5 / int.Parse(num);
			divisorStatus.sprite = goodStatus;
		}
		catch
		{
			divisorStatus.sprite = badStatus;
		}
	}

	public void SetDivisor(string num)
	{
		try
		{
			GameManager.Instance.Divisor = int.Parse(num);
		}
		catch { }

		if (GameManager.Instance.GameData != null)
		{
			SceneManager.LoadScene("UserSetup");
		}
	}
}
