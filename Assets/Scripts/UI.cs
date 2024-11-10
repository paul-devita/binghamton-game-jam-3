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

    [SerializeField] private Canvas _canvas;

    [SerializeField] private GameObject _kernelBar;
    [SerializeField] private GameObject _kernelUIObject;
    
    [SerializeField] private Sprite _kernelDefault;
    [SerializeField] private Sprite _kernelSelected;
    [SerializeField] private Sprite _kernelDanger;
    [SerializeField] private Sprite _kernelDamage;
    
    
    
    //Public methods
    public void takeDamage(int newKernelCount)
    {
        if (newKernelCount >= _kernelCount)
        {
            Debug.LogWarning("attempted to take damage with higher count value");
            return;
        }

        int childCount = _kernelBar.transform.childCount;
        
        int difference = _kernelCount - newKernelCount;

        for (int i = _kernelBar.transform.childCount - 1; i >= _kernelBar.transform.childCount - difference; i--)
        {
            Destroy(_kernelBar.transform.GetChild(i).gameObject);

            childCount--;
        }
            
        for (int i = 0; i < childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
        }
        
        _kernelCount = newKernelCount;
        
        StartCoroutine(indicateDamageRoutine());
    }

    public void updateKernelCount(int kernelCount)
    {
        if (kernelCount == _kernelCount) return;
        
        int childCount = _kernelBar.transform.childCount;
        
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
        }
        else
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
        }
        
        _kernelCount = kernelCount;
        
        if (_kernelCount <= _kernelsInUse)
        {
            for (int i = 0; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDanger;
            }
        }
        else 
        {
            for (int i = childCount - _kernelsInUse; i < childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelSelected;
            }
        }
    }
    public void updateKernelUsage(short kernelsInUse)
    {
        if (_kernelCount <= kernelsInUse)
        {
            for (int i = 0; i < _kernelBar.transform.childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDanger;
            }

            return;
        }
        
        for (int i = 0; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
        }

        for (int i = _kernelBar.transform.childCount - kernelsInUse; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelSelected;
        }

        _kernelsInUse = kernelsInUse;
    }

    public Canvas getCanvas()
    {
        return _canvas;
    }
    
    void Start()
    {
        GameObject playerObject = GameObject.Find(PLAYER_NAME);

        _playerComponent = playerObject.GetComponent<player>();

        _kernelCount = _playerComponent.getKernelCount();
        _kernelsInUse = _playerComponent.getKernelsInUse();

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
    
    private IEnumerator indicateDamageRoutine()
    {
        for (int i = 0; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDamage;
        }

        yield return new WaitForSeconds(0.4f);
 
        for (int i = 0; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
        }

        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDamage;
        }

        yield return new WaitForSeconds(0.4f);
        
        for (int i = 0; i < _kernelBar.transform.childCount; i++)
        {
            _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDefault;
        }
        
        if (_kernelCount <= _kernelsInUse)
        {
            for (int i = 0; i < _kernelBar.transform.childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelDanger;
            }
        }
        else
        {
            for (int i = _kernelBar.transform.childCount - _kernelsInUse; i < _kernelBar.transform.childCount; i++)
            {
                _kernelBar.transform.GetChild(i).GetComponent<Image>().sprite = _kernelSelected;
            }
        }
        
    }
}
