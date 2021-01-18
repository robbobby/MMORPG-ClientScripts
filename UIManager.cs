using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager SingletonInstance;
    public GameObject startMenu;
    public InputField userNameField;
    
    private void Awake() {
        if (SingletonInstance == null) {
            SingletonInstance = this;
        } else if(SingletonInstance != this) {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    public void ConnectToServer() {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        Client.singletonInstance.ConnectToServer();
    }
}
