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
    public GameObject magScroolView;
    public GameObject tacticalScrollView;

    //sight gameobjeleri
    public GameObject defaultSight;
    public GameObject thermalSight;
    public GameObject nightStalkertSight;

    //mag gameobjeleri
    public GameObject defaultMag;
    public GameObject purpleMag;
    public GameObject greenMag;
    public GameObject redMag;
    public GameObject bigMag;

    //barrel gameobjeleri
    public GameObject defaultBarrel;
    public GameObject coolBreeze;
    public GameObject peaceKeeper;

    //Tactical gameobjeleri
    public GameObject defaultTactical;
    public GameObject fireTactical;
    public GameObject lightTactical;

    public Button[] sightButtons; 
    public Button[] barrelButtons;
    public Button[] magButtons;
    public Button[] tacticalButtons;

    public GameObject equipButton;
    private Button equipbuttonn;
    public TMP_Text equipButtonText;

    public RectTransform statsScrollViewContent;

    // Sight için deðiþkenler
    private int currentEquippedSight = 0; // Þu anda donatýlmýþ sight (0: default, 1: thermal, 2: nightStalker)
    private int selectedSightIndex = -1;  

    // Barrel için deðiþkenler
    private int currentEquippedBarrel = 0; // Þu anda donatýlmýþ barrel (0: default, 1: coolBreeze, 2: peaceKeeper)
    private int selectedBarrelIndex = -1;  

    //mag için deðiþkenler
    private int currentEquippedMag = 0;
    private int selectedMagIndex = -1;

    //tactical için deðiþkenler
    private int currentEqippedTactical = 0;
    private int selectedTacticalIndex= -1;

    private Button selectedButtonAttachment = null;
    private Button selectedButtonSight = null;
    private Button selectedButtonMag = null;
    private Button selectedButtonBarrel=null;
    private Button selectedButtonTactical = null;

    void Start()
    {
        equipbuttonn = equipButton.GetComponent<Button>();
        LoadSelectedAttachments();

        UpdateInteractibleAttachmentsButton();

        DefaultPlayerPrefsData();
        UpdateEquipButton();
    }

    public void OnClickEquip()
    {
        // Sight
        if (selectedSightIndex != -1)
        {
            if (currentEquippedSight != selectedSightIndex)
            {
                sightButtons[currentEquippedSight].interactable = true; // Eski sight butonunu aktif yap
                currentEquippedSight = selectedSightIndex;
                sightButtons[currentEquippedSight].interactable = false; // Yeni sight butonunu pasif yap
            }
            UpdateSightOnWeapon();
            PlayerPrefs.SetInt("SelectedSightIndex", currentEquippedSight);
        }

        // Barrel
        if (selectedBarrelIndex != -1)
        {
            if (currentEquippedBarrel != selectedBarrelIndex)
            {
                barrelButtons[currentEquippedBarrel].interactable = true; // Eski barrel butonunu aktif yap
                currentEquippedBarrel = selectedBarrelIndex;
                barrelButtons[currentEquippedBarrel].interactable = false; // Yeni barrel butonunu pasif yap
            }
            UpdateBarrelOnWeapon();
            PlayerPrefs.SetInt("SelectedBarrelIndex", currentEquippedBarrel);
        }

        // Mag
        if (selectedMagIndex != -1)
        {
            if (currentEquippedMag != selectedMagIndex)
            {
                magButtons[currentEquippedMag].interactable = true; // Eski mag butonunu aktif yap
                currentEquippedMag = selectedMagIndex;
                magButtons[currentEquippedMag].interactable = false; // Yeni mag butonunu pasif yap
            }
            UpdateMagOnWeapon();
            PlayerPrefs.SetInt("SelectedMagIndex", currentEquippedMag);
        }
        //Tactical
        if (selectedTacticalIndex != -1)
        {
            if (currentEqippedTactical != selectedTacticalIndex)
            {
                tacticalButtons[currentEqippedTactical].interactable = true; // Eski tactical butonunu aktif yap
                currentEqippedTactical = selectedTacticalIndex;
                tacticalButtons[currentEqippedTactical].interactable = false; // Yeni Tactical butonunu pasif yap
            }
            UpdateTacticalOnWeapon();
            PlayerPrefs.SetInt("SelectedTacticalIndex", currentEqippedTactical);
        }

        PlayerPrefs.Save();
        equipButtonText.text = "Equýpped";
        equipbuttonn.interactable = false;
    }
    private void UpdateEquipButton()
    {
        if (selectedSightIndex == currentEquippedSight
            && selectedBarrelIndex == currentEquippedBarrel
            && selectedMagIndex == currentEquippedMag&& selectedTacticalIndex== currentEqippedTactical) 
        {
            equipButtonText.text = "Equýpped";
            equipbuttonn.interactable = false;
        }
        else
        {
            equipButtonText.text = "Equýp";
            equipbuttonn.interactable = true;
        }
    }
    private void DefaultPlayerPrefsData() 
    {
        selectedSightIndex = currentEquippedSight;
        selectedBarrelIndex = currentEquippedBarrel;
        selectedMagIndex = currentEquippedMag;
        selectedTacticalIndex = currentEqippedTactical;
        UpdateBarrelOnWeapon();
        UpdateSightOnWeapon();
        UpdateMagOnWeapon();
        UpdateTacticalOnWeapon();
    }
    private void UpdateInteractibleAttachmentsButton()
    {
        selectedButtonSight = sightButtons[currentEquippedSight];
        selectedButtonSight.interactable = false;
        selectedButtonBarrel = barrelButtons[currentEquippedBarrel];
        selectedButtonBarrel.interactable = false;
        selectedButtonMag=magButtons[currentEquippedMag];
        selectedButtonMag.interactable = false;
        selectedButtonTactical = tacticalButtons[currentEqippedTactical];
        selectedButtonTactical.interactable = false;
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
    private void UpdateMagOnWeapon()
    {
        defaultMag.SetActive(false);
        greenMag.SetActive(false);
        redMag.SetActive(false);
        purpleMag.SetActive(false);
        bigMag.SetActive(false);

        if (currentEquippedMag == 0) defaultMag.SetActive(true);
        else if (currentEquippedMag == 1) purpleMag.SetActive(true);
        else if (currentEquippedMag == 2) greenMag.SetActive(true);
        else if (currentEquippedMag == 3) redMag.SetActive(true);
        else if (currentEquippedMag == 4) bigMag.SetActive(true);
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
    private void UpdateTacticalOnWeapon()
    {
        defaultTactical.SetActive(false);
        lightTactical.SetActive(false);
        fireTactical.SetActive(false);

        if (currentEqippedTactical == 0) defaultTactical.SetActive(true);
        else if (currentEqippedTactical == 1) fireTactical.SetActive(true);
        else if (currentEqippedTactical == 2) lightTactical.SetActive(true);
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
    public void SelectButtonMag(Button button) 
    {
        if (selectedButtonMag != null)
        {
            selectedButtonMag.interactable = true;
        }
        selectedButtonMag = button;
        selectedButtonMag.interactable = false;
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
    public void SelectButtonTactical(Button button)
    {
        if (selectedButtonTactical != null)
        {
            selectedButtonTactical.interactable = true;
        }
        selectedButtonTactical = button;
        selectedButtonTactical.interactable = false;
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
        magScroolView.SetActive(false);
        tacticalScrollView.SetActive(false);
        /*if (selectedBarrelIndex != currentEquippedBarrel)
        {
            selectedBarrelIndex = currentEquippedBarrel;
            Debug.Log(selectedBarrelIndex);
            UpdateBarrelOnWeapon();
            UpdateMagOnWeapon();
        }*/
        for (int i = 0; i < magButtons.Length; i++)
        {
            magButtons[i].interactable = true;
        }
        for (int i = 0; i < barrelButtons.Length; i++)
        {
            barrelButtons[i].interactable = true;
        }
        for (int i = 0; i < tacticalButtons.Length; i++)
        {
            tacticalButtons[i].interactable = true;
        }
        DefaultPlayerPrefsData();
        UpdateInteractibleAttachmentsButton();
        UpdateEquipButton();
    }
    public void OnClickGrid_2()//mag
    {
        OnClickAttachmentsButton();
        sightScrollView.SetActive(false);
        barrelScroolView.SetActive(false);
        magScroolView.SetActive(true);
        tacticalScrollView.SetActive(false);
        /*if (selectedMagIndex != currentEquippedMag)
        {
            selectedMagIndex = currentEquippedMag;
            Debug.Log(selectedMagIndex);
            UpdateSightOnWeapon();
            UpdateBarrelOnWeapon();
        }*/
        for (int i = 0; i < sightButtons.Length; i++)
        {
            sightButtons[i].interactable = true;
        }
        for (int i = 0; i < barrelButtons.Length; i++)
        {
            barrelButtons[i].interactable = true;
        }
        for (int i = 0; i < tacticalButtons.Length; i++)
        {
            tacticalButtons[i].interactable = true;
        }
        DefaultPlayerPrefsData();
        UpdateInteractibleAttachmentsButton();
        UpdateEquipButton();
    }

    public void OnClickGrid_3()//barrel
    {
        OnClickAttachmentsButton();
        sightScrollView.SetActive(false);
        barrelScroolView.SetActive(true);
        magScroolView.SetActive(false);
        tacticalScrollView.SetActive(false);
        /*if (selectedSightIndex != currentEquippedSight)
        {
            selectedSightIndex = currentEquippedSight;
            Debug.Log(selectedSightIndex);
            UpdateSightOnWeapon();
            UpdateMagOnWeapon();
        }*/
        for (int i = 0; i < sightButtons.Length; i++)
        {
            sightButtons[i].interactable = true;
        }
        for (int i = 0; i < magButtons.Length; i++)
        {
            magButtons[i].interactable = true;
        }
        for (int i = 0; i < tacticalButtons.Length; i++)
        {
            tacticalButtons[i].interactable = true;
        }
        DefaultPlayerPrefsData();
        UpdateInteractibleAttachmentsButton();
        UpdateEquipButton();
    }
    public void OnClickGrid_5()//Tactical
    {
        OnClickAttachmentsButton();
        sightScrollView.SetActive(false);
        barrelScroolView.SetActive(false);
        magScroolView.SetActive(false);
        tacticalScrollView.SetActive(true);
        
        for (int i = 0; i < sightButtons.Length; i++)
        {
            sightButtons[i].interactable = true;
        }
        for (int i = 0; i < magButtons.Length; i++)
        {
            magButtons[i].interactable = true;
        }
        for (int i = 0; i < barrelButtons.Length; i++)
        {
            barrelButtons[i].interactable = true;
        }
        DefaultPlayerPrefsData();
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
    public void OnClickMag_1(Button button)
    {
        selectedMagIndex = 0;
        defaultMag.SetActive(true);
        purpleMag.SetActive(false);
        greenMag.SetActive(false);
        redMag.SetActive(false);
        bigMag.SetActive(false);
        SelectButtonMag(button);
        UpdateEquipButton();
    }
    public void OnClickMag_2(Button button)
    {
        selectedMagIndex = 1;
        defaultMag.SetActive(false);
        purpleMag.SetActive(true);
        greenMag.SetActive(false);
        redMag.SetActive(false);
        bigMag.SetActive(false);
        SelectButtonMag(button);
        UpdateEquipButton();
    }
    public void OnClickMag_3(Button button)
    {
        selectedMagIndex = 2;
        defaultMag.SetActive(false);
        purpleMag.SetActive(false);
        greenMag.SetActive(true);
        redMag.SetActive(false);
        bigMag.SetActive(false);
        SelectButtonMag(button);
        UpdateEquipButton();
    }
    public void OnClickMag_4(Button button)
    {
        selectedMagIndex = 3;
        defaultMag.SetActive(false);
        purpleMag.SetActive(false);
        greenMag.SetActive(false);
        redMag.SetActive(true);
        bigMag.SetActive(false);
        SelectButtonMag(button);
        UpdateEquipButton();
    }

    public void OnClickMag_5(Button button)
    {
        selectedMagIndex = 4;
        defaultMag.SetActive(false);
        purpleMag.SetActive(false);
        greenMag.SetActive(false);
        redMag.SetActive(false);
        bigMag.SetActive(true);
        SelectButtonMag(button);
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
    public void OnClickTactical_1(Button button)
    {
        selectedTacticalIndex = 0;
        defaultTactical.SetActive(true);
        fireTactical.SetActive(false);
        lightTactical.SetActive(false);
        SelectButtonTactical(button);
        UpdateEquipButton();
    }

    public void OnClickTactical_2(Button button)
    {
        selectedTacticalIndex = 1;
        defaultTactical.SetActive(false);
        fireTactical.SetActive(true);
        lightTactical.SetActive(false);
        SelectButtonBarrel(button);
        UpdateEquipButton();
    }

    public void OnClickTactical_3(Button button)
    {
        selectedTacticalIndex = 2;
        defaultTactical.SetActive(false);
        fireTactical.SetActive(false);
        lightTactical.SetActive(true);
        SelectButtonBarrel(button);
        UpdateEquipButton();
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

        

        // Mag'i yükle
        if (PlayerPrefs.HasKey("SelectedMagIndex"))
        {
            currentEquippedMag = PlayerPrefs.GetInt("SelectedMagIndex");
        }
        else
        {
            currentEquippedMag = 0;
        }
        selectedMagIndex = currentEquippedMag;
        UpdateMagOnWeapon();


        // Tactical yükle
        if (PlayerPrefs.HasKey("SelectedTacticalIndex"))
        {
            currentEqippedTactical = PlayerPrefs.GetInt("SelectedTacticalIndex");
        }
        else
        {
            currentEqippedTactical = 0;
        }
        selectedTacticalIndex = currentEqippedTactical;
        UpdateTacticalOnWeapon();
    }
}