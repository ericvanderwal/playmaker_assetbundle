// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Asset Bundle")]
	[Tooltip("Checks if an AssetBundle contains an object by name.")]

	public class  CheckBundleContainsObject : FsmStateAction
	{

		[RequiredField]
		[Tooltip("Asset name within the bundle to check.")]
		public FsmString AssetName;
		
		[RequiredField]
		[Tooltip("Asset bundle name to check.")]
		[UIHint(UIHint.Variable)]
		public FsmObject AssetBundle;
		
		[ActionSection("Output")]
		
		[Tooltip("Returns true if the asset is found within the bundle.")]
		[UIHint(UIHint.Variable)]
		public FsmBool containsObject;
		
		private AssetBundle _bundle;

		public override void Reset()
		{

			AssetBundle = null;
			containsObject = false;
			AssetName = null;
			
		}

		public override void OnEnter()
		{
			checkBundle();
			Finish();
		}
		
		
		void checkBundle()
		{
			_bundle = (AssetBundle)AssetBundle.Value;
			containsObject.Value = _bundle.Contains(AssetName.Value);
		}
	}
}