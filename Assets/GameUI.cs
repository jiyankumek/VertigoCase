using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public RectTransform attachments;
    public GameObject attachmentsButton;
    public TMP_Text[] attachmentsNametext;

    public GameObject selectImage;
    public GameObject sightScrollView;
    public GameObject barrelScroolView;

    public GameObject defaultSight;
    public GameObject thermalSight;
    public GameObject nightStalkertSight;

    public GameObject defaultBarrel;
    public GameObject coolBreeze;
    public GameObject peaceKeeper;

    public Button[] sightButtons; 
    public Button[] barrelButtons;

    public GameObject equipButton;
    private Button equipbuttonn;
    public TMP_Text equipButtonText;

    public RectTransform statsScrollViewContent;

    // Sight için deðiþkenler
    private int currentEquippedSight = 0; // Þu anda donatýlmýþ sight (0: default, 1: thermal, 2: nightStalker)
    private int selectedSightIndex = -1;  // hiçbiri seçili deðil

    // Barrel için deðiþkenler
    private int currentEquippedBarrel = 0; // Þu anda donatýlmýþ barrel (0: default, 1: coolBreeze, 2: peaceKeeper)
    private int selectedBarrelIndex = -1;  // hiçbiri seçili deðil

    private Button selectedButtonAttachment = null;
    private Button selectedButtonSight = null;
    private Button selectedButtonBarrel=null;

    void Start()
    {
        equipbuttonn = equipButton.GetComponent<Button>();
        LoadSelectedAttachments();

        selectedButtonSight = sightButtons[currentEquippedSight];
        selectedButtonSight.interactable = false;
        selectedButtonBarrel = barrelButtons[currentEquippedBarrel];
        selectedButtonBarrel.interactable = false;

        selectedSightIndex = currentEquippedSight;
        selectedBarrelIndex = currentEquippedBarrel;
        UpdateEquipButton();
    }

    public void OnClickEquip()
    {
        // Sight
        if (selectedSightIndex != -1)
        {
            if (currentEquippedSight != selectedSightIndex)
            {
                UpdateInteractibleAttachmentsButton();
            }
            UpdateSightOnWeapon();
            PlayerPrefs.SetInt("SelectedSightIndex", currentEquippedSight);
        }

        // Barrel
        if (selectedBarrelIndex != -1)
        {
            if (currentEquippedBarrel != selectedBarrelIndex)
            {
                UpdateInteractibleAttachmentsButton();
            }
            UpdateBarrelOnWeapon();
            PlayerPrefs.SetInt("SelectedBarrelIndex", currentEquippedBarrel);
        }

        PlayerPrefs.Save();
        equipButtonText.text = "Equýpped";
        equipbuttonn.interactable = false;
    }
    private void UpdateEquipButton()
    {
        // Eðer seçilen sight ve barrel, donatýlmýþ olanlarla tamamen eþleþiyorsa
        if (selectedSightIndex == currentEquippedSight && selectedBarrelIndex == currentEquippedBarrel)
        {
            equipButtonText.text = "Equýpped";
            equipbuttonn.interactable = false; // Butonu týklanamaz yap
        }
        else
        {
            equipButtonText.text = "Equýp";
            equipbuttonn.interactable = true;  // Butonu týklanabilir yap
        }
    }
    private void UpdateInteractibleAttachmentsButton()
    {
        selectedButtonSight = sightButtons[currentEquippedSight];
        selectedButtonSight.interactable = false;
        selectedButtonBarrel = barrelButtons[currentEquippedBarrel];
        selectedButtonBarrel.interactable = false;
    }
    private void UpdateSightOnWeapon()
    {
        defaultSight.SetActive(false);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(false);

        if (currentEquippedSight == 0) defaultSight.SetActive(true);
        else if (currentEquippedSight == 1) thermalSight.SetActive(true);
        else if (currentEquippedSight == 2) nightStalkertSight.SetActive(true);
    }
    private void UpdateBarrelOnWeapon()
    {
        defaultBarrel.SetActive(false);
        coolBreeze.SetActive(false);
        peaceKeeper.SetActive(false);

        if (currentEquippedBarrel == 0) defaultBarrel.SetActive(true);
        else if (currentEquippedBarrel == 1) coolBreeze.SetActive(true);
        else if (currentEquippedBarrel == 2) peaceKeeper.SetActive(true);
    }
    
    public void SelectButtonAttachment(Button button)
    {
        if (selectedButtonAttachment != null)
        {
            selectedButtonAttachment.interactable = true;
        }
        selectedButtonAttachment = button;
        selectedButtonAttachment.interactable = false;
    }

    public void SelectButtonSight(Button button)
    {
        if (selectedButtonSight != null)
        {
            selectedButtonSight.interactable = true;
        }
        selectedButtonSight = button;
        selectedButtonSight.interactable = false;
    }
    public void SelectButtonBarrel(Button button)
    {
        if (selectedButtonBarrel != null)
        {
            selectedButtonBarrel.interactable = true;
        }
        selectedButtonBarrel = button;
        selectedButtonBarrel.interactable = false;
    }

    public void OnClickAttachmentsButton()
    {
        attachmentsButton.SetActive(false);
        attachments.anchoredPosition = new Vector2(0, 140);
        equipButton.SetActive(true);
        statsScrollViewContent.offsetMin = new Vector2(0, 0);
        for (int i = 0; i < attachmentsNametext.Length; i++)
        {
            attachmentsNametext[i].gameObject.SetActive(true);
        }
    }

    public void OnClickGrid_1()//sight
    {
        OnClickAttachmentsButton();
        sightScrollView.SetActive(true);
        barrelScroolView.SetActive(false);
        if (selectedBarrelIndex != currentEquippedBarrel)
        {
            selectedBarrelIndex = currentEquippedBarrel;
            Debug.Log(selectedBarrelIndex);
            UpdateBarrelOnWeapon();
        }
        for (int i = 0;i<barrelButtons.Length;i++)
        {
            barrelButtons[i].interactable = true;     
        }
        UpdateInteractibleAttachmentsButton();
        UpdateEquipButton();
    }

    public void OnClickGrid_3()//barrel
    {
        OnClickAttachmentsButton();
        sightScrollView.SetActive(false);
        barrelScroolView.SetActive(true);
        if (selectedSightIndex != currentEquippedSight)
        {
            selectedSightIndex = currentEquippedSight;
            Debug.Log(selectedSightIndex);
            UpdateSightOnWeapon();
        }
        for (int i = 0; i < sightButtons.Length; i++)
        {
            sightButtons[i].interactable = true;
            
        }
        UpdateInteractibleAttachmentsButton();
        UpdateEquipButton();
    }
    public void OnClickSight_1(Button button)
    {
        selectedSightIndex = 0;
        defaultSight.SetActive(true);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(false);
        SelectButtonSight(button);
        UpdateEquipButton();
    }

    public void OnClickSight_2(Button button)
    {
        selectedSightIndex = 1;
        defaultSight.SetActive(false);
        thermalSight.SetActive(true);
        nightStalkertSight.SetActive(false);
        SelectButtonSight(button);
        UpdateEquipButton();
    }

    public void OnClickSight_3(Button button)
    {
        selectedSightIndex = 2;
        defaultSight.SetActive(false);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(true);
        SelectButtonSight(button);
        UpdateEquipButton();
    }

    public void OnClickBarrel_1(Button button)
    {
        selectedBarrelIndex = 0;
        defaultBarrel.SetActive(true);
        coolBreeze.SetActive(false);
        peaceKeeper.SetActive(false);
        SelectButtonBarrel(button);
        UpdateEquipButton();
    }

    public void OnClickBarrel_2(Button button)
    {
        selectedBarrelIndex = 1;
        defaultBarrel.SetActive(false);
        coolBreeze.SetActive(true);
        peaceKeeper.SetActive(false);
        SelectButtonBarrel(button);
        UpdateEquipButton();
    }

    public void OnClickBarrel_3(Button button)
    {
        selectedBarrelIndex = 2;
        defaultBarrel.SetActive(false);
        coolBreeze.SetActive(false);
        peaceKeeper.SetActive(true);
        SelectButtonBarrel(button);
        UpdateEquipButton();
    }

    private void LoadSelectedSight()
    {
        if (PlayerPrefs.HasKey("SelectedSightIndex"))
        {
            currentEquippedSight = PlayerPrefs.GetInt("SelectedSightIndex");
        }
        else
        {
            currentEquippedSight = 0;
        }
        selectedSightIndex = currentEquippedSight;
        UpdateSightOnWeapon();

        Debug.Log("Yüklenen Sight: " + currentEquippedSight);
    }

    private void LoadSelectedAttachments()
    {
        // Sight'ý yükle
        if (PlayerPrefs.HasKey("SelectedSightIndex"))
        {
            currentEquippedSight = PlayerPrefs.GetInt("SelectedSightIndex");
        }
        else
        {
            currentEquippedSight = 0;
        }
        selectedSightIndex = currentEquippedSight;
        UpdateSightOnWeapon();

        // Barrel'ý yükle
        if (PlayerPrefs.HasKey("SelectedBarrelIndex"))
        {
            currentEquippedBarrel = PlayerPrefs.GetInt("SelectedBarrelIndex");
        }
        else
        {
            currentEquippedBarrel = 0;
        }
        selectedBarrelIndex = currentEquippedBarrel;
        UpdateBarrelOnWeapon();

        Debug.Log("Yüklenen Sight: " + currentEquippedSight);
        Debug.Log("Yüklenen Barrel: " + currentEquippedBarrel);
    }
}