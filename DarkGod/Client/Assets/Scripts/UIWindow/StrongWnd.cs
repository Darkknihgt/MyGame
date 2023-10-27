/****************************************************
    文件：StrongWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/29 16:11:50
	功能：强化界面操作与设置
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PEProtocol;

public class StrongWnd : WindowRoot
{
    #region UI Define

    public Text txtStarLv; //强化界面的星级
    public Image imgCurtPos;//图标
    public Transform starTransGrp;//星级transform

    //强化前
    public Text propHP1;
    public Text propHurt1;
    public Text propDef1;
    //强化后
    public Text propHP2;
    public Text propHurt2;
    public Text propDef2;
    //强化需求
    public Text txtNeedLV;
    public Text txtCostCoin;
    public Text txtCostCrystal;
    //箭头
    public Transform propArr1;
    public Transform propArr2;
    public Transform propArr3;
    public Transform costTransRoot;

    public Text txtCoin;

    private StrongConfigs nextSd;

    #endregion

    #region Data Area
    public Transform posBtnTrans;  //获取界面身体部位的图片的transform
    private Image[] images = new Image[6];//用来存储遍历的6个对象图片 
    private int currentIndex;
    private PlayerData pd;  //定义玩家数据
    #endregion

    protected override void InitWnd()
    {
        base.InitWnd();
        pd = GameRoot.Instance.PlayerData;//获取客户端的玩家数据
        RegClickEvts();

        ClickPosItem(0); //初始显示第一个图片
        RefreshStrongUI();
    }

    private void RegClickEvts()  //函数是用来注册点击事件
    {
        for (int i = 0; i < posBtnTrans.childCount; i++)  //遍历装备部位界面的所有子图片对象
        {
            Image img = posBtnTrans.GetChild(i).GetComponent<Image>();  //获取该子对象的image组件

            OnClick(img.gameObject, (object args) =>
            { //想要事件触发时执行的函数
                ClickPosItem((int)args);  //该函数时切换显示图片和右边面板内容
                auSvc.PlayUIMusic(Constants.ClickUI);  //播放按钮音效
            }, i);
            images[i] = img;
        }
    }

    private void ClickPosItem(int index)
    {
        if (currentIndex == index)  //遇到bug，当再次点击该图片时，图片位置会位移，为了解决该bug，重复点击图片时直接跳过。
        {
            return;
        }
        PECommon.Log("Click Item" + index);
        currentIndex = index; //对存储的临时变量进行赋值
        for (int i = 0; i < posBtnTrans.childCount; i++) //遍历图片信息
        {
            Transform currentTrans = images[i].transform;  //获取该图片信息的trans组件
            if (i == currentIndex) //表示获取到点击的图片信息
            {
                SetSprite(images[i], PathDefine.ItemArrorBG);//对图片进行设置
                currentTrans.localPosition = new Vector3(currentTrans.localPosition.x, currentTrans.localPosition.y, 0);
                currentTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(255, 85);
            }
            else
            {
                SetSprite(images[i], PathDefine.ItemPlatBG);//对图片进行设置
                currentTrans.localPosition = new Vector3(115f, currentTrans.localPosition.y, 0);
                currentTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(230, 85);
            }
        }
        RefreshStrongUI();
    }

    private void RefreshStrongUI()  //用来更新当前强化UI界面
    {
        SetText(txtCoin, pd.coin);//更新当前拥有金币
        switch (currentIndex)
        {
            case 0:
                SetSprite(imgCurtPos, PathDefine.ItemHelmet);
                break;
            case 1:
                SetSprite(imgCurtPos, PathDefine.ItemBody);
                break;
            case 2:
                SetSprite(imgCurtPos, PathDefine.ItemPWaist);
                break;
            case 3:
                SetSprite(imgCurtPos, PathDefine.ItemHand);
                break;
            case 4:
                SetSprite(imgCurtPos, PathDefine.ItemLeg);
                break;
            case 5:
                SetSprite(imgCurtPos, PathDefine.ItemFoot);
                break;
        }
        // 更新星级
        SetText(txtStarLv, pd.strongArr[currentIndex] + "星级");
        for(int i = 0;i < starTransGrp.childCount; i++)
        {
            Image starLv = starTransGrp.GetChild(i).GetComponent<Image>();
            if(i < pd.strongArr[currentIndex])
            {
                SetSprite(starLv, PathDefine.SpStar2);
            }
            else
            {
                SetSprite(starLv, PathDefine.SpStar1);
            }
        }

        //更新属性
        int propAHP1 = resSvc.GetStrongPropAddPreVal(currentIndex, pd.strongArr[currentIndex], 1);
        int propAHurt1 = resSvc.GetStrongPropAddPreVal(currentIndex, pd.strongArr[currentIndex], 2);
        int propADef1 = resSvc.GetStrongPropAddPreVal(currentIndex, pd.strongArr[currentIndex], 3);

        int propAHP2 = resSvc.GetStrongPropAddPreVal(currentIndex, pd.strongArr[currentIndex] + 1, 1);
        int propAHurt2 = resSvc.GetStrongPropAddPreVal(currentIndex, pd.strongArr[currentIndex] + 1, 2);
        int propADef2 = resSvc.GetStrongPropAddPreVal(currentIndex, pd.strongArr[currentIndex] +1, 3);

        SetText(propHP1, "生命 +" + propAHP1);
        SetText(propHurt1, "伤害 +" + propAHurt1);
        SetText(propDef1, "防御 +" + propADef1);

        int nextStartLv = pd.strongArr[currentIndex] + 1;
        nextSd = resSvc.GetStrongConfigsByPosAndStarlv(currentIndex, nextStartLv);
        if(nextStartLv < 11)
        {
            SetActive(propHP2);
            SetActive(propHurt2);
            SetActive(propDef2);
            SetActive(costTransRoot);

            SetActive(propArr1);
            SetActive(propArr2);
            SetActive(propArr3);

            SetText(propHP2, "强化后 +" + propAHP2);
            SetText(propHurt2, "            +" + propAHurt2);
            SetText(propDef2, "            +" + propADef2);

            SetText(txtNeedLV, "需要等级 : " + nextSd.minlv);
            SetText(txtCostCoin, "需要消耗 :         " + nextSd.coin);

            SetText(txtCostCrystal, nextSd.crystal + "/" + pd.crystal);
        }
        else
        {
            SetActive(propHP2,false);
            SetActive(propHurt2, false);
            SetActive(propDef2, false);
            SetActive(costTransRoot, false);

            SetActive(propArr1, false);
            SetActive(propArr2, false);
            SetActive(propArr3, false);
        }

    }

    public void ClickCloseWnd() //关闭强化界面
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        SetWndState(false);
    }

    public void ClickStrongBtn() //强化操作
    {
        //播放UI音乐
        auSvc.PlayUIMusic(Constants.ClickUI);

        //在传递数据前先进行自检
        if(pd.strongArr[currentIndex] < 10) //如果当前界面的星级不超过10的话
        {
            if(pd.lv < nextSd.minlv) //当当前等级小于下一个星级所要求的等级时，则无法升级
            {
                GameRoot.AddTips("角色等级不够");
                return;
            }
            if(pd.coin < nextSd.coin)
            {
                GameRoot.AddTips("金币数量不够");
                return;
            }
            if(pd.crystal < nextSd.crystal)
            {
                GameRoot.AddTips("水晶数量不够");
                return;
            }

            netSvc.SendMsg(new GameMsg
            {
                cmd = (int)CMD.ReqStrong,
                reqStrong = new ReqStrong
                {
                    pos = currentIndex,
                }

            });
        }
        else
        {
            GameRoot.AddTips("星级已经升满");
        }
    }

    public void UpdateStrongUI() //强化升级后更新UI
    {
        auSvc.PlayUIMusic(Constants.UIStrong);
        //ClickPosItem(currentIndex);
        RefreshStrongUI();
    }
}