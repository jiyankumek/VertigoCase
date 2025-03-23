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
    public TMP_Text selectedSightButtonText;

    private Button selectedSightButton = null;
    

    private Button selectedButtonAttachment = null;
    private Button selectedButtonSight = null;


    
    public void OnClickEquip(Button button)
    {
        if (selectedSightButton.interactable == true)
        {
            button.interactable = true;
            selectedSightButtonText.text = "Equýp";
        }
        button.interactable = false;
        selectedSightButtonText.text = "Equýpped";
    }
    private void SelectSight(Button button)
    {
        // Yeni seçili butonu güncelle
        selectedSightButton = button;
        selectedSightButton.interactable = true;
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
        attachments.anchoredPosition =new Vector2(0,140);

        for(int i = 0; i < attachmentsNametext.Length; i++)
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
        //SightScrollView.SetActive(true);
    }

    public void OnClickSight_1(Button button)
    {
        defaultSight.SetActive(true);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(false);
        equipButton.SetActive(true);
        SelectSight(button);
        
    }
    public void OnClickSight_2(Button button)
    {
        defaultSight.SetActive(false);
        thermalSight.SetActive(true);
        nightStalkertSight.SetActive(false);
        equipButton.SetActive(true);
        SelectSight(button);
    }
    public void OnClickSight_3(Button button)
    {
        defaultSight.SetActive(false);
        thermalSight.SetActive(false);
        nightStalkertSight.SetActive(true);
        equipButton.SetActive(true);
        SelectSight(button);
    }

}
