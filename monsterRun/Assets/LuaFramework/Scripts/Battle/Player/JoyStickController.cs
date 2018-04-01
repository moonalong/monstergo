using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickController : MonoBehaviour {

    bool keyboardPressed = false;
    private float h = 0;
    private float v = 0;

    private float parentHeight;
    private float parentWidth;
    private float parentR;
    private Vector2 center;

    private bool isPress = false;

    UISprite parentSpirite;
    private Camera guiCamera;
    private Camera mainCamera;

    private int DisplayWidth;
    private int DisplayHeight;

    GameObject btnAttack = null;


    public delegate void HitPlayerOrMonster(int identity);
    public HitPlayerOrMonster hit_cb = null;

    static private JoyStickController _instance;
    static public JoyStickController getInstance()
    {
        if (_instance == null)
        {
            _instance = new JoyStickController();
        }
        return _instance;
    }

    void Awake()
    {

        parentSpirite = transform.parent.GetComponent<UISprite>();
        parentWidth = parentSpirite.width;
        parentHeight = parentSpirite.height;
        parentR = (float)Math.Sqrt(parentWidth * parentWidth + parentHeight * parentHeight) / 2.0f;
        parentR = parentR * 0.6f;
        center = transform.position;


        GameObject go = GameObject.FindWithTag("GuiCamera");
        if (go)
        {
            guiCamera = go.GetComponent<Camera>() as Camera;
        }

        UIEventListener.Get(transform.gameObject).onPress = delegate (GameObject o, bool isPressed)
        {
            this.OnPress(isPressed);
        };
    }

    void Start()
    {
        center = transform.parent.localPosition;
        setDisplayWidth();
    }


    // Update is called once per frame
    void Update()
    {
        if (keyboardPressed) return;
        Vector3 pos;
        if (isPress && PointToJoyStickObject(out pos))
        {
            Vector2 touchpos = new Vector2(pos.x, pos.y);
            touchpos -= new Vector2(center.x, center.y);
            float distance = Vector2.Distance(touchpos, Vector2.zero);
            if (distance < parentR)
            {
                transform.localPosition = touchpos;
            }
            else
            {
                transform.localPosition = touchpos.normalized * parentR;
            }

            h = transform.localPosition.x / parentR;
            v = transform.localPosition.y / parentR;

        }
        else
        {
            transform.localPosition = Vector2.zero;
            h = 0;
            v = 0;
        }

    }

    void OnPress(bool isPress)
    {
        this.isPress = isPress;
    }

    void setDisplayWidth()
    {
        var uiRoot = GameObject.FindObjectOfType<UIRoot>();
        if (uiRoot != null)
        {
            float s = (float)uiRoot.activeHeight / Screen.height;
            DisplayHeight = Mathf.CeilToInt(Screen.height * s);
            DisplayWidth = Mathf.CeilToInt(Screen.width * s);
        }
    }

    bool FindJoyStickTouch(out Vector2 pos)
    {

        pos = Vector2.zero;
        float nguiMinX = -DisplayWidth / 2;
        float nguiMinY = -DisplayHeight / 2;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (false)
        {
            return false;
        }
        else
        {
            pos = InputManager.getScreenPositionForNGUI();
            if (pos.x < nguiMinX + 200 && pos.y < nguiMinY + 200)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchSupported)
        {

            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector2 t = Input.GetTouch(i).position;
                //if (PointToUIObject(t) == false)
                if (true)
                {
                    //转换到NGUI坐标系
                    Vector3 pos2 = UICamera.currentCamera.ScreenToWorldPoint(t);
                    Vector3 pos3 = UICamera.currentCamera.transform.InverseTransformPoint(pos2);
                    Debug.Log("JoyStick pos3.x=" + pos3.x + " pos3.y=" + pos3.y);
                    if (pos3.x < nguiMinX + 200 && pos3.y < nguiMinY + 200)
                    {
                        pos = pos3;
                        return true;
                    }

                }
            }

            return false;
		}
        else
        {
            return false;
        }
		
#endif
    }

    bool isTouched()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchSupported && Input.touchCount > 0){
			return true;
		}
		return false;
#endif
    }

    public void CheckShow()
    {
        keyboardPressed = false;
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            keyboardPressed = true;
            v = 0;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            keyboardPressed = true;
            v = 1;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            keyboardPressed = true;
            v = 0;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            keyboardPressed = true;
            v = -1;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            keyboardPressed = true;
            v = 0;
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            keyboardPressed = true;
            h = 0;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            keyboardPressed = true;
            h = -1;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            keyboardPressed = true;
            h = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            keyboardPressed = true;
            h = 1;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            keyboardPressed = true;
            h = 0;
        }
        if (keyboardPressed)
        {
            return;
        }
        //UnityEngine.Debug.LogWarning("JoyStick:h=" + h + ",v=" + v);
        //当遥杆显示出来之后，PointToUIObject返回true
        Vector2 touchpos;
        if (isTouched() && !isPress && FindJoyStickTouch(out touchpos))
        {
            isPress = true;
        }
        else if (isTouched())
        {
            ////检测是否点击人物、NPC、采集点等
            //if (mainCamera == null)
            //{
            //    GameObject go = GameObject.Find("MainCamera");
            //    if (go)
            //    {
            //        mainCamera = go.GetComponent<Camera>() as Camera;
            //    }
            //}
            //if (!PointToUIObject(Input.mousePosition))
            //{
            //    RaycastHit hit;
            //    Ray ray = mainCamera.ScreenPointToRay(InputManager.getScreenPosition());
            //    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("3D_Avatar")))
            //    {
            //        //BattleManager.getInstance().HitOneAvatar(hit.collider);
            //    }
            //    else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("3D_Item")))
            //    {
            //        //BattleManager.getInstance().HitOne3DItem(hit.collider);
            //    }
            //    else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("3D_Back")))
            //    {
            //        //BattleManager.getInstance().Hit3DBack(hit.point);
            //    }
            //}
        }


        if (InputManager.getTouchUp())
        {
            isPress = false;
            //transform.parent.gameObject.SetActive(false);
            Update();
        }
    }

    bool PointToUIObject()
    {
        if (guiCamera == null) return false;
        RaycastHit hit;
        Ray ray = guiCamera.ScreenPointToRay(InputManager.getScreenPosition());
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            return true;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("TopUI")))
            return true;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI")))
            return true;
        return false;
    }
    bool PointToUIObject(Vector3 pos)
    {
        if (guiCamera == null) return false;
        RaycastHit hit;
        Ray ray = guiCamera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            return true;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("TopUI")))
            return true;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI")))
            return true;
        return false;
    }

    bool PointToJoyStickObject(out Vector3 hitpos)
    {
        hitpos = Vector3.zero;
        if (guiCamera == null) return false;
        RaycastHit hit;

        //进入这个函数说明一定Press了，获取的第一个点为遥杆
        Vector3 pos;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        pos = Input.mousePosition;

        Ray ray = guiCamera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI")))
        {
            hitpos = hit.point;
            hitpos = UICamera.currentCamera.transform.InverseTransformPoint(hitpos);
            return true;
        }
        return false;
#elif UNITY_ANDROID || UNITY_IPHONE
        if (Input.touchCount == 0)
        {
            return false;
        }

		if(Input.touchSupported){
            for (int i = 0; i < Input.touchCount; i++)
            {
			    pos = Input.GetTouch(i).position;
                Ray ray = guiCamera.ScreenPointToRay(pos);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("JoyStick")))
                {
                    hitpos = hit.point;
                    hitpos = UICamera.currentCamera.transform.InverseTransformPoint(hitpos);
                    return true;
                }
            }
            return false;
		} else {
            return false;
        }
#endif
    }

    //bool PointToPlayerOrMonster()
    public Vector2 NormFormatHVJoy(out int state)
    {
        return NormFormatHV(out state, h, v);
    }
    //返回限制在规定离散方向的单位向量
    public Vector2 NormFormatHV(out int state, float _h, float _v)
    {
        var h = _h;
        var v = _v;
        if (h > 0.1f || v > 0.1f || h < -0.1f || v < -0.1f)
        {
            double sita = 0;
            if (Math.Abs(h) < 0.0001)
            {
                if (v > 0)
                {
                    sita = Math.PI * 0.5f;
                }
                else
                {
                    sita = Math.PI * 1.5f;
                }
            }
            else
            {
                if (h < 0)
                {
                    sita = Math.Atan(v / (-h));
                    sita = Math.PI - sita;
                }
                else
                {
                    sita = Math.Atan(v / h);
                }

            }
            int degree = (int)(180.0f * sita / Math.PI);
            if (degree < 0)
            {
                degree += 360;
            }
            degree = (degree / 15) * 15;
            state = degree;
            return FormatHVWithState(state);
        }
        else
        {
            state = -1;
            return new Vector2(0, 0);
        }
    }

    public int goRotateToTarget(GameObject go, GameObject target)
    {
        var v2 = new Vector2(target.transform.position.x - go.transform.position.x,
            target.transform.position.z - go.transform.position.z);
        int state = 0;
        var hv = NormFormatHV(out state, v2.x, v2.y);
        go.transform.LookAt(new Vector3(go.transform.position.x + hv.x, go.transform.position.y, go.transform.position.y + hv.y));
        return state;
    }

    public Vector2 FormatHVWithState(int state)
    {
        double sita = state * Math.PI / 180.0f;
        return new Vector2((float)Math.Cos(sita), (float)Math.Sin(sita));
    }

    static public float State2EulerAngles(float state)
    {
        return 90.0f - state;
    }

    static public float EulerAngles2State(float eu)
    {
        float state = 90.0f - eu;
        if (state < 0)
        {
            state += 360.0f;
        }
        else if (state >= 359.99f)
        {
            state -= 360.0f;
        }
        if (Mathf.Abs(state) < 0.001f)
        {
            state = 0.0f;
        }
        if (Mathf.Abs(state - 360) < 0.001f)
        {
            state = 0.0f;
        }

        return state;
    }
}
