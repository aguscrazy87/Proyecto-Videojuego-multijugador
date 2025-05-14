using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking; //para peticiones en la web

//Tipo de dato para convertir JSON a un objecto
public struct NamesData
{
    public string[] names; //El nombre en este campo debe coincidir con el campo en el JSON
}
public class UiManager : MonoBehaviour
{
    [Header("Menus")]
    public RectTransform PanelMainMenu;
    public RectTransform PanelClient;
    public TMP_Dropdown namesSelector;


   [Header("HUD")]
    public RectTransform PanelHUD;
    public TMP_Text labelHealth;
    public GameObject playerNameTemplate;

    //lista  de los nombres permitidos
    public List<string> namesList = new List<string>();
    public int selectedNameIndex { get{return namesSelector.value;}}
    public int selectedSombrero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PanelMainMenu.gameObject.SetActive(true);
        PanelClient.gameObject.SetActive(false);
        PanelHUD.gameObject.SetActive(false);
        GetNames();
    }

    //Obtener la lista de nombres permitdos y ponerla en Dropdown
    public void GetNames()
    {
        namesSelector.ClearOptions();
        StartCoroutine(GetNamesFromServer());

    }

    //Las peticiones son asincronas por que debemos usar corroutinas
    IEnumerator GetNamesFromServer()
    {
        //URL del endpoint
        string url = "http://monsterballgo.com/api/names";
        UnityWebRequest www = UnityWebRequest.Get(url); //petición Get
        yield return www.SendWebRequest(); //esepra qie se complete la petición

        //retrona 200 si va bien
        if (www.result == UnityWebRequest.Result.Success) 
        {
            //convertir el cuerpo de la respuesta a un string JSON
            string json = www.downloadHandler.text;
            NamesData namesData = JsonUtility.FromJson<NamesData>(json);
            namesList.AddRange(namesData.names);
            //Poner la lista de nombres en el Dropdown
            namesSelector.AddOptions(namesList);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ONButtonStartHost()
    {
        //crear partida
        NetworkManager.Singleton.StartHost();

        PanelMainMenu.gameObject.SetActive(false);
        PanelHUD.gameObject.SetActive(true);
    }

    public void OnButtonClientConnect()
    {
        GameObject go = GameObject.Find("inputIP");

        string ip = go.GetComponent<TMP_InputField>().text;
        Debug.Log("Se conceto a " +  ip);

        PanelMainMenu.gameObject.SetActive(false);
        PanelClient.gameObject.SetActive(false);
        PanelHUD.gameObject.SetActive(true);

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;
        NetworkManager.Singleton.StartClient();
    } 

    public void OnButtonHat(int idx)
    {
        //cambiar el sombrero del jugador
        selectedSombrero = idx;
        Debug.Log("sombrero seleccionado: " + idx);
    }
}
