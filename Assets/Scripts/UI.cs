using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private const string PLAYER_NAME = "player";

    private int _kernelCount;
    private short _kernelsInUse;
    
    private player _playerComponent;

    [SerializeField] private GameObject _kernelBar;
    [SerializeField] private GameObject _kernelUIObject;
    
    [SerializeField] private Sprite _kernelDefault;
    [SerializeField] private Sprite _kernelSelected;
    [SerializeField] private Sprite _kernelDanger;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.Find(PLAYER_NAME);

        _playerComponent = playerObject.GetComponent<player>();

        _kernelCount = _playerComponent.getKernelCount();
        _kernelsInUse = _playerComponent.getKernelsInUse();
        
        Debug.Log(_kernelCount);

        for (int i = 0; i < _kernelCount; i++)
        {
            GameObject uiKernel = Instantiate(_kernelUIObject);
            
            uiKernel.transform.SetParent(_kernelBar.transform, false);
            
            Image kernelImage = uiKernel.GetComponent<Image>();

            kernelImage.sprite = _kernelDefault;
        }
        
        for (int i = _kernelBar.transform.childCount - _kernelsInUse; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelSelected;
        }
    }
    
    void Update()
    {
        bool updatedUI = false;

        int childCount = _kernelBar.transform.childCount;
        
        int kernelCount = _playerComponent.getKernelCount();
        short kernelsInUse = _playerComponent.getKernelsInUse();

        if (kernelCount > _kernelCount)
        {
            int difference = kernelCount - _kernelCount;

            for (int i = 0; i < difference; i++)
            {
                GameObject uiKernel = Instantiate(_kernelUIObject);
            
                uiKernel.transform.SetParent(_kernelBar.transform, false);
            
                Image kernelImage = uiKernel.GetComponent<Image>();

                kernelImage.sprite = _kernelDefault;

                childCount++;
            }
            
            for (int i = 0; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
            }

            updatedUI = true;
            
            _kernelCount = kernelCount;
        }
        else if (kernelCount < _kernelCount)
        {
            int difference = _kernelCount - kernelCount;

            for (int i = _kernelBar.transform.childCount - 1; i >= _kernelBar.transform.childCount - difference; i--)
            {
                Destroy(_kernelBar.transform.GetChild(i).gameObject);

                childCount--;
            }
            
            for (int i = 0; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
            }
            
            updatedUI = true;
            
            _kernelCount = kernelCount;
        }

        if (_kernelCount <= kernelsInUse)
        {
            for (int i = 0; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDanger;
            }
        }
        else if (updatedUI)
        {
            for (int i = childCount - kernelsInUse; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelSelected;
            }
        }
        else if (_kernelsInUse != kernelsInUse)
        {
            for (int i = 0; i < _kernelBar.transform.childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
            }

            for (int i = childCount - kernelsInUse; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelSelected;
            }
            
            _kernelsInUse = kernelsInUse;
        }
    }
}
