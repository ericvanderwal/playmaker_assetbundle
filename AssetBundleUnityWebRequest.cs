#if UNITY_5_3_OR_NEWER
// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Asset Bundle")]
	[Tooltip("Load an Asset Bundle from a Unity Web Request.")]

	public class  AssetBundleUnityWebRequest : FsmStateAction
	{

		[RequiredField]
		[Tooltip("Name of the asset bundle to load. This field is case sensitive.")]
		[Title("Bundle Name")]
		public FsmString myassetBundle;
		
		[RequiredField]
		[Tooltip("The location of the asset bundle to load. Do not include trailing slash after. If using local copy use the folder name. For remote, include the full URL.")]
		[Title("Bundle Location")]
		public FsmString myassetBundleLocation;

		[ActionSection("Events")]
		
		[Tooltip("Event to fire on load success.")]
		public FsmEvent loadSuccess;
		
		[Tooltip("Event to fire on load failure.")]
		public FsmEvent loadFailed;
		
		[ActionSection("Output")]
		
		[RequiredField]
		[Tooltip("Saved asset bundle. Important: Create an playmaker object variable with the object type of UnityEngine/AssetBundle")]
		[Title("Asset Bundle")]
		[UIHint(UIHint.Variable)]
		public FsmObject myLoadedAssetBundle;
		
		[Tooltip("Optionally save the progress of downloading.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;
		
		[ActionSection("Options")]
		
		[Tooltip("Set to true to use locally stored copy in the folder /AssetBundles/ .")]
		public FsmBool useLocalCopy;
				
		[Tooltip("Set to true for optional debug messages in the console. Turn off for builds.")]
		public FsmBool enableDebug;
		
		// private variables
		private AssetBundle _bundle;
		private string uri;
		private UnityWebRequest request;

		public override void Reset()
		{

			useLocalCopy = false;
			enableDebug = false;
			myLoadedAssetBundle = null;
			myassetBundle = new FsmString { UseVariable = false };
			loadFailed = null;
			loadSuccess = null;
			progress = null;
			myassetBundleLocation = new FsmString { UseVariable = false};

		}

		public override void OnEnter()
		{

				StartCoroutine(LoadBundleWeb());
				
		}
		
		
		public IEnumerator LoadBundleWeb()
		
		{
			
			// Use a local copy.
			if(useLocalCopy.Value)
			{
				uri = "file:///" + Application.dataPath + "/" + myassetBundleLocation.Value + "/" + myassetBundle.Value;  
			}
			
			// Use a remote copy
			else
			{
				uri = myassetBundleLocation.Value + "/" + myassetBundle.Value;  				
				
			}
			
			// enable debug, print download location
			if(enableDebug.Value)
			{
				Debug.Log("Download Location is " + uri);
			}	
			
			// Request download
			request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(uri);
			yield return request.Send();
			
			// Check if the request returned an error. If so, print.
			if(request.isNetworkError)
			{
				if(enableDebug.Value)
				{
					Debug.Log("Failed to load AssetBundle. Error: " + request.error);
				}
				
				Fsm.Event(loadFailed);
			}
			
			_bundle = DownloadHandlerAssetBundle.GetContent(request);

			// Download finished but bundle is null, do error.
			if (request.isDone && _bundle == null)
			{
				if(enableDebug.Value)
				{
					Debug.Log("Failed to load AssetBundle.");
				}
				
				Fsm.Event(loadFailed);
			}	
			
			// Download finished but bundle is not null, do success
			if(_bundle != null)
			{
				
				if(enableDebug.Value)
				{
					Debug.Log("AssetBundle load complete.");
				}
				
				myLoadedAssetBundle.Value = _bundle;
				Fsm.Event(loadSuccess);
				
			}
			
		}
		
		public override void OnUpdate()
		{
			// Do progress value
			progress.Value =  request.downloadProgress * 100f;
			if(enableDebug.Value)
			{
				Debug.Log("Bundle Download Progress: " + request.downloadProgress * 100f); 
			}
				
		}
			
	}
		
}
#endif