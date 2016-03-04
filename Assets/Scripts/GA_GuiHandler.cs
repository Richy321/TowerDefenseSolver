using UnityEngine;
using UnityEngine.UI;

public class GA_GuiHandler : MonoBehaviour
{
    public SceneController sceneController;

    public Text generationValueLabel;
    public Text waveNoValueLabel;
    public Text spawnNoValueLabel;
    public Toggle autoEvolveToggle;

    // Use this for initialization
    void Awake ()
    {
        if (!sceneController)
            sceneController = FindObjectOfType<SceneController>();

        if (!generationValueLabel)
            generationValueLabel = FindTextComponent("GenerationValue");

        if (!waveNoValueLabel)
            waveNoValueLabel = FindTextComponent("WaveNoValue");

        if (!spawnNoValueLabel)
            spawnNoValueLabel = FindTextComponent("SpawnNoValue");

        if (!autoEvolveToggle)
            autoEvolveToggle = GameObject.Find("AutoEvolveCheck").GetComponent<Toggle>();
        autoEvolveToggle.isOn = sceneController.AutoEvolve;
    }

	// Update is called once per frame
	void Update ()
	{
	    generationValueLabel.text = sceneController.ga.generationNo.ToString();
	    waveNoValueLabel.text = sceneController.waveManager.waveIndex.ToString();
	    spawnNoValueLabel.text = sceneController.waveManager.waveSpawnIndex.ToString();
	}

    Text FindTextComponent(string goName)
    {
        GameObject go = GameObject.Find(goName);
        return go ? go.GetComponent<Text>() : null;
    }
}
