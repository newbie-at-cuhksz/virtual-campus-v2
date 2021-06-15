using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
    public class SceneFunction : MonoBehaviour
    {
        #region Public Methods
        public void JumpToScene_Single(string index)
        {
            SceneManager.LoadScene(index,LoadSceneMode.Single);
        }
        public void JumpToScene_Additive(string index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Additive);
        }
        public void Debug_Log(string message)
        {
            Debug.Log(message);
        }

        #endregion
    }
}
