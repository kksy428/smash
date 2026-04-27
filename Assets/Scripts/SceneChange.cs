using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // ∏∆∂Û∑ª æ¿¿∏∑Œ ¿Ãµø
    public void GoToMcLaren()
    {
        SceneManager.LoadScene("MAIN_INT_MCLAREN");
    }

    // ∫Œ∞°∆º æ¿¿∏∑Œ ¿Ãµø
    public void GoToBugatti()
    {
        SceneManager.LoadScene("MAIN_INT_BUGATII");
    }

    public void GoToFERRARI()
    {
        SceneManager.LoadScene("MAIN_INT_FERRARI");
    }

    public void GoToMainExt()
    {
        SceneManager.LoadScene("MAIN_EXT");
    }
}