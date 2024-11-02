//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
//using System.Diagnostics;

namespace LogTrace
{
	public class HierarchyManager: MonoBehaviour
	{
		private Vector2 scrollPosition  = Vector2.zero;
		public Vector2 pos = new Vector2 (Screen.width/2+10, 30);
		public bool bShow=false;

		void EnumObject(string name,int level)
		{
			if (level > 2) {
				return;
			}

			GameObject rootobj = GameObject.Find (name);
			if (rootobj == null) {
				return;
			}

			GUILayout.BeginHorizontal ("Box");
			GUILayout.Label(name);
			GUILayout.Label(">");
			string scheck=rootobj.activeInHierarchy?"Disable":"Enable";
			bool ret=GUILayout.Button(scheck);
			if (ret)
			{
				rootobj.SetActive(!rootobj.activeInHierarchy);
			}
			GUILayout.EndHorizontal ();

			int n = rootobj.transform.childCount;
			for (int i=0; i<n; i++) {
				Transform childtransform=rootobj.transform.GetChild(i);
				if (childtransform==null){
					continue;
				}
				GameObject childobj=childtransform.gameObject;
				if (childobj==null)
				{
					continue;
				}

				EnumObject(name+"/"+childobj.name,level+1);
			}
		}
		
		void OnGUI()
		{
			if (Debug.isDebugBuild == false) {
				return;
			}

			GUI.color = Color.white;
			
			if (GUI.Button (new Rect (pos.x, pos.y, Screen.width / 2 - 20, 20), "Hierarchy")) {
				bShow=!bShow;
			}
			
			if (bShow)
			{
				scrollPosition = GUI.BeginScrollView (new Rect (pos.x,pos.y+20,Screen.width/2-20,Screen.height-pos.y-40),
				                                      scrollPosition, new Rect (0, 0, 400, 1200));
				
				GUILayout.BeginVertical ("Box");

				EnumObject("Terrains",0);
				EnumObject("Adornings",0);
				EnumObject("BattleGrid",0);
				EnumObject("BattleUnits",0);
				EnumObject("UI Root (2D)",0);
				EnumObject("UI Root",0);
				
				GUILayout.EndVertical ();
				
				// End the scroll view that we began above.
				GUI.EndScrollView ();
			}
			
		}
			

	}
}

