// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sets whether a Game Object's Rigidy Body is affected by Gravity.")]
	public class UseGravity : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		public FsmBool useGravity;

		public override void Reset()
		{
			gameObject = null;
			useGravity = true;
		}

		public override void OnEnter()
		{
			DoUseGravity();
			Finish();
		}

		void DoUseGravity()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;
			if (go.GetComponent<Rigidbody>() == null) return;
			
			go.GetComponent<Rigidbody>().useGravity = useGravity.Value;
		}
	}
}