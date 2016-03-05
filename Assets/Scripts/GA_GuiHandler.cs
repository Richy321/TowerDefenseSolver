using UnityEngine;
using UnityEngine.UI;

public class GA_GuiHandler : MonoBehaviour
{
    public SceneController sceneController;

    public Text generationValueLabel;
    public Text waveNoValueLabel;
    public Text spawnNoValueLabel;
    public Text highestFitnessValueLabel;
    public Text solutionsValueLabel;
    public Text remainingEnemiesValueLabel;
    public Text sceneStateValueLabel;
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

        if (!highestFitnessValueLabel)
            highestFitnessValueLabel = FindTextComponent("HighestFitnessValue");

        if (!solutionsValueLabel)
            solutionsValueLabel = FindTextComponent("SolutionsValue");

        if (!remainingEnemiesValueLabel)
            remainingEnemiesValueLabel = FindTextComponent("RemainingEnemiesValue");

        if (!sceneStateValueLabel)
            sceneStateValueLabel = FindTextComponent("SceneStateValue");

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
	    highestFitnessValueLabel.text = sceneController.ga.highestFitness.ToString();
	    solutionsValueLabel.text = sceneController.solutions.Count.ToString();
	    remainingEnemiesValueLabel.text = sceneController.RemainingEnemies.ToString();
	    sceneStateValueLabel.text = sceneController.state.ToString();
	}

    Text FindTextComponent(string goName)
    {
        GameObject go = GameObject.Find(goName);
        return go ? go.GetComponent<Text>() : null;
    }
}
