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

    public GameObject defaultSight;
    public GameObject thermalSight;
    public GameObject nightStalkertSight;

    public GameObject equipButton;
    private Button equipbuttonn;
    public TMP_Text equipButtonText;

    private int currentEquippedSight = 0; // �u anda donat�lm�� sight (0: default, 1: thermal, 2: nightStalker)
    private int selectedSightIndex = -1; // �u anda se�ili sight (-1: hi�biri se�ili de�il)

    private Button selectedButtonAttachment = null;
    private Button selectedButtonSight = null;

    private void Start()
    {
        equipbuttonn = equipButton.GetComponent<Button>();
        LoadSelectedSight(); // Kaydedilen sight'� y�kle
        UpdateEquipButton(); // Buton durumunu g�ncelle
    }

    public void OnClickEquip()
    {
        //if (selectedSightIndex == -1) return;

        currentEquippedSight = selectedSightIndex;
       

        UpdateSightOnWeapon(); // Silah� modifiye et
        equipButtonText.text = "Equ�pped";
        equipbuttonn.interactable = false;
        PlayerPrefs.SetInt("SelectedSightIndex", currentEquippedSight);
        PlayerPrefs.Save();
        Debug.Log("Donat�lan Sight: " + currentEquippedSight);
    }
    private void UpdateSightOnWeapon()
    {
        defaultSight.SetActive(false);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(false);

        if (currentEquippedSight == 0)
        {
            defaultSight.SetActive(true);
        }
        else if (currentEquippedSight == 1)
        {
            thermalSight.SetActive(true);
        }
        else if (currentEquippedSight == 2)
        {
            nightStalkertSight.SetActive(true);
        }
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

    public void OnClickAttachmentsButton()
    {
        attachmentsButton.SetActive(false);
        attachments.anchoredPosition = new Vector2(0, 140);

        for (int i = 0; i < attachmentsNametext.Length; i++)
        {
            attachmentsNametext[i].gameObject.SetActive(true);
        }
    }

    public void OnClickGrid_1()
    {
        OnClickAttachmentsButton();
        sightScrollView.SetActive(true);
    }

    public void OnClickGrid_2()
    {
        OnClickAttachmentsButton();
    }

    public void OnClickSight_1(Button button)
    {
        defaultSight.SetActive(true);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(false);
        selectedSightIndex = 0;
        UpdateEquipButton();
    }

    public void OnClickSight_2(Button button)
    {
        defaultSight.SetActive(false);
        thermalSight.SetActive(true);
        nightStalkertSight.SetActive(false);
        selectedSightIndex = 1;
        UpdateEquipButton();
    }

    public void OnClickSight_3(Button button)
    {
        defaultSight.SetActive(false);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(true);
        selectedSightIndex = 2;
        UpdateEquipButton();
    }

    private void UpdateEquipButton()
    {
        equipButton.SetActive(true);
        if (selectedSightIndex == currentEquippedSight)
        {
            equipButtonText.text = "Equ�pped";
            equipbuttonn.interactable = false;
        }
        else
        {
            equipButtonText.text = "Equ�p";
            equipbuttonn.interactable = true;
        }
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

        Debug.Log("Y�klenen Sight: " + currentEquippedSight);
    }

    
}