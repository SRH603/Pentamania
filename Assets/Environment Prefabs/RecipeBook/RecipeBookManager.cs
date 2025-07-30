using UnityEngine;
using System.Collections.Generic;
using System;

public class RecipeBookManager : MonoBehaviour
{
    [SerializeField] private Material[] pageList;
    [SerializeField] private int[] maxPageFromTaskProgress; // determines the maximum page based on how many tasks have been completed.

    private int taskProgress => TaskManager.Instance.taskFinished;
    private int currentPage;
    private Animator _animator;
    private int page1page;
    private int page2page;
    private int flippage;
    public Material page1mat;
    public Material page2mat;
    public Material flip1mat;
    public Material flip2mat;

    void Start()
    {
        currentPage = 0;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Empty"))
        {
            _animator.SetBool("FlipRight",false);
            _animator.SetBool("FlipLeft",false);
            page1page = currentPage;
            page2page = currentPage;
        }
        page1mat = pageList[page1page * 2 - 1];
        page2mat = pageList[page2page * 2];
        flip1mat = pageList[flippage * 2 + 1];
        flip2mat = pageList[flippage * 2];
    }

    public void GoForward()
    {
        Debug.Log("Book: Go forward attempted");
        if (currentPage < maxPageFromTaskProgress[taskProgress])
        {
            flippage = currentPage;
            currentPage++;
            page1page = currentPage;
            _animator.SetBool("FlipRight",true);
            // TRIGGER ANIMATION

            // ADJUST THE PAGES
            Debug.Log("Book: Go forward done");
        }
    }

    public void GoBackward()
    {
        Debug.Log("Book: Go backward attempted");
        if (currentPage > 0)
        {
            currentPage--;
            flippage = currentPage;
            page2page = currentPage;
            _animator.SetBool("FlipLeft",true);

            // TRIGGER ANIMATION

            // ADJUST THE PAGES
            Debug.Log("Book: Go backward done");
        }
    }

    // dunno if you'll need this or not
    // just leaving it here in case
    // delete if you don't use it
    // public Material GetPageMaterial(int pageIndex)
    // {
    //     return pageList[pageIndex];
    // }
}
