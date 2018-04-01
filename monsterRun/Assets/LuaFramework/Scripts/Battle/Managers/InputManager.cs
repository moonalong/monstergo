using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {

    public static Vector2 getScreenPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchSupported && Input.touchCount == 1){
			return Input.GetTouch(0).position;
		}
		return Vector2.zero;
#endif
    }

    public static Vector3 getScreenPositionForNGUI()
    {
        Vector3 mouse_pos = Input.mousePosition;
        //GameObject camera_obj = GameObject.FindGameObjectWithTag("GuiCamera");    
        Vector3 pos = UICamera.currentCamera.ScreenToWorldPoint(mouse_pos);
        Vector3 pos2 = UICamera.currentCamera.transform.InverseTransformPoint(pos);
        return pos2;
    }

    public static Vector2 getMoveOffset()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchSupported && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved){
			return Input.GetTouch(0).deltaPosition;
		}
		return Vector2.zero;
#endif
    }

    public static float getScaleOffset(ref float pre_distance)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Input.mouseScrollDelta.y;
#elif UNITY_ANDROID || UNITY_IPHONE
		float offset = 0.0f;
		if(Input.touchSupported && Input.multiTouchEnabled && Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(0).phase == TouchPhase.Moved){      //限定两个手指才能缩放
			Touch touch_one = Input.GetTouch(0);
			Touch touch_two = Input.GetTouch(1);
			float distance = Vector2.Distance(touch_one.position, touch_two.position);
			float offset_dis = Vector2.Distance(touch_one.deltaPosition, touch_two.deltaPosition);
			if(distance > pre_distance){
				offset = offset_dis;
			}else{
				offset = -offset_dis;
 			}
			pre_distance = distance;
		}
		return offset;
#endif
    }

    public static bool getTouchDown()      //默认是左键点击,对于移动端来说部分左右键，因此就用默认的左键代替一根手指
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchSupported && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began){
			return true;
		}
		return false;
#endif
    }

    public static bool getTouchUp()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchSupported && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended){
			return true;
		}
		return false;
#endif
    }
}
